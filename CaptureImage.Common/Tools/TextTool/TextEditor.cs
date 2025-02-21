using System.Drawing;
using System.Windows.Forms;
using System;
using CaptureImage.Common.Helpers;
using System.Collections.Generic;
using Text = CaptureImage.Common.Drawings.Text;
using System.Linq;


namespace CaptureImage.Common.Tools
{
    internal class TextEditor: IKeyInputReceiver, IDisposable
    {
        private bool shiftPressed;
        private readonly string fontName = "Veranda";
        private int numberOfCharWithCursor = 0;
        private int numberOfCharWithCursorShift = -1;
        private KeyEventArgs specialKeyDown = null;
        private readonly Color colorOfCursor = Color.DarkRed;
        private readonly Timer cursorTimer;
        private Point textCursorTop = Point.Empty;
        private Point textCursorBottom = Point.Empty;
        private bool textCursorVisible;
        private Rectangle textAreaRect;
        private readonly List<char> chars;
        private readonly int topPaddingText = 5;
        private readonly int leftPaddingText = 5;
        private readonly int rightPaddingText = 20;

        private float FontSize => MarkerDrawingHelper.GetPenDiameter() * 5;

        public Rectangle Bounds => textAreaRect;

        public Point Translate(Point point) => Bounds.Location.IsEmpty || Bounds.Contains(point) == false ? Point.Empty : new Point(point.X - Bounds.X, point.Y - Bounds.Y);

        public event EventHandler Updated;

        public TextEditor()
        {
            chars = new List<char>();

            this.cursorTimer = new Timer();
            cursorTimer.Interval = 500;
            cursorTimer.Tick += CursorTimer_Tick;
            cursorTimer.Start();
        }

        public void CleanText()
        {
            chars.Clear();
            numberOfCharWithCursor = 0;
        }
        
        public Text GetDrawing(Color textColor, Point location)
        {
            using (Bitmap bitmap = new Bitmap(1, 1))
            {
                using (Graphics gr = Graphics.FromImage(bitmap))
                {
                    float overWidth = 0;
                    if (chars.Count > 0 && numberOfCharWithCursor > 0)
                        overWidth = GraphicsHelper.GetFirstSymbolOverWidth(gr, chars.First(), fontName, FontSize);

                    int textWidth = (int)GraphicsHelper.GetStringSize(gr, new string(chars.ToArray()), fontName, FontSize).Width;
                    int cursorPosition = (int)GraphicsHelper.GetStringSize(gr, new string(chars.Take(numberOfCharWithCursor).ToArray()), fontName, FontSize).Width;

                    int textHeight = (int)GraphicsHelper.GetStringSize(gr, "1", fontName, FontSize).Height;
                    
                    textAreaRect = new Rectangle(location, new Size(0, textHeight * 2));
                    textAreaRect.Width = leftPaddingText + textWidth + rightPaddingText;

                    textCursorTop = textAreaRect.Location;
                    textCursorTop.X += leftPaddingText + cursorPosition - (int)overWidth;
                    textCursorTop.Y += topPaddingText;
                    textCursorBottom = textCursorTop;
                    textCursorBottom.Y += textHeight;
                }
            }

            Rectangle borderRect = new Rectangle(textAreaRect.Location, textAreaRect.Size);
            borderRect.Width -= 1;
            borderRect.Height -= 1;
            
            Point textLocation = textAreaRect.Location;
            textLocation.Y += topPaddingText;
            textLocation.X += leftPaddingText;

            string str = new string(chars.ToArray());
            Text text = new Text(str, fontName, FontSize, textColor, textLocation);
            GetShiftSelection(out var startIndexToHighlight, out var lengthToHighlight);

            text.Highlight(startIndexToHighlight, lengthToHighlight);
            text.SetBorderRect(borderRect);
            text.ShowBorder = true;
            text.SetCursor(textCursorTop, textCursorBottom);
            text.ShowCursor = textCursorVisible;
            return text;
        }


        #region IKeyInputReceiver

        public void MouseWheel(MouseEventArgs e)
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }
        
        public void KeyPress(KeyPressEventArgs e)
        {
            if (specialKeyDown == null)
            {
                ProcessKeyPress(e);
                Updated?.Invoke(this, EventArgs.Empty);
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

        #endregion


        #region private 
        
        private void CursorTimer_Tick(object sender, EventArgs e)
        {
            textCursorVisible = !textCursorVisible;
            Updated?.Invoke(this, EventArgs.Empty);
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

                        Updated?.Invoke(this, EventArgs.Empty);
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

                        Updated?.Invoke(this, EventArgs.Empty);
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

                    Updated?.Invoke(this, EventArgs.Empty);

                    break;

                case Keys.Right:


                    if (shiftPressed == false && numberOfCharWithCursorShift != -1 && numberOfCharWithCursorShift > numberOfCharWithCursor)
                    {
                        numberOfCharWithCursor = numberOfCharWithCursorShift;
                        numberOfCharWithCursorShift = -1;
                    }
                    else if (shiftPressed == false && numberOfCharWithCursorShift != -1 && numberOfCharWithCursorShift < numberOfCharWithCursor)
                    {
                        numberOfCharWithCursorShift = -1;
                    }
                    else
                    {
                        if (numberOfCharWithCursor < chars.Count)
                            numberOfCharWithCursor += 1;
                    }

                    Updated?.Invoke(this, EventArgs.Empty);

                    break;
            }
        }

        private void ProcessKeyPress(KeyPressEventArgs e)
        {
            if (numberOfCharWithCursorShift != -1)
            {
                if (chars.Count > 0)
                {
                    GetShiftSelection(out int start, out int length);
                    chars.RemoveRange(start, length);
                    numberOfCharWithCursor = start;
                    numberOfCharWithCursorShift = -1;
                }
            }

            chars.Insert(numberOfCharWithCursor, e.KeyChar);
            numberOfCharWithCursor += 1;
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

        public void Dispose()
        {
            cursorTimer?.Stop();
            cursorTimer?.Dispose();
        }
    }

}

