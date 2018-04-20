﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using civox.Lib;

namespace civox.Model {
    /// <summary>
    /// People file model
    /// </summary>
    class People : Model {
        const string VERSION = "2.1";

        Lib.InvoiceNames invoiceNames;

        public People(Lib.InvoiceNames files) {
            invoiceNames = files;
        }

        /// <summary>
        /// Export to XML
        /// </summary>
        /// <param name="xml">XML write helper</param>
        public override void Write(Lib.XmlExporter xml, Data.IDataProvider provider) {
            if (!xml.OK) return;
            xml.Writer.WriteStartElement("PERS_LIST");

            xml.Writer.WriteStartElement("ZGLV");

            xml.Writer.WriteElementString("VERSION", VERSION);
            xml.Writer.WriteElementString("DATA", DateTime.Today.AsXml());
            xml.Writer.WriteElementString("FILENAME", invoiceNames.PeopleFileName);
            xml.Writer.WriteElementString("FILENAME1", invoiceNames.InvoiceFileName);

            xml.Writer.WriteEndElement();

            foreach (Person p in provider.GetInvoiceRepository().LoadPeople()) p.Write(xml, null);

            xml.Writer.WriteEndElement();
        }
    }
}