using Microsoft.Win32;
using System;

namespace CaptureImage.WinForms.Helpers
{
    internal static class RegistryHelper
    {
        internal static string GetLastSaveDirectory()
        {
            string registryKeyPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU";

            try
            {
                // Чтение из реестра (например, для OpenFileDialog)
                string[] recentDirectories = (string[])Registry.GetValue(registryKeyPath, "a", null);
                if (recentDirectories != null && recentDirectories.Length > 0)
                {
                    // Возвращаем последний путь
                    return recentDirectories[0]; // Первый элемент — это последний использованный путь
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении реестра: {ex.Message}");
            }

            return null;
        }
    }
}
