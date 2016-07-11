using narmapi;
using narmapi.APIResponses;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace narmailbackground
{
    public sealed class NarmailBackground : IBackgroundTask
    {
        private BackgroundTaskDeferral backgroundTaskDeferral;
        private NarmAPI api = new NarmAPI("_LxcSGe6t3b64Q");

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // get the background task
            backgroundTaskDeferral = taskInstance.GetDeferral();

            // load in the API access token etc.
            string accessToken = NarmapiModel.getAccessToken();
            string refreshToken = NarmapiModel.getRefreshToken();
            long expiryTime = NarmapiModel.getAccessTokenExpiryTime();

            // and load it into the api
            api.loadOAuthSettings(accessToken, refreshToken, expiryTime, string.Empty);

            // setup some events
            api.events.eErrorOccured += apiErrorOccured;
            api.events.eRateLimited += apiRateLimited;
            api.events.eAccountInboxFailed += apiInboxFailed;
            api.events.eAccountInboxReceived += apiInboxReceived;

            // ask for the inbox
            api.getAccountInbox();
        }

        private void apiErrorOccured(object sender, Events.ErrorEvent e)
        {
            // complete the background task since something went wrong
            backgroundTaskDeferral.Complete();
        }

        private void apiRateLimited(object sender, Events.RateLimitInformation e)
        {
            // complete the background task since something went wrong
            backgroundTaskDeferral.Complete();
        }

        private void apiInboxFailed(object sender, Events.ErrorEvent e)
        {
            // complete the background task since something went wrong
            backgroundTaskDeferral.Complete();
        }

        private void apiInboxReceived(object sender, AccountInboxResponse e)
        {
            try
            {
                // get our toast notifier
                ToastNotifier toastNotifier = ToastNotificationManager.CreateToastNotifier();
                if (toastNotifier == null)
                    return;

                // list of all the message IDs that are unread
                List<string> unreadMessageIDs = new List<string>();
                List<string> previouslyUnreadMessageIDs = new List<string>(); ;

                // messages that we've notified the user about
                string previouslyNotifiedUnreadMessages = NarmapiModel.getNotifiedMessageIDs();

                // this list is comma seperated so we need to break it up
                if (string.IsNullOrEmpty(previouslyNotifiedUnreadMessages) == false)
                    previouslyUnreadMessageIDs = previouslyNotifiedUnreadMessages.Split(',').ToList();

                // go through the messages
                foreach (Message messageResponse in e.data.messages.Where(message => message.data.unread == true))
                {
                    // have we notified about this message before?
                    if (previouslyNotifiedUnreadMessages.Contains(messageResponse.data.name) == true)
                        continue;

                    // add it to the list
                    unreadMessageIDs.Add(messageResponse.data.name);

                    // notify about it.
                    createToastNotification(toastNotifier, (messageResponse.data.wasComment == true ? "re: " + messageResponse.data.linkTitle : messageResponse.data.subject), messageResponse.data.author, messageResponse.data.body);
                }

                // store the list of notified about messages
                string newlyNotifiedMessages = string.Empty;

                // join the list if there were items in it
                if (unreadMessageIDs.Count > 0)
                    newlyNotifiedMessages = (string.Join(",", previouslyUnreadMessageIDs) + "," + string.Join(",", unreadMessageIDs));
                else
                {
                    if (previouslyUnreadMessageIDs.Count > 0)
                        newlyNotifiedMessages = string.Join(",", previouslyUnreadMessageIDs);
                }

                // store it!
                NarmapiModel.setNotifiedMessageIDs(newlyNotifiedMessages);

                // update the badge count of unread messages
                updateAppBadgeCount(previouslyUnreadMessageIDs.Count + unreadMessageIDs.Count);
            }
            catch
            {
                // complete the background task since we're done with it now
                backgroundTaskDeferral.Complete();
                return;
            }

            // complete the background task since we're done with it now
            backgroundTaskDeferral.Complete();
        }

        private void createToastNotification(ToastNotifier toastNotifier, string messageSubject, string messageAuthor, string messageBody)
        {
            // get the xml for the toast notification
            XmlDocument toastXML = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText04);
            if (toastXML == null)
                return;

            // generate the xml
            toastXML.GetElementsByTagName("text")[0].InnerText = messageAuthor;
            toastXML.GetElementsByTagName("text")[1].InnerText = messageSubject;
            toastXML.GetElementsByTagName("text")[2].InnerText = messageBody;

            ToastNotification toastNotification = new ToastNotification(toastXML);

            // show the notification
            toastNotifier.Show(toastNotification);
        }

        private void updateAppBadgeCount(int count)
        {
            try
            {
                // get the badge updater and template
                BadgeUpdater badgeUpdater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
                XmlDocument badgeXML = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);
                if (badgeXML == null)
                    return;

                // get the element to update
                XmlElement badgeElement = (XmlElement)badgeXML.SelectSingleNode("/badge");
                if (badgeElement == null)
                    return;

                // update it
                badgeElement.SetAttribute("value", count.ToString());

                // send the badge update notification
                BadgeNotification badgeNotification = new BadgeNotification(badgeXML);
                badgeUpdater.Update(badgeNotification);
            }
            catch
            {
                return;
            }
        }
    }
}
