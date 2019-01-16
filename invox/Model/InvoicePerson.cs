using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Model {
    enum Disability : int {
        NA = -1,
        None = 0,
        Group1,
        Group2,
        Group3,
        Child
    }

    /// <summary>
    /// Сведения о пациенте (ZL_LIST/ZAP/PACIENT)
    /// <remarks>
    /// Вложен в
    ///     InvoiceRecord ZAP (Single)
    /// Содержит
    /// </remarks>
    /// D1, D2, D3 - OK
    /// </summary>
    class InvoicePerson {
        /// <summary>
        /// Код записи о пациенте	Возможно использование уникального идентификатора (учетного кода) пациента.
        /// Необходим для связи с файлом персональных данных.
        /// </summary>
        public string Identity;

        /// <summary>
        /// Тип документа, подтверждающего факт страхования по ОМС
        /// Заполняется в соответствии с F008 Приложения А.
        /// </summary>
        public int PolicyType;

        /// <summary>
        /// Серия документа, подтверждающего факт страхования по ОМС
        /// </summary>
        public string PolicySerial;

        /// <summary>
        /// Номер документа, подтверждающего факт страхования по ОМС
        /// Для полисов единого образца указывается ЕНП
        /// </summary>
        public string PolicyNumber;
        
        /// <summary>
        /// Регион страхования
        /// Указывается ОКАТО территории выдачи ДПФС для полисов старого образца при наличии данных
        /// </summary>
        public string AssuranceOkato;
        
        /// <summary>
        /// Реестровый номер СМО.
        /// Заполняется в соответствии со справочником F002 Приложения А. При отсутствии сведений может не заполняться.
        /// </summary>
        public string SmoCode;
        
        /// <summary>
        /// ОГРН СМО
        /// Заполняются при невозможности указать реестровый номер СМО.
        /// </summary>
        public string SmoOgrn;

        /// <summary>
        /// ОКАТО территории страхования
        /// </summary>
        public string SmoOkato;

        /// <summary>
        /// Наименование СМО
        /// Заполняется при невозможности указать ни реестровый номер, ни ОГРН СМО.
        /// </summary>
        public string SmoName;
        
        /// <summary>
        /// Группа инвалидности
        /// 0 - нет инвалидности;
        /// 1 - 1 группа;
        /// 2 - 2 группа;
        /// 3 - 3 группа;
        /// 4 - дети-инвалиды.
        /// Заполняется только при впервые установленной инвалидности (1 - 4) или в случае отказа в признании лица инвалидом (0).
        /// </summary>
        public Disability Disability;

        /// <summary>
        /// Направление на МСЭ
        /// Указывается "1" в случае передачи направления на МСЭ медицинской организацией в бюро медико-социальной экспертизы.
        /// </summary>
        public bool DirectedToSE;

        /// <summary>
        /// Признак новорожденного
        /// Указывается в случае оказания медицинской помощи ребенку до государственной регистрации рождения.
        /// 0 - признак отсутствует. Если значение признака отлично от нуля, он заполняется по следующему шаблону: ПДДММГГН, где
        /// П - пол ребенка в соответствии с классификатором V005 Приложения А;
        /// ДД - день рождения;
        /// ММ - месяц рождения;
        /// ГГ - последние две цифры года рождения;
        /// Н - порядковый номер ребенка (до двух знаков).
        /// </summary>
        public string NewbornCode;

        /// <summary>
        /// Вес при рождении
        /// Указывается при оказании медицинской помощи недоношенным и маловесным детям. Поле заполняется, если в качестве пациента указан ребенок.
        /// </summary>
        public int NewbornWeight;

        /// <summary>
        /// Полис пациента
        /// </summary>
        public string Policy {
            get {
                if (string.IsNullOrEmpty(PolicySerial))
                    return PolicyNumber;
                else
                    return PolicySerial + " " + PolicyNumber;
            }
            set { SetPolicy(value); }
        }

        /// <summary>
        /// Save person data to invoice XML
        /// </summary>
        /// <param name="xml">XML exporter to write to</param>
        /// <param name="section">Order #59 section</param>
        public void Write(Lib.XmlExporter xml, OrderSection section) {
            switch(section) {
                case OrderSection.D1:
                    WriteD1(xml);
                    break;
                case OrderSection.D2:
                    WriteD2(xml);
                    break;
                case OrderSection.D3:
                    WriteD3(xml);
                    break;
            }
        }

        /// <summary>
        /// Save person data to treatment invoice
        /// </summary>
        /// <param name="xml">XML exporter to use</param>
        public void WriteD1(Lib.XmlExporter xml) {
            xml.Writer.WriteStartElement("PACIENT");

            xml.Writer.WriteElementString("ID_PAC", Identity);

            xml.Writer.WriteElementString("VPOLIS", PolicyType.ToString());
            xml.WriteIfValid("SPOLIS", PolicySerial);
            xml.WriteIfValid("NPOLIS", PolicyNumber);

            xml.WriteIfValid("ST_OKATO", AssuranceOkato);
            xml.WriteIfValid("SMO", SmoCode);
            xml.WriteIfValid("SMO_OGRN", SmoOgrn);
            xml.WriteIfValid("SMO_OK", SmoOkato);
            xml.WriteIfValid("SMO_NAM", SmoName);

            if (Disability != Disability.NA)
                xml.Writer.WriteElementString("INV", ((int)Disability).ToString());

            if (DirectedToSE)
                xml.Writer.WriteElementString("MSE", "1");

            xml.Writer.WriteElementString("NOVOR", NewbornCode); 
            if (NewbornCode != "0") {
                xml.Writer.WriteElementString("VNOV_D", NewbornWeight.ToString("D4"));
            }

            xml.Writer.WriteEndElement();
        }

        /// <summary>
        /// Save person data to hi-tech invoice
        /// </summary>
        /// <param name="xml">XML exporter to use</param>
        public void WriteD2(Lib.XmlExporter xml) {
            xml.Writer.WriteStartElement("PACIENT");

            xml.Writer.WriteElementString("ID_PAC", Identity);

            xml.Writer.WriteElementString("VPOLIS", PolicyType.ToString());
            xml.WriteIfValid("SPOLIS", PolicySerial);
            xml.WriteIfValid("NPOLIS", PolicyNumber);

            xml.WriteIfValid("ST_OKATO", AssuranceOkato);
            xml.WriteIfValid("SMO", SmoCode);
            xml.WriteIfValid("SMO_OGRN", SmoOgrn);
            xml.WriteIfValid("SMO_OK", SmoOkato);
            xml.WriteIfValid("SMO_NAM", SmoName);

            if (DirectedToSE)
                xml.Writer.WriteElementString("MSE", "1");

            xml.Writer.WriteElementString("NOVOR", NewbornCode);
            if (NewbornCode != "0") {
                xml.Writer.WriteElementString("VNOV_D", NewbornWeight.ToString("D4"));
            }

            xml.Writer.WriteEndElement();
        }

        /// <summary>
        /// Save person data to dispanserisation invoice
        /// </summary>
        /// <param name="xml">XML exporter to use</param>
        public void WriteD3(Lib.XmlExporter xml) {
            xml.Writer.WriteStartElement("PACIENT");

            xml.Writer.WriteElementString("ID_PAC", Identity);

            xml.Writer.WriteElementString("VPOLIS", PolicyType.ToString());
            xml.WriteIfValid("SPOLIS", PolicySerial);
            xml.WriteIfValid("NPOLIS", PolicyNumber);

            xml.WriteIfValid("ST_OKATO", AssuranceOkato);
            xml.WriteIfValid("SMO", SmoCode);
            xml.WriteIfValid("SMO_OGRN", SmoOgrn);
            xml.WriteIfValid("SMO_OK", SmoOkato);
            xml.WriteIfValid("SMO_NAM", SmoName);
            xml.Writer.WriteElementString("NOVOR", NewbornCode);

            xml.Writer.WriteEndElement();
        }

        public void WriteD4(Lib.XmlExporter xml) {
            xml.Writer.WriteStartElement("PACIENT");

            xml.Writer.WriteElementString("ID_PAC", Identity);

            xml.Writer.WriteElementString("VPOLIS", PolicyType.ToString());
            xml.WriteIfValid("SPOLIS", PolicySerial);
            xml.WriteIfValid("NPOLIS", PolicyNumber);

            xml.WriteIfValid("ST_OKATO", AssuranceOkato);
            xml.WriteIfValid("SMO", SmoCode);
            xml.WriteIfValid("SMO_OGRN", SmoOgrn);
            xml.WriteIfValid("SMO_OK", SmoOkato);
            xml.WriteIfValid("SMO_NAM", SmoName);
            xml.Writer.WriteElementString("NOVOR", NewbornCode);

            xml.Writer.WriteEndElement();
        }
       
        void SetPolicy(string value) {
            if (Lib.UniqueNumber.Valid(value)) {
                PolicySerial = string.Empty;
                PolicyNumber = new string(value.Where(c => c != ' ').ToArray());
            } else {
                string[] parts = value.Split(' ');
                if (parts.Length > 1) {
                    PolicySerial = parts[0];
                    PolicyNumber = string.Join(string.Empty, parts.Skip(1));
                } else {
                    PolicySerial = string.Empty;
                    PolicyNumber = string.Join(string.Empty, parts);
                }
            }
        }
    }
}
