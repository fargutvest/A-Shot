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

        public void Repaint(Graphics gr)
        {
            foreach (Line line in lines)
                line.Repaint(gr);
        }

        public void Erase(Graphics gr, Pen erasePen)
        {
            foreach (Line line in lines)
                line.Erase(gr, erasePen);
        }

        #region override

        public override string ToString()
        {
            return $"lines:{lines.Count}";
        }

        #endregion
    }
}
