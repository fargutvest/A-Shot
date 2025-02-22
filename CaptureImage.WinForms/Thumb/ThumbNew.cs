using CaptureImage.Common.Helpers;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common;
using System;
using CaptureImage.Common.DrawingContext;

namespace CaptureImage.WinForms.Thumb
{
    internal partial class ThumbNew
    {
        private readonly AppContext appContext;

        public Rectangle[] HandleRectangles { get; private set; }

        public Control[] Components { get; }

        public event EventHandler<ThumbState> StateChanged;

        public event EventHandler<ThumbAction> ActionCalled;

        public Rectangle Bounds { get; set; }

        public Rectangle ClientRectangle { get; set; }

        public ThumbNew(AppContext appContext)
        {
            this.appContext = appContext;
            appContext.DrawingContextChanged += DrawingContextsKeeper_DrawingContextChanged;

            InitializeComponent();
            
            HandleRectangles = Array.Empty<Rectangle>();

            Components = new Control[]
            {
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

        private void Thumb_Paint(object sender, PaintEventArgs e)
        {
            if (this.Bounds.Width > 0 && this.Bounds.Height > 0)
            {
                e.Graphics.DrawImage(appContext.DrawingContext.GetImage(), ClientRectangle, this.Bounds,
                    GraphicsUnit.Pixel);
            }

            DrawBorder(e.Graphics);

            // displaySizeLabel
            displaySizeLabel.Text = $"{this.Bounds.Size.Width}x{this.Bounds.Size.Height}";
            displaySizeLabel.Location = new Point(this.Bounds.Location.X, this.Bounds.Location.Y - displaySizeLabel.Height);
            displaySizeLabel.Refresh();

            // panelX
            panelX.Location = new Point(this.Bounds.Location.X + this.Bounds.Size.Width - panelX.Width, this.Bounds.Location.Y + this.Bounds.Height);
            panelX.Refresh();

            // panelY
            panelY.Location = new Point(this.Bounds.Location.X + this.Bounds.Size.Width, this.Bounds.Location.Y + this.Bounds.Size.Height - panelY.Height);
            panelY.Refresh();
        }

        public void DrawBorder(Graphics gr)
        {
            int handleSize = 5;
            int padding = 2;
            Rectangle rect = new Rectangle(handleSize / 2 + padding, handleSize / 2 + padding,
                this.Bounds.Size.Width - handleSize - padding * 2, this.Bounds.Size.Height - handleSize - padding * 2);

            HandleRectangles = GraphicsHelper.DrawBorderWithHandles(gr, rect, handleSize);

            for (int i = 0; i < HandleRectangles.Length; i++)
            {
                HandleRectangles[i].Offset(this.Bounds.Location);
            }
        }

        public void Refresh(Rectangle bounds)
        {
            this.Bounds = bounds;
            this.displaySizeLabel.Visible = this.Bounds.Size.Width > 0 && this.Bounds.Size.Height > 0;
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
            //using (Graphics gr = this.CreateGraphics())
            //{
            //    toDo?.Invoke(gr, this.ClientRectangle);
            //}
        }

        public void TranslateTransform(Graphics gr)
        {
            gr.TranslateTransform(-this.Bounds.Location.X, -this.Bounds.Location.Y);
        }

        public void DrawBackgroundImage(Graphics gr, Image image)
        {
            gr.DrawImage(image, ClientRectangle, this.Bounds, GraphicsUnit.Pixel);
        }
    }
}
