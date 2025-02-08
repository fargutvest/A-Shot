using CaptureImage.Common.Helpers;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common;
using CaptureImage.Common.Extensions;
using System;
using CaptureImage.WinForms.Properties;
using CaptureImage.Common.DrawingContext;

namespace CaptureImage.WinForms.Thumb
{
    public partial class Thumb : UserControl, IThumb
    {
        public Rectangle[] HandleRectangles { get; private set; }

        private AppContext appContext;

        private Label displaySizeLabel;
        private Panel panelY;
        private Panel panelX;

        private Button btnUndo;
        private Button btnPencil;
        private Button btnLine;
        private Button btnArrow;
        private Button btnRect;
        private Button btnColor;

        private Button btnCpClipboard;
        private Button btnSave;
        private Button btnClose;

        public Control[] Components { get; }

        private ThumbState state;

        public event EventHandler<ThumbState> StateChanged;

        public event EventHandler<ThumbAction> ActionCalled;

        public Thumb(AppContext appContext)
        {
            this.appContext = appContext;
            appContext.DrawingContextChanged += DrawingContextsKeeper_DrawingContextChanged;

            InitializeComponent();

            this.DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

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
            this.btnCpClipboard.MouseClick += (sender, e) => CallAction(ThumbAction.CopyToClipboard);

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

            this.panelY.Controls.Add(this.btnUndo);
            this.panelY.Controls.Add(this.btnPencil);
            this.panelY.Controls.Add(this.btnLine);
            this.panelY.Controls.Add(this.btnArrow);
            this.panelY.Controls.Add(this.btnRect);
            this.panelY.Controls.Add(this.btnColor);

            panelX.Controls.Add(this.btnCpClipboard);
            panelX.Controls.Add(this.btnSave);
            panelX.Controls.Add(this.btnClose);

            HandleRectangles = new Rectangle[0];

            Components = new Control[]
            {
                this,
                this.displaySizeLabel,
                this.panelX,
                this.panelY,
            };
        }

        private void DrawingContextsKeeper_DrawingContextChanged(object sender, EventArgs e)
        {
            appContext.DrawingContext.Updated += DrawingContext_Updated;
        }

        private void DrawingContext_Updated(object sender, EventArgs e)
        {
            btnColor.BackColor = appContext.DrawingContext.drawingPen.Color;
            btnColor.Invalidate();
        }

        private void SelectState(ThumbState state)
        {
            this.state = state;
            StateChanged?.Invoke(this, state);
        }

        private void CallAction(ThumbAction action)
        {
            ActionCalled?.Invoke(this, action);
        }

        private void Thumb_Paint(object sender, PaintEventArgs e)
        {
            if (SelectionRectangle.Width > 0 && SelectionRectangle.Height > 0)
            {
                Rectangle thumbRect = new Rectangle(0, 0, Width, Height);
                e.Graphics.DrawImage(appContext.DrawingContext.GetImage(), thumbRect, SelectionRectangle, GraphicsUnit.Pixel);
            }

            DrawBorder(e.Graphics);

            // displaySizeLabel
            displaySizeLabel.Text = $"{Size.Width}x{Size.Height}";
            displaySizeLabel.Location = new Point(this.Location.X, this.Location.Y - displaySizeLabel.Height);
            displaySizeLabel.Refresh();

            // panelX
            panelX.Location = new Point(this.Location.X + this.Width - panelX.Width, this.Location.Y + this.Height);
            panelX.Refresh();

            // panelY
            panelY.Location = new Point(this.Location.X + this.Width, this.Location.Y + this.Height - panelY.Height);
            panelY.Refresh();
        }

        public void DrawBorder(Graphics gr)
        {
            int handleSize = 5;
            int padding = 2;
            Rectangle rect = new Rectangle(handleSize / 2 + padding, handleSize / 2 + padding, this.Width - handleSize - padding * 2, this.Height - handleSize - padding * 2);

            HandleRectangles = GraphicsHelper.DrawSelectionBorder(gr, rect, handleSize);

            for (int i = 0; i < HandleRectangles.Length; i++)
            {
                HandleRectangles[i].Offset(this.Location);
            }
        }

        public Rectangle SelectionRectangle { get; private set; }

        public void SetSize(Size size, Rectangle rect)
        {
            this.Size = size;
            this.displaySizeLabel.Visible = size.Width > 0 && size.Height > 0;
            SelectionRectangle = rect;  
        }

        public void ShowPanels()
        {
            this.panelX.Visible = true;
            this.panelY.Visible = true;
        }

        public void HidePanels()
        {
            this.panelX.Visible = false;
            this.panelY.Visible = false;
        }
    }
}
