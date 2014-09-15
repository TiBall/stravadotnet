using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using com.strava.api.Common;
using com.strava.api.Http;

namespace com.strava.api.Utilities
{
    /// <summary>
    /// This class sends your polyline to the Google Maps server which then return a map image.
    /// You have to add a Google license agreement to your application when using this method.
    /// </summary>
    public class GoogleImageLoader
    {
        /// <summary>
        /// Loads an image of a map.
        /// </summary>
        /// <param name="polyline">The polyline of your activity.</param>
        /// <param name="dimension">The dimension of the picture which will be returned.</param>
        /// <param name="mapType">Choose the map type of the image.</param>
        /// <returns>An image of your activity on the specified map.</returns>
        public static async Task<Image> LoadActivityPreviewAsync(String polyline, Dimension dimension, MapType mapType)
        {
            if (dimension.Width == 0 || dimension.Height == 0)
            {
                throw new ArgumentException("Both width and height must be greater than zero.");
            }

            List<Coordinate> c = PolylineDecoder.Decode(polyline);
            String markerStart = String.Format("&markers=icon:http://tinyurl.com/np8ozqm%7C{0},{1}", c.First().Latitude, c.First().Longitude);
            String markerEnd = String.Format("&markers=icon:http://tinyurl.com/mzj8mvq%7C{0},{1}", c.Last().Latitude, c.Last().Longitude);

            String url = String.Format("http://maps.googleapis.com/maps/api/staticmap?sensor=false&maptype={0}&size={1}x{2}&{3}&{4}&path=weight:3|color:red|enc:{5}",
                mapType.ToString().ToLower(),
                dimension.Width,
                dimension.Height,
                markerStart,
                markerEnd,
                polyline);

            Image image = await ImageLoader.LoadImage(new Uri(url));
            return image;
        }
    }
}
