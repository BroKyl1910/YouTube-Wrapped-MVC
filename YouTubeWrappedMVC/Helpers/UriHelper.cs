using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Helpers
{
    public class UriHelper
    {
        public static string PAST_VIDEO_SEARCHES_FILE_URI = "AppData/past-video-searches.csv";
        public static string PAST_CHANNEL_SEARCHES_FILE_URI = "AppData/past-channel-searches.csv";
        public static string CALCULATIONS_OUTPUT_DIR = "AppData/Outputs/";

        public static string JOB_STATUS_DATA_FILE = "AppData/JobStatuses.json";
        public static string PROCESSING_JOB_DATA_FILE = "AppData/ProcessingJobs.json";
    }
}
