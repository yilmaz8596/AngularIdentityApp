using System;

namespace API.Utils
{
    public class Utils
    {
        public const string UserId = "uid";
        public const string Username = "username";
        public const string Email = "email";
        public const string UsernamePattern = "^[a-zA-Z0-9]+$";
        public const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public const string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
    }
}
