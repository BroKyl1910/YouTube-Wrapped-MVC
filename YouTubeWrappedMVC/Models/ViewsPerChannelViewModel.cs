using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models
{
    public class ViewsPerChannelViewModel
    {
        public ViewsPerChannelViewModel()
        {
        }

        public ViewsPerChannelViewModel(string channelName, int numVideos)
        {
            ChannelName = channelName;
            NumVideos = numVideos;
        }

        public string ChannelName { get; set; }
        public int NumVideos{ get; set; }
    }
}
