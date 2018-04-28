using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace civox {
    static class Options {
        const string PERIOD_LOCATION = "OUTS{0:d4}\\PERIOD{1:d2}\\";
        const string OUTPUT_LOCATION = "OUTPUT\\";

        static string baseDirectory;
        static string okato;
        static string lpuLocation;
        static string outputLocation;
        static string lpuCode;
        static string localLpuCode;
        static string fomsCode;
        static string defaultDocument;
        static bool pediatric;
        static string periodLocation;
        static Lib.CivoxOptions options;

        static Data.IDataProvider provider;

        public static NumberFormatInfo NumberFormat { get; private set; }

        /// <summary>
        /// This application's binary location
        /// </summary>
        public static string BaseDirectory { get { return baseDirectory; } }

        /// <summary>
        /// LPU OKATO code
        /// </summary>
        public static string OKATO { get { return okato; } }

        /// <summary>
        /// Relax location
        /// </summary>
        public static string LpuLocation { get { return lpuLocation; } }

        /// <summary>
        /// Relax output location
        /// </summary>
        public static string OutputLocation { get { return outputLocation; } }

        /// <summary>
        /// Clinic code (270019)
        /// </summary>
        public static string LpuCode { get { return lpuCode; } }

        /// <summary>
        /// Clinic code in territory register (2101003)
        /// </summary>
        public static string LocalLpuCode { get { return localLpuCode; } }

        /// <summary>
        /// Territory FOMS code (27)
        /// </summary>
        public static string FomsCode { get { return fomsCode; } }

        /// <summary>
        /// Default document code (ref. F011). 14 for passport RF
        /// </summary>
        public static string DefaultDocument { get { return defaultDocument; } }

        /// <summary>
        /// This is pediatric clinic
        /// </summary>
        public static bool Pediatric { get { return pediatric; } }

        /// <summary>
        /// Report year
        /// </summary>
        public static int Year { get { return options.Year; } }

        /// <summary>
        /// Report month
        /// </summary>
        public static int Month { get { return options.Month; } }

        /// <summary>
        /// Invoice number
        /// </summary>
        public static string InvoiceNumber { get { return options.InvoiceNumber; } }

        /// <summary>
        /// Ordinal number of the packet for the period
        /// </summary>
        public static string PacketNumber { get { return options.PackageNumber; } }

        /// <summary>
        /// Invoice number. Generated from federal code of the clinic and invoice number
        /// </summary>
        public static string InvoiceCode {
            get { return lpuCode + options.InvoiceNumber; }
        }

        /// <summary>
        /// Invoice issue date
        /// </summary>
        public static DateTime InvoiceDate { get { return options.InvoiceDate; } }

        /// <summary>
        /// Relax database path for the period to export
        /// </summary>
        public static string PeriodLocation { get { return periodLocation; } }

        /// <summary>
        /// Data provider
        /// </summary>
        public static Data.IDataProvider DataProvider { get { return provider; } }

        /// <summary>
        /// True if showing help needed
        /// </summary>
        public static bool NeedHelp { get { return options.ShowHelp; } }

        /// <summary>
        /// Show command line help and error
        /// </summary>
        public static string Help { get { return Lib.CommandLineOptions.ShowHelp(options); } }

        /// <summary>
        /// Load application options
        /// </summary>
        /// <param name="args">Command line parameters</param>
        public static void Init(string[] args) {
            Assembly asm = Assembly.GetEntryAssembly();
            if (asm != null)
                baseDirectory = Path.GetDirectoryName(asm.Location);
            else
                baseDirectory = Directory.GetCurrentDirectory(); 
            
            lpuLocation = Properties.Settings.Default.LpuLocation.Trim();
            if (!lpuLocation.EndsWith("\\")) lpuLocation += '\\';
            outputLocation = lpuLocation + OUTPUT_LOCATION;

            lpuCode = Properties.Settings.Default.LpuCode;
            localLpuCode = Properties.Settings.Default.LocalLpuCode;
            fomsCode = Properties.Settings.Default.FomsCode;
            pediatric = Properties.Settings.Default.Pediatric;
            okato = Properties.Settings.Default.OKATO;
            defaultDocument = Properties.Settings.Default.DefaultDocument;

            // Decimal point instead of comma
            NumberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            NumberFormat.NumberDecimalSeparator = ".";

            options = Lib.CommandLineOptions.Get(args, typeof(Lib.CivoxOptions)) as Lib.CivoxOptions;

            periodLocation = string.Format(PERIOD_LOCATION, options.Year, options.Month);
            provider = new Data.Relax.Provider(lpuLocation);
        }
    }
}
