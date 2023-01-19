using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomThreadPool
{
    internal interface IWorkItem
    {
        void TryDoWork();
        void DoCallBack();
    }

    internal class WorkItem<T> : IWorkItem
    {
        private Func<T> work;

        private Action<WorkResult<T>> callBack;

        private WorkResult<T> workResult;

        public WorkItem(Func<T> work, Action<WorkResult<T>> callBack)
        {
            this.work = work;
            this.callBack = callBack;
        }

        public void TryDoWork()
        {
            if (work != null)
            {
                try
                {
                    var result = work.Invoke();
                    workResult = new WorkResult<T>(true, result);
                }
                catch (Exception ex)
                {
                    workResult = new WorkResult<T>(false, default, ex);
                }
            }
        }

        public void DoCallBack()
        {
            if (callBack != null)
            {
                callBack.Invoke(workResult);
            }
        }
    }
}
