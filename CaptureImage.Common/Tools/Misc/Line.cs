using System.Drawing;

namespace CaptureImage.Common.Tools.Misc
{
    public class Line
    {
        public Point Start { get; }
        public Point End { get; }

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return $"{Start} - {End}";
        }
    }
}
