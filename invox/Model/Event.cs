using System;
using invox.Lib;

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
    /// Сведения о случае (SL_LIST/ZAP/Z_SL/SL)
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
        string id;
        string unit;
        string dept;
        string profile;
        string bedProfile;
        bool child;
        string reason;
        string cardNumber;
        Transfer transfer;
        DateTime dateFrom;
        DateTime dateTill;
        int bedDays;
        string dsPrimary;
        string dsMain;
        bool firstIdentified;
        string dsConcurrent;
        string dsComplication;
        bool suspectOncology;
        DispensarySupervision dispSupervision;
        string concurrentMesCode;
        ClinicalGroup clinicalGroup;
        bool rehabilitation;
        string specialityCode;
        string doctorCode;
        double quantity;
        double tariff;
        double total;
        string comment;

        bool isOncology;

        /// <summary>
        /// Идентификатор
        /// Уникально идентифицирует элемент SL в пределах законченного случая.
        /// </summary>
        public string ID { get { return id; } }

        /// <summary>
        /// Подразделение МО
        /// Подразделение МО лечения из регионального справочника.
        /// </summary>
        public string Unit { get { return unit; } }

        /// <summary>
        /// Код отделения
        /// Отделение МО лечения из регионального справочника.
        /// </summary>
        public string Department { get { return dept; } }

        /// <summary>
        /// Профиль медицинской помощи
        /// Классификатор V002 Приложения А.
        /// </summary>
        public string Profile { get { return profile; } }

        /// <summary>
        /// Профиль койки
        /// Классификатор V020 Приложения А.
        /// Обязательно к заполнению для стационара и дневного стационара.
        /// </summary>
        public string BedProfile { get { return bedProfile; } }

        /// <summary>
        /// Признак детского профиля
        /// 0 - нет, 1 - да.
        /// Заполняется в зависимости от профиля оказанной медицинской помощи.
        /// </summary>
        public bool Child { get { return child; } }

        /// <summary>
        /// Цель посещения
        /// Классификатор целей посещения V025 Приложения А. Обязательно к заполнению для амбулаторных условий.
        /// </summary>
        public string Reason { get { return reason; } }

        /// <summary>
        /// Номер истории болезни/талона амбулаторного пациента/карты вызова скорой медицинской помощи
        /// </summary>
        public string CardNumber { get { return cardNumber; } }

        /// <summary>
        /// Признак поступления/перевода
        /// Обязательно для дневного и круглосуточного стационара.
        /// </summary>
        public Transfer Transfer { get { return transfer; } }

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
        /// Диагноз первичный
        /// Код из справочника МКБ-10 до уровня подрубрики, если она предусмотрена МКБ-10 (неуказание подрубрики допускается для случаев оказания скорой медицинской помощи (USL_OK=4).
        /// Указывается при наличии
        /// </summary>
        public string PrimaryDiagnosis { get { return dsPrimary; } }

        /// <summary>
        /// Диагноз основной
        /// Код из справочника МКБ-10 до уровня подрубрики, если она предусмотрена МКБ-10 (неуказание подрубрики допускается для случаев оказания скорой медицинской помощи).
        /// </summary>
        public string MainDiagnosis { get { return dsMain; } }

        /// <summary>
        /// Установлен впервые (основной)
        /// Обязательно указывается "1", если основной диагноз выявлен впервые в результате проведенной диспансеризации/профилактического медицинского осмотра
        /// </summary>
        public bool FirstIdentified { get { return firstIdentified; } }

        /// <summary>
        ///  Диагноз сопутствующего заболевания
        /// Код из справочника МКБ-10 до уровня подрубрики, если она предусмотрена МКБ-10 (неуказание подрубрики допускается для случаев оказания скорой медицинской помощи).
        /// Указывается в случае установления в соответствии с медицинской документацией.
        /// </summary>
        public string ConcurrentDiagnosis { get { return dsConcurrent; } }

        /// <summary>
        /// Диагноз осложнения заболевания
        /// Код из справочника МКБ-10 до уровня подрубрики, если она предусмотрена МКБ-10 (неуказание подрубрики допускается для случаев оказания скорой медицинской помощи).
        /// Указывается в случае установления в соответствии с медицинской документацией.
        /// </summary>
        public string ComplicationDiagnosis { get { return dsComplication; } }

        /// <summary>
        /// Признак подозрения на злокачественное новообразование
        /// Указывается "1" при подозрении на злокачественное новообразование.
        /// </summary>
        public bool SuspectOncology { get { return suspectOncology; } }

        /// <summary>
        /// Диспансерное наблюдение
        /// Указываются сведения о диспансерном наблюдении по поводу основного заболевания (состояния)
        /// Обязательно для заполнения, если P_CEL = 1.3
        /// </summary>
        public DispensarySupervision DispensarySupervision { get { return dispSupervision; } }

        /// <summary>
        /// Код МЭС сопутствующего заболевания
        /// </summary>
        public string ConcurrentMesCode { get { return concurrentMesCode; } }

        /// <summary>
        /// Сведения о КСГ/КПГ
        /// Заполняется при оплате случая лечения по КСГ или КПГ
        /// </summary>
        public ClinicalGroup ClinicalGroup { get { return clinicalGroup; } }

        /// <summary>
        /// Признак реабилитации
        /// Указывается значение "1" для случаев реабилитации
        /// </summary>
        public bool Rehabilitation { get { return rehabilitation; } }

        /// <summary>
        /// Специальность лечащего врача/врача, закрывшего талон (историю болезни)
        /// Классификатор медицинских специальностей (Приложение А V021). Указывается значение IDSPEC
        /// </summary>
        public string SpecialityCode { get { return specialityCode; } }

        /// <summary>
        /// Код лечащего врача/врача, закрывшего талон (историю болезни)
        /// Территориальный справочник
        /// </summary>
        public string DoctorCode { get { return doctorCode; } }

        /// <summary>
        /// Количество единиц оплаты медицинской помощи
        /// </summary>
        public double Quantity { get { return quantity; } }

        /// <summary>
        /// Тариф
        /// Тариф с учетом всех примененных коэффициентов (при оплате случая по КСГ с внутрибольничным переводом - стоимость, рассчитанная в соответствии с Методическими рекомендациями по способам оплаты медицинской помощи за счет средств ОМС)
        /// </summary>
        public double Tariff { get { return tariff; } }

        /// <summary>
        /// Стоимость случая, выставленная к оплате
        /// Может указываться нулевое значение.
        /// Может состоять из тарифа и стоимости некоторых услуг.
        /// </summary>
        public double Total { get { return total; } }

        /// <summary>
        /// Служебное поле
        /// </summary>
        public string Comment { get { return comment; } }

        public void Write(Lib.XmlExporter xml, Data.IInvoice pool, OrderSection section, InvoiceRecord irec) {
            xml.Writer.WriteStartElement("SL");
            xml.Writer.WriteElementString("SL_ID", id);
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

            if (quantity > 0)
                xml.Writer.WriteElementString("ED_COL", quantity.ToString("F2", Options.NumberFormat));

            if (tariff > 0)
                xml.Writer.WriteElementString("ED_COL", tariff.ToString("F2", Options.NumberFormat));

            xml.Writer.WriteElementString("SUM_M", total.ToString("F2", Options.NumberFormat));

            // Сведения о санкциях
            // Описывает санкции, примененные в рамках данного случая.
            foreach (Sanction s in pool.LoadSanctions())
                s.Write(xml, pool, this);

            // Сведения об услуге
            // Описывает услуги, оказанные в рамках данного случая.
            // Допускается указание услуг с нулевой стоимостью.
            // Указание услуг с нулевой стоимостью обязательно, если условие их оказания является тарифообразующим (например, при оплате по КСГ).
            foreach (Service s in pool.LoadServices()) {
                s.Oncology = isOncology;
                s.SuspectOncology = suspectOncology;
                s.Write(xml, pool, section, irec, this);
            }

            xml.WriteIfValid("COMENTSL", comment);
            xml.Writer.WriteEndElement();
        }

        void WriteD1 (Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec) {
            xml.WriteIfValid("LPU_1", unit);
            xml.WriteIfValid("PODR", dept);
            xml.Writer.WriteElementString("PROFIL", profile);
            xml.WriteIfValid("PROFIL_K", bedProfile);
            xml.WriteBool("DET", child);
            xml.WriteIfValid("P_CEL", reason);
            xml.Writer.WriteElementString("NHISTORY", cardNumber);

            if (transfer != Model.Transfer.None)
                xml.Writer.WriteElementString("P_PER", ((int)transfer).ToString());

            xml.Writer.WriteElementString("DATE_1", dateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_2", dateTill.AsXml());
            if (bedDays > 0) xml.Writer.WriteElementString("KD", bedDays.ToString());
       
            xml.WriteIfValid("DS0", dsPrimary);
            xml.Writer.WriteElementString("DS1", dsMain);

            // Диагноз сопутствующего заболевания
            // Код из справочника МКБ-10 до уровня подрубрики, если она предусмотрена МКБ-10 (неуказание подрубрики допускается для случаев оказания скорой медицинской помощи).
            // Указывается в случае установления в соответствии с медицинской документацией.
            foreach(string ds in pool.LoadConcurrentDiagnoses())
                xml.Writer.WriteElementString("DS2", ds);

            // Диагноз осложнения заболевания
            // Код из справочника МКБ-10 до уровня подрубрики, если она предусмотрена МКБ-10 (неуказание подрубрики допускается для случаев оказания скорой медицинской помощи).
            // Указывается в случае установления в соответствии с медицинской документацией.
            foreach(string ds in pool.LoadComplicationDiagnoses())
                xml.Writer.WriteElementString("DS3", ds);

            if (suspectOncology)
                xml.Writer.WriteElementString("DS_ONK", "1");

            if (dispSupervision != Model.DispensarySupervision.None)
                xml.Writer.WriteElementString("DN", ((int)dispSupervision).ToString());

            // Код МЭС
            // Классификатор МЭС. Указывается при наличии утвержденного стандарта.
            foreach(string mes in pool.LoadMesCodes())
                xml.Writer.WriteElementString("CODE_MES1", mes);

            xml.WriteIfValid("CODE_MES2", concurrentMesCode);

            isOncology = OnkologyTreat.IsOnkologyTreat(this, pool);
            if (isOncology) {
                OnkologyTreat treat = pool.GetOnkologyTreat();
                if (treat != null) treat.Write(xml, pool, OrderSection.D1);
            }

            if (clinicalGroup != null) clinicalGroup.Write(xml, pool, this);

            if (rehabilitation)
                xml.Writer.WriteElementString("REAB", "1");

            xml.Writer.WriteElementString("PRVS", specialityCode);

            // Код классификатора медицинских специальностей
            // Указывается имя используемого классификатора медицинских специальностей
            xml.Writer.WriteElementString("VERS_SPEC", Options.SpecialityClassifier);

            xml.Writer.WriteElementString("IDDOKT", doctorCode);
        }

        void WriteD2(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec) {
        }

        void WriteD3(Lib.XmlExporter xml, Data.IInvoice pool, InvoiceRecord irec) {
            xml.WriteIfValid("LPU_1", unit);
            xml.Writer.WriteElementString("NHISTORY", cardNumber);
            xml.Writer.WriteElementString("DATE_1", dateFrom.AsXml());
            xml.Writer.WriteElementString("DATE_2", dateTill.AsXml());
            xml.Writer.WriteElementString("DS1", dsMain);

            if (firstIdentified)
                xml.Writer.WriteElementString("DS1_PR", "1");
            
            if (suspectOncology)
                xml.Writer.WriteElementString("DS_ONK", "1");

            xml.Writer.WriteElementString("PR_D_N", ((int)dispSupervision).ToString());

        }
    }
}
