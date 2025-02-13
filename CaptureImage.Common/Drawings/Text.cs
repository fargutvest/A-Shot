using CaptureImage.Common.Helpers;
using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    public class Text: IDrawing
    {
        private string text;
        private Point location;
        private bool isDrawed;
        private Color color;

        public Text(string text, Color color, Point location)
        {
            this.text = text;
            this.color = color;
            this.location = location; 
        }

        public void Paint(Graphics gr, Pen pen)
        {
            PaintInternal(gr);
            isDrawed = true;
        }

        public void Repaint(Graphics gr)
        {
            PaintInternal(gr);
        }

        public void Erase(Graphics gr, Pen erasePen)
        {
            PaintInternal(gr);
        }

        private void PaintInternal(Graphics gr)
        {
            using (Font font = new Font("Arial", MarkerDrawingHelper.GetPenDiameter() * 5))
            {
                using (Brush brush = new SolidBrush(color))
                {
                    gr.DrawString(text, font, brush, location.X, location.Y);
                }
            }
        }

        #region override

        public override string ToString()
        {
            return $"{text}";
        }

        #endregion
    }
}
