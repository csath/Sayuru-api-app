using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Data.Models
{
    public class UserSecurity: MongoEntity
    {
        public long MobileNumber { get; set; }
        public int OTP { get; set; }
        public DateTime OTPExpiration { get; set; } = DateTime.UtcNow.AddMinutes(5);
    }
}
