using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Helpers;
using CaptureImage.Common.Tools.Misc;
using System.Drawing;

namespace CaptureImage.Common.Tools
{
    public class TextTool : ITool
    {
        private readonly IDrawingContextProvider drawingContextProvider;
        private DrawingState state;
        private bool isActive;
        private Point mousePosition;
        private ICanvas canvas;

        public TextTool(IDrawingContextProvider drawingContextProvider, ICanvas canvas)
        {
            this.drawingContextProvider = drawingContextProvider;
            this.canvas = canvas;
            this.state = DrawingState.None;
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
                canvas.OnGraphics(DrawBorder);
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
        }

        private void DrawBorder(Graphics gr)
        {
            Rectangle rect = new Rectangle(mousePosition.X, mousePosition.Y, 50, 50 );
            GraphicsHelper.DrawBorder(gr, rect);
        }
    }
}
