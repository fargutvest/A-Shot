using System.Drawing;

namespace CaptureImage.Common
{
    public interface IThumb
    {
        Rectangle[] HandleRectangles { get; }
        bool Visible { get; set; }
        Rectangle SelectionRectangle { get; }
        Point Location { get; set; }
        void SetSize(Size size, Rectangle rect);
        void HidePanels();
        void ShowPanels();
        void DrawBorder(Graphics gr);
        void OnGraphics(DrawingContext.DrawingContext.OnGraphicsDelegate toDo);
    }
}
