
using System;
using System.Collections.Generic;
using System.Text;
using Sayuru.Mobile.API.Common;

namespace Sayuru.Mobile.API.Models
{
    public class UserActivityLog : MongoEntity
    {
        public string UserId { get; set; }
        public long CreatedTimeStamp { get; set; }
        public LogTypes LogType { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
