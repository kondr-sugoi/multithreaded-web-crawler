using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomThreadPool.ThreadWorkers
{
    internal class ThreadWorker : WorkerBase
    {
        public WorkerState State { get; private set; }

        private IWorkItem workItem;
        private ManualResetEvent doneEvent;
        private ICancellationTokenProvider tokenProvider;

        public ThreadWorker(ICancellationTokenProvider tokenProvider)
        {
            this.State = WorkerState.Initialized;
            this.doneEvent = new ManualResetEvent(true);
            this.tokenProvider = tokenProvider;
        }

        public override void Start()
        {
            base.Start();
            this.State = WorkerState.Idle;
        }

        public override void Stop()
        {
            this.State = WorkerState.Stopped;
            base.Stop();
        }

        public void SetWorkItem(IWorkItem workItem)
        {
            if (workItem != null)
            {
                this.workItem = workItem;
                this.State = WorkerState.WaitingToRun;
                workEvent.Set();
            }
        }

        public void WaitForCompletion()
        {
            doneEvent.WaitOne();
        }

        protected override void DoWork()
        {
            while (!IsStopped)
            {
                if (WaitAny() == 1)
                {
                    ProceedWorkItem();
                    workEvent.Reset();
                }
            }
        }

        private void ProceedWorkItem()
        {
            doneEvent.Reset();
            this.State = WorkerState.Running;

            workItem.TryDoWork();

            if (tokenProvider.Token.IsCancellationRequested == false)
            {
                workItem.DoCallBack();
            }
            
            this.State = WorkerState.Idle;
            doneEvent.Set();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (doneEvent != null)
                    doneEvent.Close();
            }

            base.Dispose(disposing);
        }
    }
}
