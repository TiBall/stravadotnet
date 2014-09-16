using com.strava.api.Authentication;

namespace StravaWpTest.Common
{
    class StravaLoginHelper
    {
        #region Strava Values

        public const string CLIENT_ID = "3005";
        public const string CLIENT_SECRET = "c85a4d476f0b5391c9d62c2c4c81130fb392ae06";
        public const string CALLBACK_URI = "http://strava.ballendat.com";

        #endregion

        
        private static WebAuthentication _stravaAuth;
        public static WebAuthentication StravaAuth
        {
            get { return _stravaAuth ?? (_stravaAuth = new WebAuthentication()); }
        }
    }
}
