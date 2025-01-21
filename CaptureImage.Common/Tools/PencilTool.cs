using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Tools.Misc;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CaptureImage.Common.Tools
{
    public class PencilTool : ITool
    {
        private List<Line> lines = new List<Line>();
        private List<List<Line>> drawings = new List<List<Line>>();
        private DrawingState state;
        private Point mouseStartPos;
        private Point mousePreviousPos;
        private bool isActive;
        protected IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContextKeeper.DrawingContext;

        public PencilTool(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
            this.drawingContextProvider.Undo += DrawingContextProvider_Undo;

            this.state = DrawingState.None;
            mousePreviousPos = new Point(0, 0);
            UpdateErasingPens();
        }

        private void DrawingContextProvider_Undo(object sender, System.EventArgs e)
        {
            if (drawings.Count > 0)
            {
                List<Line> lastDrawing = drawings[drawings.Count - 1];
                drawings.RemoveAt(drawings.Count - 1);

                UpdateErasingPens();

                foreach (Line line in lastDrawing)
                {
                    DrawingContext.Erase((gr, pen) =>
                    {
                        gr.DrawLine(pen, line.Start, line.End);
                    });
                }
            }
        }

        public void MouseMove(Point mouse)
        {
            if (isActive)
            {
                if (state == DrawingState.Drawing)
                {
                    DrawingContext.Draw((gr, pen) => 
                    {
                        gr.DrawLine(pen, mousePreviousPos, mouse);
                    });

                    Line line = new Line(mousePreviousPos, mouse);
                    drawings.Last().Add(line);
                    mousePreviousPos = mouse;
                }
            }
        }

        public void MouseDown(Point mouse)
        {
            if (isActive)
            {
                drawings.Add(new List<Line>());
                mouseStartPos = mouse;
                mousePreviousPos = mouse;
                state = DrawingState.Drawing;
            }
        }

        public void MouseUp(Point mouse)
        {
            if (isActive)
            {
                UpdateErasingPens();
                state = DrawingState.None;
            }
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            isActive = false;
        }

        private void UpdateErasingPens()
        {
            DrawingContext.UpdateErasingPens(gr =>
            {
                for (int i = 0; i < drawings.Count; i++)
                {
                    List<Line> lines = drawings[i];

                    foreach (Line line in lines)
                        gr.DrawLine(DrawingContext.drawingPen, line.Start, line.End);
                }
            });
        }
    }
}
