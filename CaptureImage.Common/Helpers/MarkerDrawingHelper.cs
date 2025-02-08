using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Drawings;
using System.Drawing;
using System.Windows.Forms;

namespace CaptureImage.Common.Helpers
{
    public static class MarkerDrawingHelper
    {
        private static Pen markerPen = new Pen(Color.Violet, 2);
        private static int diameter = 2;
        private static IDrawing marker;

        public static int GetPenDiameter()
        {
            return diameter;
        }

        public static void IncreaseMarkerDiameter()
        {
            if (diameter < 20)
                diameter = diameter + 1;
        }

        public static void DecreaseMarkerDiameter()
        {
            if (diameter > 5)
                diameter = diameter - 1;
        }

        public static void ReDrawMarker(DrawingContext.DrawingContext drawingContext)
        {
            if (marker != null)
                drawingContext.Erase(marker.Erase, DrawingTarget.Canvas);

            DrawMarkerInternal(drawingContext);
        }

        public static void DrawMarker(DrawingContext.DrawingContext drawingContext) 
        {
            DrawMarkerInternal(drawingContext);
        }
        
        #region private
        
        private static void DrawMarkerInternal(DrawingContext.DrawingContext drawingContext)
        {
            drawingContext.Draw((gr, pen) =>
            {
                marker = GetMarker(Cursor.Position);
                marker.Paint(gr, markerPen);
            }, DrawingTarget.Canvas);
        }

        private static IDrawing GetMarker(Point location) => new Circle(diameter,
            location: new Point(location.X - diameter / 2, location.Y - diameter / 2));

        #endregion

    }
}
