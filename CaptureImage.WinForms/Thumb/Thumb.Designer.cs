using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common.Extensions;
using CaptureImage.WinForms.Properties;
using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Thumb;

namespace CaptureImage.WinForms.Thumb
{
    partial class Thumb
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // displaySizeLabel
            this.displaySizeLabel = new Label();
            this.displaySizeLabel.AutoSize = true;
            this.displaySizeLabel.Font = new Font(displaySizeLabel.Font.FontFamily, 10);
            this.displaySizeLabel.ForeColor = Color.White;
            this.displaySizeLabel.BackColor = Color.Black;

            // panelX
            this.panelX = new Panel();
            this.panelX.BackColor = System.Drawing.SystemColors.Control;
            this.panelX.Size = new Size(200, 30);
            this.panelX.SetRoundedShape(10);

            // panelY
            this.panelY = new Panel();
            this.panelY.BackColor = System.Drawing.SystemColors.Control;
            this.panelY.Size = new Size(30, 200);
            this.panelY.SetRoundedShape(10);

            // btnRedo
            this.btnUndo = new Button();
            this.btnUndo.Image = Resources.undo;
            this.btnUndo.Size = new Size(24, 24);
            this.btnUndo.Location = new Point(3, panelY.Location.Y + panelY.Size.Height - btnUndo.Height - 3);
            this.btnUndo.MouseClick += (sender, e) => CallAction(ThumbAction.Undo);

            // btnPencil
            this.btnPencil = new Button();
            this.btnPencil.Image = Resources.pencil;
            this.btnPencil.Size = new Size(24, 24);
            this.btnPencil.Location = new Point(3, 3);
            this.btnPencil.MouseClick += (sender, e) => SelectState(ThumbState.Pencil);

            // btnLine
            this.btnLine = new Button();
            this.btnLine.Image = Resources.line;
            this.btnLine.Size = new Size(24, 24);
            this.btnLine.Location = new Point(3, 27);
            this.btnLine.MouseClick += (sender, e) => SelectState(ThumbState.Line);

            // btnArrow
            this.btnArrow = new Button();
            this.btnArrow.Image = Resources.arrow;
            this.btnArrow.Size = new Size(24, 24);
            this.btnArrow.Location = new Point(3, 51);
            this.btnArrow.MouseClick += (sender, e) => SelectState(ThumbState.Arrow);

            // btnRect
            this.btnRect = new Button();
            this.btnRect.Image = Resources.rect;
            this.btnRect.Size = new Size(24, 24);
            this.btnRect.Location = new Point(3, 75);
            this.btnRect.MouseClick += (sender, e) => SelectState(ThumbState.Rect);

            // btnText
            this.btnText = new Button();
            this.btnText.Image = Resources.text;
            this.btnText.Size = new Size(24, 24);
            this.btnText.Location = new Point(3, 123);
            this.btnText.MouseClick += (sender, e) => SelectState(ThumbState.Text);

            // btnColor
            this.btnColor = new Button();
            this.btnColor.FlatStyle = FlatStyle.Flat;
            this.btnColor.FlatAppearance.BorderSize = 2;
            this.btnColor.FlatAppearance.BorderColor = Color.White;
            this.btnColor.BackColor = DrawingContext.DefaultDrawingPen.Color;
            this.btnColor.Size = new Size(24, 24);
            this.btnColor.Location = new Point(3, 147);
            this.btnColor.MouseClick += (sender, e) => CallAction(ThumbAction.Color);

            int xOffset = 13;
            // btnCpClipboard
            this.btnCpClipboard = new Button();
            this.btnCpClipboard.Image = Resources.copy;
            this.btnCpClipboard.Size = new Size(24, 24);
            this.btnCpClipboard.Location = new Point(xOffset + 96, 3);
            this.btnCpClipboard.MouseClick += (sender, e) => CallAction(ThumbAction.Copy);

            // btnSave
            this.btnSave = new Button();
            this.btnSave.Image = Resources.save;
            this.btnSave.Size = new Size(24, 24);
            this.btnSave.Location = new Point(xOffset + 128, 3);
            this.btnSave.MouseClick += (sender, e) => CallAction(ThumbAction.Save);

            // btnClose
            this.btnClose = new Button();
            this.btnClose.Image = Resources.close;
            this.btnClose.Size = new Size(24, 24);
            this.btnClose.Location = new Point(xOffset + 160, 3);
            this.btnClose.MouseClick += (sender, e) => CallAction(ThumbAction.Close);


            // toolTip
            this.toolTip = new ToolTip();
            this.toolTip.SetToolTip(btnPencil, "Карандаш");
            this.toolTip.SetToolTip(btnLine, "Линия");
            this.toolTip.SetToolTip(btnArrow, "Стрелка");
            this.toolTip.SetToolTip(btnRect, "Прямоугольник");
            this.toolTip.SetToolTip(btnText, "Текст");
            this.toolTip.SetToolTip(btnColor, "Цвет");
            this.toolTip.SetToolTip(btnUndo, "Отменить (Ctrl+Z)");
            this.toolTip.SetToolTip(btnClose, "Закрыть (Ctrl+X)");
            this.toolTip.SetToolTip(btnSave, "Сохранить (Ctrl+S)");
            this.toolTip.SetToolTip(btnCpClipboard, "Копировать (Ctrl+C)");

            this.panelY.Controls.Add(this.btnUndo);
            this.panelY.Controls.Add(this.btnPencil);
            this.panelY.Controls.Add(this.btnLine);
            this.panelY.Controls.Add(this.btnArrow);
            this.panelY.Controls.Add(this.btnRect);
            this.panelY.Controls.Add(this.btnText);
            this.panelY.Controls.Add(this.btnColor);

            panelX.Controls.Add(this.btnCpClipboard);
            panelX.Controls.Add(this.btnSave);
            panelX.Controls.Add(this.btnClose);


            // 
            // Thumb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "Thumb";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Thumb_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private Label displaySizeLabel;
        private Panel panelY;
        private Panel panelX;

        private Button btnUndo;
        private Button btnPencil;
        private Button btnLine;
        private Button btnArrow;
        private Button btnRect;
        private Button btnText;
        private Button btnColor;

        private Button btnCpClipboard;
        private Button btnSave;
        private Button btnClose;

        private ToolTip toolTip;
    }
}
