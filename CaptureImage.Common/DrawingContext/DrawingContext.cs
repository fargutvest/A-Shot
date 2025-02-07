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
                canvasControl.OnGraphics((gr, callBack) =>
                {
                    SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen));
                    //callBack?.Invoke(gr);
                });

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
                        canvasControl.OnGraphics((gr, callBack) =>
                        {
                            SafeHelper.OnSafe(() => drawing.Erase(gr, erasePen));
                            //callBack?.Invoke(gr);
                        });

                    if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.ImageOnly)
                        using (Graphics gr = Graphics.FromImage(canvasImage)) { SafeHelper.OnSafe(() => drawing.Erase(gr, erasePen)); }
                }
            }

            GC.Collect();

            IsClean = false;
        }

        public void ReRenderDrawingOnCanvas(IDrawing drawing)
        {
            canvasControl.OnGraphics((gr, callBack) =>
            {
                Rectangle destRectangleThumb = new Rectangle(0, 0, canvasControl.GetThumb.SelectionRectangle.Width, canvasControl.GetThumb.SelectionRectangle.Height);

                GraphicsHelper.OnBufferedGraphics(gr, callBack!= null? destRectangleThumb : canvasControl.ClientRectangle, bufferedGr =>
                {
                    bool forThumb = callBack != null;

                    drawingPen.Width = MarkerDrawingHelper.GetPenDiameter();

                    if (forThumb)
                    {
                        bufferedGr.DrawImage(canvasImage, destRectangleThumb, canvasControl.GetThumb.SelectionRectangle, GraphicsUnit.Pixel);
                        callBack?.Invoke(bufferedGr);
                        bufferedGr.TranslateTransform(-canvasControl.GetThumb.SelectionRectangle.X, - canvasControl.GetThumb.SelectionRectangle.Y);
                        SafeHelper.OnSafe(() => drawing.Paint(bufferedGr, drawingPen));
                    }
                    else
                    {
                        Rectangle destRectangle = new Rectangle(0, 0, canvasControl.ClientRectangle.Width, canvasControl.ClientRectangle.Height);
                        bufferedGr.DrawImage(canvasImage, destRectangle, destRectangle, GraphicsUnit.Pixel);
                        SafeHelper.OnSafe(() => drawing.Paint(bufferedGr, drawingPen));
                    }
                });
            });

        }

        public void ReRenderDrawings()
        {
            Rectangle destRectangleThumb = new Rectangle(0, 0, canvasControl.GetThumb.SelectionRectangle.Width, canvasControl.GetThumb.SelectionRectangle.Height);

            void ReRender(Graphics gr, Action<Graphics> callBack)
            {
                bool forThumb = callBack != null;

                if (forThumb)
                {
                    gr.DrawImage(cleanImage, destRectangleThumb, canvasControl.GetThumb.SelectionRectangle, GraphicsUnit.Pixel);
                    callBack?.Invoke(gr);
                    gr.TranslateTransform(-canvasControl.GetThumb.SelectionRectangle.X, -canvasControl.GetThumb.SelectionRectangle.Y);
                    foreach (IDrawing drawing in Drawings)
                        drawing.Repaint(gr);
                }
                else
                {
                    Rectangle destRectangle = new Rectangle(0, 0, canvasControl.ClientRectangle.Width, canvasControl.ClientRectangle.Height);
                    gr.DrawImage(cleanImage, destRectangle, destRectangle, GraphicsUnit.Pixel);

                    foreach (IDrawing drawing in Drawings)
                        drawing.Repaint(gr);
                }
            }

            canvasControl.OnGraphics((gr, callBack) =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, callBack!= null ? destRectangleThumb : canvasControl.ClientRectangle, bufferedGr =>
                {
                    ReRender(bufferedGr, callBack);
                });
            });

            using (Graphics gr = Graphics.FromImage(canvasImage))
            {
                GraphicsHelper.OnBufferedGraphics(gr, canvasControl.ClientRectangle, bufferedGr=> ReRender(bufferedGr, callBack: null));
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
