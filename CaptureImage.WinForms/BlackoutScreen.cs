using CaptureImage.Common.Tools;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.Common.Extensions;
using System.Linq;
using CaptureImage.Common;
using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Helpers;
using CaptureImage.Common.Helpers.HotKeys;
using CaptureImage.Common.Thumb;

namespace CaptureImage.WinForms
{
    public partial class BlackoutScreen : Form, ICanvas
    {
        private bool isInit = true;
        private readonly IThumb thumb;
        private readonly SelectingTool selectingTool;
        private ITool drawingTool;
        private readonly AppContext appContext;
        private Point mousePosition;
        private MouseHookHelper hookHelper;

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
            UpdateStyles();;

            hookHelper = new MouseHookHelper();

            this.appContext = appContext;
            this.appContext.DrawingContextChanged += AppContext_DrawingContextChanged;

#if RELEASE
            TopMost = true;
#endif
            
            //this.thumb = new Thumb.ThumbNew(appContext, this);
            this.thumb = new Thumb.Thumb(appContext);
            this.thumb.Bounds = Rectangle.Empty;
            this.thumb.MouseDown += (sender, e) => BlackoutScreen_MouseDown(sender, e.Offset(thumb.Location));
            this.thumb.MouseUp += (sender, e) => BlackoutScreen_MouseUp(sender, e.Offset(thumb.Location));
            this.thumb.MouseMove += (sender, e) => BlackoutScreen_MouseMove(sender, e.Offset(thumb.Location));
            this.thumb.StateChanged += Thumb_StateChanged;
            this.thumb.ActionCalled += Thumb_ActionCalled;

            foreach (Control control in thumb.Components.Except(new[] { this.thumb as Control }))
            {
                control.MouseMove += BlackoutScreen_MouseMove;
            }

            this.Controls.AddRange(thumb.Components);

            selectingTool = new SelectingTool(this);
            selectingTool.Activate();

            Mode = Mode.Selecting;
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

        private void Thumb_ActionCalled(object sender, ThumbAction e)
        {
            switch (e)
            {
                case ThumbAction.Copy:
                    appContext.MakeScreenShot();
                    break;

                case ThumbAction.Undo:
                    appContext.UndoDrawing();
                    if (appContext.DrawingContext.IsClean)
                        SwitchToSelectingMode();
                    break;

                case ThumbAction.Save:
                    appContext.SaveScreenShot();
                    break;

                case ThumbAction.Close:
                    appContext.EndSession();
                    break;

                case ThumbAction.Color:
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
                    drawingTool?.Deactivate();
                    drawingTool = new PencilTool(appContext);
                    drawingTool.Activate();
                    Mode = Mode.Drawing;
                    break;
                case ThumbState.Line:
                    selectingTool.Deactivate();
                    drawingTool?.Deactivate();
                    drawingTool = new LineTool(appContext);
                    drawingTool.Activate();
                    Mode = Mode.Drawing;
                    break;
                case ThumbState.Arrow:
                    selectingTool.Deactivate();
                    drawingTool?.Deactivate();
                    drawingTool = new ArrowTool(appContext);
                    drawingTool.Activate();
                    Mode = Mode.Drawing;
                    break;
                case ThumbState.Rect:
                    selectingTool.Deactivate();
                    drawingTool?.Deactivate();
                    drawingTool = new RectTool(appContext);
                    drawingTool.Activate();
                    Mode = Mode.Drawing;
                    break;
                case ThumbState.Text:
                    selectingTool.Deactivate();
                    drawingTool?.Deactivate();
                    drawingTool = new TextTool(appContext);
                    drawingTool.Activate();
                    Mode = Mode.Drawing;
                    break;

            }
        }

        private void BlackoutScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Control && sender is IThumb == false && sender != this)
                MarkerDrawingHelper.IsMarkerEnabled = false;
            else
                MarkerDrawingHelper.IsMarkerEnabled = true;


            mousePosition = e.Location;
            MouseMoveEvent(e.Location);
        }


        private void BlackoutScreen_MouseWheel(object sender, MouseEventArgs e)
        {
            if (drawingTool is IKeyInputReceiver textTool)
                textTool.MouseWheel(e);

            if (e.Delta > 0)
                MarkerDrawingHelper.DecreaseMarkerDiameter();

            else if (e.Delta < 0)
                MarkerDrawingHelper.IncreaseMarkerDiameter();

            MarkerDrawingHelper.ReDrawMarker(appContext.DrawingContext, mousePosition);
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
            if (drawingTool is IKeyInputReceiver textTool)
                textTool.KeyDown(e);
        }

        private void BlackoutScreen_KeyUp(object sender, KeyEventArgs e)
        {
            if (drawingTool is IKeyInputReceiver textTool)
                textTool.KeyUp(e);
        }

        private void BlackoutScreen_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (drawingTool is IKeyInputReceiver textTool)
                textTool.KeyPress(e);
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
