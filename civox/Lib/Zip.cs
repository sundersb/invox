using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace civox.Lib {
    class Zip {
        const string ZIPPER_LOCATION = "C:\\Program Files\\7-Zip\\7z.exe";

        public static bool Compress(InvoiceNames names) {
            ProcessStartInfo info = new ProcessStartInfo(ZIPPER_LOCATION);
            info.CreateNoWindow = true;
            info.Arguments = string.Empty;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            Process p = Process.Start(info);
            p.WaitForExit();

            return p.ExitCode == 0;
        }
    }
}
