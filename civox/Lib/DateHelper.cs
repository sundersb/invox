using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace civox.Lib {
    static class DateHelper {
        const string XML_HOLIDAYS = "\\Lib\\holidays.xml";
        const string XML_WORKDAYS = "\\Lib\\workdays.xml";

        static List<DateTime> holidays = null;
        static object hlock = new object();

        static List<DateTime> workdays = null;
        static object wlock = new object();

        static List<DateTime> Holidays {
            get {
                if (holidays == null) lock (hlock) {
                    if (holidays == null) {
                        holidays = new List<DateTime>();
                        LoadDates(holidays, XML_HOLIDAYS);
                    }
                }
                return holidays;
            }
        }

        static List<DateTime> Workdays {
            get {
                if (workdays == null) lock (hlock) {
                    if (workdays == null) {
                        workdays = new List<DateTime>();
                        LoadDates(workdays, XML_WORKDAYS);
                    }
                }
                return holidays;
            }
        }

        /// <summary>
        /// Helper for date formatting
        /// </summary>
        /// <param name="date">Date to be converted to string</param>
        /// <returns>Date formatted in conformance to order 79</returns>
        public static string AsXml(this DateTime date) {
            return string.Format("{0:yyyy-MM-dd}", date);
        }

        public static DateTime Parse(string value) {
            return DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns date n working days backwards from the given one
        /// </summary>
        /// <param name="date">Date to count from</param>
        /// <param name="n">Working days to count backwards</param>
        public static DateTime WorkingDaysBefore(this DateTime date, int n) {
            date = date.Date;
            while (n > 0) {
                date = date.AddDays(-1);
                if (IsWorkDay(date)) --n;
            }
            return date;
        }

        static bool IsWorkDay(DateTime date) {
            DayOfWeek dw = date.DayOfWeek;
            return ((dw != DayOfWeek.Saturday && dw != DayOfWeek.Sunday) || Workdays.Contains(date)) && !Holidays.Contains(date);
        }

        static void LoadDates(List<DateTime> days, string fName) {
            string fileName = Options.BaseDirectory + fName;

            if (!File.Exists(fileName)) return;

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (!fs.CanRead) return;

            XDocument x = XDocument.Load(fs);
            if (x.Root.Name != "dates") return;

            foreach (var n in x.Root.Elements()) {
                if (n.Name != "date") return;
                days.Add(Parse(n.Value));
            }
        }
    }
}
