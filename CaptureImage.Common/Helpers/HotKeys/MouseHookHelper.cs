using CaptureImage.Common.Helpers.HotKeys.WinApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static CaptureImage.Common.Helpers.HotKeys.WinApi.WinAPI;

namespace CaptureImage.Common.Helpers.HotKeys
{
    public class MouseHookHelper : IDisposable
    {
        private LowLevelMouseProc _mouseProc;
        private IntPtr _hookID = IntPtr.Zero;
        public event EventHandler<int> MouseWheel;

        public MouseHookHelper() 
        {
            _mouseProc = HookCallback;
            _hookID = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, IntPtr.Zero, 0);
        }

        private  IntPtr HookCallback(int nCode, IntPtr wParam, ref MSLLHOOKSTRUCT lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WinAPI.WM_MOUSEWHEEL)
            {
                int delta = (short)((lParam.mouseData >> 16) & 0xffff);
                if (delta > 0)
                {
                    MouseWheel?.Invoke(this, 1);
                    Debug.WriteLine("Колесо мыши прокручено вверх");
                }
                else
                {
                    MouseWheel?.Invoke(this, -1);
                    Debug.WriteLine("Колесо мыши прокручено вниз");
                }
            }


            IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(lParam));
            Marshal.StructureToPtr(lParam, pnt, false);

            return CallNextHookEx(_hookID, nCode, wParam, pnt);
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(_hookID);
        }

    }
}
