using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Tools.Misc;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CaptureImage.Common.Tools
{
    public class RectTool : ITool
    {
        private DrawingState state;
        private Point mouseStartPos;
        private Point mousePreviousPos;
        private Pen[] erasePens;
        private bool isActive;

        protected IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContextsKeeper.DrawingContext;

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
                erasePens = DrawingContext.CanvasImages.Select(im =>
                {
                    Pen erasePen = DrawingContext.DrawingPen.Clone() as Pen;
                    erasePen.Brush = new TextureBrush(im);
                    return erasePen;
                }).ToArray();
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
                        var rect = Rectangle.FromLTRB(
                            Math.Min(mouseStartPos.X, mousePreviousPos.X),
                            Math.Min(mouseStartPos.Y, mousePreviousPos.Y),
                            Math.Max(mouseStartPos.X, mousePreviousPos.X),
                            Math.Max(mouseStartPos.Y, mousePreviousPos.Y));

                        Image im = DrawingContext.CanvasImages[i];
                        Control ct = DrawingContext.CanvasControls[i];
                        Graphics.FromImage(im).DrawRectangle(erasePens[i], rect);
                        ct.CreateGraphics().DrawRectangle(erasePens[i], rect);
                    }

                    for (int i = 0; i < DrawingContext.CanvasImages.Length; i++)
                    {
                        var rect = Rectangle.FromLTRB(
                            Math.Min(mouseStartPos.X, mouse.X),
                            Math.Min(mouseStartPos.Y, mouse.Y),
                            Math.Max(mouseStartPos.X, mouse.X),
                            Math.Max(mouseStartPos.Y, mouse.Y));

                        Image im = DrawingContext.CanvasImages[i];
                        Control ct = DrawingContext.CanvasControls[i];
                        Graphics.FromImage(im).DrawRectangle(DrawingContext.DrawingPen, rect);
                        ct.CreateGraphics().DrawRectangle(DrawingContext.DrawingPen, rect);
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
