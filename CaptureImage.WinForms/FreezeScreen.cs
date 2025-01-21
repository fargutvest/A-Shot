using System.Drawing;

namespace CaptureImage.WinForms
{
    public partial class FreezeScreen : ScreenBase
    {
        private AppContext appContext;

        public FreezeScreen(AppContext appContext)
        {
            InitializeComponent();

            this.appContext = appContext;
            this.appContext.DrawingContextKeeper.DrawingContextChanged += DrawingContextKeeper_DrawingContextChanged;
        }

        private void DrawingContextKeeper_DrawingContextChanged(object sender, System.EventArgs e)
        {
            BackgroundImage = appContext.DrawingContextKeeper.DrawingContext.GetImage(this);
        }
    }
}
