using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
namespace CaptureImage.WinForms.Helpers
{
    internal static class FileNameHelper
    {
        internal static string SuggestFileName(string directory)
        {
            string suggestedFileName = string.Empty;

            if (string.IsNullOrEmpty(directory) == false)
            {
                string prefix = "Screenshot_";
                Regex regex = new Regex($"{prefix}.*.*[A-z]");

                string[] files = Directory.GetFiles(directory).Select(f => Path.GetFileName(f))
                    .Where(f => regex.IsMatch(f) && (f.EndsWith(".png") || f.EndsWith(".jpeg") || f.EndsWith(".bmp"))).ToArray();

                string[] extensions = new string[files.Length];
                int[] numbers = new int[files.Length];

                for (int i = 0; i < files.Length; i++)
                {
                    string ext = Path.GetExtension(files[i]);
                    extensions[i] = ext;

                    if (int.TryParse(files[i].Replace(prefix, string.Empty).Replace(ext, string.Empty), out int num))
                        numbers[i] = num;
                    else
                        numbers[i] = -1;
                }

                if (numbers.Any(n => n > 0))
                {
                    int index = numbers.ToList().IndexOf(numbers.Max());

                    suggestedFileName = $"{prefix}{numbers.Max() + 1}{extensions[index]}";
                }
                else
                {
                    suggestedFileName = $"{prefix}1.png";
                }
            }

            return suggestedFileName;
        }
    }
}
