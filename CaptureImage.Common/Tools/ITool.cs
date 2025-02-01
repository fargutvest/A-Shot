using System.Drawing;

namespace CaptureImage.Common.Tools
{
    public interface ITool
    {
        void Activate();

        void Deactivate();

        void MouseMove(Point mouse);

        void MouseUp(Point mouse);

        void MouseDown(Point mouse);
    }
}
