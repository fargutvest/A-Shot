using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    public class Line : IDrawing
    {
        public Point Start { get; }
        public Point End { get; }

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public void Paint(Graphics gr, Pen pen)
        {
            gr.DrawLine(pen, Start, End);
        }

        public override string ToString()
        {
            return $"{Start} - {End}";
        }
    }
}
