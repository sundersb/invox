using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace civox.Dict {
    abstract class Base {
        const string DEFAULT_VALUE = "???";

        static Dictionary<string, string> dict = null;
        static object flock = new object();

        static Dictionary<string, string> Instance {
            get {
                if (dict == null)
                    lock (flock) {
                        if (dict == null) {
                            dict = new Dictionary<string, string>();
                            Load(dict, Options.BaseDirectory + GetFilename());
                        }
                    }
                return dict;
            }
        }

        static void Load(Dictionary<string, string> dict, string fileName) {
            if (!File.Exists(fileName)) return;

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (!fs.CanRead) return;

            XmlDataDocument xml = new XmlDataDocument();
            xml.Load(fs);

            if (xml.DocumentElement.Name != "dict") return;

            foreach (XmlNode node in xml.DocumentElement.ChildNodes) {
                if (node.Name != "item") return;

                string ins = node.Attributes["in"].Value;
                string outs = node.Attributes["out"].Value;
                dict.Add(ins, outs);
            }
        }

        public static string Get(string key) {
            return dict.ContainsKey(key) ? dict[key] : DEFAULT_VALUE;
        }

        abstract static string GetFilename();
    }
}
