using narmail.Models;
using System;
using Windows.UI.Xaml.Data;

namespace narmail.Convertors
{
    public class MessageTimeReceivedConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double createdUTC = (double)value;
            DateTime createdTime = Utils.convertUnixTimestampToDateTime(createdUTC);
            DateTime yesterday = DateTime.Now.AddHours(-24);

            // this was created more than 24 hours ago so return the date
            if (yesterday > createdTime)
            {
                if (createdTime.Year == yesterday.Year)
                    return createdTime.ToString("d MMM");
                else
                    return createdTime.ToString("d MMM yyyy");
            }
            else
                return createdTime.ToString("HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
