using System.Drawing;

namespace CaptureImage.Common
{
    public interface IThumb
    {
        Rectangle[] HandleRectangles { get; }
        bool Visible { get; set; }
        
        Point Location { get; set; }
        void SetSize(Size size);
        void Refresh();

        void HidePanels();
        void ShowPanels();
    }
}
