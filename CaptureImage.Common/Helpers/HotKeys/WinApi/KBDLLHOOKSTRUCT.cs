using System;
using System.Runtime.InteropServices;

namespace CaptureImage.Common.Helpers.HotKeys.WinApi
{
    /// <summary>
    /// Структура для перехвата клавиш
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}
