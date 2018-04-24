namespace civox.Dict {
    /// <summary>
    /// Local to federal SMO code converter
    /// </summary>
    class SMO : Base {
        const string XML_NAME = "\\Dict\\F002-Q.xml";
        static SMO instance = null;
        static object flock = new object();

        public static SMO Instance {
            get {
                if (instance == null) lock (flock) {
                    if (instance == null) {
                        instance = new SMO();
                        instance.Load(XML_NAME);
                    }
                }
                return instance;
            }
        }

        SMO() { }
    }
}
