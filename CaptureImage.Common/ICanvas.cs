﻿using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common.Thumb;

namespace CaptureImage.Common
{
    public interface ICanvas : IWin32Window
    {
        void OnGraphics(DrawingContext.DrawingContext.OnGraphicsDelegate toDo);
        
        IThumb GetThumb { get; }

        void DrawBackgroundImage(Graphics gr, Image image);

        Cursor Cursor { get; set; }

        void Refresh();
    }
}
