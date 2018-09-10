using System;
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
        string id;
        int conditions;
        int aidKind;
        int aidForm;
        string directedFrom;
        DateTime directionDate;
        DateTime dateFrom;
        DateTime dateTill;
        int bedDays;
        int birthWeight;
        int result;
        int outcome;
        List<SpecialCase> specialCase;
        bool unitShift;
        int payKind;
        double total;
        PayType payType;
        double acceptedSum;
        double deniedSum;

        bool mobileBrigade;
        bool dispRefusal;
        string dispResult;

        /// <summary>
        /// Номер записи в реестре законченных случаев
        /// Соответствует порядковому номеру записи реестра счета на бумажном носителе при его предоставлении.
        /// </summary>
        public string Identity { get { return id; } }

        /// <summary>
        /// Условия оказания медицинской помощи
        /// Классификатор условий оказания медицинской помощи (V006 Приложения А).
        /// </summary>
        public int Conditions { get { return conditions; } }
        
        /// <summary>
        /// Вид медицинской помощи
        /// Классификатор видов медицинской помощи. Справочник V008 Приложения А.
        /// </summary>
        public int AidKind { get { return aidKind; } }
        
        /// <summary>
        /// Форма оказания медицинской помощи
        /// Классификатор форм оказания медицинской помощи. Справочник V014 Приложения А
        /// </summary>
        public int AidForm { get { return aidForm; } }

        /// <summary>
        /// Код МО, направившей на лечение (диагностику, консультацию, госпитализацию)
        /// Код МО - юридического лица. Заполняется в соответствии со справочником F003 Приложения А.
        /// Заполнение обязательно в случаях оказания:
        /// 1. плановой медицинской помощи в условиях стационара и дневного стационара (FOR_POM=3 и USL_OK=(1, 2));
        /// 2. неотложной медицинской помощи в условиях стационара (FOR_POM=2 и USL_OK=1);
        /// 3. медицинской помощи при подозрении на злокачественное новообразование (DS_ONK=l)
        /// </summary>
        public string DirectedFrom { get { return directedFrom; } }

        /// <summary>
        /// Дата направления на лечение (диагностику, консультацию, госпитализацию)
        /// Заполняется на основании направления на лечение.
        /// Заполнение обязательно в случаях оказания:
        /// 1. плановой медицинской помощи в условиях стационара и дневного стационара (FOR_POM=3 и USL_OK=(1, 2));
        /// 2. неотложной медицинской помощи в условиях стационара (FOR_POM=2 и USL_OK=1);
        /// 3. медицинской помощи при подозрении на злокачественное новообразование (DS_ONK=l)
        /// </summary>
        public DateTime DirectionDate { get { return directionDate; } }

        /// <summary>
        /// Дата начала лечения
        /// </summary>
        public DateTime DateFrom { get { return dateFrom; } }

        /// <summary>
        /// Дата окончания лечения
        /// </summary>
        public DateTime DateTill { get { return dateTill; } }

        /// <summary>
        /// Продолжительность госпитализации (койко-дни/пациенто-дни)
        /// Обязательно для заполнения для стационара и дневного стационара
        /// </summary>
        public int BedDays { get { return bedDays; } }

        /// <summary>
        /// Вес при рождении
        /// Указывается при оказании медицинской помощи недоношенным и маловесным детям.
        /// Поле заполняется, если в качестве пациента указана мать.
        /// </summary>
        public int BirthWeight { get { return birthWeight; } }

        /// <summary>
        /// Результат обращения
        /// Классификатор результатов обращения за медицинской помощью (Приложение А V009).
        /// </summary>
        public int Result { get { return result; } }

        /// <summary>
        /// Исход заболевания
        /// Классификатор исходов заболевания (Приложение А V012).
        /// </summary>
        public int Outcome { get { return outcome; } }
        
        //List<SpecialCase> specialCase { get { return ; } }

        /// <summary>
        /// Признак внутрибольничного перевода	Указывается" 1" только при оплате случая по КСГ с внутрибольничным переводом.
        /// </summary>
        public bool UnitShift { get { return unitShift; } }

        /// <summary>
        /// Код способа оплаты медицинской помощи
        /// Классификатор способов оплаты медицинской помощи V010
        /// </summary>
        public int PayKind { get { return payKind; } }

        /// <summary>
        /// Сумма, выставленная к оплате
        /// Равна сумме значений SUM_M вложенных элементов SL, не может иметь нулевое значение.
        /// </summary>
        public double Total { get { return total; } }

        /// <summary>
        /// Тип оплаты
        /// </summary>
        public PayType PayType { get { return payType; } }

        /// <summary>
        /// Сумма, принятая к оплате СМО (ТФОМС)
        /// Заполняется СМО (ТФОМС).
        /// </summary>
        public double AcceptedSum { get { return acceptedSum; } }

        /// <summary>
        /// Сумма санкций по законченному случаю
        /// Итоговые санкции определяются на основании санкций, описанных ниже
        /// </summary>
        public double DeniedSum { get { return deniedSum; } }

        /// <summary>
        /// Признак мобильной медицинской бригады (для Д3)
        /// </summary>
        public bool MobileBrigade { get { return mobileBrigade; } }

        /// <summary>
        /// Признак отказа (D3)
        /// </summary>
        public bool DispanserisationRefusal { get { return dispRefusal; } }
        
        /// <summary>
        /// Результат диспансеризации
        /// Классификатор результатов диспансеризации V017
        /// </summary>
        public string DispanserisationResult { get { return dispResult; } }

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
            }
        }

        public void WriteD1(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec) {
            xml.Writer.WriteStartElement("Z_SL");

            xml.Writer.WriteElementString("IDCASE", id);
            xml.Writer.WriteElementString("USL_OK", conditions.ToString());
            xml.Writer.WriteElementString("VIDPOM", aidKind.ToString());

            xml.Writer.WriteElementString("FOR_POM", aidForm.ToString());

            if (!string.IsNullOrEmpty(directedFrom)) {
                xml.Writer.WriteElementString("NPR_MO", directedFrom);
                xml.Writer.WriteElementString("NPR_DATE", directionDate.AsXml());
            }

            xml.Writer.WriteElementString("LPU", Options.LpuCode);

            xml.Writer.WriteElementString("DATE_Z_1", dateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_Z_2", dateTill.AsXml());

            if (bedDays > 0)
                xml.Writer.WriteElementString("KD_Z", bedDays.ToString());

            if (birthWeight > 0)
                xml.Writer.WriteElementString("VNOV_M", birthWeight.ToString());

            xml.Writer.WriteElementString("RSLT", result.ToString());
            xml.Writer.WriteElementString("ISHOD", outcome.ToString());

            if (specialCase != null) {
                foreach (SpecialCase c in specialCase)
                    xml.Writer.WriteElementString("OS_SLUCH", ((int)c).ToString());
            }

            if (unitShift)
                xml.Writer.WriteElementString("VB_P", "1");

            foreach (Event e in pool.LoadEvents())
                e.WriteD1(xml, pool, irec);

            xml.Writer.WriteElementString("IDSP", payKind.ToString());
            xml.Writer.WriteElementString("SUMV", total.ToString("F2", Options.NumberFormat));

            if (payType != Model.PayType.None)
                xml.Writer.WriteElementString("OPLATA", ((int)payType).ToString());

            if (acceptedSum > 0)
                xml.Writer.WriteElementString("SUMP", acceptedSum.ToString("F2", Options.NumberFormat));

            if (deniedSum > 0)
                xml.Writer.WriteElementString("SANKIT", deniedSum.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteEndElement();
        }

        public void WriteD2(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec) {
            xml.Writer.WriteStartElement("Z_SL");

            xml.Writer.WriteElementString("IDCASE", id);
            xml.Writer.WriteElementString("USL_OK", conditions.ToString());
            xml.Writer.WriteElementString("VIDPOM", aidKind.ToString());

            xml.Writer.WriteElementString("FOR_POM", aidForm.ToString());

            if (!string.IsNullOrEmpty(directedFrom))
                xml.Writer.WriteElementString("NPR_MO", directedFrom);

            xml.Writer.WriteElementString("LPU", Options.LpuCode);

            xml.Writer.WriteElementString("DATE_Z_1", dateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_Z_2", dateTill.AsXml());

            if (bedDays > 0)
                xml.Writer.WriteElementString("KD_Z", bedDays.ToString());

            if (birthWeight > 0)
                xml.Writer.WriteElementString("VNOV_M", birthWeight.ToString());

            xml.Writer.WriteElementString("RSLT", result.ToString());
            xml.Writer.WriteElementString("ISHOD", outcome.ToString());

            if (specialCase != null) {
                foreach (SpecialCase c in specialCase)
                    xml.Writer.WriteElementString("OS_SLUCH", ((int)c).ToString());
            }

            foreach (Event e in pool.LoadEvents())
                e.WriteD2(xml, pool, irec);

            xml.Writer.WriteElementString("IDSP", payKind.ToString());
            xml.Writer.WriteElementString("SUMV", total.ToString("F2", Options.NumberFormat));

            if (payType != Model.PayType.None)
                xml.Writer.WriteElementString("OPLATA", ((int)payType).ToString());

            if (acceptedSum > 0)
                xml.Writer.WriteElementString("SUMP", acceptedSum.ToString("F2", Options.NumberFormat));

            if (deniedSum > 0)
                xml.Writer.WriteElementString("SANKIT", deniedSum.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteEndElement();
        }

        public void WriteD3(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec) {
            xml.Writer.WriteStartElement("Z_SL");

            xml.Writer.WriteElementString("IDCASE", id);
            xml.Writer.WriteElementString("USL_OK", conditions.ToString());
            xml.Writer.WriteElementString("VIDPOM", aidKind.ToString());

            xml.Writer.WriteElementString("LPU", Options.LpuCode);

            xml.WriteBool("VBR", mobileBrigade);

            xml.Writer.WriteElementString("DATE_Z_1", dateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_Z_2", dateTill.AsXml());

            xml.WriteBool("P_OTK", dispRefusal);
            xml.Writer.WriteElementString("RSLT_D", dispResult);

            if (specialCase != null) {
                foreach (SpecialCase c in specialCase)
                    xml.Writer.WriteElementString("OS_SLUCH", ((int)c).ToString());
            }

            foreach (Event e in pool.LoadEvents())
                e.WriteD3(xml, pool, irec);

            xml.Writer.WriteElementString("IDSP", payKind.ToString());
            xml.Writer.WriteElementString("SUMV", total.ToString("F2", Options.NumberFormat));

            if (payType != Model.PayType.None)
                xml.Writer.WriteElementString("OPLATA", ((int)payType).ToString());

            if (acceptedSum > 0)
                xml.Writer.WriteElementString("SUMP", acceptedSum.ToString("F2", Options.NumberFormat));

            if (deniedSum > 0)
                xml.Writer.WriteElementString("SANKIT", deniedSum.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteEndElement();
        }
    }
}
