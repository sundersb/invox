using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Dict {
    class LocalProfile {
        const string XML_NAME     = "\\Dict\\V015-SLSPEC.xml";
        const string XML_NAME_PED = "\\Dict\\V015-SLSPEC-PED.xml";
        const string UNKNOWN_PROFILE = "???";

        static LocalProfile FInstance = null;
        static object flock = new object();

        static LocalProfile Instance {
            get {
                if (FInstance == null) {
                    lock (flock) {
                        if (FInstance == null) {
                            FInstance = new LocalProfile();
                            Helper.Load(FInstance.dict, Options.BaseDirectory
                                + (Options.Pediatric ? XML_NAME_PED : XML_NAME));
                        }
                    }
                }
                return FInstance;
            }
        }

        Dictionary<string, string> dict;

        LocalProfile() {
            dict = new Dictionary<string, string>();
        }

        /// <summary>
        /// Get federal code of the SMO
        /// </summary>
        /// <param name="value">Local SMO code</param>
        /// <returns>Federal SMO code</returns>
        public static string FromLocal(string value) {
            if (Instance.dict.ContainsKey(value))
                return Instance.dict[value];
            else
                return UNKNOWN_PROFILE;
        }
    }
}
