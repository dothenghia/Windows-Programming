using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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

        private void Open_Click(object sender, RoutedEventArgs e) { OpenProcess(); }
        private void Save_Click(object sender, RoutedEventArgs e) { SaveProcess(); }
        private void Restart_Click(object sender, RoutedEventArgs e) { RestartProcess(); }
        private void ChangeSize_Click(object sender, RoutedEventArgs e) { ChangeSizeProcess(); }
        private void Exit_Click(object sender, RoutedEventArgs e) { ExitProcess(); }

        private void OpenProcess()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                LoadGameDataFromFile(filePath);
            }
        }

        private void LoadGameDataFromFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length >= 3)
                {
                    string[] size = lines[0].Split(' ');
                    int rows = int.Parse(size[0]);
                    int cols = int.Parse(size[1]);

                    int currentPlayer = int.Parse(lines[1]);

                    game.RenderGrid(rows, cols);

                    for (int i = 0; i < rows; i++)
                    {
                        string[] cells = lines[i + 2].Split(' ');
                        for (int j = 0; j < cols; j++)
                        {
                            if (cells[j] == "1")
                            {
                                game.gridCells[i, j] = "X";
                            }
                            else if (cells[j] == "2")
                            {
                                game.gridCells[i, j] = "O";
                            }
                        }
                    }

                    game.isXTurn = (currentPlayer == 1);

                    game.RenderGrid(rows, cols);
                }
                else
                {
                    MessageBox.Show("Invalid game file format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while opening the game file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void SaveProcess()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                SaveGameDataToFile(filePath);
            }
        }

        private void SaveGameDataToFile(string filePath)
        {
            int rows = game.gridCells.GetLength(0);
            int cols = game.gridCells.GetLength(1);
            int currentPlayer = game.isXTurn ? 1 : 0;
            string[,] gridData = game.gridCells;

            StringBuilder contentBuilder = new StringBuilder();
            contentBuilder.AppendLine($"{rows} {cols}");
            contentBuilder.AppendLine(currentPlayer.ToString());
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    contentBuilder.Append($"{gridData[i, j]} ");
                }
                contentBuilder.AppendLine();
            }

            File.WriteAllText(filePath, contentBuilder.ToString());

            MessageBox.Show("Game data has been saved successfully.", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void RestartProcess()
        {
            game.ResetGame();
        }
        private void ChangeSizeProcess()
        {
            ChangeSizeDialog changeSizeDialog = new ChangeSizeDialog();
            changeSizeDialog.ShowDialog();

            if (changeSizeDialog.DialogResult == true)
            {
                game.ResetGame();
                game.RenderGrid(changeSizeDialog.Rows, changeSizeDialog.Columns);
            }

        }
        private void ExitProcess()
        {
            Window.GetWindow(this).Close();
        }

    }
}
