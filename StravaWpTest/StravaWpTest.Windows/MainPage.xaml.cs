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

        private async void ConnectToStravaClick(object s, RoutedEventArgs e)
        {
            WebAuthentication auth = new WebAuthentication();
            auth.AuthCodeReceived += (sender, args) =>
            {
                Output.Text += "Auth Code: " + args.AuthCode;
            };
            auth.AccessTokenReceived += (sender, args) =>
            {
                Output.Text += "Access Token: " + args.Token;
            };
            try
            {
                await auth.GetTokenAsync();
            }
            catch (Exception ex)
            {
                Output.Text += ex.Message;
            }

            // You can either use the StravaClient or 'single' clients like the ActivityClient.
            // The StravaClient offers predefined clients.            
            var client = new StravaClient(auth);

            try
            {
                AthleteSummary a = await client.Athletes.GetAthleteAsync();
                Output.Text = string.Format("{0} {1}", a.FirstName, a.LastName);
            }
            catch (Exception ex)
            {
                Output.Text = ex.Message;
            }            
        }
    }
}
