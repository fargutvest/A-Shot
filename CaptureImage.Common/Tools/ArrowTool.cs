using CaptureImage.Common.Tools.Misc;
using System.Drawing.Drawing2D;
using System.Drawing;
using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Drawings;

namespace CaptureImage.Common.Tools
{
    public class ArrowTool : LineTool
    {
        private CustomLineCap endCap;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;

        public ArrowTool(IDrawingContextProvider drawingContextProvider) : base (drawingContextProvider)
        {
            endCap = new AdjustableArrowCap(4, 7);
            DrawingContext.UpdateErasingPens();
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
                if (state == DrawingState.Drawing)
                {
                    DrawingContext.Erase((gr, pen) =>
                    {
                        Arrow arrow = new Arrow(mouseStartPos, mousePreviousPos, endCap);
                        arrow.Paint(gr, pen);
                    });

                    DrawingContext.Draw((gr, pen) =>
                    {
                        Arrow arrow = new Arrow(mouseStartPos, mouse, endCap);
                        arrow.Paint(gr, pen);
                    });

                    mousePreviousPos = mouse;
                }
            }
        }
        public override void MouseUp(Point mouse)
        {
            if (isActive)
            {
                Arrow arrow = new Arrow(mouseStartPos, mousePreviousPos, endCap);
                DrawingContext.Drawings.Add(arrow);
                DrawingContext.UpdateErasingPens();
                state = DrawingState.None;
            }
        }

    }
}
