using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using YouTubeWrappedMVC.Helpers;
using YouTubeWrappedMVC.Models;

namespace YouTubeWrappedMVC.Controllers
{
    public class HomeController : Controller
    {

        private static Dictionary<string, VideoViewModel> pastSearchesDict = new Dictionary<string, VideoViewModel>();

        [HttpGet]
        public async Task<IActionResult> Index([FromServices] TaskQueue taskQueue)
        {
            _ = taskQueue.Enqueue(async () => await DoSomething());

            return View();
        }

        private async Task DoSomething()
        {
            System.Diagnostics.Debug.WriteLine("Starting...");
            List<HistoryVideo> historyVideos = GetHistoryFromFile().Take(1000).ToList();
            Dictionary<string, VideoViewModel> videoViewModelsDict = await GetVideosFromApi(historyVideos.Take(1000).ToList());

            System.Diagnostics.Debug.WriteLine("Videos Retrieved");
            System.Diagnostics.Debug.WriteLine("Processing...");

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

            var orderedZip = dates.Zip(timeWatched, (x, y) => new { x, y })
                      .OrderByDescending(pair => pair.x)
                      .ToList();
            dates = orderedZip.Select(pair => pair.x).ToList();
            timeWatched = orderedZip.Select(pair => pair.y).ToList();
            using (StreamWriter sw = new StreamWriter("AppData/minutes-by-date-luke.csv"))
            {
                for (int i = 0; i < dates.Count; i++)
                {
                    sw.WriteLine(dates[i].Date.ToShortDateString() + ";" + timeWatched[i].Hours + " Hours, " + timeWatched[i].Minutes + " Minutes and " + timeWatched[i].Seconds + " Seconds");
                }
            }

            System.Diagnostics.Debug.WriteLine("Complete");
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
                        ApiVideo apiVideo = await GetVideoDataFromApi(id);
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

                using (StreamReader streamReader = new StreamReader("AppData/past-searches.csv"))
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
            using (StreamWriter sw = new StreamWriter("AppData/past-searches.csv", true))
            {
                await sw.WriteLineAsync(VideoViewModel.SerializeObject(viewModel));
            }
        }

        private async Task<ApiVideo> GetVideoDataFromApi(string videoId)
        {
            Uri uri = new Uri(@"https://youtube.googleapis.com/youtube/v3/videos?part=snippet%2CcontentDetails&id=" + videoId + "&key=AIzaSyDYJH4akcKKjhWuxJKbs3dIl_56dk6masM");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            //request.UserAgent = "12345";

            string responseString = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseString = reader.ReadToEnd();
                ApiVideo apiVideo = JsonConvert.DeserializeObject<ApiVideo>(responseString);
                return apiVideo;

            }
        }

        private List<HistoryVideo> GetHistoryFromFile()
        {
            List<HistoryVideo> historyVideos;
            string historyJson = System.IO.File.ReadAllText("AppData/watch-history-luke.json");
            historyVideos = JsonConvert.DeserializeObject<List<HistoryVideo>>(historyJson);

            return historyVideos.Where(h => h.TitleUrl != null).ToList();
        }
    }
}