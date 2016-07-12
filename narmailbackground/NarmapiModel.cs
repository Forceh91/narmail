using System;
using Windows.Storage;

namespace narmailbackground
{
    public sealed class NarmapiModel
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;

        public static string getAccessToken()
        {
            string settingName = "narmail_access_token";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                return (string)roamingSettings.Values[settingName];

            return string.Empty;
        }

        public static string getRefreshToken()
        {
            string settingName = "narmail_refresh_token";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                return (string)roamingSettings.Values[settingName];

            return string.Empty;
        }

        public static long getAccessTokenExpiryTime()
        {
            string settingName = "narmail_access_token_expires";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                return (long)roamingSettings.Values[settingName];

            return DateTime.MinValue.Ticks;
        }

        public static string getNotifiedMessageIDs()
        {
            string settingName = "narmail_background_notified_messages";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                return (string)roamingSettings.Values[settingName];

            return string.Empty;
        }

        public static void setNotifiedMessageIDs(string messageIDs)
        {
            string settingName = "narmail_background_notified_messages";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                roamingSettings.Values[settingName] = messageIDs;
            else
                roamingSettings.Values.Add(settingName, messageIDs);
        }

        public static void setLastCheckedForMessages(double unixTimestamp)
        {
            string settingName = "narmail_background_last_checked";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                roamingSettings.Values[settingName] = unixTimestamp;
            else
                roamingSettings.Values.Add(settingName, unixTimestamp);
        }

        public static double getLastCheckedForMessages()
        {
            string settingName = "narmail_background_last_checked";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                return (double)roamingSettings.Values[settingName];

            return 0;
        }
    }
}
