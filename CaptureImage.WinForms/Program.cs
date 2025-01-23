using System;
using System.Threading;
using System.Windows.Forms;

namespace CaptureImage.WinForms
{
    internal static class Program
    {
        private static string appGuid = "ae14e86b-54fc-4340-9233-017c216803dd";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, "Global\\" + appGuid))
            {
                if (mutex.WaitOne(0, false) == false)
                    return;
           
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new AppContext());
            }
        }
    }
}
