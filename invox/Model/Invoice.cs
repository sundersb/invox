using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Model {
    class Invoice {

        public Invoice(Data.IInvoice pool, int year, int month, int packetNumber, OrderSection section) {
        }

        public void Write(Lib.XmlExporter xml, Data.IInvoice repo, string outputDirectory) {
        }

        string GetPersonFile() {
        }
    }
}
