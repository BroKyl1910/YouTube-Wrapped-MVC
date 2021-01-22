using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models.ApiChannel
{

    public class ApiChannel
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public PageInfo PageInfo { get; set; }
        public Item[] Items { get; set; }
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
        public BrandingSettings BrandingSettings { get; set; }
    }

    public class Snippet
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CustomUrl { get; set; }
        public DateTime PublishedAt { get; set; }
        public Thumbnails Thumbnails { get; set; }
        public Localized Localized { get; set; }
        public string Country { get; set; }
    }

    public class Thumbnails
    {
        public Default Default { get; set; }
        public Medium Medium { get; set; }
        public High High { get; set; }
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

    public class Localized
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class BrandingSettings
    {
        public Channel Channel { get; set; }
        public Image Image { get; set; }
    }

    public class Channel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string DefaultTab { get; set; }
        public bool ModerateComments { get; set; }
        public bool ShowRelatedChannels { get; set; }
        public bool ShowBrowseView { get; set; }
        public string ProfileColor { get; set; }
        public string Country { get; set; }
    }

    public class Image
    {
        public string BannerExternalUrl { get; set; }
    }
}