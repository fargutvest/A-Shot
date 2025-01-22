using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Drawings;
using CaptureImage.Common.Tools.Misc;
using System;
using System.Drawing;

namespace CaptureImage.Common.Tools
{
    public class RectTool : ITool
    {
        private DrawingState state;
        private Point mouseStartPos;
        private Point mousePreviousPos;
        private bool isActive;

        protected IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContextKeeper.DrawingContext;

        public RectTool(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
            this.state = DrawingState.None;
            mouseStartPos = new Point(0, 0);
            DrawingContext.UpdateErasingPens();
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
                {
                    DrawingContext.Erase((gr, pen) =>
                    {
                        Rect rect = new Rect(GetRectangle(mouseStartPos, mousePreviousPos));
                        rect.Paint(gr, pen);
                    });

                    DrawingContext.Draw((gr, pen) =>
                    {
                        Rect rect = new Rect(GetRectangle(mouseStartPos, mouse));
                        rect.Paint(gr, pen);
                    });

                    mousePreviousPos = mouse;
                }
            }
        }

        public void MouseUp(Point mouse)
        {
            if (isActive)
            {
                Rect rect = new Rect(GetRectangle(mouseStartPos, mouse));
                DrawingContext.Drawings.Add(rect);
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

        private Rectangle GetRectangle(Point start, Point end) => Rectangle.FromLTRB(
            Math.Min(start.X, end.X),
            Math.Min(start.Y, end.Y),
            Math.Max(start.X, end.X),
            Math.Max(start.Y, end.Y));
    }
}
