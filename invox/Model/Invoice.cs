using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Lib;

namespace invox.Model {
    class Invoice {
        const string XML = ".xml";

        const string VERSION = "3.1";

        Lib.InvoiceFilename invoiceFilename;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="files">Сведения о файлах счета</param>
        public Invoice(Lib.InvoiceFilename files) {
            invoiceFilename = files;
        }

        /// <summary>
        /// Выгрузить счет
        /// </summary>
        /// <param name="pool">Datapool</param>
        /// <param name="outputDirectory">Каталог для экспорта</param>
        /// <param name="leaveFiles">Не удалять файлы после упаковки</param>
        public bool Export(Data.IInvoice pool, string outputDirectory, bool leaveFiles) {
            switch(invoiceFilename.Section) {
                case OrderSection.D1:
                    Console.WriteLine("Случаи обращения с лечебной целью");
                    break;
                case OrderSection.D2:
                    Console.WriteLine("Случаи ВМП");
                    break;
                case OrderSection.D3:
                    Console.WriteLine("Профосмотры и диспансеризация");
                    break;
                case OrderSection.D4:
                    Console.WriteLine("Онкология");
                    break;
            }

            Lib.XmlExporter xml = new Lib.XmlExporter();
            
            string fname = outputDirectory + invoiceFilename.PersonFile + XML;
            if (!xml.Init(fname) || !ExportPeople(xml, pool)) {
                Console.WriteLine("Ошибка при выгрузке пациентов");
                return false;
            }

            fname = outputDirectory + invoiceFilename.InvoiceFile + XML;
            if (!xml.Init(fname) || !ExportInvoice(xml, pool)) {
                Console.WriteLine("Ошибка при выгрузке счетов");
                return false;
            }

            xml.Close();

            if (Lib.Zip.Compress(invoiceFilename)) {
                if (!leaveFiles)
                    Lib.Unlinker.RemoveFiles(invoiceFilename, outputDirectory);

                Console.WriteLine(string.Format("Файл выгрузки: {0}{1}.zip",
                    outputDirectory,
                    invoiceFilename.InvoiceFile));
                return true;
            } else {
                Console.WriteLine("Ошибка при создании архива");
                return false;
            }
        }

        bool ExportPeople(Lib.XmlExporter xml, Data.IInvoice pool) {
            if (!xml.OK) return false;

            xml.Writer.WriteStartElement("PERS_LIST");

            xml.Writer.WriteStartElement("ZGLV");
            xml.Writer.WriteElementString("VERSION", VERSION);
            xml.Writer.WriteElementString("DATA", DateTime.Today.AsXml());
            xml.Writer.WriteElementString("FILENAME", invoiceFilename.PersonFile);
            xml.Writer.WriteElementString("FILENAME1", invoiceFilename.InvoiceFile);
            xml.Writer.WriteEndElement();

            int count = pool.GetPeopleCount(invoiceFilename.Section);
#if DEBUG
            count = Math.Min(Properties.Settings.Default.DebugSelectionLimit, count);
#endif

            Lib.Progress progress = new Progress("Пациенты", count);
            foreach (Person p in pool.LoadPeople(invoiceFilename.Section)) {
                p.Write(xml, pool, invoiceFilename.Section);
                progress.Step();
#if DEBUG
                if (--count <= 0) break;
#endif
            }
            progress.Close();

            xml.Writer.WriteEndElement();

            return true;
        }

        bool ExportInvoice(Lib.XmlExporter xml, Data.IInvoice pool) {
            if (!xml.OK) return false;
            xml.Writer.WriteStartElement("ZL_LIST");

            xml.Writer.WriteStartElement("ZGLV");
            xml.Writer.WriteElementString("VERSION", VERSION);
            xml.Writer.WriteElementString("DATA", DateTime.Today.AsXml());
            xml.Writer.WriteElementString("FILENAME", invoiceFilename.InvoiceFile);
            
            // TODO: Invoices count, not people
            int count = pool.GetInvoiceRecordsCount(invoiceFilename.Section);
#if DEBUG
            count = Math.Min(Properties.Settings.Default.DebugSelectionLimit, count);
#endif
            xml.Writer.WriteElementString("SD_Z", count.ToString());
            xml.Writer.WriteEndElement();


            xml.Writer.WriteStartElement("SCHET");
            xml.Writer.WriteElementString("CODE", invoiceFilename.Code.ToString());
            xml.Writer.WriteElementString("CODE_MO", invoiceFilename.ClinicCode);
            xml.Writer.WriteElementString("YEAR", invoiceFilename.Year.ToString());
            xml.Writer.WriteElementString("MONTH", invoiceFilename.Month.ToString());
            xml.Writer.WriteElementString("NSCHET", invox.Options.InvoiceNumber);
            xml.Writer.WriteElementString("DSCHET", invox.Options.InvoiceDate.AsXml());
            xml.WriteIfValid("PLAT", invoiceFilename.CompanyCode);
            xml.Writer.WriteElementString("SUMMAV", pool.Total(invoiceFilename.Section).ToString("F2", Options.NumberFormat));
            xml.Writer.WriteEndElement();

            Lib.Progress progress = new Progress("Случаи обращения", count);
            int number = 0;
            foreach (InvoiceRecord irec in pool.LoadInvoiceRecords(invoiceFilename.Section)) {
                irec.Identity = number;
                irec.Write(xml, () => progress.Step(), pool, invoiceFilename.Section);
                number = irec.Identity;
#if DEBUG
                if (--count <= 0) break;
#endif
            }

            progress.Close();

            xml.Writer.WriteEndElement();
            return true;
        }
    }
}
