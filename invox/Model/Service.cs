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

        /// <summary>
        /// Номер записи в реестре услуг
        /// Уникален в пределах случая
        /// </summary>
        public string Identity;

        /// <summary>
        /// Подразделение МО
        /// Подразделение МО лечения из регионального справочника
        /// </summary>
        public string Unit;

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
        public string InterventionKind;

        /// <summary>
        /// Признак детского профиля
        /// 0 - нет, 1 - да.
        /// Заполняется в зависимости от профиля оказанной медицинской помощи.
        /// </summary>
        public bool Child;

        /// <summary>
        /// Дата начала оказания услуги
        /// </summary>
        public DateTime DateFrom;
        
        /// <summary>
        /// Дата окончания оказания услуги
        /// </summary>
        public DateTime DateTill;

        /// <summary>
        /// Диагноз
        /// Код из справочника МКБ до уровня подрубрики
        /// </summary>
        public string Diagnosis;

        /// <summary>
        /// Код услуги
        /// Заполняется в соответствии с территориальным классификатором услуг.
        /// </summary>
        public int ServiceCode;

        /// <summary>
        /// Количество услуг (кратность услуги)
        /// </summary>
        public int Quantity;

        /// <summary>
        /// Тариф
        /// </summary>
        public decimal Tariff;

        /// <summary>
        /// Стоимость медицинской услуги, принятая к оплате (руб.)
        /// Может принимать значение 0
        /// </summary>
        public decimal Total;

        /// <summary>
        /// Специальность медработника, выполнившего услугу
        /// Классификатор медицинских специальностей (Приложение А V021). Указывается значение IDSPEC
        /// </summary>
        public string SpecialityCode;

        /// <summary>
        /// Код медицинского работника, оказавшего медицинскую услугу
        /// В соответствии с территориальным справочником
        /// </summary>
        public string DoctorCode;

        /// <summary>
        /// Неполный объем
        /// Указывается причина, по которой услуга не оказана или оказана не в полном объеме.
        /// </summary>
        public IncompleteServiceReason Incomplete;

        /// <summary>
        /// Служебное поле
        /// </summary>
        public string Comment;

        /// <summary>
        /// Признак отказа от услуги
        /// Значение по умолчанию: "0".
        /// В случае отказа указывается значение "1".
        /// </summary>
        public bool Refusal;

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

            xml.Writer.WriteElementString("IDSERV", Identity);
            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            xml.WriteIfValid("LPU_1", Unit);
            xml.WriteIfValid("PODR", rec.Department);
            xml.Writer.WriteElementString("PROFIL", rec.Profile);
            xml.WriteIfValid("VID_VME", InterventionKind);
            xml.WriteBool("DET", Child);
            xml.Writer.WriteElementString("DATE_IN", DateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_OUT", DateTill.AsXml());
            xml.Writer.WriteElementString("DS", Diagnosis);
            xml.Writer.WriteElementString("CODE_USL", ServiceCode.ToString("D6"));
            xml.Writer.WriteElementString("KOL_USL", Quantity.ToString("F2", Options.NumberFormat));

            if (Tariff > 0)
                xml.Writer.WriteElementString("TARIF", Tariff.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteElementString("SUMV_USL", Total.ToString("F2", Options.NumberFormat));
            xml.Writer.WriteElementString("PRVS", SpecialityCode);
            xml.Writer.WriteElementString("CODE_MD", DoctorCode);

            if (Incomplete != IncompleteServiceReason.None)
                xml.Writer.WriteElementString("NPL", ((int)Incomplete).ToString());

            xml.WriteIfValid("COMENTU", Comment);

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

            xml.Writer.WriteElementString("IDSERV", Identity);
            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            xml.WriteIfValid("LPU_1", Unit);
            xml.WriteIfValid("PODR", rec.Department);
            xml.Writer.WriteElementString("PROFIL", rec.Profile);
            xml.WriteIfValid("VID_VME", InterventionKind);
            xml.WriteBool("DET", Child);
            xml.Writer.WriteElementString("DATE_IN", DateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_OUT", DateTill.AsXml());
            xml.Writer.WriteElementString("DS", Diagnosis);
            xml.Writer.WriteElementString("CODE_USL", ServiceCode.ToString("D6"));
            xml.Writer.WriteElementString("KOL_USL", Quantity.ToString("D2", Options.NumberFormat));

            if (Tariff > 0)
                xml.Writer.WriteElementString("TARIF", Tariff.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteElementString("SUMV_USL", Total.ToString("F2", Options.NumberFormat));
            xml.Writer.WriteElementString("PRVS", SpecialityCode);
            xml.Writer.WriteElementString("CODE_MD", DoctorCode);

            if (rec.SuspectOncology) {
                // Направления
                // Заполняется только в случае оформления направления при подозрении на злокачественное новообразование (DS_ONK=1)
                foreach (OncologyDirection d in pool.LoadOncologyDirections(rec, evt))
                    d.Write(xml);
            }

            xml.WriteIfValid("COMENTU", Comment);

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

            xml.Writer.WriteElementString("IDSERV", Identity);
            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            xml.WriteIfValid("LPU_1", Unit);
#if FOMS
            xml.WriteIfValid("PODR", rec.Department);
            // TODO: "20190611 Приказ 124н Диспансеризация и Профосмотры" - профиль каждой услуги отдельно???
            xml.Writer.WriteElementString("PROFIL", rec.Profile);
            xml.WriteBool("DET", Child);
#endif
            xml.Writer.WriteElementString("DATE_IN", DateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_OUT", DateTill.AsXml());
            xml.WriteBool("P_OTK", Refusal);
            xml.Writer.WriteElementString("CODE_USL", ServiceCode.ToString("D6"));
#if FOMS
            xml.Writer.WriteElementString("KOL_USL", Quantity.ToString("F2", Options.NumberFormat));
#endif
            if (Tariff > 0)
                xml.Writer.WriteElementString("TARIF", Tariff.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteElementString("SUMV_USL", Total.ToString("F2", Options.NumberFormat));
            xml.Writer.WriteElementString("PRVS", SpecialityCode);
            xml.Writer.WriteElementString("CODE_MD", DoctorCode);
            xml.WriteIfValid("COMENTU", Comment);
            xml.Writer.WriteEndElement();
        }

        public void WriteD4(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec, Recourse rec, Event evt) {
            xml.Writer.WriteStartElement("USL");

            xml.Writer.WriteElementString("IDSERV", Identity);
            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            xml.WriteIfValid("LPU_1", Unit);
            xml.WriteIfValid("PODR", rec.Department);
            xml.Writer.WriteElementString("PROFIL", rec.Profile);
            xml.WriteIfValid("VID_VME", InterventionKind);
            xml.WriteBool("DET", Child);
            xml.Writer.WriteElementString("DATE_IN", DateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_OUT", DateTill.AsXml());
            xml.Writer.WriteElementString("DS", Diagnosis);
            xml.Writer.WriteElementString("CODE_USL", ServiceCode.ToString("D6"));
            xml.Writer.WriteElementString("KOL_USL", Quantity.ToString("F2", Options.NumberFormat));

            if (Tariff > 0)
                xml.Writer.WriteElementString("TARIF", Tariff.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteElementString("SUMV_USL", Total.ToString("F2", Options.NumberFormat));
            xml.Writer.WriteElementString("PRVS", SpecialityCode);
            xml.Writer.WriteElementString("CODE_MD", DoctorCode);

            if (Incomplete != IncompleteServiceReason.None)
                xml.Writer.WriteElementString("NPL", ((int)Incomplete).ToString());

            xml.WriteIfValid("COMENTU", Comment);

            xml.Writer.WriteEndElement();
        }
    }
}
