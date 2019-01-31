using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Dict {
    class BedProfile : Base {
        const string XML_NAME = "\\Dict\\V020-SLPROF.xml";
        static BedProfile instance = null;
        static object flock = new object();

        public static BedProfile Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new BedProfile();
                            instance.Load(XML_NAME);
                        }
                    }
                return instance;
            }
        }

        BedProfile() { }
    }
}
