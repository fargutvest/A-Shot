using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Drawings;
using CaptureImage.Common.Helpers;
using CaptureImage.Common.Tools.Misc;
using System.Drawing;

namespace CaptureImage.Common.Tools
{
    public class PencilTool : ITool
    {
        private Curve curve;
        private DrawingState state;
        private Point mouseStartPos;
        private Point mousePreviousPos;
        private bool isActive;
        private IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;

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
                DrawingContext.ReRenderCanvas();

                Line line = null;
                if (state == DrawingState.Drawing)
                {
                    line = new Line(mousePreviousPos, mouse);
                    curve.AddLine(line);
                    DrawingContext.Draw(line.Paint);
                }

                MarkerDrawingHelper.DrawMarker(DrawingContext, mouse);
                mousePreviousPos = mouse;
            }
        }

        public void MouseDown(Point mouse)
        {
            if (isActive)
            {
                curve = new Curve();
                mouseStartPos = mouse;
                mousePreviousPos = mouse;
                state = DrawingState.Drawing;
            }
        }

        public void MouseUp(Point mouse)
        {
            if (isActive)
            {
                DrawingContext.Drawings.Add(curve);
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
