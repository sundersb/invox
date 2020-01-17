using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Dict {
    /// <summary>
    /// Get statistics code from ICD
    /// </summary>
    class StatisticsCode : Base {
        const string XML_NAME = "\\Dict\\morbidity.xml";

        static StatisticsCode instance = null;
        static object flock = new object();

        public static StatisticsCode Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new StatisticsCode();
                            instance.Load(XML_NAME);
                        }
                    }
                return instance;
            }
        }

        StatisticsCode() { }

        new public string Get(string key) {
            if (string.IsNullOrEmpty(key) || key.Length < 3) return "1";

            string result = base.GetDefault(key.Substring(0, 3), string.Empty);
            if (result == string.Empty) {
                if (key.First() == 'C')
                    return "3";
                else
                    return "1";
            } else {
                if (result == "2")
                    return "3";
                else
                    return "1";
            }
        }
    }
}
