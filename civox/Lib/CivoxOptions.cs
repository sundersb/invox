using System;

namespace civox.Lib {
    /// <summary>
    /// Command line options for the application
    /// </summary>
    class CivoxOptions {
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
        public string PackageNumber { get; set; }

        [CommandLineOptionAttribute("i", "invoice-number", "Номер счета")]
        public string InvoiceNumber { get; set; }

        [CommandLineOptionAttribute("d", "date", "Дата счета (yyyy-mm-dd)")]
        public DateTime InvoiceDate { get; set; }

        [CommandLineOptionAttribute("h", "help", "Показать справку по программе", IsHelp = true)]
        public bool ShowHelp { get; set; }

        [ErrorMessageAttribute]
        public string Error { get; set; }

        public CivoxOptions() {
            DateTime date = DateTime.Today.AddMonths(-1);
            Year = date.Year;
            Month = date.Month;
            InvoiceNumber = "1";
            ShowHelp = false;
            Error = string.Empty;

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
