using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YouTubeWrappedMVC.Models;

namespace YouTubeWrappedMVC.Helpers
{
    public class Calculations
    {
        public static HistoryContext GetHistoryContext(List<HistoryVideo> historyVideos)
        {
            DateTime startDate = historyVideos.Min(v => v.GetTime());
            DateTime endDate = historyVideos.Max(v => v.GetTime());

            return new HistoryContext()
            {
                StartDate = startDate,
                EndDate = endDate
            };
        }
        public static List<HoursPerDayViewModel> HoursPerDay(List<HistoryVideo> historyVideos, Dictionary<string, VideoViewModel> videoViewModelsDict)
        {
            var dates = historyVideos.Select(v => v.GetTime().Date).Distinct().ToList();
            List<TimeSpan> timeWatched = new List<TimeSpan>();

            foreach (DateTime date in dates)
            {
                var videosOnDay = historyVideos.Where(h => h.GetTime().Date == date.Date).ToList();
                TimeSpan timeForDay = new TimeSpan(0, 0, 0);
                foreach (HistoryVideo video in videosOnDay)
                {
                    if (videoViewModelsDict.ContainsKey(video.GetVideoID()))
                    {
                        VideoViewModel viewModel = videoViewModelsDict[video.GetVideoID()];
                        timeForDay = timeForDay.Add(viewModel.GetDuration());

                    }
                }
                timeWatched.Add(timeForDay);
            }

            List<HoursPerDayViewModel> hoursPerDayViewModels = new List<HoursPerDayViewModel>();
            for (int i = 0; i < timeWatched.Count; i++)
            {
                hoursPerDayViewModels.Add(new HoursPerDayViewModel()
                {
                    Date = dates[i],
                    HoursWatched = timeWatched[i]
                });
            }


            using (StreamWriter sw = new StreamWriter("AppData/Hours Per Day - " + DateTime.Now.ToFileTime() + ".csv"))
            {
                foreach (var viewModel in hoursPerDayViewModels)
                {
                    sw.WriteLine(viewModel.Date.ToShortDateString() + ";" + viewModel.HoursWatched.Hours + " Hours, " + viewModel.HoursWatched.Minutes + " Minutes and " + viewModel.HoursWatched.Seconds + " Seconds");
                }
            }

            return hoursPerDayViewModels;
        }
    }
}
