using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace invox.Lib {
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
                return workdays;
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

        /// <summary>
        /// Increment days to get work day
        /// </summary>
        /// <param name="date">Date to increment</param>
        /// <param name="days">Increment amount (days). May be negative</param>
        /// <returns>Nearest incremented (decremented) day which is still a workday</returns>
        public static DateTime ShiftDays(this DateTime date, int days) {
            int m = date.Month;
            DateTime result = date;
            
            do {
                result = result.AddDays(days);
                if (result.Month != m) return date;
            } while (!result.IsWorkDay());

            return result;
        }

        /// <summary>
        /// Get DateTime from yyyy-mm-dd string
        /// </summary>
        /// <param name="value">String representation of a DateTime value to read from</param>
        /// <returns>DateTime value read from the string</returns>
        public static DateTime Parse(string value) {
            DateTime result = DateTime.Now;
            try {
                result = DateTime.ParseExact(value,
                    "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture);
            } catch (FormatException ex) {
                Lib.Logger.Log(ex.Message + "\r\nНеверный формат даты: " + value);
            }
            return result;
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
                if (date.IsWorkDay()) --n;
            }
            return date;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static DateTime WorkingDaysBeforeDayStationary (this DateTime date, int n) {
            date = date.Date;
            --n;
            while (n > 0) {
                date = date.AddDays(-1);
                if (date.IsWorkDayNoSaturdays()) --n;
            }
            return date;
        }

        public static bool IsWorkDay(this DateTime date) {
            DayOfWeek dw = date.DayOfWeek;
            return ((dw != DayOfWeek.Saturday && dw != DayOfWeek.Sunday) || Workdays.Contains(date)) && !Holidays.Contains(date);
        }

        public static bool IsWorkDayNoSaturdays(this DateTime date) {
            DayOfWeek dw = date.DayOfWeek;
            return (dw != DayOfWeek.Sunday || Workdays.Contains(date)) && !Holidays.Contains(date);
        }

        static void LoadDates(List<DateTime> days, string fName) {
            string fileName = invox.Options.BaseDirectory + fName;

            if (!File.Exists(fileName)) {
                Lib.Logger.Log("Файл не найден: " + fName);
                return;
            }

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (!fs.CanRead) {
                Lib.Logger.Log("Файл недоступен для чтения: " + fName);
                return;
            }

            XDocument x = XDocument.Load(fs);
            if (x.Root.Name != "dates") {
                Lib.Logger.Log("Неверный формат файла: " + fName);
                return;
            }

            foreach (var n in x.Root.Elements()) {
                if (n.Name != "date") {
                    Lib.Logger.Log(string.Format("Неверный параметр {0} в файле {1}", n.Name, fName));
                    return;
                }
                days.Add(Parse(n.Value));
            }
        }
    }
}
