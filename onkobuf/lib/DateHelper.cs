using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace onkobuf.lib {
    static class DateHelper {
        static DateTime today = DateTime.Today;

        /// <summary>
        /// Validate XML node of a dictionary record
        /// </summary>
        /// <param name="node">Dictionary record's XML node</param>
        /// <returns>True if the records has valida dates</returns>
        public static bool ValidNode(XmlNode node) {
            string stFrom = node.SelectSingleNode("DATEBEG").InnerText;
            string stTill = node.SelectSingleNode("DATEEND").InnerText;

            DateTime from = today;
            DateTime till = today;

            if (!string.IsNullOrEmpty(stFrom))
                DateTime.TryParseExact(stFrom, "dd.MM.yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out from);

            if (!string.IsNullOrEmpty(stTill))
                DateTime.TryParseExact(stTill, "dd.MM.yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out till);

            return today >= from && today <= till;
        }
    }
}
