using narmapi.APIResponses;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace narmapi
{
    public partial class Main
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
    }
}
