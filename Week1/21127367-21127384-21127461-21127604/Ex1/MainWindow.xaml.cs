

using System.Windows;
using System.Windows.Input;

namespace Ex1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            // Kiểm tra thông tin đăng nhập
            if (username == "admin" && password == "qwe3@1")
            {
                // Chuyển hướng đến Dashboard
                DashboardWindow dashboard = new DashboardWindow();
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Sử dụng Alt + U để chuyển đến ô văn bản Username
            if (e.SystemKey == Key.U && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                txtUsername.Focus();
            }

            // Sử dụng Alt + P để chuyển đến ô văn bản Password
            if (e.SystemKey == Key.P && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                txtPassword.Focus();
            }
        }
    }
}

