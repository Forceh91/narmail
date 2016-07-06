﻿using narmapi.APIRequests;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace narmapi
{
    public partial class Main
    {
        private readonly string _baseURI = "https://reddit.com/";
        private readonly string _baseOAuthURI = "https://oauth.reddit.com/";
        private readonly string _redirectURI = "http://madnight.co.uk/";

        private string _username = string.Empty;
        public string username { get { return _username; } set { _username = value; } }

        private string _clientID = string.Empty;
        public string clientID { get { return _clientID; } set { _clientID = value; } }

        private string _accessToken = string.Empty;
        public string accessToken { get { return _accessToken; } set { _accessToken = value; } }

        private string _refreshToken = string.Empty;
        public string refreshToken { get { return _refreshToken; } set { _refreshToken = value; } }

        private int _expiresIn = 0;
        public int expiresIn { get { return _expiresIn; } set { _expiresIn = value; } }

        private long _expiryTicks = 0;
        public long expiryTicks { get { return _expiryTicks; } set { _expiryTicks = value; } }

        private int _rateLimitUsed = 0;
        public int rateLimitUsed { get { return _rateLimitUsed; } set { _rateLimitUsed = value; } }

        private double _rateLimitRemaining = 0;
        public double rateLimitRemaining { get { return _rateLimitRemaining; } set { _rateLimitRemaining = value; } }

        private int _rateLimitReset = 0;
        public int rateLimitReset { get { return _rateLimitReset; } set { _rateLimitReset = value; } }

        private Events _events;
        public Events events { get { return _events; } }

        // reddit needs this bit to confirm some stuff
        private string _stateCode;

        public Main(string clientID)
        {
            _events = new Events();
            _clientID = clientID;
        }

        public void loadOAuthSettings(string accessToken, string refreshToken, long expiryTicks, string username)
        {
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _expiryTicks = expiryTicks;
            _username = username;
        }

        internal async void requestAccessToken(string authCode)
        {
            // the access token info
            AccessToken accessToken = new AccessToken(authCode, _redirectURI);
            StringBuilder stringBuilder = new StringBuilder(_baseURI);
            stringBuilder.AppendFormat("api/v1/access_token");

            try
            {
                // create the request and parse the response
                string postResponse = await Utils.postHTTPCodeString(new Uri(stringBuilder.ToString()), _clientID, "grant_type=authorization_code&code=" + authCode + "&redirect_uri=" + _redirectURI);
                parseAuthToken(postResponse);
            }
            catch (Exception ex)
            {
                _events.onFailedAppAuth(ex.Message);
            }
        }

        internal async Task refreshAccessToken()
        {
            StringBuilder stringBuilder = new StringBuilder(_baseURI);
            stringBuilder.AppendFormat("api/v1/access_token");

            try
            {
                // create the request and parse the response
                string postResponse = await Utils.postHTTPCodeString(new Uri(stringBuilder.ToString()), _clientID, "grant_type=refresh_token&refresh_token=" + _refreshToken);
                parseRefreshedAccess(postResponse);
            }
            catch (Exception ex)
            {
                _events.onFailedAppReauth(ex.Message);
            }
        }

        public void authorizeApp()
        {
            // unique state code to send
            _stateCode = DateTime.Now.Ticks.ToString();

            // create the string ready for OAuth
            StringBuilder stringBuilder = new StringBuilder(_baseURI);
            stringBuilder.Append("api/v1/authorize.compact?");
            stringBuilder.Append("client_id=" + Uri.EscapeDataString(_clientID));
            stringBuilder.Append("&response_type=code");
            stringBuilder.Append("&state=" + _stateCode);
            stringBuilder.Append("&redirect_uri=" + Uri.EscapeDataString(_redirectURI));
            stringBuilder.Append("&duration=permanent");
            stringBuilder.Append("&scope=identity,submit,privatemessage,read");

            // fire the event to get the app to do whatever it needs
            _events.onAppAuthCallback(new Uri(stringBuilder.ToString()), new Uri(_redirectURI));
        }

        public void continueWebAuthentication(WebAuthenticationResult webAuthenticationResult)
        {
            if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                _events.onFailedAppAuth("HTTP Error: " + webAuthenticationResult.ResponseErrorDetail.ToString());
            else if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.UserCancel)
                _events.onFailedAppAuth("Error: " + webAuthenticationResult.ResponseErrorDetail.ToString());
            else
            {
                Uri endURI = new Uri(_redirectURI);

                // get the response data (without the end uri)
                string responseData = webAuthenticationResult.ResponseData.Remove(0, endURI.ToString().Length + 1);
                string code = string.Empty;
                string state = string.Empty;
                string error = string.Empty;

                // split the response to parse some things out
                string[] responseParams = responseData.Split('&');
                string[] responseParamSplit;
                for (int i = 0; i < responseParams.Length; i++)
                {
                    // split the response params
                    responseParamSplit = responseParams[i].Split('=');

                    // split the string again so we know the key and value
                    switch (responseParamSplit[0])
                    {
                        case "code":
                            code = responseParamSplit[1];
                        break;

                        case "state":
                            state = responseParamSplit[1];
                        break;

                        case "error":
                            error = responseParamSplit[1];
                        break;
                    }

                    // check the state code matches what we sent
                    if (state != _stateCode)
                    {
                        _events.onFailedAppAuth("Error: An invalid state code was sent back.");
                        return;
                    }

                    // make sure there wasn't an error
                    if (string.IsNullOrEmpty(error) == false)
                    {
                        _events.onFailedAppAuth("Error: " + error);
                        return;
                    }

                    // request an access token
                    requestAccessToken(code);
                }
            }
        }

        private async Task checkAccessTokenIsValid()
        {
            // if the token has expired then we'll need to refresh it
            if (DateTime.Now.Ticks <= expiryTicks)
                return;

            // it's out of date so lets refresh
            await refreshAccessToken();
        }
    }
}
