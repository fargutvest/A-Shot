namespace CaptureImage.WinForms
{
    public partial class FreezeScreen : ScreenBase
    {
        private AppContext appContext;

        public FreezeScreen(AppContext appContext)
        {
            InitializeComponent();

            this.appContext = appContext;
            this.appContext.DrawingContextChanged += AppContext_DrawingContextChanged;
        }

        private void AppContext_DrawingContextChanged(object sender, System.EventArgs e)
        {
            BackgroundImage = appContext.DrawingContext.GetImage(this);
        }
    }
}
