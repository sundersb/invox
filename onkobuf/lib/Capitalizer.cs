using System.Linq;

namespace onkobuf.lib {
    public static class Capitalizer {
        public static string Capitalized(this string value) {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return char.ToUpper(value.First()) + value.Substring(1).ToLower();
        }
    }
}
