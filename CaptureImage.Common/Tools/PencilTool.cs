using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Tools.Misc;
using System.Drawing;
using System.Windows.Forms;


namespace CaptureImage.Common.Tools
{
    public class PencilTool : ITool
    {
        private DrawingState state;
        private Point mousePreviousPos;
        private bool isActive;
        protected IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContextsKeeper.DrawingContext;

        public PencilTool(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
            this.state = DrawingState.None;
            mousePreviousPos = new Point(0, 0);
        }

        public void MouseMove(Point mouse)
        {
            if (isActive)
            {
                if (state == DrawingState.Drawing)
                {
                    for (int i = 0; i < DrawingContext.CanvasImages.Length; i++)
                    {
                        Image im = DrawingContext.CanvasImages[i];
                        Control ct = DrawingContext.CanvasControls[i];

                        Graphics.FromImage(im).DrawLine(DrawingContext.DrawingPen, mousePreviousPos, mouse);
                        ct.CreateGraphics().DrawLine(DrawingContext.DrawingPen, mousePreviousPos, mouse);
                        DrawingContext.IsClean = false;
                    }

                    mousePreviousPos = mouse;
                }
            }
        }

        public void MouseDown(Point mousePosition)
        {
            if (isActive)
            {
                mousePreviousPos = mousePosition;
                state = DrawingState.Drawing;
            }
        }

        public void MouseUp()
        {
            if (isActive)
            {
                drawingContextProvider.DrawingContextsKeeper.SaveContext();
                state = DrawingState.None;
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
