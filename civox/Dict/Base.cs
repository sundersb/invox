using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace civox.Dict {
    /// <summary>
    /// Base class for dictionaries
    /// </summary>
    class Base {
        const string DEFAULT_VALUE = "???";

        Dictionary<string, string> dict = new Dictionary<string,string>();

        protected bool Load(string fName) {
            string fileName = Options.BaseDirectory + fName;

            if (!File.Exists(fileName)) return false;

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (!fs.CanRead) return false;

            XDocument x = XDocument.Load(fs);
            if (x.Root.Name != "dict") return false;

            foreach(var n in x.Root.Elements()) {
                if (n.Name != "item") return false;

                string ins = n.Attribute("in").Value;
                string outs = n.Attribute("out").Value;
                dict.Add(ins, outs);
            }
            return true;
        }

        /// <summary>
        /// Local to federal code translator
        /// </summary>
        /// <param name="key">Local code</param>
        /// <returns>Federal entity code</returns>
        public string Get(string key) {
            string k = key.TrimStart('0');
            return dict.ContainsKey(k) ? dict[k] : DEFAULT_VALUE;
        }

        protected string GetDefault(string key, string value) {
            return dict.ContainsKey(key) ? dict[key] : value;
        }
    }
}
