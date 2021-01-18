using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models
{
    public class TimePerChannelViewModel
    {
        public string ChannelName { get; set; }
        public TimeSpan TimeWatched { get; set; }

        public TimePerChannelViewModel()
        {
        }

        public TimePerChannelViewModel(string channelName, TimeSpan timeWatched)
        {
            ChannelName = channelName;
            TimeWatched = timeWatched;
        }
    }
}
