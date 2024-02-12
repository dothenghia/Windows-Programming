using System;
using System.Collections.Generic;
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
using System.Diagnostics;
using System.IO;

namespace Week3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary> 
    /// 
    class DisplayFileInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Date { get; set; }
        public DisplayFileInfo(String Name, String Type, String Size, String Date)
        {
            this.Name = Name;
            this.Type = Type;
            this.Size = Size;
            this.Date = Date;
        }
    }
    public partial class MainWindow : Window
    {
        private DriveInfo[] allDrives;
        private String path = "";
        DirectoryInfo curDir;
        List<FileSystemInfo> curItems;
        public MainWindow()
        {
            InitializeComponent();
            allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                DiskComboBox.Items.Add(drive.Name);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox DiskComboBox = (ComboBox)sender;
            PathLabel.Content = DiskComboBox.SelectedItem as string;
            path = DiskComboBox.SelectedItem as string;
            ShowDiskView(path);
        }

        
        private void ShowDiskView(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(@""+path);
            DirectoryInfo[] subDirectoryInfos = directoryInfo.GetDirectories();
            FileInfo[] files = directoryInfo.GetFiles();

            List<DisplayFileInfo> item = new List<DisplayFileInfo>();
            foreach (DirectoryInfo info in subDirectoryInfos)
            {
                item.Add(new DisplayFileInfo(info.Name, "Folder","", info.CreationTime.ToString()));
            }    
            foreach (FileInfo file in files)
            {
                item.Add(new DisplayFileInfo(file.Name, file.Extension, file.Length.ToString(), file.CreationTime.ToString()));
            }
            DiskView.ItemsSource = item;
        }

        private void showSubDir(object sender, MouseButtonEventArgs e)
        {
            DisplayFileInfo selectedItem = DiskView.SelectedItem as DisplayFileInfo;
            if (selectedItem != null && selectedItem.Type == "Folder")
            {
                PathLabel.Content += selectedItem.Name + @"\";
                path = PathLabel.Content as string;
                ShowDiskView(path);
            }
            else if (selectedItem != null)
            {
                string filePath = PathLabel.Content + selectedItem.Name;
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true,
                };
                Process.Start(processStartInfo);
            }
        }

        private void DiskViewColumnChange(object sender, EventArgs e)
        {
            GridView grid = DiskView.View as GridView;
            grid.Columns[0].Width = ActualWidth / 4;
            grid.Columns[1].Width = ActualWidth / 4;
            grid.Columns[2].Width = ActualWidth / 4;
            grid.Columns[3].Width = ActualWidth / 4;
        }
    }
}
