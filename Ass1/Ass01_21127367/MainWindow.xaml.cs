using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics; // Để sử dụng Debug.WriteLine
using System.IO;

namespace Ass01_21127367
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Responsive size for Grid Column of LEFT & RIGHT WINDOW
        private void LeftListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (LeftList_ListView.View != null && LeftList_ListView.View is GridView grid)
            {
                grid.Columns[0].Width = LeftWindow_DockPanel.ActualWidth / 4;
                grid.Columns[1].Width = LeftWindow_DockPanel.ActualWidth / 4;
                grid.Columns[2].Width = LeftWindow_DockPanel.ActualWidth / 4;
                grid.Columns[3].Width = LeftWindow_DockPanel.ActualWidth / 4;
            }

            if (ToolBar_Border != null)
            {
                ToolBar_Border.Width = LeftWindow_DockPanel.ActualWidth + RightWindow_DockPanel.ActualWidth;
            }
        }
        private void RightListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (RightList_ListView.View != null && RightList_ListView.View is GridView grid)
            {
                grid.Columns[0].Width = RightWindow_DockPanel.ActualWidth / 4;
                grid.Columns[1].Width = RightWindow_DockPanel.ActualWidth / 4;
                grid.Columns[2].Width = RightWindow_DockPanel.ActualWidth / 4;
                grid.Columns[3].Width = RightWindow_DockPanel.ActualWidth / 4;
            }

            if (ToolBar_Border != null)
            {
                ToolBar_Border.Width = LeftWindow_DockPanel.ActualWidth + RightWindow_DockPanel.ActualWidth;
            }
        }

    }
}

