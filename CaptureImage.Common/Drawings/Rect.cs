using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    public class Rect : IDrawing
    {
        private Rectangle rectangle;
        private Color drawedPenColor;
        private float drawedPenWidth;
        private bool isDrawed;

        public Rect(Rectangle rectangle)
        {
            this.rectangle = rectangle;
        }

        public void Paint(Graphics gr, Pen pen)
        {
            PaintInternal(gr, pen);
            isDrawed = true;
            drawedPenColor = pen.Color;
            drawedPenWidth = pen.Width;
        }

        public void Repaint(Graphics gr)
        {
            if(isDrawed)
            {
                using (Pen pen = new Pen(drawedPenColor, drawedPenWidth))
                    PaintInternal(gr, pen);
            }
        }

        private void PaintInternal(Graphics gr, Pen pen)
        {
            gr.DrawRectangle(pen, rectangle);
        }

        public void Erase(Graphics gr, Pen erasePen)
        {
            if (isDrawed)
            {
                erasePen.Width = drawedPenWidth;
                PaintInternal(gr, erasePen);
            }
        }

    }
}
