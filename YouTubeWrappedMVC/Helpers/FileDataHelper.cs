using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeWrappedMVC.Models;

namespace YouTubeWrappedMVC.Helpers
{
    public interface IDatabaseHelper
    {
        public JobStatus GetJobStatus(string jobId);
        public void SetJobStatus(string jobId, JobStatus jobStatus);

        public YouTubeProcessingJob GetProcessingJob(string jobId);
        public void SaveProcessingJob(YouTubeProcessingJob processingJob);
    }

    public class FileDataHelper : IDatabaseHelper
    {
        public JobStatus GetJobStatus(string jobId)
        {
            throw new NotImplementedException();
        }

        public YouTubeProcessingJob GetProcessingJob(string jobId)
        {
            throw new NotImplementedException();
        }

        public void SaveProcessingJob(YouTubeProcessingJob processingJob)
        {
            throw new NotImplementedException();
        }

        public void SetJobStatus(string jobId, JobStatus jobStatus)
        {
            throw new NotImplementedException();
        }

        private T ReadJsonDataFromTextFile<T>(string uri)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(uri))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }

            T data = JsonConvert.DeserializeObject<T>(result.ToString());
            return data;
        }
    }

    public enum JobStatus
    {
        INITIATED,
        PROCESSING,
        COMPLETED
    }
}

