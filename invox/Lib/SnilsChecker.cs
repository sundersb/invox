using System;
using System.Linq;
using System.Text;

namespace invox.Lib {
    static class SnilsChecker {
        /// <summary>
        /// SNILS validity check
        /// </summary>
        /// <param name="snils">SNILS</param>
        /// <returns>True if SNILS control sum is valid and not all-zeros</returns>
        public static bool Valid(string snils) {
            if (string.IsNullOrEmpty(snils)) return false;

            char[] chars = snils.ToCharArray().Where(x => Char.IsDigit(x)).ToArray();

            if (chars.Length != 11) return false;

            if (chars.All(c => c == '0'))
                return false;

            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (chars[i] - '0') * (9 - i);

            sum %= 101;
            if (sum > 99) sum = 0;

            return int.Parse(new string(chars, 9, 2)) == sum;
        }

        /// <summary>
        /// Return SNILS without spaces and hyphens
        /// </summary>
        /// <param name="snils">SNILS</param>
        /// <returns>SNILS without spaces and hyphens</returns>
        public static string DigitsOnly(string snils) {
            return new string(snils.ToCharArray().Where(x => Char.IsDigit(x)).ToArray());
        }

        /// <summary>
        /// Rebuild SNILS from digits-only to normal format
        /// </summary>
        /// <param name="snils">SNILS</param>
        public static string Rebuild(string snils) {
            if (string.IsNullOrEmpty(snils)) return string.Empty;

            char[] chars = snils.ToCharArray().Where(x => Char.IsDigit(x)).ToArray();
            if (chars.Length != 11) return string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append(chars[0]);
            sb.Append(chars[1]);
            sb.Append(chars[2]);
            sb.Append(chars.Take(3).ToArray());
            sb.Append('-');
            sb.Append(chars[3]);
            sb.Append(chars[4]);
            sb.Append(chars[5]);
            sb.Append('-');
            sb.Append(chars[6]);
            sb.Append(chars[7]);
            sb.Append(chars[8]);
            sb.Append(' ');
            sb.Append(chars[9]);
            sb.Append(chars[10]);

            return sb.ToString();
        }
    }
}
