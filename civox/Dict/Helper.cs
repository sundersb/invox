using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace civox.Dict {
    /// <summary>
    /// Dictionary load helper
    /// </summary>
    class Helper {
        /// <summary>
        /// Load dictionary from file
        /// </summary>
        /// <param name="dict">Dictionary to fill</param>
        /// <param name="fileName">XML file name to load data from</param>
        /// <returns>True on success</returns>
        public static bool Load(Dictionary<string, string> dict, string fileName) {
            if (!File.Exists(fileName)) return false;

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (!fs.CanRead) return false;

            XmlDataDocument xml = new XmlDataDocument();
            xml.Load(fs);

            if (xml.DocumentElement.Name != "dict") return false;

            foreach (XmlNode node in xml.DocumentElement.ChildNodes) {
                if (node.Name != "item") return false;

                string ins = node.Attributes["in"].Value;
                string outs = node.Attributes["out"].Value;
                dict.Add(ins, outs);
            }

            return true;
        }
    }
}
