using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Security.Authentication.Web;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using com.strava.api.Authentication;
using StravaWpTest.Common;
using StravaWpTest.Pages;

namespace StravaWpTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IWebAuthenticationContinuable
    {
        public static MainPage Current;

        public MainPage()
        {
            this.InitializeComponent();            

            // This is a static public property that allows downstream pages to get a handle to the MainPage instance
            // in order to call methods that are in this class.
            Current = this;            

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        
            this.NavigationCacheMode = NavigationCacheMode.Required;

            Loaded += (x,a) => CheckLoggedIn();
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            var rootframe = Window.Current.Content as Frame;
            if (rootframe != null && rootframe.CanGoBack)
            {
                rootframe.GoBack();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {            
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {            
            var startUri = new Uri(StravaLoginHelper.StravaAuth.BuildStravaTokenUrl(
                StravaLoginHelper.CLIENT_ID, 
                StravaLoginHelper.CLIENT_SECRET,
                StravaLoginHelper.CALLBACK_URI));
            var endUri = new Uri(StravaLoginHelper.CALLBACK_URI);

            WebAuthenticationBroker.AuthenticateAndContinue(startUri,endUri);
        }
        
        public async void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            try
            {
                await StravaLoginHelper.StravaAuth.ContinueGettingTokenWithTempCodeAsync(args.WebAuthenticationResult.ResponseData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            var dialog = new MessageDialog("login sucessfull. Token: " + StravaLoginHelper.StravaAuth.AccessToken);
            dialog.ShowAsync();

            CheckLoggedIn();
        }

        private void CheckLoggedIn()
        {
            ShowActivitesBtn.IsEnabled = !string.IsNullOrEmpty(StravaLoginHelper.StravaAuth.AccessToken);
        }

        private static void ShowFeedPage()
        {
            var frame = Window.Current.Content as Frame;
            if (frame != null)
                frame.Navigate(typeof (StravaFeed));
        }

        private void ShowActivitesClick(object sender, RoutedEventArgs e)
        {
            ShowFeedPage();
        }
    }
}
