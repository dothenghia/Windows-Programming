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
    public class Book : INotifyPropertyChanged
    {
        private string _title;
        private string _author;
        private string _publishedDate;
        private string _coverPath;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Title"));
            }
        }
        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Author"));
            }
        }
        public string PublishedDate
        {
            get { return _publishedDate; }
            set
            {
                _publishedDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PublishedDate"));
            }
        }
        public string CoverPath 
        { 
            get { return _coverPath; }
            set
            {
                _coverPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CoverPath"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public Book(string title, string author, string publishedDate, string coverPath)
        {
            this.Title = title;
            this.Author = author;
            this.PublishedDate = publishedDate;
            this.CoverPath = coverPath;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Book> _bookList;
        public MainWindow()
        {
            _bookList = new List<Book>();
            InitializeComponent();
            _bookList.Add(new Book("The Hobbit", "J.R.R. Tolkien", "21 Sep 1937", "sample.jpg"));
            _bookList.Add(new Book("The Lord of the Rings", "J.R.R. Tolkien", "29 Jul 1954", "intuition.jpg"));
            _bookList.Add(new Book("The Catcher in the Rye", "J.D. Salinger", "16 Jul 1951", "spring.jpg"));

            BookListView.ItemsSource = _bookList;
        }

        private void UpdateABook(object sender, RoutedEventArgs e)
        {
           if (BookListView.SelectedIndex != -1)
            {
                Window editWindows = new EditInfo(_bookList[BookListView.SelectedIndex]);
                editWindows.ShowDialog();
                _bookList[BookListView.SelectedIndex] = ((EditInfo)editWindows).returnData;
                BookListView.Items.Refresh();
            }
        }

        private void AddABook(object sender, RoutedEventArgs e)
        {
            Window editWindows = new EditInfo(new Book("", "", "", ""));
            editWindows.ShowDialog();
            Book res = ((EditInfo)editWindows).returnData;
            if (res !=null && res.Title != "")
            {
                _bookList.Add(res);
            }
            BookListView.Items.Refresh();
        }

        private void DeleteABook(object sender, RoutedEventArgs e)
        {
            if (BookListView.SelectedIndex != -1)
            {
                _bookList.RemoveAt(BookListView.SelectedIndex);
                BookListView.Items.Refresh();
            }
        }
    }
}