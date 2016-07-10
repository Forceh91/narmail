using narmail.Models;
using narmail.Mvvm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace narmail.Views
{
    public sealed partial class Inbox : Page
    {
        private InboxViewModel ViewModel = null;
        private bool isInboxScrollingEventAssigned = false;
        private bool isSentScrollingEventAssigned = false;

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

        private void inboxListSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // check we haven't assigned the scrollviewer event already
            if (isInboxScrollingEventAssigned == true)
                return;

            // try and find the scroll viewer for this
            ScrollViewer scrollViewer = Utils.GetScrollViewer((sender as ListView));
            if (scrollViewer == null)
                return;

            // assign the event
            scrollViewer.ViewChanged += inboxListScrolled;

            // tell it not to do it again
            isInboxScrollingEventAssigned = true;
        }

        private void inboxListScrolled(object sender, ScrollViewerViewChangedEventArgs e)
        {
            // check we have a scrollviewer (if not something has badly broken)
            ScrollViewer scrollViewer = (sender as ScrollViewer);
            if (scrollViewer == null)
                return;

            // get the progress
            double scrollProgress = (scrollViewer.VerticalOffset / scrollViewer.ScrollableHeight);

            // load more messages if we've hit the bottom (almost)
            if (scrollProgress >= 0.98)
                ViewModel.fetchMoreInboxMessages();
        }

        private void sentListSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // check we haven't assigned the scrollviewer event already
            if (isSentScrollingEventAssigned == true)
                return;

            // try and find the scroll viewer for this
            ScrollViewer scrollViewer = Utils.GetScrollViewer((sender as ListView));
            if (scrollViewer == null)
                return;

            // assign the event
            scrollViewer.ViewChanged += sentListScrolled;

            // tell it not to do it again
            isSentScrollingEventAssigned = true;
        }

        private void sentListScrolled(object sender, ScrollViewerViewChangedEventArgs e)
        {
            // check we have a scrollviewer (if not something has badly broken)
            ScrollViewer scrollViewer = (sender as ScrollViewer);
            if (scrollViewer == null)
                return;

            // get the progress
            double scrollProgress = (scrollViewer.VerticalOffset / scrollViewer.ScrollableHeight);

            // load more messages if we've hit the bottom (almost)
            if (scrollProgress >= 0.98)
                ViewModel.fetchMoreSentMessages();
        }

        private void gotoComposeMessage(object sender, RoutedEventArgs e)
        {
            // simply just navigate to the compose view
            Frame.Navigate(typeof(Compose));
        }
    }
}
