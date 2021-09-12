using Sayuru.Mobile.API.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Data.Models
{
    public class UserActivityLog : MongoEntity
    {
        public string UserId { get; set; }
        public long CreatedTimeStamp { get; set; }
        public LogTypes LogType { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
