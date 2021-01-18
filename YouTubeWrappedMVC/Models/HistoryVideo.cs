using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeWrappedMVC.Models
{
    public class HistoryVideo
    {
        public string Header { get; set; }
        public string Title { get; set; }
        public string TitleUrl { get; set; }
        public List<Subtitle> Subtitles { get; set; }
        public string Time { get; set; }
        public List<string> Products { get; set; }

        public HistoryVideo()
        {
        }

        public HistoryVideo(string header, string title, string titleUrl, List<Subtitle> subtitles, string time, List<string> products)
        {
            Header = header;
            Title = title;
            TitleUrl = titleUrl;
            Subtitles = subtitles;
            Time = time;
            Products = products;
        }

        public string GetVideoID()
        {
            int index = TitleUrl.IndexOf("=") + 1;
            string id = TitleUrl.Substring(index);

            return id;
        }
        public DateTime GetTime()
        {
            return DateTime.Parse(Time);
        }
    }

    public class Subtitle
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public Subtitle()
        {
        }

        public Subtitle(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}
