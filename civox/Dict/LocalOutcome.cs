using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Dict {
    class LocalOutcome {
        const string XML_NAME = "\\Dict\\V012-BOLEND.xml";
        const string EXAMINATION_CODE = "306";

        static LocalOutcome FInstance = null;
        static object flock = new object();

        static LocalOutcome Instance {
            get {
                if (FInstance == null) {
                    lock (flock) {
                        if (FInstance == null) {
                            FInstance = new LocalOutcome();
                            Helper.Load(FInstance.dict, Options.BaseDirectory + XML_NAME);
                        }
                    }
                }
                return FInstance;
            }
        }

        Dictionary<string, string> dict;

        LocalOutcome() {
            dict = new Dictionary<string, string>();
        }

        /// <summary>
        /// Get hospitalization outcome
        /// </summary>
        /// <param name="condition">Aid condition</param>
        /// <param name="outcome">Relax PATU.IG (BOLEND) code</param>
        /// <returns>V012 code for the outcome</returns>
        public static string FromLocal(string condition, string outcome) {
            string k = condition + "-" + outcome;
            if (Instance.dict.ContainsKey(k))
                return Instance.dict[k];
            else
                return EXAMINATION_CODE;
        }
    }
}
