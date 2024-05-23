using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyShapes;

namespace MyPaint
{
    public partial class MainWindow : Window
    {
        // ==================== Attributes ====================
        public Point startPoint; // Start point of the shape
        public Point endPoint; // End point of the shape
        Dictionary<string, DoubleCollection> dashCollections = new Dictionary<string, DoubleCollection>(); // List of dash styles

        List<IShape> prototypeShapes = new List<IShape>(); // List of prototype shapes
        List<IShape> drawnShapes = new List<IShape>(); // List of drawn shapes in the MainCanvas
        IShape currentShape; // Current selected shape to draw

        bool isDrawing = false; // Is Drawing - Used to remove the last shape (preview shape) when drawing
        bool isDrawn = false; // Is Drawn - Used to check if the shape is drawn or not (handle MouseUp event is triggered)

        int selectingIndex = -1; // Index of the selecting single shape
        bool isSelecting = false; // Is Selecting - Used to check if the shape is selecting or not (avoid drawing new shape when selecting shape)
        Point dragStartPoint; // Start point when dragging the shape

        bool isTransforming = false; // Is Transforming


        // ==================== Methods ====================
        public MainWindow() {
            InitializeComponent();
            dashCollections["Solid"] = null;
            dashCollections["Dash"] = new DoubleCollection { 4, 4 };
            dashCollections["Dot"] = new DoubleCollection { 1, 2 };
            dashCollections["Dash Dot"] = new DoubleCollection { 4, 2, 1, 2 };
            dashCollections["Dash Dot Dot"] = new DoubleCollection { 4, 2, 1, 2, 1, 2 };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory; // Single configuration
            var fis = new DirectoryInfo(folder).GetFiles("*.dll");

            foreach (var fi in fis) {
                var assembly = Assembly.LoadFrom(fi.FullName); // Get all types in the assembly (in the DLL)
                var types = assembly.GetTypes();

                foreach (var type in types) {
                    if ((type.IsClass) && (typeof(IShape).IsAssignableFrom(type))) {
                        prototypeShapes.Add((IShape)Activator.CreateInstance(type)!); // Add the shape to the list of prototype shapes
                    }
                }
            }

            RenderShapeButtons(prototypeShapes); // Render the shape buttons

            currentShape = prototypeShapes[0]; // Set the default shape
        }

        private void RenderShapeButtons(List<IShape> _prototypeShapes)
        {
            foreach (var shape in _prototypeShapes) {
                var button = new Button();
                button.Tag = shape;
                Style style = this.FindResource("FunctionalBarButtonImage_Style") as Style;
                button.Style = style;
                var image = new Image { Source = new BitmapImage(new Uri(shape.Icon, UriKind.Relative)) };
                button.Content = image;
                button.Click += ShapeButton_Click;
                Shapes_StackPanel.Children.Add(button);
            }
        }


        // ==================== Tool Bar Handlers ====================
        private void OpenButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Open File"); }
        private void SaveButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Save File"); }
        private void UndoButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Undo"); }
        private void RedoButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Redo"); }
        private void ExitButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Exit"); }


        // ==================== Functional Bar Handlers ====================
        // --- Select Tool "Select Area"
        private void SelectButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Select"); }

        // --- Select Shape Button
        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            IShape item = (IShape)(sender as Button)!.Tag;
            currentShape = item;
        }

        // --- Select Color Stroke & Fill
        private void StrokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e) 
        {
            SolidColorBrush color = new SolidColorBrush(e.NewValue.Value);
            foreach (IShape shape in prototypeShapes) { shape.SetStrokeColor(color); }
        }
        
        private void FillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            SolidColorBrush color = new SolidColorBrush(e.NewValue.Value);
            foreach (IShape shape in prototypeShapes) { shape.SetFillColor(color); }
        }

        // --- Select Stroke Thickness
        private void StrokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            StrokeThickness_TextBlock.Text = Math.Ceiling(e.NewValue).ToString();
            foreach (IShape shape in prototypeShapes) { shape.SetStrokeThickness(e.NewValue); }
        }

        // --- Select Stroke Dash
        private void StrokeDashComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)StrokeDash_ComboBox.SelectedItem;
            string selectedTag = selectedItem.Tag.ToString();
            
            foreach (IShape shape in prototypeShapes) {
                if (selectedTag == "Solid") { shape.SetStrokeDashArray(null); }
                else { shape.SetStrokeDashArray(dashCollections[selectedTag]); }
            }
        }

        // --- Layers Button => TEST LOAD FILE
        // REDRAW ALL SHAPES FROM THE LIST
        private void LayersButton_Click(object sender, RoutedEventArgs e) 
        { 
            foreach (IShape shape in drawnShapes)
            {
                Canvas shapeCanvas = shape.Convert();
                shapeCanvas.PreviewMouseDown += ShapeCanvas_PreviewMouseDown;
                shapeCanvas.PreviewMouseMove += ShapeCanvas_PreviewMouseMove;
                shapeCanvas.PreviewMouseUp += ShapeCanvas_PreviewMouseUp;
                Main_Canvas.Children.Add(shapeCanvas);
            }
        }

        // --- Erase Button => TEST CLEAR ALL SHAPES
        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            Main_Canvas.Children.Clear();
            // drawnShapes.Clear();
        }



        // ==================== Main Canvas Handlers ====================
        
        // MainCanvas_PreviewMouseDown will be triggered before ShapeCanvas_PreviewMouseDown
        // Used to remove the selecting shape when clicking outside the shape
        private void MainCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Main Canvas - PreviewMouseDown");
            isSelecting = false;
            RemoveSelectingShape();
            selectingIndex = -1;
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Main Canvas - MouseDown");
            if (isSelecting == false)
            {
                startPoint = e.GetPosition(Main_Canvas);
                currentShape.SetStartPoint(startPoint);
                isDrawing = false;
                isDrawn = false;
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting == false && e.LeftButton == MouseButtonState.Pressed)
            {
                endPoint = e.GetPosition(Main_Canvas);

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    double width = Math.Abs(endPoint.X - startPoint.X);
                    double height = Math.Abs(endPoint.Y - startPoint.Y);
                    double edge = Math.Min(width, height);

                    endPoint.X = startPoint.X + edge * Math.Sign(endPoint.X - startPoint.X);
                    endPoint.Y = startPoint.Y + edge * Math.Sign(endPoint.Y - startPoint.Y);
                }

                currentShape.SetEndPoint(endPoint);

                // Remove the last shape (preview shape)
                if (isDrawing == true) { Main_Canvas.Children.RemoveAt(Main_Canvas.Children.Count - 1); }

                // Then re-draw it
                Canvas shapeCanvas = currentShape.Convert();
                shapeCanvas.PreviewMouseDown += ShapeCanvas_PreviewMouseDown;
                shapeCanvas.PreviewMouseMove += ShapeCanvas_PreviewMouseMove;
                shapeCanvas.PreviewMouseUp += ShapeCanvas_PreviewMouseUp;

                Main_Canvas.Children.Add(shapeCanvas);
                isDrawing = true;
            }
        }

        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing == true && isDrawn == false)
            {
                IShape clone = (IShape)currentShape.Clone();
                drawnShapes.Add(clone);
                isDrawn = true;
            }
        }


        private void ShapeCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas shapeCanvas)
            {
                Debug.WriteLine("Shape Canvas - PreviewMouseDown");
                isSelecting = true;

                DrawSelectingShape(shapeCanvas);
                
                selectingIndex = Main_Canvas.Children.IndexOf(shapeCanvas);

                shapeCanvas.Cursor = Cursors.SizeAll;

                dragStartPoint = e.GetPosition(Main_Canvas);
            }
        }

        private void ShapeCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting == true && e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Canvas shapeCanvas)
                {
                    Point currentPoint = e.GetPosition(Main_Canvas);
                    double offsetX = currentPoint.X - dragStartPoint.X;
                    double offsetY = currentPoint.Y - dragStartPoint.Y;

                    // Move the shape
                    Canvas.SetLeft(shapeCanvas, Canvas.GetLeft(shapeCanvas) + offsetX);
                    Canvas.SetTop(shapeCanvas, Canvas.GetTop(shapeCanvas) + offsetY);

                    // Update the start and end points of the shape
                    dragStartPoint = currentPoint;
                }
            }
        }

        private void ShapeCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isSelecting == true) {
                if (sender is Canvas shapeCanvas)
                {
                    drawnShapes[selectingIndex].SetStartPoint(new Point(Canvas.GetLeft(shapeCanvas), Canvas.GetTop(shapeCanvas)));
                    drawnShapes[selectingIndex].SetEndPoint(new Point(Canvas.GetLeft(shapeCanvas) + shapeCanvas.ActualWidth, Canvas.GetTop(shapeCanvas) + shapeCanvas.ActualHeight));
                }
            }
            // For Line, Rectangle because they are hit the cursor => Do not catch the MainCanvas_MouseUp event
            else if (isDrawing == true && isDrawn == false)
            {
                IShape clone = (IShape)currentShape.Clone();
                drawnShapes.Add(clone);
                isDrawn = true;
            }
        }



        private void RemoveSelectingShape()
        {
            if (selectingIndex != -1)
            {
                Canvas selectingCanvas = (Canvas)Main_Canvas.Children[selectingIndex];
                UIElement[] children = new UIElement[selectingCanvas.Children.Count];
                selectingCanvas.Children.CopyTo(children, 0);

                foreach (UIElement child in children)
                {
                    if (child is Line line && line.Name.StartsWith("Selecting_Line"))
                    {
                        selectingCanvas.Children.Remove(line);
                    }
                }
            }
        }

        private void DrawSelectingShape(Canvas shapeCanvas)
        {
            double width = shapeCanvas.ActualWidth;
            double height = shapeCanvas.ActualHeight;
            double strokeWidth = 2;

            // Top line
            Line topLine = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = width,
                Y2 = 0,
                Stroke = Brushes.Gray,
                Fill = Brushes.LightSlateGray,
                StrokeThickness = strokeWidth,
                StrokeDashArray = new DoubleCollection { 2, 2 },
                Name = "Selecting_Line_Top"
            };

            // Right line
            Line rightLine = new Line
            {
                X1 = width,
                Y1 = 0,
                X2 = width,
                Y2 = height,
                Stroke = Brushes.Gray,
                StrokeThickness = strokeWidth,
                StrokeDashArray = new DoubleCollection { 2, 2 },
                Name = "Selecting_Line_Right"
            };

            // Bottom line
            Line bottomLine = new Line
            {
                X1 = 0,
                Y1 = height,
                X2 = width,
                Y2 = height,
                Stroke = Brushes.Gray,
                StrokeThickness = strokeWidth,
                StrokeDashArray = new DoubleCollection { 2, 2 },
                Name = "Selecting_Line_Bottom"
            };

            // Left line
            Line leftLine = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = height,
                Stroke = Brushes.Gray,
                StrokeThickness = strokeWidth,
                StrokeDashArray = new DoubleCollection { 2, 2 },
                Name = "Selecting_Line_Left"
            };

            // Set cursor for resizing
            topLine.Cursor = Cursors.SizeNS;
            bottomLine.Cursor = Cursors.SizeNS;
            leftLine.Cursor = Cursors.SizeWE;
            rightLine.Cursor = Cursors.SizeWE;

            topLine.PreviewMouseDown += TopLine_PreviewMouseDown;

            shapeCanvas.Children.Add(topLine);
            shapeCanvas.Children.Add(bottomLine);
            shapeCanvas.Children.Add(leftLine);
            shapeCanvas.Children.Add(rightLine);
        }

        private void TopLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Line line)
            {
                //isTransforming = true;
                //dragStartPoint = e.GetPosition(Main_Canvas);
                Debug.WriteLine("Top Line - PreviewMouseDown");
            }
        }

    }
}


            // if (isSelecting == true)
            // {
            //     Debug.WriteLine("Selecting");
            //     if (sender is Canvas shapeCanvas)
            //     {
            //         Point currentPoint = e.GetPosition(Main_Canvas);
            //         // Transform left
            //         if (currentPoint.X <= 5)
            //         {
            //             shapeCanvas.Cursor = Cursors.SizeWE;
            //         }
            //         // Transform right
            //         else if (currentPoint.X >= shapeCanvas.ActualWidth - 5)
            //         {
            //             shapeCanvas.Cursor = Cursors.SizeWE;
            //         }
            //         // Transform top
            //         else if (currentPoint.Y <= 5)
            //         {
            //             shapeCanvas.Cursor = Cursors.SizeNS;
            //         }
            //         // Transform bottom
            //         else if (currentPoint.Y >= shapeCanvas.ActualHeight - 5)
            //         {
            //             shapeCanvas.Cursor = Cursors.SizeNS;
            //         }
            //         // Drag
            //         else
            //         {
            //             shapeCanvas.Cursor = Cursors.SizeAll;
            //             if (e.LeftButton == MouseButtonState.Pressed)
            //             {
            //                 double offsetX = currentPoint.X - dragStartPoint.X;
            //                 double offsetY = currentPoint.Y - dragStartPoint.Y;
            //                 // Move the shape
            //                 Canvas.SetLeft(shapeCanvas, Canvas.GetLeft(shapeCanvas) + offsetX);
            //                 Canvas.SetTop(shapeCanvas, Canvas.GetTop(shapeCanvas) + offsetY);
            //                 // Update the start and end points of the shape
            //                 dragStartPoint = currentPoint;
            //             }
            //         }
            //     }
            // }