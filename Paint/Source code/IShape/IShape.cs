
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MyShapes
{
    public interface IShape : ICloneable
    {
        string Name { get; }
        string Icon { get; }
        //string Text { get; set; }

        public RichTextBox richTextBox {get;set;}
        public double Thickness { get; set; }
        public double Angle { get; set; }
        public double ScaleH { get; set; }
        public double ScaleV { get; set; }
        public DoubleCollection StrokeDash { get; set; }
        public SolidColorBrush Brush { get; set; }
        public Point startPoint { get; set; }
        public Point endPoint { get; set; }
        public SolidColorBrush fillColor { get; set; }
        Canvas Convert();

        public class ShapeState
        {
            public SolidColorBrush fillColor_clone { get; set; } = Brushes.Transparent; // Fil color
            public double Thickness_clone { get; set; } = 3;
            public double Angle_clone { get; set; } = 0;
            public double ScaleH_clone { get; set; } = 0;
            public double ScaleV_clone { get; set; } = 0;
            public DoubleCollection StrokeDash_clone { get; set; } = new DoubleCollection();
            public SolidColorBrush Brush_clone { get; set; } = Brushes.Black;
            public Point startPoint_clone { get; set; }
            public Point endPoint_clone { get; set; }
            public Brush textColor { get; set; }
            public SolidColorBrush textBackgroundColor { get; set; }
            public string textContent { get; set; }
            public string fontFamily { get; set; }
            public double fontSize { get; set; }
            public bool hasTextBox { get; set; } = false;
        }
        public List<Action> actionHistory { get; set; }

        public Stack<Action> actionBuffer { get; set; }
        public enum ActionType
        {
            Modify,
            Create,
            Delete
        }

        public class Action
        {
            public ActionType Type { get; set; }
            public ShapeState State { get; set; }

            public Action Clone()
            {
                return (Action)MemberwiseClone();
            }
        }

        public void CaptureState(ActionType action);
        public bool undo();
        public string redo();
        public void updateData(ShapeState shapeState);
        public void ClearBufferData();
    }
}
