using CaptureImage.Common.Helpers;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common;
using System;
using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Thumb;

namespace CaptureImage.WinForms.Thumb
{
    internal partial class ThumbNew : IThumb
    {
        private readonly AppContext appContext;
        private readonly ICanvas canvas;

        public Rectangle[] HandleRectangles { get; private set; }

        public Control[] Components { get; }

        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseMove;

        public event EventHandler<ThumbState> StateChanged;

        public event EventHandler<ThumbAction> ActionCalled;

        public Rectangle Bounds { get; set; }
        public Point Location => Bounds.Location;

        public ThumbNew(AppContext appContext, ICanvas canvas)
        {
            this.appContext = appContext;
            this.canvas = canvas;
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

        private void Paint(Graphics gr)
        {
            if (this.Bounds.Width > 0 && this.Bounds.Height > 0)
            {
                gr.DrawImage(appContext.DrawingContext.GetImage(), this.Bounds, this.Bounds,
                    GraphicsUnit.Pixel);
            }

            DrawBorder(gr);

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
            Rectangle rect = new Rectangle(this.Bounds.X + handleSize / 2 + padding, this.Bounds.Y + handleSize / 2 + padding,
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

            OnGraphics((gr, rect) =>
            {
                Paint(gr);
            });
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
            this.canvas.OnGraphics((gr, rect) =>
            {
                toDo?.Invoke(gr, this.Bounds);
            });
        }

        public void TranslateTransform(Graphics gr)
        {
            gr.TranslateTransform(-this.Bounds.Location.X, -this.Bounds.Location.Y);
        }

        public void DrawBackgroundImage(Graphics gr, Image image)
        {
            gr.DrawImage(image, this.Bounds, this.Bounds, GraphicsUnit.Pixel);
        }
    }
}
