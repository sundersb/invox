using System;
using System.Linq;

namespace civox.Lib {
    /// <summary>
    /// Person's unique number (ENP) validator
    /// </summary>
    class UniqueNumber {
        /// <summary>
        /// Check person's unique number (ENP)
        /// </summary>
        /// <param name="enp">Unique number</param>
        /// <returns>True if the value is a valid unique number</returns>
        public static bool Valid(string enp) {
            if (string.IsNullOrEmpty(enp)) return false;
            char[] cs = enp.Where(c => c != ' ').ToArray();
            if (cs.Length != 16) return false;
            // All are digits and whole unique number is not zero
            return cs.All(c => { return Char.IsDigit(c); }) && !cs.All(c => { return c == '0'; });
        }
    }
}
