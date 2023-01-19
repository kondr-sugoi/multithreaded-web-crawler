using CustomThreadPool.ThreadWorkers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomThreadPool
{
    public sealed partial class ThreadPool : ICancellationTokenProvider
    {
        private Queue<IWorkItem> queue;

        private List<ThreadWorker> pool;
        CancellationToken ICancellationTokenProvider.Token => tokenSource.Token;

        private CancellationTokenSource tokenSource;

        private Thread dispatcher;

        private readonly object locker = new object();

        private bool initialized;

        private ThreadPool()
        {
            queue = new Queue<IWorkItem>();
            pool = new List<ThreadWorker>();
            tokenSource = new CancellationTokenSource();

            StartDispatcher();
        }

        public void Initialize(int threadsCount)
        {
            if (threadsCount == this.pool.Count)
                return;

            if (this.initialized)
            {
                ClearWorkers();
            }

            for (int i = 0; i < threadsCount; i++)
            {
                var worker = new ThreadWorker(this);
                worker.Start();
                
                lock (locker)
                {
                    this.pool.Add(worker);
                }
            }

            this.initialized = true;
        }

        public void QueueUserWorkItem<T>(Func<T> work, Action<WorkResult<T>> callBack)
        {
            if (!initialized)
                throw new Exception("Pool is not initialized");

            var workItem = new WorkItem<T>(work, callBack);

            lock (locker)
            {
                this.queue.Enqueue(workItem);
            }
        }

        public void CancelAll()
        {
            lock (locker)
            {
                queue.Clear();
                CancelWorkers();
            }
        }

        public void Stop()
        {
            lock (locker)
            {
                this.dispatcher.Abort();
                StopWorkers();
                ClearWorkers();
                queue.Clear();
            }
        }

        private void StartDispatcher()
        {
            this.dispatcher = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        lock (locker)
                        {
                            if (queue.Count > 0)
                            {
                                var worker = pool.FirstOrDefault(x => x.State == WorkerState.Idle);

                                if (worker != null)
                                {
                                    var workItem = queue.Dequeue();
                                    worker.SetWorkItem(workItem);
                                }
                            }
                        }

                        Thread.Sleep(20);
                    }
                    catch (ThreadAbortException)
                    {
                        return;
                    }
                }
            });

            this.dispatcher.Priority = ThreadPriority.AboveNormal;
            this.dispatcher.Start();
        }

        private void CancelWorkers()
        {
            tokenSource.Cancel();

            foreach (var worker in pool)
            {
                worker.WaitForCompletion();
            }

            tokenSource.Dispose();
            tokenSource = new CancellationTokenSource();
        }

        private void StopWorkers()
        {
            foreach (var worker in pool)
            {
                worker.Stop();
            }
        }

        private void ClearWorkers()
        {
            foreach (var worker in pool)
            {
                worker.Dispose();
            }

            pool.Clear();
        }
    }
}
