using CaptureImage.Common.Drawings;
using CaptureImage.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CaptureImage.Common.DrawingContext
{
    public class DrawingContext
    {
        public delegate void OnGraphicsDelegate(Graphics canvasGraphics);

        private readonly List<IDrawing> drawings;
        private Image imageToDraw;
        private Image imageToDrawCanvas;
        private Image cleanImage;
        private Image cleanImageCanvas;
        private ICanvas canvasControl;

        public static readonly Pen DefaultDrawingPen = new Pen(Color.Yellow, MarkerDrawingHelper.GetPenDiameter());

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
            Updated?.Invoke(this, EventArgs.Empty);
        }

        public void UndoDrawing()
        {
            if (drawings.Count > 0)
            {
                drawings.RemoveAt(drawings.Count - 1);
                ReRenderDrawings();
            }
        }

        public void Erase(Action<Graphics, Pen> action, DrawingTarget drawingTarget)
        {
            if (drawingTarget == DrawingTarget.Canvas)
            {
                canvasControl.OnGraphics(gr =>
                {
                    OnTexturePen(imageToDrawCanvas, drawingPen.Width, pen =>
                    {
                        SafeHelper.OnSafe(() => action?.Invoke(gr, pen));
                    });
                });

                canvasControl.GetThumb.OnGraphics(gr =>
                {
                    gr.TranslateTransform(-canvasControl.GetThumb.SelectionRectangle.X,
                        -canvasControl.GetThumb.SelectionRectangle.Y);

                    OnTexturePen(imageToDraw, drawingPen.Width, pen =>
                    {
                        SafeHelper.OnSafe(() => action?.Invoke(gr, pen));
                    });


                    //gr.TranslateTransform(canvasControl.GetThumb.SelectionRectangle.X,
                    //    canvasControl.GetThumb.SelectionRectangle.Y);

                    //canvasControl.GetThumb.DrawBorder(gr);
                });
            }

            if (drawingTarget == DrawingTarget.Image)
            {
                using (Graphics gr = Graphics.FromImage(imageToDrawCanvas))
                {
                    OnTexturePen(imageToDraw, drawingPen.Width, pen =>
                    {
                        SafeHelper.OnSafe(() => action?.Invoke(gr, pen));
                    });
                }
            }

            GC.Collect();

            IsClean = false;
        }

        public void Draw(Action<Graphics, Pen> action, DrawingTarget drawingTarget)
        {
            drawingPen.Width = MarkerDrawingHelper.GetPenDiameter();

            if (drawingTarget == DrawingTarget.Canvas)
            {
                canvasControl.OnGraphics(gr =>
                {
                    SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen));
                });

                canvasControl.GetThumb.OnGraphics(gr =>
                {
                    gr.TranslateTransform(-canvasControl.GetThumb.SelectionRectangle.X, -canvasControl.GetThumb.SelectionRectangle.Y);
                    SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen));
                });
            }

            if (drawingTarget == DrawingTarget.Image)
                using (Graphics gr = Graphics.FromImage(imageToDrawCanvas)) { SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen)); }

            if (drawingTarget == DrawingTarget.Image)
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

            Rectangle destRectangleThumb = new Rectangle(0, 0, canvasControl.GetThumb.SelectionRectangle.Width,
                canvasControl.GetThumb.SelectionRectangle.Height);

            Rectangle destRectangle = new Rectangle(0, 0, canvasControl.ClientRectangle.Width, canvasControl.ClientRectangle.Height);

            canvasControl.OnGraphics(gr =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, canvasControl.ClientRectangle, bufferedGr =>
                    {
                        bufferedGr.DrawImage(imageToDrawCanvas, destRectangle, destRectangle, GraphicsUnit.Pixel);
                        SafeHelper.OnSafe(() => drawing?.Paint(bufferedGr, drawingPen));
                    });
            });

            canvasControl.GetThumb.OnGraphics(gr =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, destRectangleThumb, bufferedGr =>
                {
                    bufferedGr.DrawImage(imageToDraw, destRectangleThumb, canvasControl.GetThumb.SelectionRectangle,
                        GraphicsUnit.Pixel);

                    bufferedGr.TranslateTransform(-canvasControl.GetThumb.SelectionRectangle.X,
                        -canvasControl.GetThumb.SelectionRectangle.Y);

                    SafeHelper.OnSafe(() => drawing?.Paint(bufferedGr, drawingPen));

                    bufferedGr.TranslateTransform(canvasControl.GetThumb.SelectionRectangle.X,
                        canvasControl.GetThumb.SelectionRectangle.Y);

                    canvasControl.GetThumb.DrawBorder(bufferedGr);
                });
            });
        }

        private void ReRenderDrawings()
        {
            Rectangle destRectangleThumb = new Rectangle(0, 0, canvasControl.GetThumb.SelectionRectangle.Width, canvasControl.GetThumb.SelectionRectangle.Height);

            canvasControl.OnGraphics(gr =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, canvasControl.ClientRectangle, bufferedGr =>
                {
                    Rectangle destRectangle = new Rectangle(0, 0, canvasControl.ClientRectangle.Width, canvasControl.ClientRectangle.Height);

                    bufferedGr.DrawImage(cleanImageCanvas, destRectangle, destRectangle, GraphicsUnit.Pixel);

                    foreach (IDrawing drawing in drawings)
                        drawing.Repaint(bufferedGr);

                });
            });

            canvasControl.GetThumb.OnGraphics(gr =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, destRectangleThumb, bufferedGr =>
                {
                    bufferedGr.DrawImage(cleanImage, destRectangleThumb, canvasControl.GetThumb.SelectionRectangle, GraphicsUnit.Pixel);

                    bufferedGr.TranslateTransform(-canvasControl.GetThumb.SelectionRectangle.X, -canvasControl.GetThumb.SelectionRectangle.Y);

                    foreach (IDrawing drawing in drawings)
                        drawing.Repaint(bufferedGr);

                    bufferedGr.TranslateTransform(canvasControl.GetThumb.SelectionRectangle.X, canvasControl.GetThumb.SelectionRectangle.Y);

                    canvasControl.GetThumb.DrawBorder(bufferedGr);
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

        private void OnTexturePen(Image textureImage, float penWidth, Action<Pen> toDo)
        {
            using (TextureBrush texture = new TextureBrush(textureImage))
            {
                using (Pen texturePen = new Pen(texture, penWidth))
                {
                    toDo?.Invoke(texturePen);
                }
            }
        }

        #endregion
    }
}
