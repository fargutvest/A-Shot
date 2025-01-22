using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    public class Rect : IDrawing
    {
        private Rectangle rectangle;

        public Rect(Rectangle rectangle)
        {
            this.rectangle = rectangle;
        }

        public void Paint(Graphics gr, Pen pen)
        {
            gr.DrawRectangle(pen, rectangle);
        }
    }
}
