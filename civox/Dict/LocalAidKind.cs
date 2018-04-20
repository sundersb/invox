using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Dict {
    class LocalAidKind {
        const string XML_NAME = "\\Dict\\V002-SLMSP.xml";

        static LocalAidKind FInstance = null;
        static object flock = new object();

        static LocalAidKind Instance {
            get {
                if (FInstance == null) {
                    lock (flock) {
                        if (FInstance == null) {
                            FInstance = new LocalAidKind();
                            Helper.Load(FInstance.dict, Options.BaseDirectory + XML_NAME);
                        }
                    }
                }
                return FInstance;
            }
        }

        Dictionary<string, string> dict;

        LocalAidKind() {
            dict = new Dictionary<string, string>();
        }

        /// <summary>
        /// Get federal code of the SMO
        /// </summary>
        /// <param name="value">Local SMO code</param>
        /// <returns>Federal SMO code</returns>
        public static string FromLocal(string value) {
            return Instance.dict[value];
        }
    }
}
