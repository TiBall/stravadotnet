using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using com.strava.api.Athletes;
using com.strava.api.Authentication;
using com.strava.api.Client;

namespace StravaWpTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        #region Strava Values

        private const string CLIENT_ID = "1234";
        private const string CLIENT_SECRET = "c85a412345432562c2c4c81130fb392ae06";
        private const string CALLBACK_URI = "http://mycallbackdomainthatreturnsanyvalidpage.com";

        #endregion

        private async void ConnectToStravaClick(object s, RoutedEventArgs e)
        {
            var auth = new WebAuthentication();
            auth.AuthCodeReceived += (sender, args) => WriteLine("Auth Code: " + args.AuthCode);
            auth.AccessTokenReceived += (sender, args) => WriteLine("Access Token: " + args.Token);
            try
            {
                await auth.GetTokenAsync(CLIENT_ID, CLIENT_SECRET, CALLBACK_URI);
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }

            // You can either use the StravaClient or 'single' clients like the ActivityClient.
            // The StravaClient offers predefined clients.            
            var client = new StravaClient(auth);

            try
            {
                AthleteSummary a = await client.Athletes.GetAthleteAsync();
                WriteLine(string.Format("{0} {1}", a.FirstName, a.LastName));
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }
        }

        private void WriteLine(string text)
        {
            Output.Text += "\n\r" + text;
        }
    }
}
