
using MyShapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Documents;

namespace UpArrow
{
    public class MyUpArrow : IShape
    {
        // ==================== Attributes ====================
        public string Name => "UpArrow"; // Name of the shape
        public string Icon => "Assets/up-arrow.png"; // Path to the icon
        public RichTextBox richTextBox { get; set; }
        public SolidColorBrush fillColor { get; set; } = Brushes.Transparent; // Fil color
        public double Thickness { get; set; } = 3;
        public double Angle { get; set; } = 0;
        public double ScaleH { get; set; } = 1;
        public double ScaleV { get; set; } = 1;
        public DoubleCollection StrokeDash { get; set; } = new DoubleCollection();
        public SolidColorBrush Brush { get; set; } = Brushes.Black;
        public Point startPoint { get; set; }
        public Point endPoint { get; set; }
        public MyUpArrow()
        {
            actionBuffer = new Stack<IShape.Action>();
            actionHistory = new List<IShape.Action>();
        }
        // Clone the object
        public object Clone()
        {
            IShape clonedShape = (IShape)MemberwiseClone();

            // Clone the RichTextBox if it exists
            if (richTextBox != null)
            {
                // Create a RichTextBox
                clonedShape.richTextBox = new System.Windows.Controls.RichTextBox()
                {
                    Width = Math.Abs(endPoint.X - startPoint.X),
                    Height = Math.Abs(endPoint.Y - startPoint.Y),
                    Background = Brushes.Transparent,
                    Foreground = richTextBox.Foreground,
                    BorderThickness = new Thickness(0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                richTextBox.LostFocus += RichTextBox_LostFocus;

                TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                string text = textRange.Text;

                string fontFamily = richTextBox.FontFamily.ToString();
                double fontSize = richTextBox.FontSize;

                Paragraph paragraph = new Paragraph();
                paragraph.TextAlignment = TextAlignment.Center;

                clonedShape.richTextBox.Document.Blocks.Add(paragraph);
                clonedShape.richTextBox.FontFamily = new FontFamily(fontFamily);
                clonedShape.richTextBox.FontSize = fontSize;
                clonedShape.richTextBox.Padding = new Thickness(0, 0, 0, 0);
                TextRange textRangeCloned = new TextRange(paragraph.ContentStart, paragraph.ContentEnd);
                textRangeCloned.Text = text.Replace("\r\n", ""); ;

                foreach (var child in richTextBox.Document.Blocks.ToList())
                {
                    if (child is Paragraph)
                    {
                        TextRange t = new TextRange(child.ContentStart, child.ContentEnd);
                        if (t.GetPropertyValue(TextElement.BackgroundProperty) is SolidColorBrush backgroundTextColor)
                        {
                            textRangeCloned.ApplyPropertyValue(TextElement.BackgroundProperty, backgroundTextColor);
                        }
                    }
                }
            }
            clonedShape.actionHistory = new List<IShape.Action>(actionHistory.Select(action => action.Clone()));
            clonedShape.actionBuffer = new Stack<IShape.Action>(actionBuffer.Select(action => action.Clone()));
            clonedShape.CaptureState(IShape.ActionType.Create);
            return clonedShape;
        }

        // Define the event handler
        private void RichTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            RichTextBox richTextBox1 = sender as RichTextBox;
            if (richTextBox != null)
            {
                // Get the text content when the RichTextBox loses focus
                TextRange textRange1 = new TextRange(richTextBox1.Document.ContentStart, richTextBox1.Document.ContentEnd);
                string textContent = textRange1.Text;

                TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                if (textContent != textRange.Text)
                {
                    //CaptureState(IShape.ActionType.Modify);
                    //ClearBufferData();
                }
                // Now you can save the text content or perform any other action you need
            }
        }

        // Convert the object to a UIElement - Draw the shape
        public Canvas Convert()
        {
            // Calculate canvas position and size
            double canvasLeft = Math.Min(startPoint.X, endPoint.X);
            double canvasTop = Math.Min(startPoint.Y, endPoint.Y);
            double canvasWidth = Math.Abs(endPoint.X - startPoint.X);
            double canvasHeight = Math.Abs(endPoint.Y - startPoint.Y);

            // Set canvas position and size
            Canvas frameCanvas = new Canvas();
            Canvas.SetLeft(frameCanvas, canvasLeft);
            Canvas.SetTop(frameCanvas, canvasTop);
            frameCanvas.Width = canvasWidth;
            frameCanvas.Height = canvasHeight;

            Point[] points = new Point[]
            {
                new Point(canvasWidth * 0.25, canvasHeight),
                new Point(canvasWidth * 0.75, canvasHeight),
                new Point(canvasWidth * 0.75, canvasHeight * 0.5),
                new Point(canvasWidth * 1, canvasHeight * 0.5),
                new Point(canvasWidth * 0.5, 0),
                new Point(0, canvasHeight * 0.5),
                new Point(canvasWidth * 0.25, canvasHeight * 0.5)
            };

            // Create a Polygon with the calculated points
            Polygon upArrowPolygon = new Polygon()
            {
                Points = new PointCollection(points),
                Fill = fillColor,
                Stroke = Brush,
                StrokeThickness = Thickness,
                StrokeDashArray = StrokeDash
            };

            frameCanvas.Children.Add(upArrowPolygon);

            // Remove the RichTextBox from its current parent before adding it to frameCanvas
            if (richTextBox != null && richTextBox.Parent != null)
            {
                var parent = richTextBox.Parent as Panel;
                parent.Children.Remove(richTextBox);
            }

            if (richTextBox != null)
            {
                frameCanvas.Children.Add(richTextBox);
            }
            
            // Apply rotate & scale transform
            RotateTransform rotateTransform = new RotateTransform(Angle);
            frameCanvas.RenderTransformOrigin = new Point(0.5, 0.5);
            ScaleTransform scaleTransform = new ScaleTransform(ScaleH, ScaleV);
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(rotateTransform); // Add rotate transform first
            transformGroup.Children.Add(scaleTransform); // Add scale transform
            frameCanvas.RenderTransform = transformGroup;
            
            return frameCanvas;
        }
        public List<IShape.Action> actionHistory { get; set; }
        public Stack<IShape.Action> actionBuffer { get; set; }
        public void CaptureState(IShape.ActionType action)
        {
            IShape.ShapeState currentState = new IShape.ShapeState
            {
                fillColor_clone = fillColor,
                Thickness_clone = Thickness,
                Angle_clone = Angle,
                ScaleH_clone = ScaleH,
                ScaleV_clone = ScaleV,
                StrokeDash_clone = StrokeDash,
                Brush_clone = Brush,
                startPoint_clone = startPoint,
                endPoint_clone = endPoint,
            };

            // Clone the RichTextBox if it exists
            if (richTextBox != null)
            {
                currentState.hasTextBox = true;
                currentState.textColor = richTextBox.Foreground;

                TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                currentState.textContent = textRange.Text;

                currentState.fontFamily = richTextBox.FontFamily.ToString();
                currentState.fontSize = richTextBox.FontSize;

                Paragraph paragraph = new Paragraph();
                paragraph.TextAlignment = TextAlignment.Center;

                TextRange textRangeCloned = new TextRange(paragraph.ContentStart, paragraph.ContentEnd);

                foreach (var child in richTextBox.Document.Blocks.ToList())
                {
                    if (child is Paragraph)
                    {
                        TextRange t = new TextRange(child.ContentStart, child.ContentEnd);
                        if (t.GetPropertyValue(TextElement.BackgroundProperty) is SolidColorBrush backgroundTextColor)
                        {
                            currentState.textBackgroundColor = backgroundTextColor;
                        }
                    }
                }
            }

            actionHistory.Add(new IShape.Action { Type = action, State = currentState });
        }

        public bool undo()
        {
            IShape.Action getPopped = actionHistory[actionHistory.Count - 1].Clone();

            if (actionHistory[actionHistory.Count - 1].Type == IShape.ActionType.Modify || actionHistory[actionHistory.Count - 1].Type == IShape.ActionType.Delete)
            {
                actionHistory.RemoveAt(actionHistory.Count - 1);
                updateData(actionHistory[actionHistory.Count - 1].State);
                actionBuffer.Push(getPopped);
                return true;
            }
            else if (actionHistory[actionHistory.Count - 1].Type == IShape.ActionType.Create)
            {
                getPopped.Type = IShape.ActionType.Delete;
                actionBuffer.Push(getPopped);
                return false;
            }
            else
            {
                return false;
            }
        }

        public string redo()
        {
            if (actionBuffer.Count > 0)
            {
                IShape.Action buffer = actionBuffer.Pop().Clone();

                if (buffer.Type == IShape.ActionType.Modify || buffer.Type == IShape.ActionType.Create)
                {
                    updateData(buffer.State);
                    actionHistory.Add(buffer);
                    return "true";
                }
                else if (buffer.Type == IShape.ActionType.Delete)
                {
                    buffer.Type = IShape.ActionType.Create;
                    actionHistory.Add(buffer);
                    return "false";
                }
            }
            return "";
        }

        public void updateData(IShape.ShapeState shapeState)
        {
            fillColor = shapeState.fillColor_clone;
            Thickness = shapeState.Thickness_clone;
            Angle = shapeState.Angle_clone;
            ScaleH = shapeState.ScaleH_clone;
            ScaleV = shapeState.ScaleV_clone;
            StrokeDash = shapeState.StrokeDash_clone;
            Brush = shapeState.Brush_clone;
            startPoint = shapeState.startPoint_clone;
            endPoint = shapeState.endPoint_clone;

            if (shapeState.hasTextBox)
            {
                var parent = richTextBox.Parent as Panel;
                parent.Children.Remove(richTextBox);

                richTextBox = new System.Windows.Controls.RichTextBox()
                {
                    Width = Math.Abs(shapeState.endPoint_clone.X - shapeState.startPoint_clone.X),
                    Height = Math.Abs(shapeState.endPoint_clone.Y - shapeState.startPoint_clone.Y),
                    Background = Brushes.Transparent,
                    Foreground = shapeState.textColor,
                    BorderThickness = new Thickness(0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = shapeState.fontSize,
                    FontFamily = new FontFamily(shapeState.fontFamily),
                };

                Paragraph paragraph = new Paragraph();
                richTextBox.Document.Blocks.Add(paragraph);
                richTextBox.Padding = new Thickness(0, 0, 0, 0);
                string text = shapeState.textContent;
                TextRange textRange = new TextRange(paragraph.ContentStart, paragraph.ContentEnd);
                textRange.Text = text.Replace("\r\n", "");

                textRange.ApplyPropertyValue(TextElement.BackgroundProperty, shapeState.textBackgroundColor);
                paragraph.TextAlignment = TextAlignment.Center;
            }
            else
            {
                if (richTextBox != null && richTextBox.Parent != null)
                {
                    richTextBox = richTextBox = new System.Windows.Controls.RichTextBox()
                    {
                        Visibility = Visibility.Collapsed,
                    };
                }
            }
        }

        public void ClearBufferData()
        {
            if (actionBuffer.Count > 0)
                actionBuffer.Clear();
        }

    }
}
