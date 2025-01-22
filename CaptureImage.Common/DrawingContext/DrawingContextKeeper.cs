

using System;

namespace CaptureImage.Common.DrawingContext
{
    public class DrawingContextKeeper
    {
        public event EventHandler DrawingContextChanged;

        public DrawingContext DrawingContext { get; private set; }

        public DrawingContextKeeper() { }

        public void SetDrawingContext(DrawingContext drawingContext)
        {
            DrawingContext = drawingContext;
            DrawingContextChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
