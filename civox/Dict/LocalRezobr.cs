using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Dict {
    class LocalRezobr {
        const string XML_NAME = "\\Dict\\V009-BE.xml";

        static LocalRezobr FInstance = null;
        static object flock = new object();

        static LocalRezobr Instance {
            get {
                if (FInstance == null) {
                    lock (flock) {
                        if (FInstance == null) {
                            FInstance = new LocalRezobr();
                            Helper.Load(FInstance.beToV009, Options.BaseDirectory + XML_NAME);
                        }
                    }
                }
                return FInstance;
            }
        }

        Dictionary<string, string> beToV009;

        LocalRezobr() {
            beToV009 = new Dictionary<string, string>();
        }

        /// <summary>
        /// Get federal code of the SMO
        /// </summary>
        /// <param name="value">Local SMO code</param>
        /// <returns>Federal SMO code</returns>
        public static string FromLocal(string value) {
            return Instance.beToV009[value];
        }
    }
}
