using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Lib {
    class UniqueNumber {
        /// <summary>
        /// Check person's unique number (ENP)
        /// </summary>
        /// <param name="enp">Unique number</param>
        /// <returns>True if the value is a valid unique number</returns>
        public static bool Valid(string enp) {
            if (string.IsNullOrEmpty(enp)) return false;
            char[] cs = enp.Where(c => char.IsDigit(c)).ToArray();
            if (cs.Length != 16) return false;
            return cs.Any(c => c != '0');
        }
    }
}
