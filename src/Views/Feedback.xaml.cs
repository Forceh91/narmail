using narmail.Mvvm;
using Windows.UI.Xaml.Controls;

namespace narmail.Views
{
    public sealed partial class Feedback : Page
    {
        private FeedbackPageViewModel ViewModel = null;
        public Feedback()
        {
            this.InitializeComponent();
            ViewModel = (DataContext as FeedbackPageViewModel);
        }
    }
}
