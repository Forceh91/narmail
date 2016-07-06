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

        }

        public void connectRedditAccount()
        {
            // disable the button
            toggleLoginButton(false);

            
        }
    }
}
