using CaptureImage.Common.Tools;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common.Extensions;
using System.Linq;
using System;
using CaptureImage.Common;

namespace CaptureImage.WinForms
{
    public partial class BlackoutScreen : Form, ICanvas
    {
        private bool isInit = true;
        public Thumb.Thumb thumb;
        public SelectingTool selectingTool;
        private ITool drawingTool;

        private AppContext appContext;

        public Mode Mode { get; set; }

        public BlackoutScreen(AppContext appContext)
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            this.appContext = appContext;
            this.appContext.DrawingContextChanged += AppContext_DrawingContextChanged;
            
            //TopMost = true;

            selectingTool = new SelectingTool();
            selectingTool.Activate();

            Mode = Mode.Selecting;

            this.thumb = new Thumb.Thumb(appContext);
            this.thumb.Size = new Size(0, 0);
            this.thumb.MouseDown += (sender, e) => BlackoutScreen_MouseDown(sender, e.Offset(thumb.Location));
            this.thumb.MouseUp += (sender, e) => BlackoutScreen_MouseUp(sender, e.Offset(thumb.Location));
            this.thumb.MouseMove += (sender, e) => BlackoutScreen_MouseMove(sender, e.Offset(thumb.Location));
            this.thumb.StateChanged += Thumb_StateChanged;
            this.thumb.ActionCalled += Thumb_ActionCalled;

            foreach (Control control in thumb.Components.Except(new Control[] { this.thumb }))
            {
                control.MouseMove += (sender, e) => BlackoutScreen_MouseMove(sender, e);
            }

            this.Controls.AddRange(thumb.Components);
        }

        private void AppContext_DrawingContextChanged(object sender, System.EventArgs e)
        {
            BackgroundImage = appContext.DrawingContext.GetImage();
        }

        public void SwitchToSelectingMode()
        {
            selectingTool?.Activate();
            drawingTool?.Deactivate();
            Mode = Mode.Selecting;
        }

        public void ResetSelection()
        {
            selectingTool.Select(Rectangle.Empty);
        }

        private void Thumb_ActionCalled(object sender, Thumb.ThumbAction e)
        {
            switch (e)
            {
                case Thumb.ThumbAction.CopyToClipboard:
                    appContext.MakeScreenshot(selectingTool.selectingRect);
                    appContext.EndSession();
                    break;

                case Thumb.ThumbAction.Undo:
                    appContext.UndoDrawing();
                    if (appContext.DrawingContext.IsClean)
                    {
                        SwitchToSelectingMode();
                    }
                    break;

                case Thumb.ThumbAction.Save:
                    appContext.SaveScreenshot(selectingTool.selectingRect);
                    break;

                case Thumb.ThumbAction.Close:
                    appContext.EndSession();
                    break;

                case Thumb.ThumbAction.Color:
                    appContext.SelectColor();
                    break;
            }
        }

        private void Thumb_StateChanged(object sender, ThumbState e)
        {
            switch (e)
            {
                case ThumbState.Selecting:
                    selectingTool.Activate();
                    drawingTool.Deactivate();
                    Mode = Mode.Drawing;
                    break;
                case ThumbState.Pencil:
                    selectingTool.Deactivate();
                    drawingTool = new PencilTool(appContext);
                    drawingTool.Activate();
                    Mode = Mode.Drawing;
                    break;
                case ThumbState.Line:
                    selectingTool.Deactivate();
                    drawingTool = new LineTool(appContext);
                    drawingTool.Activate();
                    Mode = Mode.Drawing;
                    break;
                case ThumbState.Arrow:
                    selectingTool.Deactivate();
                    drawingTool = new ArrowTool(appContext);
                    drawingTool.Activate();
                    Mode = Mode.Drawing;
                    break;
                case ThumbState.Rect:
                    selectingTool.Deactivate();
                    drawingTool = new RectTool(appContext);
                    drawingTool.Activate();
                    Mode = Mode.Drawing;
                    break;

            }
        }

        private void BlackoutScreen_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMoveEvent(e.Location);
        }

        private void MouseMoveEvent(Point mouse)
        {
            selectingTool.MouseMove(mouse, this);
            selectingTool.Paint(this.thumb);
            drawingTool?.MouseMove(mouse);
        }

        private void BlackoutScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectingTool.MouseUp(e.Location);
                drawingTool?.MouseUp(e.Location);
            }
        }

        private void BlackoutScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectingTool.MouseDown(e.Location);
                drawingTool?.MouseDown(e.Location);
            }
        }

        private void BlackoutScreen_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        public void OnGraphics(Action<Graphics, Action<Graphics>> action)
        {
            using (Graphics gr = this.CreateGraphics()) { action?.Invoke(gr, null); }

            using (Graphics gr = this.thumb.CreateGraphics())
            {
                action?.Invoke(gr, bufferedGr =>
                {
                    thumb.DrawBorder(bufferedGr);
                });
            }
        }

        public IThumb GetThumb => thumb;
    }
}
