using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Tools.Misc;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common.Drawings;
using System;

namespace CaptureImage.Common.Tools
{
    public class TextTool : ITool, ITextTool
    {
        private readonly TextArea textArea;
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
            textArea = new TextArea(drawingContextProvider);
            textArea.Parent = canvas as Control;

            textArea.MouseDown += TextArea_MouseDown;
            textArea.MouseUp += TextArea_MouseUp;
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
            mousePos = mousePosition;
            IsMouseOver = textArea.Bounds.Contains(mousePos);

            if (isActive)
            {
                if (IsMouseOver)
                {
                    textArea.Cursor = Cursors.SizeAll;

                    if (isMoving)
                        textArea.Location = new Point(mouse.X, mouse.Y);
                }
                else
                {
                    textArea.Cursor = Cursors.Default;
                }
            }
        }

        public void MouseUp(Point mouse)
        {
            if (isActive)
            {
                SaveText();
                mousePosition = mouse;
                textArea.Refresh(mousePosition);
                textArea.Show();
            }
        }

        public void MouseDown(Point mouse)
        {
            if (isActive)
            {
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
            textArea.Hide();
            SaveText();
        }

        private void SaveText()
        {
            Text text = new Text(new string(textArea.Chars.ToArray()), DrawingContext.GetColorOfPen(), mousePosition);
            DrawingContext.RenderDrawing(text, needRemember: true);
        }

        #region ITextTool

        public void KeyPress(char keyChar)
        {
            textArea.KeyPress(keyChar);
        }

        #endregion
    }
}
