﻿using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CaptureImage.Common.Tools
{
    public partial class TextArea : UserControl
    {
        private Timer cursorTimer;
        private bool textCursorVisible;
        private readonly IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;

        public TextArea(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
            InitializeComponent();

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
                toDo?.Invoke(gr);
            }
        }

        
        private void CursorTimer_Tick(object sender, EventArgs e)
        {
            OnGraphics(gr =>
            {
                textCursorVisible = !textCursorVisible;
                this.Refresh();
            });
        }

        private void TextArea_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            DrawBackgroundImage( e.Graphics, DrawingContext.GetCanvasImage());
            DrawBorder(e.Graphics);

            using (Pen pen = new Pen(Color.DimGray))
            {
                Point textCursorUp = new Point(0, 0);
                Point textCursorDown = new Point(0, 20);

                if (textCursorVisible)
                    e.Graphics.DrawLine(pen, textCursorUp, textCursorDown);
            }
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
    }
}
