using CaptureImage.Common.Drawings;
using CaptureImage.Common.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace CaptureImage.Common.DrawingContext
{
    public class DrawingContext
    {
        private readonly List<IDrawing> drawings;    

        public static readonly Pen DefaultDrawingPen = new Pen(Color.Yellow, MarkerDrawingHelper.GetPenDiameter());
        public event EventHandler DrawingContextEdited;
        private Image imageToDraw;
        public Image imageToDrawCanvas;
        public Image cleanImage;
        public Image cleanImageCanvas;
        public ICanvas canvasControl;

        public Pen drawingPen;

        public bool IsClean { get; set; }

        public event EventHandler Updated;

        public DrawingContext()
        {
            drawingPen = DefaultDrawingPen;
            drawings = new List<IDrawing>();
        }

        public static DrawingContext Create(Image imageToDraw, ICanvas canvasControl, bool isClean = false) => new DrawingContext
        {
            imageToDraw = imageToDraw,
            imageToDrawCanvas = BitmapHelper.DarkenImage(imageToDraw.Clone() as Bitmap, 0.5f),
            canvasControl = canvasControl,
            IsClean = isClean,
            cleanImage = imageToDraw.Clone() as Image,
            cleanImageCanvas = BitmapHelper.DarkenImage(imageToDraw.Clone() as Bitmap, 0.5f)
        };


        public Image GetImage()
        {
            return imageToDraw;
        }

        public Image GetCanvasImage()
        {
            return imageToDrawCanvas;
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

        public void Erase(Action<Graphics, Pen> action, DrawingTarget drawingTarget = DrawingTarget.CanvasAndImage)
        {
            using (TextureBrush texture = new TextureBrush(imageToDrawCanvas))
            {
                using (Pen erasePen = new Pen(texture, drawingPen.Width))
                {
                    using (TextureBrush textureCanvas = new TextureBrush(imageToDrawCanvas))
                    {
                        using (Pen erasePenCanvas = new Pen(textureCanvas, drawingPen.Width))
                        {
                            if (drawingTarget == DrawingTarget.CanvasAndImage ||
                                drawingTarget == DrawingTarget.CanvasOnly)
                                canvasControl.OnGraphics((gr, callBack) =>
                                {
                                    bool forThumb = callBack != null;
                                    if (forThumb)
                                    {
                                        gr.TranslateTransform(-canvasControl.GetThumb.SelectionRectangle.X,
                                            -canvasControl.GetThumb.SelectionRectangle.Y);
                                        SafeHelper.OnSafe(() => action?.Invoke(gr, erasePen));
                                        gr.TranslateTransform(canvasControl.GetThumb.SelectionRectangle.X,
                                            canvasControl.GetThumb.SelectionRectangle.Y);
                                        //callBack?.Invoke(gr);
                                    }
                                    else
                                    {
                                        SafeHelper.OnSafe(() => action?.Invoke(gr, erasePenCanvas));
                                    }
                                });

                            if (drawingTarget == DrawingTarget.CanvasAndImage ||
                                drawingTarget == DrawingTarget.ImageOnly)
                                using (Graphics gr = Graphics.FromImage(imageToDrawCanvas))
                                {
                                    SafeHelper.OnSafe(() => action?.Invoke(gr, erasePen));
                                }
                        }
                    }
                }
            }

            GC.Collect();

            IsClean = false;
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
                using (Graphics gr = Graphics.FromImage(imageToDrawCanvas)) { SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen)); }

            if (drawingTarget == DrawingTarget.CanvasAndImage || drawingTarget == DrawingTarget.ImageOnly)
                using (Graphics gr = Graphics.FromImage(imageToDraw)) { SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen)); }

            IsClean = false;
        }
        
        public void RenderDrawing(IDrawing drawing, bool save)
        {
            if (save)
            {
                drawings.Add(drawing);
                ReRenderDrawings();
                Updated?.Invoke(this, EventArgs.Empty);
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
                        bufferedGr.DrawImage(imageToDraw, destRectangleThumb, canvasControl.GetThumb.SelectionRectangle, GraphicsUnit.Pixel);

                        bufferedGr.TranslateTransform(-canvasControl.GetThumb.SelectionRectangle.X, -canvasControl.GetThumb.SelectionRectangle.Y);

                        SafeHelper.OnSafe(() => drawing?.Paint(bufferedGr, drawingPen));

                        bufferedGr.TranslateTransform(canvasControl.GetThumb.SelectionRectangle.X, canvasControl.GetThumb.SelectionRectangle.Y);

                        callBack?.Invoke(bufferedGr);
                    }
                    else
                    {
                        Rectangle destRectangle = new Rectangle(0, 0, canvasControl.ClientRectangle.Width, canvasControl.ClientRectangle.Height);

                        bufferedGr.DrawImage(imageToDrawCanvas, destRectangle, destRectangle, GraphicsUnit.Pixel);

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

                        bufferedGr.DrawImage(cleanImageCanvas, destRectangle, destRectangle, GraphicsUnit.Pixel);

                        foreach (IDrawing drawing in drawings)
                            drawing.Repaint(bufferedGr);
                    }
                });
            });

            RefreshImages();
        }

        private void RefreshImages()
        {
            Image[] imagesToDraw = { imageToDraw, imageToDrawCanvas };
            Image[] cleanImages = { cleanImage, cleanImageCanvas };

            for (int i = 0; i < imagesToDraw.Length; i++)
            {
                int index = i;
                using (Graphics gr = Graphics.FromImage(imagesToDraw[index]))
                {
                    GraphicsHelper.OnBufferedGraphics(gr, canvasControl.ClientRectangle, bufferedGr =>
                    {
                        Rectangle destRectangle = new Rectangle(0, 0, canvasControl.ClientRectangle.Width, canvasControl.ClientRectangle.Height);
                        bufferedGr.DrawImage(cleanImages[index], destRectangle, destRectangle, GraphicsUnit.Pixel);

                        foreach (IDrawing drawing in drawings)
                            drawing.Repaint(bufferedGr);
                    });
                }
            }
        }

        #endregion
    }
}
