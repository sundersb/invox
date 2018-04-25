using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace civox {
    static class Options {
        const string PERIOD_LOCATION = "OUTS{0:d4}\\PERIOD{1:d2}\\";
        const string OUTPUT_LOCATION = "OUTPUT\\";

        static string baseDirectory;
        static string okato;
        static string lpuLocation;
        static string outputLocation;
        static string lpuCode;
        static string fomsCode;
        static string defaultDocument;
        static bool pediatric;
        static int year;
        static int month;
        static string periodLocation;
        static Data.IDataProvider provider;

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
        public static int Year { get { return year; } }

        /// <summary>
        /// Report month
        /// </summary>
        public static int Month { get { return month; } }

        /// <summary>
        /// Relax database path for the period to export
        /// </summary>
        public static string PeriodLocation { get { return periodLocation; } }

        /// <summary>
        /// Data provider
        /// </summary>
        public static Data.IDataProvider DataProvider { get { return provider; } }

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
            fomsCode = Properties.Settings.Default.FomsCode;
            pediatric = Properties.Settings.Default.Pediatric;
            okato = Properties.Settings.Default.OKATO;
            defaultDocument = Properties.Settings.Default.DefaultDocument;

            // Default period - previous month
            DateTime date = DateTime.Today.AddMonths(-1);
            year = date.Year;
            month = date.Month;

            if (args.Length > 0) {
                int yy, mm;
                string m = args[0];
                if (m.Length == 6) {
                    string y = m.Substring(0, m.Length - 2);
                    if (int.TryParse(y, out yy) && yy > 2000 && yy < 2100) year = yy;
                    m = m.Substring(4);
                }
                if (int.TryParse(m, out mm) && mm > 0 && mm < 13) month = mm;
            }

            periodLocation = string.Format(PERIOD_LOCATION, year, month);
            provider = new Data.Relax.Provider(lpuLocation);
        }

    }
}
