using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomThreadPool
{
    internal interface ICancellationTokenProvider
    {
        CancellationToken Token { get; }
    }
}
