using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace civox.Dict {
    /// <summary>
    /// Local to federal SMO code converter
    /// </summary>
    class LocalSMO {
        const string XML_NAME = "\\Dict\\F002-Q.xml";

        static LocalSMO FInstance = null;
        static object flock = new object();

        static LocalSMO Instance {
            get {
                if (FInstance == null) {
                    lock (flock) {
                        if (FInstance == null) {
                            FInstance = new LocalSMO();
                            Helper.Load(FInstance.qToF002, Options.BaseDirectory + XML_NAME);
                        }
                    }
                }
                return FInstance;
            }
        }

        Dictionary<string, string> qToF002;
        
        LocalSMO() {
            qToF002 = new Dictionary<string, string>();
        }

        /// <summary>
        /// Get federal code of the SMO
        /// </summary>
        /// <param name="value">Local SMO code</param>
        /// <returns>Federal SMO code</returns>
        public static string FromLocal(string value) {
            return Instance.qToF002[value];
        }
    }
}
