using CaptureImage.Common.Drawings;
using CaptureImage.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CaptureImage.Common.DrawingContext
{
    public class DrawingContext
    {
        private readonly List<IDrawing> drawings;    

        public static readonly Pen DefaultDrawingPen = new Pen(Color.Yellow, MarkerDrawingHelper.GetPenDiameter());
        public event EventHandler DrawingContextEdited;
        public Image canvasImage;
        public Image cleanImage;
        public ICanvas canvasControl;

        public Pen drawingPen;

        public bool IsClean { get; set; }


        public DrawingContext()
        {
            drawingPen = DefaultDrawingPen;
            drawings = new List<IDrawing>();
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

        public void UndoDrawing()
        {
            if (drawings.Count > 0)
            {
                drawings.RemoveAt(drawings.Count - 1);
                ReRenderDrawings();
            }
        }

        public void Draw(Action<Graphics, Pen> action, DrawingTarget drawingTarget = DrawingTarget.CanvasAndImage)
        {
            drawingPen.Width = MarkerDrawingHelper.GetPenDiameter();

            if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.CanvasOnly)
                canvasControl.OnGraphics((gr, callBack) =>
                {
                    bool forThumb = callBack != null;
                    if (forThumb)
                    {
                        gr.TranslateTransform(-canvasControl.GetThumb.SelectionRectangle.X, -canvasControl.GetThumb.SelectionRectangle.Y);
                        SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen));
                    }
                    else
                    {
                        SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen));
                    }
                });

            if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.ImageOnly)
                using (Graphics gr = Graphics.FromImage(canvasImage)) { SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen)); }

            IsClean = false;
        }

        public void Erase(Action<Graphics, Pen> action, DrawingTarget drawingTarget = DrawingTarget.CanvasAndImage)
        {
            using (TextureBrush texture = new TextureBrush(canvasImage))
            {
                using (Pen erasePen = new Pen(texture, drawingPen.Width))
                {
                    if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.CanvasOnly)
                        canvasControl.OnGraphics((gr, callBack) =>
                        {
                            bool forThumb = callBack != null;
                            if (forThumb)
                            {
                                gr.TranslateTransform(-canvasControl.GetThumb.SelectionRectangle.X, -canvasControl.GetThumb.SelectionRectangle.Y);
                                SafeHelper.OnSafe(() => action?.Invoke(gr, erasePen));
                                gr.TranslateTransform(canvasControl.GetThumb.SelectionRectangle.X, canvasControl.GetThumb.SelectionRectangle.Y);
                                //callBack?.Invoke(gr);
                            }
                            else
                            {
                                SafeHelper.OnSafe(() => action?.Invoke(gr, erasePen));
                            }
                        });

                    if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.ImageOnly)
                        using (Graphics gr = Graphics.FromImage(canvasImage)) { SafeHelper.OnSafe(() => action?.Invoke(gr, erasePen)); }
                }
            }

            GC.Collect();

            IsClean = false;
        }
        

        public void RenderDrawing(IDrawing drawing, bool save)
        {
            if (save)
            {
                drawings.Add(drawing);
                ReRenderDrawings();
            }
            else
            {
                RenderDrawingOnCanvas(drawing);
            }
        }
        
        #region private

        private void RenderDrawingOnCanvas(IDrawing drawing)
        {
            drawingPen.Width = MarkerDrawingHelper.GetPenDiameter();
            Rectangle destRectangleThumb = new Rectangle(0, 0, canvasControl.GetThumb.SelectionRectangle.Width, canvasControl.GetThumb.SelectionRectangle.Height);

            canvasControl.OnGraphics((gr, callBack) =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, callBack != null ? destRectangleThumb : canvasControl.ClientRectangle, bufferedGr =>
                {
                    bool forThumb = callBack != null;

                    if (forThumb)
                    {
                        bufferedGr.DrawImage(canvasImage, destRectangleThumb, canvasControl.GetThumb.SelectionRectangle, GraphicsUnit.Pixel);

                        bufferedGr.TranslateTransform(-canvasControl.GetThumb.SelectionRectangle.X, -canvasControl.GetThumb.SelectionRectangle.Y);

                        SafeHelper.OnSafe(() => drawing?.Paint(bufferedGr, drawingPen));

                        bufferedGr.TranslateTransform(canvasControl.GetThumb.SelectionRectangle.X, canvasControl.GetThumb.SelectionRectangle.Y);

                        callBack?.Invoke(bufferedGr);
                    }
                    else
                    {
                        Rectangle destRectangle = new Rectangle(0, 0, canvasControl.ClientRectangle.Width, canvasControl.ClientRectangle.Height);

                        bufferedGr.DrawImage(canvasImage, destRectangle, destRectangle, GraphicsUnit.Pixel);

                        SafeHelper.OnSafe(() => drawing?.Paint(bufferedGr, drawingPen));
                    }
                });
            });
        }

        private void ReRenderDrawings()
        {
            Rectangle destRectangleThumb = new Rectangle(0, 0, canvasControl.GetThumb.SelectionRectangle.Width, canvasControl.GetThumb.SelectionRectangle.Height);

            canvasControl.OnGraphics((gr, callBack) =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, callBack != null ? destRectangleThumb : canvasControl.ClientRectangle, bufferedGr =>
                {
                    bool forThumb = callBack != null;

                    if (forThumb)
                    {
                        bufferedGr.DrawImage(cleanImage, destRectangleThumb, canvasControl.GetThumb.SelectionRectangle, GraphicsUnit.Pixel);

                        bufferedGr.TranslateTransform(-canvasControl.GetThumb.SelectionRectangle.X, -canvasControl.GetThumb.SelectionRectangle.Y);

                        foreach (IDrawing drawing in drawings)
                            drawing.Repaint(bufferedGr);

                        bufferedGr.TranslateTransform(canvasControl.GetThumb.SelectionRectangle.X, canvasControl.GetThumb.SelectionRectangle.Y);

                        callBack?.Invoke(bufferedGr);
                    }
                    else
                    {
                        Rectangle destRectangle = new Rectangle(0, 0, canvasControl.ClientRectangle.Width, canvasControl.ClientRectangle.Height);

                        bufferedGr.DrawImage(cleanImage, destRectangle, destRectangle, GraphicsUnit.Pixel);

                        foreach (IDrawing drawing in drawings)
                            drawing.Repaint(bufferedGr);
                    }
                });
            });

            RefreshCanvasImage();
        }

        private void RefreshCanvasImage()
        {
            using (Graphics gr = Graphics.FromImage(canvasImage))
            {
                GraphicsHelper.OnBufferedGraphics(gr, canvasControl.ClientRectangle, bufferedGr =>
                {
                    Rectangle destRectangle = new Rectangle(0, 0, canvasControl.ClientRectangle.Width, canvasControl.ClientRectangle.Height);
                    bufferedGr.DrawImage(cleanImage, destRectangle, destRectangle, GraphicsUnit.Pixel);

                    foreach (IDrawing drawing in drawings)
                        drawing.Repaint(bufferedGr);
                });
            }
        }

        #endregion
    }
}
