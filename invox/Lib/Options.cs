using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Lib {
    class Options {
        [CommandLineOptionAttribute("y", "year", "Год выгружаемого периода")]
        public int Year { get; set; }

        [CommandLineOptionAttribute("m", "month", "Месяц выгружаемого периода")]
        public int Month { get; set; }

        [CommandLineOptionAttribute("p", "period", "Год и месяц выгружаемого периода (yyyymm)")]
        public string Period {
            get {
                return string.Format("{0:d4}{1:d2}", Year, Month);
            }
            set {
                SetPeriod(value);
            }
        }

        [CommandLineOptionAttribute("n", "packet-number", "Порядковый номер пакета в данном периоде")]
        public int PackageNumber { get; set; }

        [CommandLineOptionAttribute("f", "files", "Выгружаемые счета (разделы приложения Д приказа 79)")]
        public string Sections { get; set; }

        [CommandLineOptionAttribute("i", "invoice-number", "Номер счета")]
        public string InvoiceNumber { get; set; }

        [CommandLineOptionAttribute("r", "reading", "Путь к файлу с текстом комментариев (UTF-8)")]
        public string Reading { get; set; }

        [CommandLineOptionAttribute("d", "date", "Дата счета (yyyy-mm-dd)")]
        public DateTime InvoiceDate { get; set; }

        [CommandLineOptionAttribute("h", "help", "Показать справку по программе", IsHelp = true)]
        public bool ShowHelp { get; set; }

        [CommandLineOptionAttribute("l", "leave-files", "Не удалять файлы счетов после упаковки")]
        public bool LeaveFiles { get; set; }

        [ErrorMessageAttribute]
        public string Error { get; set; }

        public Options() {
            DateTime date = DateTime.Today.AddMonths(-1);
            Year = date.Year;
            Month = date.Month;
            InvoiceNumber = "1";
            PackageNumber = 1;
            ShowHelp = false;
            Error = string.Empty;
            LeaveFiles = false;

            // Get last working day of the report month
            InvoiceDate = new DateTime(Year, Month, 1);
            InvoiceDate = InvoiceDate.AddMonths(1).AddDays(-1);
            while (!InvoiceDate.IsWorkDay()) InvoiceDate = InvoiceDate.AddDays(-1);
        }

        void SetPeriod(string period) {
            if (period.Length == 6) {
                string y = period.Substring(0, period.Length - 2);
                int yy, mm;
                if (int.TryParse(y, out yy) && yy > 2000 && yy < 2100) {
                    Year = yy;
                    period = period.Substring(4);
                    if (int.TryParse(period, out mm) && mm > 0 && mm < 13) {
                        Month = mm;
                        // Parsed OK
                        return;
                    };
                }
            }
            Error = string.Format("Неверный формат периода: {0}, ожидается 'yyyymm'", period);
            ShowHelp = true;
            return;
        }
    }
}
