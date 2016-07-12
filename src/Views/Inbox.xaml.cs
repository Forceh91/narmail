﻿using narmail.Models;
using narmail.Mvvm;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
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
            NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
            ViewModel = (DataContext as InboxViewModel);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // assign events
            ViewModel.loadEvents();

            // base stuff!
            base.OnNavigatedTo(e);
        }

        private void pageUnloaded(object sender, RoutedEventArgs e)
        {
            // unload the events
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

        private void logoutUser(object sender, RoutedEventArgs e)
        {
            ViewModel.logout();
        }

        private void friendClicked(object sender, ItemClickEventArgs e)
        {
            // get the message model from the clicked item
            RedditFriendsModel redditFriendModel = (e.ClickedItem as RedditFriendsModel);
            if (redditFriendModel == null)
                return;

            ViewModel.messageFriend(redditFriendModel);
        }

        private void feedbackPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Feedback));
        }

        private async void rateReview(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=57919e11-d572-4c16-8e28-1191c51531b3"));
        }
    }
}
