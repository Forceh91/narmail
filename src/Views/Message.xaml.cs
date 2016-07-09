using narmail.Models;
using narmail.Mvvm;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace narmail.Views
{
    public sealed partial class Message : Page
    {
        private MessageViewModel ViewModel = null;
        public Message()
        {
            this.InitializeComponent();
            ViewModel = (DataContext as MessageViewModel);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RedditMessageModel redditMessageModel = null;

            // see if we got sent a message
            if (e.Parameter == null)
                return;

            // make sure it's the right type of model
            if (e.Parameter.GetType() != typeof(RedditMessageModel))
                return;

            // convert the parameter
            redditMessageModel = (e.Parameter as RedditMessageModel);

            // tell the view model what message we're using
            ViewModel.pageLoaded(redditMessageModel);

            // base stuff
            base.OnNavigatedTo(e);
        }
    }
}
