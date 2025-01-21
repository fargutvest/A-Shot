using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Drawings;
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

            this.state = DrawingState.None;

            mouseStartPos = new Point(0, 0);
            DrawingContext.UpdateErasingPens();
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
                DrawingContext.Drawings.Add(line);
                DrawingContext.UpdateErasingPens();
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
    }
}
