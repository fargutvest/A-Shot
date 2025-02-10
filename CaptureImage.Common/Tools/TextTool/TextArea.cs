using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CaptureImage.Common.Tools
{
    public partial class TextArea : UserControl
    {
        private readonly Timer cursorTimer;
        private bool textCursorVisible;
        private readonly IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;

        private List<char> pressedKeys;
        private Point textCursorUp = new Point(0, 0);
        private Point textCursorDown = new Point(0, 20);

        public TextArea(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
            InitializeComponent();

            pressedKeys = new List<char>();

            this.cursorTimer = new Timer();
            cursorTimer.Interval = 500;
            cursorTimer.Tick += new EventHandler(CursorTimer_Tick);
            cursorTimer.Start();
        }

        public void DrawBorder(Graphics gr)
        {
            Rectangle rect = new Rectangle(0,0, this.Width - 1, this.Height -1);
            GraphicsHelper.DrawBorder(gr, rect);
        }

        public void OnGraphics(DrawingContext.DrawingContext.OnGraphicsDelegate toDo)
        {
            using (Graphics gr = this.CreateGraphics())
            {
                toDo?.Invoke(gr, this.ClientRectangle);
            }
        }


        private void CursorTimer_Tick(object sender, EventArgs e)
        {
            textCursorVisible = !textCursorVisible;
            this.Refresh();
        }

        private void TextArea_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            GraphicsHelper.OnBufferedGraphics(e.Graphics, this.ClientRectangle, bufferedGr =>
            {
                DrawBackgroundImage(bufferedGr, DrawingContext.GetCanvasImage());
                DrawBorder(bufferedGr);

                using (Pen pen = new Pen(Color.DimGray))
                {
                    if (textCursorVisible)
                        bufferedGr.DrawLine(pen, textCursorUp, textCursorDown);

                    DrawText(bufferedGr, new string(pressedKeys.ToArray()), new Point(0,0));
                }
            });
        }

        public void DrawBackgroundImage(Graphics gr, Image image)
        {
            gr.DrawImage(image, ClientRectangle, Bounds, GraphicsUnit.Pixel);
        }

        public void Refresh(Point location)
        {
            this.Visible = false;
            this.Location = location;
            this.Visible = true;
        }

        private void DrawText(Graphics gr, string text, Point mouse)
        {
            int offsetX = 10;
            int offsetY = 10;

            using (Font font = new Font("Arial", 12))
            {
                using (Brush brush = new SolidBrush(DrawingContext.GetColorOfPen()))
                {
                    gr.DrawString(text, font, brush, mouse.X + offsetX, mouse.Y + offsetY);
                }
            }
        }

        public new void KeyPress(char keyChar)
        {
            pressedKeys.Add(keyChar);
            this.Width += 10;

            this.textCursorUp.X += 10;
            this.textCursorDown.X += 10;
        }
    }
}
