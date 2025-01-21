using CaptureImage.Common;
using CaptureImage.Common.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;
using CaptureImage.WinForms.Properties;
using System.IO;
using CaptureImage.WinForms.Helpers;
using CaptureImage.Common.DrawingContext;
using System.Collections.Generic;

namespace CaptureImage.WinForms
{
    public class AppContext : ApplicationContext, IDrawingContextProvider
    {
        private NotifyIcon trayIcon;
        private HotKeysHelper hotKeysHelper;
        private FreezeScreen freezeScreen;
        private BlackoutScreen blackoutScreen;
        private bool isHidden;

        public DrawingContextKeeper DrawingContextKeeper { get; private set; }

        public event EventHandler Undo;


        public AppContext()
        {
            isHidden = true;
            DrawingContextKeeper = new DrawingContextKeeper();

            MainForm = new MainForm(this);

            hotKeysHelper = new HotKeysHelper();
            hotKeysHelper.RegisterHotKey(MainForm.Handle, Keys.F6, ShowForm);
            hotKeysHelper.RegisterHotKey(MainForm.Handle, Keys.Escape, OnEscape);

            trayIcon = new NotifyIcon()
            {
                Icon = Resources.ashot,
                Visible = true,
                ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Сделать скриншот", Show),
                    new MenuItem("Выход", Exit)}),
            };

            freezeScreen = new FreezeScreen(this);
            blackoutScreen = new BlackoutScreen(this);
            blackoutScreen.TransparencyKey = Color.Red;
            blackoutScreen.AllowTransparency = true;
        }

        public void UndoDrawing()
        {
            Undo?.Invoke(this, EventArgs.Empty);
            //DrawingContextKeeper.RevertToPreviousContext();
        }

        private void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void Show(object sender, EventArgs e)
        {
            ShowForm();
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
                    HideForm();
            }
        }

        public void HideForm()
        {
            freezeScreen.Hide();
            blackoutScreen.Hide();
            isHidden = true;
        }

        private void ShowForm()
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

         

            DrawingContext drawingContext = DrawingContext.Create(images, controls, isClean: true);
            DrawingContextKeeper.SetDrawingContext(drawingContext);

            DrawingContextKeeper.SaveContext();

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
            {
                DrawingContextKeeper.DrawingContext.SetColorOfPen(colorDialog.Color);
            }
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
    