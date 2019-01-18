using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Model {
    /// <summary>
    /// Записи о законченных случаях оказания медицинской помощи (ZL_LIST/ZAP)
    /// <remarks>
    /// Вложен в
    ///     ZL_LIST (Multiple)
    /// Содержит
    ///     InvoicePerson PACIENT (Single)
    ///     Recourse Z_SL (Single)
    /// </remarks>
    /// D1, D2, D3 - OK
    /// </summary>
    class InvoiceRecord {
        /// <summary>
        /// Номер позиции записи
        /// Уникально идентифицирует запись в пределах счета.
        /// </summary>
        public int Identity { get; set; }
        
        /// <summary>
        /// Признак исправленной записи
        /// </summary>
        public bool Revised { get; set; }

        public InvoicePerson Person { get; set; }

        /// <summary>
        /// Save invoice record to XML
        /// </summary>
        /// <param name="xml">XML exporter to use</param>
        /// <param name="pool">Datapool</param>
        /// <param name="section">Order #59 section</param>
        public void Write(Lib.XmlExporter xml, Action onRecourse, Data.IInvoice pool, OrderSection section) {
            foreach (Recourse recourse in pool.LoadRecourses(this, section)) {
                ++Identity;
                xml.Writer.WriteStartElement("ZAP");
                xml.Writer.WriteElementString("N_ZAP", Identity.ToString());
                xml.WriteBool("PR_NOV", Revised);
                Person.Write(xml, section);
                recourse.Write(xml, pool, section, this);
                xml.Writer.WriteEndElement();
                if (onRecourse != null) onRecourse();
            }
        }
    }
}
