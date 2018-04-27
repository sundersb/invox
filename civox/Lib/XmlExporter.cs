using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace civox.Lib {
    /// <summary>
    /// Export to XML helper
    /// </summary>
    class XmlExporter {
        XmlWriter writer = null;
        FileStream stream = null;

        /// <summary>
        /// Main horse
        /// </summary>
        public XmlWriter Writer { get { return writer; } }

        /// <summary>
        /// This instance is ready to write data
        /// </summary>
        public bool OK { get { return writer != null; } }

        /// <summary>
        /// Initialize instance
        /// </summary>
        /// <param name="fileName">Name of the XML file to stream into</param>
        /// <returns>True on success</returns>
        public bool Init(string fileName) {
            Close();

            try {
                stream = new FileStream(fileName, FileMode.Create);
                XmlWriterSettings ws = new XmlWriterSettings();

                ws.Indent = true;
                ws.IndentChars = "  ";
                ws.Encoding = Encoding.GetEncoding("windows-1251");

                writer = XmlWriter.Create(stream, ws);
            } catch (Exception ex) {
                Lib.Logger.Log(string.Format("XmlExporter.Init('{0}'):\r\n", fileName) + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Close the writer
        /// </summary>
        public void Close() {
            if (writer != null) {
                writer.Close();
                writer = null;
            }

            // Force flush and close or else Zip complains
            if (stream != null) {
                stream.Dispose();
                stream = null;
            }
        }

        /// <summary>
        /// Write string element to XML if it is not empty
        /// </summary>
        /// <param name="node">XML node name</param>
        /// <param name="value">Node value to check and write</param>
        /// <param name="xml">XML write helper</param>
        /// <remarks>No checks for the writer validity performed</remarks>
        public void WriteIfValid(string node, string value) {
            if (!string.IsNullOrEmpty(value))
                writer.WriteElementString(node, value);
        }

        /// <summary>
        /// Write boolean value node to XML (0 or 1)
        /// </summary>
        /// <param name="node">Node name</param>
        /// <param name="value">Value to write</param>
        public void WriteBool(string node, bool value) {
            writer.WriteElementString(node, value ? "1" : "0");
        }
    }
}
