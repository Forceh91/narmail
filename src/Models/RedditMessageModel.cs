using narmail.Mvvm;

namespace narmail.Models
{
    public class RedditMessageModel : BindableBase
    {
        private string _kind = string.Empty;
        public string kind { get { return _kind; } set { SetProperty(ref _kind, value); } }

        private string _body = string.Empty;
        public string body { get { return _body; } set { SetProperty(ref _body, value); } }

        private string _bodyHTML = string.Empty;
        public string bodyHTML { get { return _bodyHTML; } set { SetProperty(ref _bodyHTML, value); } }

        private bool _wasComment = false;
        public bool wasComment { get { return _wasComment; } set { SetProperty(ref _wasComment, value); } }

        private long _firstMessage = 0;
        public long firstMessage { get { return _firstMessage; } set { SetProperty(ref _firstMessage, value); } }

        private string _name = string.Empty;
        public string name { get { return _name; } set { SetProperty(ref _name, value); } }

        private string _firstMessageName = string.Empty;
        public string firstMessageName { get { return _firstMessageName; } set { SetProperty(ref _firstMessageName, value); } }

        private double _created = 0;
        public double created { get { return _created; } set { SetProperty(ref _created, value); } }

        private double _createdUTC = 0;
        public double createdUTC { get { return _createdUTC; } set { SetProperty(ref _createdUTC, value); } }

        private string _dest = string.Empty;
        public string dest { get { return _dest; } set { SetProperty(ref _dest, value); } }

        private string _author = string.Empty;
        public string author { get { return _author; } set { SetProperty(ref _author, value); } }

        private string _subreddit = string.Empty;
        public string subreddit { get { return _subreddit; } set { SetProperty(ref _subreddit, value); } }

        private string _id = string.Empty;
        public string id { get { return _id; } set { SetProperty(ref _id, value); } }

        private string _parentID = string.Empty;
        public string parentID { get { return _parentID; } set { SetProperty(ref _parentID, value); } }

        private string _likes = string.Empty;
        public string likes { get { return _likes; } set { SetProperty(ref _likes, value); } }

        private string _context = string.Empty;
        public string context { get { return _context; } set { SetProperty(ref _context, value); } }

        private string _replies = string.Empty;
        public string replies { get { return _replies; } set { SetProperty(ref _replies, value); } }

        private bool _isUnread = false;
        public bool isUnread { get { return _isUnread; } set { SetProperty(ref _isUnread, value); } }

        private string _distinguished = string.Empty;
        public string distinguished { get { return _distinguished; } set { SetProperty(ref _distinguished, value); } }

        private string _subject = string.Empty;
        public string subject { get { return _subject; } set { SetProperty(ref _subject, value); } }

        private string _linkTitle = string.Empty;
        public string linkTitle { get { return _linkTitle; } set { SetProperty(ref _linkTitle, value); } }
    }
}
