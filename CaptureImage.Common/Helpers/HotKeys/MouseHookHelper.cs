using CaptureImage.Common.Helpers.HotKeys.WinApi;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static CaptureImage.Common.Helpers.HotKeys.WinApi.WinAPI;

namespace CaptureImage.Common.Helpers.HotKeys
{
    public class MouseHookHelper : IDisposable
    {
        private IntPtr _hookID = IntPtr.Zero;

        public MouseHookHelper() 
        {
            _hookID = SetWindowsHookEx(WH_MOUSE_LL, HookCallback, IntPtr.Zero, 0);
        }

        private  IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WinAPI.WM_MOUSEWHEEL)
            {
                MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

                int delta = (short)((hookStruct.mouseData >> 16) & 0xffff);
                if (delta > 0)
                {
                    Debug.WriteLine("Колесо мыши прокручено вверх");
                }
                else
                {
                    Debug.WriteLine("Колесо мыши прокручено вниз");
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(_hookID);
        }

    }
}
