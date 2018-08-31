using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Globalization;

namespace invox {
    static class Options {
        const string PERIOD_LOCATION = "OUTS{0:d4}\\PERIOD{1:d2}\\";
        const string OUTPUT_LOCATION = "OUTPUT\\";
        const string SPECIALITY_CLASSIFIER = "V021";

        static string baseDirectory;
        static string lpuLocation;
        static string outputLocation;
        static string lpuCode;
        static string localLpuCode;
        static string periodLocation;
        static string fomsCode;
        static bool pediatric;
        static string okato;
        static string defaultDocument;
        static Lib.Options options;

        public static NumberFormatInfo NumberFormat { get; private set; }

        public static string BaseDirectory { get { return baseDirectory; } }
        public static string LpuCode { get { return lpuCode; } }
        public static string FomsCode { get { return fomsCode; } }
        public static string LpuLocation { get { return lpuLocation; } }
        public static string PeriodLocation { get { return periodLocation; } }
        public static bool NeedHelp { get { return options.ShowHelp; } }
        public static string Help { get { return Lib.CommandLineOptions.ShowHelp(options); } }
        public static string Version { get { return GetVersion(); } }
        public static string LocalLpuCode { get { return localLpuCode; } }
        public static string InvoiceNumber { get { return options.InvoiceNumber; } }
        public static string PacketNumber { get { return options.PackageNumber; } }
        public static string SpecialityClassifier { get { return SPECIALITY_CLASSIFIER; } }

        static string GetVersion() {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        static string GetBaseDirectory() {
            Assembly asm = Assembly.GetEntryAssembly();
            if (asm != null)
                return Path.GetDirectoryName(asm.Location);
            else
                return Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Load application options
        /// </summary>
        /// <param name="args">Command line parameters</param>
        public static void Init(string[] args) {
            baseDirectory = GetBaseDirectory();

            lpuLocation = Properties.Settings.Default.LpuLocation;
            outputLocation = Path.Combine(lpuLocation, OUTPUT_LOCATION);

            lpuCode = Properties.Settings.Default.LpuCode;
            localLpuCode = Properties.Settings.Default.LocalLpuCode;
            fomsCode = Properties.Settings.Default.FomsCode;
            pediatric = Properties.Settings.Default.Pediatric;
            okato = Properties.Settings.Default.OKATO;
            defaultDocument = Properties.Settings.Default.DefaultDocument;

            // Decimal point instead of comma
            NumberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            NumberFormat.NumberDecimalSeparator = ".";

            options = Lib.CommandLineOptions.Get(args, typeof(Lib.Options)) as Lib.Options;

            periodLocation = string.Format(PERIOD_LOCATION, options.Year, options.Month);
        }
    }
}
