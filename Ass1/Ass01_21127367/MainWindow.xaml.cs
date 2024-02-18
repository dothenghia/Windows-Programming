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
using System.Diagnostics; // To use Debug.WriteLine
using System.IO;

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
        // =============== Initialization
        private DriveInfo[] allDrives = DriveInfo.GetDrives();
        private string leftPath = "";
        private string rightPath = "";

        private Stack<string> leftStack = new Stack<string>();
        private Stack<string> rightStack = new Stack<string>();

        private bool isLeftWindowFocused = false;
        private bool isRightWindowFocused = false;

        public MainWindow()
        {
            InitializeComponent();

            foreach (DriveInfo drive in allDrives) {
                Debug.WriteLine(drive.Name);
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
            DirectoryInfo directoryInformation = new DirectoryInfo(@"" + path);
            DirectoryInfo[] subDirectoryList = directoryInformation.GetDirectories();
            FileInfo[] fileList = directoryInformation.GetFiles();

            List<MyFileInfo> listViewItems = new List<MyFileInfo>();
            foreach (DirectoryInfo subDirectory in subDirectoryList)
            {
                listViewItems.Add(new MyFileInfo(subDirectory.Name, "Folder", "", subDirectory.CreationTime.ToString()));
            }
            foreach (FileInfo file in fileList)
            {
                listViewItems.Add(new MyFileInfo(file.Name, file.Extension, convertSize(file.Length), file.CreationTime.ToString()));
            }
            lv.ItemsSource = listViewItems;
        }

        // Convert size of file into Unit ngắn gọn
        private string convertSize(long size)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB" };
            int suffixIndex = 0;

            double doubleSize = size;

            while (doubleSize >= 1024 && suffixIndex < suffixes.Length - 1) {
                doubleSize /= 1024;
                suffixIndex++;
            }

            return $"{doubleSize:0.##} {suffixes[suffixIndex]}";
        }


        // =============== Get Focused Window
        private void leftWindow_GotFocus(object sender, EventArgs e) {
            isLeftWindowFocused = true;
            isRightWindowFocused = false;
            Debug.WriteLine("Left - Focus");
        }
        private void rightWindow_GotFocus(object sender, EventArgs e) {
            isLeftWindowFocused = false;
            isRightWindowFocused = true;
            Debug.WriteLine("Right - Focus");
        }


        // =============== CLICK HANDLERS ===============
        // Resize Button - Click Handler
        private void ResizeButton_Click(object sender, EventArgs e)
        {
            TwoWindows_Grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            TwoWindows_Grid.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
        }

        // List View Item - DoubleClick Handler
        private void ShowItemInformation_DBClick(object sender, EventArgs e)
        {
            if (isLeftWindowFocused) {
                MyFileInfo selectedItem = LeftList_ListView.SelectedItem as MyFileInfo;
                if (selectedItem != null && selectedItem.Type == "Folder") {
                    leftStack.Push(leftPath);

                    if (!leftPath.EndsWith("\\"))
                    {
                        leftPath += "\\";
                        LeftPath_Label.Content += "\\";
                    }
                    LeftPath_Label.Content += selectedItem.Name;
                    leftPath += selectedItem.Name;
                    showDirectoryInformation(leftPath, LeftList_ListView);
                }
                else if (selectedItem != null)
                {
                    string filePath = LeftPath_Label.Content + "\\" + selectedItem.Name;
                    Debug.WriteLine(filePath);
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true,
                    };
                    Process.Start(processStartInfo);
                }
            }
            else if (isRightWindowFocused)
            {
                MyFileInfo selectedItem = RightList_ListView.SelectedItem as MyFileInfo;
                if (selectedItem != null && selectedItem.Type == "Folder") {
                    rightStack.Push(rightPath);

                    if (!rightPath.EndsWith("\\"))
                    {
                        rightPath += "\\";
                        RightPath_Label.Content += "\\";
                    }
                    RightPath_Label.Content += selectedItem.Name;
                    rightPath += selectedItem.Name;
                    showDirectoryInformation(rightPath, RightList_ListView);
                }
                else if (selectedItem != null)
                {
                    string filePath = RightPath_Label.Content + "\\" + selectedItem.Name;
                    Debug.WriteLine(filePath);
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true,
                    };
                    Process.Start(processStartInfo);
                }
            }
        }
        // List View Item - Click Handler + Enter
        private void ShowItemInformation_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (isLeftWindowFocused) {
                MyFileInfo selectedItem = LeftList_ListView.SelectedItem as MyFileInfo;
                if (selectedItem != null && selectedItem.Type == "Folder") {
                    leftStack.Push(leftPath);

                    if (!leftPath.EndsWith("\\"))
                        {
                        leftPath += "\\";
                        LeftPath_Label.Content += "\\";
                    }
                    LeftPath_Label.Content += selectedItem.Name;
                    leftPath += selectedItem.Name;
                    showDirectoryInformation(leftPath, LeftList_ListView);
                }
                else if (selectedItem != null)
                {
                    string filePath = LeftPath_Label.Content + "\\" + selectedItem.Name;
                    Debug.WriteLine(filePath);
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true,
                    };
                    Process.Start(processStartInfo);
                }
            }
            else if (isRightWindowFocused)
            {
                MyFileInfo selectedItem = RightList_ListView.SelectedItem as MyFileInfo;
                if (selectedItem != null && selectedItem.Type == "Folder") {
                    rightStack.Push(rightPath);

                    if (!rightPath.EndsWith("\\"))
                        {
                        rightPath += "\\";
                        RightPath_Label.Content += "\\";
                    }
                    RightPath_Label.Content += selectedItem.Name;
                    rightPath += selectedItem.Name;
                    showDirectoryInformation(rightPath, RightList_ListView);
                }
                else if (selectedItem != null)
                {
                    string filePath = RightPath_Label.Content + "\\" + selectedItem.Name;
                    Debug.WriteLine(filePath);
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true,
                    };
                    Process.Start(processStartInfo);
                }
            }
            }
        }

        // Up Button - Click Handler
        private void UpButton_Click(object sender, EventArgs e)
        {
            string currentPath;
            if (isLeftWindowFocused)
                currentPath = leftPath;
            else if (isRightWindowFocused)
                currentPath = rightPath;
            else
                return; // if no window got focus

            if (Directory.GetParent(currentPath) != null)
            {
                string parentDirectory = Directory.GetParent(currentPath).FullName;

                if (isLeftWindowFocused)
                {
                    leftStack.Push(leftPath);
                    LeftPath_Label.Content = parentDirectory;
                    leftPath = parentDirectory;
                    showDirectoryInformation(parentDirectory, LeftList_ListView);
                }
                else if (isRightWindowFocused)
                {
                    rightStack.Push(rightPath);
                    RightPath_Label.Content = parentDirectory;
                    rightPath = parentDirectory;
                    showDirectoryInformation(parentDirectory, RightList_ListView);
                }
            }
        }
    
        // Back Button - Click Handler
        private void BackButton_Click(object sender, EventArgs e)
        {
            if (isLeftWindowFocused)
            {
                if (leftStack.Count > 0)
                {
                    string previousDirectory = leftStack.Pop();
                    Debug.WriteLine(previousDirectory);
                    LeftPath_Label.Content = previousDirectory;
                    leftPath = previousDirectory;
                    showDirectoryInformation(previousDirectory, LeftList_ListView);
                }
            }
            else if (isRightWindowFocused)
            {
                if (rightStack.Count > 0)
                {
                    string previousDirectory = rightStack.Pop();
                    Debug.WriteLine(previousDirectory);
                    RightPath_Label.Content = previousDirectory;
                    rightPath = previousDirectory;
                    showDirectoryInformation(previousDirectory, RightList_ListView);
                }
            }
        }




    }
}

