using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Tools.Misc;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common.Drawings;

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
      
        public TextTool(IDrawingContextProvider drawingContextProvider, ICanvas canvas)
        {
            this.drawingContextProvider = drawingContextProvider;
            this.canvas = canvas;
            this.state = DrawingState.None;
            textArea = new TextArea(drawingContextProvider);
            textArea.Parent = canvas as Control;
        }
        
        public void MouseMove(Point mouse)
        {
            if (isActive)
            {

            }
        }

        public void MouseUp(Point mouse)
        {
            if (isActive)
            {
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
