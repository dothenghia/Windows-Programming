using BookManager_BindingData.Models;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookManager_BindingData.ViewModels
{
    class BookViewModel
    {
        public ObservableCollection<Book> Books { get; set;}
        public Book selectedBook { get; set; }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand UpdateCommand { get; set; }

        public List<string> Labels {  get; set; }
        public ChartValues<double> Prices;
        public SeriesCollection SeriesCollection { get; set; }
        public BookViewModel() 
        {
            Books = new ObservableCollection<Book>()
            {
                new Book("The Hobbit", "J.R.R. Tolkien", "21 Sep 1937", "sample.jpg", 123),
                new Book("The Lord of the Rings", "J.R.R. Tolkien", "29 Jul 1954", "intuition.jpg", 100),
                new Book("The Catcher in the Rye", "J.D. Salinger", "16 Jul 1951", "spring.jpg", 150)
            };
            Labels = new List<string>();
            Prices = new ChartValues<double>();
            for (int i = 0; i < Books.Count(); i++)
            {
                Labels.Add(Books[i].Title);
                Prices.Add(Books[i].Price);
            }
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = Prices
                }
            };
        selectedBook = Books[0];

            DeleteCommand = new RelayCommand(OnDelete, CanDelete);
            AddCommand = new RelayCommand(OnAdd, CanAdd);
            UpdateCommand = new RelayCommand(OnUpdate, CanUpdate);
        }

        private void OnDelete(object value)
        {
            Books.Remove(selectedBook);
        }
        private bool CanDelete(object value)
        {
            return selectedBook != null;
        }

        private void OnAdd(object value)
        {
            EditWindow newWindow = new EditWindow(new Book("","","","", 0));
            newWindow.ShowDialog();

            Book newBook = newWindow.returnData;
            if (newBook != null)
            {
                Books.Add(newBook);
            }
        }
        private bool CanAdd(object value)
        {
            return true;
        }

        private void OnUpdate(object value)
        {
            EditWindow newWindow = new EditWindow(selectedBook);
            newWindow.ShowDialog();

            selectedBook = newWindow.returnData;
        }
        private bool CanUpdate(object value)
        {
            return selectedBook != null;
        }
    }
}
