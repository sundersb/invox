using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace invox.Lib {
    class Zip {
        const string ARGUMENTS_7Z = "a -tzip -mx9 \"{0}{1}.zip\" \"{0}{1}.xml\" \"{0}{2}.xml\"";

        /// <summary>
        /// Compress invoice files to ZIP archive
        /// </summary>
        /// <param name="names">Invoice filenames</param>
        /// <returns>True on success</returns>
        public static bool Compress(InvoiceFilename files) {
            string bin = Properties.Settings.Default.PathTo7Zip;
            if (!System.IO.File.Exists(bin)) return false;

            ProcessStartInfo info = new ProcessStartInfo(bin);

            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            info.UseShellExecute = false;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;

            info.Arguments = string.Format(ARGUMENTS_7Z,
                invox.Options.OutputLocation,
                files.InvoiceFile,
                files.PersonFile);

            Process p = Process.Start(info);
            string error = p.StandardError.ReadToEnd();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            if (p.ExitCode != 0) {
                Logger.Log(output);
                Logger.Log(error);
                return false;
            } else return true;
        }
    }
}
