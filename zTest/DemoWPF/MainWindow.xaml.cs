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
using System.Diagnostics; // Để sử dụng Debug.WriteLine

namespace DemoWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //MessageBox_Demo();
        }

        private static void MessageBox_Demo()
        {
            Debug.WriteLine("========== Xin chao The gioi ==========");

            //MessageBoxResult choice = MessageBox.Show(
            //    "Thông tin của MessageBox",
            //    "Tiêu đề của MessageBox",
            //    MessageBoxButton.YesNo, // Loại nút bấm
            //    MessageBoxImage.Question // Loại Icon
            //    );

            //if (choice == MessageBoxResult.Yes) { Debug.WriteLine("Yes"); }
            //else if (choice == MessageBoxResult.No) { Debug.WriteLine("No"); }
        }

        private void agreeButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Okii");
            //DialogResult = true;
            Debug.WriteLine(sender.ToString());
            Debug.WriteLine(e.ToString());
            DialogResult = false;
        }
    }
}
