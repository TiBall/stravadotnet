﻿using System.Net.Http;

namespace com.strava.api.Http
{
    /// <summary>
    /// This class holds information about a asynchronously received server response.
    /// </summary>
    public class AsyncResponseReceivedEventArgs
    {
        /// <summary>
        /// The HttpResponseMessage that was received from the server.
        /// </summary>
        public HttpResponseMessage Response { get; set; }

        /// <summary>
        /// Initializes a new instance of the AsyncResponseReceivedEventArgs class.
        /// </summary>
        /// <param name="response">The HttpResponseMessage object received from the server.</param>
        public AsyncResponseReceivedEventArgs(HttpResponseMessage response)
        {
            Response = response;
        }
    }
}
