using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTubeWrappedMVC.Models;
using YouTubeWrappedMVC.Helpers;

namespace YouTubeWrappedMVC.Controllers
{
    public class PresentationController : Controller
    {
        public IActionResult Index(string jobId)
        {
            FileDataHelper fileDataHelper = new FileDataHelper();
            YouTubeProcessingJobStatus status = fileDataHelper.GetJobStatus(jobId);
            //if(status.JobStatus == JobStatus.COMPLETED)
            YouTubeProcessingJobData data = fileDataHelper.GetProcessingJob(jobId);
            return View(data);
            
        }
    }
}