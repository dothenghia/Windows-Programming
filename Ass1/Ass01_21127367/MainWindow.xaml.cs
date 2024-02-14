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
using System.Diagnostics; // To use Debug.WriteLine
using System.IO;
using System;

namespace Ass01_21127367
{
    // =============== DEFINE CLASS OF FILE INFORMATION ===============
    class MyFileInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Date { get; set; }

        public MyFileInfo(String Name, String Type, String Size, String Date) {
            this.Name = Name;
            this.Type = Type;
            this.Size = Size;
            this.Date = Date;
        }
    }

    // =============== MainWindow CODE ===============
    public partial class MainWindow : Window
    {
        private DriveInfo[] allDrives = DriveInfo.GetDrives();
        private String leftPath = "";
        private String rightPath = "";

        // Initialization
        public MainWindow()
        {
            InitializeComponent();

            foreach (DriveInfo drive in allDrives) {
                LeftDrives_ComboBox.Items.Add(drive.Name);
                RightDrives_ComboBox.Items.Add(drive.Name);
            }
        }

        // Responsive size for Grid Column of LEFT & RIGHT WINDOW
        private void LeftListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (LeftList_ListView.View != null && LeftList_ListView.View is GridView grid) {
                grid.Columns[0].Width = LeftWindow_DockPanel.ActualWidth / 4;
                grid.Columns[1].Width = LeftWindow_DockPanel.ActualWidth / 4;
                grid.Columns[2].Width = LeftWindow_DockPanel.ActualWidth / 4;
                grid.Columns[3].Width = LeftWindow_DockPanel.ActualWidth / 4;
            }
            if (ToolBar_Border != null) {
                ToolBar_Border.Width = LeftWindow_DockPanel.ActualWidth + RightWindow_DockPanel.ActualWidth;
            }
        }
        private void RightListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (RightList_ListView.View != null && RightList_ListView.View is GridView grid) {
                grid.Columns[0].Width = RightWindow_DockPanel.ActualWidth / 4;
                grid.Columns[1].Width = RightWindow_DockPanel.ActualWidth / 4;
                grid.Columns[2].Width = RightWindow_DockPanel.ActualWidth / 4;
                grid.Columns[3].Width = RightWindow_DockPanel.ActualWidth / 4;
            }
            if (ToolBar_Border != null) {
                ToolBar_Border.Width = LeftWindow_DockPanel.ActualWidth + RightWindow_DockPanel.ActualWidth;
            }
        }

        // Selection Handler of LEFT & RIGHT ComboBox
        private void LeftComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox DiskComboBox = (ComboBox)sender;
            LeftPath_Label.Content = DiskComboBox.SelectedItem as string;
            leftPath = DiskComboBox.SelectedItem as string;
            ShowDiskView(leftPath, LeftList_ListView);
        }
        private void RightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox DiskComboBox = (ComboBox)sender;
            RightPath_Label.Content = DiskComboBox.SelectedItem as string;
            rightPath = DiskComboBox.SelectedItem as string;
            ShowDiskView(rightPath, RightList_ListView);
        }

        private void ShowDiskView(string path, ListView lv)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(@"" + path);
            DirectoryInfo[] subDirectoryInfos = directoryInfo.GetDirectories();
            FileInfo[] files = directoryInfo.GetFiles();

            List<MyFileInfo> item = new List<MyFileInfo>();
            foreach (DirectoryInfo info in subDirectoryInfos)
            {
                item.Add(new MyFileInfo(info.Name, "Folder", "", info.CreationTime.ToString()));
            }
            foreach (FileInfo file in files)
            {
                item.Add(new MyFileInfo(file.Name, file.Extension, file.Length.ToString(), file.CreationTime.ToString()));
            }
            lv.ItemsSource = item;
        }
    }
}

