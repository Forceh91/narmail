﻿using narmail.Models;
using narmapi;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace narmail.Mvvm
{
    public class InboxViewModel : BindableBase
    {
        private bool _isCommandBarButtonEnabled = false;
        public bool isCommandBarButtonEnabled { get { return _isCommandBarButtonEnabled; } set { SetProperty(ref _isCommandBarButtonEnabled, value); } }

        private Visibility _inboxNoMessageVisibility = Visibility.Visible;
        public Visibility inboxNoMessageVisibility { get { return _inboxNoMessageVisibility; } set { SetProperty(ref _inboxNoMessageVisibility, value); } }

        private ObservableCollection<RedditMessageModel> _inboxList = new ObservableCollection<RedditMessageModel>();
        public ObservableCollection<RedditMessageModel> inboxList { get { return _inboxList; } set { SetProperty(ref _inboxList, value); } }

        private Visibility _sentNoMessageVisibility = Visibility.Visible;
        public Visibility sentNoMessageVisibility { get { return _sentNoMessageVisibility; } set { SetProperty(ref _sentNoMessageVisibility, value); } }

        private ObservableCollection<RedditMessageModel> _sentList = new ObservableCollection<RedditMessageModel>();
        public ObservableCollection<RedditMessageModel> sentList { get { return _sentList; } set { SetProperty(ref _sentList, value); } }

        private Visibility _isCommunicatingWithRedditVisibility = Visibility.Visible;
        public Visibility isCommunicatingWithRedditVisibility { get { return _isCommunicatingWithRedditVisibility; } set { SetProperty(ref _isCommunicatingWithRedditVisibility, value); } }

        private string _username = NarmapiModel.getAccountUsername();
        public string username { get { return _username; } }

        // store the before and after for the next time we request the inbox messages
        private string inboxBefore = string.Empty;
        private string inboxAfter = string.Empty;
        private bool areMoreMessagesAvailable = false;

        // store the before and after for the next time we request the sent messages
        private string sentBefore = string.Empty;
        private string sentAfter = string.Empty;
        private bool areMoreSentMessagesAvailable = false;

        // are we loading things?
        bool isInboxLoading = false;
        bool isSentLoading = false;

        public InboxViewModel()
        {
            // assign events
            NarmapiModel.api.events.eAccountInboxFailed += accountInboxFailed;
            NarmapiModel.api.events.eAccountInboxReceived += accountInboxReceived;

            NarmapiModel.api.events.eAccountSentFailed += accountSentFailed;
            NarmapiModel.api.events.eAccountSentReceived += accountSentReceived;

            // fetch the inbox messages
            NarmapiModel.api.getAccountInbox();
            isInboxLoading = true;

            // fetch the sent messages
            NarmapiModel.api.getAccountSent();
            isSentLoading = true;
        }

        public void unloadEvents()
        {
            NarmapiModel.api.events.eAccountInboxFailed -= accountInboxFailed;
            NarmapiModel.api.events.eAccountInboxReceived -= accountInboxReceived;
            NarmapiModel.api.events.eAccountSentFailed -= accountSentFailed;
            NarmapiModel.api.events.eAccountSentReceived -= accountSentReceived;
        }

        public void fetchMoreInboxMessages()
        {
            // make sure we're not loading messages
            if (isInboxLoading == true)
                return;

            // make sure messages are available
            if (areMoreMessagesAvailable == false)
                return;

            // we're loading more messages
            isInboxLoading = true;

            // fetch the messages
            NarmapiModel.api.getAccountInbox(string.Empty, inboxAfter);
        }

        public void fetchMoreSentMessages()
        {
            // make sure we're not loading messages
            if (isSentLoading == true)
                return;

            // make sure messages are available
            if (areMoreSentMessagesAvailable == false)
                return;

            // we're loading more messages
            isSentLoading = true;

            // fetch the messages
            NarmapiModel.api.getAccountSent(string.Empty, sentAfter);
        }

        public void logout()
        {
            // get the current frame
            Frame currentFrame = (Window.Current.Content as Frame);
            if (currentFrame == null)
                return;

            // seems we want to logout
            NarmapiModel.logout();

            // so send them to the connection page (and clear the backstack so they can't navigate backwards)
            currentFrame.Navigate(typeof(Views.Landing));
        }

        private void accountInboxFailed(object sender, Events.ErrorEvent e)
        {
            // not loading
            isInboxLoading = false;

            // progress updated
            retrievingProgressUpdated();
        }

        private void accountInboxReceived(object sender, narmapi.APIResponses.AccountInboxResponse inboxResponse)
        {
            // we have received our inbox, lets store stuff locally
            inboxBefore = inboxResponse.data.before;
            inboxAfter = inboxResponse.data.after;

            // check we got some a message list
            if (inboxResponse.data.messages == null)
            {
                inboxNoMessageVisibility = Visibility.Visible;
                return;
            }

            // and that there are messages in that list
            if (inboxResponse.data.messages.Count == 0)
            {
                inboxNoMessageVisibility = Visibility.Visible;
                return;
            }

            // our list of messages
            foreach (narmapi.APIResponses.Message inboxMessage in inboxResponse.data.messages)
            {
                // add to our list
                inboxList.Add(new RedditMessageModel()
                {
                    author = inboxMessage.data.author,
                    body = inboxMessage.data.body,
                    bodyHTML = inboxMessage.data.bodyHTML,
                    context = inboxMessage.data.context,
                    created = inboxMessage.data.created,
                    createdUTC = inboxMessage.data.createdUTC,
                    dest = inboxMessage.data.dest,
                    distinguished = inboxMessage.data.distinguished,
                    firstMessage = Convert.ToInt64(inboxMessage.data.firstMessage),
                    firstMessageName = inboxMessage.data.firstMessageName,
                    id = inboxMessage.data.id,
                    isUnread = inboxMessage.data.unread,
                    kind = inboxMessage.kind,
                    likes = inboxMessage.data.likes,
                    linkTitle = inboxMessage.data.linkTitle,
                    name = inboxMessage.data.name,
                    parentID = inboxMessage.data.parentID,
                    replies = inboxMessage.data.replies,
                    subject = inboxMessage.data.subject,
                    subreddit = inboxMessage.data.subreddit,
                    wasComment = inboxMessage.data.wasComment
                });
            }

            // hide the no message text
            inboxNoMessageVisibility = Visibility.Collapsed;

            // no longer loading
            isInboxLoading = false;

            // retrieving progress changed
            retrievingProgressUpdated();

            // check if more messages are available (otherwise we'll just infinite load messages!)
            if (string.IsNullOrEmpty(inboxResponse.data.after) == true)
                areMoreMessagesAvailable = false;
            else
                areMoreMessagesAvailable = true;
        }

        private void accountSentFailed(object sender, Events.ErrorEvent e)
        {
            // not loading
            isSentLoading = false;

            // retrieving progress changed
            retrievingProgressUpdated();
        }

        private void accountSentReceived(object sender, narmapi.APIResponses.AccountSentResponse sentResponse)
        {
            // we have received our inbox, lets store stuff locally
            sentBefore = sentResponse.data.before;
            sentAfter = sentResponse.data.after;

            // check we got some a message list
            if (sentResponse.data.messages == null)
            {
                sentNoMessageVisibility = Visibility.Visible;
                return;
            }

            // and that there are messages in that list
            if (sentResponse.data.messages.Count == 0)
            {
                sentNoMessageVisibility = Visibility.Visible;
                return;
            }

            // our list of messages
            foreach (narmapi.APIResponses.Message sentMessage in sentResponse.data.messages)
            {
                // add to our list
                sentList.Add(new RedditMessageModel()
                {
                    author = sentMessage.data.author,
                    body = sentMessage.data.body,
                    bodyHTML = sentMessage.data.bodyHTML,
                    context = sentMessage.data.context,
                    created = sentMessage.data.created,
                    createdUTC = sentMessage.data.createdUTC,
                    dest = sentMessage.data.dest,
                    distinguished = sentMessage.data.distinguished,
                    firstMessage = Convert.ToInt64(sentMessage.data.firstMessage),
                    firstMessageName = sentMessage.data.firstMessageName,
                    id = sentMessage.data.id,
                    isUnread = sentMessage.data.unread,
                    kind = sentMessage.kind,
                    likes = sentMessage.data.likes,
                    linkTitle = sentMessage.data.linkTitle,
                    name = sentMessage.data.name,
                    parentID = sentMessage.data.parentID,
                    replies = sentMessage.data.replies,
                    subject = sentMessage.data.subject,
                    subreddit = sentMessage.data.subreddit,
                    wasComment = sentMessage.data.wasComment
                });
            }

            // hide the no message text
            sentNoMessageVisibility = Visibility.Collapsed;

            // no longer loading
            isSentLoading = false;

            // retrieving progress changed
            retrievingProgressUpdated();

            // check if more messages are available (otherwise we'll just infinite load messages!)
            if (string.IsNullOrEmpty(sentResponse.data.after) == true)
                areMoreSentMessagesAvailable = false;
            else
                areMoreSentMessagesAvailable = true;
        }

        private void retrievingProgressUpdated()
        {
            // still loading, don't do anything
            if (isInboxLoading == true || isSentLoading == true)
                return;

            // enable the command bar buttons
            isCommandBarButtonEnabled = true;

            // hide the communicating... text
            isCommunicatingWithRedditVisibility = Visibility.Collapsed;
        }
    }
}
