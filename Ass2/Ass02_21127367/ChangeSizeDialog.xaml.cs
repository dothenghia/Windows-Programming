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
using System.Windows.Shapes;

namespace Ass02_21127367
{
    public partial class ChangeSizeDialog : Window
    {
        public int Rows { get; set; } = 12;
        public int Columns { get; set; } = 12;

        public ChangeSizeDialog()
        {
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedItem = (Size_ComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (selectedItem != null)
            {
                switch (selectedItem)
                {
                    case "Small (12 x 12)":
                        Rows = 12;
                        Columns = 12;
                        break;
                    case "Medium (16 x 16)":
                        Rows = 16;
                        Columns = 16;
                        break;
                    case "Large (20 x 20)":
                        Rows = 20;
                        Columns = 20;
                        break;
                    case "Horizontal (20 x 30)":
                        Rows = 20;
                        Columns = 30;
                        break;
                }
            }

            this.DialogResult = true; // Close the dialog
        }

    }
}
