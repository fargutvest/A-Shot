using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Tools.Misc;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common.Drawings;
using System;
using CaptureImage.Common.Helpers;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using Text = CaptureImage.Common.Drawings.Text;

namespace CaptureImage.Common.Tools
{
    public class TextTool : ITool, ITextTool
    {
        //private readonly TextArea textArea;
        private DrawingState state;
        private bool isActive;
        private Point mousePosition;
        private ICanvas canvas;
        private readonly IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;

        bool isMoving;


        private bool isMouseOver;

        private Point mousePos;

        public bool IsMouseOver
        {
            get
            {
                return isMouseOver;
            }
            private set
            {
                if (isMouseOver != value)
                {
                    isMouseOver = value;

                    if (isMouseOver)
                        MouseEnterSelection?.Invoke(this, mousePos);
                }
            }
        }

        public event EventHandler<Point> MouseEnterSelection;


        public TextTool(IDrawingContextProvider drawingContextProvider, ICanvas canvas)
        {
            this.drawingContextProvider = drawingContextProvider;
            this.canvas = canvas;
            this.state = DrawingState.None;

            Chars = new List<char>() { 'П', 'р', 'и', 'в', 'е', 'т' };
            // textArea = new TextArea(drawingContextProvider);
            // textArea.Parent = canvas as Control;

            //  textArea.MouseDown += TextArea_MouseDown;
            //  textArea.MouseUp += TextArea_MouseUp;


        }

        private void TextArea_MouseUp(object sender, MouseEventArgs e)
        {
            isMoving = false;
        }

        private void TextArea_MouseDown(object sender, MouseEventArgs e)
        {
            isMoving = true;
        }

        public void MouseMove(Point mouse)
        {
            mousePos = mouse;
           // IsMouseOver = textArea.Bounds.Contains(mousePos);

            if (isActive)
            {
                if (isMouseDown)
                {
                    this.textAreaRect = new Rectangle(mousePos, new Size(200, 200));
                    DrawingContext.RenderDrawing(null, needRemember: false);
                    DrawingContext.Draw((gr, pen) => Paint(gr), DrawingTarget.Canvas);
                }

                if (IsMouseOver)
                {
                   // textArea.Cursor = Cursors.SizeAll;

                    //if (isMoving)
                    //    textArea.Location = new Point(mouse.X, mouse.Y);
                }
                else
                {
                   // textArea.Cursor = Cursors.Default;
                }
            }
        }

        public void MouseUp(Point mouse)
        {
            if (isActive)
            {
                isMouseDown = false;
                SaveText();
                mousePosition = mouse;
                this.textAreaRect = new Rectangle(mouse, new Size(200, 200));
                DrawingContext.RenderDrawing(null, needRemember: false);
                DrawingContext.Draw((gr, pen) => Paint(gr), DrawingTarget.Canvas);

                //textArea.Refresh(mousePosition);
                //textArea.Show();
            }
        }

        public void MouseDown(Point mouse)
        {
            if (isActive)
            {
                isMouseDown = true;
                mousePosition = mouse;
            }
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            isActive = false;
           // textArea.Hide();
            SaveText();
        }

        private void SaveText()
        {
           // Text text = new Text(new string(textArea.Chars.ToArray()), DrawingContext.GetColorOfPen(), mousePosition);
         //   DrawingContext.RenderDrawing(text, needRemember: true);
        }

        #region ITextTool

        public void KeyPress(char keyChar)
        {
           // textArea.KeyPress(keyChar);
        }

        #endregion

        private Point textCursorUp = new Point(0, 0);
        private Point textCursorDown = new Point(0, 20);
        private bool textCursorVisible;

        private Rectangle textAreaRect;
        public List<char> Chars;

        private bool isMouseDown;

        private void Paint(Graphics gr)
        {
            GraphicsHelper.OnBufferedGraphics(gr, this.textAreaRect, bufferedGr =>
            {
                bufferedGr.DrawImage(DrawingContext.GetCanvasImage(), textAreaRect, textAreaRect, GraphicsUnit.Pixel);

                Rectangle borderRect = new Rectangle(textAreaRect.Location, textAreaRect.Size);
                borderRect.Width -= 1;
                borderRect.Height -= 1;
                GraphicsHelper.DrawBorder(bufferedGr, borderRect);

                using (Pen pen = new Pen(Color.DimGray))
                {
                    if (textCursorVisible)
                        bufferedGr.DrawLine(pen, textCursorUp, textCursorDown);

                    Text text = new Text(new string(Chars.ToArray()), DrawingContext.GetColorOfPen(), textAreaRect.Location);
                    text.Paint(bufferedGr, null);
                }
            });
        }
    }
}
