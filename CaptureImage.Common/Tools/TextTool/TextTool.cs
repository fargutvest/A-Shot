using CaptureImage.Common.DrawingContext;
using System.Drawing;
using System.Windows.Forms;
using System;
using CaptureImage.Common.Helpers;
using System.Collections.Generic;
using Text = CaptureImage.Common.Drawings.Text;

namespace CaptureImage.Common.Tools
{
    public class TextTool : ITool, ITextTool
    {
        private readonly Color colorOfCursor = Color.DimGray;
        private Text text;
        private readonly Timer cursorTimer;
        private Point textCursorUp = new Point(0, 0);
        private Point textCursorDown = new Point(0, 20);
        private bool textCursorVisible;
        private Rectangle textAreaRect;
        private readonly List<char> chars;
        private bool isMouseDown;
        private readonly int topPaddingText = 5;
        private readonly int leftPaddingText = 5;
        private readonly int rightPaddingText = 20;
        private readonly int symbolWidth = 10;
        private bool isActive;
        private Point mousePosition;
        private ICanvas canvas;
        private readonly IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;
        
        public TextTool(IDrawingContextProvider drawingContextProvider, ICanvas canvas)
        {
            this.drawingContextProvider = drawingContextProvider;
            this.canvas = canvas;
            chars = new List<char>();

            this.cursorTimer = new Timer();
            cursorTimer.Interval = 500;
            cursorTimer.Tick += CursorTimer_Tick;
            cursorTimer.Start();
        }

        #region ITool

        public void MouseMove(Point mouse)
        {
            if (isMouseDown)
                mousePosition = mouse;

            bool isMouseOver = textAreaRect.Contains(mouse);

            if (isActive)
            {
                if (isMouseDown)
                    Refresh();
                

                if (isMouseOver)
                    Cursor.Current = Cursors.SizeAll;
                
                else
                    Cursor.Current = Cursors.Default;
            }
        }

        public void MouseUp(Point mouse)
        {
            if (isActive)
            {
                isMouseDown = false;
                mousePosition = mouse;
                Refresh();
            }
        }

        public void MouseDown(Point mouse)
        {
            if (isActive)
            {
                isMouseDown = true;
                mousePosition = mouse;

                if (textAreaRect.Contains(mouse) == false)
                    RememberText();
            }
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            cursorTimer.Stop();
            isActive = false;
            RememberText();
        }

        #endregion

        #region ITextTool

        public void KeyPress(char keyChar)
        {
            chars.Add(keyChar); 
            Refresh();
        }

        #endregion

        #region private 

        private void RememberText()
        {
            chars.Clear();

            if (text != null)
                DrawingContext.RenderDrawing(text, needRemember: true);
        }

        private void Paint(Graphics gr, Image canvasImage, Color textColor)
        {
            CalculateForPaint();

            gr.DrawImage(canvasImage, textAreaRect, textAreaRect, GraphicsUnit.Pixel);

            Rectangle borderRect = new Rectangle(textAreaRect.Location, textAreaRect.Size);
            borderRect.Width -= 1;
            borderRect.Height -= 1;
            GraphicsHelper.DrawBorder(gr, borderRect);

            using (Pen pen = new Pen(colorOfCursor))
            {
                if (textCursorVisible)
                    gr.DrawLine(pen, textCursorUp, textCursorDown);

                Point textLocation = textAreaRect.Location;
                textLocation.X += topPaddingText;
                textLocation.Y += leftPaddingText;

                text = new Text(new string(chars.ToArray()), textColor, textAreaRect.Location);
                text.Paint(gr, null);
            }
        }

        private void CalculateForPaint()
        {
            int textAreaHeight = 70;
            textAreaRect = new Rectangle(mousePosition, new Size(0, textAreaHeight));
            textAreaRect.Width = chars.Count * symbolWidth + leftPaddingText + rightPaddingText;

            textCursorUp = textAreaRect.Location;
            textCursorUp.X += chars.Count * symbolWidth;
            textCursorDown = textCursorUp;
            textCursorDown.Y += 20;
        }
        
        private void CursorTimer_Tick(object sender, EventArgs e)
        {
            textCursorVisible = !textCursorVisible;
            Refresh();
        }

        private void Refresh()
        {
            DrawingContext.RenderDrawing(null, needRemember: false);

            Image image = canvas.GetThumb.Bounds.Contains(mousePosition) ? DrawingContext.GetImage() : DrawingContext.GetCanvasImage();
            DrawingContext.Draw((gr, _) => Paint(gr, image, DrawingContext.GetColorOfPen()), DrawingTarget.Canvas);
        }

        #endregion
    }
}
