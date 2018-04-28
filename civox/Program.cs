using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace civox {
    class Program {
        static string[] INTRO = { "CIVOX.EXE v.{0}",
                                 "",
                                 "\tЭкспорт счетов Релакс в XML",
                                 "\t***************************",
                                 "",
                                 "Код МО: {1}",
                                 "Код ФОМС: {2}",
                                 "Каталог Релакс: {3}",
                                 "База месяца: {4}"
                               };

        static bool Checkup() {
            if (!Directory.Exists(Options.LpuLocation)) {
                Console.WriteLine("Каталог не найден: " + Options.LpuLocation);
                return false;
            }

            string path = Path.Combine(Options.LpuLocation, Options.PeriodLocation);
            if (!Directory.Exists(path)) {
                Console.WriteLine("Каталог не найден: " + path);
                return false;
            }
            
            return true;
        }

        static bool Run(Lib.InvoiceNames names) {
            string fname;
            Lib.XmlExporter xml;

            fname = Options.OutputLocation + names.PeopleFileName + ".xml";
            xml = new Lib.XmlExporter();
            if (xml.Init(fname)) {
                Model.People people = new Model.People(names);
                people.Write(xml, Options.DataProvider.GetInvoiceRepository());
            } else {
                Console.WriteLine("Ошибка при выгрузке пациентов");
                return false;
            }

            fname = Options.OutputLocation + names.InvoiceFileName + ".xml";
            if (xml.Init(fname)) {
                Model.Invoice invoice = new Model.Invoice(names);
                invoice.Write(xml, Options.DataProvider.GetInvoiceRepository());
            } else {
                Console.WriteLine("Ошибка при выгрузке счета");
                return false;
            }
            xml.Close();

            if (Lib.Zip.Compress(names)) {
                Lib.Unlinker.RemoveFiles(names);
                return true;
            } else return false;
        }

        static void Main(string[] args) {
            Options.Init(args);

            Console.WriteLine(string.Format(string.Join ("\r\n", INTRO),
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                Options.LpuCode,
                Options.FomsCode,
                Options.LpuLocation,
                Options.PeriodLocation));

            if (Options.NeedHelp) {
                Console.WriteLine("\r\nПараметры:");
                Console.WriteLine(Options.Help);
                Console.WriteLine();
            } else {
                if (Checkup()) {
                    Lib.InvoiceNames names = Lib.InvoiceNames.InvoiceToFoms(Options.PacketNumber,
                        Model.InvoiceKind.GeneralTreatment);

                    if (Run(names))
                        Console.WriteLine("\r\nОК\r\n");
                    else
                        Console.WriteLine("\r\nОшибка!\r\n");
                } else {
                    Console.WriteLine("\r\nВыгрузка не произведена!");
                }
            }

            Console.ReadKey();
        }
    }
}
