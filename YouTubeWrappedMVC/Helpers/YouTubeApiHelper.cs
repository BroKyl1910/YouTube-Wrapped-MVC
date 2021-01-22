using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using YouTubeWrappedMVC.Models;
using YouTubeWrappedMVC.Models.ApiChannelNS;

namespace YouTubeWrappedMVC.Helpers
{
    public class YouTubeApiHelper
    {
        private static YouTubeApiHelper _instance;
        private Dictionary<string, VideoViewModel> PastVideoSearchesDict = new Dictionary<string, VideoViewModel>();
        private Dictionary<string, ChannelViewModel> PastChannelSearchesDict = new Dictionary<string, ChannelViewModel>();

        public static YouTubeApiHelper GetInstance()
        {
            if (_instance == null)
            {
                _instance = new YouTubeApiHelper();
            }

            return _instance;
        }


        public async Task<Dictionary<string, VideoViewModel>> GetVideos(List<HistoryVideo> historyVideos)
        {
            PopulatePastVideoSearchesDict();
            Dictionary<string, VideoViewModel> apiVideosDict = new Dictionary<string, VideoViewModel>();

            foreach (var historyVideo in historyVideos)
            {
                string id = historyVideo.GetVideoID();

                if (!apiVideosDict.ContainsKey(id))
                {
                    if (PastVideoSearchesDict.ContainsKey(id))
                    {
                        apiVideosDict.Add(id, PastVideoSearchesDict[id]);
                    }
                    else
                    {
                        ApiVideo apiVideo = await GetVideoDataFromApi(id);
                        if (apiVideo.Items.Length > 0)
                        {
                            VideoViewModel viewModel = VideoViewModel.FromApiVideo(apiVideo);
                            await WriteVideoViewModelToFile(viewModel);
                            apiVideosDict.Add(id, viewModel);
                            PastVideoSearchesDict.Add(id, viewModel);
                        }

                    }
                }

            }

            return apiVideosDict;
        }

        public async Task<Dictionary<string, ChannelViewModel>> GetChannels(List<string> channelIds)
        {
            PopulatePastChannelSearchesDict();
            Dictionary<string, ChannelViewModel> apiChannelsDict = new Dictionary<string, ChannelViewModel>();

            foreach (var id in channelIds)
            {

                if (!apiChannelsDict.ContainsKey(id))
                {
                    if (PastVideoSearchesDict.ContainsKey(id))
                    {
                        apiChannelsDict.Add(id, PastChannelSearchesDict[id]);
                    }
                    else
                    {
                        ApiChannel apiChannel = await GetChannelDataFromApi(id);
                        if (apiChannel.Items.Length > 0)
                        {
                            ChannelViewModel viewModel = ChannelViewModel.FromApiChannel(apiChannel);
                            await WriteChannelViewModelToFile(viewModel);
                            apiChannelsDict.Add(id, viewModel);
                            PastChannelSearchesDict.Add(id, viewModel);
                        }

                    }
                }

            }

            return apiChannelsDict;
        }

        public async Task<ChannelViewModel> GetChannel(string channelId)
        {
            PopulatePastChannelSearchesDict();


            if (PastChannelSearchesDict.ContainsKey(channelId))
            {
                return PastChannelSearchesDict[channelId];
            }
            else
            {
                ApiChannel apiChannel = await GetChannelDataFromApi(channelId);
                if (apiChannel.Items.Length > 0)
                {
                    ChannelViewModel viewModel = ChannelViewModel.FromApiChannel(apiChannel);
                    PastChannelSearchesDict.Add(channelId, viewModel);
                    await WriteChannelViewModelToFile(viewModel);
                    return viewModel;
                }

                return null;

            }
        }

        private void PopulatePastVideoSearchesDict()
        {
            if (PastVideoSearchesDict.Count == 0)
            {

                using (StreamReader streamReader = new StreamReader(UriHelper.PAST_VIDEO_SEARCHES_FILE_URI))
                {
                    string line = streamReader.ReadLine();
                    while (line != null)
                    {
                        VideoViewModel viewModel = VideoViewModel.DeserializeObject(line);
                        PastVideoSearchesDict.Add(viewModel.Id, viewModel);
                        line = streamReader.ReadLine();
                    }
                }
            }


        }

        private void PopulatePastChannelSearchesDict()
        {
            if (PastChannelSearchesDict.Count == 0)
            {

                using (StreamReader streamReader = new StreamReader(UriHelper.PAST_CHANNEL_SEARCHES_FILE_URI))
                {
                    string line = streamReader.ReadLine();
                    while (line != null)
                    {
                        ChannelViewModel viewModel = ChannelViewModel.DeserializeObject(line);
                        PastChannelSearchesDict.Add(viewModel.Id, viewModel);
                        line = streamReader.ReadLine();
                    }
                }
            }


        }

        private async Task WriteVideoViewModelToFile(VideoViewModel viewModel)
        {
            using (StreamWriter sw = new StreamWriter(UriHelper.PAST_VIDEO_SEARCHES_FILE_URI, true))
            {
                await sw.WriteLineAsync(VideoViewModel.SerializeObject(viewModel));
            }
        }

        private async Task WriteChannelViewModelToFile(ChannelViewModel viewModel)
        {
            using (StreamWriter sw = new StreamWriter(UriHelper.PAST_CHANNEL_SEARCHES_FILE_URI, true))
            {
                await sw.WriteLineAsync(ChannelViewModel.SerializeObject(viewModel));
            }
        }

        public async Task<ApiVideo> GetVideoDataFromApi(string videoId)
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

        public async Task<ApiChannel> GetChannelDataFromApi(string channelId)
        {
            Uri uri = new Uri(@"https://youtube.googleapis.com/youtube/v3/channels?part=snippet,brandingSettings&id=" + channelId + "&key=AIzaSyDYJH4akcKKjhWuxJKbs3dIl_56dk6masM");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            //request.UserAgent = "12345";

            string responseString = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                responseString = reader.ReadToEnd();
                ApiChannel apiChannel = JsonConvert.DeserializeObject<ApiChannel>(responseString);
                return apiChannel;

            }
        }
    }
}
