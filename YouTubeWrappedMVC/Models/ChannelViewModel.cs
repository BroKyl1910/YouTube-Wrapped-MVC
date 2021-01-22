using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models
{
    public class ChannelViewModel
    {
        public string ChannelId { get; set; }
        public string ChannelUrl { get; set; }
        public string ChannelTitle { get; set; }
        public string ChannelThumbnail { get; set; }
        public string ChannelBanner { get; set; }
    }
}
