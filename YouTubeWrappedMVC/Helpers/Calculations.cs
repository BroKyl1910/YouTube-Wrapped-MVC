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

        public Calculations()
        {
        }

        public Calculations(List<HistoryVideo> historyVideos, Dictionary<string, VideoViewModel> videoViewModelsDict)
        {
            HistoryVideos = historyVideos;
            VideoViewModelsDict = videoViewModelsDict;
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

        // Calculate how many hours the user watched per day of their history
        public List<HoursPerDayViewModel> HoursPerDay()
        {
            var dates = HistoryVideos.Select(v => v.GetTime().Date).Distinct().ToList();
            List<TimeSpan> timeWatched = new List<TimeSpan>();

            foreach (DateTime date in dates)
            {
                var videosOnDay = HistoryVideos.Where(h => h.GetTime().Date == date.Date).ToList();
                TimeSpan timeForDay = new TimeSpan(0, 0, 0);
                foreach (HistoryVideo video in videosOnDay)
                {
                    if (VideoViewModelsDict.ContainsKey(video.GetVideoID()))
                    {
                        VideoViewModel viewModel = VideoViewModelsDict[video.GetVideoID()];
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
        public List<ViewsPerVideoViewModel> MostViewedVideos()
        {
            var videoIds = HistoryVideos.Select(v => v.GetVideoID()).Distinct().ToList();
            var timesWatched = videoIds.Select(id => HistoryVideos.Count(v => v.GetVideoID() == id)).ToList();

            List<ViewsPerVideoViewModel> viewsPerVideoViewModels = new List<ViewsPerVideoViewModel>();
            for (int i = 0; i < videoIds.Count; i++)
            {
                if (VideoViewModelsDict.ContainsKey(videoIds[i]))
                {
                    viewsPerVideoViewModels.Add(new ViewsPerVideoViewModel()
                    {
                        VideoViewModel = VideoViewModelsDict[videoIds[i]],
                        NumViews = timesWatched[i]
                    });
                }
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
        public List<ViewsPerChannelViewModel> MostViewedChannel()
        {
            var channelTitles = new List<string>();
            var numVideosWatched = new List<int>();
            foreach(var video in HistoryVideos)
            {
                if (VideoViewModelsDict.ContainsKey(video.GetVideoID()))
                {
                    channelTitles.Add(VideoViewModelsDict[video.GetVideoID()].ChannelTitle);
                }
            }

            channelTitles = channelTitles.Distinct().ToList();
            foreach(var channelTitle in channelTitles)
            {
                int numVids = 0;
                foreach(HistoryVideo historyVideo in HistoryVideos)
                {
                    if (VideoViewModelsDict.ContainsKey(historyVideo.GetVideoID()))
                    {
                        if(VideoViewModelsDict[historyVideo.GetVideoID()].ChannelTitle == channelTitle)
                        {
                            numVids++;
                        }
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
        public List<TimePerChannelViewModel> MostTimeChannel()
        {
            var channelTitles = new List<string>();
            var timeWatched = new List<TimeSpan>();
            foreach(var video in HistoryVideos)
            {
                if (VideoViewModelsDict.ContainsKey(video.GetVideoID()))
                {
                    channelTitles.Add(VideoViewModelsDict[video.GetVideoID()].ChannelTitle);
                }
            }

            channelTitles = channelTitles.Distinct().ToList();
            foreach(var channelTitle in channelTitles)
            {
                TimeSpan time = new TimeSpan(0, 0, 0);
                foreach(HistoryVideo historyVideo in HistoryVideos)
                {
                    if (VideoViewModelsDict.ContainsKey(historyVideo.GetVideoID()))
                    {
                        if(VideoViewModelsDict[historyVideo.GetVideoID()].ChannelTitle == channelTitle)
                        {
                            time = time.Add(VideoViewModelsDict[historyVideo.GetVideoID()].GetDuration());
                        }
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
                    sw.WriteLine(viewModel.ChannelName + ";" + Math.Round(viewModel.TimeWatched.TotalHours) + " Hours ("+ Math.Round(viewModel.TimeWatched.TotalHours*60)+" minutes)");
                   
                }
            }

            return timePerChannelViewModels;
        }


    }
}
