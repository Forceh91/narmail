using narmail.Mvvm;

namespace narmail.Models
{
    public class ComposedMessageModel : BindableBase
    {
        private string _subject = string.Empty;
        public string subject { get { return _subject; } set { SetProperty(ref _subject, value); } }

        private string _destination = string.Empty;
        public string destination { get { return _destination; } set { SetProperty(ref _destination, value); } }

        private string _body = string.Empty;
        public string body { get { return _body; } set { SetProperty(ref _body, value); } }

        private string _messageID = string.Empty;
        public string messageID { get { return _messageID; } set { SetProperty(ref _messageID, value); } }

        private bool _isReply = false;
        public bool isReply { get { return _isReply; } set { SetProperty(ref _isReply, value); } }
    }
}
