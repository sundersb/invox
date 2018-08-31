using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Model {
    /// <summary>
    /// Записи о законченных случаях оказания медицинской помощи (SL_LIST/ZAP)
    /// <remarks>
    /// Вложен в
    ///     ZAP (Multiple)
    /// Содержит
    ///     InvoicePerson PACIENT (Single)
    ///     Recourse Z_SL (Single)
    /// </remarks>
    /// D1, D2, D3 - OK
    /// </summary>
    class InvoiceRecord {
        string id;
        bool revised;
        InvoicePerson person;
        Recourse recourse;

        /// <summary>
        /// Номер позиции записи
        /// Уникально идентифицирует запись в пределах счета.
        /// </summary>
        public string Identity { get { return id; } }
        
        /// <summary>
        /// Признак исправленной записи
        /// </summary>
        public bool Revised { get { return revised; } }

        public InvoicePerson Person { get { return person; } }
        public Recourse Recourse { get { return recourse; } }

        public void Write(Lib.XmlExporter xml, Data.IInvoice pool, OrderSection section) {
            xml.Writer.WriteStartElement("ZAP");

            xml.Writer.WriteElementString("N_ZAP", id);
            xml.WriteBool("PR_NOV", revised);

            person.Write(xml, pool, section, this);
            recourse.Write(xml, pool, section, this);

            xml.Writer.WriteEndElement();
        }
    }
}
