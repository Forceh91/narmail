using narmapi;
using narmapi.APIResponses;
using System;
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
            api.getAccountInboxUnread();
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

                // get the current timestamp
                double currentUTCUnixTimestamp = getCurrentUnixTimestamp();
                double lastCheckedForMessages = NarmapiModel.getLastCheckedForMessages();

                // go through the messages that have appeared since we last notified
                foreach (Message messageResponse in e.data.messages.Where(message => message.data.createdUTC >= lastCheckedForMessages))
                {
                    // notify about it.
                    createToastNotification(toastNotifier, (messageResponse.data.wasComment == true ? "re: " + messageResponse.data.linkTitle : messageResponse.data.subject), messageResponse.data.author, messageResponse.data.body);
                }

                // store it!
                NarmapiModel.setLastCheckedForMessages(currentUTCUnixTimestamp);

                // update the badge count of unread messages
                updateAppBadgeCount(e.data.messages.Count);
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

        private double getCurrentUnixTimestamp()
        {
            // when the unix time started and what the current time is
            DateTime unixTimestampStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime currentTime = DateTime.UtcNow;

            // return the number of seconds that have passed since 1970
            return ((currentTime - unixTimestampStart).TotalSeconds);
        }
    }
}
