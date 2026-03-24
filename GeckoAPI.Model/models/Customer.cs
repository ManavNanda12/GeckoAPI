using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public  class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string ContactNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime DeletedAt { get; set; }
        public int DeletedBy { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string CustomerPassword { get; set; }
        public string FCMToken { get; set; }

    }

    public class CustomerJWTModel
    {
        public long CustomerId { get; set; }
        public string JWTToken { get; set; }
        public DateTime JWTCreatedDate { get; set; }
        public DateTime JWTExpiryDate { get; set; }
    }

    public class CustomerLoginModel
    {
        public string CustomerEmail { get; set; }
        public string? Password { get; set; }

    }

    public class CustomerListModel
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string ContactNumber { get; set; }
        public bool IsWelcomeMailSent { get; set; }
        public long TotalRecords { get; set; }
    }

    public class CustomerSaveModel
    {
        public long CustomerId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string CountryCode { get; set; }

        public string ContactNumber { get; set; }

        public long? CreatedBy { get; set; }

        public string? PasswordHash { get; set; }

        public string? PasswordSalt { get; set; }
        public string? GeneratedPassword { get; set; }
    }

    public class CustomerWelcomeEmailRequestModel
    {
        public long CustomerId { get; set; }
        public string EmailTemplate { get; set; }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }

    public class EmailRequestModel
    {
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = true;
        public List<string> CcEmails { get; set; } = new List<string>();
        public List<string> BccEmails { get; set; } = new List<string>();
    }

    public class FCMTokenUpdateRequestModel
    {
        public long CustomerId { get; set; }
        public string FCMToken { get; set; }
    }
    public class InactiveCartUserNotificationResponseModel
    {
        public long CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FCMToken { get; set; }

        public long CartId { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public decimal CartValue { get; set; }
        public int LastAttempt { get; set; }
    }

    public class NotificationSaveRequestModel
    {
        public long CartId { get; set; }
        public long CustomerId { get; set; }
        public string NotificationType { get; set; }
    }


}
