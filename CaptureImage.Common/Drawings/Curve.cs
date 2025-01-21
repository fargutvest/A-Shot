using System.Collections.Generic;
using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    public class Curve : IDrawing
    {
        public List<Line> Lines;

        public Curve()
        {
            Lines = new List<Line>();
        }

        public void Paint(Graphics gr, Pen pen)
        {
            foreach (Line line in Lines)
                line.Paint(gr, pen);   
        }
    }
}
