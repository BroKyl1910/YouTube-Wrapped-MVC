using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeWrappedMVC.Models.ApiChannelNS;

namespace YouTubeWrappedMVC.Models
{
    public class ChannelViewModel
    {
        public string Id { get; set; }
        public string ChannelTitle { get; set; }
        public string ChannelUrl { get; set; }
        public string ChannelThumbnail { get; set; }
        public string ChannelBanner { get; set; }


        public static ChannelViewModel FromApiChannel(ApiChannel apiChannel)
        {
            ChannelViewModel viewModel = new ChannelViewModel()
            {
                Id = apiChannel.Items[0].Id,
                ChannelTitle = apiChannel.Items[0].Snippet.Title,
                ChannelUrl = "https://www.youtube.com/channel/"+ apiChannel.Items[0].Id,
                ChannelThumbnail = apiChannel.Items[0].Snippet.Thumbnails.Default.Url == null ? "" : apiChannel.Items[0].Snippet.Thumbnails.Default.Url,
                ChannelBanner = apiChannel.Items[0].BrandingSettings.Image == null ? "" : apiChannel.Items[0].BrandingSettings.Image.BannerExternalUrl
            };
            return viewModel;
        }
        public static ChannelViewModel DeserializeObject(string line)
        {
            var lineParts = line.Split("#del#");
            return new ChannelViewModel()
            {
                Id = lineParts[0],
                ChannelTitle = lineParts[1],
                ChannelUrl = lineParts[2],
                ChannelThumbnail = lineParts[3],
                ChannelBanner = lineParts[4]
            };
        }

        public static string SerializeObject(ChannelViewModel viewModel)
        {
            return viewModel.Id + "#del#" + viewModel.ChannelTitle + "#del#" + viewModel.ChannelUrl + "#del#" + viewModel.ChannelThumbnail + "#del#" + viewModel.ChannelBanner;
        }
    }
}
