using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;
using BookManager_BindingData.Models;

namespace BookManager_BindingData
{
    public class BookNameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Book name cannot be empty.");
            }
            else
            {
                string regex = @"([A-Z][a-z]+\s)+[A-Z][a-z]+";
                if (System.Text.RegularExpressions.Regex.IsMatch(value.ToString(), regex))
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    return new ValidationResult(false, "Book name must be in the format: 'The Alchemist'");
                }
            }
        }
    }

    public class PublishDateValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || value.ToString() == "")
            {
                return new ValidationResult(false, "Publish date cannot be empty.");
            }
            else
            {
                string regex = @"\d{1,2}\s(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s\d{4}";
                if (System.Text.RegularExpressions.Regex.IsMatch(value.ToString(), regex))
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    return new ValidationResult(false, "Publish date must be in the format: '1 Jan 2021'");
                }
            }
        }
    }



    public partial class EditWindow : Window
    {
        public Book returnData { get; set; }

        public EditWindow(Book selectedBook)
        {
            InitializeComponent();
            this.DataContext = selectedBook;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            returnData = (Book)this.DataContext;
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            returnData = (Book)this.DataContext;
            this.Close();
        }


    }
}
