using narmail.Models;

namespace narmail.Mvvm
{
    public class MessageViewModel : BindableBase
    {
        private RedditMessageModel _message = null;
        public RedditMessageModel message { get { return _message; } set { SetProperty(ref _message, value); } }

        public void pageLoaded(RedditMessageModel redditMessageModel)
        {
            // load in the message
            message = redditMessageModel;

            // mark it as read
            if (message.isUnread == true)
                message.isUnread = false;
        }
    }
}
