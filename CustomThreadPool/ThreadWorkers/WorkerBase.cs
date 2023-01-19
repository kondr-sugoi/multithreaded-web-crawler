using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomThreadPool.ThreadWorkers
{
    internal abstract class WorkerBase : IDisposable
    {
        private Thread controlThread;
        private ManualResetEvent stopEvent;
        protected ManualResetEvent workEvent;

        public bool IsStopped { get; protected set; }

        public WorkerBase()
        {
            stopEvent = new ManualResetEvent(false);
            workEvent = new ManualResetEvent(false);
            IsStopped = true;
        }

        public virtual void Start()
        {
            if (IsStopped)
            {
                IsStopped = false;
                stopEvent.Reset();
                controlThread = new Thread(WorkerMethod);
                controlThread.Start();
            }
        }

        public virtual void Stop()
        {
            if (controlThread != null)
            {
                IsStopped = true;
                stopEvent.Set();
                controlThread.Join();
                controlThread = null;
                stopEvent.Reset();
            }
        }

        protected abstract void DoWork();

        protected bool Wait()
        {
            return stopEvent.WaitOne();
        }

        protected bool Wait(TimeSpan interval)
        {
            return stopEvent.WaitOne(interval);
        }
        protected int WaitAny()
        {
            return WaitHandle.WaitAny(new WaitHandle[] { stopEvent, workEvent });
        }

        protected int WaitAny(TimeSpan timeout)
        {
            return WaitHandle.WaitAny(new WaitHandle[] { stopEvent, workEvent }, timeout);
        }

        #region Private nethods

        private void WorkerMethod()
        {
            try
            {
                DoWork();
            }
            catch
            {

            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
                if (stopEvent != null)
                    stopEvent.Close();
                if (workEvent != null)
                    workEvent.Close();
            }
        }

        #endregion
    }
}
