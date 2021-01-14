using System;
using System.Collections.Generic;
using System.Text;

namespace YouTubeWrappedMVC.Models
{
    public class ApiVideo
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public Item[] Items { get; set; }
        public PageInfo PageInfo { get; set; }
    }

    public class PageInfo
    {
        public int TotalResults { get; set; }
        public int ResultsPerPage { get; set; }
    }

    public class Item
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public string Id { get; set; }
        public Snippet Snippet { get; set; }
        public ContentDetails ContentDetails { get; set; }
    }

    public class Snippet
    {
        public DateTime PublishedAt { get; set; }
        public string ChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Thumbnails Thumbnails { get; set; }
        public string ChannelTitle { get; set; }
        public string[] Tags { get; set; }
        public string CategoryId { get; set; }
        public string LiveBroadcastContent { get; set; }
        public Localized Localized { get; set; }
        public string DefaultAudioLanguage { get; set; }
    }

    public class Thumbnails
    {
        public Default Default { get; set; }
        public Medium Medium { get; set; }
        public High High { get; set; }
        public Standard Standard { get; set; }
        public MaxRes Maxres { get; set; }
    }

    public class Default
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Medium
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class High
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Standard
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class MaxRes
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Localized
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class ContentDetails
    {
        public string Duration { get; set; }
        public string Dimension { get; set; }
        public string Definition { get; set; }
        public string Caption { get; set; }
        public bool LicensedContent { get; set; }
    }

}
