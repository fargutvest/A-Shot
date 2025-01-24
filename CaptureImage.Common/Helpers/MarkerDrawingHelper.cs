using CaptureImage.Common.Drawings;
using System;
using System.Drawing;

namespace CaptureImage.Common.Helpers
{
    internal static class MarkerDrawingHelper
    {
        private static Pen markerPen = new Pen(Color.Black);

        private static IDrawing GetMarker(Point location)
        {
            int diameter = 5;
            int offset = 5;

            return new Circle(diameter,
                location: new Point(location.X - offset, location.Y - offset));
        }

        internal static void EraseMarker(DrawingContext.DrawingContext drawingContext, Point location)
        {
            drawingContext.Erase(GetMarker(location).Paint);
        }

        internal static void DrawMarker(DrawingContext.DrawingContext drawingContext, IDrawing latestDrawing, Point location)
        {
            if (latestDrawing != null)
                drawingContext.DrawOverErasingPens(latestDrawing.Paint);

            drawingContext.Draw((gr, pen) =>
            {
                GetMarker(location).Paint(gr, markerPen);
            });
        }
    }
}
