using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using civox.Lib;

namespace civox.Model {
    class Invoice : Model {
        const string VERSION = "2.1";

        Lib.InvoiceNames invoiceNames;

        public Invoice(Lib.InvoiceNames files) {
            invoiceNames = files;
        }

        public override void Write(Lib.XmlExporter xml, Data.IDataProvider provider) {
            if (!xml.OK) return;
            xml.Writer.WriteStartElement("ZL_LIST");

            // ZGLV record
            xml.Writer.WriteStartElement("ZGLV");
            xml.Writer.WriteElementString("VERSION", VERSION);
            xml.Writer.WriteElementString("DATA", DateTime.Today.AsXml());
            xml.Writer.WriteElementString("FILENAME", invoiceNames.InvoiceFileName);
            // TODO: Number of ZAP
            xml.Writer.WriteElementString("SD_Z", string.Empty);
            xml.Writer.WriteEndElement();


            // SCHET record
            xml.Writer.WriteStartElement("SCHET");
            
            // TODO: Invoice code
            xml.Writer.WriteElementString("CODE", string.Empty);
            xml.Writer.WriteElementString("CODE_MO", Options.LpuCode);
            xml.Writer.WriteElementString("YEAR", Options.Year.ToString());
            xml.Writer.WriteElementString("MONTH", Options.Month.ToString());
            // TODO: Invoice number
            xml.Writer.WriteElementString("NSCHET", string.Empty);
            // TODO: Invoice date
            xml.Writer.WriteElementString("DSCHET", DateTime.Today.AsXml());
            // TODO: Invoice SMO - unnecessary
            xml.Writer.WriteElementString("PLAT", string.Empty);
            // TODO: Dot decimal separator
            string dummy = string.Format ("{0:f2}", provider.GetInvoiceRepository().TotalToPay());
            xml.Writer.WriteElementString("SUMMAV", dummy);
            xml.Writer.WriteEndElement();

            foreach (InvoiceRecord irec in provider.GetInvoiceRepository().LoadInvoiceRecords())
                irec.Write(xml, provider);

            xml.Writer.WriteEndElement();
        }
    }
}
