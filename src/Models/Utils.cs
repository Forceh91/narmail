using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace narmail.Models
{
    public static class Utils
    {
        public static DateTime convertUnixTimestampToDateTime(double unixTimestamp)
        {
            // utc starts at 1970
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // add the seconds on and convert it to local time
            dateTime = dateTime.AddSeconds(unixTimestamp).ToLocalTime();

            // send it back
            return dateTime;
        }

        public static ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer) return depObj as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = GetScrollViewer(child);
                if (result != null) return result;
            }
            return null;
        }

        public static void clearNotifications()
        {
            try
            {
                // remove all notifcations this app has sent
                ToastNotificationManager.History.Clear();

                // clear what messages we have notified about
                NarmapiModel.setNotifiedMessageIDs(string.Empty);

                // set the badge count back to 0
                updateAppBadgeCount(0);
            }
            catch
            {
                return;
            }
        }

        private static void updateAppBadgeCount(int count)
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
