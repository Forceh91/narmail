﻿using narmail.Models;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace narmail
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static App currentApp { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            // setup the current app
            currentApp = this;

            // initialize the api
            NarmapiModel.initializeAPI();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        { 
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter

                    if (string.IsNullOrEmpty(NarmapiModel.getAccessToken()) == true)
                        rootFrame.Navigate(typeof(Views.Landing), e.Arguments);
                    else
                        rootFrame.Navigate(typeof(Views.Inbox), e.Arguments);
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            Frame rootFrame = (Window.Current.Content as Frame);

            // clear the backstack if we go to the home page
            if (e.SourcePageType == typeof(Views.Inbox) || e.SourcePageType == typeof(Views.Landing))
                rootFrame.BackStack?.Clear();

            // handle the back button
            SystemNavigationManager systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            if (systemNavigationManager == null)
                return;

            // can we go back?
            if (rootFrame.CanGoBack == true)
                systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            else
                systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            // handle the back button event
            systemNavigationManager.BackRequested -= backButtonEvent;
            systemNavigationManager.BackRequested += backButtonEvent;
        }

        private void backButtonEvent(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = (Window.Current.Content as Frame);

            // can we go back?
            if (rootFrame.CanGoBack == true)
                rootFrame.GoBack();
            else
            {
                // we can't go back, if we're on the front page then exit the app
                if (rootFrame.CurrentSourcePageType == typeof(Views.Inbox))
                    Current.Exit();
            }

            e.Handled = true;
        }
    }
}
