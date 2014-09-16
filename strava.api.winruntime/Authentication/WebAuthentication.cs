using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using com.strava.api.Common;
using WebRequest = com.strava.api.Http.WebRequest;

namespace com.strava.api.Authentication
{
    /// <summary>
    /// This class is used to start a local web server to receive a callback message from Strava. This message 
    /// contains a auth token. This token is then used to obtain an access token.
    /// Using this class requires admin privileges.
    /// </summary>
    public class WebAuthentication : IAuthentication
    {
        private string _clientId;
        private string _accessToken;

        /// <summary>
        /// AccessTokenReceived is raised when an access token is received from the Strava server.
        /// </summary>
        public event EventHandler<TokenReceivedEventArgs> AccessTokenReceived;

        /// <summary>
        /// AuthCodeReceived is raised when an auth token is received from the Strava server.
        /// </summary>
        public event EventHandler<AuthCodeReceivedEventArgs> AuthCodeReceived;


        private const string ACCESSTOKEN_KEY = "AccessToken";

        /// <summary>
        /// The access token that was received from the Strava server.
        /// </summary>
        public string AccessToken
        {
            get
            {
                return _accessToken ??
                 (_accessToken = (String)Windows.Storage.ApplicationData.Current.RoamingSettings.Values[ACCESSTOKEN_KEY]);                
            }
            set 
            {
                if (_accessToken != value)
                {
                    _accessToken = value;
                    Windows.Storage.ApplicationData.Current.RoamingSettings.Values[ACCESSTOKEN_KEY] = value;
                } 
            }
        }
        
        private const string CLIENTID_KEY = "clientId";

        /// <summary>
        /// The Client Id provided by Strava upon registering your application.
        /// </summary>
        public String ClientId
        {
            get
            {
                return _clientId ??
                    (_clientId = (String)Windows.Storage.ApplicationData.Current.RoamingSettings.Values[CLIENTID_KEY]);
            }
            set
            {
                if (_clientId != value)
                {
                    _clientId = value;                    
                    Windows.Storage.ApplicationData.Current.RoamingSettings.Values[CLIENTID_KEY] = value;
                }
            }
        }

        /// <summary>
        /// The Client secret provided by Strava upon registering your application.
        /// </summary>
        public String ClientSecret { get; set; }

        public string BuildStravaTokenUrl(String clientId, string clientSecret, string callbackUri,
            Scope scope = Scope.Full)
        {
            ClientId = clientId;

            ClientSecret = clientSecret;

            String scopeLevel = String.Empty;

            switch (scope)
            {
                case Scope.Full:
                    scopeLevel = "view_private,write";
                    break;
                case Scope.Public:
                    scopeLevel = "public";
                    break;
                case Scope.ViewPrivate:
                    scopeLevel = "view_private";
                    break;
                case Scope.Write:
                    scopeLevel = "write";
                    break;
            }

            return String.Format("https://"+"www.strava.com/oauth/authorize?client_id={0}&response_type=code&redirect_uri={1}&scope={2}&state=private&approval_prompt=auto",
                clientId,callbackUri,scopeLevel);
        }


        /// <summary>
        /// Loads an access token asynchronously from the Strava servers. Invoking this method opens a web browser.
        /// </summary>
        /// <param name="clientId">The client id from your application (provided by Strava).</param>
        /// <param name="clientSecret">The client secret (provided by Strava).</param>
        /// <param name="callbackUri">the callback uri exactly as provided with the strava app registration</param>
        /// <param name="scope">Define what your application is allowed to do.</param>
        public async Task GetTokenAsync(string clientId, string clientSecret, string callbackUri, Scope scope = Scope.Full)
        {
            string stravaUrl = BuildStravaTokenUrl(clientId, clientSecret, callbackUri, scope);

            var startUri = new Uri(stravaUrl);
            var endUri = new Uri(callbackUri);

//#if WINDOWS_PHONE_APP
                //WebAuthenticationBroker.AuthenticateAndContinue(StartUri, EndUri, null, WebAuthenticationOptions.None);
//#else
            WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                    WebAuthenticationOptions.None,
                                                    startUri,
                                                    endUri);
            if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                await ContinueGettingTokenWithTempCodeAsync(webAuthenticationResult.ResponseData);
            }
            else if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                throw new HttpRequestException("HTTP Error returned by AuthenticateAsync() : " + webAuthenticationResult.ResponseErrorDetail);
            }
            else
            {
                throw new HttpRequestException("Error returned by AuthenticateAsync() : " + webAuthenticationResult.ResponseStatus);
            }    
        }
        
        /// <summary>
        /// After receiving the temporary code, this method continues authentication with strava to receive the token
        /// On Windows Phone you have to call this method after the app is resumed with WebAuthentificationBroikerEventArgs
        /// On WIndows, this is called automatically when calling GetTokenAsync
        /// </summary>
        /// <param name="responseWithCode">The cesponse string containing the tempporary code</param>
        public async Task ContinueGettingTokenWithTempCodeAsync(string responseWithCode)
        {
            string code = responseWithCode.Substring(responseWithCode.IndexOf("code=") + "code=".Length);
            
            if (AuthCodeReceived != null)
            {
                AuthCodeReceived(this, new AuthCodeReceivedEventArgs(code));                
            }

            
            // Getting the Access Token            
            String url = String.Format("https://www.strava.com/oauth/token?client_id={0}&client_secret={1}&code={2}", ClientId, ClientSecret, code);

            String json = await WebRequest.SendPostAsync(new Uri(url));
            AccessToken auth = Unmarshaller<AccessToken>.Unmarshal(json);

            AccessToken = auth.Token;

            if (AccessTokenReceived != null)
            {                
                AccessTokenReceived(this, new TokenReceivedEventArgs(auth.Token));                
            }
        }
    }
}