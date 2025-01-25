using System.Drawing;
using System.Drawing.Drawing2D;

namespace CaptureImage.Common.Drawings
{
    public class Arrow : IDrawing
    {
        private Point start;
        private Point end;
        private CustomLineCap endCup;
        private Pen drawedByPen;

        public Arrow(Point start, Point end, CustomLineCap endCup)
        {
            this.start = start;
            this.end = end;
            this.endCup = endCup;
        }

        public void Erase(Graphics gr, Pen erasePen)
        {
            throw new System.NotImplementedException();
        }

        public void Paint(Graphics gr, Pen pen)
        {
            using (Pen drawingPen = pen.Clone() as Pen)
            {
                drawingPen.CustomEndCap = endCup;
                gr.DrawLine(drawingPen, start, end);
                drawedByPen = pen.Clone() as Pen;
            }
        }

        public void Repaint(Graphics gr)
        {
            if (drawedByPen != null)
                gr.DrawLine(drawedByPen, start, end);
        }

        public override string ToString()
        {
            return $"{start} - {end}";
        }
    }
}
