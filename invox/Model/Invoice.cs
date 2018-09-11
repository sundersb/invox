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
        public bool Export(Data.IInvoice pool, string outputDirectory) {
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
            
            int count = pool.GetPeopleCount(invoiceFilename.Section);
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
            xml.Writer.WriteElementString("SUMMAV", pool.Total(invoiceFilename.Section).ToString("D2"));
            xml.Writer.WriteEndElement();

            Lib.Progress progress = new Progress("Случаи обращения", count);

            switch(invoiceFilename.Section) {
                case OrderSection.D1:
                    foreach (InvoiceRecord irec in pool.LoadInvoiceRecords(OrderSection.D1)) {
                        irec.WriteD1(xml, pool);
                        progress.Step();
#if DEBUG
                        if (--count <= 0) break;
#endif
                    }
                    break;

                case OrderSection.D2:
                    foreach (InvoiceRecord irec in pool.LoadInvoiceRecords(OrderSection.D2)) {
                        irec.WriteD2(xml, pool);
                        progress.Step();
#if DEBUG
                        if (--count <= 0) break;
#endif
                    }
                    break;

                case OrderSection.D3:
                    foreach (InvoiceRecord irec in pool.LoadInvoiceRecords(OrderSection.D3)) {
                        irec.WriteD3(xml, pool);
                        progress.Step();
#if DEBUG
                        if (--count <= 0) break;
#endif
                    }
                    break;
            }
            progress.Close();

            xml.Writer.WriteEndElement();
            return true;
        }
    }
}
