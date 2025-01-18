using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Tools.Misc;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CaptureImage.Common.Tools
{
    public class LineTool : ITool
    {
        protected DrawingState state;
        protected Point mouseStartPos;
        private Point mousePreviousPos;
        protected Pen[] erasePens;
        protected bool isActive;

        protected IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContextsKeeper.DrawingContext;
        

        public LineTool(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
            this.state = DrawingState.None;

            mouseStartPos = new Point(0, 0);
        }

        public virtual void MouseDown(Point mousePosition)
        {
            if (isActive)
            {
                erasePens = DrawingContext.CanvasImages.Select(im => new Pen(new TextureBrush(im)) { Width = 2 }).ToArray();
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
                    for (int i = 0; i < DrawingContext.CanvasImages.Length; i++)
                    {
                        Image im = DrawingContext.CanvasImages[i];
                        Control ct = DrawingContext.CanvasControls[i];
                        Graphics.FromImage(im).DrawLine(erasePens[i], mouseStartPos, mousePreviousPos);
                        ct.CreateGraphics().DrawLine(erasePens[i], mouseStartPos, mousePreviousPos);
                    }

                    for (int i = 0; i < DrawingContext.CanvasImages.Length; i++)
                    {
                        Image im = DrawingContext.CanvasImages[i];
                        Control ct = DrawingContext.CanvasControls[i];

                        Graphics.FromImage(im).DrawLine(DrawingContext.DrawingPen, mouseStartPos, mouse);
                        ct.CreateGraphics().DrawLine(DrawingContext.DrawingPen, mouseStartPos, mouse);
                        DrawingContext.IsClean = false;
                    }

                    mousePreviousPos = mouse;
                }
            }
        }

        public void MouseUp()
        {
            if (isActive)
            {
                drawingContextProvider.DrawingContextsKeeper.SaveContext();
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
