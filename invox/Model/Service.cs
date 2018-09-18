using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Lib;

namespace invox.Model {
    /// <summary>
    /// Сведения об услуге (ZL_LIST/ZAP/Z_SL/SL/USL)
    /// <remarks>
    /// Вложен в
    ///     Event SL (Multiple)
    /// Содержит
    ///     OncologyDirection NAPR (Multiple)
    ///     OncologyService ONK_USL (Single)
    /// </remarks>
    /// </summary>
    class Service {
        /// <summary>
        /// Неполный объем
        /// </summary>
        public enum IncompleteServiceReason : int {
            None = 0,
            Refusal = 1,          // документированный отказ больного,
            Contraindication = 2, // медицинские противопоказания,
            Other = 3,            // прочие причины (умер, переведен в другое отделение и пр.)
            AlreadyDone = 4       // ранее проведенные услуги в пределах установленных сроков.

        }

        string id;
        string unit;
        string interventionKind;
        bool child;
        DateTime dateFrom;
        DateTime dateTill;
        string diagnosis;
        string serviceCode;
        double quantity;
        double tariff;
        double total;
        string specialityCode;
        string doctorCode;
        IncompleteServiceReason incomplete;
        string comment;

        bool refusal;

        public bool Oncology;

        /// <summary>
        /// Номер записи в реестре услуг
        /// Уникален в пределах случая
        /// </summary>
        public string Identity { get { return id; } }

        /// <summary>
        /// Подразделение МО
        /// Подразделение МО лечения из регионального справочника
        /// </summary>
        public string Unit { get { return unit; } }

        /// <summary>
        /// Код отделения
        /// Отделение МО лечения из регионального справочника
        /// </summary>
        //public string Department { get { return dept; } }

        /// <summary>
        /// Профиль медицинской помощи
        /// Классификатор V002 Приложения А.
        /// </summary>
        // public string ProfileCode { get { return profileCode; } }

        /// <summary>
        /// Вид медицинского вмешательства
        /// Указывается в соответствии с номенклатурой медицинских услуг (V001), в том числе для услуг диализа.
        /// </summary>
        public string InterventionKind { get { return interventionKind; } }

        /// <summary>
        /// Признак детского профиля
        /// 0 - нет, 1 - да.
        /// Заполняется в зависимости от профиля оказанной медицинской помощи.
        /// </summary>
        public bool Child { get { return child; } }

        /// <summary>
        /// Дата начала оказания услуги
        /// </summary>
        public DateTime DateFrom { get { return dateFrom; } }
        
        /// <summary>
        /// Дата окончания оказания услуги
        /// </summary>
        public DateTime DateTill { get { return dateTill; } }

        /// <summary>
        /// Диагноз
        /// Код из справочника МКБ до уровня подрубрики
        /// </summary>
        public string Diagnosis { get { return diagnosis; } }

        /// <summary>
        /// Код услуги
        /// Заполняется в соответствии с территориальным классификатором услуг.
        /// </summary>
        public string ServiceCode { get { return serviceCode; } }

        /// <summary>
        /// Количество услуг (кратность услуги)
        /// </summary>
        public double Quantity { get { return quantity; } }

        /// <summary>
        /// Тариф
        /// </summary>
        public double Tariff { get { return tariff; } }

        /// <summary>
        /// Стоимость медицинской услуги, принятая к оплате (руб.)
        /// Может принимать значение 0
        /// </summary>
        public double Total { get { return total; } }

        /// <summary>
        /// Специальность медработника, выполнившего услугу
        /// Классификатор медицинских специальностей (Приложение А V021). Указывается значение IDSPEC
        /// </summary>
        public string SpecialityCode { get { return specialityCode; } }

        /// <summary>
        /// Код медицинского работника, оказавшего медицинскую услугу
        /// В соответствии с территориальным справочником
        /// </summary>
        public string DoctorCode { get { return doctorCode; } }

        /// <summary>
        /// Неполный объем
        /// Указывается причина, по которой услуга не оказана или оказана не в полном объеме.
        /// </summary>
        public IncompleteServiceReason Incomplete { get { return incomplete; } }

        /// <summary>
        /// Служебное поле
        /// </summary>
        public string Comment { get { return comment; } }

        /// <summary>
        /// Признак отказа от услуги
        /// Значение по умолчанию: "0".
        /// В случае отказа указывается значение "1".
        /// </summary>
        public bool Refusal { get { return refusal; } }

        /// <summary>
        /// Save service instance to a XML
        /// </summary>
        /// <param name="xml">XML exporter to use</param>
        /// <param name="pool">Datapool</param>
        /// <param name="section">Section of the order #59</param>
        /// <param name="irec">Invoice record parental to the event</param>
        /// <param name="evt">Event to which this service belongs</param>
        public void Write(Lib.XmlExporter xml, Data.IInvoice pool, OrderSection section, InvoiceRecord irec, Recourse rec, Event evt) {
            switch (section) {
                case OrderSection.D1:
                    WriteD1(xml, pool, irec, rec, evt);
                    break;
                case OrderSection.D2:
                    WriteD2(xml, pool, irec, rec, evt);
                    break;
                case OrderSection.D3:
                    WriteD3(xml, pool, irec, rec, evt);
                    break;
            }
        }

        /// <summary>
        /// Save service for a treatment case
        /// </summary>
        /// <param name="xml">XML exporter to use</param>
        /// <param name="pool">Datapool</param>
        /// <param name="irec">Invoice record parental to the event</param>
        /// <param name="evt">Event to which this service belongs</param>
        public void WriteD1(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec, Recourse rec, Event evt) {
            xml.Writer.WriteStartElement("USL");

            xml.Writer.WriteElementString("IDSERV", id);
            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            xml.WriteIfValid("LPU_1", unit);
            xml.WriteIfValid("PODR", rec.Department);
            xml.Writer.WriteElementString("PROFIL", rec.Profile);
            xml.WriteIfValid("VID_VME", interventionKind);
            xml.WriteBool("DET", child);
            xml.Writer.WriteElementString("DATE_IN", dateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_OUT", dateTill.AsXml());
            xml.Writer.WriteElementString("DS", diagnosis);
            xml.Writer.WriteElementString("CODE_USL", serviceCode);
            xml.Writer.WriteElementString("KOL_USL", quantity.ToString("F2", Options.NumberFormat));

            if (tariff > 0)
                xml.Writer.WriteElementString("TARIF", tariff.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteElementString("SUMV_USL", total.ToString("F2", Options.NumberFormat));
            xml.Writer.WriteElementString("PRVS", specialityCode);
            xml.Writer.WriteElementString("CODE_MD", doctorCode);

            if (rec.SuspectOncology) {
                // Направления
                // Заполняется только в случае оформления направления при подозрении на злокачественное новообразование (DS_ONK=1)
                foreach (OncologyDirection d in pool.LoadOncologyDirections())
                    d.Write(xml);
            }

            if (Oncology) {
                OncologyService o = pool.GetOncologyService();
                // Сведения об услуге при лечении онкологического заболевания
                // Обязательно к заполнению при заполненном элементе ONK_SL.
                // Не подлежит заполнению при DS_ONK=1
                if (o != null) o.Write(xml);
            }

            if (incomplete != IncompleteServiceReason.None)
                xml.Writer.WriteElementString("NPL", ((int)incomplete).ToString());

            xml.WriteIfValid("COMENTU", comment);

            xml.Writer.WriteEndElement();
        }

        /// <summary>
        /// Save service for a hi-tech case
        /// </summary>
        /// <param name="xml">XML exporter to use</param>
        /// <param name="pool">Datapool</param>
        /// <param name="irec">Invoice record parental to the event</param>
        /// <param name="evt">Event to which this service belongs</param>
        public void WriteD2(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec, Recourse rec, Event evt) {
            xml.Writer.WriteStartElement("USL");

            xml.Writer.WriteElementString("IDSERV", id);
            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            xml.WriteIfValid("LPU_1", unit);
            xml.WriteIfValid("PODR", rec.Department);
            xml.Writer.WriteElementString("PROFIL", rec.Profile);
            xml.WriteIfValid("VID_VME", interventionKind);
            xml.WriteBool("DET", child);
            xml.Writer.WriteElementString("DATE_IN", dateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_OUT", dateTill.AsXml());
            xml.Writer.WriteElementString("DS", diagnosis);
            xml.Writer.WriteElementString("CODE_USL", serviceCode);
            xml.Writer.WriteElementString("KOL_USL", quantity.ToString("F2", Options.NumberFormat));

            if (tariff > 0)
                xml.Writer.WriteElementString("TARIF", tariff.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteElementString("SUMV_USL", total.ToString("F2", Options.NumberFormat));
            xml.Writer.WriteElementString("PRVS", specialityCode);
            xml.Writer.WriteElementString("CODE_MD", doctorCode);

            if (rec.SuspectOncology) {
                // Направления
                // Заполняется только в случае оформления направления при подозрении на злокачественное новообразование (DS_ONK=1)
                foreach (OncologyDirection d in pool.LoadOncologyDirections())
                    d.Write(xml);
            }

            if (Oncology) {
                OncologyService o = pool.GetOncologyService();
                if (o != null) o.Write(xml);
            }
            xml.WriteIfValid("COMENTU", comment);

            xml.Writer.WriteEndElement();
        }

        /// <summary>
        /// Save service for a dispanserisation case
        /// </summary>
        /// <param name="xml">XML exporter to use</param>
        /// <param name="pool">Datapool</param>
        /// <param name="irec">Invoice record parental to the event</param>
        /// <param name="evt">Event to which this service belongs</param>
        public void WriteD3(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec, Recourse rec, Event evt) {
            xml.Writer.WriteStartElement("USL");

            xml.Writer.WriteElementString("IDSERV", id);
            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            xml.WriteIfValid("LPU_1", unit);
            xml.Writer.WriteElementString("DATE_IN", dateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_OUT", dateTill.AsXml());
            xml.WriteBool("P_OTK", refusal);
            xml.Writer.WriteElementString("CODE_USL", serviceCode);

            if (tariff > 0)
                xml.Writer.WriteElementString("TARIF", tariff.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteElementString("SUMV_USL", total.ToString("F2", Options.NumberFormat));
            xml.Writer.WriteElementString("PRVS", specialityCode);
            xml.Writer.WriteElementString("CODE_MD", doctorCode);
            xml.WriteIfValid("COMENTU", comment);
            xml.Writer.WriteEndElement();
        }
    }
}
