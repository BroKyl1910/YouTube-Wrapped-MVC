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
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index([FromServices] TaskQueue taskQueue)
        {
            _ = taskQueue.Enqueue(async () => await new ProcessYouTubeData().Initialise());

            return View();
        }

       
    }
}