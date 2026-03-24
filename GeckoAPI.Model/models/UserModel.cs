using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebAPI.model.Models
{
    public class UserModel
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? PasswordHash { get; set; } 
        public string? PasswordSalt { get; set; } 
        public string? Password { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedTime { get; set; }
    }

    public class UserJWTModel
    {
        public long UserId { get; set; }
        public string JWTToken { get; set; }
        public DateTime JWTCreatedDate { get; set; }
        public DateTime JWTExpiryDate { get; set; }
    }

    public class UserLoginModel
    {
        public string UserEmail { get; set; }
        public string? Password { get; set; }
    }

    public class LoginAttemptSaveModel
    {
        public long UserId { get; set; }
        public DateTime AttemptTime { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class LoginAttemptModel
    {
        public long AttemptId { get; set; }
        public long UserId { get; set; }
        public DateTime AttemptTime { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class ChangeLockStatusModel
    {
        public long UserId { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LockedTime { get; set; }
    }

}
