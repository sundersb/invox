using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// Комментарий
        /// </summary>
        public string Comment { get { return comment; } }

        public void Write(Lib.XmlExporter xml, Data.IInvoice pool, Event evt) {
            xml.Writer.WriteStartElement("SANK");

            xml.Writer.WriteElementString("S_CODE", id);
            xml.Writer.WriteElementString("S_SUM", total.ToString("C2", Options.NumberFormat));

            xml.Writer.WriteElementString("S_TIP", controlCode);
            xml.Writer.WriteElementString("S_OSN", reasonCode);
            xml.Writer.WriteElementString("S_COM", comment);

            // Источник: 1 - СМО/ТФОМС к МО.
            xml.Writer.WriteElementString("S_IST", "1");

            xml.Writer.WriteEndElement();
        }
    }
}
