﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace com.strava.api.Http
{
    /// <summary>
    /// This class can be used to download a picture.
    /// </summary>
    public class ImageLoader
    {
        /// <summary>
        /// DOwnloads a picture from the specified url.
        /// </summary>
        /// <param name="uri">The url of the image.</param>
        /// <returns>The downloaded image.</returns>
        public async static Task<Image> LoadImage(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentException("The uri object must not be null.");
            }

            try
            {                
                var streamRef = RandomAccessStreamReference.CreateFromUri(uri);

                using (IRandomAccessStreamWithContentType fileStream = await streamRef.OpenReadAsync())
                {
                    var bitmapImage = new BitmapImage(uri);                    
                    await bitmapImage.SetSourceAsync(fileStream);
                    var img = new Image();
                    img.Source = bitmapImage;
                    return img;
                }                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Couldn't load the image: {0}", ex.Message);
            }

            return null;
        }
    }
}
