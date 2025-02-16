
using System.Windows.Forms;

namespace CaptureImage.Common.Tools
{
    public interface IKeyInputReceiver
    {
        void KeyPress(KeyPressEventArgs e);
        void KeyDown(KeyEventArgs e);
        void KeyUp(KeyEventArgs e);

        void MouseWheel(MouseEventArgs e);
    }
}
