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

        /// <summary>
        /// Tries to bring leading roman number from I to V to arabic digit
        /// </summary>
        /// <param name="value">Value to transform</param>
        public static string RomanToArabic(this string value) {
            if (string.IsNullOrEmpty(value)) return value;
            if (value.First() == 'V') {
                return '5' + value.Substring(1);
            } else if (value.StartsWith("IV")) {
                return '4' + value.Substring(2);
            } else if (value.StartsWith("III")) {
                return '3' + value.Substring(3);
            } else if (value.StartsWith("II")) {
                return '2' + value.Substring(2);
            } else if (value.First() == 'I') {
                return '1' + value.Substring(1);
            } else return value;
        }

        /// <summary>
        /// Try to bring leading arabic digit to roman
        /// </summary>
        public static string ArabicToRoman(this string value) {
            if (string.IsNullOrEmpty(value)) return value;

            switch (value.First()) {
                case '1': return 'I' + value.Substring(1);
                case '2': return "II" + value.Substring(1);
                case '3': return "III" + value.Substring(1);
                case '4': return "IV" + value.Substring(1);
                case '5': return 'V' + value.Substring(1);
                default: return value;
            }
        }
    }
}
