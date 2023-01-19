using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MultithreadedWebCrawler.PageLoad
{
    public class PageLoader
    {
        public ObservableCollection<Page> Scanned { get; private set; }

        //уникальные отканированные страницы
        private ISet<Page> scanned;

        private Page root;
        private int maxCount;
        private readonly object locker;

        private Thread loader;

        //событие завершения поиска
        public event Action EndSearch;

        public PageLoader()
        {
            this.scanned = new HashSet<Page>();
            this.Scanned = new ObservableCollection<Page>();
            this.locker = new object();
            BindingOperations.EnableCollectionSynchronization(Scanned, locker);
        }

        public void Start(string startUrl, int pageCount, string searchPattern)
        {
            this.root = new Page(startUrl, searchPattern);
            this.maxCount = pageCount;

            Scanned.Clear();
            scanned.Clear();

            Start();
        }

        public void Stop()
        {
            this.loader.Abort();
            CustomThreadPool.ThreadPool.Instance.CancelAll();

            foreach (var page in Scanned)
                if (page.Status == PageStatus.Loading)
                    page.CancelLoading();
        }

        private bool AddScannedPage(Page page)
        {
            if (this.scanned.Add(page))
            {
                this.Scanned.Add(page);
                return true;
            }

            return false;
        }

        private void Start()
        {
            this.loader = new Thread(() =>
            {
                try
                {
                    var queue = new Queue<Page>();

                    queue.Enqueue(root);
                    AddScannedPage(root);
                    root.LoadPageAsync();

                    while (queue.Count > 0 && scanned.Count < maxCount)
                    {
                        var page = queue.Dequeue();

                        foreach (var child in page.Children)
                            if (scanned.Count < maxCount && AddScannedPage(child))
                            {
                                queue.Enqueue(child);
                                child.LoadPageAsync();
                            }
                    }

                    //ждём загрузки всех страниц, прежде чем сообщать о завершении
                    foreach (var page in scanned)
                        page.WaitForLoad();

                    EndSearch.Invoke();
                }
                catch (ThreadAbortException)
                {
                    EndSearch.Invoke();
                    return;
                }
            });

            this.loader.Start();
        }
    }
}