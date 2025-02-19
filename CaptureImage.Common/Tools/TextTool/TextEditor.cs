﻿using CaptureImage.Common.DrawingContext;
using System.Drawing;
using System.Windows.Forms;
using System;
using CaptureImage.Common.Helpers;
using System.Collections.Generic;
using Text = CaptureImage.Common.Drawings.Text;
using System.Linq;


namespace CaptureImage.Common.Tools
{
    internal class TextEditor: IKeyInputReceiver
    {
        private bool shiftPressed;
        private string fontName = "Veranda";
        public int numberOfCharWithCursor = 0;
        private int numberOfCharWithCursorShift = -1;
        private KeyEventArgs specialKeyDown = null;
        private readonly Color colorOfCursor = Color.DarkRed;
        public Text text;
        private readonly Timer cursorTimer;
        private Point textCursorUp = Point.Empty;
        private Point textCursorDown = Point.Empty;
        private bool textCursorVisible;
        public Rectangle textAreaRect;
        public readonly List<char> chars;
        private bool isMouseDown;
        private readonly int topPaddingText = 5;
        private readonly int leftPaddingText = 5;
        private readonly int rightPaddingText = 20;
        private bool isActive;

        private float FontSize => MarkerDrawingHelper.GetPenDiameter() * 5;

        public event EventHandler Refresh;

        public TextEditor()
        {
            chars = new List<char>();

            this.cursorTimer = new Timer();
            cursorTimer.Interval = 500;
            cursorTimer.Tick += CursorTimer_Tick;
            cursorTimer.Start();
        }


        #region IKeyInputReceiver

        public void MouseWheel(MouseEventArgs e)
        {
            Refresh?.Invoke(this, new EventArgs());
        }


        public void KeyPress(KeyPressEventArgs e)
        {
            if (specialKeyDown == null)
            {
                chars.Insert(numberOfCharWithCursor, e.KeyChar);
                numberOfCharWithCursor += 1;
                Refresh?.Invoke(this, new EventArgs());
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


        public void Paint(Graphics gr, Color textColor, Point mousePosition, Point relativeMouseStartPos)
        {
            CalculateForPaint(gr, mousePosition, relativeMouseStartPos);

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

        private void CalculateForPaint(Graphics gr, Point mousePosition, Point relativeMouseStartPos)
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
            Refresh?.Invoke(this, new EventArgs());
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

                        Refresh?.Invoke(this, new EventArgs());
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

                        Refresh?.Invoke(this, new EventArgs());
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

                    Refresh?.Invoke(this, new EventArgs());

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

                    Refresh?.Invoke(this, new EventArgs());

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

