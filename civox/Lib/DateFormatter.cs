using System;

namespace civox.Lib {
    static class DateFormatter {
        /// <summary>
        /// Helper for date formatting
        /// </summary>
        /// <param name="date">Date to be converted to string</param>
        /// <returns>Date formatted in conformance to order 79</returns>
        public static string AsXml(this DateTime date) {
            return string.Format("{0:yyyy-MM-dd}", date);
        }
    }
}
