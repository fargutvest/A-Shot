using CaptureImage.Common.Helpers;
using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    public class Text: IDrawing
    {
        private readonly Color highlightColor = Color.Indigo;
        private readonly string text;
        private int startIndexToHighlight;
        private int lengthToHighlight;
        private Point location;
        private bool isDrawed;
        private readonly Color color;
        private readonly string fontName;
        private readonly float fontSize;
        

        public Text(string text, string fontName, float fontSize, Color color, Point location)
        {
            this.text = text;
            this.fontName = fontName;
            this.fontSize = fontSize;
            this.color = color;
            this.location = location; 
        }

        public void Highlight(int startIndexToHighlight, int lengthToHighlight)
        {
            this.startIndexToHighlight = startIndexToHighlight;
            this.lengthToHighlight = lengthToHighlight;
        }

        public void ResetHighlight()
        {
            this.startIndexToHighlight = 0;
            this.lengthToHighlight = 0;
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

        #region private

        private void PaintInternal(Graphics gr)
        {
            using (Font font = new Font(fontName, fontSize))
            {
                HighlightSubstring(gr, font);

                using (Brush brush = new SolidBrush(color))
                {
                    gr.DrawString(text, font, brush, location.X, location.Y);
                }
            }
        }

        private void HighlightSubstring(Graphics gr, Font font)
        {
            if (lengthToHighlight == 0)
                return; 

            string substr = text.Substring(startIndexToHighlight, lengthToHighlight);
            float overWidth = GraphicsHelper.GetFirstSymbolOverWidth(gr, substr[0], fontName, fontSize);
            SizeF substrSize = gr.MeasureString(substr, font);

            string textBeforeSubstr = text.Substring(0, startIndexToHighlight);
            SizeF textBeforeSubstrSize = gr.MeasureString(textBeforeSubstr, font);
            
            Point substrLocation = location;

            if (textBeforeSubstrSize.Width > 0)
            {
                substrSize.Width -= overWidth * 2;
                substrLocation.X += (int)textBeforeSubstrSize.Width - (int)overWidth;
            }
            else if (textBeforeSubstrSize.Width == 0)
            {
                substrSize.Width -= overWidth;
            }

            using (Brush highlightBrush = new SolidBrush(highlightColor))
            {
                gr.FillRectangle(highlightBrush, new RectangleF(substrLocation, substrSize));
            }
        }

        #endregion

        #region override

        public override string ToString()
        {
            return $"{text}";
        }

        #endregion
    }
}
