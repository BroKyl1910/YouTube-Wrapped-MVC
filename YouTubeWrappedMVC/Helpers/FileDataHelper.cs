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
        public YouTubeProcessingJobStatus GetJobStatus(string jobId);
        public void SaveJobStatus(YouTubeProcessingJobStatus jobStatus);

        public YouTubeProcessingJobData GetProcessingJob(string jobId);
        public void SaveProcessingJob(YouTubeProcessingJobData processingJob);
    }

    public class FileDataHelper : IDatabaseHelper
    {
        public YouTubeProcessingJobStatus GetJobStatus(string jobId)
        {
            List<YouTubeProcessingJobStatus> jobStatuses = ReadJsonDataFromTextFile<List<YouTubeProcessingJobStatus>>(UriHelper.JOB_STATUS_DATA_FILE);
            return jobStatuses.First(js => js.JobId == jobId);
        }

        public YouTubeProcessingJobData GetProcessingJob(string jobId)
        {
            List<YouTubeProcessingJobData> jobs = ReadJsonDataFromTextFile<List<YouTubeProcessingJobData>>(UriHelper.PROCESSING_JOB_DATA_FILE);
            return jobs.First(js => js.JobId == jobId);
        }

        public void SaveProcessingJob(YouTubeProcessingJobData processingJob)
        {
            List<YouTubeProcessingJobData> jobs = ReadJsonDataFromTextFile<List<YouTubeProcessingJobData>>(UriHelper.PROCESSING_JOB_DATA_FILE);
            jobs.Add(processingJob);

            SaveJsonDataToTextFile(UriHelper.PROCESSING_JOB_DATA_FILE, JsonConvert.SerializeObject(jobs));
        }

        public void SaveJobStatus(YouTubeProcessingJobStatus jobStatus)
        {
            List<YouTubeProcessingJobStatus> jobStatuses = ReadJsonDataFromTextFile<List<YouTubeProcessingJobStatus>>(UriHelper.JOB_STATUS_DATA_FILE);
            if(jobStatus.JobStatus == JobStatus.INITIATED)
            {
                // New job
                jobStatuses.Add(jobStatus);
                string json = JsonConvert.SerializeObject(jobStatuses);
                SaveJsonDataToTextFile(UriHelper.JOB_STATUS_DATA_FILE, json);
            }
            else
            {
                // Edit existing job
                var exisiting = jobStatuses.FindIndex(js => js.JobId == jobStatus.JobId);
                jobStatuses[exisiting] = jobStatus;
                string json = JsonConvert.SerializeObject(jobStatuses);
                SaveJsonDataToTextFile(UriHelper.JOB_STATUS_DATA_FILE, json);
            }
        }

        private T ReadJsonDataFromTextFile<T>(string uri) where T : new()
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(uri))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }

            if (result.Length == 0)
                return new T();

            T data = JsonConvert.DeserializeObject<T>(result.ToString());
            return data;
        }

        private void SaveJsonDataToTextFile(string uri, string json)
        {
            using (StreamWriter sw = new StreamWriter(uri))
            {
                sw.WriteLine(json);
            }
        }

    }

    public enum JobStatus
    {
        INITIATED,
        PROCESSING,
        COMPLETED
    }
}

