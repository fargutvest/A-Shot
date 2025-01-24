using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Drawings;
using CaptureImage.Common.Helpers;
using CaptureImage.Common.Tools.Misc;
using System.Drawing;

namespace CaptureImage.Common.Tools
{
    public class LineTool : ITool
    {
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
            DrawingContext.UpdateErasingPens();
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
               // MarkerDrawingHelper.EraseMarker(DrawingContext, mousePreviousPos);

                Line line = null;
                if (state == DrawingState.Drawing)
                {
                    DrawingContext.Erase(new Line(mouseStartPos, mousePreviousPos).Paint);
                    line = new Line(mouseStartPos, mouse);
                    DrawingContext.Draw(line.Paint);
                }

               // MarkerDrawingHelper.DrawMarker(DrawingContext, line, mouse);
                mousePreviousPos = mouse;
            }
        }

        public virtual void MouseUp(Point mouse)
        {
            if (isActive)
            {
                Line line = new Line(mouseStartPos, mousePreviousPos);
                DrawingContext.Drawings.Add(line);
                DrawingContext.UpdateErasingPens();
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
