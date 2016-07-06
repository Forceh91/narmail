using narmail.Mvvm;
using Windows.UI.Xaml.Controls;

namespace narmail.Views
{
    public sealed partial class Landing : Page
    {
        private LandingViewModel ViewModel = null;
        public Landing()
        {
            this.InitializeComponent();
            ViewModel = (DataContext as LandingViewModel);
        }

        private void connectRedditAccount(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.connectRedditAccount();
        }
    }
}
