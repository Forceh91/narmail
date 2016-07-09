using narmapi.APIResponses;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace narmapi
{
    public partial class NarmAPI
    {
        private void parseAuthToken(string response)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(response)))
                {
                    // read the memory stream for the data
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AuthorizedAccessToken));
                    AuthorizedAccessToken authorizedAccessToken = (serializer.ReadObject(memoryStream) as AuthorizedAccessToken);

                    // check the object was properly made
                    if (authorizedAccessToken == null)
                        return;

                    // assign stuff to the class
                    accessToken = authorizedAccessToken.accessToken;
                    refreshToken = authorizedAccessToken.refreshToken;
                    expiryTicks = DateTime.Now.AddSeconds(authorizedAccessToken.expiresIn).Ticks;

                    // fire the app authorized event!
                    _events.onAppAuthorized(authorizedAccessToken);
                }
            }
            catch (Exception e)
            {
                _events.onErrorOccured(e.Message);
            }
        }

        private void parseRefreshedAccess(string response)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(response)))
                {
                    // read the memory stream for the data
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AuthorizedRefreshToken));
                    AuthorizedRefreshToken authorizedRefreshToken = (serializer.ReadObject(memoryStream) as AuthorizedRefreshToken);

                    //get the access token
                    accessToken = authorizedRefreshToken.accessToken;
                    expiryTicks = DateTime.Now.AddSeconds(authorizedRefreshToken.expiresIn).Ticks;
                }
            }
            catch (Exception e)
            {
                _events.onErrorOccured(e.Message);
            }
        }

        private void parseAccountInfoResponse(string response)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(response)))
                {
                    // read the memory stream for the data
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AccountInformationResponse));
                    AccountInformationResponse accountInformation = (serializer.ReadObject(memoryStream) as AccountInformationResponse);

                    // get the user's name
                    username = accountInformation.name;

                    // call the account info received event
                    _events.onAccountInfoReceived(accountInformation);
                }
            }
            catch (Exception e)
            {
                _events.onFailedAppAuth(e.Message);
            }
        }

        private void parseAccountInboxResponse(string response)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(response)))
                {
                    // read the memory stream for the data
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AccountInboxResponse));
                    AccountInboxResponse accountInbox = (serializer.ReadObject(memoryStream) as AccountInboxResponse);

                    // call the account inbox received event
                    _events.onAccountInboxReceived(accountInbox);
                }
            }
            catch (Exception e)
            {
                _events.onAccountInboxFailed(e.Message);
            }
        }

        private void parseAccountSentResponse(string response)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(response)))
                {
                    // read the memory stream for the data
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AccountSentResponse));
                    AccountSentResponse accountSent = (serializer.ReadObject(memoryStream) as AccountSentResponse);

                    // call the account inbox received event
                    _events.onAccountSentReceived(accountSent);
                }
            }
            catch (Exception e)
            {
                _events.onAccountSentFailed(e.Message);
            }
        }
    }
}
