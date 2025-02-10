using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CaptureImage.Common.Helpers.HotKeys.WinApi;
using static CaptureImage.Common.Helpers.HotKeys.WinApi.WinAPI;

namespace CaptureImage.Common.Helpers.HotKeys
{
    public class HotKeysHelper : IDisposable
    {
        private LowLevelKeyboardProc _keyboardProc;
        private IntPtr _hookID = IntPtr.Zero;
        private Dictionary<HotKey, Action> hotKeyDict = new Dictionary<HotKey, Action>();


        public event EventHandler<char> KeyPress;

        public HotKeysHelper()
        {
            _keyboardProc = HookCallback;
            _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardProc, IntPtr.Zero, 0);
        }

        public void RegisterHotKey(Keys modifierKey, Keys key, Action action)
        {
            HotKey hotkey = new HotKey()
            {
                ModifierKey = modifierKey,
                Key = key,
            };

            hotKeyDict.Add(hotkey, action);
        }

        public void RegisterHotKey(Keys key, Action action)
        {
            HotKey hotkey = new HotKey()
            {
                ModifierKey = Keys.None,
                Key = key,
            };

            hotKeyDict.Add(hotkey, action);
        }

        public void RegisterHotKey(HotKey hotKey, Action action)
        {
            hotKeyDict.Add(hotKey, action);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                Keys key = (Keys)lParam.vkCode;

                char keyChar = GetCharFromVirtualKey(lParam.vkCode);

                KeyPress?.Invoke(this, keyChar);

#if DEBUG

                Debug.WriteLine($"{key.ToString()} - {keyChar}");

#endif

                HotKey pressedHotKey = new HotKey()
                {
                    ModifierKey = Control.ModifierKeys,
                    Key = key,
                };

                if (hotKeyDict.Keys.Contains(pressedHotKey))
                    hotKeyDict[pressedHotKey]?.Invoke();
            }

            IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(lParam));
            Marshal.StructureToPtr(lParam, pnt, false);

            return CallNextHookEx(_hookID, nCode, wParam, pnt);
        }
        
        private char GetCharFromVirtualKey(uint vkCode)
        {
            uint charCode = MapVirtualKey(vkCode, MAPVK_VK_TO_CHAR);
            
            if (charCode != 0)
            {
                return (char)charCode;
            }
            
            return '\0';
        }


        public void Dispose()
        {
            UnhookWindowsHookEx(_hookID);
        }
    }
}
