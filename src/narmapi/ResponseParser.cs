using narmapi.APIResponses;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using static narmapi.Events;

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

        private void parseSendMessageResponse(string response)
        {
            SendMessageError sendMessageError = new SendMessageError()
            {
                errorID = "UNKNOWN_ERROR",
                errorMessage = "An unknown error occured",
                errorInput = string.Empty
            };

            try
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(response)))
                {
                    // read the memory stream for the data
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SendMessageResponse));
                    SendMessageResponse sendMessageResponse = (serializer.ReadObject(memoryStream) as SendMessageResponse);

                    // check we got the json object
                    if (sendMessageResponse.json == null)
                    {
                        _events.onSendMessageFailed(sendMessageError);
                        return;
                    }

                    // and the errors object
                    if (sendMessageResponse.json.errors == null)
                    {
                        _events.onSendMessageFailed(sendMessageError);
                        return;
                    }

                    // check the error count and success if there are 0 errors
                    if (sendMessageResponse.json.errors.Count == 0)
                        _events.onSendMessageSuccess();
                    else
                    {
                        // grab the first error and tell the user about that
                        sendMessageError.errorID = sendMessageResponse.json.errors[0][0];
                        sendMessageError.errorMessage = sendMessageResponse.json.errors[0][1];
                        sendMessageError.errorInput = sendMessageResponse.json.errors[0][2];

                        // now we need to figure out what went wrong
                        _events.onSendMessageFailed(sendMessageError);
                    }
                }
            }
            catch (Exception e)
            {
                sendMessageError.errorMessage = e.Message;
                _events.onSendMessageFailed(sendMessageError);
            }
        }

        private void parseSendCommentResponse(string response)
        {
            SendMessageError sendMessageError = new SendMessageError()
            {
                errorID = "UNKNOWN_ERROR",
                errorMessage = "An unknown error occured",
                errorInput = string.Empty
            };

            try
            {
                using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(response)))
                {
                    // read the memory stream for the data
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SendCommentResponse));
                    SendCommentResponse sendCommentResponse = (serializer.ReadObject(memoryStream) as SendCommentResponse);

                    // check we got the json object
                    if (sendCommentResponse.json == null)
                    {
                        _events.onSendMessageFailed(sendMessageError);
                        return;
                    }

                    // and the errors object
                    if (sendCommentResponse.json.errors == null)
                    {
                        _events.onSendMessageFailed(sendMessageError);
                        return;
                    }

                    // check the error count and success if there are 0 errors
                    if (sendCommentResponse.json.errors.Count == 0)
                        _events.onSendMessageSuccess();
                    else
                    {
                        // grab the first error and tell the user about that
                        sendMessageError.errorID = sendCommentResponse.json.errors[0][0];
                        sendMessageError.errorMessage = sendCommentResponse.json.errors[0][1];
                        sendMessageError.errorInput = sendCommentResponse.json.errors[0][2];

                        // now we need to figure out what went wrong
                        _events.onSendMessageFailed(sendMessageError);
                    }
                }
            }
            catch (Exception e)
            {
                sendMessageError.errorMessage = e.Message;
                _events.onSendMessageFailed(sendMessageError);
            }
        }
    }
}
