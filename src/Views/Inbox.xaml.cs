using narmail.Mvvm;
using Windows.UI.Xaml.Controls;

namespace narmail.Views
{
    public sealed partial class Inbox : Page
    {
        private InboxViewModel ViewModel = null;
        public Inbox()
        {
            this.InitializeComponent();
            ViewModel = (DataContext as InboxViewModel);
        }

        private void pageUnloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.unloadEvents();
        }
    }
}
