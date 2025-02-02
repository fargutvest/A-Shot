using CaptureImage.Common.Tools.Misc;
using System.Drawing.Drawing2D;
using System.Drawing;
using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Drawings;
using CaptureImage.Common.Helpers;

namespace CaptureImage.Common.Tools
{
    public class ArrowTool : LineTool
    {
        private Arrow arrow;
        private CustomLineCap endCap;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;

        public ArrowTool(IDrawingContextProvider drawingContextProvider) : base (drawingContextProvider)
        {
            endCap = new AdjustableArrowCap(4, 7);
        }

        public override void MouseDown(Point mousePosition)
        {
            if (isActive)
            {
                mouseStartPos = mousePosition;
                state = DrawingState.Drawing;
            }
        }

        public override void MouseMove(Point mouse)
        {
            if (isActive)
            {
                MarkerDrawingHelper.EraseMarker(DrawingContext);

                if (state == DrawingState.Drawing)
                {
                    arrow = new Arrow(mouseStartPos, mouse, endCap);
                    DrawingContext.ReRenderDrawing(arrow, DrawingTarget.CanvasOnly);
                }

                MarkerDrawingHelper.DrawMarker(DrawingContext, mouse);
                mousePreviousPos = mouse;
            }
        }

        public override void MouseUp(Point mouse)
        {
            if (isActive)
            {
                if (arrow != null)
                    DrawingContext.Drawings.Add(arrow);

                DrawingContext.ReRenderDrawings();
                state = DrawingState.None;
            }
        }

    }
}
