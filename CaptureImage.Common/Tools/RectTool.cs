using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Drawings;
using CaptureImage.Common.Helpers;
using CaptureImage.Common.Tools.Misc;
using System;
using System.Drawing;

namespace CaptureImage.Common.Tools
{
    public class RectTool : ITool
    {
        private Rect rect;
        private DrawingState state;
        private Point mouseStartPos;
        private bool isActive;
        private readonly IDrawingContextProvider drawingContextProvider;

        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;

        public RectTool(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
            this.state = DrawingState.None;
            mouseStartPos = new Point(0, 0);
        }

        public void MouseDown(Point mouse)
        {
            if (isActive)
            {
                mouseStartPos = mouse;
                state = DrawingState.Drawing;
            }
        }

        public void MouseMove(Point mouse)
        {
            if (isActive)
            {
                if (state == DrawingState.Drawing)
                    rect = new Rect(GetRectangle(mouseStartPos, mouse));

                DrawingContext.RenderDrawing(rect, needRemember: false);
                MarkerDrawingHelper.DrawMarker(DrawingContext, mouse);
            }
        }

        public void MouseUp(Point mouse)
        {
            if (isActive)
            {
                if (rect != null)
                    DrawingContext.RenderDrawing(rect, needRemember: true);
                rect = null;
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

        private Rectangle GetRectangle(Point start, Point end) => Rectangle.FromLTRB(
            Math.Min(start.X, end.X),
            Math.Min(start.Y, end.Y),
            Math.Max(start.X, end.X),
            Math.Max(start.Y, end.Y));
    }
}
