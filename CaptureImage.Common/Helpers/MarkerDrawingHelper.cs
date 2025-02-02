using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Drawings;
using System.Drawing;

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

        private static IDrawing GetMarker(Point location) => new Circle(diameter,
                location: new Point(location.X - diameter /2, location.Y - diameter / 2));

        public static void EraseMarker(DrawingContext.DrawingContext drawingContext)
        {
            if (marker != null)
                drawingContext.Erase(marker, DrawingTarget.CanvasOnly);
        }

        public static void DrawMarker(DrawingContext.DrawingContext drawingContext, Point location) 
        {
            DrawMarker(drawingContext, latestDrawing: null, location);
        }

        public static void DrawMarker(DrawingContext.DrawingContext drawingContext, IDrawing latestDrawing, Point location)
        {
            if (latestDrawing != null)
                drawingContext.ReRenderDrawingOnCanvas(latestDrawing);

            drawingContext.Draw((gr, pen) =>
            {
                marker = GetMarker(location);
                marker.Paint(gr, markerPen);
            }, DrawingTarget.CanvasOnly);
        }
    }
}
