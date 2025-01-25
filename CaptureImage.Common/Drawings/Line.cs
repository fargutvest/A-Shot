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

            gr.FillEllipse(pen.Brush, start.X - pen.Width / 2, start.Y - pen.Width / 2, pen.Width, pen.Width);
            gr.FillEllipse(pen.Brush, end.X - pen.Width /2, end.Y - pen.Width / 2, pen.Width, pen.Width);
        }

        public override string ToString()
        {
            return $"{start} - {end}";
        }
    }
}
