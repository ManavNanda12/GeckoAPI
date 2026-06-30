using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Data;

namespace GeckoAPI.Repository.payment
{
    public class PaymentRepository : BaseRepository, IPaymentRepository
    {
        #region Constructor
        public PaymentRepository(IOptions<DbConfig> config) : base(config)
        {
        }
        #endregion

        #region Methods
        public async Task<string> CreatePaymentIntent(PaymentIntentRequestModel model)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = model.Amount * 100, // Stripe works in cents
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return paymentIntent.ClientSecret;
        }

        public async Task<string> CreateSubscription(CreateSessionRequest model)
        {
            var options = new SessionCreateOptions
            {
                Mode = "subscription",
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                Price = model.PriceId,
                Quantity = 1,
            }
        },
                // Metadata on the session itself (for checkout.session.* events).
                Metadata = new Dictionary<string, string>
        {
            { "CustomerId", model.CustomerId.ToString() }
        },
                SubscriptionData = new SessionSubscriptionDataOptions
                {
                    // Metadata copied onto the Subscription so customer.subscription.* webhooks
                    // can resolve our CustomerId/PlanId.
                    Metadata = new Dictionary<string, string>
            {
                { "CustomerId", model.CustomerId.ToString() },
                { "PlanId", model.PlanId.ToString() }
            }
                },
                SuccessUrl = $"https://geckocustomerportal.onrender.com/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = "https://geckocustomerportal.onrender.com/cancel",
            };

            // Reuse the existing Stripe customer when we already have one, otherwise let
            // Stripe create a customer (its id is persisted by the webhook). This prevents
            // a brand-new Stripe customer being created on every purchase.
            if (!string.IsNullOrWhiteSpace(model.StripeCustomerId))
            {
                options.Customer = model.StripeCustomerId;
            }

            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            return session.Url;
        }

        public Task<long> SaveWebhookEvent(SaveStripeWebhookEventRequest model)
        {
            var param = new DynamicParameters();
            param.Add("@StripeEventId", model.StripeEventId);
            param.Add("@EventType", model.EventType);
            param.Add("@ApiVersion", model.ApiVersion);
            param.Add("@PaymentIntentId", model.PaymentIntentId);
            param.Add("@CheckoutSessionId", model.CheckoutSessionId);
            param.Add("@ChargeId", model.ChargeId);
            param.Add("@PayloadJson", model.PayloadJson);

            var query = GetPgFunctionQuery(
                StoredProcedures.SaveStripeWebHookEvents,
                false,
                "@StripeEventId,@EventType,@ApiVersion,@PaymentIntentId,@CheckoutSessionId,@ChargeId,@PayloadJson"
            );

            var response = Execute(query, param);

            // Execute() swallows DB errors and reports them via Success. If the insert truly
            // failed (not just a duplicate), surface it so the webhook returns 500 and Stripe retries.
            if (!response.Success)
                throw new Exception($"Failed to save webhook event: {response.Message}");

            // SP uses ON CONFLICT (StripeEventId) DO NOTHING; a duplicate event returns 0.
            return Task.FromResult(response.Data);
        }

        public Task<long> SaveSubscriptionEvent(SaveCustomerSubscriptionRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", model.CustomerId, DbType.Int32);
            param.Add("@SubscriptionStatus", model.SubscriptionStatus);
            param.Add("@StripeSubscriptionId", model.StripeSubscriptionId);
            param.Add("@PlanId", model.PlanId, DbType.Int32);
            param.Add("@StripeCustomerId", model.StripeCustomerId);
            param.Add("@CancelAtPeriodEnd", model.CancelAtPeriodEnd);

            // Period start/end can legitimately be null on some events (e.g. incomplete
            // subscriptions). Guard the cast instead of blindly unboxing a nullable.
            param.Add("@CurrentPeriodEnd", model.CurrentPeriodEnd.HasValue
                ? DateTime.SpecifyKind(model.CurrentPeriodEnd.Value, DateTimeKind.Unspecified)
                : (DateTime?)null);
            param.Add("@CurrentPeriodStart", model.CurrentPeriodStart.HasValue
                ? DateTime.SpecifyKind(model.CurrentPeriodStart.Value, DateTimeKind.Unspecified)
                : (DateTime?)null);

            var query = GetPgFunctionQuery(
                StoredProcedures.SaveCustomerSubscription,
                false,
                "@CustomerId,@PlanId,@StripeSubscriptionId,@SubscriptionStatus,@StripeCustomerId,@CurrentPeriodStart,@CurrentPeriodEnd,@CancelAtPeriodEnd"
            );

            var response = Execute(query, param);

            if (!response.Success)
                throw new Exception($"Failed to save customer subscription: {response.Message}");

            return Task.FromResult(response.Data);
        }


        public async Task<string?> ChangePlanAsync(ChangePlanRequest model)
        {
            // 1️⃣ Decide the action AND fetch all Stripe identifiers server-side.
            //    We never trust PriceId / CurrentStripeSubscriptionId / IsFree from the client:
            //    a tampered request could otherwise mismatch the plan and the price charged.
            var planCheck = await CheckPlan(new PlanCheckRequestModel
            {
                CustomerId = model.CustomerId,
                PlanId = model.PlanId
            });

            if (planCheck == null)
                throw new BusinessRuleException("Unable to validate subscription.");

            // Server-derived values, with a temporary fallback to client input so the flow keeps
            // working until SP_CheckSubscriptionAction is updated to return these columns.
            var requestedPriceId = planCheck.RequestedStripePriceId ?? model.PriceId;
            var currentSubscriptionId = planCheck.CurrentStripeSubscriptionId ?? model.CurrentStripeSubscriptionId;
            var requestedIsFree = planCheck.RequestedPlanIsFree || model.IsFree;

            switch (planCheck.ActionType)
            {
                case "ALLOW_BUY":
                    // Selecting the free plan with nothing active = nothing to charge.
                    if (requestedIsFree)
                        return null;

                    return await CreateSubscription(new CreateSessionRequest
                    {
                        CustomerId = model.CustomerId,
                        PlanId = model.PlanId,
                        PriceId = requestedPriceId,
                        StripeCustomerId = planCheck.StripeCustomerId
                    });

                case "BLOCK_ALREADY_ACTIVE":
                    throw new BusinessRuleException("You already have this plan active.");

                case "ALLOW_UPGRADE":
                    await UpgradeSubscription(currentSubscriptionId, requestedPriceId, model.CustomerId, model.PlanId);
                    return null; // applied immediately, no checkout redirect

                case "ALLOW_DOWNGRADE":
                    await ScheduleDowngrade(currentSubscriptionId, requestedPriceId, requestedIsFree, model.CustomerId, model.PlanId);
                    return null; // takes effect at period end, no checkout redirect

                default:
                    throw new BusinessRuleException("Invalid subscription action.");
            }
        }

        /// <summary>
        /// Upgrade applies immediately and invoices the prorated difference now.
        /// Also updates the PlanId metadata so the webhook records the NEW plan,
        /// and releases any pending downgrade schedule.
        /// </summary>
        public async Task UpgradeSubscription(string subscriptionId, string newPriceId, long customerId, long newPlanId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
                throw new BusinessRuleException("No active subscription found to upgrade.");

            var service = new SubscriptionService();
            var subscription = await service.GetAsync(subscriptionId);

            // If a downgrade was previously scheduled, release it so the immediate upgrade wins.
            if (!string.IsNullOrEmpty(subscription.ScheduleId))
            {
                await new SubscriptionScheduleService().ReleaseAsync(subscription.ScheduleId);
                subscription = await service.GetAsync(subscriptionId);
            }

            await service.UpdateAsync(subscriptionId,
                new SubscriptionUpdateOptions
                {
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Id = subscription.Items.Data[0].Id,
                            Price = newPriceId
                        }
                    },
                    // Invoice the proration immediately rather than deferring it to the next cycle.
                    ProrationBehavior = "always_invoice",
                    // Keep the subscription metadata in sync so the resulting
                    // customer.subscription.updated webhook saves the correct plan.
                    Metadata = new Dictionary<string, string>
                    {
                        { "CustomerId", customerId.ToString() },
                        { "PlanId", newPlanId.ToString() }
                    }
                });
        }

        /// <summary>
        /// Downgrade takes effect at the end of the current billing period.
        ///  - Downgrade to the FREE plan => cancel the paid subscription at period end.
        ///  - Downgrade to a cheaper PAID plan => a subscription schedule that switches
        ///    to the new price when the current period ends (keeps current benefits until then).
        /// </summary>
        public async Task ScheduleDowngrade(string subscriptionId, string newPriceId, bool requestedIsFree, long customerId, long newPlanId)
        {
            if (string.IsNullOrWhiteSpace(subscriptionId))
                throw new BusinessRuleException("No active subscription found to downgrade.");

            var subscriptionService = new SubscriptionService();

            // Downgrade to free = simply stop the paid subscription at period end.
            if (requestedIsFree)
            {
                await subscriptionService.UpdateAsync(subscriptionId,
                    new SubscriptionUpdateOptions { CancelAtPeriodEnd = true });
                return;
            }

            // Downgrade to a cheaper paid plan = schedule the price change for period end.
            var subscription = await subscriptionService.GetAsync(subscriptionId);
            var scheduleService = new SubscriptionScheduleService();

            // Reuse an existing schedule if one is attached, otherwise create one from the sub.
            SubscriptionSchedule schedule = string.IsNullOrEmpty(subscription.ScheduleId)
                ? await scheduleService.CreateAsync(new SubscriptionScheduleCreateOptions
                {
                    FromSubscription = subscriptionId
                })
                : await scheduleService.GetAsync(subscription.ScheduleId);

            // The current (last) phase mirrors the live subscription; keep it untouched and
            // append a second phase that takes over with the cheaper price at period end.
            var currentPhase = schedule.Phases[schedule.Phases.Count - 1];

            await scheduleService.UpdateAsync(schedule.Id, new SubscriptionScheduleUpdateOptions
            {
                EndBehavior = "release", // hand control back to the subscription after the switch
                Phases = new List<SubscriptionSchedulePhaseOptions>
                {
                    new SubscriptionSchedulePhaseOptions
                    {
                        Items = currentPhase.Items.Select(i => new SubscriptionSchedulePhaseItemOptions
                        {
                            Price = i.PriceId,
                            Quantity = i.Quantity
                        }).ToList(),
                        StartDate = currentPhase.StartDate,
                        EndDate = currentPhase.EndDate
                    },
                    new SubscriptionSchedulePhaseOptions
                    {
                        Items = new List<SubscriptionSchedulePhaseItemOptions>
                        {
                            new SubscriptionSchedulePhaseItemOptions { Price = newPriceId, Quantity = 1 }
                        },
                        // Phase metadata is applied to the subscription when the phase activates,
                        // so the webhook fired at period end records the new plan.
                        Metadata = new Dictionary<string, string>
                        {
                            { "CustomerId", customerId.ToString() },
                            { "PlanId", newPlanId.ToString() }
                        }
                    }
                }
            });
        }

        public Task<PlanCheckResponseModel> CheckPlan(PlanCheckRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", model.CustomerId, DbType.Int32);
            param.Add("@RequestedPlanId", model.PlanId, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.CheckSubscriptionAction,
                true,
                "@CustomerId,@RequestedPlanId"
            );

            var response = QueryFirstOrDefault<PlanCheckResponseModel>(query, param);
            return Task.FromResult(response.Data);
        }

        #endregion
    }
}