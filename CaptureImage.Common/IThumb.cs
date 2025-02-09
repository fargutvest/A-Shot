using System.Drawing;

namespace CaptureImage.Common
{
    public interface IThumb
    {
        void Refresh(Rectangle bounds);
        Rectangle[] HandleRectangles { get; }
        Rectangle Bounds { get; }
        void HidePanels();
        void ShowPanels();
        void DrawBorder(Graphics gr);
        void OnGraphics(DrawingContext.DrawingContext.OnGraphicsDelegate toDo);
        void TranslateTransform(Graphics gr);
        void DrawBackgroundImage(Graphics gr, Image image);
    }
}
