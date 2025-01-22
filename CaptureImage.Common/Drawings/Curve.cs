using System.Collections.Generic;
using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    public class Curve : IDrawing
    {
        private List<Line> lines;

        public Curve()
        {
            lines = new List<Line>();
        }

        public void AddLine(Line line)
        {
            lines.Add(line);
        }

        public void Paint(Graphics gr, Pen pen)
        {
            foreach (Line line in lines)
                line.Paint(gr, pen);   
        }
    }
}
