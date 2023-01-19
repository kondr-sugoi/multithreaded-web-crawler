using MultithreadedWebCrawler.PageLoad;
using MultithreadedWebCrawler.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MultithreadedWebCrawler.Converters
{
    class PageStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var status = (PageStatus)value;

            switch (status)
            {
                case PageStatus.Loading: return new RoundProgressBar();
                case PageStatus.Found: return new Image { Source = new BitmapImage(new Uri(@"Icons\Check.png", UriKind.Relative)), ToolTip = status };
                case PageStatus.NotFound: return new Image { Source = new BitmapImage(new Uri(@"Icons\Cross.png", UriKind.Relative)), ToolTip = status };
                case PageStatus.Error: return new Image { Source = new BitmapImage(new Uri(@"Icons\NoEntry.png", UriKind.Relative)), ToolTip = status };

                default: return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
