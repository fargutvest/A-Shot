﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace CaptureImage.Common.Helpers
{
    public static class GraphicsHelper
    {
        public static Rectangle[] DrawBorderWithHandles(Graphics gr, Rectangle rect, int handleSize = 5)
        {
            Pen solidPen = new Pen(Color.Black, 1);  
            Pen dashedPen = new Pen(Color.White, 1);  
            dashedPen.DashPattern = new float[] { 3, 3 };

            gr.DrawRectangle(solidPen, rect);  
            gr.DrawRectangle(dashedPen, rect);

            int halfHandleSizeMin = handleSize / 2;
            int halfHandleSizeMax = halfHandleSizeMin + handleSize % 2;
            int handleSizeDiff = halfHandleSizeMax - halfHandleSizeMin;

            int halfRectWidthMin = rect.Width / 2;
            int halfRectWidthMax = halfRectWidthMin + rect.Width % 2;
            int halfRectWidthDiff = halfRectWidthMax - halfRectWidthMin;

            int halfRectHeightMin = rect.Height / 2;
            int halfRectHeightMax = halfRectHeightMin + rect.Height % 2;
            int halfRectHeightDiff = halfRectHeightMax - halfRectHeightMin;

            int antiShakeOffsetX = halfRectWidthDiff == 0 ? handleSizeDiff : handleSizeDiff * 2;
            int antiShakeOffsetY = halfRectHeightDiff == 0 ? handleSizeDiff : handleSizeDiff * 2;

            var rectangles = new List<Rectangle>();
            var r1 = new Rectangle(rect.Left - halfHandleSizeMax, rect.Top - halfHandleSizeMax, handleSize, handleSize); rectangles.Add(r1); // угол
            r1.Offset(rect.Width / 2, 0); rectangles.Add(r1);
            r1.Offset(rect.Width / 2 + antiShakeOffsetX, 0); rectangles.Add(r1); // угол
            r1.Offset(0, rect.Height / 2 + antiShakeOffsetY); rectangles.Add(r1);
            r1.Offset(0, rect.Height / 2); rectangles.Add(r1); // угол
            r1.Offset(-rect.Width / 2 - antiShakeOffsetX, 0); rectangles.Add(r1);
            r1.Offset(-rect.Width / 2, 0); rectangles.Add(r1); // угол
            r1.Offset(0, -rect.Height / 2); rectangles.Add(r1);
            gr.FillRectangles(Brushes.Black, rectangles.ToArray());
            gr.DrawRectangles(Pens.White, rectangles.ToArray());

            return rectangles.ToArray();
        }

        public static void DrawBorder(Graphics gr, Rectangle rect)
        {
            Pen solidPen = new Pen(Color.Black, 1);
            Pen dashedPen = new Pen(Color.White, 1);
            dashedPen.DashPattern = new float[] { 3, 3 };

            gr.DrawRectangle(solidPen, rect);
            gr.DrawRectangle(dashedPen, rect);
        }

        public static void OnBufferedGraphics(Graphics gr, Rectangle bounds, Action<Graphics> action)
        {
            using (BufferedGraphics bufferedGraphics = BufferedGraphicsManager.Current.Allocate(gr, bounds))
            {
                action?.Invoke(bufferedGraphics.Graphics);
                bufferedGraphics.Render(gr);
            }
        }

        public static SizeF GetStringSize(Graphics gr, string str, string fontName, float fontSize)
        {
            using (Font font = new Font(fontName, fontSize))
            {
                SizeF textSize = gr.MeasureString(str, font);
                return textSize;
            }
        }

        public static float GetFirstSymbolOverWidth(Graphics gr, char firstChar, string fontName, float fontSize)
        {
            using (Font font = new Font(fontName, fontSize))
            {
                string oneChar = new string(firstChar, 1);
                string twoChar = new string(firstChar, 2);


                float oneCharWidth = gr.MeasureString(oneChar, font).Width;
                float twoCharWidth = gr.MeasureString(twoChar, font).Width;
                
                return (oneCharWidth * 2 - twoCharWidth) / 2;
                
            }
        }
    }
}
