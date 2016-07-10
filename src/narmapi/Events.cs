using narmapi.APIResponses;
using System;

namespace narmapi
{
    public class Events
    {
        public class ErrorEvent : EventArgs
        {
            public string error { get; set; }
        }

        public class AuthCallbackEvent : EventArgs
        {
            public Uri startURI { get; set; }
            public Uri endURI { get; set; }
        }

        public class AppAuthorized : EventArgs
        {
            public AuthorizedAccessToken tokenResponse { get; set; }
        }

        public class FailedAppAuth : EventArgs
        {
            public string error { get; set; }
        }

        public class FailedAppReauth : EventArgs
        {
            public string error { get; set; }
        }

        public class AccountInformation : EventArgs
        {
            public AccountInformationResponse account { get; set; }
        }

        public class SendMessageError : EventArgs
        {
            public string errorID { get; set; }
            public string errorMessage { get; set; }
            public string errorInput { get; set; }
        }

        public class RateLimitInformation : EventArgs
        {
            public int secondsRemaining { get; set; }
        }

        public delegate void eventErrorOccured(object sender, ErrorEvent e);
        public delegate void eventAppAuthCallback(object sender, AuthCallbackEvent e);
        public delegate void eventAppAuthorized(object sender, AppAuthorized e);
        public delegate void eventFailedAppAuth(object sender, FailedAppAuth e);
        public delegate void eventFailedAppReauth(object sender, FailedAppReauth e);
        public delegate void eventAccountInfoReceived(object sender, AccountInformation e);
        public delegate void eventAccountInboxReceived(object sender, AccountInboxResponse e);
        public delegate void eventAccountInboxFailed(object sender, ErrorEvent e);
        public delegate void eventAccountSentReceived(object sender, AccountSentResponse e);
        public delegate void eventAccountSentFailed(object sender, ErrorEvent e);
        public delegate void eventSendMessageSuccess(object sender);
        public delegate void eventSendMessageFailed(object sender, SendMessageError e);
        public delegate void eventRateLimited(object sender, RateLimitInformation e);

        public event eventErrorOccured eErrorOccured;
        public event eventAppAuthCallback eAppAuthCallback;
        public event eventAppAuthorized eAppAuthorized;
        public event eventFailedAppAuth eFailedAppAuth;
        public event eventFailedAppReauth eFailledAppReauth;
        public event eventAccountInfoReceived eAccountInfoReceived;
        public event eventAccountInboxReceived eAccountInboxReceived;
        public event eventAccountInboxFailed eAccountInboxFailed;
        public event eventAccountSentReceived eAccountSentReceived;
        public event eventAccountSentFailed eAccountSentFailed;
        public event eventSendMessageSuccess eSendMessageSuccess;
        public event eventSendMessageFailed eSendMessageFailed;
        public event eventRateLimited eRateLimited;

        public void onErrorOccured(string error)
        {
            eErrorOccured?.Invoke(this, new ErrorEvent() { error = error });
        }

        public void onAppAuthCallback(Uri startURI, Uri endURI)
        {
            eAppAuthCallback?.Invoke(this, new AuthCallbackEvent() { startURI = startURI, endURI = endURI });
        }

        public void onAppAuthorized(AuthorizedAccessToken token)
        {
            eAppAuthorized?.Invoke(this, new AppAuthorized() { tokenResponse = token });
        }

        public void onFailedAppAuth(string error)
        {
            eFailedAppAuth?.Invoke(this, new FailedAppAuth() { error = error });
        }

        public void onFailedAppReauth(string error)
        {
            eFailledAppReauth?.Invoke(this, new FailedAppReauth() { error = error });
        }

        public void onAccountInfoReceived(AccountInformationResponse accountInfo)
        {
            eAccountInfoReceived?.Invoke(this, new AccountInformation() { account = accountInfo });
        }

        public void onAccountInboxReceived(AccountInboxResponse inboxResponse)
        {
            eAccountInboxReceived?.Invoke(this, inboxResponse);
        }

        public void onAccountInboxFailed(string error)
        {
            eAccountInboxFailed?.Invoke(this, new ErrorEvent() { error = error });
        }

        public void onAccountSentReceived(AccountSentResponse sentResponse)
        {
            eAccountSentReceived?.Invoke(this, sentResponse);
        }

        public void onAccountSentFailed(string error)
        {
            eAccountSentFailed?.Invoke(this, new ErrorEvent() { error = error });
        }

        public void onSendMessageSuccess()
        {
            eSendMessageSuccess?.Invoke(this);
        }

        public void onSendMessageFailed(SendMessageError e)
        {
            eSendMessageFailed?.Invoke(this, e);
        }

        public void onRateLimited(int secondsRemaining)
        {
            eRateLimited?.Invoke(this, new RateLimitInformation() { secondsRemaining = secondsRemaining });
        }
    }
}
