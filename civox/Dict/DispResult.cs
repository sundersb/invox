using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Dict {
    class DispResult : Base {
        const string XML_NAME = "\\Dict\\V017-REZOBR.xml";

        static DispResult instance = null;
        static object flock = new object();

        public static DispResult Instance {
            get {
                if (instance == null) lock (flock) {
                    if (instance == null) {
                        instance = new DispResult();
                        instance.Load(XML_NAME);
                    }
                }
                return instance;
            }
        }

        DispResult() { }
    }
}
