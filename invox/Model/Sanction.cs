using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Lib;

namespace invox.Model {
    /// <summary>
    /// Сведения о санкциях (ZL_LIST/ZAP/Z_SL/SL/SANK)
    /// Описывает санкции, примененные в рамках данного случая.
    /// <remarks>
    /// Вложен в
    ///     Event SL (Multiple)
    /// </remarks>
    /// </summary>
    class Sanction {
        string id;
        double total;
        
        string controlCode;
        string reasonCode;
        string comment;
        string[] events;
        DateTime actDate;
        string actNumber;
        string expertCode;

        /// <summary>
        /// Идентификатор санкции
        /// Уникален в пределах случая.
        /// </summary>
        public string Identity { get { return id; } }
        
        /// <summary>
        /// Финансовая санкция
        /// </summary>
        public double Total { get { return total; } }

        /// <summary>
        /// Идентификатор случая
        /// Идентификатор случая, в котором выявлена причина для отказа (частичной) оплаты, в пределах законченного случая.
        /// Обязательно к заполнению, если S_SUM не равна 0
        /// </summary>
        public string[] EventID { get { return events; } }

        /// <summary>
        /// Код вида контроля
        /// F006 Классификатор видов контроля, Приложение А
        /// </summary>
        public string ControlCode { get { return controlCode; } }

        /// <summary>
        /// Код причины отказа (частичной) оплаты
        /// F014 Классификатор причин отказа в оплате медицинской помощи, Приложение А
        /// </summary>
        public string ReasonCode { get { return reasonCode; } }

        /// <summary>
        /// Дата акта МЭК, МЭЭ или ЭКМП
        /// </summary>
        public DateTime ActDate { get { return actDate; } }
        
        /// <summary>
        /// Номер акта МЭК, МЭЭ или ЭКМП
        /// </summary>
        public string ActNumber { get { return actNumber; } }
        
        /// <summary>
        /// Код эксперта качества медицинской помощи
        /// Обязательно к заполнению в соответствии с F004 (Реестр экспертов
        ///  качества медицинской помощи, Приложение А)
        ///  для экспертиз качества медицинской помощи (S_TIP>=30)
        /// </summary>
        public string ExpertCode { get { return expertCode; } }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get { return comment; } }

        public void Write(Lib.XmlExporter xml, Data.IInvoice pool) {
            xml.Writer.WriteStartElement("SANK");

            xml.Writer.WriteElementString("S_CODE", id);
            xml.Writer.WriteElementString("S_SUM", total.ToString("C2", Options.NumberFormat));
            xml.Writer.WriteElementString("S_TIP", controlCode);

            foreach (string sl in events)
                xml.Writer.WriteElementString("SL_ID", sl);

            xml.Writer.WriteElementString("S_OSN", reasonCode);

            xml.Writer.WriteElementString("DATE_ACT", actDate.AsXml());
            xml.Writer.WriteElementString("NUM_ACT", actNumber);
            xml.Writer.WriteElementString("CODE_EXP", expertCode);

            xml.Writer.WriteElementString("S_COM", comment);

            // Источник: 1 - СМО/ТФОМС к МО.
            xml.Writer.WriteElementString("S_IST", "1");

            xml.Writer.WriteEndElement();
        }
    }
}
