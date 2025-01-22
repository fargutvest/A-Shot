using System;

namespace CaptureImage.Common.DrawingContext
{
    public interface IDrawingContextProvider
    {
        DrawingContext DrawingContext { get; }
        event EventHandler DrawingContextChanged;
    }
}
