using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Tools.Misc;
using System.Collections.Generic;
using System.Drawing;

namespace CaptureImage.Common.Tools
{
    public class LineTool : ITool
    {
        private List<Line> lines = new List<Line>();
        protected DrawingState state;
        protected Point mouseStartPos;
        protected Point mousePreviousPos;
        protected bool isActive;


        protected IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContextKeeper.DrawingContext;
        

        public LineTool(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;
            this.drawingContextProvider.Undo += DrawingContextProvider_Undo;

            this.state = DrawingState.None;

            mouseStartPos = new Point(0, 0);
            UpdateErasingPens();
        }

        private void DrawingContextProvider_Undo(object sender, System.EventArgs e)
        {
            if (lines.Count > 0)
            {
                Line lastLine = lines[lines.Count - 1];
                lines.RemoveAt(lines.Count - 1);
                UpdateErasingPens();

                DrawingContext.Erase((gr, pen) =>
                {
                    gr.DrawLine(pen, lastLine.Start, lastLine.End);
                });
            }
        }

        public virtual void MouseDown(Point mousePosition)
        {
            if (isActive)
            {
                mouseStartPos = mousePosition;
                state = DrawingState.Drawing;
            }
        }

        public virtual void MouseMove(Point mouse)
        {
            if (isActive)
            {
                if (state == DrawingState.Drawing)
                {
                    DrawingContext.Erase((gr, pen) =>
                    {
                        gr.DrawLine(pen, mouseStartPos, mousePreviousPos);
                    });

                    DrawingContext.Draw((gr, pen) => 
                    {
                        gr.DrawLine(pen, mouseStartPos, mouse);
                    });

                    mousePreviousPos = mouse;
                }
            }
        }

        public void MouseUp(Point mouse)
        {
            if (isActive)
            {
                Line line = new Line(mouseStartPos, mousePreviousPos);
                lines.Add(line);
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
                for (int i = 0; i < lines.Count; i++)
                {
                    gr.DrawLine(DrawingContext.drawingPen, lines[i].Start, lines[i].End);
                }
            });
        }
    }
}
