using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models
{
    public class ViewsPerChannelViewModel
    {
        public ChannelViewModel Channel { get; set; }
        public int NumVideos { get; set; }

        public ViewsPerChannelViewModel()
        {
        }

        public ViewsPerChannelViewModel(ChannelViewModel channel, int numVideos)
        {
            Channel = channel;
            NumVideos = numVideos;
        }

    }
}
