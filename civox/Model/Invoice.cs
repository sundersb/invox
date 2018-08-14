using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using civox.Lib;

namespace civox.Model {
    class Invoice : Model {

#if !NO59
        // Приказ 59 ФФОМС от 30.03.2018 (онкология) = "3.1";
        const string VERSION = "3.1";
#else
        const string VERSION = "2.1";
#endif

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
            xml.Writer.WriteElementString("DSCHET", Options.InvoiceDate.AsXml());

            xml.WriteIfValid("PLAT", invoiceNames.SmoCode);

            // TODO: Где ее брать, если финансирование подушевое?
            string dummy = string.Format(Options.NumberFormat, "{0:f2}", repo.TotalToPay());
            xml.Writer.WriteElementString("SUMMAV", dummy);

            // DISP     У Тип диспансеризации V016. И как это, если в файле счета случаи любого повода обращения?

            xml.Writer.WriteEndElement();

#if DEBUG
            int c = Properties.Settings.Default.DebugSelectionLimit;
            Lib.Progress progress = new Progress("Случаи обращения", c);
            foreach (InvoiceRecord irec in repo.LoadInvoiceRecords()) {
                irec.Write(xml, repo);
                progress.Step();
                if (--c <= 0) break;
            }
            progress.Close();
#else
            Lib.Progress progress = new Progress("Случаи обращения", repo.GetPeopleCount());
            foreach (InvoiceRecord irec in repo.LoadInvoiceRecords()) {
                irec.Write(xml, repo);
                progress.Step();
            }
            progress.Close();
#endif

            xml.Writer.WriteEndElement();
        }
    }
}
