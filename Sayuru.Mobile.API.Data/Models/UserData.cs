using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Sayuru.Mobile.API.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Data.Models
{
    public class UserData : MongoEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Language PreferedLanguage { get; set; }
        public long MobileNumber { get; set; }
        public long EmergencyContact { get; set; }
        public List<Zone> Zones { get; set; }
        public string Nic { get; set; }
        public string BirthDate { get; set; }
        public string ProfilePicture { get; set; }
        public string FishermanID { get; set; }
        public District District { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; } = false;
        public bool HasRegisteredIVR { get; set; } = false;
        public string FishermanLicenseStatus { get; set; }
    }

    public class District
    {
        public int Id { get; set; }
        public string LabelEN { get; set; }
        public string LabelSI { get; set; }
        public string LabelTA { get; set; }
    }
}
