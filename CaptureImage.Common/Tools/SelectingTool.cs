using CaptureImage.Common.Extensions;
using CaptureImage.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CaptureImage.Common.Thumb;
using CaptureImage.Common.Tools.Misc;


namespace CaptureImage.Common.Tools
{
    public class SelectingTool
    {
        private SelectingState state;
        private Rectangle selectingRect;
        private Point mousePos;
        private Point mouseStartPos;
        private Point relativeMouseStartPos;
        private Rectangle[] handleRectangles;
        private int hoveredHandleIndex;
        private Rectangle selectingRectResizeStart;
        private readonly Dictionary<int, Cursor> handleCursors;
        private bool isActive;
        private ICanvas canvas;
        private readonly ToolTip cursorHint;

        private bool IsHandleHovered => handleRectangles.Any(rect => rect.Contains(mousePos));

        private bool isMouseOver;

        public bool IsMouseOver
        {
            get 
            { 
                return isMouseOver; 
            }
            private set
            {
                if (isMouseOver != value)
                {
                    isMouseOver = value;

                    if (isMouseOver)
                        MouseEnterSelection?.Invoke(this, mousePos);
                }
            }
        }

        public event EventHandler<Point> MouseEnterSelection;

        public SelectingTool(ICanvas canvas)
        {
            this.canvas = canvas;
            handleRectangles = new Rectangle[0];

            handleCursors = new Dictionary<int, Cursor>
            {
                { 0, Cursors.SizeNWSE },/// угол
                { 1, Cursors.SizeNS },
                { 2, Cursors.SizeNESW }, // угол
                { 3, Cursors.SizeWE },
                { 4, Cursors.SizeNWSE }, // угол
                { 5, Cursors.SizeNS },
                { 6, Cursors.SizeNESW }, // угол
                { 7, Cursors.SizeWE }
            };

            cursorHint = new ToolTip();
        }
        
        public void Paint(Graphics gr, Bitmap background)
        {
            gr.DrawImage(background, selectingRect, selectingRect, GraphicsUnit.Pixel);

            handleRectangles = GraphicsHelper.DrawBorderWithHandles(gr, selectingRect);
        }

        public void Paint(IThumb selector)
        {
            if (isActive)
            {
                if (selectingRect.Size == Size.Empty)
                    return;
                
                switch (state)
                {
                    case SelectingState.Selecting:
                    case SelectingState.Moving:
                    case SelectingState.Resizing:
                        selector.HidePanels();
                        handleRectangles = selector.HandleRectangles;

                        selector.Location = selectingRect.Location;

                        if (selector.Size != selectingRect.Size)
                        {
                            selector.Size = selectingRect.Size;
                        }
                        else
                        {
                            selector.Refresh();
                            canvas.Refresh();
                        }

                        break;
                    case SelectingState.None:
                        if (selectingRect.Width > 0 && selectingRect.Height > 0)
                            selector.ShowPanels();
                        else
                            selector.HidePanels();
                        break;
                }
            }
        }

        public void MouseDown(Point mousePosition)
        {
            if (isActive)
            {
                mouseStartPos = mousePosition;

                if ((selectingRect.IsEmpty || IsMouseOver == false) && IsHandleHovered == false)
                {
                    state = SelectingState.Selecting;
                }
                else if (selectingRect.IsEmpty == false && IsMouseOver && IsHandleHovered == false)
                {
                    state = SelectingState.Moving;

                    relativeMouseStartPos = new Point(mousePosition.X - selectingRect.X, mousePosition.Y - selectingRect.Y);
                }
                else if (selectingRect.IsEmpty == false && IsHandleHovered)
                {
                    state = SelectingState.Resizing;

                    Rectangle hoveredHandleRect = handleRectangles.First(rect => rect.Contains(mousePos));
                    hoveredHandleIndex = handleRectangles.ToList().IndexOf(hoveredHandleRect);
                    selectingRectResizeStart = selectingRect.Clone();
                }
            }
        }

        public void MouseUp(Point mousePosition)
        {
            if (isActive)
            {
                if (state == SelectingState.Selecting)
                {
                    mousePos = mousePosition;
                    UpdateSelectingRect();
                }

                state = SelectingState.None;
            }
        }


        public void MouseMove(Point mousePosition)
        {
            mousePos = mousePosition;
            IsMouseOver = selectingRect.Contains(mousePos);
            
            if (isActive)
            {
                if (IsHandleHovered)
                {
                    Rectangle hoveredHandleRect = handleRectangles.First(rect => rect.Contains(mousePos));
                    int rectangleIndex = handleRectangles.ToList().IndexOf(hoveredHandleRect);
                    canvas.Cursor = handleCursors[rectangleIndex];
                }
                else if (IsMouseOver)
                {
                    canvas.Cursor = Cursors.SizeAll;
                }
                else
                {
                    canvas.Cursor = Cursors.Default;
                }

                switch (state)
                {
                    case SelectingState.Selecting:
                        UpdateSelectingRect();
                        break;
                    case SelectingState.Moving:
                        MoveSelectingRect();
                        break;
                    case SelectingState.Resizing:
                        ResizeSelectingRect();
                        break;
                }
                
                int offsetY = 30;

                if (selectingRect.IsEmpty)
                    cursorHint.Show("Выберите область", canvas, mousePosition.X, mousePosition.Y + offsetY);
                else
                    cursorHint.Hide(canvas);
            }
        }

        public void HideSelecting()
        {
            selectingRect = Rectangle.Empty;
        }

        public void Select(Rectangle selectingRect)
        {
            this.selectingRect = selectingRect;
        }

        private void UpdateSelectingRect()
        {
            selectingRect = new Rectangle(
                mouseStartPos.X, mouseStartPos.Y,
                mousePos.X - mouseStartPos.X, mousePos.Y - mouseStartPos.Y
            );

            FlipIfNeeded();
        }

        private void MoveSelectingRect()
        {
            selectingRect = new Rectangle(mousePos.X - relativeMouseStartPos.X, mousePos.Y - relativeMouseStartPos.Y, selectingRect.Width, selectingRect.Height);
        }

        private void ResizeSelectingRect()
        {

            int deltaX = mousePos.X - mouseStartPos.X;
            int deltaY = mousePos.Y - mouseStartPos.Y;

            switch (hoveredHandleIndex)
            {
                case 0: // угол
                    selectingRect = new Rectangle(selectingRectResizeStart.X + deltaX, selectingRectResizeStart.Y + deltaY,
                        selectingRectResizeStart.Width - deltaX, selectingRectResizeStart.Height - deltaY);
                    break;

                case 1:
                    selectingRect = new Rectangle(selectingRect.X, selectingRectResizeStart.Y + deltaY,
                      selectingRect.Width, selectingRectResizeStart.Height - deltaY);
                    break;

                case 2: // угол
                    selectingRect = new Rectangle(selectingRectResizeStart.X, selectingRectResizeStart.Y + deltaY,
                      selectingRectResizeStart.Width + deltaX, selectingRectResizeStart.Height - deltaY);
                    break;

                case 3:
                    selectingRect = new Rectangle(selectingRectResizeStart.X, selectingRect.Y,
                    selectingRectResizeStart.Width + deltaX, selectingRect.Height);
                    break;

                case 4: // угол
                    selectingRect = new Rectangle(selectingRectResizeStart.X, selectingRectResizeStart.Y,
                     selectingRectResizeStart.Width + deltaX, selectingRectResizeStart.Height + deltaY);
                    break;

                case 5:
                    selectingRect = new Rectangle(selectingRect.X, selectingRect.Y,
                        selectingRect.Width, selectingRectResizeStart.Height + deltaY);
                    break;

                case 6: // угол
                    selectingRect = new Rectangle(selectingRectResizeStart.X + deltaX, selectingRectResizeStart.Y,
                    selectingRectResizeStart.Width - deltaX, selectingRectResizeStart.Height + deltaY);
                    break;

                case 7:
                    selectingRect = new Rectangle(selectingRectResizeStart.X + deltaX, selectingRect.Y,
                    selectingRectResizeStart.Width - deltaX, selectingRect.Height);
                    break;

            }

            FlipIfNeeded();
        }

        private void FlipIfNeeded()
        {
            if (selectingRect.Width < 0)
            {
                selectingRect.X = selectingRect.X + selectingRect.Width;
                selectingRect.Width = -selectingRect.Width;
            }


            if (selectingRect.Height < 0)
            {
                selectingRect.Y = selectingRect.Y + selectingRect.Height;
                selectingRect.Height = -selectingRect.Height;

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
