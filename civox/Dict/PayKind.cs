﻿namespace civox.Dict {
    class PayKind : Base {
        const string XML_NAME = "\\Dict\\V010-OPLMSP.xml";
        static PayKind instance = null;
        static object flock = new object();

        public static PayKind Instance {
            get {
                if (instance == null) lock (flock) {
                    if (instance == null) {
                        instance = new PayKind();
                        instance.Load(XML_NAME);
                    }
                }
                return instance;
            }
        }

        PayKind() { }
    }
}
