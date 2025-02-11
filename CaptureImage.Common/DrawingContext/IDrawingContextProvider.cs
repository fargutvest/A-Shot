using CaptureImage.Common.Helpers.HotKeys;
using System;

namespace CaptureImage.Common.DrawingContext
{
    public interface IDrawingContextProvider
    {
        MouseHookHelper mouseHookHelper { get; }
        DrawingContext DrawingContext { get; }
        event EventHandler DrawingContextChanged;
    }
}
