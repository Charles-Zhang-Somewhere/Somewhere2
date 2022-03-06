using System.Windows;

namespace Somewhere2.WPFApplication.Applets
{
    public partial class Browser : Window
    {
        public Browser(string url)
        {
            InitializeComponent();
            
            WebBrowser.Address = url;
        }
    }
}