using System;
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
    }
}
