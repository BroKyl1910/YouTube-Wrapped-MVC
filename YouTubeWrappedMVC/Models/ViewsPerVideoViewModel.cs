using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Models
{
    public class ViewsPerVideoViewModel
    {
        public VideoViewModel VideoViewModel { get; set; }
        public int NumViews { get; set; }
    }
}
