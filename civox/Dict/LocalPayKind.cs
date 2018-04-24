using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Dict {
    class LocalPayKind {
        const string XML_NAME = "\\Dict\\V010-OPLMSP.xml";
        const string DEFAULT_VALUE = "???";

        static LocalPayKind FInstance = null;
        static object flock = new object();

        static LocalPayKind Instance {
            get {
                if (FInstance == null) {
                    lock (flock) {
                        if (FInstance == null) {
                            FInstance = new LocalPayKind();
                            Helper.Load(FInstance.dict, Options.BaseDirectory + XML_NAME);
                        }
                    }
                }
                return FInstance;
            }
        }

        Dictionary<string, string> dict;

        LocalPayKind() {
            dict = new Dictionary<string, string>();
        }

        /// <summary>
        /// Get federal code of the SMO
        /// </summary>
        /// <param name="value">Local SMO code</param>
        /// <returns>Federal SMO code</returns>
        public static string FromLocal(string value) {
            return Instance.dict.ContainsKey(value) ? Instance.dict[value] : DEFAULT_VALUE;
        }
    }
}
