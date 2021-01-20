using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using YouTubeWrappedMVC.Models;

namespace YouTubeWrappedMVC.Helpers
{
    public class ProcessYouTubeData
    {
        private static Dictionary<string, VideoViewModel> pastSearchesDict = new Dictionary<string, VideoViewModel>();

        public async Task Initialise(string jobId, string takeoutDataJson, string emailAddress)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Update DB Status


            System.Diagnostics.Debug.WriteLine("Starting");
            List<HistoryVideo> historyVideos = GetHistoryFromJson(takeoutDataJson).ToList();
            System.Diagnostics.Debug.WriteLine("Fetching video data");
            Dictionary<string, VideoViewModel> videoViewModelsDict = await GetVideosFromApi(historyVideos.Take(5000).ToList());
            System.Diagnostics.Debug.WriteLine("Doing calculations");

            PerformCalculations(historyVideos, videoViewModelsDict);
            System.Diagnostics.Debug.WriteLine("Sending email");
            //await MailJetHelper.SendEmail(emailAddress, "https://localhost:44369/");

            // Update DB Status

            // Save Job to DB


            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            System.Diagnostics.Debug.WriteLine("Completed in "+elapsedTime);
        }

        private YouTubeProcessingJob PerformCalculations(List<HistoryVideo> historyVideos, Dictionary<string, VideoViewModel> videoViewModelsDict)
        {
            Calculations calculations = new Calculations(historyVideos, videoViewModelsDict);

            YouTubeProcessingJob job = new YouTubeProcessingJob()
            {
                HistoryContext = calculations.GetHistoryContext(),
                TotalVideosWatched = calculations.GetTotalVideosWatched(),
                TotalUniqueVideosWatched = calculations.GetTotalUniqueVideosWatched(),
                MostViewedVideo = calculations.GetMostViewedVideos(),
                TotalUniqueChannelsWatched = calculations.GetTotalUniqueChannelsWatched(),
                TimeWatchedPerMonthViewModel = calculations.GetTimeWatchedPerMonth(),
                AverageDailyWatchTime = calculations.GetAverageDailyWatchTime(),
                AverageVideoLength = calculations.GetAverageLengthOfVideo(),
                TimeWatchedPerTimeframe = calculations.GetHoursMostFrequentlyWatched(),
                ViewsPerChannel = calculations.GetMostViewedChannel(),
                TimeWatchedPerChannel = calculations.GetMostTimeChannel(),
            };

            return job;
        }

        private async Task<Dictionary<string, VideoViewModel>> GetVideosFromApi(List<HistoryVideo> historyVideos)
        {
            PopulatePastSearchesDict();
            Dictionary<string, VideoViewModel> apiVideosDict = new Dictionary<string, VideoViewModel>();

            foreach (var historyVideo in historyVideos)
            {
                string id = historyVideo.GetVideoID();

                if (!apiVideosDict.ContainsKey(id))
                {
                    if (pastSearchesDict.ContainsKey(id))
                    {
                        apiVideosDict.Add(id, pastSearchesDict[id]);
                    }
                    else
                    {
                        ApiVideo apiVideo = await YouTubeApiHelper.GetVideoDataFromApi(id);
                        if (apiVideo.Items.Length > 0)
                        {
                            VideoViewModel viewModel = VideoViewModel.FromApiVideo(apiVideo);
                            await WriteVideoViewModelToFile(viewModel);
                            apiVideosDict.Add(id, viewModel);
                            pastSearchesDict.Add(id, viewModel);
                        }

                    }
                }

            }

            return apiVideosDict;
        }

        private void PopulatePastSearchesDict()
        {
            if (pastSearchesDict.Count == 0)
            {

                using (StreamReader streamReader = new StreamReader(UriHelper.PAST_SEARCHES_FILE_URI))
                {
                    string line = streamReader.ReadLine();
                    while (line != null)
                    {
                        VideoViewModel viewModel = VideoViewModel.DeserializeObject(line);
                        pastSearchesDict.Add(viewModel.Id, viewModel);
                        line = streamReader.ReadLine();
                    }
                }
            }


        }

        private async Task WriteVideoViewModelToFile(VideoViewModel viewModel)
        {
            using (StreamWriter sw = new StreamWriter(UriHelper.PAST_SEARCHES_FILE_URI, true))
            {
                await sw.WriteLineAsync(VideoViewModel.SerializeObject(viewModel));
            }
        }



        private List<HistoryVideo> GetHistoryFromJson(string takeoutDataJson)
        {
            List<HistoryVideo> historyVideos;
            historyVideos = JsonConvert.DeserializeObject<List<HistoryVideo>>(takeoutDataJson);

            return historyVideos.Where(h => h.TitleUrl != null).ToList();
        }
    }
}
