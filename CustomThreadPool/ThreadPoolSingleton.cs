using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomThreadPool
{
    public partial class ThreadPool
    {
        public static ThreadPool Instance { get { return instance; } }

        private static readonly ThreadPool instance = new ThreadPool();

        static ThreadPool()
        { }
    }
}
