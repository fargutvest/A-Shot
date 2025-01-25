using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    internal class Circle : IDrawing
    {
        private int diameter;
        private Point location;
        private Color drawedPenColor;
        private float drawedPenWidth;
        private bool isDrawed;


        public Circle(int diameter, Point location)
        {
            this.diameter = diameter;
            this.location = location;
        }

        public void Erase(Graphics gr, Pen erasePen)
        {
            if (isDrawed)
            {
                erasePen.Width = drawedPenWidth;
                PaintInternal(gr, erasePen);
            }
        }

        public void Paint(Graphics gr, Pen pen)
        {
            if (isDrawed == false)
            {
                PaintInternal(gr, pen);
                isDrawed = true;
                drawedPenColor = pen.Color;
                drawedPenWidth = pen.Width;
            }
        }

        public void Repaint(Graphics gr)
        {
            if (isDrawed)
            {
                using (Pen pen = new Pen(drawedPenColor, drawedPenWidth))
                    PaintInternal(gr, pen);
            }
        }


        private void PaintInternal(Graphics gr, Pen pen)
        {
            gr.DrawEllipse(pen, new Rectangle(location.X, location.Y, diameter, diameter));
        }
    }
}
