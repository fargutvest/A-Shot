using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CaptureImage.Common.DrawingContext
{
    public class DrawingContext : ICloneable
    {
        public static readonly Pen DefaultDrawingPen = new Pen(Color.Yellow, 2);

        public event EventHandler DrawingContextEdited;

        public Image[] canvasImages;

        public Image[] cleanImages;

        private Control[] canvasControls;

        private Dictionary<Control, Image> dict { get; set; }

        public Pen drawingPen;
        
        public Pen[] erasePens;


        public bool IsClean { get; set; }


        public DrawingContext()
        {
            drawingPen = DefaultDrawingPen;
        }

        public static DrawingContext Create(Image[] canvasImages, Control[] canvasControls, bool isClean = false)
        {
            Dictionary<Control, Image> dict = new Dictionary<Control, Image>();

            for(int i = 0; i< canvasImages.Length; i++)
            {
                dict[canvasControls[i]] = canvasImages[i];
            }

            DrawingContext drawingContext = new DrawingContext()
            {
                canvasImages = canvasImages,
                canvasControls = canvasControls,
                IsClean = isClean,
                dict = dict
            };

            drawingContext.cleanImages = canvasImages.Select(img => img.Clone() as Image).ToArray();

            return drawingContext;
        }

        public Image GetImage(Control control)
        {
            return dict[control];
        }
           

        public void Draw(Action<Graphics, Pen> action)
        {
            for (int i = 0; i < canvasImages.Length; i++)
            {
                Image im = canvasImages[i];
                Control ct = canvasControls[i];

                using (Graphics gr = Graphics.FromImage(im))
                {
                    OnSafe(() => action?.Invoke(gr, drawingPen));
                }

                using (Graphics gr = ct.CreateGraphics())
                {
                    OnSafe(() => action?.Invoke(gr, drawingPen));
                }

                IsClean = false;
            }
        }

        public void Erase(Action<Graphics, Pen> action)
        {
            for (int i = 0; i < canvasImages.Length; i++)
            {
                Image im = canvasImages[i];
                Control ct = canvasControls[i];
                Pen erasePen = erasePens[i];

                using (Graphics gr = Graphics.FromImage(im))
                {
                    OnSafe(() => action?.Invoke(gr, erasePen));
                }

                using (Graphics gr = ct.CreateGraphics())
                {
                    OnSafe(() => action?.Invoke(gr, erasePen));
                }

                IsClean = false;
            }
        }

        public void UpdateErasingPens(Action<Graphics> action)
        {
            if (erasePens == null)
                erasePens = new Pen[canvasImages.Length];
            

            for (int i = 0; i < canvasImages.Length; i++)
            {
                Image canvasImage = canvasImages[i];

                using (Graphics gr = Graphics.FromImage(canvasImage))
                {
                    gr.DrawImage(cleanImages[i], new PointF(0, 0));

                    action?.Invoke(gr);
                }

                if (erasePens[i] == null)
                    erasePens[i] = drawingPen.Clone() as Pen;

                erasePens[i]?.Brush.Dispose();
                GC.Collect();
                erasePens[i].Brush = new TextureBrush(canvasImage);

            }
        }


        public void SetColorOfPen(Color color)
        {
            drawingPen.Color = color;
            DrawingContextEdited?.Invoke(this, EventArgs.Empty);
        }

        public object Clone()
        {
            Dictionary<Control, Image> dictClone = new Dictionary<Control, Image>();

            for (int i = 0; i < canvasImages.Length; i++)
            {
                dictClone[canvasControls[i]] = canvasImages[i];
            }

            DrawingContext clone = new DrawingContext()
            {
                drawingPen = drawingPen,
                canvasControls = canvasControls,
                canvasImages = canvasImages.Select(im => im.Clone() as Image).ToArray(),
                dict = dictClone,
                IsClean = IsClean
            };

            return clone;
        }

        private void OnSafe(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
