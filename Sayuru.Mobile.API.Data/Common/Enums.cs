using System;
using System.Collections.Generic;
using System.Text;

namespace Sayuru.Mobile.API.Data.Common
{
    public enum Language
    {
        Sinhala = 1,
        English = 2,
        Tamil = 3
    }

    public enum LogTypes
    {
        PROFILE_UPDATE = 0,
        FUEL_SETTINGS_UPDATE = 1,
        START_LIVE_TRACKING = 2,
        END_LIVE_TRACKING = 3,
        SAVE_LOCATION_POINT = 4,
        START_JOURNEY = 5,
        END_JOURNEY = 6,
        SEND_SOS = 7,
        REGISTER_FOR_IVR = 8,
    }

    public enum AlertType
    {
        High = 1,
        Medium = 2,
        Low = 3,
    }
}
