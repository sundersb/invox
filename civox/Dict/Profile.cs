using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Dict {
    class Profile : Base {
        const string XML_NAME     = "\\Dict\\V015-SLSPEC.xml";
        const string XML_NAME_PED = "\\Dict\\V015-SLSPEC-PED.xml";
        const string UNKNOWN_PROFILE = "???";

        static Profile instance = null;
        static object flock = new object();

        public static Profile Instance {
            get {
                if (instance == null) lock (flock) {
                    if (instance == null) {
                        instance = new Profile();
                        instance.Load(Options.Pediatric ? XML_NAME_PED : XML_NAME);
                    }
                }
                return instance;
            }
        }

        Profile() { }
    }
}
