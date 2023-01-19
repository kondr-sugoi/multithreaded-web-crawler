using CustomThreadPool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MultithreadedWebCrawler.PageLoad
{
    public class Page : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string URL { get; private set; }
        public PageStatus Status { get; set; }
        public string Error { get; private set; }

        //Список дочерних страниц, ссылки на которые найдены в данной странице
        public IList<Page> Children
        {
            get
            {
                load.Wait();
                return children;
            }
        }
        private List<Page> children;

        //Содержание страницы
        public string Body
        {
            get
            {
                load.Wait();
                return body;
            }
        }

        private static string searchPattern;
        private string body;
        private ManualResetEventSlim load;

        public Page(string url, string searchPattern)
            : this(url)
        {
            Page.searchPattern = searchPattern;
        }

        private Page(string url)
        {
            this.URL = url;
            this.load = new ManualResetEventSlim();
        }

        //Запуск асинхронной загрузки страницы
        public void LoadPageAsync()
        {
            RisePropertyChanged("URL");
            this.Status = PageStatus.Loading;
            CustomThreadPool.ThreadPool.Instance.QueueUserWorkItem(Load, OnComplete);
        }

        //Ожидание завершения загрузки
        public void WaitForLoad()
        {
            this.load.Wait();
        }

        //Отмена загрузки
        public void CancelLoading()
        {
            this.Status = PageStatus.Error;
            this.Error = "Load process terminated";
            this.load.Set();

            RisePropertyChanged("Status");
            RisePropertyChanged("Error");
        }

        private string Load()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.Timeout = 5000;

            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(String.Format("Error loading page: {0} - {1}", (int)response.StatusCode, response.StatusCode));

            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        //ожидание колбека
        private void OnComplete(WorkResult<string> result)
        {
            this.children = new List<Page>();

            if (result.Success)
            {
                this.body = result.Result;

                //проверка на наличие искомого текста
                this.Status = Regex.IsMatch(body, Page.searchPattern) ? PageStatus.Found : PageStatus.NotFound;

                //нахождение дочерних страниц
                foreach (Match item in
                    Regex.Matches(body, (@"(http:\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)")))
                {
                    children.Add(new Page(item.Value));
                }
            }
            else
            {
                this.Status = PageStatus.Error;
                this.Error = result.Exception.Message;
                RisePropertyChanged("Error");
            }

            RisePropertyChanged("Status");
            this.load.Set();
        }

        public override string ToString()
        {
            return URL;
        }

        public override int GetHashCode()
        {
            return URL.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Page;

            if (other != null)
                return this.URL == other.URL;

            return false;
        }
    }
}
