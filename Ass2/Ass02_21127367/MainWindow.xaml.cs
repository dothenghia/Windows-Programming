using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ass02_21127367
{
    public partial class MainWindow : Window
    {
        public Game game;

        public MainWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            game = new Game(Main_Canvas);
            game.RenderGrid(12, 12); // Default when the program starts
            SizeChanged += MainWindow_SizeChanged;
        }


        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            game.RenderGrid(game.gridCells.GetLength(0), game.gridCells.GetLength(1));
        }


        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                game.MoveSelectedCell("Left");
            }
            else if (e.Key == Key.Right)
            {
                game.MoveSelectedCell("Right");
            }
            else if (e.Key == Key.Up)
            {
                game.MoveSelectedCell("Up");
            }
            else if (e.Key == Key.Down)
            {
                game.MoveSelectedCell("Down");
            }
            else if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                game.SelectCell_PreviewKeyDown();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
            {
                OpenProcess();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                SaveProcess();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.R)
            {
                RestartProcess();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                ChangeSizeProcess();
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.F4)
            {
                ExitProcess();
            }

        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenProcess();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveProcess();
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            RestartProcess();
        }

        private void ChangeSize_Click(object sender, RoutedEventArgs e)
        {
            ChangeSizeProcess();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitProcess();
        }

        private void OpenProcess()
        {
            Debug.WriteLine("Open");
        }
        private void SaveProcess()
        {
            Debug.WriteLine("Save");
        }
        private void RestartProcess()
        {
            game.ResetGame();
        }
        private void ExitProcess()
        {
            Window.GetWindow(this).Close();
        }
        private void ChangeSizeProcess()
        {
            Debug.WriteLine("ChangeSize");
        }

    }
}
