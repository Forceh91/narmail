using narmail.Models;
using narmapi;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace narmail.Mvvm
{
    public class ComposeViewModel : BindableBase
    {
        private ComposedMessageModel _message = new ComposedMessageModel();
        public ComposedMessageModel message { get { return _message; } set { SetProperty(ref _message, value); } }

        private bool _isRecipientEnabled = true;
        public bool isRecipientEnabled { get { return _isRecipientEnabled; } set { SetProperty(ref _isRecipientEnabled, value); } }

        private bool _isSubjectEnabled = true;
        public bool isSubjectEnabled { get { return _isSubjectEnabled; } set { SetProperty(ref _isSubjectEnabled, value); } }

        private bool _isBodyEnabled = true;
        public bool isBodyEnabled { get { return _isBodyEnabled; } set { SetProperty(ref _isBodyEnabled, value); } }

        private bool _isSendEnabled = false;
        public bool isSendEnabled { get { return _isSendEnabled; } set { SetProperty(ref _isSendEnabled, value); } }

        private string _parentMessage = string.Empty;
        public string parentMessage { get { return _parentMessage; } set { SetProperty(ref _parentMessage, value); } }

        private Visibility _parentMessageVisibility = Visibility.Collapsed;
        public Visibility parentMessageVisibility { get { return _parentMessageVisibility; } set { SetProperty(ref _parentMessageVisibility, value); } }

        private Visibility _sendingMessageOverlayVisibility = Visibility.Collapsed;
        public Visibility sendingMessageOverlayVisibility { get { return _sendingMessageOverlayVisibility; } set { SetProperty(ref _sendingMessageOverlayVisibility, value); } }

        public ComposeViewModel()
        {
            
        }

        public void initializeComposedMessage(InitialComposeModel initialComposeModel)
        {
            // set some info up about this message to help us figure things out
            message.subject = initialComposeModel.subject;
            message.destination = initialComposeModel.destination;
            message.messageID = initialComposeModel.messageID;
            message.isReply = initialComposeModel.isReply;

            // did we get sent a message that we're reply to?
            parentMessage = initialComposeModel.parentMessage;
            parentMessageVisibility = (string.IsNullOrEmpty(parentMessage) == true ? Visibility.Collapsed : Visibility.Visible);

            // disable the recipient/subject if this is a reply
            if (message.isReply == true)
            {
                isSubjectEnabled = false;
                isRecipientEnabled = false;
            }
        }

        public void updateSendAvailability()
        {
            // disable it whilst we check
            isSendEnabled = false;

            // make sure we have everything we need to send the message
            if (string.IsNullOrEmpty(message.subject) == true || string.IsNullOrEmpty(message.destination) == true || string.IsNullOrEmpty(message.body) == true)
                return;

            // everything seems good
            isSendEnabled = true;
        }

        public void sendMessage()
        {
            // there should never be a case where the message is null but hey.
            if (message == null)
            {
                MessageModel.sendDialogMessage("Unable to send", "An unknown error occured and we were unable to send the message");
                return;
            }

            // make sure there is a recipient
            if (string.IsNullOrEmpty(message.destination) == true)
            {
                MessageModel.sendDialogMessage("Unable to send", "You haven't specified a recipient for this message.");
                return;
            }

            // make sure there is a body
            if (string.IsNullOrEmpty(message.body) == true)
            {
                MessageModel.sendDialogMessage("Unable to send", "You haven't specified the message you want to send.");
                return;
            }

            // alright setup events to handle sending (or an error)
            NarmapiModel.api.events.eSendMessageFailed += sendMessageFailed;
            NarmapiModel.api.events.eSendMessageSuccess += sendMessageSuccess;

            // if it's a reply then we're sending a "comment"
            if (message.isReply == true)
                NarmapiModel.api.sendComment(message.messageID, message.body);
            else
                NarmapiModel.api.sendMessage(message.subject, message.destination, message.body);

            // show the overlay
            toggleSendingOverlay(true);
        }

        private void sendMessageSuccess(object sender)
        {
            // get the current frame
            Frame currentFrame = (Window.Current.Content as Frame);
            if (currentFrame == null)
                return;

            // show a dialog message
            MessageModel.sendDialogMessage("Message sent", "Your message was sent successfully");

            // send them back a page (providing they can, of course)
            if (currentFrame.CanGoBack == true)
                currentFrame.GoBack();

            // remove the spinny circle thing
            toggleSendingOverlay(false);
        }

        private void sendMessageFailed(object sender, Events.SendMessageError e)
        {
            // remove the event
            NarmapiModel.api.events.eSendMessageFailed -= sendMessageFailed;

            // throw an alert box
            if (e.errorID == "BAD_CAPTCHA")
                MessageModel.sendDialogMessage("Unable to send", "Unfortunately reddit is asking for a captcha, you will have to send this message from the desktop site. Sorry!");
            else
                MessageModel.sendDialogMessage("Unable to send", "There was an error sending your message: " + e.errorMessage);

            // remove the spinny circle thing
            toggleSendingOverlay(false);

        }

        private void toggleSendingOverlay(bool isVisible)
        {
            sendingMessageOverlayVisibility = (isVisible == true ? Visibility.Visible : Visibility.Collapsed);
            isSendEnabled = (!isVisible);
            isBodyEnabled = (!isVisible);

            // toggle the subject/recipient if it's not a reply
            if (message.isReply == false)
            {
                isSubjectEnabled = (!isVisible);
                isRecipientEnabled = (!isVisible);
            }
        }
    }
}
