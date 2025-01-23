using System;
using System.Windows.Forms;

namespace CaptureImage.Common.Helpers.HotKeys
{
    public class HotKey : IEquatable<HotKey>
    {
        public Keys Key { get; set; }
        public Keys ModifierKey { get; set; }

        public bool Equals(HotKey other)
        {
            return Key == other.Key && ModifierKey == other.ModifierKey;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            HotKey other = obj as HotKey;
            if (other == null)
                return false;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode() ^ ModifierKey.GetHashCode();
        }

        public override string ToString()
        {
            return $"{ModifierKey.ToString()} {Key}";
        }
    }
}
