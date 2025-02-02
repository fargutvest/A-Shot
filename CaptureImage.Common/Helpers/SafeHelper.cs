using System;
using System.Diagnostics;

namespace CaptureImage.Common.Helpers
{
    internal static class SafeHelper
    {
        internal static void OnSafe(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
