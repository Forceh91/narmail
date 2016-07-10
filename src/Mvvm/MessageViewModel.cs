using narmail.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace narmail.Mvvm
{
    public class MessageViewModel : BindableBase
    {
        private RedditMessageModel _message = null;
        public RedditMessageModel message { get { return _message; } set { SetProperty(ref _message, value); } }

        private Visibility _isCommandBarVisible = Visibility.Visible;
        public Visibility isCommandBarVisible { get { return _isCommandBarVisible; } set { SetProperty(ref _isCommandBarVisible, value); } }

        public void pageLoaded(RedditMessageModel redditMessageModel)
        {
            // load in the message
            message = redditMessageModel;

            // mark it as read
            if (message.isUnread == true)
            {
                NarmapiModel.api.markMessageAsRead(message.name);
                message.isUnread = false;
            }

            // remove the command bar if we sent the message
            if (message.author == NarmapiModel.getAccountUsername())
                isCommandBarVisible = Visibility.Collapsed;
        }

        public void replyToMessage()
        {
            // find the current frame since we'll need that to navigate
            Frame currentFrame = (Window.Current.Content as Frame);
            if (currentFrame == null)
                return;

            // do some tings
            InitialComposeModel initialComposeModel = new InitialComposeModel()
            {
                destination = message.author,
                isReply = true,
                messageID = message.name,
                parentMessage = message.body,
                subject = (message.wasComment == true ? message.linkTitle : message.subject)
            };

            // direct the user to that page
            currentFrame.Navigate(typeof(Views.Compose), initialComposeModel);
        }
    }
}
