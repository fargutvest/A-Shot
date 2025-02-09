using CaptureImage.Common.DrawingContext;
using System.Drawing;

namespace CaptureImage.Common.Tools
{
    public class TextTool : ITool
    {
        private readonly IDrawingContextProvider drawingContextProvider;

        public TextTool(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
        }

        public void Activate()
        {

        }

        public void Deactivate()
        {

        }

        public void MouseMove(Point mouse)
        {

        }

        public void MouseUp(Point mouse)
        {

        }

        public void MouseDown(Point mouse)
        {

        }
    }
}
