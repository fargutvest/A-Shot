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
            this.textEditor = new TextEditor();
            textEditor.Updated += TextEditor_Updated;

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
                    relativeMouseStartPos = textEditor.Bounds.Location.IsEmpty ? Point.Empty :
                        new Point(mousePosition.X - textEditor.Bounds.X, mousePosition.Y - textEditor.Bounds.Y);
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
            textEditor.CleanText();

            if (text != null)
            {
                text.ResetHighlight();
                DrawingContext.RenderDrawing(text, needRemember: true);
            }
        }

        private void ReRender()
        {
            DrawingContext.RenderDrawing(null, needRemember: false);
            DrawingContext.Draw((gr, _) =>
            {
                text = textEditor.Render(gr, DrawingContext.GetColorOfPen(), mousePosition, relativeMouseStartPos);
            },
            DrawingTarget.Canvas);
        }


        #endregion
    }
}
