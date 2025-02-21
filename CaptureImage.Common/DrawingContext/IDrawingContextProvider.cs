using CaptureImage.Common.Helpers.HotKeys;
using System;

namespace CaptureImage.Common.DrawingContext
{
    public interface IDrawingContextProvider
    {
        DrawingContext DrawingContext { get; }
    }
}
