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
        public delegate void eventRateLimited(object sender, RateLimitInformation e);

        public event eventErrorOccured eErrorOccured;
        public event eventAppAuthCallback eAppAuthCallback;
        public event eventAppAuthorized eAppAuthorized;
        public event eventFailedAppAuth eFailedAppAuth;
        public event eventFailedAppReauth eFailledAppReauth;
        public event eventAccountInfoReceived eAccountInfoReceived;
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

        public void onRateLimited(int secondsRemaining)
        {
            eRateLimited?.Invoke(this, new RateLimitInformation() { secondsRemaining = secondsRemaining });
        }
    }
}
