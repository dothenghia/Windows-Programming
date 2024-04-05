using System.Text.RegularExpressions;
using System.Windows;

namespace Ass02_21127367
{
    public partial class ChangeSizeDialog : Window
    {
        public int Rows { get; set; } = 12;
        public int Columns { get; set; } = 12;

        public ChangeSizeDialog()
        {
            InitializeComponent();
            Row_TextBox.Text = Rows.ToString();
            Col_TextBox.Text = Columns.ToString();
        }

        private void RowIncrementButton_Click(object sender, RoutedEventArgs e) {
            Rows++;
            Row_TextBox.Text = Rows.ToString();
        }
        private void RowDecrementButton_Click(object sender, RoutedEventArgs e) {
            Rows--;
            Row_TextBox.Text = Rows.ToString();
        }

        private void ColIncrementButton_Click(object sender, RoutedEventArgs e) {
            Columns++;
            Col_TextBox.Text = Columns.ToString();
        }
        private void ColDecrementButton_Click(object sender, RoutedEventArgs e) {
            Columns--;
            Col_TextBox.Text = Columns.ToString();
        }


        // ====== Apply Button Click Event
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate the input
            Regex regex = new Regex(@"^[6-9]|1[0-9]|20$");
            if (regex.IsMatch(Row_TextBox.Text) && regex.IsMatch(Col_TextBox.Text))
            {
                Rows = int.Parse(Row_TextBox.Text);
                Columns = int.Parse(Col_TextBox.Text);
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Rows and Columns must be between 6 and 20.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
