using CaptureImage.Common.Drawings;
using CaptureImage.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CaptureImage.Common.DrawingContext
{
    public class DrawingContext
    {
        public static readonly Pen DefaultDrawingPen = new Pen(Color.Yellow, MarkerDrawingHelper.GetPenDiameter());
        public event EventHandler DrawingContextEdited;
        public Image[] canvasImages;
        public Image[] eraseImages;
        public Image[] cleanImages;
        public Control[] canvasControls;
        private Dictionary<Control, Image> dict { get; set; }
        public Pen drawingPen;

        public bool IsClean { get; set; }
        public List<IDrawing> Drawings { get; set; }    


        public DrawingContext()
        {
            drawingPen = DefaultDrawingPen;
            Drawings = new List<IDrawing>();
        }

        public static DrawingContext Create(Image[] canvasImages, Control[] canvasControls, bool isClean = false)
        {
            Dictionary<Control, Image> dict = new Dictionary<Control, Image>();

            for (int i = 0; i< canvasImages.Length; i++)
                dict[canvasControls[i]] = canvasImages[i];
            
            DrawingContext drawingContext = new DrawingContext()
            {
                canvasImages = canvasImages,
                canvasControls = canvasControls,             
                IsClean = isClean,
                dict = dict
            };

            drawingContext.cleanImages = canvasImages.Select(img => img.Clone() as Image).ToArray();
            drawingContext.eraseImages = canvasImages.Select(img => img.Clone() as Image).ToArray();

            return drawingContext;
        }

        public Image GetImage(Control control)
        {
            return dict[control];
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



        public void Draw(Action<Graphics, Pen> action, bool onlyOnCanvas = false)
        {
            drawingPen.Width = MarkerDrawingHelper.GetPenDiameter();
            for (int i = 0; i < canvasImages.Length; i++)
            {
                Image im = canvasImages[i];
                Control ct = canvasControls[i];

                if (onlyOnCanvas == false)
                    using (Graphics gr = Graphics.FromImage(im)) { OnSafe(() => action?.Invoke(gr, drawingPen)); }

                using (Graphics gr = ct.CreateGraphics()) { OnSafe(() => action?.Invoke(gr, drawingPen)); }

                IsClean = false;
            }
        }

        public void Erase(IDrawing drawing, bool onlyOnCanvas = false)
        {
            for (int i = 0; i < canvasImages.Length; i++)
            {
                Image im = canvasImages[i];
                Control ct = canvasControls[i];

                using (TextureBrush texture = new TextureBrush(im))
                {
                    using (Pen erasePen = new Pen(texture, drawingPen.Width))
                    {
                        if (onlyOnCanvas == false)
                            using (Graphics gr = Graphics.FromImage(im)) { OnSafe(() => drawing.Erase(gr, erasePen)); }

                        using (Graphics gr = ct.CreateGraphics()) { OnSafe(() => drawing.Erase(gr, erasePen)); }
                    }
                }

                ct.Invalidate();
                GC.Collect();

                IsClean = false;
            }
        }

        public void ReRenderDrawings(bool canvasImagesOnly = false)
        {
            ReRenderDrawings(gr =>
            {
                for (int i = 0; i < Drawings.Count; i++)
                {
                    IDrawing drawing = Drawings[i];
                    drawing.Repaint(gr);
                }
            }, canvasImagesOnly: canvasImagesOnly);
        }

        public void ReRenderDrawings(Action<Graphics> action, bool needClean = true, bool canvasImagesOnly = false)
        {
            for (int i = 0; i < canvasImages.Length; i++)
            {
                Image im = canvasImages[i];
                Control ct = canvasControls[i];

                void ReRender(Graphics gr)
                {
                    if (needClean)
                        gr.DrawImage(cleanImages[i], new PointF(0, 0));

                    action?.Invoke(gr);
                }

                using (Graphics gr = Graphics.FromImage(im)) { ReRender(gr); }

                if (canvasImagesOnly == false)
                    using (Graphics gr = ct.CreateGraphics()) { ReRender(gr); }
            }
        }

        public void ReRenderDrawings()
        {
            for (int i = 0; i < canvasImages.Length; i++)
            {
                Image im = canvasImages[i];
                Control ct = canvasControls[i];
                using (Graphics gr = ct.CreateGraphics()) { gr.DrawImage(canvasImages[i], new PointF(0, 0)); }
            }
        }

        public void UpdateEraseImages()
        {
            for (int i = 0; i < canvasImages.Length; i++)
            {
                eraseImages[i].Dispose();
                eraseImages[i] = canvasImages[i].Clone() as Image;
            }
        }

        public void UndoDrawing()
        {
            if (Drawings.Count > 0)
            {
                IDrawing lastDrawing = Drawings[Drawings.Count - 1];
                Drawings.RemoveAt(Drawings.Count - 1);
                ReRenderDrawings();
            }
        }
    }
}
