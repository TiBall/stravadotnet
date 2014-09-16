using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using com.strava.api.Activities;
using com.strava.api.Client;
using StravaWpTest.Annotations;
using StravaWpTest.Common;

namespace StravaWpTest.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StravaFeed : Page, INotifyPropertyChanged
    {
        public StravaFeed()
        {
            this.InitializeComponent();

            DataContext = this;

            Loaded += StravaFeed_Loaded;

        }

        async void StravaFeed_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(StravaLoginHelper.StravaAuth.AccessToken))
            {
                var rootframe = Window.Current.Content as Frame;
                if (rootframe.CanGoBack)
                {
                    rootframe.GoBack();
                }
                else
                {
                    rootframe.Navigate(typeof (MainPage));
                }
            }
            var client = new StravaClient(StravaLoginHelper.StravaAuth);
            var activities = await client.Activities.GetActivitiesBeforeAsync(DateTime.Now,1,30);

            Activities = new ObservableCollection<ActivitySummary>(activities);            

            ActivitiesList.ItemsSource = Activities;
        }

        ObservableCollection<ActivitySummary> Activities = new ObservableCollection<ActivitySummary>();
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
