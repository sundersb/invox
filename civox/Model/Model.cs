using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Model {
    /// <summary>
    /// Base class for data model
    /// </summary>
    abstract class Model {
        /// <summary>
        /// Write model to XML
        /// </summary>
        /// <param name="xml">XML write helper</param>
        public abstract void Write(Lib.XmlExporter xml, Data.IInvoice repo);
    }
}
