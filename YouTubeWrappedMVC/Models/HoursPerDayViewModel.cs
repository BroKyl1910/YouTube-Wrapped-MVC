using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models
{
    public class HoursPerDayViewModel
    {
        public DateTime Date { get; set; }
        public TimeSpan HoursWatched { get; set; }
    }
}
