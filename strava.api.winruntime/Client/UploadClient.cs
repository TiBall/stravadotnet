using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using com.strava.api.Activities;
using com.strava.api.Api;
using com.strava.api.Authentication;
using com.strava.api.Common;
using com.strava.api.Http;
using com.strava.api.Upload;

namespace com.strava.api.Client
{
    /// <summary>
    /// This class contains methods for uploading new activities to Strava. Use the UploadStatusCheck class to 
    /// check the status of an upload.
    /// </summary>
    public class UploadClient : BaseClient
    {
        #region Async

        /// <summary>
        /// Initializes a new instance of the UploadClient class.
        /// </summary>
        /// <param name="auth">The IAuthenticator object used to authenticate with Strava.</param>
        public UploadClient(IAuthentication auth) : base(auth) { }

        /// <summary>
        /// Uploads an activity.
        /// </summary>
        /// <param name="file">The path to the activity file on your local hard disk.</param>
        /// <param name="dataFormat">The format of the file.</param>
        /// <param name="activityType">The type of the activity.</param>
        /// <returns>The status of the upload.</returns>
        public async Task<UploadStatus> UploadActivityAsync(StorageFile file, DataFormat dataFormat, ActivityType activityType = ActivityType.Ride)
        {
            String format = String.Empty;

            switch (dataFormat)
            {
                case DataFormat.Fit:
                    format = "fit";
                    break;
                case DataFormat.FitGZipped:
                    format = "fit.gz";
                    break;
                case DataFormat.Gpx:
                    format = "gpx";
                    break;
                case DataFormat.GpxGZipped:
                    format = "gpx.gz";
                    break;
                case DataFormat.Tcx:
                    format = "tcx";
                    break;
                case DataFormat.TcxGZipped:
                    format = "tcx.gz";
                    break;
            }
           
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", Authentication.AccessToken));

            MultipartFormDataContent content = new MultipartFormDataContent();

            byte[] fileBytes = null;
            using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }

            var byteArrayContent = new ByteArrayContent(fileBytes);

            content.Add(byteArrayContent, "file", file.Name);

            HttpResponseMessage result = await client.PostAsync(
                String.Format("https://www.strava.com/api/v3/uploads?data_type={0}&activity_type={1}",
                format,
                activityType.ToString().ToLower()),
                content);

            String json = await result.Content.ReadAsStringAsync();

            return Unmarshaller<UploadStatus>.Unmarshal(json);
        }

        /// <summary>
        /// Checks the status of an upload.
        /// </summary>
        /// <param name="uploadId">The id of the upload.</param>
        /// <returns>The status of the upload.</returns>
        public async Task<UploadStatus> CheckUploadStatusAsync(String uploadId)
        {
            String checkUrl = String.Format("{0}/{1}?access_token={2}", Endpoints.Uploads, uploadId, Authentication.AccessToken);
            String json = await WebRequest.SendGetAsync(new Uri(checkUrl));

            return Unmarshaller<UploadStatus>.Unmarshal(json);
        }

        #endregion

        #region Sync

        /// <summary>
        /// Checks the status of an upload.
        /// </summary>
        /// <param name="uploadId">The id of the upload.</param>
        /// <returns>The status of the upload.</returns>
        public async Task<UploadStatus> CheckUploadStatus(String uploadId)
        {
            String checkUrl = String.Format("{0}/{1}?access_token={2}", Endpoints.Uploads, uploadId, Authentication.AccessToken);
            String json = await WebRequest.SendGet(new Uri(checkUrl));

            return Unmarshaller<UploadStatus>.Unmarshal(json);
        }

        #endregion
    }
}
