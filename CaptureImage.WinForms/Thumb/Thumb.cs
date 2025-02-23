using CaptureImage.Common.Helpers;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common;
using System;
using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Thumb;

namespace CaptureImage.WinForms.Thumb
{
    public partial class Thumb : Control, IThumb
    {
        private readonly AppContext appContext;
        private ICanvas canvas;

        public Rectangle[] HandleRectangles { get; private set; }

        public Control[] Components { get; }
        
        public event EventHandler<ThumbState> StateChanged;

        public event EventHandler<ThumbAction> ActionCalled;

        public Thumb(AppContext appContext, ICanvas canvas)
        {
            this.appContext = appContext;
            this.canvas = canvas;
            appContext.DrawingContextChanged += DrawingContextsKeeper_DrawingContextChanged;
            InitializeComponent();

            this.DoubleBuffered = true;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            HandleRectangles = Array.Empty<Rectangle>();

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
            btnColor.BackColor = appContext.DrawingContext.GetColorOfPen();
            btnColor.Invalidate();
        }

        private void SelectState(ThumbState state)
        {
            StateChanged?.Invoke(this, state);
        }

        private void CallAction(ThumbAction action)
        {
            ActionCalled?.Invoke(this, action);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
           CalculateBeforePaint();   
           base.OnPaint(e);
        }

        private void Thumb_Paint(object sender, PaintEventArgs e)
        {
            if (this.Width > 0 && this.Height > 0)
            {
                e.Graphics.DrawImage(appContext.DrawingContext.GetImage(), ClientRectangle, this.Bounds, GraphicsUnit.Pixel);
            }

            DrawBorder(e.Graphics);
        }

        private void CalculateBeforePaint()
        {
            // displaySizeLabel
            displaySizeLabel.Visible = this.Size.Width > 0 && this.Size.Height > 0;
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
            Rectangle rect = new Rectangle(handleSize / 2 + padding, handleSize / 2 + padding,
                this.Width - handleSize - padding * 2, this.Height - handleSize - padding * 2);

            HandleRectangles = GraphicsHelper.DrawBorderWithHandles(gr, rect, handleSize);

            for (int i = 0; i < HandleRectangles.Length; i++)
            {
                HandleRectangles[i].Offset(this.Location);
            }
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

        public void OnGraphics(DrawingContext.OnGraphicsDelegate toDo)
        {
            using (Graphics gr = this.CreateGraphics())
            {
                toDo?.Invoke(gr, this.ClientRectangle);
            }
        }

        public void TranslateTransform(Graphics gr)
        {
            gr.TranslateTransform(-this.Location.X, -this.Location.Y);
        }

        public void DrawBackgroundImage(Graphics gr, Image image)
        {
            gr.DrawImage(image, ClientRectangle, this.Bounds, GraphicsUnit.Pixel);
        }
    }
}
