using narmail.Models;
using narmapi;
using System;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace narmail.Mvvm
{
    public class LandingViewModel : BindableBase
    {
        private bool _loginButtonEnabled = true;
        public bool loginButtonEnabled { get { return _loginButtonEnabled; } set { SetProperty(ref _loginButtonEnabled, value); } }

        private string _loggingInText = string.Empty;
        public string loggingInText { get { return _loggingInText; } set { SetProperty(ref _loggingInText, value); } }

        private Visibility _loggingInProgressRing = Visibility.Collapsed;
        public Visibility loggingInProgressRing { get { return _loggingInProgressRing; } set { SetProperty(ref _loggingInProgressRing, value); } }

        private Visibility _loggingInTextVisibility = Visibility.Collapsed;
        public Visibility loggingInTextVisibility { get { return _loggingInTextVisibility; } set { SetProperty(ref _loggingInTextVisibility, value); } }

        private void toggleLoginButton(bool state = true)
        {
            loginButtonEnabled = state;
            loggingInProgressRing = (state == false ? Visibility.Visible : Visibility.Collapsed);
            loggingInTextVisibility = (state == false ? Visibility.Visible : Visibility.Collapsed);
        }

        public LandingViewModel()
        {
            NarmapiModel.api.events.eAppAuthCallback += appAuthCallback;
            NarmapiModel.api.events.eFailedAppAuth += appFailedAuthCallback;
            NarmapiModel.api.events.eAppAuthorized += appAuthorized;
            NarmapiModel.api.events.eAccountInfoReceived += appAccountInfoReceived;
        }

        public void connectRedditAccount()
        {
            // disable the button
            toggleLoginButton(false);

            // update the logging in text
            loggingInText = "Communicating with reddit...";

            // fire up the api
            NarmapiModel.api.authorizeApp();
        }

        private async void appAuthCallback(object sender, Events.AuthCallbackEvent e)
        {
            // grab the authentication result
            WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, e.startURI, e.endURI);
            if (webAuthenticationResult == null)
                return;

            // continue onwards
            NarmapiModel.api.continueWebAuthentication(webAuthenticationResult);
        }

        private void appAuthorized(object sender, Events.AppAuthorized e)
        {
            // store settings
            NarmapiModel.setAccessToken(e.tokenResponse.accessToken);
            NarmapiModel.setRefreshToken(e.tokenResponse.refreshToken);
            NarmapiModel.setAccessTokenExpiryTime(DateTime.Now.AddSeconds(e.tokenResponse.expiresIn).Ticks);

            // update the logging in text
            loggingInText = "Fetching your account information...";

            // request user info
            NarmapiModel.api.getAccountInformation();
        }

        private void appFailedAuthCallback(object sender, Events.FailedAppAuth e)
        {
            // something went wrong :(
            toggleLoginButton();

            // show a message
            MessageModel.sendDialogMessage("Authentication error", "There was an error authenticating you with reddit (" + e.error + ")");
        }

        private void appAccountInfoReceived(object sender, Events.AccountInformation e)
        {
            // set the username
            NarmapiModel.setAccountUsername(e.account.name);

            // re-enable the button
            toggleLoginButton();

            // send them to the inbox
            sendUserToInbox();

            // show a success message!
            MessageModel.sendDialogMessage("Authorization success", "You have successfully authenticated your reddit account with Narmail!");
        }

        private void sendUserToInbox()
        {
            // force the user to the inbox (without allowing them to go back)
            Frame rootFrame = new Frame();
            Window.Current.Content = rootFrame;
            rootFrame.Navigate(typeof(Views.Inbox));
        }
    }
}
