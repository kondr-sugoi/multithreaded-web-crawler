using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomThreadPool
{
    public class WorkResult<T>
    {
        public bool Success { get; internal set; }

        public T Result { get; internal set; }

        public Exception Exception { get; internal set; }

        internal WorkResult(bool success, T result, Exception exception = null)
        {
            this.Success = success;
            this.Result = result;
            this.Exception = exception;
        }
    }
}
