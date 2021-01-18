using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using YouTubeWrappedMVC.Models;

namespace YouTubeWrappedMVC.Helpers
{
    public class YouTubeApiHelper
    {
        public static async Task<ApiVideo> GetVideoDataFromApi(string videoId)
        {
            Uri uri = new Uri(@"https://youtube.googleapis.com/youtube/v3/videos?part=snippet%2CcontentDetails&id=" + videoId + "&key=AIzaSyDYJH4akcKKjhWuxJKbs3dIl_56dk6masM");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            //request.UserAgent = "12345";

            string responseString = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseString = reader.ReadToEnd();
                ApiVideo apiVideo = JsonConvert.DeserializeObject<ApiVideo>(responseString);
                return apiVideo;

            }
        }
        public static async Task<ApiVideo> GetChannelDataFromApi(string channelId)
        {
            Uri uri = new Uri(@"https://youtube.googleapis.com/youtube/v3/videos?part=snippet%2CcontentDetails&id=" + channelId + "&key=AIzaSyDYJH4akcKKjhWuxJKbs3dIl_56dk6masM");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            //request.UserAgent = "12345";

            string responseString = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseString = reader.ReadToEnd();
                ApiVideo apiVideo = JsonConvert.DeserializeObject<ApiVideo>(responseString);
                return apiVideo;

            }
        }
    }
}
