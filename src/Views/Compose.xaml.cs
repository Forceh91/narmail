using narmail.Models;
using narmail.Mvvm;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace narmail.Views
{
    public sealed partial class Compose : Page
    {
        private ComposeViewModel ViewModel = null;
        public Compose()
        {
            this.InitializeComponent();

            ViewModel = (DataContext as ComposeViewModel);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // see if we got a parameter
            if (e.Parameter != null)
            {
                // check it's the right type of model
                if (e.Parameter.GetType() == typeof(InitialComposeModel))
                {
                    // convert the parameter to a model and initialize the view model with these details
                    InitialComposeModel initialComposeModel = (e.Parameter as InitialComposeModel);
                    if (initialComposeModel != null)
                        ViewModel.initializeComposedMessage(initialComposeModel);
                }
            }

            // base navigation stuff
            base.OnNavigatedTo(e);
        }

        private void sendComposedMessage(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // send the composed message
            ViewModel.sendMessage();
        }

        private void messageInputKeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            // check that we can send the message
            ViewModel.updateSendAvailability();
        }
    }
}
