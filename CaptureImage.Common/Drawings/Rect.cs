using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    public class Rect : IDrawing
    {
        private Rectangle rectangle;
        private Pen drawedByPen;

        public Rect(Rectangle rectangle)
        {
            this.rectangle = rectangle;
        }

        public void Paint(Graphics gr, Pen pen)
        {
            PaintInternal(gr, pen);
            drawedByPen = pen.Clone() as Pen;
        }

        public void Repaint(Graphics gr)
        {
            if (drawedByPen != null)
                PaintInternal(gr, drawedByPen);
        }

        private void PaintInternal(Graphics gr, Pen pen)
        {
            gr.DrawRectangle(pen, rectangle);
        }

        public void Erase(Graphics gr, Pen erasePen)
        {
            throw new System.NotImplementedException();
        }

    }
}
