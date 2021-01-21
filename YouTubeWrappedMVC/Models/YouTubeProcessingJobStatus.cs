using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeWrappedMVC.Helpers;

namespace YouTubeWrappedMVC.Models
{
    public class YouTubeProcessingJobStatus
    {
        public string JobId { get; set; }
        public JobStatus JobStatus { get; set; }
    }
}
