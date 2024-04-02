using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        public bool isXTurn = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            if (button.Content != null)
            {
                return;
            }

            if (isXTurn)
            {
                button.Content = DrawX(button.ActualWidth, button.ActualHeight);
                button.Tag = "X";
                isXTurn = false;
                if (CheckWinRow(button.Name, "X") || CheckWinCol(button.Name, "X") || CheckWinDiag(button.Name, "X"))
                {
                    MessageBoxResult result = MessageBox.Show("X wins!", "Result", MessageBoxButton.OK);
                    if (result == MessageBoxResult.OK)
                    {
                        ResetGame();
                    }
                }
            }
            else
            {
                button.Content = DrawO(button.ActualWidth, button.ActualHeight);
                button.Tag = "O";
                isXTurn = true;
                if (CheckWinRow(button.Name, "O") || CheckWinCol(button.Name, "O") || CheckWinDiag(button.Name, "O"))
                {
                    MessageBoxResult result = MessageBox.Show("O wins!", "Result", MessageBoxButton.OK);
                    if (result == MessageBoxResult.OK)
                    {
                        ResetGame();
                    }
                }
            }

            if (CheckDraw())
            {
                MessageBoxResult result = MessageBox.Show("Draw!", "Result", MessageBoxButton.OK);
                if (result == MessageBoxResult.OK)
                {
                    ResetGame();
                }
            }
        }

        private bool CheckDraw()
        {
            foreach (Button button in Board_Grid.Children)
            {
                if (button.Content == null)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckWinRow(string ButtonName, string Turn) 
        {
            string row = ButtonName.Substring(1, 1);

            foreach (Button button in Board_Grid.Children)
            {
                if (button.Name.StartsWith("C" + row) && (string)button.Tag != Turn)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckWinCol(string ButtonName, string Turn)
        {
            string col = ButtonName.Substring(2, 1);

            foreach (Button button in Board_Grid.Children)
            {
                if (button.Name.EndsWith(col) && (string)button.Tag != Turn)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckWinDiag(string ButtonName, string Turn)
        {
            if (ButtonName == "C00" || ButtonName == "C11" || ButtonName == "C22")
            {
                if ((string)C00.Tag == Turn && (string)C11.Tag == Turn && (string)C22.Tag == Turn)
                {
                    return true;
                }
            }
            if (ButtonName == "C02" || ButtonName == "C11" || ButtonName == "C20")
            {
                if ((string)C02.Tag == Turn && (string)C11.Tag == Turn && (string)C20.Tag == Turn)
                {
                    return true;
                }
            }
            return false;
        }

        private Canvas DrawX(double w, double h)
        {
            var line1 = new Line();
            line1.Stroke = Brushes.Red;
            line1.StrokeThickness = 6;
            line1.X1 = -w/2 * 0.6;
            line1.Y1 = -h/ 2 * 0.6;
            line1.X2 = w/ 2 * 0.6;
            line1.Y2 = h/ 2 * 0.6;

            var line2 = new Line();
            line2.Stroke = Brushes.Red;
            line2.StrokeThickness = 6;
            line2.X1 = -w/ 2 * 0.6;
            line2.Y1 = h/ 2 * 0.6;
            line2.X2 = w / 2 * 0.6;
            line2.Y2 = -h/ 2 * 0.6;

            var canvas = new Canvas();
            canvas.Children.Add(line1);
            canvas.Children.Add(line2);
            return canvas;
        }

        private Canvas DrawO(double w, double h)
        {
            var ellipse = new Ellipse();
            ellipse.Stroke = Brushes.Blue;
            ellipse.StrokeThickness = 6;
            ellipse.Width = w * 0.7;
            ellipse.Height = h * 0.7;

            var canvas = new Canvas();
            canvas.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, -w / 2 * 0.7);
            Canvas.SetTop(ellipse, -h / 2 * 0.7);
            return canvas;
        }

        private void ResetGame()
        {
            foreach (Button button in Board_Grid.Children)
            {
                button.Content = null;
                button.Tag = null;
            }

            isXTurn = true;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
    }
}