using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace civox.Lib {
    /// <summary>
    /// Remove invoice XML files helper
    /// </summary>
    class Unlinker {
        /// <summary>
        /// Remove invoice XML files
        /// </summary>
        /// <param name="names">Invoice filenames' instance</param>
        public static void RemoveFiles(InvoiceNames names) {
            string fname = Options.OutputLocation + names.PeopleFileName + ".xml";
            if (File.Exists(fname)) File.Delete(fname);

            fname = Options.OutputLocation + names.InvoiceFileName + ".xml";
            if (File.Exists(fname)) File.Delete(fname);
        }
    }
}
