using CaptureImage.Common;
using CaptureImage.Common.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.WinForms.Properties;
using System.IO;
using CaptureImage.WinForms.Helpers;
using CaptureImage.Common.DrawingContext;
using CaptureImage.Common.Helpers.HotKeys;

namespace CaptureImage.WinForms
{
    public class AppContext : ApplicationContext, IDrawingContextProvider
    {
        private NotifyIcon trayIcon;
        private HotKeysHelper hotKeysHelper;
        private MouseHookHelper mouseHookHelper;
        private BlackoutScreen blackoutScreen;
        private bool isSessionOn;
        private Bitmap screenShot;

        private DrawingContext drawingContext;
        public DrawingContext DrawingContext
        {
            get
            {
                return drawingContext;
            }
            private set
            {
                drawingContext = value;
                DrawingContextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler DrawingContextChanged;

        public AppContext()
        {
            isSessionOn = false;

            hotKeysHelper = new HotKeysHelper();
            hotKeysHelper.RegisterHotKey(Keys.Control, Keys.P, StartSession);
            hotKeysHelper.RegisterHotKey(Keys.Control, Keys.Z, UndoDrawing);
            hotKeysHelper.RegisterHotKey(Keys.Control, Keys.C, MakeScreenshot);
            hotKeysHelper.RegisterHotKey(Keys.Escape, OnEscape);

            mouseHookHelper = new MouseHookHelper();
            mouseHookHelper.MouseWheel += MouseHookHelper_MouseWheel;

            trayIcon = new NotifyIcon()
            {
                Icon = Resources.ashot,
                Visible = true,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Сделать скриншот", (sender, e) => StartSession()),
                    new MenuItem("Выход", Exit)}),
            };

            blackoutScreen = new BlackoutScreen(this);
        }

        private void MouseHookHelper_MouseWheel(object sender, int e)
        {
            if (e > 0)
                MarkerDrawingHelper.DecreaseMarkerDiameter();
            
            else if (e < 0)
                MarkerDrawingHelper.IncreaseMarkerDiameter();
        }

        public void UndoDrawing()
        {
            DrawingContext.UndoDrawing();
        }

        private void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void OnEscape()
        {
            if (blackoutScreen.Mode == Mode.Drawing)
            {
                blackoutScreen.SwitchToSelectingMode();
                return;
            }
            else if (blackoutScreen.Mode == Mode.Selecting)
            {
                if (isSessionOn)
                    EndSession();
            }
        }

        public void EndSession()
        {
            blackoutScreen.Hide();
            blackoutScreen.ResetSelection();
            isSessionOn = false;
        }

        private void StartSession()
        {
            if (isSessionOn)
                return;

            DesktopInfo desktopInfo = ScreensHelper.GetDesktopInfo();

            screenShot = desktopInfo.Background;

            DrawingContext = DrawingContext.Create(BitmapHelper.DarkenImage(desktopInfo.Background, 0.5f), 
                blackoutScreen, isClean: true);

            ConfigureForm(blackoutScreen, desktopInfo);

            blackoutScreen.Invalidate();
            blackoutScreen.Show();

            isSessionOn = true;
        }

        private Bitmap GetScreenshot(Rectangle rect)
        {
            if (screenShot != null)
                return BitmapHelper.Crop((Bitmap)screenShot, rect);

            return null;
        }

        public void MakeScreenshot()
        {
            MakeScreenshot(blackoutScreen.selectingTool.selectingRect);
            EndSession();
        }

        public void MakeScreenshot(Rectangle rect)
        {
            Clipboard.SetImage(GetScreenshot(rect));
        }

        internal void SaveScreenshot(Rectangle rect)
        {
            Bitmap bitmap = GetScreenshot(rect);
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG|*.png|JPEG|*.jpeg|BMP|*.bmp";

            string lastDirectory = Properties.Settings.Default.LastSaveDirectory;


            if (string.IsNullOrEmpty(lastDirectory) == false)
            {
                sfd.InitialDirectory = lastDirectory;
                sfd.FileName = FileNameHelper.SuggestFileName(lastDirectory);
            }

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                bitmap.Save(sfd.FileName);

                Properties.Settings.Default.LastSaveDirectory = Path.GetDirectoryName(sfd.FileName);
                Properties.Settings.Default.Save();
            }
        }

        internal void SelectColor()
        {
            ColorDialog colorDialog = new ColorDialog();
            
            if (colorDialog.ShowDialog() == DialogResult.OK)
                DrawingContext.SetColorOfPen(colorDialog.Color);
        }

        private void ConfigureForm(Form form, DesktopInfo desktopInfo) 
        {
            form.ClientSize = desktopInfo.ClientSize;
            form.Location = desktopInfo.Location;
            form.Region = new Region(desktopInfo.Path);
        }


        protected override void ExitThreadCore()
        {
            this.hotKeysHelper.Dispose();
            base.ExitThreadCore();
        }

    }
}
    