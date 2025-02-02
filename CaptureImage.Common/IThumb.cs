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
        void Refresh();
        void HidePanels();
        void ShowPanels();
    }
}
