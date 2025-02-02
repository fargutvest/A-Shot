using CaptureImage.Common.Drawings;
using CaptureImage.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static DrawingContext Create(Image canvasImage, ICanvas canvasControl, bool isClean = false)
        {
            DrawingContext drawingContext = new DrawingContext()
            {
                canvasImage = canvasImage,
                canvasControl = canvasControl,             
                IsClean = isClean,
            };

            drawingContext.cleanImage = canvasImage.Clone() as Image;

            return drawingContext;
        }

        public Image GetImage()
        {
            return canvasImage;
        }

        public void SetColorOfPen(Color color)
        {
            drawingPen.Color = color;
            DrawingContextEdited?.Invoke(this, EventArgs.Empty);
        }

        private void OnSafe(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }



        public void Draw(Action<Graphics, Pen> action, DrawingTarget drawingTarget = DrawingTarget.CanvasAndImage)
        {
            drawingPen.Width = MarkerDrawingHelper.GetPenDiameter();

            if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.CanvasOnly)
                canvasControl.OnGraphics(gr => OnSafe(() => action?.Invoke(gr, drawingPen)));

            if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.ImageOnly)
                using (Graphics gr = Graphics.FromImage(canvasImage)) { OnSafe(() => action?.Invoke(gr, drawingPen)); }

            IsClean = false;
        }

        public void Erase(IDrawing drawing, DrawingTarget drawingTarget = DrawingTarget.CanvasAndImage)
        {
            using (TextureBrush texture = new TextureBrush(canvasImage))
            {
                using (Pen erasePen = new Pen(texture, drawingPen.Width))
                {
                    if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.CanvasOnly)
                        canvasControl.OnGraphics(gr => OnSafe(() => drawing.Erase(gr, erasePen)));

                    if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.ImageOnly)
                        using (Graphics gr = Graphics.FromImage(canvasImage)) { OnSafe(() => drawing.Erase(gr, erasePen)); }
                }
            }

            GC.Collect();

            IsClean = false;
        }

        public void ReRenderDrawing(IDrawing drawing, DrawingTarget drawingTarget = DrawingTarget.CanvasAndImage)
        {
            void ReRender(Graphics gr)
            {
                gr.DrawImage(canvasImage, new Point(0, 0));
                drawingPen.Width = MarkerDrawingHelper.GetPenDiameter();
                OnSafe(() => drawing.Paint(gr, drawingPen));
            }

            if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.CanvasOnly)
            {
                canvasControl.OnGraphics(gr =>
                {
                    GraphicsHelper.OnBufferedGraphics(gr, canvasControl.ClientRectangle, ReRender);
                });
            }

            if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.ImageOnly)
            {
                using (Graphics gr = Graphics.FromImage(canvasImage))
                {
                    GraphicsHelper.OnBufferedGraphics(gr, canvasControl.ClientRectangle, ReRender);
                }
            }
        }

        public void ReRenderDrawings(DrawingTarget drawingTarget = DrawingTarget.CanvasAndImage)
        {
            void ReRender(Graphics gr)
            {
                gr.DrawImage(cleanImage, new PointF(0, 0));

                foreach (IDrawing drawing in Drawings)
                    drawing.Repaint(gr);
            }

            if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.CanvasOnly)
            {
                canvasControl.OnGraphics(gr =>
                {
                    GraphicsHelper.OnBufferedGraphics(gr, canvasControl.ClientRectangle, ReRender);
                });
            }

            if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.ImageOnly)
            {
                using (Graphics gr = Graphics.FromImage(canvasImage))
                {
                    GraphicsHelper.OnBufferedGraphics(gr, canvasControl.ClientRectangle, ReRender);
                }
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
