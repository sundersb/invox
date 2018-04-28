using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Lib
{
    /// <summary>
    /// SNILS validity checker
    /// </summary>
    public static class SnilsChecker
    {
        /// <summary>
        /// SNILS validity check
        /// </summary>
        /// <param name="snils">SNILS</param>
        /// <returns>True if SNILS control sum is valid and not all-zeros</returns>
        public static bool Valid(string snils)
        {
            if (string.IsNullOrEmpty(snils)) return false;

            char[] chars = snils.ToCharArray().Where(x => Char.IsDigit(x)).ToArray();

            if (chars.All(c => c == '0'))
                return false;

            if (chars.Length != 11) return false;

            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (chars[i] - '0') * (9 - i);

            sum %= 101;
            if (sum > 99) sum = 0;

            return int.Parse(new string(chars, 9, 2)) == sum;
        }
    }
}
