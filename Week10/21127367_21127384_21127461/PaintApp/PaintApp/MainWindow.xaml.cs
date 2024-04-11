using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaintApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum MyShape
        {
            Line, Ellipse, Rectangle, Text
        }

        private MyShape currShape;
        Point start;
        Point end;
        private Shape previewShape;

        double thickness = 3;
        SolidColorBrush brush = Brushes.DarkCyan;
        DoubleCollection strokedash = null;
        private bool isShiftKeyPressed = false;

        public MainWindow()
        {
            InitializeComponent();
            previewShape = null;
        }

        private void LineButton_Click(object sender, RoutedEventArgs e)
        {
            currShape = MyShape.Line;
            TextCommand.Visibility = Visibility.Hidden;
        }
        private void EllipseButton_Click(object sender, RoutedEventArgs e)
        {
            currShape = MyShape.Ellipse;
            TextCommand.Visibility = Visibility.Hidden;
        }
        private void RectangleButton_Click(object sender, RoutedEventArgs e)
        {
            currShape = MyShape.Rectangle;
            TextCommand.Visibility = Visibility.Hidden;
        }
        private void TextButton_Click(object sender, RoutedEventArgs e)
        {
            currShape = MyShape.Text;
            TextCommand.Visibility = Visibility.Visible;
        }

        private void MyCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            start = e.GetPosition(this);
            end = e.GetPosition(this);
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                isShiftKeyPressed = true;
            }
            else
            {
                isShiftKeyPressed = false;
            }
            switch (currShape)
            {
                case MyShape.Line:
                    Line myLine = new Line();
                    myLine.Stroke = System.Windows.Media.Brushes.Blue; myLine.StrokeThickness = 1;
                    myLine.X1 = start.X;
                    myLine.Y1 = start.Y;
                    myLine.X2 = start.X;
                    myLine.Y2 = start.Y;
                    myCanvas.Children.Add(myLine);
                    previewShape = myLine;

                    break;

                case MyShape.Ellipse:
                    Ellipse newEllipse = new Ellipse()
                    {
                        Stroke = Brushes.Green,
                        Fill = Brushes.Red,
                        StrokeThickness = 4,
                        Height = 10,
                        Width = 10
                    };

                    if (end.X >= start.X)
                    {
                        // Defines the left part of the ellipse
                        newEllipse.SetValue(Canvas.LeftProperty, start.X);
                        newEllipse.Width = end.X - start.X;
                    }
                    else
                    {
                        newEllipse.SetValue(Canvas.LeftProperty, end.X);
                        newEllipse.Width = start.X - end.X;
                    }

                    if (end.Y >= start.Y)
                    {
                        // Defines the top part of the ellipse
                        newEllipse.SetValue(Canvas.TopProperty, start.Y - 50);
                        newEllipse.Height = end.Y - start.Y;
                    }
                    else
                    {
                        newEllipse.SetValue(Canvas.TopProperty, end.Y - 50);
                        newEllipse.Height = start.Y - end.Y;
                    }
                    previewShape = newEllipse;
                    myCanvas.Children.Add(newEllipse);
                    break;

                case MyShape.Rectangle:
                    Rectangle newRectangle = new Rectangle()
                    {
                        Stroke = Brushes.Orange,
                        Fill = Brushes.Blue,
                        StrokeThickness = 4
                    };

                    if (end.X >= start.X)
                    {
                        // Defines the left part of the rectangle
                        newRectangle.SetValue(Canvas.LeftProperty, start.X);
                        newRectangle.Width = end.X - start.X;
                    }
                    else
                    {
                        newRectangle.SetValue(Canvas.LeftProperty, end.X);
                        newRectangle.Width = start.X - end.X;
                    }

                    if (end.Y >= start.Y)
                    {
                        // Defines the top part of the rectangle
                        newRectangle.SetValue(Canvas.TopProperty, start.Y - 50);
                        newRectangle.Height = end.Y - start.Y;
                    }
                    else
                    {
                        newRectangle.SetValue(Canvas.TopProperty, end.Y - 50);
                        newRectangle.Height = start.Y - end.Y;
                    }
                    previewShape = newRectangle;
                    myCanvas.Children.Add(newRectangle);
                    break;

                case MyShape.Text:
                    Point position = e.GetPosition(myCanvas);

                    TextBlock newTextBlock = new TextBlock
                    {
                        Text = TextBox.Text,
                        Foreground = new SolidColorBrush((Color)TextColorPicker.SelectedColor),
                        FontSize = FontSizeSlider.Value,
                        FontFamily = new FontFamily(FontFamilyComboBox.Text),
                        Background = new SolidColorBrush((Color)BackgroundColorPicker.SelectedColor),
                        TextAlignment = TextAlignment.Left,
                        Width = 200, // Set width and height according to your preference
                        Height = 50,
                        Margin = new Thickness(position.X, position.Y, 0, 0) // Set margin for positioning
                    };

                    myCanvas.Children.Add(newTextBlock);
                    break;
                default:
                    return;
            }
        }

        private void MyCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Draw the correct shape
            isShiftKeyPressed = false;
            switch (currShape)
            {
                case MyShape.Line:
                    DrawLine();
                    break;

                case MyShape.Ellipse:
                    DrawEllipse();
                    break;

                case MyShape.Rectangle:
                    DrawRectangle();
                    break;

                default:
                    return;
            }
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                end = e.GetPosition(this);

                // Adjust dimensions for square or circle if Shift key is pressed
                if (isShiftKeyPressed)
                {
                    double width = Math.Abs(end.X - start.X);
                    double height = Math.Abs(end.Y - start.Y);
                    double maxDimension = Math.Max(width, height);

                    if (currShape == MyShape.Rectangle)
                    {
                        end.X = start.X + (end.X > start.X ? maxDimension : -maxDimension);
                        end.Y = start.Y + (end.Y > start.Y ? maxDimension : -maxDimension);
                    }
                    else if (currShape == MyShape.Ellipse)
                    {
                        end.X = start.X + (end.X > start.X ? maxDimension : -maxDimension);
                        end.Y = start.Y + (end.Y > start.Y ? maxDimension : -maxDimension);
                    }

                }
                switch (currShape)
                {
                    case MyShape.Line:
                        DrawLine();
                        break;

                    case MyShape.Ellipse:
                        DrawEllipse();
                        break;

                    case MyShape.Rectangle:
                        DrawRectangle();
                        break;

                    default:
                        return;

                }
            }
        }
        private void DrawLine()
        {

            if (previewShape != null)
                myCanvas.Children.Remove(previewShape);

            Line newLine = new Line()
            {
                Stroke = brush,
                StrokeDashArray = strokedash,
                StrokeThickness = thickness,
                X1 = start.X,
                Y1 = start.Y - 50,
                X2 = end.X,
                Y2 = end.Y - 50,
            };
            previewShape = newLine;
            myCanvas.Children.Add(newLine);
        }

        // Sets and draws ellipse after mouse is released
        private void DrawEllipse()
        {
            if (previewShape != null)
                myCanvas.Children.Remove(previewShape);
            Ellipse newEllipse = new Ellipse()
            {
                Stroke = brush,
                Fill = brush,
                StrokeDashArray = strokedash,
                StrokeThickness = thickness,
                Height = 10,
                Width = 10
            };

            if (end.X >= start.X)
            {
                // Defines the left part of the ellipse
                newEllipse.SetValue(Canvas.LeftProperty, start.X);
                newEllipse.Width = end.X - start.X;
            }
            else
            {
                newEllipse.SetValue(Canvas.LeftProperty, end.X);
                newEllipse.Width = start.X - end.X;
            }

            if (end.Y >= start.Y)
            {
                // Defines the top part of the ellipse
                newEllipse.SetValue(Canvas.TopProperty, start.Y - 50);
                newEllipse.Height = end.Y - start.Y;
            }
            else
            {
                newEllipse.SetValue(Canvas.TopProperty, end.Y - 50);
                newEllipse.Height = start.Y - end.Y;
            }
            previewShape = newEllipse;
            myCanvas.Children.Add(newEllipse);
        }

        // Sets and draws rectangle after mouse is released
        private void DrawRectangle()
        {
            if (previewShape != null)
                myCanvas.Children.Remove(previewShape);
            Rectangle newRectangle = new Rectangle()
            {
                Stroke = brush,
                Fill = brush,
                StrokeDashArray = strokedash,
                StrokeThickness = thickness,
            };

            if (end.X >= start.X)
            {
                // Defines the left part of the rectangle
                newRectangle.SetValue(Canvas.LeftProperty, start.X);
                newRectangle.Width = end.X - start.X;
            }
            else
            {
                newRectangle.SetValue(Canvas.LeftProperty, end.X);
                newRectangle.Width = start.X - end.X;
            }

            if (end.Y >= start.Y)
            {
                // Defines the top part of the rectangle
                newRectangle.SetValue(Canvas.TopProperty, start.Y - 50);
                newRectangle.Height = end.Y - start.Y;
            }
            else
            {
                newRectangle.SetValue(Canvas.TopProperty, end.Y - 50);
                newRectangle.Height = start.Y - end.Y;
            }
            previewShape = newRectangle;
            myCanvas.Children.Add(newRectangle);
        }
        
        private void PaintColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue.HasValue)
            {
                brush = new SolidColorBrush(e.NewValue.Value);
                // Update the paint color
                if (previewShape != null)
                {
                    if (previewShape is Shape shape)
                    {
                        shape.Stroke = new SolidColorBrush(e.NewValue.Value);
                    }
                }
            }
        }
        private void PenWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            thickness = e.NewValue;
          
        }

        private void StrokeTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                string selectedStrokeType = (StrokeTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (selectedStrokeType == "Dash")
                {
                   strokedash = new DoubleCollection(new double[] { 4, 2 });
                }
                else if (selectedStrokeType == "Dot")
                {
                   strokedash = new DoubleCollection(new double[] { 1, 2 });
                }
                else if (selectedStrokeType == "DashDot")
                {
                    strokedash = new DoubleCollection(new double[] { 4, 2, 1, 2 });
                }
                else
                {
                    strokedash = null; // Solid stroke
                }
        }

    }
}