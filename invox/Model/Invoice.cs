using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Lib;

namespace invox.Model {
    class Invoice {
        const string XML = ".xml";
        const string VERSION_INVOICES = "3.1";
        const string VERSION_PEOPLE = "3.2";
        const ConsoleColor COLOR_TITLE = ConsoleColor.Magenta;

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
            Console.WriteLine();
            Console.WriteLine();
            WriteSectionTitle(invoiceFilename.Section);

            Lib.XmlExporter xml = new Lib.XmlExporter();

            int count = pool.GetPeopleCount(invoiceFilename.Section, invoiceFilename.Subsection);
#if DEBUG
            count = Math.Min(Properties.Settings.Default.DebugSelectionLimit, count);
#endif
            if (count > 0) {
                string fname = outputDirectory + invoiceFilename.PersonFile + XML;
                if (!xml.Init(fname) || !ExportPeople(xml, pool, count)) {
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
            } else {
                Console.WriteLine("Нет данных для выгрузки");
                return true;
            }
        }

        bool ExportPeople(Lib.XmlExporter xml, Data.IInvoice pool, int count) {
            if (!xml.OK) return false;

            xml.Writer.WriteStartElement("PERS_LIST");

            xml.Writer.WriteStartElement("ZGLV");
            xml.Writer.WriteElementString("VERSION", VERSION_PEOPLE);
            xml.Writer.WriteElementString("DATA", DateTime.Today.AsXml());
            xml.Writer.WriteElementString("FILENAME", invoiceFilename.PersonFile);
            xml.Writer.WriteElementString("FILENAME1", invoiceFilename.InvoiceFile);
            xml.Writer.WriteEndElement();

            Lib.Progress progress = new Progress("Пациенты", count);
            foreach (Person p in pool.LoadPeople(invoiceFilename.Section, invoiceFilename.Subsection)) {
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
            xml.Writer.WriteElementString("VERSION", VERSION_INVOICES);
            xml.Writer.WriteElementString("DATA", DateTime.Today.AsXml());
            xml.Writer.WriteElementString("FILENAME", invoiceFilename.InvoiceFile);
            
            // TODO: Invoices count, not people
            int count = pool.GetInvoiceRecordsCount(invoiceFilename.Section, invoiceFilename.Subsection);
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
            xml.Writer.WriteElementString("SUMMAV", pool.Total(invoiceFilename.Section, invoiceFilename.Subsection).ToString("F2", Options.NumberFormat));

            // 20191028
            if (invoiceFilename.Section == OrderSection.D3)
                xml.Writer.WriteElementString("DISP", ProphSubsectionHelper.GetCodeV016(invoiceFilename.Subsection));

            xml.Writer.WriteEndElement();

            Lib.Progress progress = new Progress("Случаи обращения", count);
            int number = 0;
            foreach (InvoiceRecord irec in pool.LoadInvoiceRecords(invoiceFilename.Section, invoiceFilename.Subsection)) {
                irec.Identity = number;
                irec.Write(xml, () => progress.Step(), pool, invoiceFilename.Section, invoiceFilename.Subsection);
                number = irec.Identity;
#if DEBUG
                if (--count <= 0) break;
#endif
            }

            progress.Close();

            xml.Writer.WriteEndElement();
            return true;
        }

        void WriteCenter(string value) {
            if (Console.WindowWidth > value.Length)
                Console.CursorLeft = (Console.WindowWidth - value.Length) / 2;

            Console.WriteLine(value);
        }

        void WriteSectionTitle(OrderSection section) {
            ConsoleColor fg = Console.ForegroundColor;
            Console.ForegroundColor = COLOR_TITLE;

            switch (section) {
                case OrderSection.D1:
                    WriteCenter("Случаи обращения с лечебной целью");
                    break;
                case OrderSection.D2:
                    WriteCenter("Случаи ВМП");
                    break;
                case OrderSection.D3:
                    WriteCenter("Профосмотры и диспансеризация ("
                        + ProphSubsectionHelper.AsString(invoiceFilename.Subsection)
                        + ")");
                    break;
                case OrderSection.D4:
                    WriteCenter("Онкология");
                    break;
            }
            Console.ForegroundColor = fg;
        }
    }
}
