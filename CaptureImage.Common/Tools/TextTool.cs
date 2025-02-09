using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Tools.Misc;
using System.Drawing;

namespace CaptureImage.Common.Tools
{
    public class TextTool : ITool
    {
        private readonly IDrawingContextProvider drawingContextProvider;
        private DrawingState state;
        private bool isActive;


        public TextTool(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
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

            }
        }

        public void MouseDown(Point mouse)
        {
            if (isActive)
            {

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
    }
}
