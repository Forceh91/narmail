using narmail.Models;
using System;
using Windows.UI.Xaml.Data;

namespace narmail.Convertors
{
    public class MessageToFromUserConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // get the message model
            RedditMessageModel message = (RedditMessageModel)value;
            if (message == null)
                return string.Empty;

            // if it's a sent message then obviously we need to show the destination rather than the author
            if (message.author == NarmapiModel.getAccountUsername())
                return message.dest;
            else
                return message.author;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
