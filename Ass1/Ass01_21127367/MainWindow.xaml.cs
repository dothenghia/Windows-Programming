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

/* Note:
 * - Tên Element Tag : <TênChứcNăng>_<TênElement>
 * - Tên Event Handler : <TênVịTríElement>_<TênEvent>
 * - Private: camelCase
 * - Public: PascalCase
*/

namespace Ass01_21127367
{
    // =============== DEFINE CLASS OF FILE INFORMATION ===============
    class MyFileInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Date { get; set; }

        public MyFileInfo(string Name, string Type, string Size, string Date) {
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
        private string leftPath = "";
        private string rightPath = "";

        // =============== Initialization
        public MainWindow()
        {
            InitializeComponent();

            foreach (DriveInfo drive in allDrives) {
                LeftDrives_ComboBox.Items.Add(drive.Name);
                RightDrives_ComboBox.Items.Add(drive.Name);
            }
        }


        // =============== Responsive size of Grid Columns of LEFT & RIGHT WINDOW
        private void leftListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (LeftList_ListView.View != null && LeftList_ListView.View is GridView grid) {
                grid.Columns[0].Width = LeftWindow_DockPanel.ActualWidth * 4/11;
                grid.Columns[1].Width = LeftWindow_DockPanel.ActualWidth * 2/11;
                grid.Columns[2].Width = LeftWindow_DockPanel.ActualWidth * 2/11;
                grid.Columns[3].Width = LeftWindow_DockPanel.ActualWidth * 3/11;
            }
            if (ToolBar_Border != null) {
                ToolBar_Border.Width = LeftWindow_DockPanel.ActualWidth + RightWindow_DockPanel.ActualWidth;
            }
        }
        private void rightListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (RightList_ListView.View != null && RightList_ListView.View is GridView grid) {
                grid.Columns[0].Width = RightWindow_DockPanel.ActualWidth * 4/11;
                grid.Columns[1].Width = RightWindow_DockPanel.ActualWidth * 2/11;
                grid.Columns[2].Width = RightWindow_DockPanel.ActualWidth * 2/11;
                grid.Columns[3].Width = RightWindow_DockPanel.ActualWidth * 3/11;
            }
            if (ToolBar_Border != null) {
                ToolBar_Border.Width = LeftWindow_DockPanel.ActualWidth + RightWindow_DockPanel.ActualWidth;
            }
        }


        // =============== Selection Handler of LEFT & RIGHT ComboBox
        private void leftComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox DiskComboBox = (ComboBox)sender;
            string selectedPath = DiskComboBox.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedPath))
            {
                LeftPath_Label.Content = selectedPath;
                leftPath = selectedPath;
                showDirectoryInformation(leftPath, LeftList_ListView);
            }
        }
        private void rightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox DiskComboBox = (ComboBox)sender;
            string selectedPath = DiskComboBox.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedPath))
            {
                RightPath_Label.Content = selectedPath;
                rightPath = selectedPath;
                showDirectoryInformation(rightPath, RightList_ListView);
            }
        }


        // =============== Show Directory Information
        private void showDirectoryInformation(string path, ListView lv)
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
                item.Add(new MyFileInfo(file.Name, file.Extension, convertSize(file.Length), file.CreationTime.ToString()));
            }
            lv.ItemsSource = item;
        }

        // Convert size of file into Unit ngắn gọn
        private string convertSize(long size)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int suffixIndex = 0;

            double doubleSize = size;

            while (doubleSize >= 1024 && suffixIndex < suffixes.Length - 1) {
                doubleSize /= 1024;
                suffixIndex++;
            }

            return $"{doubleSize:0.##} {suffixes[suffixIndex]}";
        }


        // =============== CLICK HANDLERS ===============
        // Resize Button - Click Handler
        private void Resize_Click(object sender, EventArgs e)
        {
            TwoWindows_Grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            TwoWindows_Grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
        }




    }
}

