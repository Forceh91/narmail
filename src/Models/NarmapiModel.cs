using narmapi;
using System;
using Windows.Storage;

namespace narmail.Models
{
    public static class NarmapiModel
    {
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer roamingSettings = ApplicationData.Current.RoamingSettings;

        public static NarmAPI api = new NarmAPI("_LxcSGe6t3b64Q");

        public static void initializeAPI()
        {
            // get the stuff the api will need
            string username = getAccountUsername();
            string accessToken = getAccessToken();
            string refreshToken = getRefreshToken();
            long expiryTime = getAccessTokenExpiryTime();

            // and load it into the api
            api.loadOAuthSettings(accessToken, refreshToken, expiryTime, username);

            // setup some events
            api.events.eErrorOccured += apiErrorOccured;
            api.events.eRateLimited += apiRateLimited;
        }

        public static void logout()
        {
            // logging out is simple - just set everything to a blank string
            setAccessToken(string.Empty);
            setRefreshToken(string.Empty);
            setAccountUsername(string.Empty);
            setAccessTokenExpiryTime(0);
        }

        public static string getAccessToken()
        {
            string settingName = "narmail_access_token";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                return (string)roamingSettings.Values[settingName];

            return string.Empty;
        }

        public static void setAccessToken(string accessToken)
        {
            string settingName = "narmail_access_token";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                roamingSettings.Values[settingName] = accessToken;
            else
                roamingSettings.Values.Add(settingName, accessToken);
        }

        public static string getRefreshToken()
        {
            string settingName = "narmail_refresh_token";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                return (string)roamingSettings.Values[settingName];

            return string.Empty;
        }

        public static void setRefreshToken(string refreshToken)
        {
            string settingName = "narmail_refresh_token";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                roamingSettings.Values[settingName] = refreshToken;
            else
                roamingSettings.Values.Add(settingName, refreshToken);
        }

        public static long getAccessTokenExpiryTime()
        {
            string settingName = "narmail_access_token_expires";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                return (long)roamingSettings.Values[settingName];

            return DateTime.MinValue.Ticks;
        }

        public static void setAccessTokenExpiryTime(long expiryTime)
        {
            string settingName = "narmail_access_token_expires";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                roamingSettings.Values[settingName] = expiryTime;
            else
                roamingSettings.Values.Add(settingName, expiryTime);
        }

        public static string getAccountUsername()
        {
            string settingName = "narmail_account_username";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                return (string)roamingSettings.Values[settingName];

            return string.Empty;
        }

        public static void setAccountUsername(string username)
        {
            string settingName = "narmail_account_username";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                roamingSettings.Values[settingName] = username;
            else
                roamingSettings.Values.Add(settingName, username);
        }

        public static void setNotifiedMessageIDs(string messageIDs)
        {
            string settingName = "narmail_background_notified_messages";
            if (roamingSettings.Values.ContainsKey(settingName) == true)
                roamingSettings.Values[settingName] = messageIDs;
            else
                roamingSettings.Values.Add(settingName, messageIDs);
        }

        private static void apiErrorOccured(object sender, Events.ErrorEvent e)
        {
            MessageModel.sendDialogMessage("NarmAPI Error", e.error);
        }

        private static void apiRateLimited(object sender, Events.RateLimitInformation e)
        {
            MessageModel.sendDialogMessage("NarmAPI Error", "Woah, slow down! You're too quick for us, please try again in " + e.secondsRemaining + " seconds.");
        }
    }
}
