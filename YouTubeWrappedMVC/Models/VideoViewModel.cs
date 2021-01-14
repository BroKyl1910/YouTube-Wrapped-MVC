using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models
{
    public class VideoViewModel
    {
        public string Id { get; set; }
        public string VideoTitle { get; set; }
        public string VideoUrl { get; set; }
        public string Duration { get; set; }
        public string VideoThumbnail { get; set; }
        public string ChannelTitle { get; set; }
        public string ChannelId { get; set; }


        public VideoViewModel()
        {
        }

        public VideoViewModel(string id, string videoTitle, string videoUrl, string duration, string videoThumbnail, string channelTitle, string channelId)
        {
            Id = id;
            VideoTitle = videoTitle;
            VideoUrl = videoUrl;
            Duration = duration;
            VideoThumbnail = videoThumbnail;
            ChannelTitle = channelTitle;
            ChannelId = channelId;
        }

        public TimeSpan GetDuration()
        {

            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            //PT1H5M45S
            string strDuration = Duration.Substring(2);
            //1H5M45S
            int hIndex = strDuration.IndexOf('H');
            if (hIndex > -1)
            {
                hours = Convert.ToInt32(strDuration.Substring(0, hIndex));
                strDuration = strDuration.Substring(hIndex + 1);
            }
            //5M45S
            int mIndex = strDuration.IndexOf('M');
            if (mIndex > -1)
            {
                minutes = Convert.ToInt32(strDuration.Substring(0, mIndex));
                strDuration = strDuration.Substring(mIndex + 1);
            }
            //45S
            int sIndex = strDuration.IndexOf('S');
            if (sIndex > -1)
            {
                seconds = Convert.ToInt32(strDuration.Substring(0, sIndex));
            }

            TimeSpan duration = new TimeSpan(hours, minutes, seconds);
            return duration;

        }

        public override bool Equals(object obj)
        {
            return (Id == ((VideoViewModel)obj).Id);
        }

        public static VideoViewModel DeserializeObject(string line)
        {
            var lineParts = line.Split("#del#");
            return new VideoViewModel()
            {
                Id = lineParts[0],
                VideoTitle = lineParts[1],
                VideoUrl = lineParts[2],
                Duration = lineParts[3],
                VideoThumbnail = lineParts[4],
                ChannelTitle = lineParts[5],
                ChannelId = lineParts[6]
            };
        }

        public static VideoViewModel FromApiVideo(ApiVideo apiVideo)
        {
            VideoViewModel viewModel = new VideoViewModel()
            {
                Id = apiVideo.Items[0].Id,
                ChannelId = apiVideo.Items[0].Snippet.ChannelId,
                ChannelTitle = apiVideo.Items[0].Snippet.ChannelTitle,
                Duration = apiVideo.Items[0].ContentDetails.Duration,
                VideoThumbnail = apiVideo.Items[0].Snippet.Thumbnails.Default.Url,
                VideoTitle = apiVideo.Items[0].Snippet.Title,
                VideoUrl = "https://www.youtube.com/watch?v=" + apiVideo.Items[0].Id,
            };
            return viewModel;
        }

        public static string SerializeObject(VideoViewModel viewModel)
        {
            return viewModel.Id + "#del#" + viewModel.VideoTitle + "#del#" + viewModel.VideoUrl + "#del#" + viewModel.Duration + "#del#" + viewModel.VideoThumbnail + "#del#" + viewModel.ChannelTitle + "#del#" + viewModel.ChannelId;
        }
    }
}
