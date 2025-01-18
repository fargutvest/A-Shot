﻿

using System;

namespace CaptureImage.Common.DrawingContext
{
    public class DrawingContextsKeeper
    {
        public event EventHandler DrawingContextChanged;

        private ChangesHistory changesHistory;
        
        public DrawingContext DrawingContext { get; set; }

        public DrawingContextsKeeper()
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

    }
}
