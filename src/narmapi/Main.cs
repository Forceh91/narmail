using narmapi.APIRequests;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using static narmapi.Events;

namespace narmapi
{
    public partial class NarmAPI
    {
        private readonly string _baseURI = "https://ssl.reddit.com/";
        private readonly string _baseOAuthURI = "https://oauth.reddit.com/";
        private readonly string _redirectURI = "http://madnight.co.uk";

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

        public NarmAPI(string clientID)
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
                string postResponse = await Utils.postHTTPCodeString(new Uri(stringBuilder.ToString()), _clientID, authCode, _redirectURI);
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
                string postResponse = await Utils.postHTTPCodeString(new Uri(stringBuilder.ToString()), _clientID, _refreshToken, _redirectURI, true);
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
            stringBuilder.Append("&redirect_uri=" + Uri.EscapeUriString(_redirectURI));
            stringBuilder.Append("&duration=permanent");
            stringBuilder.Append("&scope=identity,submit,privatemessages,read");

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
                }

                // request an access token
                requestAccessToken(code);
            }
        }

        public async void getAccountInformation()
        {
            if (await verifyAPI() == false)
                return;

            StringBuilder stringBuilder = new StringBuilder(_baseOAuthURI);
            stringBuilder.AppendFormat("api/v1/me.json");

            // grab the information about the user
            try
            {
                HTTPResponse getResponse = await Utils.getHTTPString(new Uri(stringBuilder.ToString()), _accessToken);
                parseAccountInfoResponse(getResponse.response);

                // update rate limits
                updateRateLimits(getResponse.rateLimitUsed, getResponse.rateLimitRemaining, getResponse.rateLimitReset);
            }
            catch (Exception ex)
            {
                _events.onErrorOccured(ex.Message);
            }
        }

        public async void getAccountInbox(string before = "", string after = "")
        {
            // make sure we're all good to use the api
            if (await verifyAPI() == false)
                return;

            // build the url
            StringBuilder stringBuilder = new StringBuilder(_baseOAuthURI);
            stringBuilder.Append("message/inbox/?raw_json=1");

            // append the before/after if need to
            if (string.IsNullOrEmpty(before) == false)
                stringBuilder.AppendFormat("&before={0}", before);
            else if (string.IsNullOrEmpty(after) == false)
                stringBuilder.AppendFormat("&after={0}", after);

            // grab the information about the inbox
            try
            {
                HTTPResponse getResponse = await Utils.getHTTPString(new Uri(stringBuilder.ToString()), _accessToken);
                parseAccountInboxResponse(getResponse.response);

                // update rate limits
                updateRateLimits(getResponse.rateLimitUsed, getResponse.rateLimitRemaining, getResponse.rateLimitReset);
            }
            catch (Exception ex)
            {
                _events.onErrorOccured(ex.Message);
            }
        }

        public async void getAccountSent(string before = "", string after = "")
        {
            // make sure we're all good to use the api
            if (await verifyAPI() == false)
                return;

            // build the url
            StringBuilder stringBuilder = new StringBuilder(_baseOAuthURI);
            stringBuilder.Append("message/sent/?raw_json=1");

            // append the before/after if need to
            if (string.IsNullOrEmpty(before) == false)
                stringBuilder.AppendFormat("&before={0}", before);
            else if (string.IsNullOrEmpty(after) == false)
                stringBuilder.AppendFormat("&after={0}", after);

            // grab the information about the inbox
            try
            {
                HTTPResponse getResponse = await Utils.getHTTPString(new Uri(stringBuilder.ToString()), _accessToken);
                parseAccountSentResponse(getResponse.response);

                // update rate limits
                updateRateLimits(getResponse.rateLimitUsed, getResponse.rateLimitRemaining, getResponse.rateLimitReset);
            }
            catch (Exception ex)
            {
                _events.onErrorOccured(ex.Message);
            }
        }

        public async void sendMessage(string subject, string destination, string message)
        {
            // make sure we're ok to use the api
            if (await verifyAPI() == false)
                return;

            // build the url
            StringBuilder stringBuilder = new StringBuilder(_baseOAuthURI);
            stringBuilder.AppendFormat("api/compose/");

            // build the form values
            string formValues = string.Format("api_type=json&to={0}&subject={1}&text={2}", destination, subject, message);

            // attempt to send the message
            try
            {
                HTTPResponse postResponse = await Utils.postHTTPString(new Uri(stringBuilder.ToString()), _accessToken, formValues);
                parseSendMessageResponse(postResponse.response);

                // update rate limits
                updateRateLimits(postResponse.rateLimitUsed, postResponse.rateLimitRemaining, postResponse.rateLimitReset);
            }
            catch (Exception ex)
            {
                SendMessageError sendMessageError = new SendMessageError()
                {
                    errorID = "UNKNOWN_ERROR",
                    errorMessage = ex.Message,
                    errorInput = string.Empty
                };

                _events.onSendMessageFailed(sendMessageError);
            }
        }

        public async void sendComment(string messageID, string message)
        {
            // make sure we're ok to use the api
            if (await verifyAPI() == false)
                return;

            // build the url
            StringBuilder stringBuilder = new StringBuilder(_baseOAuthURI);
            stringBuilder.AppendFormat("api/comment/");

            // build the form values
            string formValues = string.Format("api_type=json&thing_id={0}&text={1}", messageID, message);

            // attempt to send the message
            try
            {
                HTTPResponse postResponse = await Utils.postHTTPString(new Uri(stringBuilder.ToString()), _accessToken, formValues);
                parseSendCommentResponse(postResponse.response);

                // update rate limits
                updateRateLimits(postResponse.rateLimitUsed, postResponse.rateLimitRemaining, postResponse.rateLimitReset);
            }
            catch (Exception ex)
            {
                SendMessageError sendMessageError = new SendMessageError()
                {
                    errorID = "UNKNOWN_ERROR",
                    errorMessage = ex.Message,
                    errorInput = string.Empty
                };

                _events.onSendMessageFailed(sendMessageError);
            }
        }

        public async void markMessageAsRead(string messageID)
        {
            // make sure we're ok to use the api
            if (await verifyAPI() == false)
                return;

            // build the url
            StringBuilder stringBuilder = new StringBuilder(_baseOAuthURI);
            stringBuilder.AppendFormat("api/read_message/");

            // build the form values
            string formValues = string.Format("id={0}", messageID);

            // attempt to mark the message as read
            try
            {
                HTTPResponse postResponse = await Utils.postHTTPString(new Uri(stringBuilder.ToString()), _accessToken, formValues);

                // update rate limits
                updateRateLimits(postResponse.rateLimitUsed, postResponse.rateLimitRemaining, postResponse.rateLimitReset);
            }
            catch
            {
                return;
            }
        }

        private void updateRateLimits(int rateUsed, double rateRemaining, int rateReset)
        {
            _rateLimitUsed = rateUsed;
            _rateLimitRemaining = rateRemaining;
            _rateLimitReset = rateReset;
        }

        private async Task checkAccessTokenIsValid()
        {
            // if the token has expired then we'll need to refresh it
            if (DateTime.Now.Ticks <= expiryTicks)
                return;

            // it's out of date so lets refresh
            await refreshAccessToken();
        }

        private bool checkIsRateLimited()
        {
            // there's nothing left and they've used the limit
            if (rateLimitRemaining == 0 && rateLimitUsed > 0)
                return true;

            // all good
            return false;
        }

        private async Task<bool> verifyAPI()
        {
            // check we have an access token
            if (string.IsNullOrEmpty(_accessToken) == true)
                throw new Exception("No access token has been provided.");

            // check the access token is still valid
            await checkAccessTokenIsValid();

            // make sure we're not rate limited
            if (checkIsRateLimited() == true)
            {
                _events.onRateLimited(rateLimitReset);
                return false;
            }

            return true;
        }
    }
}
