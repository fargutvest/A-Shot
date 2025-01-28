using System.Windows.Forms;

namespace CaptureImage.WinForms
{
    public partial class FreezeScreen : ScreenBase
    {
        private AppContext appContext;

        public FreezeScreen(AppContext appContext)
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            this.appContext = appContext;
            this.appContext.DrawingContextChanged += AppContext_DrawingContextChanged;
        }

        private void AppContext_DrawingContextChanged(object sender, System.EventArgs e)
        {
            BackgroundImage = appContext.DrawingContext.GetImage(this);
        }
    }
}
