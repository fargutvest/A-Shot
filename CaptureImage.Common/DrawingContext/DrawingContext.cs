using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CaptureImage.Common.DrawingContext
{
    public class DrawingContext : ICloneable
    {
        public event EventHandler DrawingContextEdited;

        public static DrawingContext Create(Image[] canvasImages, Control[] canvasControls, bool isClean = false) =>
            new DrawingContext()
            {
                CanvasImages = canvasImages,
                CanvasControls = canvasControls,
                IsClean = isClean
            };

        public Image[] CanvasImages { get; set; }
        public Control[] CanvasControls { get; set; }

        public Pen DrawingPen { get; set; }

        public bool IsClean { get; set; }

        public void SetColorOfPen(Color color)
        {
            DrawingPen.Color = color;
            DrawingContextEdited?.Invoke(this, EventArgs.Empty);
        }

        public object Clone()
        {
            DrawingContext clone = new DrawingContext()
            {
                DrawingPen = DrawingPen,
                CanvasControls = CanvasControls,
                CanvasImages = CanvasImages.Select(im => im.Clone() as Image).ToArray(),
                IsClean = IsClean
            };

            return clone;
        }
    }
}
