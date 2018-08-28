using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Model {
    abstract class Base {
        public abstract void Write(Lib.XmlExporter xml, Data.IInvoice pool);
    }
}
