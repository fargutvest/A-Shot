using System.Collections.Generic;

namespace CaptureImage.Common
{
    public class ChangesHistory
    {
        private Stack<DrawingContext.DrawingContext> changes;

        public ChangesHistory()
        {
            changes = new Stack<DrawingContext.DrawingContext>();
        }

        public DrawingContext.DrawingContext GetPrevious()
        {
            if (changes.Count > 1)
                changes.Pop();

            if (changes.Count == 0)
                return new DrawingContext.DrawingContext();

            DrawingContext.DrawingContext drawingContexts = changes.Peek();

            return drawingContexts.Clone() as DrawingContext.DrawingContext;
        }

        public DrawingContext.DrawingContext GetCurrent()
        {
            return changes.Peek();
        }

        public void SaveChange(DrawingContext.DrawingContext drawingContext)
        {
            DrawingContext.DrawingContext clone = drawingContext.Clone() as DrawingContext.DrawingContext;
            changes.Push(clone);
        }
    }
}
