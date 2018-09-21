using System;
using invox.Lib;
using System.Collections.Generic;

namespace invox.Model {
    /// <summary>
    /// Признак поступления/перевода
    /// </summary>
    enum Transfer {
        None = 0,
        Independently = 1, // Самостоятельно
        Emergency = 2,     // СМП
        Transferred = 3,   // Перевод из другой МО
        ProfileShift = 4   // Перевод внутри МО с другого профиля
    }

    /// <summary>
    /// Диспансерное наблюдение
    /// </summary>
    enum DispensarySupervision : int {
        None = 0,
        Observed = 1,   // состоит
        Taken = 2,      // взят
        NotSubject = 3, // не подлежит
        Recovery = 4,   // снят по причине выздоровления
        Cancelled = 6   // снят по другим причинам.
    }

    /// <summary>
    /// Сведения о случае (ZL_LIST/ZAP/Z_SL/SL)
    /// Может указываться несколько раз для случаев с внутрибольничным переводом при оплате по КСГ, обращениях по заболеваниям в амбулаторных условиях.
    /// <remarks>
    /// Вложен в
    ///     Recourse Z_SL (Multiple)
    /// Содержит
    ///     OnkologyTreat ONK_SL (Single)
    ///     ClinicalGroup KSG_KPG (Single)
    ///     Sanction SANK (Multiple)
    ///     Service USL (Multiple)
    /// </remarks>
    /// </summary>
    class Event {
        bool isOncology;

        public bool IsOncology { get { return isOncology; } }

        public List<Service> Services { get; set; }

        /// <summary>
        /// Идентификатор
        /// Уникально идентифицирует элемент SL в пределах законченного случая.
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// Подразделение МО
        /// Подразделение МО лечения из регионального справочника.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Код отделения
        /// Отделение МО лечения из регионального справочника.
        /// </summary>
        // public string Department { get; set; }

        /// <summary>
        /// Профиль медицинской помощи
        /// Классификатор V002 Приложения А.
        /// </summary>
        // public string Profile { get; set; }

        /// <summary>
        /// Профиль койки
        /// Классификатор V020 Приложения А.
        /// Обязательно к заполнению для стационара и дневного стационара.
        /// </summary>
        public string BedProfile { get; set; }

        /// <summary>
        /// Признак детского профиля
        /// 0 - нет, 1 - да.
        /// Заполняется в зависимости от профиля оказанной медицинской помощи.
        /// </summary>
        public bool Child { get; set; }

        /// <summary>
        /// Цель посещения
        /// Классификатор целей посещения V025 Приложения А. Обязательно к заполнению для амбулаторных условий.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Номер истории болезни/талона амбулаторного пациента/карты вызова скорой медицинской помощи
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Признак поступления/перевода
        /// Обязательно для дневного и круглосуточного стационара.
        /// </summary>
        public Transfer Transfer { get; set; }

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
        /// Диагноз первичный
        /// Код из справочника МКБ-10 до уровня подрубрики, если она предусмотрена МКБ-10 (неуказание подрубрики допускается для случаев оказания скорой медицинской помощи (USL_OK=4).
        /// Указывается при наличии
        /// </summary>
        public string PrimaryDiagnosis { get; set; }

        /// <summary>
        /// Диагноз основной
        /// Код из справочника МКБ-10 до уровня подрубрики, если она предусмотрена МКБ-10 (неуказание подрубрики допускается для случаев оказания скорой медицинской помощи).
        /// </summary>
        public string MainDiagnosis { get; set; }

        /// <summary>
        /// Установлен впервые (основной)
        /// Обязательно указывается "1", если основной диагноз выявлен впервые в результате проведенной диспансеризации/профилактического медицинского осмотра
        /// </summary>
        public bool FirstIdentified { get; set; }

        /// <summary>
        ///  Диагноз сопутствующего заболевания
        /// Код из справочника МКБ-10 до уровня подрубрики, если она предусмотрена МКБ-10 (неуказание подрубрики допускается для случаев оказания скорой медицинской помощи).
        /// Указывается в случае установления в соответствии с медицинской документацией.
        /// </summary>
        public List<string> ConcurrentDiagnoses { get; set; }

        /// <summary>
        /// Диагноз осложнения заболевания
        /// Код из справочника МКБ-10 до уровня подрубрики, если она предусмотрена МКБ-10 (неуказание подрубрики допускается для случаев оказания скорой медицинской помощи).
        /// Указывается в случае установления в соответствии с медицинской документацией.
        /// </summary>
        public List<string> ComplicationDiagnoses { get; set; }

        /// <summary>
        /// Признак подозрения на злокачественное новообразование
        /// Указывается "1" при подозрении на злокачественное новообразование.
        /// </summary>
        // public bool SuspectOncology { get; set; }

        /// <summary>
        /// Диспансерное наблюдение
        /// Указываются сведения о диспансерном наблюдении по поводу основного заболевания (состояния)
        /// Обязательно для заполнения, если P_CEL = 1.3
        /// </summary>
        public DispensarySupervision DispensarySupervision { get; set; }

        /// <summary>
        /// Код МЭС сопутствующего заболевания
        /// </summary>
        public string ConcurrentMesCode { get; set; }

        /// <summary>
        /// Сведения о КСГ/КПГ
        /// Заполняется при оплате случая лечения по КСГ или КПГ
        /// </summary>
        public ClinicalGroup ClinicalGroup { get; set; }

        /// <summary>
        /// Признак реабилитации
        /// Указывается значение "1" для случаев реабилитации
        /// </summary>
        public bool Rehabilitation { get; set; }

        /// <summary>
        /// Специальность лечащего врача/врача, закрывшего талон (историю болезни)
        /// Классификатор медицинских специальностей (Приложение А V021). Указывается значение IDSPEC
        /// </summary>
        public string SpecialityCode { get; set; }

        /// <summary>
        /// Код лечащего врача/врача, закрывшего талон (историю болезни)
        /// Территориальный справочник
        /// </summary>
        public string DoctorCode { get; set; }

        /// <summary>
        /// Количество единиц оплаты медицинской помощи
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Тариф
        /// Тариф с учетом всех примененных коэффициентов (при оплате случая по КСГ с внутрибольничным переводом - стоимость, рассчитанная в соответствии с Методическими рекомендациями по способам оплаты медицинской помощи за счет средств ОМС)
        /// </summary>
        public decimal Tariff { get; set; }

        /// <summary>
        /// Стоимость случая, выставленная к оплате
        /// Может указываться нулевое значение.
        /// Может состоять из тарифа и стоимости некоторых услуг.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Служебное поле
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Вид высокотехнологичной медицинской помощи
        /// Классификатор видов высокотехнологичной медицинской помощи. Справочник V018 Приложения А
        /// </summary>
        public string HiTechKind { get; set; }
        
        /// <summary>
        /// Метод высокотехнологичной медицинской помощи
        /// Классификатор методов высокотехнологичной медицинской помощи. Справочник V019 Приложения А
        /// </summary>
        public string HiTechMethod { get; set; }

        /// <summary>
        /// Дата выдачи талона на ВМП
        /// Заполняется на основании талона на ВМП
        /// </summary>
        public DateTime HiTechCheckDate { get; set; }

        /// <summary>
        /// Номер талона на ВМП
        /// </summary>
        public string HiTechCheckNumber { get; set; }

        /// <summary>
        /// Дата планируемой госпитализации
        /// </summary>
        public DateTime HiTechPlannedHospitalizationDate { get; set; }

        /// <summary>
        /// Save invoice event to XML
        /// </summary>
        /// <param name="xml">XML exporter to save into</param>
        /// <param name="pool">Datapool</param>
        /// <param name="section">Section of the order #59</param>
        /// <param name="irec">Invoice record to which this event belongs</param>
        public void Write(Lib.XmlExporter xml, Data.IInvoice pool, OrderSection section, InvoiceRecord irec, Recourse rec) {
            switch (section) {
                case OrderSection.D1:
                    WriteD1(xml, pool, irec, rec);
                    break;
                case OrderSection.D2:
                    WriteD2(xml, pool, irec, rec);
                    break;
                case OrderSection.D3:
                    WriteD3(xml, pool, irec, rec);
                    break;
            }
        }

        /// <summary>
        /// Save treatment case to XML
        /// </summary>
        /// <param name="xml">XML exporter to save into</param>
        /// <param name="pool">Datapool</param>
        /// <param name="irec">Invoice record to which this event belongs</param>
        public void WriteD1(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec, Recourse rec) {
            xml.Writer.WriteStartElement("SL");

            xml.Writer.WriteElementString("SL_ID", Identity);
            xml.WriteIfValid("LPU_1", Unit);
            xml.WriteIfValid("PODR", rec.Department);
            xml.Writer.WriteElementString("PROFIL", rec.Profile);
            xml.WriteIfValid("PROFIL_K", BedProfile);
            xml.WriteBool("DET", Child);
            xml.WriteIfValid("P_CEL", Reason);
            xml.Writer.WriteElementString("NHISTORY", CardNumber);

            if (Transfer != Model.Transfer.None)
                xml.Writer.WriteElementString("P_PER", ((int)Transfer).ToString());

            xml.Writer.WriteElementString("DATE_1", DateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_2", DateTill.AsXml());
            if (BedDays > 0) xml.Writer.WriteElementString("KD", BedDays.ToString());
       
            xml.WriteIfValid("DS0", PrimaryDiagnosis);
            xml.Writer.WriteElementString("DS1", MainDiagnosis);

            // Диагноз сопутствующего заболевания
            // Код из справочника МКБ-10 до уровня подрубрики, если она предусмотрена МКБ-10 (неуказание подрубрики допускается для случаев оказания скорой медицинской помощи).
            // Указывается в случае установления в соответствии с медицинской документацией.
            if (ConcurrentDiagnoses != null)
                foreach (string ds in ConcurrentDiagnoses)
                    xml.Writer.WriteElementString("DS2", ds);

            // Диагноз осложнения заболевания
            // Код из справочника МКБ-10 до уровня подрубрики, если она предусмотрена МКБ-10 (неуказание подрубрики допускается для случаев оказания скорой медицинской помощи).
            // Указывается в случае установления в соответствии с медицинской документацией.
            if (ComplicationDiagnoses != null)
                foreach (string ds in ComplicationDiagnoses)
                    xml.Writer.WriteElementString("DS3", ds);

            if (rec.SuspectOncology)
                xml.Writer.WriteElementString("DS_ONK", "1");

            if (DispensarySupervision != Model.DispensarySupervision.None)
                xml.Writer.WriteElementString("DN", ((int)DispensarySupervision).ToString());

            // Код МЭС
            // Классификатор МЭС. Указывается при наличии утвержденного стандарта.
            foreach(string mes in pool.LoadMesCodes(irec, rec, this))
                xml.Writer.WriteElementString("CODE_MES1", mes);

            xml.WriteIfValid("CODE_MES2", ConcurrentMesCode);

            isOncology = OnkologyTreat.IsOnkologyTreat(rec, this, pool);
            if (isOncology) {
                OnkologyTreat treat = pool.GetOnkologyTreat();
                if (treat != null) treat.Write(xml, pool);
            }

            if (ClinicalGroup != null) ClinicalGroup.Write(xml, pool, this);

            if (Rehabilitation)
                xml.Writer.WriteElementString("REAB", "1");

            xml.Writer.WriteElementString("PRVS", SpecialityCode);

            // Код классификатора медицинских специальностей
            // Указывается имя используемого классификатора медицинских специальностей
            xml.Writer.WriteElementString("VERS_SPEC", Options.SpecialityClassifier);

            xml.Writer.WriteElementString("IDDOKT", DoctorCode);

            if (Quantity > 0)
                xml.Writer.WriteElementString("ED_COL", Quantity.ToString("F2", Options.NumberFormat));

            if (Tariff > 0)
                xml.Writer.WriteElementString("TARIF", Tariff.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteElementString("SUM_M", Total.ToString("F2", Options.NumberFormat));

            // Сведения о санкциях
            // Описывает санкции, примененные в рамках данного случая.
            foreach (Sanction s in pool.LoadSanctions(irec, rec, this))
                s.Write(xml, pool, this);

            // Сведения об услуге
            // Описывает услуги, оказанные в рамках данного случая.
            // Допускается указание услуг с нулевой стоимостью.
            // Указание услуг с нулевой стоимостью обязательно, если условие их оказания является тарифообразующим (например, при оплате по КСГ).
            foreach (Service s in Services)
                s.WriteD1(xml, pool, irec, rec, this);

            xml.WriteIfValid("COMENTSL", Comment);
            xml.Writer.WriteEndElement();
        }

        /// <summary>
        /// Save hi-tech aid case to XML
        /// </summary>
        /// <param name="xml">XML exporter to save into</param>
        /// <param name="pool">Datapool</param>
        /// <param name="irec">Invoice record to which this event belongs</param>
        public void WriteD2(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec, Recourse rec) {
            xml.Writer.WriteStartElement("SL");
            xml.Writer.WriteElementString("SL_ID", Identity);
            xml.Writer.WriteElementString("VID_HMP", HiTechKind);
            xml.Writer.WriteElementString("METOD_HMP", HiTechMethod);
            xml.WriteIfValid("LPU_1", Unit);
            xml.WriteIfValid("PODR", rec.Department);
            xml.Writer.WriteElementString("PROFIL", rec.Profile);
            xml.WriteIfValid("PROFIL_K", BedProfile);
            xml.WriteBool("DET", Child);

            xml.Writer.WriteElementString("TAL_D", HiTechCheckDate.AsXml());
            xml.Writer.WriteElementString("TAL_NUM", HiTechCheckNumber);
            xml.Writer.WriteElementString("TAL_P", HiTechPlannedHospitalizationDate.AsXml());

            xml.Writer.WriteElementString("NHISTORY", CardNumber);
            xml.Writer.WriteElementString("DATE_1", DateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_2", DateTill.AsXml());
            xml.WriteIfValid("DS0", PrimaryDiagnosis);
            xml.Writer.WriteElementString("DS1", MainDiagnosis);

            if (ConcurrentDiagnoses != null)
                foreach (string ds in ConcurrentDiagnoses)
                    xml.Writer.WriteElementString("DS2", ds);

            if (ComplicationDiagnoses != null)
                foreach (string ds in ComplicationDiagnoses)
                    xml.Writer.WriteElementString("DS3", ds);

            if (rec.SuspectOncology)
                xml.Writer.WriteElementString("DS_ONK", "1");

            foreach (string mes in pool.LoadMesCodes(irec, rec, this))
                xml.Writer.WriteElementString("CODE_MES1", mes);

            xml.WriteIfValid("CODE_MES2", ConcurrentMesCode);

            isOncology = OnkologyTreat.IsOnkologyTreat(rec, this, pool);
            if (isOncology) {
                OnkologyTreat treat = pool.GetOnkologyTreat();
                if (treat != null) treat.Write(xml, pool);
            }

            xml.Writer.WriteElementString("PRVS", SpecialityCode);
            xml.Writer.WriteElementString("VERS_SPEC", Options.SpecialityClassifier);
            xml.Writer.WriteElementString("IDDOKT", DoctorCode);

            if (Quantity > 0)
                xml.Writer.WriteElementString("ED_COL", Quantity.ToString("F2", Options.NumberFormat));

            if (Tariff > 0)
                xml.Writer.WriteElementString("TARIF", Tariff.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteElementString("SUM_M", Total.ToString("F2", Options.NumberFormat));

            // Сведения о санкциях
            // Описывает санкции, примененные в рамках данного случая.
            foreach (Sanction s in pool.LoadSanctions(irec, rec, this))
                s.Write(xml, pool, this);

            // Сведения об услуге
            // Описывает услуги, оказанные в рамках данного случая.
            // Допускается указание услуг с нулевой стоимостью.
            // Указание услуг с нулевой стоимостью обязательно, если условие их оказания является тарифообразующим (например, при оплате по КСГ).
            foreach (Service s in Services)
                s.WriteD2(xml, pool, irec, rec, this);

            xml.WriteIfValid("COMENTSL", Comment);
            xml.Writer.WriteEndElement();
        }

        /// <summary>
        /// Save dispanserisation case to XML
        /// </summary>
        /// <param name="xml">XML exporter to save into</param>
        /// <param name="pool">Datapool</param>
        /// <param name="irec">Invoice record to which this event belongs</param>
        public void WriteD3(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec, Recourse rec) {
            xml.Writer.WriteStartElement("SL");
            xml.Writer.WriteElementString("SL_ID", Identity);
            xml.WriteIfValid("LPU_1", Unit);
            xml.Writer.WriteElementString("NHISTORY", CardNumber);
            xml.Writer.WriteElementString("DATE_1", DateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_2", DateTill.AsXml());
            xml.Writer.WriteElementString("DS1", MainDiagnosis);

            if (FirstIdentified)
                xml.Writer.WriteElementString("DS1_PR", "1");
            
            if (rec.SuspectOncology)
                xml.Writer.WriteElementString("DS_ONK", "1");

            xml.Writer.WriteElementString("PR_D_N", ((int)DispensarySupervision).ToString());

            foreach (ConcomitantDisease d in pool.GetConcomitantDiseases(irec, this))
                d.Write(xml);

            foreach (DispAssignment d in pool.GetDispanserisationAssignments(this))
                d.Write(xml);

            if (Quantity > 0)
                xml.Writer.WriteElementString("ED_COL", Quantity.ToString("F2", Options.NumberFormat));

            if (Tariff > 0)
                xml.Writer.WriteElementString("TARIF", Tariff.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteElementString("SUM_M", Total.ToString("F2", Options.NumberFormat));

            // Сведения о санкциях
            // Описывает санкции, примененные в рамках данного случая.
            foreach (Sanction s in pool.LoadSanctions(irec, rec, this))
                s.Write(xml, pool, this);

            // Сведения об услуге
            // Описывает услуги, оказанные в рамках данного случая.
            // Допускается указание услуг с нулевой стоимостью.
            // Указание услуг с нулевой стоимостью обязательно, если условие их оказания является тарифообразующим (например, при оплате по КСГ).
            foreach (Service s in Services)
                s.WriteD3(xml, pool, irec, rec, this);

            xml.WriteIfValid("COMENTSL", Comment);
            xml.Writer.WriteEndElement();
        }
    }
}
