using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
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
using Microsoft.Win32;
using MyShapes;
using DownArrow;
using Ellipse_;
using LeftArrow;
using Line_;
using Rectangle_;
using RightArrow;
using Star;
using Triangle;
using UpArrow;
using ICommand;
using MyUndoCommand;
using MyRevisionControl;
using MyToolbarCommand;
using MyCutCommand;
using MyClipboardControl;
using System.Globalization;
using Xceed.Wpf.Toolkit;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;

namespace MyPaint
{
    public class NumericValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()) || !double.TryParse(value.ToString(), out double result))
            {
                return 0; // Return 0 if the value is null, empty, or not a number
            }

            return result;
        }
    }

    public partial class MainWindow : Window
    {
        // ==================== Attributes ====================
        public System.Windows.Point startPoint; // Start point of the shape
        public System.Windows.Point endPoint; // End point of the shape
        Dictionary<string, DoubleCollection> dashCollections = new Dictionary<string, DoubleCollection>(); // List of dash styles
        int count = 0;
        private Stack<IShape> forwardBuffer = new Stack<IShape>();// Danh sách các hình vẽ được pop ra sau khi undo

        List<IShape> prototypeShapes = new List<IShape>(); // Danh sách các hình vẽ có thể chọn từ giao diện (Sản phẩm mẫu)
        List<IShape> drawnShapes = new List<IShape>(); // Danh sách các hình đã vẽ trên canvas
        List<IShape> backwardBuffer = new List<IShape>();
        IShape currentShape; // Current Shape  - Hình vẽ hiện tại đang vẽ
        IShape memoryShape;
        bool isSelectingArea = false;
        string choice;
        List<IShape> memory = new List<IShape> ();
        enum myMode
        {
            draw,
            select,
            text
        };
        enum actionType
        {
            delete,
            create,
            update,
            clear
        }
        List<int> positionTurns = new List<int> ();
        Stack<int> positionTurnBuffer = new Stack<int>();

        bool isDrawing = false; // Is Drawing - Used to remove the last shape (preview shape) when drawing
        bool isDrawn = false; // Is Drawn - Used to check if the shape is drawn or not (handle MouseUp event is triggered)
        bool isClicked = true;
        int selectingIndex = -1; // Index of the selecting single shape
        bool isSelecting = false; // Is Selecting - Used to check if the shape is selecting or not (avoid drawing new shape when selecting shape)
        System.Windows.Point dragStartPoint; // Start point when dragging the shape
        System.Windows.Point previousLocation;
        SolidColorBrush textColor;
        SolidColorBrush backgroundColor;
        RevisionControl revisionControl = new RevisionControl();
        ClipboardControl clipboard = new ClipboardControl();

        // ==================== Methods ====================
        public MainWindow()
        {
            InitializeComponent();
            dashCollections["Solid"] = null;
            dashCollections["Dash"] = new DoubleCollection { 4, 4 };
            dashCollections["Dot"] = new DoubleCollection { 1, 2 };
            dashCollections["Dash Dot"] = new DoubleCollection { 4, 2, 1, 2 };
            dashCollections["Dash Dot Dot"] = new DoubleCollection { 4, 2, 1, 2, 1, 2 };
            FontComboBox.SelectedIndex = 0;
            FontSizeComboBox.SelectedIndex = 3;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory; // Single configuration
            var fis = new DirectoryInfo(folder).GetFiles("*.dll");

            foreach (var fi in fis)
            {
                var assembly = Assembly.LoadFrom(fi.FullName); // Get all types in the assembly (in the DLL)
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if ((type.IsClass) && (typeof(IShape).IsAssignableFrom(type)))
                    {
                        prototypeShapes.Add((IShape)Activator.CreateInstance(type)!); // Add the shape to the list of prototype shapes
                    }
                }
            }

            RenderShapeButtons(prototypeShapes); // Render the shape buttons

            currentShape = prototypeShapes[0]; // Set the default shape
            choice = myMode.draw.ToString();
        }

        private void RenderShapeButtons(List<IShape> _prototypeShapes)
        {
            foreach (var shape in _prototypeShapes)
            {
                var button = new Button();
                button.Tag = shape;
                Style style = this.FindResource("FunctionalBarButtonImage_Style") as Style;
                button.Style = style;
                var image = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri(shape.Icon, UriKind.Relative)) };
                button.Content = image;
                button.Click += ShapeButton_Click;
                Shapes_StackPanel.Children.Add(button);
            }
        }


        // ==================== Tool Bar Handlers ====================
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            drawnShapes.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "dat";        // Default file extension
            openFileDialog.Filter = "dat files (*.dat)|*.dat"; // Filter for .dat files
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == true)
            {
                importCanvas(openFileDialog.FileName);
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = @"C:\";
            saveFileDialog.DefaultExt = "dat"; // Default file extension
            saveFileDialog.Filter = "dat files (*.dat)|*.dat"; // Filter for .dat files

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                exportCanvas(fileName);
            }
        }
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if(selectingIndex > -1)
            {
                CutCommand cut = new CutCommand(clipboard, drawnShapes, drawnShapes[selectingIndex], memory, forwardBuffer, positionTurns, positionTurnBuffer, true);
                ToolBarCommand toolBarCommand = new ToolBarCommand(cut, new UndoCommand(revisionControl, drawnShapes, forwardBuffer, memory, positionTurns, positionTurnBuffer, count));
                toolBarCommand.Toolbar_Copy();
            }
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectingIndex > -1)
            {
                CutCommand cut = new CutCommand(clipboard, drawnShapes, drawnShapes[selectingIndex], memory, forwardBuffer, positionTurns, positionTurnBuffer, false);
                ToolBarCommand toolBarCommand = new ToolBarCommand(cut, new UndoCommand(revisionControl, drawnShapes, forwardBuffer, memory, positionTurns, positionTurnBuffer, count));
                toolBarCommand.Toolbar_Cut();
                RedrawCanvas();
            }
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            CutCommand cut = new CutCommand(clipboard, drawnShapes, memory, true);
            ToolBarCommand toolBarCommand = new ToolBarCommand(cut, new UndoCommand(revisionControl, drawnShapes, forwardBuffer, memory, positionTurns, positionTurnBuffer, count));
            toolBarCommand.Toolbar_Paste();
            positionTurns.Add(drawnShapes.Count - 1);
            RedrawCanvas();
        }

        public void exportCanvas(string fileName)
        {
            try
            {
                using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(fileStream))
                {
                    writer.Write(drawnShapes.Count);
                    foreach (var shape in drawnShapes)
                    {
                        WriteShapeData(writer, shape);
                    }
                }
                System.Windows.MessageBox.Show("Shapes exported successfully.", "Export Shapes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error exporting shapes: " + ex.Message, "Export Shapes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Method to write shape data
        private void WriteShapeData(BinaryWriter writer, IShape shape)
        {
            writer.Write(shape.Name);
            writer.Write(shape.Thickness);

            // Write DoubleCollection
            if(shape.StrokeDash != null)
            {
                writer.Write(shape.StrokeDash.Count);
                foreach (double value in shape.StrokeDash)
                {
                    writer.Write(value);
                }
            } else
            {
                writer.Write(0);
            }

            // Write SolidColorBrush color
            System.Windows.Media.Color color = shape.Brush.Color;
            writer.Write(color.A);
            writer.Write(color.R);
            writer.Write(color.G);
            writer.Write(color.B);

            System.Windows.Media.Color background = shape.fillColor.Color;
            writer.Write(background.A);
            writer.Write(background.R);
            writer.Write(background.G);
            writer.Write(background.B);

            writer.Write(shape.startPoint.X);
            writer.Write(shape.startPoint.Y);
            writer.Write(shape.endPoint.X);
            writer.Write(shape.endPoint.Y);

            writer.Write(shape.Angle);
            writer.Write(shape.ScaleH);
            writer.Write(shape.ScaleV);

            if (shape.richTextBox != null)
            {
                writer.Write(true);

                // Get the text content of the RichTextBox
                TextRange textRange = new TextRange(shape.richTextBox.Document.ContentStart, shape.richTextBox.Document.ContentEnd);
                string text = textRange.Text;

                // Write the length of the text
                writer.Write(text.Length);

                // Write the text content
                writer.Write(text);

                // Write the foreground color
                System.Windows.Media.Color foregroundColor = (shape.richTextBox.Foreground as SolidColorBrush)?.Color ?? Colors.Black;
                writer.Write(foregroundColor.A);
                writer.Write(foregroundColor.R);
                writer.Write(foregroundColor.G);
                writer.Write(foregroundColor.B);

                // Write the font family
                writer.Write(shape.richTextBox.FontFamily.ToString());

                // Write the font size
                writer.Write(shape.richTextBox.FontSize);


                foreach (var child in shape.richTextBox.Document.Blocks)
                {
                    if (child is Paragraph)
                    {
                        TextRange t = new TextRange(child.ContentStart, child.ContentEnd);
                        // Check if the background property is set
                        if (t.GetPropertyValue(TextElement.BackgroundProperty) is SolidColorBrush backgroundBrush)
                        {
                            // Get the color
                            System.Windows.Media.Color color1 = backgroundBrush.Color;
                            // Write the color components
                            writer.Write(color1.A);
                            writer.Write(color1.R);
                            writer.Write(color1.G);
                            writer.Write(color1.B);
                        }
                        else
                        {
                            // Write default color if background property is not set
                            writer.Write(Colors.Transparent.A);
                            writer.Write(Colors.Transparent.R);
                            writer.Write(Colors.Transparent.G);
                            writer.Write(Colors.Transparent.B);
                        }
                    }
                }

            } else
            {
                writer.Write(false);
            }


        }
        public void importCanvas(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        // Read shape data
                        int numberofShape = reader.ReadInt32();
                        for (int i = 0; i < numberofShape; i++)
                        {
                            string name = reader.ReadString();
                            double thickness = reader.ReadDouble();
                            int strokeDashCount = reader.ReadInt32();
                            DoubleCollection strokeDash = null;
                            if (strokeDashCount != 0)
                            {
                                strokeDash = new DoubleCollection();
                                for (int j = 0; j < strokeDashCount; j++)
                                {
                                    strokeDash.Add(reader.ReadDouble());
                                }
                            }

                            byte a = reader.ReadByte();
                            byte r = reader.ReadByte();
                            byte g = reader.ReadByte();
                            byte b = reader.ReadByte();
                            SolidColorBrush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(a, r, g, b));
                            byte ba = reader.ReadByte();
                            byte br = reader.ReadByte();
                            byte bg = reader.ReadByte();
                            byte bb = reader.ReadByte();
                            SolidColorBrush background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(ba, br, bg, bb));
                            System.Windows.Point startPoint = new System.Windows.Point(reader.ReadDouble(), reader.ReadDouble());
                            System.Windows.Point endPoint = new System.Windows.Point(reader.ReadDouble(), reader.ReadDouble());
                            double angle = reader.ReadDouble();
                            double scaleH = reader.ReadDouble();
                            double scaleV = reader.ReadDouble();

                            // Read RichTextBox data if available
                            System.Windows.Controls.RichTextBox richTextBox = null;

                            IShape shape;
                            // Create an instance of IShape using the read data
                            switch (name)
                            {
                                case "DownArrow":
                                    MyDownArrow myDownArrow = new MyDownArrow();
                                    myDownArrow.Thickness = thickness;
                                    myDownArrow.startPoint = startPoint;
                                    myDownArrow.endPoint = endPoint;
                                    myDownArrow.Brush = brush;
                                    myDownArrow.fillColor = background;
                                    myDownArrow.StrokeDash = strokeDash;
                                    myDownArrow.Angle = angle;
                                    myDownArrow.ScaleH = scaleH;
                                    myDownArrow.ScaleV = scaleV;
                                    shape = (IShape)myDownArrow;
                                    break;
                                case "Ellipse":
                                    MyEllipse myEllipse = new MyEllipse();
                                    myEllipse.Thickness = thickness;
                                    myEllipse.startPoint = startPoint;
                                    myEllipse.endPoint = endPoint;
                                    myEllipse.Brush = brush;
                                    myEllipse.fillColor = background;
                                    myEllipse.StrokeDash = strokeDash;
                                    myEllipse.Angle = angle;
                                    myEllipse.ScaleH = scaleH;
                                    myEllipse.ScaleV = scaleV;
                                    shape = (IShape)myEllipse;
                                    break;
                                case "LeftArrow":
                                    MyLeftArrow myLeftArrow = new MyLeftArrow();
                                    myLeftArrow.Thickness = thickness;
                                    myLeftArrow.startPoint = startPoint;
                                    myLeftArrow.endPoint = endPoint;
                                    myLeftArrow.Brush = brush;
                                    myLeftArrow.StrokeDash = strokeDash;
                                    myLeftArrow.fillColor = background;
                                    myLeftArrow.Angle = angle;
                                    myLeftArrow.ScaleH = scaleH;
                                    myLeftArrow.ScaleV = scaleV;
                                    shape = (IShape)myLeftArrow;
                                    break;
                                case "Line":
                                    MyLine myLine = new MyLine();
                                    myLine.Thickness = thickness;
                                    myLine.startPoint = startPoint;
                                    myLine.endPoint = endPoint;
                                    myLine.Brush = brush;
                                    myLine.StrokeDash = strokeDash;
                                    myLine.fillColor = background;
                                    myLine.Angle = angle;
                                    myLine.ScaleH = scaleH;
                                    myLine.ScaleV = scaleV;
                                    shape = (IShape)myLine;
                                    break;
                                case "Rectangle":
                                    MyRectangle myRectangle = new MyRectangle();
                                    myRectangle.Thickness = thickness;
                                    myRectangle.startPoint = startPoint;
                                    myRectangle.endPoint = endPoint;
                                    myRectangle.Brush = brush;
                                    myRectangle.StrokeDash = strokeDash;
                                    myRectangle.fillColor = background;
                                    myRectangle.Angle = angle;
                                    myRectangle.ScaleH = scaleH;
                                    myRectangle.ScaleV = scaleV;
                                    shape = (IShape)myRectangle;
                                    break;
                                case "RightArrow":
                                    MyRightArrow myRightArrow = new MyRightArrow();
                                    myRightArrow.Thickness = thickness;
                                    myRightArrow.startPoint = startPoint;
                                    myRightArrow.endPoint = endPoint;
                                    myRightArrow.Brush = brush;
                                    myRightArrow.StrokeDash = strokeDash;
                                    myRightArrow.fillColor = background;
                                    myRightArrow.Angle = angle;
                                    myRightArrow.ScaleH = scaleH;
                                    myRightArrow.ScaleV = scaleV;
                                    shape = (IShape)myRightArrow;
                                    break;
                                case "Star":
                                    MyStar myStar = new MyStar();
                                    myStar.Thickness = thickness;
                                    myStar.startPoint = startPoint;
                                    myStar.endPoint = endPoint;
                                    myStar.Brush = brush;
                                    myStar.StrokeDash = strokeDash;
                                    myStar.fillColor = background;
                                    myStar.Angle = angle;
                                    myStar.ScaleH = scaleH;
                                    myStar.ScaleV = scaleV;
                                    shape = (IShape)myStar;
                                    break;
                                case "Triangle":
                                    MyTriangle myTriangle = new MyTriangle();
                                    myTriangle.Thickness = thickness;
                                    myTriangle.startPoint = startPoint;
                                    myTriangle.endPoint = endPoint;
                                    myTriangle.Brush = brush;
                                    myTriangle.StrokeDash = strokeDash;
                                    myTriangle.fillColor = background;
                                    myTriangle.Angle = angle;
                                    myTriangle.ScaleH = scaleH;
                                    myTriangle.ScaleV = scaleV;
                                    shape = (IShape)myTriangle;
                                    break;
                                case "UpArrow":
                                    MyUpArrow myUpArrow = new MyUpArrow();
                                    myUpArrow.Thickness = thickness;
                                    myUpArrow.startPoint = startPoint;
                                    myUpArrow.endPoint = endPoint;
                                    myUpArrow.Brush = brush;
                                    myUpArrow.StrokeDash = strokeDash;
                                    myUpArrow.fillColor = background;
                                    myUpArrow.Angle = angle;
                                    myUpArrow.ScaleH = scaleH;
                                    myUpArrow.ScaleV = scaleV;
                                    shape = (IShape)myUpArrow;
                                    drawnShapes.Add(shape);
                                    break;
                                default:
                                    throw new ArgumentException("Unexpected value for name");
                            }
                            if (reader.ReadBoolean())
                            {
                                // Read text content
                                int textLength = reader.ReadInt32();
                                string text = reader.ReadString();

                                // Read foreground color
                                byte foregroundA = reader.ReadByte();
                                byte foregroundR = reader.ReadByte();
                                byte foregroundG = reader.ReadByte();
                                byte foregroundB = reader.ReadByte();
                                SolidColorBrush foregroundColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(foregroundA, foregroundR, foregroundG, foregroundB));

                                // Read font family
                                string fontFamily = reader.ReadString();

                                // Read font size
                                double fontSize = reader.ReadDouble();

                                richTextBox = new System.Windows.Controls.RichTextBox()
                                {
                                    Width = Math.Abs(endPoint.X - startPoint.X),
                                    Height = Math.Abs(endPoint.Y - startPoint.Y),
                                    Background = Brushes.Transparent,
                                    Foreground = foregroundColor,
                                    FontFamily = new FontFamily(fontFamily),
                                    FontSize = fontSize,
                                    BorderThickness = new Thickness(0), // Remove the border of the RichTextBox
                                    HorizontalAlignment = HorizontalAlignment.Center, // Center the RichTextBox horizontally
                                    VerticalAlignment = VerticalAlignment.Center, // Center the RichTextBox vertically
                                };

                                // Read paragraph background color
                                byte paraBackgroundA = reader.ReadByte();
                                byte paraBackgroundR = reader.ReadByte();
                                byte paraBackgroundG = reader.ReadByte();
                                byte paraBackgroundB = reader.ReadByte();
                                SolidColorBrush paraBackgroundColor = new SolidColorBrush(System.Windows.Media.Color.FromArgb(paraBackgroundA, paraBackgroundR, paraBackgroundG, paraBackgroundB));

                                // Create a new paragraph and add it to the RichTextBox
                                Paragraph paragraph = new Paragraph();

                                // Set paragraph alignment to center
                                paragraph.TextAlignment = TextAlignment.Center;
                                richTextBox.Document.Blocks.Add(paragraph);
                                richTextBox.Padding = new Thickness(0, 0, 0, 0);

                                // Set the text
                                TextRange textRange = new TextRange(paragraph.ContentStart, paragraph.ContentEnd);
                                textRange.Text = text;

                                // Set the background color of the selected text
                                textRange.ApplyPropertyValue(TextElement.BackgroundProperty, paraBackgroundColor);

                                shape.richTextBox = richTextBox;
                            }
                            drawnShapes.Add(shape);
                        }
                    }
                }
                Main_Canvas.Children.Clear();
                RedrawCanvas();
                System.Windows.MessageBox.Show("Shapes imported successfully.", "Import Shapes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error importing shapes: " + ex.Message, "Import Shapes", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            Command control = new UndoCommand(revisionControl, drawnShapes, forwardBuffer, memory, positionTurns, positionTurnBuffer, count);
            ToolBarCommand toolBarCommand = new ToolBarCommand(control);
            toolBarCommand.Toolbar_Undo();
            RedrawCanvas();
        }
        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            Command control = new UndoCommand(revisionControl, drawnShapes, forwardBuffer, memory, positionTurns, positionTurnBuffer, count);
            ToolBarCommand toolBarCommand = new ToolBarCommand(control);
            toolBarCommand.Toolbar_Redo();
            RedrawCanvas();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e) { Debug.WriteLine("Exit"); }
        private void RedrawCanvas()
        {
            Main_Canvas.Children.Clear();
            foreach (var shape in drawnShapes)
            {
                Canvas shapeCanvas = shape.Convert();
                shapeCanvas.PreviewMouseDown += ShapeCanvas_PreviewMouseDown;
                shapeCanvas.PreviewMouseMove += ShapeCanvas_PreviewMouseMove;
                shapeCanvas.PreviewMouseUp += ShapeCanvas_PreviewMouseUp;

                Main_Canvas.Children.Add(shapeCanvas);
            }
        }
        // ==================== Functional Bar Handlers ====================


        // --- Select Tool "Select Area"
        private void CopySelectedAreaToClipboard()
        {
            double left;
            double top;
            double width;
            double height;

            // Adjust the selected area
            left = Math.Max(0, Math.Min(startPoint.X, endPoint.X));
            top = Math.Max(0, Math.Min(startPoint.Y, endPoint.Y));
            width = Math.Abs(endPoint.X - startPoint.X);
            height = Math.Abs(endPoint.Y - startPoint.Y);

            drawnShapes.RemoveAt(drawnShapes.Count - 1);
            Main_Canvas.Children.RemoveAt(Main_Canvas.Children.Count - 1);

            // Create a DrawingVisual for the selected area
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                // Draw the Main_Canvas content to the DrawingContext with the selected area
                drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, width, height)));
                VisualBrush visualBrush = new VisualBrush(Main_Canvas)
                {
                    Stretch = Stretch.None,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top,
                };
                drawingContext.DrawRectangle(new VisualBrush(Main_Canvas), null, new Rect(-left, -top, Main_Canvas.ActualWidth, Main_Canvas.ActualHeight));
            }

            // Render the DrawingVisual to a RenderTargetBitmap
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)width,
                (int)height,
                96,
                96,
                PixelFormats.Pbgra32);

            renderTargetBitmap.Render(drawingVisual);

            Clipboard.SetImage(renderTargetBitmap);
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            if (choice == myMode.select.ToString())
            {
                CopySelectedAreaToClipboard();
                choice = "";
                TextBlock selectTextBlock = (TextBlock)FindName("SelectTB");
                if (selectTextBlock != null)
                {
                    selectTextBlock.Text = "Select";
                }
            }
            else
            {
                choice = myMode.select.ToString();
                MyRectangle rect = new MyRectangle();
                currentShape = rect;
                TextBlock selectTextBlock = (TextBlock)FindName("SelectTB");
                if (selectTextBlock != null)
                {
                    selectTextBlock.Text = "Copy to Clipboard";
                }
            }
        }

        private void LayersButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (choice == myMode.select.ToString())
            {
                CopySelectedAreaToClipboard();
                choice = "";
                TextBlock selectTextBlock = (TextBlock)FindName("SelectTB");
                if (selectTextBlock != null)
                {
                    selectTextBlock.Text = "Select";
                }
            }
            else
            {
                choice = myMode.select.ToString();
                MyRectangle rect = new MyRectangle();
                currentShape = rect;
                TextBlock selectTextBlock = (TextBlock)FindName("SelectTB");
                if (selectTextBlock != null)
                {
                    selectTextBlock.Text = "Copy to Clipboard";
                }
            }
        }

        // --- Add text button
        private void TextButton_Click(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)FindName("AddTextTB");
            if (choice != myMode.text.ToString())
            {
                if (selectingIndex > -1)
                {
                    Grid stackPanel = (Grid)FindName("TextToolbar");
                    if (stackPanel != null)
                    {
                        stackPanel.Visibility = Visibility.Visible;
                    }
                    if (drawnShapes[selectingIndex].richTextBox == null)
                    {
                        // Create a RichTextBox
                        System.Windows.Controls.RichTextBox richTextBox = new System.Windows.Controls.RichTextBox()
                        {
                            Width = Math.Abs(drawnShapes[selectingIndex].endPoint.X - drawnShapes[selectingIndex].startPoint.X), // Set the width of the RichTextBox
                            Height = Math.Abs(drawnShapes[selectingIndex].endPoint.Y - drawnShapes[selectingIndex].startPoint.Y), // Set the height of the RichTextBox
                            Background = Brushes.Transparent,
                            Foreground = Brushes.Black, // Set the foreground color of the RichTextBox
                            BorderThickness = new Thickness(0), // Remove the border of the RichTextBox
                            HorizontalAlignment = HorizontalAlignment.Center, // Center the RichTextBox horizontally
                            VerticalAlignment = VerticalAlignment.Center, // Center the RichTextBox vertically
                            FontFamily = new FontFamily("Arial"),
                            FontSize = 12.0,
                        };

                        // Create a new paragraph and add it to the RichTextBox
                        Paragraph paragraph = new Paragraph();
                        richTextBox.Document.Blocks.Add(paragraph);
                        richTextBox.Padding = new Thickness(0, 0, 0, 0);
                        // Set the text
                        string text = "Sample";
                        TextRange textRange = new TextRange(paragraph.ContentStart, paragraph.ContentEnd);
                        textRange.Text = text;

                        // Set the background color of the selected text
                        textRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
                        // Set paragraph alignment to center
                        paragraph.TextAlignment = TextAlignment.Center;

                        drawnShapes[selectingIndex].richTextBox = richTextBox;
                        drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                        for(int i = 0; i < drawnShapes.Count; i++)
                        {
                            drawnShapes[i].ClearBufferData();
                        }

                        forwardBuffer.Clear();
                        positionTurns.Add(selectingIndex);
                        RedrawCanvas();
                    }
                    textBlock.Text = "Exit Mode";
                    choice = myMode.text.ToString();
                }
            } else
            {
                textBlock.Text = "Add Text";
                Grid stackPanel = (Grid)FindName("TextToolbar");
                if (stackPanel != null)
                {
                    stackPanel.Visibility = Visibility.Hidden;
                }
                choice = myMode.draw.ToString();
            }
            
        }

        // --- Select Shape Button
        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            choice = myMode.draw.ToString();
            IShape item = (IShape)(sender as Button)!.Tag;
            currentShape = item;
        }

        // --- Select TextColor
        private void TextColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            textColor = new SolidColorBrush(e.NewValue.Value);
            if(selectingIndex > -1)
            {
                if (drawnShapes[selectingIndex].richTextBox != null)
                {
                    drawnShapes[selectingIndex].richTextBox.Foreground = textColor;

                    drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                    for (int i = 0; i < drawnShapes.Count; i++)
                    {
                        drawnShapes[i].ClearBufferData();
                    }
                    forwardBuffer.Clear();
                    positionTurns.Add(selectingIndex);
                    RedrawCanvas();
                }
            }
            
        }
        private void TextBackgroundColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            backgroundColor = new SolidColorBrush(e.NewValue.Value);
            if (selectingIndex > -1)
            {
                if (drawnShapes[selectingIndex].richTextBox != null)
                {
                    foreach (var child in drawnShapes[selectingIndex].richTextBox.Document.Blocks)
                    {
                        if(child is Paragraph)
                        {
                            TextRange textRange = new TextRange(child.ContentStart, child.ContentEnd);
                            // Set the background color of the selected text
                            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, backgroundColor);
                        }
                    }

                    drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                    for (int i = 0; i < drawnShapes.Count; i++)
                    {
                        drawnShapes[i].ClearBufferData();
                    }
                    forwardBuffer.Clear();
                    positionTurns.Add(selectingIndex);
                    RedrawCanvas();
                }
            }
        }

        // --- Select Color Stroke & Fill
        private void StrokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            SolidColorBrush color = new SolidColorBrush(e.NewValue.Value);
            foreach (IShape shape in prototypeShapes) { shape.Brush = color; }
            if(selectingIndex > -1)
            {
                drawnShapes[selectingIndex].Brush = color;
                drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                for (int i = 0; i < drawnShapes.Count; i++)
                {
                    drawnShapes[i].ClearBufferData();
                }
                forwardBuffer.Clear();
                positionTurns.Add(selectingIndex);
                RedrawCanvas();
            }
        }
        
        private void FillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            SolidColorBrush color = new SolidColorBrush(e.NewValue.Value);
            foreach (IShape shape in prototypeShapes) { shape.fillColor = color; }
            if (selectingIndex > -1)
            {
                drawnShapes[selectingIndex].fillColor = color;
                drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                for (int i = 0; i < drawnShapes.Count; i++)
                {
                    drawnShapes[i].ClearBufferData();
                }
                forwardBuffer.Clear();
                positionTurns.Add(selectingIndex);
                RedrawCanvas();
            }
        }

        // --- Select Stroke Thickness
        private void StrokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            StrokeThickness_TextBlock.Text = Math.Ceiling(e.NewValue).ToString();
            foreach (IShape shape in prototypeShapes) { shape.Thickness = e.NewValue; }
            if (selectingIndex > -1)
            {
                drawnShapes[selectingIndex].Thickness = e.NewValue;
                drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                for (int i = 0; i < drawnShapes.Count; i++)
                {
                    drawnShapes[i].ClearBufferData();
                }
                forwardBuffer.Clear();
                positionTurns.Add(selectingIndex);
                RedrawCanvas();
            }
        }

        // --- Select Stroke Dash
        private void StrokeDashComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)StrokeDash_ComboBox.SelectedItem;
            string selectedTag = selectedItem.Tag.ToString();

            foreach (IShape shape in prototypeShapes)
            {
                if (selectedTag == "Solid") { shape.StrokeDash = null; }
                else { shape.StrokeDash = dashCollections[selectedTag]; }
            }
            if (selectingIndex > -1)
            {
                drawnShapes[selectingIndex].StrokeDash = dashCollections[selectedTag];
                drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                for (int i = 0; i < drawnShapes.Count; i++)
                {
                    drawnShapes[i].ClearBufferData();
                }
                forwardBuffer.Clear();
                positionTurns.Add(selectingIndex);
                RedrawCanvas();
            }
        }

        // --- Clean Button
        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            Main_Canvas.Children.Clear();
            drawnShapes.Clear();
        }

        // --- TEST Redraw Button
        private void RedrawButton_Click(object sender, RoutedEventArgs e)
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


        // --- Angle Slider
        private void AngleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (selectingIndex != -1)
            {
                double angle = Angle_Slider.Value;
                Canvas selectingCanvas = (Canvas)Main_Canvas.Children[selectingIndex];

                RotateTransform rotateTransform = new RotateTransform(angle);
                selectingCanvas.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                ScaleTransform scaleTransform = new ScaleTransform(drawnShapes[selectingIndex].ScaleH, drawnShapes[selectingIndex].ScaleV);
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(rotateTransform); // Add rotate transform first
                transformGroup.Children.Add(scaleTransform); // Add scale transform

                selectingCanvas.RenderTransform = transformGroup;

                drawnShapes[selectingIndex].Angle = angle;
                drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                for (int i = 0; i < drawnShapes.Count; i++)
                {
                    drawnShapes[i].ClearBufferData();
                }
                forwardBuffer.Clear();
                positionTurns.Add(selectingIndex);
            }
        }

        private void ScaleXSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (selectingIndex != -1)
            {
                double scaleX = ScaleX_Slider.Value;
                Canvas selectingCanvas = (Canvas)Main_Canvas.Children[selectingIndex];

                RotateTransform rotateTransform = new RotateTransform(drawnShapes[selectingIndex].Angle);
                selectingCanvas.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                ScaleTransform scaleTransform = new ScaleTransform(scaleX, drawnShapes[selectingIndex].ScaleV);
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(rotateTransform); // Add rotate transform first
                transformGroup.Children.Add(scaleTransform); // Add scale transform

                selectingCanvas.RenderTransform = transformGroup;

                drawnShapes[selectingIndex].ScaleH = scaleX;
                drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                for (int i = 0; i < drawnShapes.Count; i++)
                {
                    drawnShapes[i].ClearBufferData();
                }
                forwardBuffer.Clear();
                positionTurns.Add(selectingIndex);
            }
        }

        private void ScaleYSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (selectingIndex != -1)
            {
                double scaleY = ScaleY_Slider.Value;
                Canvas selectingCanvas = (Canvas)Main_Canvas.Children[selectingIndex];

                RotateTransform rotateTransform = new RotateTransform(drawnShapes[selectingIndex].Angle);
                selectingCanvas.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                ScaleTransform scaleTransform = new ScaleTransform(drawnShapes[selectingIndex].ScaleH, scaleY);
                TransformGroup transformGroup = new TransformGroup();
                transformGroup.Children.Add(rotateTransform); // Add rotate transform first
                transformGroup.Children.Add(scaleTransform); // Add scale transform

                selectingCanvas.RenderTransform = transformGroup;

                drawnShapes[selectingIndex].ScaleV = scaleY;
                drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                for (int i = 0; i < drawnShapes.Count; i++)
                {
                    drawnShapes[i].ClearBufferData();
                }
                forwardBuffer.Clear();
                positionTurns.Add(selectingIndex);
            }
        }


        // ==================== Main Canvas Handlers ====================

        // MainCanvas_PreviewMouseDown will be triggered before ShapeCanvas_PreviewMouseDown
        // Used to remove the selecting shape when clicking outside the shape
        private void MainCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isSelecting = false;
            Main_Canvas.Focus();
            RemoveSelectingShape();
            selectingIndex = -1;
            TransformShape_Border.Visibility = Visibility.Hidden;
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isSelecting == false)
            {
                startPoint = e.GetPosition(Main_Canvas);
                currentShape.startPoint = startPoint;
                isDrawing = false;
                isDrawn = false;
                isSelectingArea = false;
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

                currentShape.endPoint = endPoint;

                // Remove the last shape (preview shape)
                if (isDrawing == true || isSelectingArea == true)
                {
                    Main_Canvas.Children.RemoveAt(Main_Canvas.Children.Count - 1);
                    drawnShapes.RemoveAt(drawnShapes.Count - 1);
                }
                drawnShapes.Add((IShape)currentShape.Clone());
                if (forwardBuffer.Count > 0)
                {
                    forwardBuffer.Clear();
                }
                // Then re-draw it
                Canvas shapeCanvas = currentShape.Convert();
                shapeCanvas.PreviewMouseDown += ShapeCanvas_PreviewMouseDown;
                shapeCanvas.PreviewMouseMove += ShapeCanvas_PreviewMouseMove;
                shapeCanvas.PreviewMouseUp += ShapeCanvas_PreviewMouseUp;

                Main_Canvas.Children.Add(shapeCanvas);
                if (choice == myMode.select.ToString())
                {
                    isSelectingArea = true;
                    currentShape.StrokeDash = dashCollections["Dot"];
                    currentShape.Thickness = 3;
                    currentShape.fillColor = new SolidColorBrush(Colors.Transparent);
                }
                else if (choice == myMode.draw.ToString() || choice == myMode.text.ToString())
                {
                    isDrawing = true;
                }
            }
        }

        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing == true && isDrawn == false)
            {
                positionTurns.Add(count++);
                isDrawn = true;
            }
        }


        private void ShapeCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Canvas shapeCanvas)
            {
                isSelecting = true;

                DrawSelectingShape(shapeCanvas);
                
                selectingIndex = Main_Canvas.Children.IndexOf(shapeCanvas);

                dragStartPoint = e.GetPosition(Main_Canvas);
                isClicked = true;

                if(drawnShapes[selectingIndex].richTextBox != null)
                {
                    drawnShapes[selectingIndex].richTextBox.Focus();    
                }

                TransformShape_Border.Visibility = Visibility.Visible;
                Angle_Slider.Value = drawnShapes[selectingIndex].Angle;
                ScaleX_Slider.Value = drawnShapes[selectingIndex].ScaleH;
                ScaleY_Slider.Value = drawnShapes[selectingIndex].ScaleV;
            }
        }

        private void ShapeCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting == true && e.LeftButton == MouseButtonState.Pressed)
            {
                Canvas shapeCanvas = Main_Canvas.Children[selectingIndex] as Canvas;
                shapeCanvas.Cursor = Cursors.SizeAll;
                System.Windows.Point currentPoint = e.GetPosition(Main_Canvas);
                double offsetX = currentPoint.X - dragStartPoint.X;
                double offsetY = currentPoint.Y - dragStartPoint.Y;

                // Move the shape
                Canvas.SetLeft(shapeCanvas, Canvas.GetLeft(shapeCanvas) + offsetX);
                Canvas.SetTop(shapeCanvas, Canvas.GetTop(shapeCanvas) + offsetY);

                if(offsetX > 0)
                {
                    isClicked = false;
                }

                // Update the start and end points of the shape
                dragStartPoint = currentPoint;
            }
        }

        private void ShapeCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isSelecting == true)
            {
                if (sender is Canvas shapeCanvas)
                {
                    drawnShapes[selectingIndex].startPoint = new System.Windows.Point(Canvas.GetLeft(shapeCanvas), Canvas.GetTop(shapeCanvas));
                    drawnShapes[selectingIndex].endPoint = new System.Windows.Point(Canvas.GetLeft(shapeCanvas) + shapeCanvas.ActualWidth, Canvas.GetTop(shapeCanvas) + shapeCanvas.ActualHeight);

                    if(isClicked == false)
                    {
                        drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                        for (int i = 0; i < drawnShapes.Count; i++)
                        {
                            drawnShapes[i].ClearBufferData();
                        }
                        forwardBuffer.Clear();
                        positionTurns.Add(selectingIndex);
                    }
                }
            }
            // For Line, Rectangle because they are hit the cursor => Do not catch the MainCanvas_MouseUp event
            else if (isDrawing == true && isDrawn == false)
            {
                positionTurns.Add(count++);
                isDrawn = true;
            }
        }


        private void RemoveSelectingShape()
        {
            if (selectingIndex != -1)
            {
                if(Main_Canvas.Children.Count > 0)
                {
                    Canvas selectingCanvas = (Canvas)Main_Canvas.Children[selectingIndex];
                    foreach (UIElement child in selectingCanvas.Children)
                    {
                        if (child is System.Windows.Shapes.Rectangle rectangle && rectangle.Name == "Selecting_Rectangle")
                        {
                            selectingCanvas.Children.Remove(rectangle);
                            break;
                        }
                    }
                }
            }
        }

        private void DrawSelectingShape(Canvas canvas)
        {
            System.Windows.Shapes.Rectangle selectingRectangle = new System.Windows.Shapes.Rectangle
            {
                Width = canvas.ActualWidth,
                Height = canvas.ActualHeight,
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 2, 2 },
                Fill = Brushes.Transparent,
                Name = "Selecting_Rectangle"
            };
            
            canvas.Children.Add(selectingRectangle);
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(selectingIndex > -1)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)FontSizeComboBox.SelectedItem;

                // Check if an item is selected
                if (selectedItem != null)
                {
                    string fontSize = selectedItem.Content.ToString();
                    double selectedFontSize = Convert.ToDouble(fontSize);
                    drawnShapes[selectingIndex].richTextBox.FontSize = selectedFontSize;
                    drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                    for (int i = 0; i < drawnShapes.Count; i++)
                    {
                        drawnShapes[i].ClearBufferData();
                    }
                    forwardBuffer.Clear();
                    positionTurns.Add(selectingIndex);
                    RedrawCanvas();
                }
            }
        }
        private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectingIndex > -1)
            {
                ComboBoxItem selectedItem = (ComboBoxItem)FontComboBox.SelectedItem;

                // Check if an item is selected
                if (selectedItem != null)
                {
                    string font = selectedItem.Content.ToString();
                    drawnShapes[selectingIndex].richTextBox.FontFamily = new FontFamily(font);
                    drawnShapes[selectingIndex].CaptureState(IShape.ActionType.Modify);
                    drawnShapes[selectingIndex].ClearBufferData();
                    forwardBuffer.Clear();
                    positionTurns.Add(selectingIndex);
                    RedrawCanvas();
                }
            }
        }
    }
}
