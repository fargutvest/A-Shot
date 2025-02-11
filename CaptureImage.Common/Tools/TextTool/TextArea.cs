using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common.Drawings;

namespace CaptureImage.Common.Tools
{
    public partial class TextArea : UserControl
    {
        private readonly Timer cursorTimer;
        private bool textCursorVisible;
        private readonly IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;

        public List<char> Chars;
        private Point textCursorUp = new Point(0, 0);
        private Point textCursorDown = new Point(0, 20);

        private int width;
        private int height;

        public TextArea(IDrawingContextProvider drawingContextProvider)
        {
            width = this.Width;
            height = this.Height;
            this.drawingContextProvider = drawingContextProvider;

            drawingContextProvider.mouseHookHelper.MouseWheel += MouseHookHelper_MouseWheel;
            
            InitializeComponent();

            Chars = new List<char>();

            this.cursorTimer = new Timer();
            cursorTimer.Interval = 500;
            cursorTimer.Tick += new EventHandler(CursorTimer_Tick);
            cursorTimer.Start();
        }

        private void MouseHookHelper_MouseWheel(object sender, int e)
        {
            CalculateSize();
            Refresh();
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
            Width = width;
            Height = height;

            GraphicsHelper.OnBufferedGraphics(e.Graphics, this.ClientRectangle, bufferedGr =>
            {
                DrawBackgroundImage(bufferedGr, DrawingContext.GetCanvasImage());
                DrawBorder(bufferedGr);

                using (Pen pen = new Pen(Color.DimGray))
                {
                    if (textCursorVisible)
                        bufferedGr.DrawLine(pen, textCursorUp, textCursorDown);

                    Text text = new Text(new string(Chars.ToArray()), DrawingContext.GetColorOfPen(), new Point(0,0));
                    text.Paint(bufferedGr, null);
                }
            });
        }

        public void DrawBackgroundImage(Graphics gr, Image image)
        {
            gr.DrawImage(image, ClientRectangle, Bounds, GraphicsUnit.Pixel);
        }

        public void Refresh(Point location)
        {
            Chars.Clear();
            this.Visible = false;
            this.Location = location;
            this.Visible = true;
        }

        public new void KeyPress(char keyChar)
        {
            Chars.Add(keyChar);
            width += 10;

            this.textCursorUp.X += 10;
            this.textCursorDown.X += 10;

            Refresh();
        }

        private void CalculateSize()
        {
            width =  MarkerDrawingHelper.GetPenDiameter() * 3;
            height = MarkerDrawingHelper.GetPenDiameter() * 7;
        }
    }
}
