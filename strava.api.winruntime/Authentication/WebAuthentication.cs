using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using com.strava.api.Common;
using com.strava.api.Http;

namespace com.strava.api.Authentication
{
    /// <summary>
    /// This class is used to start a local web server to receive a callback message from Strava. This message 
    /// contains a auth token. This token is then used to obtain an access token.
    /// Using this class requires admin privileges.
    /// </summary>
    public class WebAuthentication : IAuthentication
    {
        /// <summary>
        /// AccessTokenReceived is raised when an access token is received from the Strava server.
        /// </summary>
        public event EventHandler<TokenReceivedEventArgs> AccessTokenReceived;

        /// <summary>
        /// AuthCodeReceived is raised when an auth token is received from the Strava server.
        /// </summary>
        public event EventHandler<AuthCodeReceivedEventArgs> AuthCodeReceived;

        /// <summary>
        /// The access token that was received from the Strava server.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// the auth token that was received from the Strava server.
        /// </summary>
        public String AuthCode { get; set; }

        /// <summary>
        /// The Client Id provided by Strava upon registering your application.
        /// </summary>
        public String ClientId { get; set; }

        /// <summary>
        /// The Client secret provided by Strava upon registering your application.
        /// </summary>
        public String ClientSecret { get; set; }

        /// <summary>
        /// Loads an access token asynchronously from the Strava servers. Invoking this method opens a web browser.
        /// </summary>
        /// <param name="clientId">The client id from your application (provided by Strava).</param>
        /// <param name="clientSecret">The client secret (provided by Strava).</param>
        /// <param name="scope">Define what your application is allowed to do.</param>
        public async Task GetTokenAsync(String clientId, String clientSecret, Scope scope = Scope.Full)
        {
            ClientId = clientId;

            ClientSecret = clientSecret;


            string stravaCallbackuri = "http://team2b.ballendat.com";

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

            string StravaUrl = "https://www.strava.com/oauth/authorize"+
                "?client_id=" + clientId +
                "&response_type=code" + 
                "&redirect_uri="+ stravaCallbackuri +
                "&scope=" + scopeLevel +
                "&state=private" +
                "&approval_prompt=auto";
            var startUri = new Uri(StravaUrl);
            var endUri = new Uri(stravaCallbackuri);

//#if WINDOWS_PHONE_APP
//                WebAuthenticationBroker.AuthenticateAndContinue(StartUri, EndUri, null, WebAuthenticationOptions.None);
//#else
            WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                    WebAuthenticationOptions.None,
                                                    startUri,
                                                    endUri);
            if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            {
                await GetStravaTokenAsync(webAuthenticationResult.ResponseData);
            }
            else if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
                //OutputToken("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
            }
            else
            {
                //OutputToken("Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString());
            }     
//#endif
        }

        private async Task GetStravaTokenAsync(string responseWithCode)
        {
            string code = responseWithCode.Substring(responseWithCode.IndexOf("code=") + "code0".Length);


            //TODO: Save code to Storage

            if (AuthCodeReceived != null)
            {
                AuthCodeReceived(this, new AuthCodeReceivedEventArgs(code));
                AuthCode = code;
            }

            
            // Getting the Access Token            
            String url = String.Format("https://www.strava.com/oauth/token?client_id={0}&client_secret={1}&code={2}", ClientId, ClientSecret, code);

            String json = await WebRequest.SendPostAsync(new Uri(url));
            AccessToken auth = Unmarshaller<AccessToken>.Unmarshal(json);

            if (AccessTokenReceived != null)
            {
                AccessTokenReceived(this, new TokenReceivedEventArgs(auth.Token));
                AccessToken = auth.Token;
            }
        }
    }
}