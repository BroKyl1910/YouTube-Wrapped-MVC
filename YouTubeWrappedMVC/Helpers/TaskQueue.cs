using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace YouTubeWrappedMVC.Helpers
{
    //https://stackoverflow.com/questions/50944058/queuing-tasks-in-asp-net-core
    public class TaskQueue
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public TaskQueue()
        {
            _semaphoreSlim = new SemaphoreSlim(1);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            await _semaphoreSlim.WaitAsync();

            try
            {
                return await taskGenerator();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task Enqueue(Func<Task> taskGenerator)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                await taskGenerator();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
