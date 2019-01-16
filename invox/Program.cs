using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace invox {
    class Program {
        const string DBF = ".DBF";

        static string[] INTRO = { "INVOX.EXE v.{0}",
                                 "",
                                 "\tЭкспорт счетов Релакс в XML",
                                 "\t***************************",
                                 "",
                                 "Код МО: {1}",
                                 "Код ФОМС: {2}",
                                 "Каталог Релакс: {3}",
                                 "База месяца: {4}",
                                 "Счет №: {5}",
                                 "Номер пакета: {6}"
                               };


        static bool Checkup(Data.IInvoice pool) {
            if (!Directory.Exists(Options.LpuLocation)) {
                Console.WriteLine("Каталог не найден: " + Options.LpuLocation);
                return false;
            }

            string path = Path.Combine(Options.LpuLocation, Options.PeriodLocation);
            if (!Directory.Exists(path)) {
                Console.WriteLine("Каталог не найден: " + path);
                return false;
            }

            if (!File.Exists(Path.Combine(path, 'P' + Options.LocalLpuCode + DBF))) {
                Console.WriteLine("Данный период не содержит файла пациентов своей территории");
                return false;
            }

            if (!File.Exists(Path.Combine(path, 'S' + Options.LocalLpuCode + DBF))) {
                Console.WriteLine("Данный период не содержит файла счетов своей территории");
                return false;
            }

            if (!File.Exists(Path.Combine(path, "PAT.DBF"))) {
                Console.WriteLine("Данный период не содержит сводного счета");
                return false;
            }

            if (!File.Exists(Path.Combine(path, "DIAGNOZ.DBF"))) {
                Console.WriteLine("Данный период не содержит файла диагнозов");
                return false;
            }

            List<string> errors = pool.LoadNoDeptDoctors().ToList();
            if (errors.Count > 0) {
                Console.WriteLine("\r\nИмеются формальные ошибки:");
                foreach (string e in errors) {
                    Console.WriteLine("\t" + e);
                }
                return false;
            }

            return true;
        }

        static void Main(string[] args) {
            Options.Init(args);

            Console.WriteLine(string.Format(string.Join("\r\n", INTRO),
                Options.Version,
                Options.LpuCode,
                Options.FomsCode,
                Options.LpuLocation,
                Options.PeriodLocation,
                Options.InvoiceNumber,
                Options.PacketNumber));

            if (Options.NeedHelp) {
                Console.WriteLine("\r\nПараметры:");
                Console.WriteLine(Options.Help);
                Console.WriteLine();
            } else {
                Data.IInvoice pool = new Data.Relax.Pool(Options.LpuLocation, Options.LocalLpuCode, Options.PeriodLocation);

                if (Checkup(pool)) {
                    // TODO: Section from commandline: --files=1234
                    Model.OrderSection[] ss = new Model.OrderSection[] {
                        Model.OrderSection.D1,
                        Model.OrderSection.D2,
                        Model.OrderSection.D3,
                        Model.OrderSection.D4
                    };
                    int packet = Options.PacketNumber - 1;

                    bool error = false;
                    foreach (Model.OrderSection section in Options.Sections) {
                        if (!Run(pool, section, ++packet)) {
                            error = true;
                            Console.WriteLine("Ошибка при выгрузке счетов " + Model.OrderSectionHelper.AsString(section));
                            break;
                        }
                    }

                    if (!error)
                        Console.WriteLine("Счета выгружены!");
                } else {
                    Console.WriteLine("\r\nВыгрузка не произведена!");
                }
            }

            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        static bool Run(Data.IInvoice pool, Model.OrderSection section, int packet) {
            Lib.InvoiceFilename files = Lib.InvoiceFilename.ToAssuranceFund(
                Options.LpuCode,
                Options.FomsCode,
                Options.Year,
                Options.Month,
                packet,
                section);

            Model.Invoice invoice = new Model.Invoice(files);

            if (!invoice.Export(pool, Options.OutputLocation)) {
                Console.WriteLine("\r\nОшибка!\r\n");
                return false;
            }
            
            return true;
        }
    }
}
