using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models
{
    public class TimePerChannelViewModel
    {
        public ChannelViewModel Channel { get; set; }
        public TimeSpan TimeWatched { get; set; }

        public TimePerChannelViewModel()
        {
        }

        public TimePerChannelViewModel(ChannelViewModel channel, TimeSpan timeWatched)
        {
            Channel = channel;
            TimeWatched = timeWatched;
        }
    }
}
