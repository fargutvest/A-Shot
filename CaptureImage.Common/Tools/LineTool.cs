using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Drawings;
using CaptureImage.Common.Helpers;
using CaptureImage.Common.Tools.Misc;
using System.Drawing;

namespace CaptureImage.Common.Tools
{
    public class LineTool : ITool
    {
        private Line line = null;
        protected DrawingState state;
        protected Point mouseStartPos;
        protected Point mousePreviousPos;
        protected bool isActive;


        protected IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;
        

        public LineTool(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
            this.state = DrawingState.None;
            mouseStartPos = new Point(0, 0);
        }

        public virtual void MouseDown(Point mousePosition)
        {
            if (isActive)
            {
                mouseStartPos = mousePosition;
                state = DrawingState.Drawing;
            }
        }

        public virtual void MouseMove(Point mouse)
        {
            if (isActive)
            {
                MarkerDrawingHelper.EraseMarker(DrawingContext);

                if (state == DrawingState.Drawing)
                {
                    DrawingContext.Erase(line, DrawingTarget.CanvasOnly);
                    line = new Line(mouseStartPos, mouse);
                    DrawingContext.Draw(line.Paint, DrawingTarget.CanvasOnly);
                }

                MarkerDrawingHelper.DrawMarker(DrawingContext, mouse);
                mousePreviousPos = mouse;
            }
        }

        public virtual void MouseUp(Point mouse)
        {
            if (isActive)
            {
                DrawingContext.Drawings.Add(line);
                DrawingContext.ReRenderDrawings(DrawingTarget.ImageOnly);
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
