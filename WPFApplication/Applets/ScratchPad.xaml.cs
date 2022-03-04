using System.Windows;
using System.Windows.Input;

namespace Somewhere2.WPFApplication.Applets
{
    public partial class ScratchPad : Window
    {
        public ScratchPad()
        {
            InitializeComponent();

            App = Application.Current as App;
        }

        private App App { get; }

        #region Events
        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void MainWindow_OnDrop(object sender, DragEventArgs e)
        {
            throw new System.NotImplementedException();
        }
        #endregion
        
    }
}