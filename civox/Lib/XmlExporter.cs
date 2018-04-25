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
            if (writer != null) writer.Close();

            FileStream s = null;
            try {
                s = new FileStream(fileName, FileMode.Create);
                XmlWriterSettings ws = new XmlWriterSettings();

                ws.Indent = true;
                ws.IndentChars = "  ";
                // TODO: XML encoding to 1251
                ws.Encoding = Encoding.UTF8;// Encoding.GetEncoding("windows-1251");

                writer = XmlWriter.Create(s, ws);
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
        }
    }
}
