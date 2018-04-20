using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Dict {
    class LocalCondition {
        const string XML_NAME = "\\Dict\\V006-SLUSL.xml";

        static LocalCondition FInstance = null;
        static object flock = new object();

        static LocalCondition Instance {
            get {
                if (FInstance == null) {
                    lock (flock) {
                        if (FInstance == null) {
                            FInstance = new LocalCondition();
                            Helper.Load(FInstance.dict, Options.BaseDirectory + XML_NAME);
                        }
                    }
                }
                return FInstance;
            }
        }

        Dictionary<string, string> dict;

        LocalCondition() {
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
