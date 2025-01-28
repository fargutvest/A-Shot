using System.Drawing;
using System.Drawing.Drawing2D;

namespace CaptureImage.Common.Drawings
{
    public class Arrow : IDrawing
    {
        private Point start;
        private Point end;
        private CustomLineCap endCup;
        private Color drawedPenColor;
        private float drawedPenWidth;
        private bool isDrawed;

        public Arrow(Point start, Point end, CustomLineCap endCup)
        {
            this.start = start;
            this.end = end;
            this.endCup = endCup;
        }

        public void Erase(Graphics gr, Pen erasePen)
        {
            if (isDrawed)
            {
                using (Pen pen = erasePen.Clone() as Pen)
                {
                    erasePen.Width = drawedPenWidth;
                    erasePen.CustomEndCap = endCup;
                    PaintInternal(gr, erasePen);
                }
            }
        }

        public void Paint(Graphics gr, Pen pen)
        {
            using (Pen drawingPen = pen.Clone() as Pen)
            {
                drawingPen.CustomEndCap = endCup;
                PaintInternal(gr, drawingPen);
                isDrawed = true;
                drawedPenColor = pen.Color;
                drawedPenWidth = pen.Width;
            }
        }

        public void Repaint(Graphics gr)
        {
            if (isDrawed)
            {
                using (Pen pen = new Pen(drawedPenColor, drawedPenWidth) { CustomEndCap = endCup })
                    PaintInternal(gr, pen);
            }
        }

        private void PaintInternal(Graphics gr, Pen pen)
        {
            gr.DrawLine(pen, start, end);
            gr.FillEllipse(pen.Brush, start.X - pen.Width / 2, start.Y - pen.Width / 2, pen.Width, pen.Width);
        }

        public override string ToString()
        {
            return $"{start} - {end}";
        }
    }
}
