using CustomThreadPool;
using MultithreadedWebCrawler.PageLoad;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultithreadedWebCrawler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PageLoader loader;

        public MainWindow()
        {
            InitializeComponent();

            ThreadPool.Instance.Initialize(3);
            loader = new PageLoader();
            loader.EndSearch += Loader_EndSearch;
            this.Pages.ItemsSource = loader.Scanned;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            int count = 3;
            int.TryParse(this.MaxThreads.Text, out count);

            ThreadPool.Instance.Initialize(count);

            int pageCount = 10;
            int.TryParse(this.URLMaxCount.Text, out pageCount);

            loader.Start(this.URL.Text, pageCount, this.SearchPattern.Text);

            this.StartButton.IsEnabled = false;
            this.StopButton.IsEnabled = true;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            loader.Stop();

            this.StartButton.IsEnabled = true;
            this.StopButton.IsEnabled = false;
        }

        private void Loader_EndSearch()
        {
            Dispatcher.Invoke(() =>
            {
                this.StartButton.IsEnabled = true;
                this.StopButton.IsEnabled = false;
            });
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            Process.Start(link.NavigateUri.AbsoluteUri);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loader.Stop();
            ThreadPool.Instance.Stop();
        }
    }
}
