using System;
using System.Drawing;

namespace CaptureImage.Common
{
    public interface ICanvas
    {
        void OnGraphics(DrawingContext.DrawingContext.OnGraphicsDelegate toDo);

        Rectangle ClientRectangle { get; }

        IThumb GetThumb { get; }
    }
}
