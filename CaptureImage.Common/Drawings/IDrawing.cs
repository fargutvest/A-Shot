using System.Drawing;

namespace CaptureImage.Common.Drawings
{
    public interface IDrawing
    {
        void Paint(Graphics gr, Pen pen);
        void Repaint(Graphics gr);

        void Erase(Graphics gr, Pen erasePen);
    }
}
