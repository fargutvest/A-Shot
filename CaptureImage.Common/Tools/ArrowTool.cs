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
        }

        public override void MouseDown(Point mousePosition)
        {
            if (isActive)
            {
                drawingPen = DrawingContext.DrawingPen.Clone() as Pen;
                drawingPen.CustomEndCap = endCap; 

                erasePens = DrawingContext.CanvasImages.Select(im =>
                {
                    Pen erasePen = DrawingContext.DrawingPen.Clone() as Pen;
                    erasePen.Brush = new TextureBrush(im);
                    erasePen.CustomEndCap = endCap;
                    return erasePen;
                }).ToArray();

                mouseStartPos = mousePosition;
                state = DrawingState.Drawing;
            }
        }
    }
}
