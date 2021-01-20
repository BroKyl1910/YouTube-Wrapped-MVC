using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models
{
    public class TimeWatchedPerMonthViewModel
    {
        public MonthYearViewModel MonthYearViewModel { get; set; }
        public double HoursWatched { get; set; }

        public TimeWatchedPerMonthViewModel()
        {
        }

        public TimeWatchedPerMonthViewModel(MonthYearViewModel monthYearViewModel, double hoursWatched)
        {
            MonthYearViewModel = monthYearViewModel;
            HoursWatched = hoursWatched;
        }
    }
}
