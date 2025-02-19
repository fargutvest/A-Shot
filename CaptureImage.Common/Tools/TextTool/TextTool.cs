using CaptureImage.Common.DrawingContext;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace CaptureImage.Common.Tools
{
    public class TextTool : ITool, IKeyInputReceiver
    {
        private TextEditor textEditor;

        private Point relativeMouseStartPos;
        private bool isMouseDown;
        private Point mousePosition;
        
        private bool isActive;

        private readonly IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;

        public TextTool(IDrawingContextProvider drawingContextProvider)
        {
            this.textEditor = new TextEditor();
            textEditor.Refresh += TextEditor_Refresh;

            this.drawingContextProvider = drawingContextProvider;
        }

        private void TextEditor_Refresh(object sender, EventArgs e)
        {
            Refresh();
        }


        #region ITool

        public void MouseMove(Point mouse)
        {
            if (isMouseDown)
                mousePosition = mouse;

            bool isMouseOver = textEditor.textAreaRect.Contains(mouse);

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

                if (textEditor.textAreaRect.Contains(mouse))
                    relativeMouseStartPos = textEditor.textAreaRect.Location.IsEmpty ? Point.Empty :
                        new Point(mousePosition.X - textEditor.textAreaRect.X, mousePosition.Y - textEditor.textAreaRect.Y);
                else
                    RememberText();
            }
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            isActive = false;
            RememberText();
        }

        #endregion

        #region IKeyInputReceiver

        public void KeyPress(KeyPressEventArgs e)
        {
            textEditor.KeyPress(e);
        }

        public void KeyDown(KeyEventArgs e)
        {
            textEditor.KeyDown(e);
        }

        public void KeyUp(KeyEventArgs e)
        {
            textEditor.KeyUp(e);
        }

        public void MouseWheel(MouseEventArgs e)
        {
            textEditor.MouseWheel(e);
        }

        #endregion

        #region private 

        private void RememberText()
        {
            textEditor.chars.Clear();
            textEditor.numberOfCharWithCursor = 0;

            if (textEditor.text != null)
                DrawingContext.RenderDrawing(textEditor.text, needRemember: true);
        }

        private void Refresh()
        {
            DrawingContext.RenderDrawing(null, needRemember: false);
            DrawingContext.Draw((gr, _) =>  textEditor.Paint(gr, DrawingContext.GetColorOfPen(), mousePosition, relativeMouseStartPos), DrawingTarget.Canvas);
        }


        #endregion
    }
}
