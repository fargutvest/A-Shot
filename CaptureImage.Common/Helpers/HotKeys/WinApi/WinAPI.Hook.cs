using System;
using System.Runtime.InteropServices;

namespace CaptureImage.Common.Helpers.HotKeys.WinApi
{
    public class WinAPI
    {
        #region Декларации Windows API

        /// <summary>
        /// Хук клавиатуры
        /// </summary>
        public const int WH_KEYBOARD_LL = 13; 
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;

        #endregion

        #region Windows API вызовы

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnhookWindowsHookEx(IntPtr hHook);

        [DllImport("user32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        #endregion

        /// <summary>
        /// Делегат для обработки перехвата клавиш
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    }
}
