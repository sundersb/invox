using System;
using System.Linq;
using System.Collections.Generic;
using invox.Lib;

namespace invox.Model {
    /// <summary>
    /// Признак "Особый случай" при регистрации обращения за медицинской помощью
    /// </summary>
    enum SpecialCase : int {
        None = 0,
        Prolific = 1, // медицинская помощь оказана новорожденному ребенку до государственной регистрации рождения при многоплодных родах;
        NoPatronymic = 2 // в документе, удостоверяющем личность пациента/родителя (представителя) пациента, отсутствует отчество.
    }

    /// <summary>
    /// Тип оплаты
    /// </summary>
    enum PayType {
        None = 0,            // не принято решение об оплате
        Full = 1,            // полная;
        Refused = 2,         // полный отказ;
        PartiallyRefused = 3 // частичный отказ.
    }

    /// <summary>
    /// Сведения о законченном случае (ZL_LIST/ZAP/Z_SL)
    /// Сведения о законченном случае оказания медицинской помощи
    /// <remarks>
    /// Вложен в
    ///     InvoiceRecord ZAP (Single)
    /// Содержит
    ///     Event SL (Multiple)
    /// </remarks>
    /// </summary>
    class Recourse {
        List<SpecialCase> specialCase;
        
        string condition;
        bool isHospitalization;
        
        public List<Event> Events { get; set; }

        /// <summary>
        /// Подозрение на онко. Не используется в выгрузке
        /// </summary>
        public bool SuspectOncology { get; set; }

        public bool IsHospitalization { get { return isHospitalization; } }

        public string Department { get; set; }
        public string Profile { get; set; }
        
        /// <summary>
        /// Номер записи в реестре законченных случаев
        /// Соответствует порядковому номеру записи реестра счета на бумажном носителе при его предоставлении.
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// Условия оказания медицинской помощи
        /// Классификатор условий оказания медицинской помощи (V006 Приложения А).
        /// </summary>
        public string Conditions {
            get { return condition; }
            set {
                condition = value;
                isHospitalization = value == "1" || value == "2";
            }
        }
        
        /// <summary>
        /// Вид медицинской помощи
        /// Классификатор видов медицинской помощи. Справочник V008 Приложения А.
        /// </summary>
        public int AidKind { get; set; }
        
        /// <summary>
        /// Форма оказания медицинской помощи
        /// Классификатор форм оказания медицинской помощи. Справочник V014 Приложения А
        /// </summary>
        public int AidForm { get; set; }

        /// <summary>
        /// Код МО, направившей на лечение (диагностику, консультацию, госпитализацию)
        /// Код МО - юридического лица. Заполняется в соответствии со справочником F003 Приложения А.
        /// Заполнение обязательно в случаях оказания:
        /// 1. плановой медицинской помощи в условиях стационара (FOR_POM=3 и USL_OK = 1);
        /// 2. в условиях дневного стационара (USL_OK =2)
        /// </summary>
        public string DirectedFrom { get; set; }

        /// <summary>
        /// Дата направления на лечение (диагностику, консультацию, госпитализацию)
        /// Заполняется на основании направления на лечение.
        /// Заполнение обязательно в случаях оказания:
        /// 1. плановой медицинской помощи в условиях стационара (FOR_POM=3 и USL_OK = 1);
        /// 2. в условиях дневного стационара (USL_OK =2)
        /// </summary>
        public DateTime DirectionDate { get; set; }

        /// <summary>
        /// Дата начала лечения
        /// </summary>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// Дата окончания лечения
        /// </summary>
        public DateTime DateTill { get; set; }

        /// <summary>
        /// Продолжительность госпитализации (койко-дни/пациенто-дни)
        /// Обязательно для заполнения для стационара и дневного стационара
        /// </summary>
        public int BedDays { get; set; }

        /// <summary>
        /// Вес при рождении
        /// Указывается при оказании медицинской помощи недоношенным и маловесным детям.
        /// Поле заполняется, если в качестве пациента указана мать.
        /// </summary>
        public int BirthWeight { get; set; }

        /// <summary>
        /// Результат обращения
        /// Классификатор результатов обращения за медицинской помощью (Приложение А V009).
        /// </summary>
        public int Result { get; set; }

        /// <summary>
        /// Исход заболевания
        /// Классификатор исходов заболевания (Приложение А V012).
        /// </summary>
        public string Outcome { get; set; }
        
        //List<SpecialCase> specialCase { get { return ; } }

        /// <summary>
        /// Признак внутрибольничного перевода	Указывается" 1" только при оплате случая по КСГ с внутрибольничным переводом.
        /// </summary>
        public bool UnitShift { get; set; }

        /// <summary>
        /// Код способа оплаты медицинской помощи
        /// Классификатор способов оплаты медицинской помощи V010
        /// TODO: Изменен 11.01.2019
        /// </summary>
        public string PayKind { get; set; }

        /// <summary>
        /// Сумма, выставленная к оплате
        /// Равна сумме значений SUM_M вложенных элементов SL, не может иметь нулевое значение.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Тип оплаты
        /// </summary>
        public PayType PayType { get; set; }

        /// <summary>
        /// Сумма, принятая к оплате СМО (ТФОМС)
        /// Заполняется СМО (ТФОМС).
        /// </summary>
        public double AcceptedSum { get; set; }

        /// <summary>
        /// Сумма санкций по законченному случаю
        /// Итоговые санкции определяются на основании санкций, описанных ниже
        /// </summary>
        public double DeniedSum { get; set; }

        /// <summary>
        /// Признак мобильной медицинской бригады (для Д3)
        /// </summary>
        public bool MobileBrigade { get; set; }

        /// <summary>
        /// Признак отказа (D3)
        /// </summary>
        public bool DispanserisationRefusal { get; set; }
        
        /// <summary>
        /// Результат диспансеризации
        /// Классификатор результатов диспансеризации V017
        /// </summary>
        public string DispanserisationResult { get; set; }

        public void Write(Lib.XmlExporter xml, Data.IInvoice pool, OrderSection section, InvoiceRecord irec) {
            switch (section) {
                case OrderSection.D1:
                    WriteD1(xml, pool, irec);
                    break;
                case OrderSection.D2:
                    WriteD2(xml, pool, irec);
                    break;
                case OrderSection.D3:
                    WriteD3(xml, pool, irec);
                    break;
                case OrderSection.D4:
                    WriteD4(xml, pool, irec);
                    break;
            }
        }
        
        public void WriteD1(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec) {
            xml.Writer.WriteStartElement("Z_SL");

            xml.Writer.WriteElementString("IDCASE", Identity);
            xml.Writer.WriteElementString("USL_OK", Conditions);
            xml.Writer.WriteElementString("VIDPOM", AidKind.ToString());

            xml.Writer.WriteElementString("FOR_POM", AidForm.ToString());

            if (!string.IsNullOrEmpty(DirectedFrom)) {
                xml.Writer.WriteElementString("NPR_MO", DirectedFrom);
                xml.Writer.WriteElementString("NPR_DATE", DirectionDate.AsXml());
            }

            xml.Writer.WriteElementString("LPU", Options.LpuCode);

            xml.Writer.WriteElementString("DATE_Z_1", DateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_Z_2", DateTill.AsXml());

            if (BedDays > 0)
                xml.Writer.WriteElementString("KD_Z", BedDays.ToString());

            if (BirthWeight > 0)
                xml.Writer.WriteElementString("VNOV_M", BirthWeight.ToString());

            xml.Writer.WriteElementString("RSLT", Result.ToString());
            xml.Writer.WriteElementString("ISHOD", Outcome);

            if (specialCase != null) {
                foreach (SpecialCase c in specialCase)
                    xml.Writer.WriteElementString("OS_SLUCH", ((int)c).ToString());
            }

            if (UnitShift)
                xml.Writer.WriteElementString("VB_P", "1");

            foreach (Event e in Events)
                e.WriteD1(xml, pool, irec, this);

            xml.Writer.WriteElementString("IDSP", PayKind);
            xml.Writer.WriteElementString("SUMV", Total.ToString("F2", Options.NumberFormat));

            if (PayType != Model.PayType.None)
                xml.Writer.WriteElementString("OPLATA", ((int)PayType).ToString());

            if (AcceptedSum > 0)
                xml.Writer.WriteElementString("SUMP", AcceptedSum.ToString("F2", Options.NumberFormat));

            // Сведения о санкциях
            // Описывает санкции, примененные в рамках данного случая.
            foreach (Sanction s in pool.LoadSanctions(irec, this))
                s.Write(xml, pool);

            if (DeniedSum > 0)
                xml.Writer.WriteElementString("SANKIT", DeniedSum.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteEndElement();
        }

        public void WriteD2(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec) {
            xml.Writer.WriteStartElement("Z_SL");

            xml.Writer.WriteElementString("IDCASE", Identity);
            xml.Writer.WriteElementString("USL_OK", Conditions.ToString());
            xml.Writer.WriteElementString("VIDPOM", AidKind.ToString());

            xml.Writer.WriteElementString("FOR_POM", AidForm.ToString());

            if (!string.IsNullOrEmpty(DirectedFrom))
                xml.Writer.WriteElementString("NPR_MO", DirectedFrom);

            xml.Writer.WriteElementString("LPU", Options.LpuCode);

            xml.Writer.WriteElementString("DATE_Z_1", DateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_Z_2", DateTill.AsXml());

            if (BedDays > 0)
                xml.Writer.WriteElementString("KD_Z", BedDays.ToString());

            if (BirthWeight > 0)
                xml.Writer.WriteElementString("VNOV_M", BirthWeight.ToString());

            xml.Writer.WriteElementString("RSLT", Result.ToString());
            xml.Writer.WriteElementString("ISHOD", Outcome.ToString());

            if (specialCase != null) {
                foreach (SpecialCase c in specialCase)
                    xml.Writer.WriteElementString("OS_SLUCH", ((int)c).ToString());
            }

            foreach (Event e in Events)
                e.WriteD2(xml, pool, irec, this);

            xml.Writer.WriteElementString("IDSP", PayKind.ToString());
            xml.Writer.WriteElementString("SUMV", Total.ToString("F2", Options.NumberFormat));

            if (PayType != Model.PayType.None)
                xml.Writer.WriteElementString("OPLATA", ((int)PayType).ToString());

            if (AcceptedSum > 0)
                xml.Writer.WriteElementString("SUMP", AcceptedSum.ToString("F2", Options.NumberFormat));

            // Сведения о санкциях
            // Описывает санкции, примененные в рамках данного случая.
            foreach (Sanction s in pool.LoadSanctions(irec, this))
                s.Write(xml, pool);

            if (DeniedSum > 0)
                xml.Writer.WriteElementString("SANKIT", DeniedSum.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteEndElement();
        }

        public void WriteD3(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec) {
            xml.Writer.WriteStartElement("Z_SL");

            xml.Writer.WriteElementString("IDCASE", Identity);

#if FOMS
            xml.Writer.WriteElementString("USL_OK", Conditions);
#endif

            xml.Writer.WriteElementString("VIDPOM", AidKind.ToString());

#if FOMS
            xml.Writer.WriteElementString("FOR_POM", AidForm.ToString());
#endif

            xml.Writer.WriteElementString("LPU", Options.LpuCode);

            // Dinamically?
            xml.WriteBool("VBR", MobileBrigade);

            xml.Writer.WriteElementString("DATE_Z_1", DateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_Z_2", DateTill.AsXml());

            // Dynamically
            xml.WriteBool("P_OTK", DispanserisationRefusal);
            xml.Writer.WriteElementString("RSLT_D", DispanserisationResult);

#if FOMS
            xml.Writer.WriteElementString("ISHOD", Outcome);
#endif

            if (specialCase != null) {
                foreach (SpecialCase c in specialCase)
                    xml.Writer.WriteElementString("OS_SLUCH", ((int)c).ToString());
            }

            foreach (Event e in Events)
                e.WriteD3(xml, pool, irec, this);

            xml.Writer.WriteElementString("IDSP", PayKind.ToString());
            xml.Writer.WriteElementString("SUMV", Total.ToString("F2", Options.NumberFormat));

            if (PayType != Model.PayType.None)
                xml.Writer.WriteElementString("OPLATA", ((int)PayType).ToString());

            if (AcceptedSum > 0)
                xml.Writer.WriteElementString("SUMP", AcceptedSum.ToString("F2", Options.NumberFormat));

            // Сведения о санкциях
            // Описывает санкции, примененные в рамках данного случая.
            foreach (Sanction s in pool.LoadSanctions(irec, this))
                s.Write(xml, pool);

            if (DeniedSum > 0)
                xml.Writer.WriteElementString("SANKIT", DeniedSum.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteEndElement();
        }
        
        public void WriteD4(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec) {
            xml.Writer.WriteStartElement("Z_SL");

            xml.Writer.WriteElementString("IDCASE", Identity);
            xml.Writer.WriteElementString("USL_OK", Conditions);
            xml.Writer.WriteElementString("VIDPOM", AidKind.ToString());

            xml.Writer.WriteElementString("FOR_POM", AidForm.ToString());

            if (!string.IsNullOrEmpty(DirectedFrom)) {
                xml.Writer.WriteElementString("NPR_MO", DirectedFrom);
                xml.Writer.WriteElementString("NPR_DATE", DirectionDate.AsXml());
            }

            xml.Writer.WriteElementString("LPU", Options.LpuCode);

            xml.Writer.WriteElementString("DATE_Z_1", DateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_Z_2", DateTill.AsXml());

            if (BedDays > 0)
                xml.Writer.WriteElementString("KD_Z", BedDays.ToString());

            if (BirthWeight > 0)
                xml.Writer.WriteElementString("VNOV_M", BirthWeight.ToString());

            xml.Writer.WriteElementString("RSLT", Result.ToString());
            xml.Writer.WriteElementString("ISHOD", Outcome);

            if (specialCase != null) {
                foreach (SpecialCase c in specialCase)
                    xml.Writer.WriteElementString("OS_SLUCH", ((int)c).ToString());
            }

            if (UnitShift)
                xml.Writer.WriteElementString("VB_P", "1");

            foreach (Event e in Events)
                e.WriteD4(xml, pool, irec, this);

            xml.Writer.WriteElementString("IDSP", PayKind);
            xml.Writer.WriteElementString("SUMV", Total.ToString("F2", Options.NumberFormat));

            if (PayType != Model.PayType.None)
                xml.Writer.WriteElementString("OPLATA", ((int)PayType).ToString());

            if (AcceptedSum > 0)
                xml.Writer.WriteElementString("SUMP", AcceptedSum.ToString("F2", Options.NumberFormat));

            foreach (Sanction s in pool.LoadSanctions(irec, this))
                s.Write(xml, pool);

            if (DeniedSum > 0)
                xml.Writer.WriteElementString("SANKIT", DeniedSum.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteEndElement();
        }
    }
}
