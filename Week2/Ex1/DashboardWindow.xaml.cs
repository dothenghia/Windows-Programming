using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Ex1
{
    public partial class DashboardWindow : Window
    {
        private string[] imagePaths = { "images/1.jpg", "images/2.jpg", "images/3.jpg", "images/4.jpg", "images/5.jpg", "images/6.jpg", "images/7.jpg" };
        private string[] quotes = { "There is nothing impossible to they who will try.", "Champions keep playing until they get it right.", "With great power comes great responsibility", "You are your best thing.", "A person who never made a mistake never tried anything new.", "Believe you can and you're halfway there.", "Love yourself first and everything else falls into line.", "Be yourself; everyone else is already taken." };
        private int currentImageIndex = 0;

        public DashboardWindow()
        {
            InitializeComponent();
            ShowImage();
        }

        private void ShowImage()
        {
            if (currentImageIndex >= 0 && currentImageIndex < imagePaths.Length)
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(imagePaths[currentImageIndex], UriKind.Relative));
                imgSlideshow.Source = bitmapImage;
                txtCaption.Text = quotes[currentImageIndex];
            }
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            currentImageIndex = (currentImageIndex - 1 + imagePaths.Length) % imagePaths.Length;
            ShowImage();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            currentImageIndex = (currentImageIndex + 1) % imagePaths.Length;
            ShowImage();
        }
    }
}
