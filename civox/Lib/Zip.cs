using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace civox.Lib {
    /// <summary>
    /// ZIP archiver helper
    /// </summary>
    class Zip {
        /// <summary>
        /// Compress invoice files to ZIP archive
        /// </summary>
        /// <param name="names">Invoice filenames</param>
        /// <returns>True on success</returns>
        public static bool Compress(InvoiceNames names) {
            string bin = Properties.Settings.Default.PathTo7Zip;
            if (!System.IO.File.Exists(bin)) return false;

            ProcessStartInfo info = new ProcessStartInfo(bin);

            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            StringBuilder sb = new StringBuilder("a -tzip \"");
            sb.Append(Options.OutputLocation);
            sb.Append(names.InvoiceFileName);
            sb.Append(".zip\" \"");
            sb.Append(Options.OutputLocation);
            sb.Append(names.InvoiceFileName);
            sb.Append(".xml\" \"");
            sb.Append(Options.OutputLocation);
            sb.Append(names.PeopleFileName);
            sb.Append(".xml\"");

            info.Arguments = sb.ToString();

            Process p = Process.Start(info);
            p.WaitForExit();

            return p.ExitCode == 0;
        }
    }
}
