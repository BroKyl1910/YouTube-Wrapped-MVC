using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        //[ValidateAntiForgeryToken]
        public string Index([FromServices] TaskQueue taskQueue, [FromForm] IFormFile file, string email)
        {
            try
            {
                string json = ReadTakeoutFile(file);
                string jobId = AddJobToDB();

                _ = taskQueue.Enqueue(async () => await new ProcessYouTubeData().Initialise(jobId, json, email));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new
                {
                    error = ex.Message
                });
            }

            return JsonConvert.SerializeObject(new
            {
                message = "Ok"
            });
        }

        private string ReadTakeoutFile(IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }
            return result.ToString();
        }

        private string AddJobToDB()
        {
            string jobId = Guid.NewGuid().ToString();
            //Add to db

            return jobId;
        }
    }
}