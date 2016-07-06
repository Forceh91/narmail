using System;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace narmail.Models
{
    public static class MessageModel
    {
        private static IAsyncAction asyncCommand = null;

        public static void sendDialogMessage(string dialogTitle, string dialogMessage)
        {
            // check the async command, and cacncel if not null
            if (asyncCommand != null)
                asyncCommand.Cancel();

            // and grab the dispatcher so that we can safely create our dialog message
            asyncCommand = CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    MessageDialog messageDialog = new MessageDialog(dialogMessage, dialogTitle);
                    await messageDialog.ShowAsync();
                }
            );
        }

        public static void sendAcceptCancelMessage(string dialogTitle, string dialogMessage, UICommand acceptCommand, UICommand declineCommand)
        {
            // check the async command, and cacncel if not null
            if (asyncCommand != null)
                asyncCommand.Cancel();

            // and grab the dispatcher so that we can safely create our dialog message
            asyncCommand = CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    MessageDialog messageDialog = new MessageDialog(dialogMessage, dialogTitle);
                    messageDialog.Commands.Add(acceptCommand);
                    messageDialog.Commands.Add(declineCommand);
                    await messageDialog.ShowAsync();
                }
            );
        }
    }
}
