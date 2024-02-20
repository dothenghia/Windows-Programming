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

/* Name Convention :
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
        public ImageSource Icon { get; set; }

        public MyFileInfo(string Name, string Type, string Size, string Date, ImageSource Icon)
        {
            this.Name = Name;
            this.Type = Type;
            this.Size = Size;
            this.Date = Date;
            this.Icon = Icon;
        }
    }


    // =============== MainWindow CODE ===============
    public partial class MainWindow : Window
    {
        // =============== Initialization
        private DriveInfo[] allDrives = DriveInfo.GetDrives();
        private string leftPath = "";
        private string rightPath = "";

        private Stack<string> leftStack = new Stack<string>(); // Backward Stacks
        private Stack<string> rightStack = new Stack<string>();

        private Stack<string> leftForwardStack = new Stack<string>(); // Forward Stacks
        private Stack<string> rightForwardStack = new Stack<string>();

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

        // =============== Get File Icon
        private ImageSource GetFileIcon(string filePath)
        {
            if (File.Exists(filePath)) {
                return ShellIcon.GetSmallIcon(filePath);
            }
            return ShellIcon.GetSmallFolderIcon();
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
            DirectoryInfo directoryInformation = new DirectoryInfo(path);
            DirectoryInfo[] subDirectoryList = directoryInformation.GetDirectories();
            FileInfo[] fileList = directoryInformation.GetFiles();

            List<MyFileInfo> listViewItems = new List<MyFileInfo>();
            foreach (DirectoryInfo subDirectory in subDirectoryList)
            {
                listViewItems.Add(new MyFileInfo(subDirectory.Name, "Folder", "", subDirectory.LastWriteTime.ToString(), GetFileIcon(subDirectory.FullName)));
            }
            foreach (FileInfo file in fileList)
            {
                listViewItems.Add(new MyFileInfo(file.Name, file.Extension, convertSize(file.Length), file.LastWriteTime.ToString(), GetFileIcon(file.FullName)));
            }
            lv.ItemsSource = listViewItems;
        }

        // Convert size of file into suitable Unit
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
                    leftForwardStack.Clear();

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
                    ProcessStartInfo processStartInfo = new ProcessStartInfo // Open File
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
                    rightForwardStack.Clear();

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
                    ProcessStartInfo processStartInfo = new ProcessStartInfo // Open File
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
                    leftForwardStack.Clear();

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
                    rightForwardStack.Clear();

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
                    leftForwardStack.Clear();
                    LeftPath_Label.Content = parentDirectory;
                    leftPath = parentDirectory;
                    showDirectoryInformation(parentDirectory, LeftList_ListView);
                }
                else if (isRightWindowFocused)
                {
                    rightStack.Push(rightPath);
                    rightForwardStack.Clear();
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
                    leftForwardStack.Push(leftPath);
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
                    rightForwardStack.Push(rightPath);
                    RightPath_Label.Content = previousDirectory;
                    rightPath = previousDirectory;
                    showDirectoryInformation(previousDirectory, RightList_ListView);
                }
            }
        }

        // Forward Button - Click Handler
        private void ForwardButton_Click(object sender, EventArgs e)
        {
            if (isLeftWindowFocused)
            {
                if (leftForwardStack.Count > 0)
                {
                    string nextDirectory = leftForwardStack.Pop();
                    Debug.WriteLine(nextDirectory);
                    leftStack.Push(leftPath);
                    LeftPath_Label.Content = nextDirectory;
                    leftPath = nextDirectory;
                    showDirectoryInformation(nextDirectory, LeftList_ListView);
                }
            }
            else if (isRightWindowFocused)
            {
                if (rightForwardStack.Count > 0)
                {
                    string nextDirectory = rightForwardStack.Pop();
                    Debug.WriteLine(nextDirectory);
                    rightStack.Push(rightPath);
                    RightPath_Label.Content = nextDirectory;
                    rightPath = nextDirectory;
                    showDirectoryInformation(nextDirectory, RightList_ListView);
                }
            }
        }

        // Check if the source path is within the destination path
        private bool IsSourcePathWithinDestination(string sourcePath, string destinationPath)
        {
            DirectoryInfo sourceDirInfo = new DirectoryInfo(sourcePath);
            DirectoryInfo destinationDirInfo = new DirectoryInfo(destinationPath);

            // Loop through the parent directories of the destination directory
            while (destinationDirInfo.Parent != null)
            {
                if (sourceDirInfo.FullName.Equals(destinationDirInfo.FullName))
                {
                    return true;
                }
                destinationDirInfo = destinationDirInfo.Parent;
            }
            return false;
        }

        // Copy Button - Click Handler
        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (isLeftWindowFocused)
            {
                MyFileInfo selectedItem = LeftList_ListView.SelectedItem as MyFileInfo;
                if (selectedItem != null)
                {
                    string sourcePath = System.IO.Path.Combine(LeftPath_Label.Content.ToString(), selectedItem.Name);
                    string destinationPath = System.IO.Path.Combine(RightPath_Label.Content.ToString(), selectedItem.Name);

                    if (File.Exists(sourcePath)) // FILE
                    {
                        try
                        {
                            CopyFileWithUniqueName(sourcePath, destinationPath);
                            showDirectoryInformation(rightPath, RightList_ListView);
                            if (leftPath == rightPath)
                            {
                                showDirectoryInformation(leftPath, LeftList_ListView);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error copying file: {ex.Message}");
                            MessageBox.Show($"Error copying file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else if (Directory.Exists(sourcePath)) // FOLDER
                    {
                        if (leftPath != rightPath) {
                            if (IsSourcePathWithinDestination(sourcePath, destinationPath))
                            {
                                MessageBox.Show("Cannot copy a parent directory into its own subdirectory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }

                        try
                        {
                            CopyDirectoryWithUniqueName(sourcePath, destinationPath);
                            showDirectoryInformation(rightPath, RightList_ListView);
                            if (leftPath == rightPath)
                            {
                                showDirectoryInformation(leftPath, LeftList_ListView);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error copying directory: {ex.Message}");
                            MessageBox.Show($"Error copying directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else if (isRightWindowFocused)
            {
                MyFileInfo selectedItem = RightList_ListView.SelectedItem as MyFileInfo;
                if (selectedItem != null)
                {
                    string sourcePath = System.IO.Path.Combine(RightPath_Label.Content.ToString(), selectedItem.Name);
                    string destinationPath = System.IO.Path.Combine(LeftPath_Label.Content.ToString(), selectedItem.Name);

                    if (File.Exists(sourcePath))
                    {
                        try
                        {
                            CopyFileWithUniqueName(sourcePath, destinationPath);
                            showDirectoryInformation(leftPath, LeftList_ListView);
                            if (leftPath == rightPath)
                            {
                                showDirectoryInformation(rightPath, RightList_ListView);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error copying file: {ex.Message}");
                            MessageBox.Show($"Error copying file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else if (Directory.Exists(sourcePath))
                    {
                        if (leftPath != rightPath) {
                            if (IsSourcePathWithinDestination(sourcePath, destinationPath))
                            {
                                MessageBox.Show("Cannot copy a parent directory into its own subdirectory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                        
                        try
                        {
                            CopyDirectoryWithUniqueName(sourcePath, destinationPath);
                            showDirectoryInformation(leftPath, LeftList_ListView);
                            if (leftPath == rightPath)
                            {
                                showDirectoryInformation(rightPath, RightList_ListView);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error copying directory: {ex.Message}");
                            MessageBox.Show($"Error copying directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        // Copy Directory with unique name (if directory already exists in the destination)
        private void CopyDirectoryWithUniqueName(string sourceDir, string targetDir)
        {
            string newTargetDir = targetDir;

            int count = 1;
            while (Directory.Exists(newTargetDir))
            {
                string newDirectoryName = System.IO.Path.GetFileName(targetDir) + " - Copy";
                if (count > 1)
                    newDirectoryName += $" ({count})";

                newTargetDir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(targetDir), newDirectoryName);
                count++;
            }

            CopyDirectory(sourceDir, newTargetDir);
        }

        // Copy Directory
        private void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir); // Create folder if it doesn't exist

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = System.IO.Path.GetFileName(file);
                string destFile = System.IO.Path.Combine(targetDir, fileName);
                CopyFileWithUniqueName(file, destFile); // Use unique name function for files
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string dirName = System.IO.Path.GetFileName(subDir);
                string destDir = System.IO.Path.Combine(targetDir, dirName);
                CopyDirectory(subDir, destDir); // Recursively copy subdirectories
            }
        }

        // Copy file with a unique name (if file already exists in the destination)
        private void CopyFileWithUniqueName(string sourcePath, string destinationPath)
        {
            string destinationFile = destinationPath;

            int count = 1;
            while (File.Exists(destinationFile))
            {
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(destinationPath);
                string fileExtension = System.IO.Path.GetExtension(destinationPath);
                string newFileName = $"{fileNameWithoutExtension} - Copy ({count}){fileExtension}";

                destinationFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(destinationPath), newFileName);
                count++;
            }

            File.Copy(sourcePath, destinationFile, true);
        }


        // Move Button - Click Handler
        private void MoveButton_Click(object sender, EventArgs e)
        {
            if (isLeftWindowFocused)
            {
                MyFileInfo selectedItem = LeftList_ListView.SelectedItem as MyFileInfo;
                if (selectedItem != null)
                {
                    string sourcePath = System.IO.Path.Combine(LeftPath_Label.Content.ToString(), selectedItem.Name);
                    string destinationPath = System.IO.Path.Combine(RightPath_Label.Content.ToString(), selectedItem.Name);

                    if (File.Exists(sourcePath))
                    {
                        if (File.Exists(destinationPath))
                        {
                            // File exists at destination, ask for confirmation to replace
                            MessageBoxResult result = MessageBox.Show("A file with the same name already exists in the destination folder. Do you want to replace it?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    File.Delete(destinationPath); // Delete existing file at destination
                                    File.Move(sourcePath, destinationPath); // Move file from source to destination
                                    showDirectoryInformation(LeftPath_Label.Content.ToString(), LeftList_ListView);
                                    showDirectoryInformation(RightPath_Label.Content.ToString(), RightList_ListView);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Error moving file: {ex.Message}");
                                    MessageBox.Show($"Error moving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                File.Move(sourcePath, destinationPath); // Move file from source to destination
                                showDirectoryInformation(LeftPath_Label.Content.ToString(), LeftList_ListView);
                                showDirectoryInformation(RightPath_Label.Content.ToString(), RightList_ListView);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error moving file: {ex.Message}");
                                MessageBox.Show($"Error moving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else if (Directory.Exists(sourcePath))
                    {
                        if (Directory.Exists(destinationPath))
                        {
                            // Directory exists at destination, ask for confirmation to replace
                            MessageBoxResult result = MessageBox.Show("A folder with the same name already exists in the destination folder. Do you want to replace it?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    Directory.Delete(destinationPath, true); // Delete existing directory at destination
                                    CopyDirectory(sourcePath, destinationPath); // Copy folder recursively
                                    Directory.Delete(sourcePath, true); // Delete source directory
                                    showDirectoryInformation(LeftPath_Label.Content.ToString(), LeftList_ListView);
                                    showDirectoryInformation(RightPath_Label.Content.ToString(), RightList_ListView);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Error moving directory: {ex.Message}");
                                    MessageBox.Show($"Error moving directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                Directory.Move(sourcePath, destinationPath); // Move folder from source to destination
                                showDirectoryInformation(LeftPath_Label.Content.ToString(), LeftList_ListView);
                                showDirectoryInformation(RightPath_Label.Content.ToString(), RightList_ListView);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error moving directory: {ex.Message}");
                                MessageBox.Show($"Error moving directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            else if (isRightWindowFocused)
            {
                MyFileInfo selectedItem = RightList_ListView.SelectedItem as MyFileInfo;
                if (selectedItem != null)
                {
                    string sourcePath = System.IO.Path.Combine(RightPath_Label.Content.ToString(), selectedItem.Name);
                    string destinationPath = System.IO.Path.Combine(LeftPath_Label.Content.ToString(), selectedItem.Name);

                    if (File.Exists(sourcePath))
                    {
                        if (File.Exists(destinationPath))
                        {
                            // File exists at destination, ask for confirmation to replace
                            MessageBoxResult result = MessageBox.Show("A file with the same name already exists in the destination folder. Do you want to replace it?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    File.Delete(destinationPath); // Delete existing file at destination
                                    File.Move(sourcePath, destinationPath); // Move file from source to destination
                                    showDirectoryInformation(LeftPath_Label.Content.ToString(), LeftList_ListView);
                                    showDirectoryInformation(RightPath_Label.Content.ToString(), RightList_ListView);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Error moving file: {ex.Message}");
                                    MessageBox.Show($"Error moving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                File.Move(sourcePath, destinationPath); // Move file from source to destination
                                showDirectoryInformation(LeftPath_Label.Content.ToString(), LeftList_ListView);
                                showDirectoryInformation(RightPath_Label.Content.ToString(), RightList_ListView);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error moving file: {ex.Message}");
                                MessageBox.Show($"Error moving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else if (Directory.Exists(sourcePath))
                    {
                        if (Directory.Exists(destinationPath))
                        {
                            // Directory exists at destination, ask for confirmation to replace
                            MessageBoxResult result = MessageBox.Show("A folder with the same name already exists in the destination folder. Do you want to replace it?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    Directory.Delete(destinationPath, true); // Delete existing directory at destination
                                    CopyDirectory(sourcePath, destinationPath); // Copy folder recursively
                                    Directory.Delete(sourcePath, true); // Delete source directory
                                    showDirectoryInformation(LeftPath_Label.Content.ToString(), LeftList_ListView);
                                    showDirectoryInformation(RightPath_Label.Content.ToString(), RightList_ListView);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"Error moving directory: {ex.Message}");
                                    MessageBox.Show($"Error moving directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                Directory.Move(sourcePath, destinationPath); // Move folder from source to destination
                                showDirectoryInformation(LeftPath_Label.Content.ToString(), LeftList_ListView);
                                showDirectoryInformation(RightPath_Label.Content.ToString(), RightList_ListView);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error moving directory: {ex.Message}");
                                MessageBox.Show($"Error moving directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
        }


        // Delete Button - Click Handler
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (isLeftWindowFocused)
            {
                MyFileInfo selectedItem = LeftList_ListView.SelectedItem as MyFileInfo;
                if (selectedItem != null)
                {
                    string filePath = System.IO.Path.Combine(LeftPath_Label.Content.ToString(), selectedItem.Name);

                    if (filePath == rightPath)
                    {
                        MessageBox.Show("Cannot delete the folder currently opened in the other window.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    DeleteFileOrDirectory(filePath);
                    showDirectoryInformation(LeftPath_Label.Content.ToString(), LeftList_ListView);
                    showDirectoryInformation(RightPath_Label.Content.ToString(), RightList_ListView);
                }
            }
            else if (isRightWindowFocused)
            {
                MyFileInfo selectedItem = RightList_ListView.SelectedItem as MyFileInfo;
                if (selectedItem != null)
                {
                    string filePath = System.IO.Path.Combine(RightPath_Label.Content.ToString(), selectedItem.Name);

                    if (filePath == leftPath)
                    {
                        MessageBox.Show("Cannot delete the folder currently opened in the other window.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    DeleteFileOrDirectory(filePath);
                    showDirectoryInformation(LeftPath_Label.Content.ToString(), LeftList_ListView);
                    showDirectoryInformation(RightPath_Label.Content.ToString(), RightList_ListView);
                }
            }
        }

        private void DeleteFileOrDirectory(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting file or directory: {ex.Message}");
                MessageBox.Show($"Error deleting file or directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}

