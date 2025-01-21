
using System;

namespace CaptureImage.Common.DrawingContext
{
    public interface IDrawingContextProvider
    {
        DrawingContextKeeper DrawingContextKeeper { get; }
    }
}
