using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    public class Line : IDrawing
    {
        private Point start;
        private Point end;

        public Line(Point start, Point end)
        {
            this.start = start;
            this.end = end;
        }

        public void Paint(Graphics gr, Pen pen)
        {
            gr.DrawLine(pen, start, end);
        }

        public override string ToString()
        {
            return $"{start} - {end}";
        }
    }
}
