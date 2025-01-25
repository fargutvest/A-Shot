using CaptureImage.Common.Drawings;
using System.Drawing;

namespace CaptureImage.Common.Helpers
{
    public static class MarkerDrawingHelper
    {
        private static Pen markerPen = new Pen(Color.Violet, 2);

        private static int diameter = 5;

        private static int offset = 5;

        private static IDrawing marker;

        public static int GetPenDiameter()
        {
            return diameter;
        }

        public static void IncreaseMarkerDiameter()
        {
            if (diameter < 20)
            {
                diameter = diameter + 1;
                offset = offset + 1;
            }
        }

        public static void DecreaseMarkerDiameter()
        {
            if (diameter > 5)
            {
                diameter = diameter - 1;
                offset = offset - 1;
            }
        }

        private static IDrawing GetMarker(Point location) => new Circle(diameter,
                location: new Point(location.X - offset, location.Y - offset));
        

        public static void EraseMarker(DrawingContext.DrawingContext drawingContext, Point location)
        {
            IDrawing marker = GetMarker(location);
            drawingContext.Erase(marker.Erase, onlyOnCanvas: true);
        }

        public static void EraseMarker(DrawingContext.DrawingContext drawingContext)
        {
            if (marker != null)
                drawingContext.Erase(marker.Erase, onlyOnCanvas: true);
        }

        public static void DrawMarker(DrawingContext.DrawingContext drawingContext, IDrawing latestDrawing, Point location)
        {
            if (latestDrawing != null)
                drawingContext.DrawOverErasingPens(latestDrawing.Paint);

            drawingContext.Draw((gr, pen) =>
            {
                marker = GetMarker(location);
                marker.Paint(gr, markerPen);
            }, onlyOnCanvas: true);
        }
    }
}
