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
        private List<HistoryVideo> HistoryVideos { get; set; }
        private Dictionary<string, VideoViewModel> VideoViewModelsDict { get; set; }


        public Calculations(List<HistoryVideo> historyVideos, Dictionary<string, VideoViewModel> videoViewModelsDict)
        {
            HistoryVideos = historyVideos;
            VideoViewModelsDict = videoViewModelsDict;
            CleanseHistoryVideos();
            //System.Diagnostics.Debug.WriteLine("HistoryVideos Count: " + HistoryVideos.Count);
        }

        // Remove all videos that don't have data in the dictionary
        private void CleanseHistoryVideos()
        {
            HistoryVideos = HistoryVideos.Where(v => VideoViewModelsDict.ContainsKey(v.GetVideoID())).ToList();
        }

        // Get a list of timeframes and amount of time watched in each
        public List<TimeWatchedPerTimeframeViewModel> GetHoursMostFrequentlyWatched()
        {
            List<string> timeCategoryNames = new List<string> {
                "Late Evening",
                "Night Owl Hours",
                "Crack of Dawn",
                "Early Morning",
                "Midday",
                "Late Afternoon",
                "Early Evening",
            };

            List<string> timeCategoryBounds = new List<string> {
                "Between 9pm and 12am",
                "Between 12am and 4am",
                "Between 4am and 7am",
                "Between 7am and 10am",
                "Between 10am and 2pm",
                "Between 2pm and 5pm",
                "Between 5pm and 9pm",
            };

            List<DateTime> timeUpperBounds = new List<DateTime>()
            {
                new DateTime (1,1,1,0,0,0),
                new DateTime (1,1,1,4,0,0),
                new DateTime (1,1,1,7,0,0),
                new DateTime (1,1,1,10,0,0),
                new DateTime (1,1,1,14,0,0),
                new DateTime (1,1,1,17,0,0),
                new DateTime (1,1,1,21,0,0),

            };

            List<TimeSpan> timesWatched = new List<TimeSpan>() {
                new TimeSpan(0,0,0),
                new TimeSpan(0,0,0),
                new TimeSpan(0,0,0),
                new TimeSpan(0,0,0),
                new TimeSpan(0,0,0),
                new TimeSpan(0,0,0),
                new TimeSpan(0,0,0),
            };

            foreach (var video in HistoryVideos)
            {
                bool categorised = false;
                for (int i = 0; i < timeUpperBounds.Count; i++)
                {
                    DateTime timeFrame = timeUpperBounds[i];
                    if (video.GetTime().TimeOfDay.CompareTo(timeFrame.TimeOfDay) <= 0)
                    {
                        timesWatched[i] = timesWatched[i].Add(VideoViewModelsDict[video.GetVideoID()].GetDuration());
                        categorised = true;
                        break;
                    }
                }
                if (!categorised)
                {
                    timesWatched[0] = timesWatched[0].Add(VideoViewModelsDict[video.GetVideoID()].GetDuration());
                }
            }

            List<TimeWatchedPerTimeframeViewModel> viewModels = new List<TimeWatchedPerTimeframeViewModel>();
            var totalTimeWatched = timesWatched.Sum(t => t.TotalMinutes);
            for (int i = 0; i < timesWatched.Count; i++)
            {
                viewModels.Add(new TimeWatchedPerTimeframeViewModel()
                {
                    TimeFrameName = timeCategoryNames[i],
                    TimeFrameTimes = timeCategoryBounds[i],
                    TimeWatched = timesWatched[i],
                    PercentageOfTotal = Math.Round((timesWatched[i].TotalMinutes / totalTimeWatched) * 100) + "%"
                });
            }
            viewModels = viewModels.OrderByDescending(vm => vm.TimeWatched).ToList();

            using (StreamWriter sw = new StreamWriter(UriHelper.CALCULATIONS_OUTPUT_DIR + "/Times Most Frequently Watched - " + DateTime.Now.ToFileTime() + ".csv"))
            {
                foreach (var viewModel in viewModels)
                {
                    sw.WriteLine(viewModel.TimeFrameName + ";" + viewModel.TimeFrameTimes + ";" + Math.Round(viewModel.TimeWatched.TotalHours) + " Hours;" + viewModel.PercentageOfTotal);
                }
            }

            return viewModels;
        }

        // Get month with the most time watched
        public List<TimeWatchedPerMonthViewModel> GetTimeWatchedPerMonth()
        {
            var monthViewModels = HistoryVideos.Select(v => new MonthYearViewModel(v.GetTime().Month, v.GetTime().Year)).Distinct().ToList();
            var monthlyHours = monthViewModels.Select(vm => HistoryVideos.Where(h => new MonthYearViewModel(h.GetTime().Month, h.GetTime().Year) == vm).Sum(v => VideoViewModelsDict[v.GetVideoID()].GetDuration().TotalHours)).ToList();

            List<TimeWatchedPerMonthViewModel> viewModels = new List<TimeWatchedPerMonthViewModel>();
            for (int i = 0; i < monthViewModels.Count; i++)
            {
                viewModels.Add(new TimeWatchedPerMonthViewModel(monthViewModels[i], Math.Round(monthlyHours[i])));
            }

            return viewModels.OrderByDescending(vm => vm.HoursWatched).ToList();
        }

        // Get average length of video in history in miniutes
        public double GetAverageLengthOfVideo()
        {
            var lengths = HistoryVideos.Select(v => VideoViewModelsDict[v.GetVideoID()].GetDuration());
            double averageTotalSeconds = lengths.Average(l => l.TotalSeconds);
            double averageMinutes = Math.Round(averageTotalSeconds / 60);
            return averageMinutes;
        }

        // Get start and end date of history
        public HistoryContext GetHistoryContext()
        {
            DateTime startDate = HistoryVideos.Min(v => v.GetTime());
            DateTime endDate = HistoryVideos.Max(v => v.GetTime());

            return new HistoryContext()
            {
                StartDate = startDate,
                EndDate = endDate
            };
        }

        // Get average time watched per day in hours
        public double GetAverageDailyWatchTime()
        {
            var dates = HistoryVideos.Select(v => v.GetTime().Date).Distinct().ToList();
            var dailyTimesMinutes = dates.Select(d => HistoryVideos.Where(h => h.GetTime().Date.Equals(d.Date)).Sum(v => VideoViewModelsDict[v.GetVideoID()].GetDuration().TotalMinutes));
            double averageTimeMinutes = dailyTimesMinutes.Average();
            double averageTimeHours = Math.Round(averageTimeMinutes / 60);
            return averageTimeHours;
        }

        // Get total number of videos watched
        public int GetTotalVideosWatched()
        {
            return HistoryVideos.Count;
        }

        // Get total number of unqiue videos watched
        public int GetTotalUniqueVideosWatched()
        {
            return HistoryVideos.Select(hv => hv.GetVideoID()).Distinct().Count();
        }

        // Get total number of channels watched
        public int GetTotalUniqueChannelsWatched()
        {
            return HistoryVideos.Select(v => VideoViewModelsDict[v.GetVideoID()].ChannelId).Distinct().Count();
        }

        // Calculate how many hours the user watched per day of their history
        public List<HoursPerDayViewModel> GetHoursPerDay()
        {
            var dates = HistoryVideos.Select(v => v.GetTime().Date).Distinct().ToList();
            List<TimeSpan> timeWatched = new List<TimeSpan>();

            foreach (DateTime date in dates)
            {
                var videosOnDay = HistoryVideos.Where(h => h.GetTime().Date == date.Date).ToList();
                TimeSpan timeForDay = new TimeSpan(0, 0, 0);
                foreach (HistoryVideo video in videosOnDay)
                {
                    VideoViewModel viewModel = VideoViewModelsDict[video.GetVideoID()];
                    timeForDay = timeForDay.Add(viewModel.GetDuration());
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


            using (StreamWriter sw = new StreamWriter(UriHelper.CALCULATIONS_OUTPUT_DIR + "/Hours Per Day - " + DateTime.Now.ToFileTime() + ".csv"))
            {
                foreach (var viewModel in hoursPerDayViewModels)
                {
                    sw.WriteLine(viewModel.Date.ToShortDateString() + ";" + viewModel.HoursWatched.Hours + " Hours, " + viewModel.HoursWatched.Minutes + " Minutes and " + viewModel.HoursWatched.Seconds + " Seconds");
                }
            }

            return hoursPerDayViewModels;
        }

        // Calculate most viewed videos
        public List<ViewsPerVideoViewModel> GetMostViewedVideos()
        {
            var videoIds = HistoryVideos.Select(v => v.GetVideoID()).Distinct().ToList();
            var timesWatched = videoIds.Select(id => HistoryVideos.Count(v => v.GetVideoID() == id)).ToList();

            List<ViewsPerVideoViewModel> viewsPerVideoViewModels = new List<ViewsPerVideoViewModel>();
            for (int i = 0; i < videoIds.Count; i++)
            {
                viewsPerVideoViewModels.Add(new ViewsPerVideoViewModel()
                {
                    VideoViewModel = VideoViewModelsDict[videoIds[i]],
                    NumViews = timesWatched[i]
                });
            }

            viewsPerVideoViewModels = viewsPerVideoViewModels.OrderByDescending(vm => vm.NumViews).ToList();
            using (StreamWriter sw = new StreamWriter(UriHelper.CALCULATIONS_OUTPUT_DIR + "/Views Per Video - " + DateTime.Now.ToFileTime() + ".csv"))
            {
                foreach (var viewModel in viewsPerVideoViewModels)
                {
                    sw.WriteLine(viewModel.VideoViewModel.VideoTitle + ";" + viewModel.NumViews);
                }
            }

            return viewsPerVideoViewModels;
        }

        // Calculate most viewed channel
        public List<ViewsPerChannelViewModel> GetMostViewedChannel()
        {
            var channelTitles = new List<string>();
            var numVideosWatched = new List<int>();
            foreach (var video in HistoryVideos)
            {

                channelTitles.Add(VideoViewModelsDict[video.GetVideoID()].ChannelTitle);

            }

            channelTitles = channelTitles.Distinct().ToList();
            foreach (var channelTitle in channelTitles)
            {
                int numVids = 0;
                foreach (HistoryVideo historyVideo in HistoryVideos)
                {
                    if (VideoViewModelsDict[historyVideo.GetVideoID()].ChannelTitle == channelTitle)
                    {
                        numVids++;
                    }

                }
                numVideosWatched.Add(numVids);
                numVids = 0;
            }

            List<ViewsPerChannelViewModel> viewsPerChannelViewModels = new List<ViewsPerChannelViewModel>();
            for (int i = 0; i < channelTitles.Count; i++)
            {

                viewsPerChannelViewModels.Add(new ViewsPerChannelViewModel()
                {
                    ChannelName = channelTitles[i],
                    NumVideos = numVideosWatched[i]
                });

            }

            viewsPerChannelViewModels = viewsPerChannelViewModels.OrderByDescending(vm => vm.NumVideos).ToList();
            using (StreamWriter sw = new StreamWriter(UriHelper.CALCULATIONS_OUTPUT_DIR + "/Views Per Channel - " + DateTime.Now.ToFileTime() + ".csv"))
            {
                foreach (var viewModel in viewsPerChannelViewModels)
                {
                    sw.WriteLine(viewModel.ChannelName + ";" + viewModel.NumVideos);
                }
            }

            return viewsPerChannelViewModels;
        }

        // Calculate most viewed channel
        public List<TimePerChannelViewModel> GetMostTimeChannel()
        {
            var channelTitles = new List<string>();
            var timeWatched = new List<TimeSpan>();
            foreach (var video in HistoryVideos)
            {
                channelTitles.Add(VideoViewModelsDict[video.GetVideoID()].ChannelTitle);
            }

            channelTitles = channelTitles.Distinct().ToList();
            foreach (var channelTitle in channelTitles)
            {
                TimeSpan time = new TimeSpan(0, 0, 0);
                foreach (HistoryVideo historyVideo in HistoryVideos)
                {
                    if (VideoViewModelsDict[historyVideo.GetVideoID()].ChannelTitle == channelTitle)
                    {
                        time = time.Add(VideoViewModelsDict[historyVideo.GetVideoID()].GetDuration());
                    }
                }
                timeWatched.Add(time);
                time = new TimeSpan(0, 0, 0);
            }

            List<TimePerChannelViewModel> timePerChannelViewModels = new List<TimePerChannelViewModel>();
            for (int i = 0; i < channelTitles.Count; i++)
            {

                timePerChannelViewModels.Add(new TimePerChannelViewModel()
                {
                    ChannelName = channelTitles[i],
                    TimeWatched = timeWatched[i]
                });

            }

            timePerChannelViewModels = timePerChannelViewModels.OrderByDescending(vm => vm.TimeWatched).ToList();
            using (StreamWriter sw = new StreamWriter(UriHelper.CALCULATIONS_OUTPUT_DIR + "/Time Watched Per Channel - " + DateTime.Now.ToFileTime() + ".csv"))
            {
                foreach (var viewModel in timePerChannelViewModels)
                {
                    //sw.WriteLine(viewModel.ChannelName + ";" + viewModel.TimeWatched.TotalHours + " Hours " + viewModel.TimeWatched.Minutes + " Minutes and " + viewModel.TimeWatched.Seconds + " Seconds");
                    sw.WriteLine(viewModel.ChannelName + ";" + Math.Round(viewModel.TimeWatched.TotalHours) + " Hours (" + Math.Round(viewModel.TimeWatched.TotalHours * 60) + " minutes)");

                }
            }

            return timePerChannelViewModels;
        }


    }
}
