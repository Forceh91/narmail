using narmail.Models;
using narmapi;
using System;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;

namespace narmail.Mvvm
{
    public class LandingViewModel : BindableBase
    {
        private bool _loginButtonEnabled = true;
        public bool loginButtonEnabled { get { return _loginButtonEnabled; } set { SetProperty(ref _loginButtonEnabled, value); } }

        private Visibility _loggingInProgressRing = Visibility.Collapsed;

        public Visibility loggingInProgressRing { get { return _loggingInProgressRing; } set { SetProperty(ref _loggingInProgressRing, value); } }

        private void toggleLoginButton(bool state = true)
        {
            loginButtonEnabled = state;
            loggingInProgressRing = (state == false ? Visibility.Visible : Visibility.Collapsed);
        }

        public LandingViewModel()
        {
            App.currentApp.narmailAPI.events.eAppAuthCallback += appAuthCallback;
            App.currentApp.narmailAPI.events.eFailedAppAuth += appFailedAuthCallback;
        }

        private async void appAuthCallback(object sender, Events.AuthCallbackEvent e)
        {
            // grab the authentication result
            WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, e.startURI, e.endURI);
            if (webAuthenticationResult == null)
                return;

            // continue onwards
            App.currentApp.narmailAPI.continueWebAuthentication(webAuthenticationResult);
        }

        private void appFailedAuthCallback(object sender, Events.FailedAppAuth e)
        {
            // something went wrong :(
            toggleLoginButton();

            // show a message
            MessageModel.sendDialogMessage("Authentication error", "There was an error authenticating you with reddit (" + e.error + ")");
        }

        public void connectRedditAccount()
        {
            // disable the button
            toggleLoginButton(false);

            // fire up the api
            App.currentApp.narmailAPI.authorizeApp();
        }
    }
}
