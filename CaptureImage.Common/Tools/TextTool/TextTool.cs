using CaptureImage.Common.DrawingContext;
using System.Drawing;
using System.Windows.Forms;
using System;
using CaptureImage.Common.Helpers;
using System.Collections.Generic;
using Text = CaptureImage.Common.Drawings.Text;
using System.Linq;

namespace CaptureImage.Common.Tools
{
    public class TextTool : ITool, IKeyInputReceiver
    {
        private bool shiftPressed;
        private Point relativeMouseStartPos;
        private string fontName = "Veranda";
        private int numberOfCharWithCursor = 0;
        private int numberOfCharWithCursorShift = -1;
        private KeyEventArgs specialKeyDown = null;
        private readonly Color colorOfCursor = Color.DarkRed;
        private Text text;
        private readonly Timer cursorTimer;
        private Point textCursorUp = Point.Empty;
        private Point textCursorDown = Point.Empty;
        private bool textCursorVisible;
        private Rectangle textAreaRect;
        private readonly List<char> chars;
        private bool isMouseDown;
        private readonly int topPaddingText = 5;
        private readonly int leftPaddingText = 5;
        private readonly int rightPaddingText = 20;
        private bool isActive;
        private Point mousePosition;
        private ICanvas canvas;
        private readonly IDrawingContextProvider drawingContextProvider;
        private DrawingContext.DrawingContext DrawingContext => drawingContextProvider.DrawingContext;

        private float FontSize => MarkerDrawingHelper.GetPenDiameter() * 5;

        public TextTool(IDrawingContextProvider drawingContextProvider, ICanvas canvas)
        {
            this.drawingContextProvider = drawingContextProvider;
            this.canvas = canvas;
            chars = new List<char>();

            this.cursorTimer = new Timer();
            cursorTimer.Interval = 500;
            cursorTimer.Tick += CursorTimer_Tick;
            cursorTimer.Start();

        }

        #region ITool

        public void MouseMove(Point mouse)
        {
            if (isMouseDown)
                mousePosition = mouse;

            bool isMouseOver = textAreaRect.Contains(mouse);

            if (isActive)
            {
                if (isMouseDown)
                    Refresh();
                

                if (isMouseOver)
                    Cursor.Current = Cursors.SizeAll;
                
                else
                    Cursor.Current = Cursors.Default;
            }
        }

        public void MouseUp(Point mouse)
        {
            if (isActive)
            {
                isMouseDown = false;
                mousePosition = mouse;
                Refresh();
            }
        }

        public void MouseDown(Point mouse)
        {
            if (isActive)
            {
                isMouseDown = true;
                mousePosition = mouse;

                if (textAreaRect.Contains(mouse))
                    relativeMouseStartPos = textAreaRect.Location.IsEmpty ? Point.Empty :
                        new Point(mousePosition.X - textAreaRect.X, mousePosition.Y - textAreaRect.Y);
                else
                    RememberText();
            }
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Deactivate()
        {
            cursorTimer.Stop();
            isActive = false;
            RememberText();
        }

        #endregion

        #region IKeyInputReceiver

        public void KeyPress(KeyPressEventArgs e)
        {
            if (specialKeyDown == null)
            {
                chars.Insert(numberOfCharWithCursor, e.KeyChar);
                numberOfCharWithCursor += 1;
                Refresh();
            }
        }

        public void KeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                numberOfCharWithCursorShift = numberOfCharWithCursor;
                shiftPressed = true;
            }
               

            if (IsSpecialKey(e.KeyCode))
            {
                specialKeyDown = e;
            }
        }

        public void KeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                shiftPressed = false;

            if (IsSpecialKey(e.KeyCode))
            {
                if (e.KeyData == specialKeyDown?.KeyData)
                {
                    specialKeyDown = null;
                }

                ProcessSpecialKeyUp(e);
            }
        }

        public void MouseWheel(MouseEventArgs e)
        {
            Refresh();
        }

        #endregion

        #region private 

        private void RememberText()
        {
            chars.Clear();
            numberOfCharWithCursor = 0;

            if (text != null)
                DrawingContext.RenderDrawing(text, needRemember: true);
        }

        private void Paint(Graphics gr, Color textColor)
        {
            CalculateForPaint(gr);
            
            Rectangle borderRect = new Rectangle(textAreaRect.Location, textAreaRect.Size);
            borderRect.Width -= 1;
            borderRect.Height -= 1;
            GraphicsHelper.DrawBorder(gr, borderRect);

            using (Pen pen = new Pen(colorOfCursor, 5))
            {
                if (textCursorVisible)
                    gr.DrawLine(pen, textCursorUp, textCursorDown);

                Point textLocation = textAreaRect.Location;
                textLocation.Y += topPaddingText;
                textLocation.X += leftPaddingText;

                string str = new string(chars.ToArray());

                int startIndexToHighlight = 0;
                int lengthToHighlight = 0;

                GetShiftSelection(out startIndexToHighlight, out lengthToHighlight);

                text = new Text(str, startIndexToHighlight, lengthToHighlight, fontName, FontSize, textColor, textLocation);
                text.Paint(gr, null);
            }
        }

        private void CalculateForPaint(Graphics gr)
        {
            float overWidth = 0;
            if (chars.Count > 0 && numberOfCharWithCursor > 0)
                overWidth = GraphicsHelper.GetFirstSymbolOverWidth(gr, chars.First(), fontName, FontSize); 

            int textWidth = (int)GraphicsHelper.GetStringSize(gr, new string(chars.ToArray()), fontName, FontSize).Width;
            int cursorPosition = (int)GraphicsHelper.GetStringSize(gr, new string(chars.Take(numberOfCharWithCursor).ToArray()), fontName, FontSize).Width;

            int textHeight = (int)GraphicsHelper.GetStringSize(gr, "1", fontName, FontSize).Height;

            Point textAreaRectPos = new Point(mousePosition.X - relativeMouseStartPos.X, mousePosition.Y - relativeMouseStartPos.Y);

            textAreaRect = new Rectangle(textAreaRectPos, new Size(0, textHeight * 2));
            textAreaRect.Width = leftPaddingText + textWidth + rightPaddingText;
                
            textCursorUp = textAreaRect.Location;
            textCursorUp.X += leftPaddingText + cursorPosition - (int)overWidth;
            textCursorUp.Y += topPaddingText;
            textCursorDown = textCursorUp;
            textCursorDown.Y += textHeight;
        }
        
        private void CursorTimer_Tick(object sender, EventArgs e)
        {
            textCursorVisible = !textCursorVisible;
            Refresh();
        }

        private void Refresh()
        {
            DrawingContext.RenderDrawing(null, needRemember: false);
            DrawingContext.Draw((gr, _) => Paint(gr, DrawingContext.GetColorOfPen()), DrawingTarget.Canvas);
        }

        private bool IsSpecialKey(Keys key)
        {
            if (key < Keys.D0 || key > Keys.Z)
                return true;

            return false;
        }
        
        private void ProcessSpecialKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Back:

                    if (chars.Count > 0)
                    {
                        if (numberOfCharWithCursorShift != -1)
                        {
                            GetShiftSelection(out int start, out int length);

                            chars.RemoveRange(start, length);

                            numberOfCharWithCursor = start;
                            numberOfCharWithCursorShift = -1;
                        }
                        else
                        {
                            chars.RemoveAt(numberOfCharWithCursor - 1);
                            numberOfCharWithCursor -= 1;
                        }
                       
                        Refresh();
                    }

                    break;

                case Keys.Delete:

                    if (chars.Count > 0)
                    {
                        if (numberOfCharWithCursorShift != -1)
                        {
                            GetShiftSelection(out int start, out int length);
                            
                            chars.RemoveRange(start, length);

                            numberOfCharWithCursor = start;
                            numberOfCharWithCursorShift = -1;
                        }
                        else
                        {
                            chars.RemoveAt(numberOfCharWithCursor);
                        }

                        Refresh();
                    }

                    break;

                case Keys.Left:

                    if (shiftPressed == false && numberOfCharWithCursorShift != -1 && numberOfCharWithCursorShift < numberOfCharWithCursor)
                    {
                        numberOfCharWithCursor = numberOfCharWithCursorShift;
                        numberOfCharWithCursorShift = -1;
                    }
                    else if (shiftPressed == false && numberOfCharWithCursorShift != -1 && numberOfCharWithCursorShift > numberOfCharWithCursor)
                    {
                        numberOfCharWithCursorShift = -1;
                    }
                    else
                    {
                        if (numberOfCharWithCursor > 0)
                            numberOfCharWithCursor -= 1;
                    }

                    Refresh();

                break;

                case Keys.Right:


                    if (shiftPressed == false && numberOfCharWithCursorShift != -1 && numberOfCharWithCursorShift > numberOfCharWithCursor)
                    {
                        numberOfCharWithCursor = numberOfCharWithCursorShift;
                        numberOfCharWithCursorShift = -1;
                    }
                    else if (shiftPressed == false && numberOfCharWithCursorShift!= -1 && numberOfCharWithCursorShift < numberOfCharWithCursor)
                    {
                        numberOfCharWithCursorShift = -1;
                    }
                    else
                    {
                        if (numberOfCharWithCursor < chars.Count)
                            numberOfCharWithCursor += 1;
                    }

                    Refresh();

                    break;
            }

        }

        private void GetShiftSelection(out int start, out int length)
        {
            start = 0;
            length = 0;

            if (numberOfCharWithCursorShift == -1)
                return;

            if (numberOfCharWithCursor > numberOfCharWithCursorShift)
            {
                start = numberOfCharWithCursorShift;
                length = numberOfCharWithCursor - numberOfCharWithCursorShift;
            }

            else if (numberOfCharWithCursor < numberOfCharWithCursorShift)
            {
                start = numberOfCharWithCursor;
                length = numberOfCharWithCursorShift - numberOfCharWithCursor;
            }
        }


        #endregion
    }
}
