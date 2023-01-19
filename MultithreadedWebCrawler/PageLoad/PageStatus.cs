using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultithreadedWebCrawler.PageLoad
{
    public enum PageStatus
    {
        NotLoaded,
        Loading,
        NotFound,
        Found,
        Error
    }
}
