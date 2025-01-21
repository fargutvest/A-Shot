using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Drawings;
using CaptureImage.Common.Tools.Misc;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CaptureImage.Common.Tools
{
    public class PencilTool : ITool
    {
        private List<Curve> drawings;
        private DrawingState state;
        private Point mouseStartPos;
        private Point mousePreviousPos;
        private bool isActive;
        protected IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContextKeeper.DrawingContext;

        public PencilTool(IDrawingContextProvider drawingContextProvider)
        {
            this.drawingContextProvider = drawingContextProvider;

            this.state = DrawingState.None;
            mousePreviousPos = new Point(0, 0);

            this.drawings = new List<Curve>();
            DrawingContext.UpdateErasingPens();
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
                    drawings.Last().Lines.Add(line);
                    mousePreviousPos = mouse;
                }
            }
        }

        public void MouseDown(Point mouse)
        {
            if (isActive)
            {
                Curve drawing = new Curve();
                DrawingContext.Drawings.Add(drawing);
                drawings.Add(drawing);
                mouseStartPos = mouse;
                mousePreviousPos = mouse;
                state = DrawingState.Drawing;
            }
        }

        public void MouseUp(Point mouse)
        {
            if (isActive)
            {
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
