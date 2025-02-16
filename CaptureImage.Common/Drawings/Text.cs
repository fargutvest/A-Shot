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
        private string fontName;
        private float fontSize;

        public Text(string text, string fontName, float fontSize, Color color, Point location)
        {
            this.text = text;
            this.fontName = fontName;
            this.fontSize = fontSize;
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
            using (Font font = new Font(fontName, fontSize))
            {
                using (Brush brush = new SolidBrush(color))
                {
                    gr.DrawString(text, font, brush, location.X, location.Y);
                }
            }
        }

        private void HighlightSubstring(Graphics gr, string substring)
        {
            using (Font font = new Font(fontName, fontSize))
            {
                SizeF textSize = gr.MeasureString(text, font);
                SizeF substringSize = gr.MeasureString(substring, font);
                Point substringLocation = location;

                using (Brush highlightBrush = new SolidBrush(Color.Yellow))
                {
                    gr.FillRectangle(highlightBrush, new RectangleF(substringLocation, textSize));
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
