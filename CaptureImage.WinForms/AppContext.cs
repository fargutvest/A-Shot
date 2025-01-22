using CaptureImage.Common;
using CaptureImage.Common.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.WinForms.Properties;
using System.IO;
using CaptureImage.WinForms.Helpers;
using CaptureImage.Common.DrawingContext;

namespace CaptureImage.WinForms
{
    public class AppContext : ApplicationContext, IDrawingContextProvider
    {
        private NotifyIcon trayIcon;
        private HotKeysHelper hotKeysHelper;
        private FreezeScreen freezeScreen;
        private BlackoutScreen blackoutScreen;
        private bool isHidden;

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
            isHidden = true;

            MainForm = new MainForm(this);

            hotKeysHelper = new HotKeysHelper();
            hotKeysHelper.RegisterHotKey(MainForm.Handle, Keys.F6, StartSession);
            hotKeysHelper.RegisterHotKey(MainForm.Handle, Keys.Escape, OnEscape);

            trayIcon = new NotifyIcon()
            {
                Icon = Resources.ashot,
                Visible = true,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Сделать скриншот", (sender, e) => StartSession()),
                    new MenuItem("Выход", Exit)}),
            };

            freezeScreen = new FreezeScreen(this);
            blackoutScreen = new BlackoutScreen(this);
            blackoutScreen.TransparencyKey = Color.Red;
            blackoutScreen.AllowTransparency = true;
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
                if (isHidden == false)
                    EndSession();
            }
        }

        public void EndSession()
        {
            freezeScreen.Hide();
            blackoutScreen.Hide();
            blackoutScreen.ResetSelection();
            isHidden = true;
        }

        private void StartSession()
        {
            DesktopInfo desktopInfo = ScreensHelper.GetDesktopInfo();

            Image[] images = new Image[]
            {
                desktopInfo.Background,
                BitmapHelper.DarkenImage(desktopInfo.Background, 0.5f)
            };

            Control[] controls = new Control[] 
            {
                freezeScreen,
                blackoutScreen
            };

            DrawingContext = DrawingContext.Create(images, controls, isClean: true);

            ConfigureForm(freezeScreen, desktopInfo);
            ConfigureForm(blackoutScreen, desktopInfo);

            freezeScreen.Invalidate();
            freezeScreen.Show();

            blackoutScreen.Invalidate();
            blackoutScreen.Show();

            isHidden = false;
        }

        private Bitmap GetScreenshot(Rectangle rect)
        {
            return BitmapHelper.Crop((Bitmap)freezeScreen.BackgroundImage, rect);
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

        public void WndProc(ref Message m)
        {
            hotKeysHelper.WndProc(ref m);
        }
    }
}
    