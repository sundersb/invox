using System.Linq;

namespace onkobuf.lib {
    public static class Capitalizer {
        /// <summary>
        /// Capitalize string: make first letter large, setting other letters to small
        /// </summary>
        public static string Capitalized(this string value) {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return char.ToUpper(value.First()) + value.Substring(1).ToLower();
        }
    }
}
