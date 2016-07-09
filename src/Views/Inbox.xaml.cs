using narmail.Models;
using narmail.Mvvm;
using Windows.UI.Xaml;
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

        private void pageUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.unloadEvents();
        }

        private void inboxMessageClicked(object sender, ItemClickEventArgs e)
        {
            // get the message model from the clicked item
            RedditMessageModel redditMessageModel = (e.ClickedItem as RedditMessageModel);
            if (redditMessageModel == null)
                return;

            // open it up!
            this.Frame.Navigate(typeof(Message), redditMessageModel);
        }

        private void sentMessageClicked(object sender, ItemClickEventArgs e)
        {
            // get the message model from the clicked item
            RedditMessageModel redditMessageModel = (e.ClickedItem as RedditMessageModel);
            if (redditMessageModel == null)
                return;

            // open it up!
            this.Frame.Navigate(typeof(Message), redditMessageModel);
        }
    }
}
