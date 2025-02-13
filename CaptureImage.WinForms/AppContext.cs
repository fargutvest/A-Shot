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
        private readonly NotifyIcon trayIcon;
        public readonly HotKeysHelper hotKeysHelper;
        public MouseHookHelper mouseHookHelper { get; }
        private readonly BlackoutScreen blackoutScreen;
        private bool isSessionOn;
        private Bitmap screenShot;

        private DrawingContext drawingContext;
        public DrawingContext DrawingContext
        {
            get => drawingContext;
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
            hotKeysHelper.RegisterHotKey(Keys.Control, Keys.C, MakeScreenShot);
            hotKeysHelper.RegisterHotKey(Keys.Control, Keys.S, SaveScreenShot);
            hotKeysHelper.RegisterHotKey(Keys.Control, Keys.X, EndSession);
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

            MarkerDrawingHelper.ReDrawMarker(DrawingContext);
        }

        public void UndoDrawing()
        {
            if (isSessionOn == false)
                return;

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

            DrawingContext = DrawingContext.Create(desktopInfo.Background, blackoutScreen, isClean: true);
            
            blackoutScreen.ClientSize = desktopInfo.ClientSize;
            blackoutScreen.Location = desktopInfo.Location;
            blackoutScreen.Region = new Region(desktopInfo.Path);

            blackoutScreen.Invalidate();
            blackoutScreen.Show();

            isSessionOn = true;
        }

        private Bitmap GetScreenShot(Rectangle rect)
        {
            if (screenShot != null)
                return BitmapHelper.Crop((Bitmap)screenShot, rect);

            return null;
        }

        public void MakeScreenShot()
        {
            if (isSessionOn == false)
                return;

            Clipboard.SetImage(GetScreenShot(blackoutScreen.GetThumb.Bounds));
            EndSession();
        }

        public void SaveScreenShot()
        {
            if (isSessionOn == false)
                return;

            Bitmap bitmap = GetScreenShot(blackoutScreen.GetThumb.Bounds);
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
        
        protected override void ExitThreadCore()
        {
            this.hotKeysHelper.Dispose();
            base.ExitThreadCore();
        }

    }
}
    