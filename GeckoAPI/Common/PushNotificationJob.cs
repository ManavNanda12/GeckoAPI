using FirebaseAdmin.Messaging;
using GeckoAPI.Model.models;
using GeckoAPI.Service.customer;

namespace GeckoAPI.Common
{
    public class PushNotificationJob
    {
        private readonly ICustomerService _customerService;

        public PushNotificationJob(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task SendAbandonedCartNotifications()
        {
            var users = await _customerService.GetInactiveCartCustomers();

            foreach (var user in users)
            {
                try
                {
                    if (string.IsNullOrEmpty(user.FCMToken))
                        continue;

                    // ✅ Personalize message based on attempt
                    var (title, body) = GetNotificationContent(user.LastAttempt + 1, user.TotalItems, user.CartValue);

                    var message = new Message()
                    {
                        Token = user.FCMToken,
                        Notification = new Notification()
                        {
                            Title = title,
                            Body = body
                        }
                    };

                    await FirebaseMessaging.DefaultInstance.SendAsync(message);

                    var saveReqModel = new NotificationSaveRequestModel
                    {
                        NotificationType = "InactiveCartNotify",
                        CustomerId = user.CustomerId,
                        CartId = user.CartId
                    };

                    await _customerService.SaveNotification(saveReqModel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending notification to {user.CustomerId}: {ex.Message}");
                }
            }
        }

        private (string Title, string Body) GetNotificationContent(int attempt, int totalItems, decimal cartValue)
        {
            return attempt switch
            {
                1 => ("🛒 Your cart is waiting!", $"You left {totalItems} items worth ₹{cartValue} behind."),
                2 => ("⏰ Still thinking?", $"Your cart with ₹{cartValue} worth of items won't last forever!"),
                3 => ("🔥 Last reminder!", $"Complete your purchase of ₹{cartValue} before your cart expires."),
                _ => ("🛒 Your cart is waiting!", $"You have {totalItems} items worth ₹{cartValue}")
            };
        }
    }
}