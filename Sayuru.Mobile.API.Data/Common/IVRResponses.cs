using System;
using System.Collections.Generic;

namespace Sayuru.Mobile.API.Data.Common
{
    public class RegisterRes
    {
        public string ResultDesc { get; set; }
        public int ResultCode { get; set; }
        public bool Status { get; set; }
    }

    public class ZoneRes
    {
        public string ResultDesc { get; set; }
        public int ResultCode { get; set; }
        public List<Zone> Zones { get; set; }
    }

    public class PredictionRes
    {
        public string ResultDesc { get; set; }
        public int ResultCode { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public List<Prediction> Predictions { get; set; }
    }

    public class Prediction
    {
        public int ZoneId { get; set; }
        public bool IsEmergency { get; set; }
        public int GeneralWindMin { get; set; }
        public int GeneralWindMax { get; set; }
        public int GustyWind { get; set; }
        public int ThundershowerWind { get; set; }
        public string WeatherCondition { get; set; }
        public string SeaCondition { get; set; }
        public string Advice { get; set; }
        public string Si_emergencyMsg { get; set; }
        public string Ta_emergencyMsg { get; set; }
        public string En_emergencyMsg { get; set; }
    }

    public class Zone
    {
        public string Id { get; set; }
        public string Si_name { get; set; }
        public string En_name { get; set; }
        public string Ta_name { get; set; }
    }

    public class FishermanRes
    {
        public string Msg { get; set; }
        public string Status { get; set; }
        public Fisherman Fishermans_data { get; set; }
    }

    public class Fisherman
    {
        public string FishermanID { get; set; }
        public string LandNumber { get; set; }
        public string Address { get; set; }
        public string Active_status { get; set; }
        public string NameAsId { get; set; }
        public string MobileNumber { get; set; }
        public string Nic { get; set; }
        public string District { get; set; }
        public string OtherName { get; set; }
        public string ApplicantName { get; set; }
    }
}
