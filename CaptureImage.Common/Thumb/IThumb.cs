using System;
using System.Drawing;
using System.Windows.Forms;

namespace CaptureImage.Common.Thumb
{
    public interface IThumb
    {
        void Refresh(Rectangle bounds);
        Rectangle[] HandleRectangles { get; }
        Rectangle Bounds { get; set; }
        Point Location { get; }
        void HidePanels();
        void ShowPanels();
        void DrawBorder(Graphics gr);
        void OnGraphics(DrawingContext.DrawingContext.OnGraphicsDelegate toDo);
        void TranslateTransform(Graphics gr);
        void DrawBackgroundImage(Graphics gr, Image image);
        event EventHandler<ThumbState> StateChanged;
        event EventHandler<ThumbAction> ActionCalled;
        Control[] Components { get; }
        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseUp;
        event MouseEventHandler MouseMove;

    }
}
