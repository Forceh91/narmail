using narmail.Models;
using narmapi;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace narmail.Mvvm
{
    public class InboxViewModel : BindableBase
    {
        private Visibility _inboxNoMessageVisibility = Visibility.Visible;
        public Visibility inboxNoMessageVisibility { get { return _inboxNoMessageVisibility; } set { SetProperty(ref _inboxNoMessageVisibility, value); } }

        private ObservableCollection<RedditMessageModel> _inboxList = new ObservableCollection<RedditMessageModel>();
        public ObservableCollection<RedditMessageModel> inboxList { get { return _inboxList; } set { SetProperty(ref _inboxList, value); } }

        // store the before and after for the next time we request the inbox messages
        private string inboxBefore = string.Empty;
        private string inboxAfter = string.Empty;


        public InboxViewModel()
        {
            // assign events
            NarmapiModel.api.events.eAccountInboxFailed += accountInboxFailed;
            NarmapiModel.api.events.eAccountInboxReceived += accountInboxReceived;

            // fetch the inbox messages
            NarmapiModel.api.getAccountInbox();
        }

        public void unloadEvents()
        {
            NarmapiModel.api.events.eAccountInboxFailed -= accountInboxFailed;
            NarmapiModel.api.events.eAccountInboxReceived -= accountInboxReceived;
        }

        private void accountInboxFailed(object sender, Events.ErrorEvent e)
        {

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
        }
    }
}
