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
                // ✅ FIX: Add metadata to BOTH session AND subscription
                Metadata = new Dictionary<string, string>
        {
            { "CustomerId", model.CustomerId.ToString() } // Session metadata
        },
                SubscriptionData = new SessionSubscriptionDataOptions
                {
                    Metadata = new Dictionary<string, string>
            {
                { "CustomerId", model.CustomerId.ToString() } ,// Subscription metadata - THIS IS KEY!
                        {"PlanId", model.PlanId.ToString() }
            }
                },
                SuccessUrl = $"http://localhost:4200/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = "http://localhost:4200/cancel",
            };

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
            return Task.FromResult(response.Data);
        }

        public Task<long> SaveSubscriptionEvent(SaveCustomerSubscriptionRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", model.CustomerId,DbType.Int32);
            param.Add("@SubscriptionStatus", model.SubscriptionStatus);
            param.Add("@StripeSubscriptionId", model.StripeSubscriptionId);
            param.Add("@PlanId", model.PlanId, DbType.Int32);
            param.Add("@CancelAtPeriodEnd", model.CancelAtPeriodEnd);
            param.Add("@CurrentPeriodEnd", model.CurrentPeriodEnd);
            param.Add("@CurrentPeriodStart", model.CurrentPeriodStart);

            var query = GetPgFunctionQuery(
                StoredProcedures.SaveCustomerSubscription,
                false,
                "@CustomerId,@SubscriptionStatus,@StripeSubscriptionId,@PlanId,@CancelAtPeriodEnd,@CurrentPeriodEnd,@CurrentPeriodStart"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }


        public async Task<string?> ChangePlanAsync(ChangePlanRequest model)
        {
            // 1️⃣ Check plan action from DB
            var planCheck = await CheckPlan(new PlanCheckRequestModel
            {
                CustomerId = model.CustomerId,
                PlanId = model.PlanId
            });

            if (planCheck == null)
                throw new Exception("Unable to validate subscription.");

            switch (planCheck.ActionType)
            {
                case "ALLOW_BUY":
                    return await CreateSubscription(new CreateSessionRequest
                    {
                        CustomerId = model.CustomerId,
                        PlanId = model.PlanId,
                        PriceId = model.PriceId
                    });

                case "BLOCK_ALREADY_ACTIVE":
                    throw new Exception("You already have this plan active.");

                case "ALLOW_UPGRADE":
                    return await UpgradeSubscription(model);

                case "ALLOW_DOWNGRADE":
                    await ScheduleDowngrade(model);
                    return null;

                default:
                    throw new Exception("Invalid subscription action.");
            }
        }

        public async Task<string?> UpgradeSubscription(ChangePlanRequest model)
        {
            var service = new SubscriptionService();

            var subscription = await service.GetAsync(model.CurrentStripeSubscriptionId);

            var updated = await service.UpdateAsync(model.CurrentStripeSubscriptionId,
                new SubscriptionUpdateOptions
                {
                    Items = new List<SubscriptionItemOptions>
                    {
                new SubscriptionItemOptions
                {
                    Id = subscription.Items.Data[0].Id,
                    Price = model.PriceId
                }
                    },
                    ProrationBehavior = "create_prorations"
                });

            return null; // no checkout redirect needed
        }

        public async Task ScheduleDowngrade(ChangePlanRequest model)
        {
            var service = new SubscriptionService();

            await service.UpdateAsync(model.CurrentStripeSubscriptionId,
                new SubscriptionUpdateOptions
                {
                    CancelAtPeriodEnd = true
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