using CaptureImage.Common.Helpers;
using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    public class Text: IDrawing
    {
        private string text;
        private string substrToHighLight;
        private Point location;
        private bool isDrawed;
        private Color color;
        private string fontName;
        private float fontSize;

        public Text(string text, string fontName, float fontSize, Color color, Point location) :
            this(text, string.Empty, fontName, fontSize, color, location) { }
        

        public Text(string text, string substrToHighLight, string fontName, float fontSize, Color color, Point location)
        {
            this.text = text;
            this.substrToHighLight = substrToHighLight;
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
                HighlightSubstring(gr, font, substrToHighLight);

                using (Brush brush = new SolidBrush(color))
                {
                    gr.DrawString(text, font, brush, location.X, location.Y);
                }
            }
        }

        private void HighlightSubstring(Graphics gr, Font font, string substr)
        {
            SizeF textSize = gr.MeasureString(text, font);
            SizeF substrSize = gr.MeasureString(substr, font);

            int substrIndex = text.IndexOf(substr);
            string textBeforeSubstr = text.Substring(0, substrIndex);

            SizeF textBeforuSubstrSize = gr.MeasureString(textBeforeSubstr, font); 
            
            Point substrLocation = location;
            substrLocation.X += (int)textBeforuSubstrSize.Width;

            using (Brush highlightBrush = new SolidBrush(Color.Indigo))
            {
                gr.FillRectangle(highlightBrush, new RectangleF(substrLocation, substrSize));
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
