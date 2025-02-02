using CaptureImage.Common.Drawings;
using CaptureImage.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CaptureImage.Common.DrawingContext
{
    public class DrawingContext
    {
        public static readonly Pen DefaultDrawingPen = new Pen(Color.Yellow, MarkerDrawingHelper.GetPenDiameter());
        public event EventHandler DrawingContextEdited;
        public Image canvasImage;
        public Image cleanImage;
        public ICanvas canvasControl;

        public Pen drawingPen;

        public bool IsClean { get; set; }
        public List<IDrawing> Drawings { get; set; }    


        public DrawingContext()
        {
            drawingPen = DefaultDrawingPen;
            Drawings = new List<IDrawing>();
        }

        public static DrawingContext Create(Image canvasImage, ICanvas canvasControl, bool isClean = false) => new DrawingContext
        {
            canvasImage = canvasImage,
            canvasControl = canvasControl,
            IsClean = isClean,
            cleanImage = canvasImage.Clone() as Image
        };


        public Image GetImage()
        {
            return canvasImage;
        }

        public void SetColorOfPen(Color color)
        {
            drawingPen.Color = color;
            DrawingContextEdited?.Invoke(this, EventArgs.Empty);
        }

        public void Draw(Action<Graphics, Pen> action, DrawingTarget drawingTarget = DrawingTarget.CanvasAndImage)
        {
            drawingPen.Width = MarkerDrawingHelper.GetPenDiameter();

            if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.CanvasOnly)
                canvasControl.OnGraphics(gr => SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen)));

            if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.ImageOnly)
                using (Graphics gr = Graphics.FromImage(canvasImage)) { SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen)); }

            IsClean = false;
        }

        public void Erase(IDrawing drawing, DrawingTarget drawingTarget = DrawingTarget.CanvasAndImage)
        {
            using (TextureBrush texture = new TextureBrush(canvasImage))
            {
                using (Pen erasePen = new Pen(texture, drawingPen.Width))
                {
                    if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.CanvasOnly)
                        canvasControl.OnGraphics(gr => SafeHelper.OnSafe(() => drawing.Erase(gr, erasePen)));

                    if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.ImageOnly)
                        using (Graphics gr = Graphics.FromImage(canvasImage)) { SafeHelper.OnSafe(() => drawing.Erase(gr, erasePen)); }
                }
            }

            GC.Collect();

            IsClean = false;
        }

        public void ReRenderDrawingOnCanvas(IDrawing drawing)
        {
            canvasControl.OnGraphics(gr =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, canvasControl.ClientRectangle, bufferedGr =>
                {
                    bufferedGr.DrawImage(canvasImage, new Point(0, 0));
                    drawingPen.Width = MarkerDrawingHelper.GetPenDiameter();
                    SafeHelper.OnSafe(() => drawing.Paint(bufferedGr, drawingPen));
                });
            });

        }

        public void ReRenderDrawings()
        {
            void ReRender(Graphics gr)
            {
                gr.DrawImage(cleanImage, new PointF(0, 0));

                foreach (IDrawing drawing in Drawings)
                    drawing.Repaint(gr);
            }

            canvasControl.OnGraphics(gr =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, canvasControl.ClientRectangle, ReRender);
            });

            using (Graphics gr = Graphics.FromImage(canvasImage))
            {
                GraphicsHelper.OnBufferedGraphics(gr, canvasControl.ClientRectangle, ReRender);
            }
        }

        public void UndoDrawing()
        {
            if (Drawings.Count > 0)
            {
                Drawings.RemoveAt(Drawings.Count - 1);
                ReRenderDrawings();
            }
        }
    }
}
