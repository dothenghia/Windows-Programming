using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
using BookManager_BindingData.ViewModels;
using LiveCharts;
using LiveCharts.Wpf;

namespace BookManager_BindingData
{
    public class ConvertImageNameToAbsPath : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string imageName = value as string;
            if (imageName != null)
            {
                var currentFolder = (string)AppDomain.CurrentDomain.BaseDirectory;  
                Debug.WriteLine(currentFolder);
                return currentFolder + imageName;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new BookViewModel();
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{

        //    var viewModel = new BookViewModel();
        //    DataContext = viewModel;
        //}

    }
}