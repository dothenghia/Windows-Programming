using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ass02_21127367
{
    public class Game
    {
        public Canvas mainCanvas;
        public string[,] gridCells;
        public bool isXTurn = true;

        public Game(Canvas Main_Canvas)
        {
            mainCanvas = Main_Canvas;
        }

        public void RenderGrid(int rows, int cols)
        {
            mainCanvas.Children.Clear();
            gridCells = new string[rows, cols];

            double cellWidth = mainCanvas.ActualWidth / cols;
            double cellHeight = mainCanvas.ActualHeight / rows;

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

                    rect.MouseLeftButtonDown += Cell_Click;

                    mainCanvas.Children.Add(rect);

                    gridCells[i, j] = "";
                }
            }
        }


        private void Cell_Click(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;

            if (isXTurn) {
                DrawX(rect);
            }
            else {
                DrawO(rect);
            }

            isXTurn = !isXTurn;
        }

        private void DrawX(Rectangle cell)
        {
            double x1 = Canvas.GetLeft(cell) + (cell.Width * 0.2);
            double y1 = Canvas.GetTop(cell) + (cell.Height * 0.2);
            double x2 = x1 + (cell.Width * 0.6);
            double y2 = y1 + (cell.Width * 0.6);

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

        private void DrawO(Rectangle cell)
        {
            double x = Canvas.GetLeft(cell) + (cell.Width / 2);
            double y = Canvas.GetTop(cell) + (cell.Height / 2);
            double radius = (cell.Width * 0.7) / 2;

            Ellipse ellipse = new Ellipse {
                Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3dbfb0")),
                StrokeThickness = 4,
                Width = radius * 2,
                Height = radius * 2
            };

            Canvas.SetLeft(ellipse, x - radius);
            Canvas.SetTop(ellipse, y - radius);

            mainCanvas.Children.Add(ellipse);
        }

    }
}
