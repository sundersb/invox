using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using civox.Lib;

namespace civox.Model {
    class Invoice : Model {
        const string VERSION = "2.1";
        const int PROGRESS_WIDTH = 74;

        Lib.InvoiceNames invoiceNames;

        public Invoice(Lib.InvoiceNames files) {
            invoiceNames = files;
        }

        public override void Write(Lib.XmlExporter xml, Data.IInvoice repo) {
            if (!xml.OK) return;
            xml.Writer.WriteStartElement("ZL_LIST");

            // ZGLV record
            xml.Writer.WriteStartElement("ZGLV");
            xml.Writer.WriteElementString("VERSION", VERSION);
            xml.Writer.WriteElementString("DATA", DateTime.Today.AsXml());
            xml.Writer.WriteElementString("FILENAME", invoiceNames.InvoiceFileName);

            xml.Writer.WriteElementString("SD_Z", repo.GetRecourcesCount().ToString());
            xml.Writer.WriteEndElement();


            // SCHET record
            xml.Writer.WriteStartElement("SCHET");
            
            xml.Writer.WriteElementString("CODE", Options.InvoiceCode);
            xml.Writer.WriteElementString("CODE_MO", Options.LpuCode);
            xml.Writer.WriteElementString("YEAR", Options.Year.ToString());
            xml.Writer.WriteElementString("MONTH", Options.Month.ToString());
            xml.Writer.WriteElementString("NSCHET", Options.InvoiceNumber);

            // Invoice date
            // Get last working day of the report month
            DateTime date = new DateTime(Options.Year, Options.Month, 1);
            date = date.AddMonths(1).AddDays(-1);
            while (!date.IsWorkDay()) date = date.AddDays(-1);
            xml.Writer.WriteElementString("DSCHET", date.AsXml());

            xml.WriteIfValid("PLAT", invoiceNames.SmoCode);

            string dummy = string.Format(Options.NumberFormat, "{0:f2}", repo.TotalToPay());
            xml.Writer.WriteElementString("SUMMAV", dummy);

            xml.Writer.WriteEndElement();

            Lib.Progress progress = new Progress("Случаи обращения", repo.GetPeopleCount());
            foreach (InvoiceRecord irec in repo.LoadInvoiceRecords()) {
                irec.Write(xml, repo);
                progress.Step();
            }
            progress.Close();

            xml.Writer.WriteEndElement();
        }
    }
}
