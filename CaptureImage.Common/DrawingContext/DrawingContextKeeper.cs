

using System;

namespace CaptureImage.Common.DrawingContext
{
    public class DrawingContextKeeper
    {
        public event EventHandler DrawingContextChanged;

        private ChangesHistory changesHistory;
        
        public DrawingContext DrawingContext { get; private set; }

        public DrawingContextKeeper()
        {
            changesHistory = new ChangesHistory();
        }

        public void SaveContext()
        {
            changesHistory.SaveChange(DrawingContext);
        }

        public void RevertToPreviousContext()
        {
            DrawingContext = changesHistory.GetPrevious();
            DrawingContextChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetDrawingContext(DrawingContext drawingContext)
        {
            DrawingContext = drawingContext;
            DrawingContextChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
