using System;
using invox.Lib;

namespace invox.Model {
    class Representative {
        /// <summary>
        /// Фамилия представителя пациента
        /// Заполняются данные о представителе пациента-ребенка до государственной регистрации рождения.
        /// Реквизиты указываются обязательно, если значение поля NOVOR отлично от нуля.
        /// FAM_P (фамилия представителя) и/или IM_P (имя представителя) указываются обязательно при наличии в документе УДЛ.
        /// В случае отсутствия кого-либо реквизита в документе УДЛ в поле DOST_P обязательно включается соответствующее значение, и реквизит не указывается.
        /// </summary>
        public string Family { get; set; }

        /// <summary>
        /// Имя представителя пациента
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отчество представителя пациента
        /// OT_P (отчество представителя) указывается при наличии в документе УДЛ. В случае отсутствия реквизит не указывается и в поле DOST_P можно опустить соответствующее значение.
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Пол представителя пациента
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// Дата рождения представителя пациента
        /// Если в документе, удостоверяющем личность, не указан день рождения, то он принимается равным "01". При этом в поле DOST_P должно быть указано значение "4".
        /// Если в документе, удостоверяющем личность, не указан месяц рождения, то месяц рождения принимается равным "01" (январь). При этом в поле DOST_P должно быть указано значение "5".
        /// Если в документе, удостоверяющем личность, дата рождения не соответствует календарю, то из такой даты должны быть удалены ошибочные элементы и указана часть даты рождения с точностью до года или до месяца (как описано выше). При этом в поле DOST_P должно быть указано значение "6", а также значение "4" или "5" соответственно
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Признак неправильной даты рождения (но известен месяц и год)
        /// </summary>
        public bool WrongDate { get; set; }

        /// <summary>
        /// Признак неверного месяца и даты рождения (год известен)
        /// </summary>
        public bool WrongMonth { get; set; }

        void WritePersonIdentityError(Lib.XmlExporter xml) {
            if (string.IsNullOrEmpty(Family)) xml.Writer.WriteElementString("DOST_P", "2");
            if (string.IsNullOrEmpty(Name)) xml.Writer.WriteElementString("DOST_P", "3");
            if (string.IsNullOrEmpty(Patronymic)) xml.Writer.WriteElementString("DOST_P", "1");
            if (WrongDate) {
                xml.Writer.WriteElementString("DOST_P", "6");
                xml.Writer.WriteElementString("DOST_P", "4");
            } else if (WrongMonth) {
                xml.Writer.WriteElementString("DOST_P", "6");
                xml.Writer.WriteElementString("DOST_P", "5");
            }
        }

        public void Write(XmlExporter xml, Data.IInvoice pool) {
            xml.WriteIfValid("FAM_P", Family);
            xml.WriteIfValid("IM_P", Name);
            xml.WriteIfValid("OT_P", Patronymic);
            xml.Writer.WriteElementString("W_P", Sex.ToString());
            xml.Writer.WriteElementString("DR_P", BirthDate.AsXml());

            WritePersonIdentityError(xml);
        }
    }

    /// <summary>
    /// Содержит персональные данные пациента
    /// </summary>
    class Person {
        static char[] SEPARATORS = " \r\n\t".ToCharArray();

        /// <summary>
        /// Код записи о пациенте
        /// Соответствует аналогичному номеру в файле со сведениями счетов об оказанной медицинской помощи.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Фамилия пациента
        /// FAM (фамилия) и/или IM (имя) указываются обязательно при наличии в документе УДЛ.
        /// В случае отсутствия кого-либо реквизита в документе УДЛ в поле DOST обязательно включается соответствующее значение, и реквизит не указывается.
        /// </summary>
        public string Family { get; set; }

        /// <summary>
        /// Имя пациента
        /// OT (отчество) указывается при наличии в документе УДЛ. В случае отсутствия реквизит не указывается и в поле DOST можно опустить соответствующее значение.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отчество пациента
        /// Для детей при отсутствии данных ФИО до государственной регистрации не указываются. В этом случае значение поля NOVOR должно быть отлично от нуля.
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Пол пациента
        /// Заполняется в соответствии с классификатором V005 Приложения А.
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// Дата рождения пациента
        /// Если в документе, удостоверяющем личность, не указан день рождения, то он принимается равным "01". При этом в поле DOST должно быть указано значение "4". Если в документе, удостоверяющем личность, не указан месяц рождения, то месяц рождения принимается равным "01" (январь). При этом в поле DOST должно быть указано значение "5".
        /// Если в документе, удостоверяющем личность, дата рождения не соответствует календарю, то из такой даты должны быть удалены ошибочные элементы и указана часть даты рождения с точностью до года или до месяца (как описано выше). При этом в поле DOST должно быть указано значение "6", а также значение "4" или "5" соответственно
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Признак неправильной даты рождения (но известен месяц и год)
        /// </summary>
        public bool WrongDate { get; set; }

        /// <summary>
        /// Признак неверного месяца и даты рождения (год известен)
        /// </summary>
        public bool WrongMonth { get; set; }

        /// <summary>
        /// Номер телефона пациента
        /// Указывается только для диспансеризации при предоставлении сведений.
        /// Информация для страхового представителя.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Место рождения пациента или представителя
        /// Место рождения указывается в том виде, в котором оно записано в предъявленном документе, удостоверяющем личность.
        /// </summary>
        public string BirthPlace { get; set; }

        /// <summary>
        /// Тип документа, удостоверяющего личность пациента или представителя
        /// F011 "Классификатор типов документов, удостоверяющих личность".
        /// При указании ЕНП в соответствующем основном файле, поле может не заполняться.
        /// </summary>
        public string DocumentType { get; set; }

        /// <summary>
        /// Серия документа, удостоверяющего личность пациента или представителя
        /// При указании ЕНП в соответствующем основном файле, поле может не заполняться.
        /// </summary>
        public string DocumentSerial { get; set; }

        /// <summary>
        /// Номер документа, удостоверяющего личность пациента или представителя
        /// При указании ЕНП в соответствующем основном файле, поле может не заполняться.
        /// </summary>
        public string DocumentNumber { get; set; }

        /// <summary>
        /// Дата выдачи документа. Обязательна для иногородних, у которых не указан ЕНП
        /// </summary>
        public DateTime? DocumentDate { get; set; }

        /// <summary>
        /// Орган, выдавший документ. Обязателен для иногородних, у которых не указан ЕНП
        /// </summary>
        public string DocumentOrganization { get; set; }

        /// <summary>
        /// СНИЛС пациента или представителя
        /// СНИЛС с разделителями. Указывается при наличии.
        /// </summary>
        public string Snils { get; set; }

        /// <summary>
        /// Код места жительства по ОКАТО
        /// Заполняется при наличии сведений. По ФОМС - обязательное
        /// </summary>
        public string ResidenceOkato { get; set; }

        /// <summary>
        /// Код места пребывания по ОКАТО
        /// Заполняется при наличии сведений
        /// </summary>
        public string PresenceOkato { get; set; }

        /// <summary>
        /// Служебное поле
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Законный представитель пациента
        /// </summary>
        public Representative Representative { get; set; }

        /// <summary>
        /// ФОМС: Адрес
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// ФОМС: Социальное положение
        /// </summary>
        public string SocialPosition { get; set; }
        
        /// <summary>
        /// ФОМС: Код льготы
        /// </summary>
        public string SocialFavour { get; set; }

        void WritePersonIdentityError(Lib.XmlExporter xml) {
            if (string.IsNullOrEmpty(Family)) xml.Writer.WriteElementString("DOST", "2");
            if (string.IsNullOrEmpty(Name)) xml.Writer.WriteElementString("DOST", "3");
            if (string.IsNullOrEmpty(Patronymic)) xml.Writer.WriteElementString("DOST", "1");
            if (WrongDate) {
                xml.Writer.WriteElementString("DOST", "6");
                xml.Writer.WriteElementString("DOST", "4");
            } else if (WrongMonth) {
                xml.Writer.WriteElementString("DOST", "6");
                xml.Writer.WriteElementString("DOST", "5");
            }
        }

        public void Write(Lib.XmlExporter xml, Data.IInvoice pool, OrderSection section) {
            xml.Writer.WriteStartElement("PERS");

            xml.WriteIfValid("ID_PAC", ID);
            xml.WriteIfValid("FAM", Family);
            xml.WriteIfValid("IM", Name);
            xml.WriteIfValid("OT", Patronymic);
            xml.Writer.WriteElementString("W", Sex.ToString());
            xml.Writer.WriteElementString("DR", BirthDate.AsXml());

            WritePersonIdentityError(xml);

            if (section == OrderSection.D3) xml.WriteIfValid("TEL", Phone);

            if (Representative != null) Representative.Write(xml, pool);
                        
            // FOMS
            //xml.WriteIfValid("MR", Address);
            xml.WriteIfValid("MR", BirthPlace);

            xml.WriteIfValid("DOCTYPE", DocumentType);
            
#if FOMS
            xml.WriteIfValid("SOC", SocialPosition);
#endif

            xml.WriteIfValid("DOCSER", DocumentSerial);
            xml.WriteIfValid("DOCNUM", DocumentNumber);
#if FOMS
            if (DocumentDate.HasValue)
                xml.Writer.WriteElementString("DOCDATE", DocumentDate.Value.AsXml());
            xml.WriteIfValid("DOCORG", DocumentOrganization);
#endif

            xml.WriteIfValid("SNILS", Snils);
#if FOMS
            xml.Writer.WriteElementString("OKATOG", ResidenceOkato);
#else
            xml.WriteIfValid("OKATOG", ResidenceOkato);
#endif
            xml.WriteIfValid("OKATOP", PresenceOkato);

#if FOMS
            xml.WriteIfValid("KT", SocialFavour);
#endif

            xml.WriteIfValid("COMENTP", Comment);

            xml.Writer.WriteEndElement();
        }

        public void SetDocument(string number) {
            // TODO: Форматирование с учетом типа документа F011
            number = number.Trim();
            if (string.IsNullOrEmpty(number)) {
                DocumentNumber = string.Empty;
                DocumentSerial = string.Empty;
            } else {
                string[] parts = number.Split(SEPARATORS);
                int l = parts.Length - 1;
                DocumentNumber = parts[l];

                if (l > 0) {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(parts[0]);
                    for (int i = 1; i < l; ++i)
                        sb.Append(parts[i]);
                    DocumentSerial = sb.ToString();
                } else {
                    DocumentSerial = string.Empty;
                }
            }
        }
    }
}
