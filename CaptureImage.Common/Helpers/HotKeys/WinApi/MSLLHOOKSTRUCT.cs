using System;

namespace CaptureImage.Common.Helpers.HotKeys.WinApi
{
    /// <summary>
    /// Структура для информации о событиях мыши
    /// </summary>
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public int mouseData;
        public int flags;
        public int time;
        public IntPtr dwExtraInfo;
    }
}
