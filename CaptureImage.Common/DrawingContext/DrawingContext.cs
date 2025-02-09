using CaptureImage.Common.Drawings;
using CaptureImage.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace CaptureImage.Common.DrawingContext
{
    public class DrawingContext
    {
        public delegate void OnGraphicsDelegate(Graphics canvasGraphics, Rectangle clientRectangle);

        private readonly List<IDrawing> drawings;
        private Image image;
        private Image imageCanvas;
        private Image cleanImage;
        private Image cleanImageCanvas;
        private ICanvas canvasControl;
        private readonly Pen drawingPen;

        public static readonly Pen DefaultDrawingPen = new Pen(Color.Yellow, MarkerDrawingHelper.GetPenDiameter());
        
        public bool IsClean { get; set; }

        public event EventHandler Updated;

        public DrawingContext()
        {
            drawingPen = DefaultDrawingPen;
            drawings = new List<IDrawing>();
        }

        public static DrawingContext Create(Image imageToDraw, ICanvas canvasControl, bool isClean = false) => new DrawingContext
        {
            image = imageToDraw,
            imageCanvas = BitmapHelper.DarkenImage(imageToDraw.Clone() as Bitmap, 0.5f),
            canvasControl = canvasControl,
            IsClean = isClean,
            cleanImage = imageToDraw.Clone() as Image,
            cleanImageCanvas = BitmapHelper.DarkenImage(imageToDraw.Clone() as Bitmap, 0.5f)
        };


        public Image GetImage()
        {
            return image;
        }

        public Image GetCanvasImage()
        {
            return imageCanvas;
        }

        public void SetColorOfPen(Color color)
        {
            drawingPen.Color = color;
            Updated?.Invoke(this, EventArgs.Empty);
        }

        public Color GetColorOfPen()
        {
            return drawingPen.Color;
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
                canvasControl.OnGraphics((gr, clientRectangle) =>
                {
                    OnTexturePen(imageCanvas, drawingPen.Width, pen => { SafeHelper.OnSafe(() => action?.Invoke(gr, pen)); });
                });

                canvasControl.GetThumb.OnGraphics((gr, clientRectangle) =>
                {
                    canvasControl.GetThumb.TranslateTransform(gr);
                    OnTexturePen(image, drawingPen.Width, pen => { SafeHelper.OnSafe(() => action?.Invoke(gr, pen)); });
                    //gr.ResetTransform();
                    //canvasControl.GetThumb.DrawBorder(gr);
                });
            }

            if (drawingTarget == DrawingTarget.Image)
            {
                using (Graphics gr = Graphics.FromImage(imageCanvas))
                {
                    OnTexturePen(image, drawingPen.Width, pen => { SafeHelper.OnSafe(() => action?.Invoke(gr, pen)); });
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
                canvasControl.OnGraphics((gr, clientRectangle) => { SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen));  });

                canvasControl.GetThumb.OnGraphics((gr, clientRectangle) =>
                {
                    canvasControl.GetThumb.TranslateTransform(gr);
                    SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen));
                });
            }

            if (drawingTarget == DrawingTarget.Image)
            {
                using (Graphics gr = Graphics.FromImage(imageCanvas)) { SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen)); }
                using (Graphics gr = Graphics.FromImage(image)) { SafeHelper.OnSafe(() => action?.Invoke(gr, drawingPen)); }
            }

            IsClean = false;
        }

        public void RenderDrawing(IDrawing drawing, bool needRemember)
        {
            if (needRemember)
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
            
            canvasControl.OnGraphics((gr, clientRectangle) =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, clientRectangle, bufferedGr =>
                    {
                        canvasControl.DrawBackgroundImage(bufferedGr, imageCanvas);
                        SafeHelper.OnSafe(() => drawing?.Paint(bufferedGr, drawingPen));
                    });
            });

            canvasControl.GetThumb.OnGraphics((gr, clientRectangle) =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, clientRectangle, bufferedGr =>
                {
                    canvasControl.GetThumb.DrawBackgroundImage(bufferedGr, image);

                    canvasControl.GetThumb.TranslateTransform(bufferedGr);

                    SafeHelper.OnSafe(() => drawing?.Paint(bufferedGr, drawingPen));

                    bufferedGr.ResetTransform();

                    canvasControl.GetThumb.DrawBorder(bufferedGr);
                });
            });
        }

        private void ReRenderDrawings()
        {
            canvasControl.OnGraphics((gr, clientRectangle) =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, clientRectangle, bufferedGr =>
                {
                    canvasControl.DrawBackgroundImage(bufferedGr, cleanImageCanvas);

                    foreach (IDrawing drawing in drawings)
                        drawing.Repaint(bufferedGr);

                });
            });

            canvasControl.GetThumb.OnGraphics((gr, clientRectangle) =>
            {
                GraphicsHelper.OnBufferedGraphics(gr, clientRectangle, bufferedGr =>
                {
                    canvasControl.GetThumb.DrawBackgroundImage(bufferedGr, cleanImage);

                    canvasControl.GetThumb.TranslateTransform(bufferedGr);

                    foreach (IDrawing drawing in drawings)
                        drawing.Repaint(bufferedGr);

                    bufferedGr.ResetTransform();

                    canvasControl.GetThumb.DrawBorder(bufferedGr);
                });
            });

            RefreshImages();
        }

        private void RefreshImages()
        {
            Image[] imagesToDraw = { image, imageCanvas };
            Image[] cleanImages = { cleanImage, cleanImageCanvas };

            for (int i = 0; i < imagesToDraw.Length; i++)
            {
                int index = i;
                using (Graphics gr = Graphics.FromImage(imagesToDraw[index]))
                {
                    Rectangle imageBounds = new Rectangle(new Point(0,0), imagesToDraw[index].Size);

                    GraphicsHelper.OnBufferedGraphics(gr, imageBounds, bufferedGr =>
                    {
                        Rectangle destRectangle = new Rectangle(0, 0, imageBounds.Width, imageBounds.Height);
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
