using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Lib {
    /// <summary>
    /// Поставщик имен для XML файлов счета
    /// </summary>
    class InvoiceFilename {
        string personFile;
        string invoiceFile;
        string smoCode;
        string clinicCode;
        string fundCode;
        int code;
        int year;
        int month;
        Model.OrderSection section;

        /// <summary>
        /// Имя файла пациентов (без каталога)
        /// </summary>
        public string PersonFile { get { return personFile; } }

        /// <summary>
        /// Имя файла счетов (без каталога)
        /// </summary>
        public string InvoiceFile { get { return invoiceFile; } }

        /// <summary>
        /// Код фонда ОМС
        /// </summary>
        public string TerritoryFundCode { get { return fundCode; } }

        /// <summary>
        /// Код МО
        /// </summary>
        public string ClinicCode { get { return clinicCode; } }

        /// <summary>
        /// Код СМО
        /// </summary>
        public string CompanyCode { get { return smoCode; } }

        /// <summary>
        /// Раздел приложения к приказу 59
        /// </summary>
        public Model.OrderSection Section { get { return section; } }

        /// <summary>
        /// True - счет для ФОМС, false - счет для СМО
        /// </summary>
        public bool DirectedToFund { get { return !string.IsNullOrEmpty(fundCode); } }

        /// <summary>
        /// Уникальный код счета
        /// </summary>
        public int Code { get { return code; } }

        /// <summary>
        /// Отчетный год
        /// </summary>
        public int Year { get { return year; } }

        /// <summary>
        /// Отчетный месяц
        /// </summary>
        public int Month { get { return month; } }

        InvoiceFilename(string personFilename, string invoiceFilename) {
            personFile = personFilename;
            invoiceFile = invoiceFilename;
        }

        /// <summary>
        /// Получить поставщик имен для счета в ФОМС
        /// </summary>
        /// <param name="lpuCode">Код МО</param>
        /// <param name="territoryFundCode">Код ФОМС</param>
        /// <param name="year">Год выгрузки</param>
        /// <param name="month">Месяц выгрузки</param>
        /// <param name="packetNumber">Номер пакет</param>
        /// <param name="orderSection">Приложение к приказу 59</param>
        public static InvoiceFilename ToAssuranceFund(string lpuCode,
            string territoryFundCode,
            int year,
            int month,
            int packetNumber,
            Model.OrderSection orderSection) {

            if (string.IsNullOrEmpty(territoryFundCode)) return null;

            StringBuilder sb = new StringBuilder();

            // Source
            sb.Append('M');
            sb.Append(lpuCode);

            // Destination
            sb.Append('T');
            sb.Append(territoryFundCode);

            sb.Append('_');
            
            // Terms
            sb.Append(year % 100);
            sb.Append(month.ToString("D2"));

            // Packet
            sb.Append(packetNumber % 10);

            string bulk = sb.ToString();
            string invoice = null;
            string persons = null;

            switch (orderSection) {
                case Model.OrderSection.D1:
                    invoice = "H" + bulk;
                    persons = "L" + bulk;
                    break;
                case Model.OrderSection.D2:
                    invoice = "T" + bulk;
                    persons = "LT" + bulk;
                    break;
                case Model.OrderSection.D3:
                    // TODO: GetProfKindCode(profKind), so far default - adults' prof exams
                    bulk = 'O' + bulk;
                    invoice = "D" + bulk;
                    persons = "L" + bulk;
                    break;
                case Model.OrderSection.D4:
                    invoice = "C" + bulk;
                    persons = "LC" + bulk;
                    break;
            }

            return new InvoiceFilename(persons, invoice) {
                clinicCode = lpuCode,
                fundCode = territoryFundCode,
                section = orderSection,
                year = year,
                month = month,
                code = (year % 100) * 10000 + month * 100 + (packetNumber % 100)
            };
        }


        public static InvoiceFilename ToAssuranceCompany(string lpuCode,
            string assuranceCompanyCode,
            int year,
            int month,
            int packetNumber,
            Model.OrderSection orderSection) {

            StringBuilder sb = new StringBuilder();

            // Source
            sb.Append('M');
            sb.Append(lpuCode);

            // Destination
            sb.Append('S');
            sb.Append(assuranceCompanyCode);

            sb.Append('_');
            sb.Append(year % 100);
            sb.Append(month.ToString("D2"));
            sb.Append(packetNumber % 10);

            string bulk = sb.ToString();
            string invoice = null;
            string persons = null;

            switch (orderSection) {
                case Model.OrderSection.D1:
                    invoice = "H" + bulk;
                    persons = "L" + bulk;
                    break;
                case Model.OrderSection.D2:
                    invoice = "T" + bulk;
                    persons = "LT" + bulk;
                    break;
                case Model.OrderSection.D3:
                    // TODO: GetProfKindCode(profKind), so far default - adults' prof exams
                    bulk = 'O' + bulk;
                    invoice = "D" + bulk;
                    persons = "L" + bulk;
                    break;
                case Model.OrderSection.D4:
                    invoice = "C" + bulk;
                    persons = "LC" + bulk;
                    break;
            }

            return new InvoiceFilename(persons, invoice) {
                clinicCode = lpuCode,
                smoCode = assuranceCompanyCode,
                section = orderSection,
                year = year,
                month = month,
                code = (year % 100) * 10000 + month * 100 + (packetNumber % 100)
            };
        }
    }
}
