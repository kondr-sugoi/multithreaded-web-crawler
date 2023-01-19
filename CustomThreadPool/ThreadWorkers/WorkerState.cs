using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomThreadPool.ThreadWorkers
{
    internal enum WorkerState
    {
        Initialized,
        Idle,
        WaitingToRun,
        Running,
        Stopped
    }
}
