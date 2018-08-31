using System;
using invox.Lib;

namespace invox.Model {
    class Representative {
        string family;
        string name;
        string patronymic;
        int sex;

        DateTime birthDate;
        bool wrongDate;
        bool wrongMonth;
        
        /// <summary>
        /// Фамилия представителя пациента
        /// Заполняются данные о представителе пациента-ребенка до государственной регистрации рождения.
        /// Реквизиты указываются обязательно, если значение поля NOVOR отлично от нуля.
        /// FAM_P (фамилия представителя) и/или IM_P (имя представителя) указываются обязательно при наличии в документе УДЛ.
        /// В случае отсутствия кого-либо реквизита в документе УДЛ в поле DOST_P обязательно включается соответствующее значение, и реквизит не указывается.
        /// </summary>
        public string Family { get { return family; } }

        /// <summary>
        /// Имя представителя пациента
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// Отчество представителя пациента
        /// OT_P (отчество представителя) указывается при наличии в документе УДЛ. В случае отсутствия реквизит не указывается и в поле DOST_P можно опустить соответствующее значение.
        /// </summary>
        public string Patronymic { get { return patronymic; } }

        /// <summary>
        /// Пол представителя пациента
        /// </summary>
        public int Sex { get { return sex; } }

        /// <summary>
        /// Дата рождения представителя пациента
        /// Если в документе, удостоверяющем личность, не указан день рождения, то он принимается равным "01". При этом в поле DOST_P должно быть указано значение "4".
        /// Если в документе, удостоверяющем личность, не указан месяц рождения, то месяц рождения принимается равным "01" (январь). При этом в поле DOST_P должно быть указано значение "5".
        /// Если в документе, удостоверяющем личность, дата рождения не соответствует календарю, то из такой даты должны быть удалены ошибочные элементы и указана часть даты рождения с точностью до года или до месяца (как описано выше). При этом в поле DOST_P должно быть указано значение "6", а также значение "4" или "5" соответственно
        /// </summary>
        public DateTime BirthDate { get { return birthDate; } }

        /// <summary>
        /// Признак неправильной даты рождения (но известен месяц и год)
        /// </summary>
        public bool WrongDate { get { return wrongDate; } }

        /// <summary>
        /// Признак неверного месяца и даты рождения (год известен)
        /// </summary>
        public bool WrongMonth { get { return wrongMonth; } }

        void WritePersonIdentityError(Lib.XmlExporter xml) {
            if (string.IsNullOrEmpty(family)) xml.Writer.WriteElementString("DOST_P", "2");
            if (string.IsNullOrEmpty(name)) xml.Writer.WriteElementString("DOST_P", "3");
            if (string.IsNullOrEmpty(patronymic)) xml.Writer.WriteElementString("DOST_P", "1");
            if (wrongDate) {
                xml.Writer.WriteElementString("DOST_P", "6");
                xml.Writer.WriteElementString("DOST_P", "4");
            } else if (wrongMonth) {
                xml.Writer.WriteElementString("DOST_P", "6");
                xml.Writer.WriteElementString("DOST_P", "5");
            }
        }

        public void Write(XmlExporter xml, Data.IInvoice pool) {
            xml.WriteIfValid("FAM_P", family);
            xml.WriteIfValid("IM_P", name);
            xml.WriteIfValid("OT_P", patronymic);
            xml.Writer.WriteElementString("W_P", sex.ToString());
            xml.Writer.WriteElementString("DR_P", birthDate.AsXml());

            WritePersonIdentityError(xml);
        }
    }

    /// <summary>
    /// Содержит персональные данные пациента
    /// </summary>
    class Person {
        string id;
        string family;
        string name;
        string patronymic;
        int sex;

        DateTime birthDate;
        bool wrongDate;
        bool wrongMonth;

        string phone;

        Representative representative;

        string birthPlace;

        string doctype;
        string docSerial;
        string docNumber;

        string snils;
        string residenceOkato;
        string presenceOkato;
        string comment;

        /// <summary>
        /// Код записи о пациенте
        /// Соответствует аналогичному номеру в файле со сведениями счетов об оказанной медицинской помощи.
        /// </summary>
        public string ID { get { return id; } }

        /// <summary>
        /// Фамилия пациента
        /// FAM (фамилия) и/или IM (имя) указываются обязательно при наличии в документе УДЛ.
        /// В случае отсутствия кого-либо реквизита в документе УДЛ в поле DOST обязательно включается соответствующее значение, и реквизит не указывается.
        /// </summary>
        public string Family { get { return family; } }

        /// <summary>
        /// Имя пациента
        /// OT (отчество) указывается при наличии в документе УДЛ. В случае отсутствия реквизит не указывается и в поле DOST можно опустить соответствующее значение.
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// Отчество пациента
        /// Для детей при отсутствии данных ФИО до государственной регистрации не указываются. В этом случае значение поля NOVOR должно быть отлично от нуля.
        /// </summary>
        public string Patronymic { get { return patronymic; } }

        /// <summary>
        /// Пол пациента
        /// Заполняется в соответствии с классификатором V005 Приложения А.
        /// </summary>
        public int Sex { get { return sex; } }

        /// <summary>
        /// Дата рождения пациента
        /// Если в документе, удостоверяющем личность, не указан день рождения, то он принимается равным "01". При этом в поле DOST должно быть указано значение "4". Если в документе, удостоверяющем личность, не указан месяц рождения, то месяц рождения принимается равным "01" (январь). При этом в поле DOST должно быть указано значение "5".
        /// Если в документе, удостоверяющем личность, дата рождения не соответствует календарю, то из такой даты должны быть удалены ошибочные элементы и указана часть даты рождения с точностью до года или до месяца (как описано выше). При этом в поле DOST должно быть указано значение "6", а также значение "4" или "5" соответственно
        /// </summary>
        public DateTime BirthDate { get { return birthDate; } }

        /// <summary>
        /// Признак неправильной даты рождения (но известен месяц и год)
        /// </summary>
        public bool WrongDate { get { return wrongDate; } }

        /// <summary>
        /// Признак неверного месяца и даты рождения (год известен)
        /// </summary>
        public bool WrongMonth { get { return wrongMonth; } }

        /// <summary>
        /// Номер телефона пациента
        /// Указывается только для диспансеризации при предоставлении сведений.
        /// Информация для страхового представителя.
        /// </summary>
        public string Phone { get { return phone; } }

        /// <summary>
        /// Место рождения пациента или представителя
        /// Место рождения указывается в том виде, в котором оно записано в предъявленном документе, удостоверяющем личность.
        /// </summary>
        public string BirthPlace { get { return birthPlace; } }

        /// <summary>
        /// Тип документа, удостоверяющего личность пациента или представителя
        /// F011 "Классификатор типов документов, удостоверяющих личность".
        /// При указании ЕНП в соответствующем основном файле, поле может не заполняться.
        /// </summary>
        public string DocumentType { get { return doctype; } }

        /// <summary>
        /// Серия документа, удостоверяющего личность пациента или представителя
        /// При указании ЕНП в соответствующем основном файле, поле может не заполняться.
        /// </summary>
        public string DocumentSerial { get { return docSerial; } }

        /// <summary>
        /// Номер документа, удостоверяющего личность пациента или представителя
        /// При указании ЕНП в соответствующем основном файле, поле может не заполняться.
        /// </summary>
        public string DocumentNumber { get { return docNumber; } }

        /// <summary>
        /// СНИЛС пациента или представителя
        /// СНИЛС с разделителями. Указывается при наличии.
        /// </summary>
        public string Snils { get { return snils; } }

        /// <summary>
        /// Код места жительства по ОКАТО
        /// Заполняется при наличии сведений
        /// </summary>
        public string ResidenceOkato { get { return residenceOkato; } }

        /// <summary>
        /// Код места пребывания по ОКАТО
        /// Заполняется при наличии сведений
        /// </summary>
        public string PresenceOkato { get { return presenceOkato; } }

        /// <summary>
        /// Служебное поле
        /// </summary>
        public string Comment { get { return comment; } }

        void WritePersonIdentityError(Lib.XmlExporter xml) {
            if (string.IsNullOrEmpty(family)) xml.Writer.WriteElementString("DOST", "2");
            if (string.IsNullOrEmpty(name)) xml.Writer.WriteElementString("DOST", "3");
            if (string.IsNullOrEmpty(patronymic)) xml.Writer.WriteElementString("DOST", "1");
            if (wrongDate) {
                xml.Writer.WriteElementString("DOST", "6");
                xml.Writer.WriteElementString("DOST", "4");
            } else if (wrongMonth) {
                xml.Writer.WriteElementString("DOST", "6");
                xml.Writer.WriteElementString("DOST", "5");
            }
        }

        public void Write(Lib.XmlExporter xml, Data.IInvoice pool, OrderSection section) {
            xml.Writer.WriteStartElement("PERS");

            xml.WriteIfValid("ID_PAC", id);
            xml.WriteIfValid("FAM", family);
            xml.WriteIfValid("IM", name);
            xml.WriteIfValid("OT", patronymic);
            xml.Writer.WriteElementString("W", sex.ToString());
            xml.Writer.WriteElementString("DR", birthDate.AsXml());

            WritePersonIdentityError(xml);

            if (section == OrderSection.D3) xml.WriteIfValid("TEL", phone);

            if (representative != null) representative.Write(xml, pool);

            xml.WriteIfValid("MR", birthPlace);
            xml.WriteIfValid("DOCTYPE", doctype);
            xml.WriteIfValid("DOCSER", doctype);
            xml.WriteIfValid("DOCNUM", doctype);

            xml.WriteIfValid("SNILS", snils);
            xml.WriteIfValid("OKATOG", residenceOkato);
            xml.WriteIfValid("OKATOP", presenceOkato);
            xml.WriteIfValid("COMENTP", comment);

            xml.Writer.WriteEndElement();
        }
    }
}
