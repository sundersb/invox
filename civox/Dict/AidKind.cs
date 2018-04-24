namespace civox.Dict {
    class AidKind : Base {
        const string XML_NAME = "\\Dict\\V002-SLMSP.xml";

        static AidKind instance = null;
        static object flock = new object();

        public static AidKind Instance {
            get {
                if (instance == null) lock (flock) {
                    if (instance == null) {
                        instance = new AidKind();
                        instance.Load(XML_NAME);
                    }
                }
                return instance;
            }
        }

        AidKind() { }
    }
}
