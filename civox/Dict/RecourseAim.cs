using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Dict {
    class RecourseAim : Base {
        const string XML_NAME = "\\Dict\\CEL-OPLMSP.xml";

        static RecourseAim instance = null;
        static object flock = new object();

        public static RecourseAim Instance {
            get {
                if (instance == null) lock (flock) {
                    if (instance == null) {
                        instance = new RecourseAim();
                        instance.Load(XML_NAME);
                    }
                }
                return instance;
            }
        }

        RecourseAim() { }
    }
}
