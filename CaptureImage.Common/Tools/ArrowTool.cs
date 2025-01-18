using CaptureImage.Common.Tools.Misc;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Drawing;
using CaptureImage.Common.DrawingContext;

namespace CaptureImage.Common.Tools
{
    public class ArrowTool : LineTool
    {
        private CustomLineCap endCap;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContextsKeeper.DrawingContext;

        public ArrowTool(IDrawingContextProvider drawingContextProvider) : base (drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
            endCap = new AdjustableArrowCap(4, 7);
            //DrawingContext.DrawingPen.CustomEndCap = endCap;
        }

        public override void MouseDown(Point mousePosition)
        {
            if (isActive)
            {
                erasePens = DrawingContext.CanvasImages.Select(im => new Pen(new TextureBrush(im))
                {
                    Width = 2,
                    //CustomEndCap = endCap
                }).ToArray();

                mouseStartPos = mousePosition;
                state = DrawingState.Drawing;
            }
        }
    }
}
