using CaptureImage.Common.DrawingContext;
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
        }

        public void MouseDown(Point mousePosition)
        {
            if (isActive)
            {
                mouseStartPos = mousePosition;
                state = DrawingState.Drawing;
            }
        }

        public void MouseMove(Point mouse)
        {
            if (isActive)
            {
                if (state == DrawingState.Drawing)
                {
                    var rect = Rectangle.FromLTRB(
                            Math.Min(mouseStartPos.X, mousePreviousPos.X),
                            Math.Min(mouseStartPos.Y, mousePreviousPos.Y),
                            Math.Max(mouseStartPos.X, mousePreviousPos.X),
                            Math.Max(mouseStartPos.Y, mousePreviousPos.Y));

                    DrawingContext.Erase((gr, pen) =>
                    {
                        gr.DrawRectangle(pen, rect);
                    });

                   rect = Rectangle.FromLTRB(
                            Math.Min(mouseStartPos.X, mouse.X),
                            Math.Min(mouseStartPos.Y, mouse.Y),
                            Math.Max(mouseStartPos.X, mouse.X),
                            Math.Max(mouseStartPos.Y, mouse.Y));

                    DrawingContext.Draw((gr, pen) =>
                    {
                        gr.DrawRectangle(pen, rect);
                    });

                    mousePreviousPos = mouse;
                }
            }
        }

        public void MouseUp(Point mouse)
        {
            if (isActive)
            {
                drawingContextProvider.DrawingContextKeeper.SaveContext();
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
