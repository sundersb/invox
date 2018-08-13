using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using civox.Lib;

namespace civox.Model {
    /// <summary>
    /// People file model
    /// </summary>
    class People : Model {
        // Приказ 59 ФФОМС от 30.03.2018 (онкология) - про версию в части Д.4 не сказано ни слова. Вероятно д/б тоже 3.1
        const string VERSION = "3.1";// "2.1";

        Lib.InvoiceNames invoiceNames;

        public People(Lib.InvoiceNames files) {
            invoiceNames = files;
        }

        /// <summary>
        /// Export to XML
        /// </summary>
        /// <param name="xml">XML write helper</param>
        public override void Write(Lib.XmlExporter xml, Data.IInvoice repo) {
            if (!xml.OK) return;
            xml.Writer.WriteStartElement("PERS_LIST");

            xml.Writer.WriteStartElement("ZGLV");

            xml.Writer.WriteElementString("VERSION", VERSION);
            xml.Writer.WriteElementString("DATA", DateTime.Today.AsXml());
            xml.Writer.WriteElementString("FILENAME", invoiceNames.PeopleFileName);
            xml.Writer.WriteElementString("FILENAME1", invoiceNames.InvoiceFileName);

            xml.Writer.WriteEndElement();

#if DEBUG
            int count = Properties.Settings.Default.DebugSelectionLimit;
            Lib.Progress progress = new Progress("Пациенты", count);
            foreach (Person p in repo.LoadPeople()) {
                p.Write(xml, null);
                progress.Step();
                if (--count <= 0) break;
            }
            progress.Close();
#else
            int count = repo.GetPeopleCount();
            Lib.Progress progress = new Progress("Пациенты", count);
            foreach (Person p in repo.LoadPeople()) {
                p.Write(xml, null);
                progress.Step();
            }
            progress.Close();
#endif

            xml.Writer.WriteEndElement();
        }
    }
}
