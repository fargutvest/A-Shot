using System.Drawing;
using System.Drawing.Drawing2D;

namespace CaptureImage.Common.Drawings
{
    public class Arrow : IDrawing
    {
        private Point start;
        private Point end;
        private CustomLineCap endCup;

        public Arrow(Point start, Point end, CustomLineCap endCup)
        {
            this.start = start;
            this.end = end;
            this.endCup = endCup;
        }

        public void Paint(Graphics gr, Pen pen)
        {
            using (Pen drawingPen = pen.Clone() as Pen)
            {
                drawingPen.CustomEndCap = endCup;
                gr.DrawLine(drawingPen, start, end);
            }
        }

        public override string ToString()
        {
            return $"{start} - {end}";
        }
    }
}
