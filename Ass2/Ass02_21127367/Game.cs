using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ass02_21127367
{
    public class Game
    {
        public Canvas mainCanvas;

        public string[,]? gridCells = null; // Store the state of each cell "X" or "O"
        public bool isXTurn = true;

        public int currentRow = 0;
        public int currentCol = 0;
        public Rectangle? selectedCell = null;

        public Game(Canvas Main_Canvas)
        {
            mainCanvas = Main_Canvas;
        }

        // ========== Render Chessboard
        public void RenderGrid(int rows, int cols)
        {
            if (rows <= 0 || cols <= 0 || mainCanvas == null) return;

            // --- For the first time or when the size of the grid changes
            if (gridCells == null || gridCells.GetLength(0) != rows || gridCells.GetLength(1) != cols) {
                gridCells = new string[rows, cols];
                for (int i = 0; i < rows; i++) {
                    for (int j = 0; j < cols; j++) {
                        gridCells[i, j] = "";
                    }
                }
            }

            mainCanvas.Children.Clear();
            double cellWidth = mainCanvas.ActualWidth / cols;
            double cellHeight = mainCanvas.ActualHeight / rows;

            // --- Draw Cells
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = cellWidth,
                        Height = cellHeight,
                        Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD")),
                        StrokeThickness = 2,
                        Fill = Brushes.White
                    };
                    Canvas.SetLeft(rect, j * cellWidth);
                    Canvas.SetTop(rect, i * cellHeight);

                    // --- Add event click for cell
                    rect.MouseLeftButtonDown += SelectCell_Click;

                    mainCanvas.Children.Add(rect);

                    // --- Draw "X" or "O" on the cell if it's already filled
                    if (gridCells[i, j] == "X") {
                        DrawX(rect);
                    } else if (gridCells[i, j] == "O") {
                        DrawO(rect);
                    }
                }
            }

            // --- Update the selected cell
            RenderSelectedCell();
        }

        // ========== Event Click for Cell
        private void SelectCell_Click(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;

            // --- Calculate the row and column index of the clicked cell
            int rowIndex = (int)(Canvas.GetTop(rect) / (mainCanvas.ActualHeight / gridCells.GetLength(0)));
            int colIndex = (int)(Canvas.GetLeft(rect) / (mainCanvas.ActualWidth / gridCells.GetLength(1)));

            MarkCell(rect, rowIndex, colIndex);
        }

        // ========== Event PreviewKeyDown for Cell (Enter or Space)
        public void SelectCell_PreviewKeyDown()
        {
            Rectangle selectedRect = null;

            // --- Get the selected cell by the current row and column index
            double cellWidth = mainCanvas.ActualWidth / gridCells.GetLength(1);
            double cellHeight = mainCanvas.ActualHeight / gridCells.GetLength(0);
            double left = currentCol * cellWidth;
            double top = currentRow * cellHeight;

            foreach (var child in mainCanvas.Children)
            {
                if (child is Rectangle rect)
                {
                    double rectLeft = Canvas.GetLeft(rect);
                    double rectTop = Canvas.GetTop(rect);
                    if (rectLeft == left && rectTop == top)
                    {
                        selectedRect = rect;
                        break; // Stop the loop after finding the selected rectangle
                    }
                }
            }

            if (selectedRect != null) {
                MarkCell(selectedRect, currentRow, currentCol);
            }
        }

        // ========== Handle marking the cell
        private void MarkCell(Rectangle rect, int rowIndex, int colIndex)
        {
            // --- Check if the cell is already filled
            if (gridCells == null || gridCells[rowIndex, colIndex] != "") return;

            // --- Play the sound
            MainWindow.turnSoundPlayer.Play();

            // --- Handle the clicked cell
            gridCells[rowIndex, colIndex] = isXTurn ? "X" : "O";

            currentCol = colIndex;
            currentRow = rowIndex;
            RenderSelectedCell(); // --- Update the selected cell

            if (isXTurn)
            {
                DrawX(rect);
                isXTurn = false;
                if (CheckWin("X", rowIndex, colIndex)) {
                    MainWindow.endSoundPlayer.Play(); // --- Play the sound

                    MessageBoxResult result = MessageBox.Show("X wins!", "Result", MessageBoxButton.OK);
                    if (result == MessageBoxResult.OK) {
                        ResetGame();
                    }
                }
            }
            else
            {
                DrawO(rect);
                isXTurn = true;
                if (CheckWin("O", rowIndex, colIndex)) {
                    MainWindow.endSoundPlayer.Play(); // --- Play the sound

                    MessageBoxResult result = MessageBox.Show("O wins!", "Result", MessageBoxButton.OK);
                    if (result == MessageBoxResult.OK) {
                        ResetGame();
                    }
                }
            }

            // --- Check Draw game
            if (CheckDraw()) {
                MainWindow.endSoundPlayer.Play(); // --- Play the sound
                MessageBoxResult result = MessageBox.Show("Draw!", "Result", MessageBoxButton.OK);
                if (result == MessageBoxResult.OK) {
                    ResetGame();
                }
            }
        }

        // ========== Draw "X" on the cell
        private void DrawX(Rectangle cell)
        {
            double x1 = Canvas.GetLeft(cell) + (cell.Width * 0.2);
            double y1 = Canvas.GetTop(cell) + (cell.Height * 0.2);
            double x2 = x1 + (cell.Width * 0.6);
            double y2 = y1 + (cell.Height * 0.6);

            Line line1 = new Line {
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e14e51")),
                StrokeThickness = 4,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };

            Line line2 = new Line {
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e14e51")),
                StrokeThickness = 4,
                X1 = x1,
                Y1 = y2,
                X2 = x2,
                Y2 = y1
            };

            mainCanvas.Children.Add(line1);
            mainCanvas.Children.Add(line2);
        }

        // ========== Draw "O" on the cell
        private void DrawO(Rectangle cell)
        {
            double x = Canvas.GetLeft(cell) + (cell.Width / 2);
            double y = Canvas.GetTop(cell) + (cell.Height / 2);
            double radiusX = (cell.Width * 0.7) / 2;
            double radiusY = (cell.Height * 0.7) / 2;

            Ellipse ellipse = new Ellipse {
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3dbfb0")),
                StrokeThickness = 4,
                Width = radiusX * 2,
                Height = radiusY * 2
            };

            Canvas.SetLeft(ellipse, x - radiusX);
            Canvas.SetTop(ellipse, y - radiusY);

            mainCanvas.Children.Add(ellipse);
        }

        // ========== Check Draw game
        private bool CheckDraw()
        {
            for (int i = 0; i < gridCells.GetLength(0); i++)
            {
                for (int j = 0; j < gridCells.GetLength(1); j++)
                {
                    if (gridCells[i, j] == "") return false;
                }
            }
            return true;
        }

        // ========== Check Win game
        private bool CheckWin(string turn, int row, int col)
        {
            // Check Row
            int count = 0;
            for (int c = col - 4; c <= col + 4; c++)
            {
                if (c >= 0 && c < gridCells.GetLength(1) && gridCells[row, c] == turn) {
                    count++;
                    if (count == 5) return true;
                }
                else {
                    count = 0;
                }
            }

            // Check Column
            count = 0;
            for (int r = row - 4; r <= row + 4; r++)
            {
                if (r >= 0 && r < gridCells.GetLength(0) && gridCells[r, col] == turn) {
                    count++;
                    if (count == 5) return true;
                }
                else {
                    count = 0;
                }
            }

            // Check main diagonal
            count = 0;
            for (int d = -4; d <= 4; d++)
            {
                int r = row + d;
                int c = col + d;
                if (r >= 0 && r < gridCells.GetLength(0) && c >= 0 && c < gridCells.GetLength(1) && gridCells[r, c] == turn) {
                    count++;
                    if (count == 5) return true;
                }
                else {
                    count = 0;
                }
            }

            // Check sub diagonal
            count = 0;
            for (int d = -4; d <= 4; d++)
            {
                int r = row + d;
                int c = col - d;
                if (r >= 0 && r < gridCells.GetLength(0) && c >= 0 && c < gridCells.GetLength(1) && gridCells[r, c] == turn) {
                    count++;
                    if (count == 5) return true;
                }
                else {
                    count = 0;
                }
            }

            return false;
        }

        // ========== Reset Game
        public void ResetGame()
        {
            MainWindow.startSoundPlayer.Play();

            for (int i = 0; i < gridCells.GetLength(0); i++)
            {
                for (int j = 0; j < gridCells.GetLength(1); j++)
                {
                    gridCells[i, j] = "";
                }
            }
            isXTurn = true;
            RenderGrid(gridCells.GetLength(0), gridCells.GetLength(1));

            // Reset the selected cell
            currentCol = 0;
            currentRow = 0;
            RenderSelectedCell();
        }

        // ========== Move Selected Cell
        public void MoveSelectedCell(string direction)
        {
            if (direction == "Left") {
                if (currentCol > 0) {
                    currentCol--;
                }
            }
            else if (direction == "Right") {
                if (currentCol < gridCells.GetLength(1) - 1) {
                    currentCol++;
                }
            }
            else if (direction == "Up") {
                if (currentRow > 0) {
                    currentRow--;
                }
            }
            else if (direction == "Down") {
                if (currentRow < gridCells.GetLength(0) - 1) {
                    currentRow++;
                }
            }

            RenderSelectedCell();
        }

        // ========== Render Selected Cell
        private void RenderSelectedCell()
        {
            if (selectedCell != null) {
                mainCanvas.Children.Remove(selectedCell);
            }

            double cellWidth = mainCanvas.ActualWidth / gridCells.GetLength(1);
            double cellHeight = mainCanvas.ActualHeight / gridCells.GetLength(0);

            selectedCell = new Rectangle
            {
                Width = cellWidth,
                Height = cellHeight,
                Stroke = Brushes.Blue,
                StrokeThickness = 3,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft(selectedCell, currentCol * cellWidth);
            Canvas.SetTop(selectedCell, currentRow * cellHeight);

            mainCanvas.Children.Add(selectedCell);
        }
    }
}
