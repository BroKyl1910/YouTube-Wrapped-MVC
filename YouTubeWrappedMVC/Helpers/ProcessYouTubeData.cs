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

        public async Task Initialise(string jobId, string takeoutDataJson, string emailAddress)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Update DB Status

            YouTubeProcessingJobStatus jobStatus = new YouTubeProcessingJobStatus()
            {
                JobId = jobId,
                JobStatus = JobStatus.INITIATED
            };

            FileDataHelper fileDataHelper = new FileDataHelper();
            fileDataHelper.SaveJobStatus(jobStatus);

            System.Diagnostics.Debug.WriteLine("Starting");
            List<HistoryVideo> historyVideos = GetHistoryFromJson(takeoutDataJson).ToList();
            System.Diagnostics.Debug.WriteLine("Fetching video data");

            Dictionary<string, VideoViewModel> videoViewModelsDict = await YouTubeApiHelper.GetInstance().GetVideos(historyVideos.Take(8000).ToList());
            System.Diagnostics.Debug.WriteLine("Doing calculations");

            jobStatus.JobStatus = JobStatus.PROCESSING;
            fileDataHelper.SaveJobStatus(jobStatus);

            YouTubeProcessingJobData processingJobData = await PerformCalculations(jobId, historyVideos, videoViewModelsDict);
            fileDataHelper.SaveProcessingJob(processingJobData);

            jobStatus.JobStatus = JobStatus.COMPLETED;
            fileDataHelper.SaveJobStatus(jobStatus);


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

        private async Task<YouTubeProcessingJobData> PerformCalculations(string jobId, List<HistoryVideo> historyVideos, Dictionary<string, VideoViewModel> videoViewModelsDict)
        {
            Calculations calculations = new Calculations(historyVideos, videoViewModelsDict);

            YouTubeProcessingJobData job = new YouTubeProcessingJobData()
            {
                JobId = jobId,
                HistoryContext = calculations.GetHistoryContext(),
                TotalVideosWatched = calculations.GetTotalVideosWatched(),
                TotalUniqueVideosWatched = calculations.GetTotalUniqueVideosWatched(),
                MostViewedVideo = calculations.GetMostViewedVideos().Take(10).ToList(),
                TotalUniqueChannelsWatched = calculations.GetTotalUniqueChannelsWatched(),
                TimeWatchedPerMonthViewModel = calculations.GetTimeWatchedPerMonth().Take(10).ToList(),
                AverageDailyWatchTime = calculations.GetAverageDailyWatchTime(),
                AverageVideoLength = calculations.GetAverageLengthOfVideo(),
                TimeWatchedPerTimeframe = calculations.GetHoursMostFrequentlyWatched(),
                ViewsPerChannel = await calculations.GetMostViewedChannel(),
                TimeWatchedPerChannel = await calculations.GetMostTimeChannel(),
            };

            return job;
        }



        private List<HistoryVideo> GetHistoryFromJson(string takeoutDataJson)
        {
            List<HistoryVideo> historyVideos;
            historyVideos = JsonConvert.DeserializeObject<List<HistoryVideo>>(takeoutDataJson);

            return historyVideos.Where(h => h.TitleUrl != null).ToList();
        }
    }
}
