using CaptureImage.Common.Tools.Misc;
using System.Drawing.Drawing2D;
using System.Drawing;
using CaptureImage.Common.DrawingContext;
using static System.Windows.Forms.LinkLabel;

namespace CaptureImage.Common.Tools
{
    public class ArrowTool : LineTool
    {
        private CustomLineCap endCap;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContextKeeper.DrawingContext;

        public ArrowTool(IDrawingContextProvider drawingContextProvider) : base (drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
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
                if (state == DrawingState.Drawing)
                {
                    DrawingContext.Erase((gr, pen) =>
                    {
                        Pen erasePen = pen.Clone() as Pen;
                        erasePen.CustomEndCap = endCap;
                        gr.DrawLine(erasePen, mouseStartPos, mousePreviousPos);
                    });

                    DrawingContext.Draw((gr, pen) =>
                    {
                        Pen drawingPen = pen.Clone() as Pen;
                        drawingPen.CustomEndCap = endCap;
                        gr.DrawLine(drawingPen, mouseStartPos, mouse);
                    });

                    mousePreviousPos = mouse;
                }
            }
        }

    }
}
