using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models
{
    public class TimeWatchedPerTimeframeViewModel
    {
        public string TimeFrameName { get; set; }
        public string TimeFrameTimes { get; set; }
        public TimeSpan TimeWatched { get; set; }
        public string PercentageOfTotal { get; set; }

        public TimeWatchedPerTimeframeViewModel()
        {
        }

        public TimeWatchedPerTimeframeViewModel(string timeFrameName, string timeFrameTimes, TimeSpan timeWatched, string percentageOfTotal)
        {
            TimeFrameName = timeFrameName;
            TimeFrameTimes = timeFrameTimes;
            TimeWatched = timeWatched;
            PercentageOfTotal = percentageOfTotal;
        }
    }
}
