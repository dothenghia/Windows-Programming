using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManager_BindingData.Models
{
    public class Book : INotifyPropertyChanged
    {
        private string _title;
        private string _author;
        private string _publishedDate;
        private string _coverPath;
        private double _price;

        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Price"));
            }
        }
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

        public Book(string title, string author, string publishedDate, string coverPath, double price)
        {
            this.Title = title;
            this.Author = author;
            this.PublishedDate = publishedDate;
            this.CoverPath = coverPath;
            this.Price = price;
        }
    }
}
