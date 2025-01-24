using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    internal class Circle : IDrawing
    {
        private int diameter;
        private Point location;

        public Circle(int diameter, Point location)
        {
            this.diameter = diameter;
            this.location = location;
        }

        public void Paint(Graphics gr, Pen pen)
        {
            gr.DrawEllipse(pen, new Rectangle(location.X, location.Y, diameter, diameter));
        }
    }
}
