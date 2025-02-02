using System;
using System.Drawing;

namespace CaptureImage.Common
{
    public interface ICanvas
    {
        void OnGraphics(Action<Graphics, Action<Graphics>> action);

        Rectangle ClientRectangle { get; }

        IThumb GetThumb { get; }

        Image BackgroundImage { get; set; }

        void Refresh();
    }
}
