using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MultithreadedWebCrawler.UserControls
{
    /// <summary>
    /// Interaction logic for RoundProgressBar.xaml
    /// </summary>
    public partial class RoundProgressBar : UserControl
    {
        static RoundProgressBar()
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 30 });
        }

        public RoundProgressBar()
        {
            InitializeComponent();
        }
    }
}
