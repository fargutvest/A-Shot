﻿using System;
using System.Drawing;

namespace CaptureImage.Common
{
    public interface ICanvas
    {
        void OnGraphics(Action<Graphics> action);

        Rectangle ClientRectangle { get; }

        Image BackgroundImage { get; set; }

        void Refresh();
    }
}
