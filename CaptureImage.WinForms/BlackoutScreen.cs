using CaptureImage.Common.Tools;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common.Extensions;
using System.Linq;
using CaptureImage.Common;
using CaptureImage.Common.DrawingContext;

namespace CaptureImage.WinForms
{
    public partial class BlackoutScreen : Form, ICanvas
    {
        private bool isInit = true;
        private readonly Thumb.Thumb thumb;
        private readonly SelectingTool selectingTool;
        private ITool drawingTool;
        private readonly AppContext appContext;

        

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

            this.appContext.hotKeysHelper.KeyPress += HotKeysHelper_KeyPress;

#if RELEASE
            TopMost = true;
#endif
            selectingTool = new SelectingTool(this);
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
                control.MouseMove += BlackoutScreen_MouseMove;
            }

            this.Controls.AddRange(thumb.Components);
        }

        private void HotKeysHelper_KeyPress(object sender, char keyChar)
        {
            if (drawingTool is ITextTool textTool)
            {
                textTool.KeyPress(keyChar);
            }
        }

        private void AppContext_DrawingContextChanged(object sender, System.EventArgs e)
        {
            BackgroundImage = appContext.DrawingContext.GetCanvasImage();

            this.appContext.DrawingContext.Updated += DrawingContext_Updated;
        }

        private void DrawingContext_Updated(object sender, System.EventArgs e)
        {
            BackgroundImage = appContext.DrawingContext.GetCanvasImage();
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
                case Thumb.ThumbAction.Copy:
                    appContext.MakeScreenShot();
                    break;

                case Thumb.ThumbAction.Undo:
                    appContext.UndoDrawing();
                    if (appContext.DrawingContext.IsClean)
                        SwitchToSelectingMode();
                    break;

                case Thumb.ThumbAction.Save:
                    appContext.SaveScreenShot();
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
                case ThumbState.Text:
                    selectingTool.Deactivate();
                    drawingTool = new TextTool(appContext, this);
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
            selectingTool.MouseMove(mouse);
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

        private void BlackoutScreen_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            
        }

        public void OnGraphics(DrawingContext.OnGraphicsDelegate toDo)
        {
            using (Graphics gr = this.CreateGraphics()) { toDo?.Invoke(gr, this.ClientRectangle); }
        }
        
        public IThumb GetThumb => thumb;

        public void DrawBackgroundImage(Graphics gr, Image image)
        {
            gr.DrawImage(image, ClientRectangle, ClientRectangle, GraphicsUnit.Pixel);
        }
    }
}
