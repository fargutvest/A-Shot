using CaptureImage.Common.DrawingContext;
using System.Drawing;
using System.Windows.Forms;
using System;
using CaptureImage.Common.Drawings;

namespace CaptureImage.Common.Tools
{
    public class TextTool : ITool, IKeyInputReceiver
    {
        private TextEditor textEditor;
        private Text text;

        private Point relativeMouseStartPos;
        private bool isMouseDown;
        private Point mousePosition;
        
        private bool isActive;

        private readonly IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;

        public TextTool(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
        }

        private void TextEditor_Updated(object sender, EventArgs e)
        {
            ReRender();
        }


        #region ITool

        public void MouseMove(Point mouse)
        {
            if (isMouseDown)
                mousePosition = mouse;

            bool isMouseOver = textEditor.Bounds.Contains(mouse);

            if (isActive)
            {
                if (isMouseDown)
                    ReRender();
                
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
                ReRender();
            }
        }

        public void MouseDown(Point mouse)
        {
            if (isActive)
            {
                isMouseDown = true;
                mousePosition = mouse;
                
                if (textEditor.Bounds.Contains(mouse))
                    relativeMouseStartPos = textEditor.Translate(mousePosition);
                else
                    RememberText();
            }
        }

        public void Activate()
        {
            textEditor = new TextEditor();
            textEditor.Updated += TextEditor_Updated;
            isActive = true;
        }

        public void Deactivate()
        {
            textEditor.Updated -= TextEditor_Updated;
            textEditor.Dispose();
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
            textEditor.CleanText();

            if (text != null)
            {
                text.ResetHighlight();
                text.ShowBorder = false;
                text.ShowCursor = false;
                DrawingContext.RenderDrawing(text, needRemember: true);
            }
        }

        private void ReRender()
        {
            Point drawingLocation = new Point(mousePosition.X - relativeMouseStartPos.X, mousePosition.Y - relativeMouseStartPos.Y);
            text = textEditor.GetDrawing(DrawingContext.GetColorOfPen(), drawingLocation);
            DrawingContext.RenderDrawing(text, needRemember: false);
        }


        #endregion
    }
}
