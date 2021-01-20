using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models
{
    public class YouTubeProcessingJob
    {
        public HistoryContext HistoryContext { get; set; }
        public int TotalVideosWatched { get; set; }
        public int TotalUniqueVideosWatched { get; set; }
        public List<ViewsPerVideoViewModel> MostViewedVideo { get; set; }
        public int TotalUniqueChannelsWatched { get; set; }
        public List<TimeWatchedPerMonthViewModel> TimeWatchedPerMonthViewModel { get; set; }
        public double AverageDailyWatchTime { get; set; }
        public double AverageVideoLength { get; set; }
        public List<TimeWatchedPerTimeframeViewModel>TimeWatchedPerTimeframe { get; set; }
        public List<ViewsPerChannelViewModel> ViewsPerChannel { get; set; }
        public List<TimePerChannelViewModel> TimeWatchedPerChannel { get; set; }

    }
}
