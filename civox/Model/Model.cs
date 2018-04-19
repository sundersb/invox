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
        public abstract void Write(Lib.XmlExporter xml, Data.IDataProvider provider);

        /// <summary>
        /// Write string element to XML if it is not empty
        /// </summary>
        /// <param name="node">XML node name</param>
        /// <param name="value">Node value to check and write</param>
        /// <param name="xml">XML write helper</param>
        protected void WriteIfValid(string node, string value, Lib.XmlExporter xml) {
            if (!string.IsNullOrEmpty(value))
                xml.Writer.WriteElementString(node, value);
        }

        protected void WriteBool(string node, bool value, Lib.XmlExporter xml) {
            if (value)
                xml.Writer.WriteElementString(node, "1");
            else
                xml.Writer.WriteElementString(node, "0");
        }
    }
}
