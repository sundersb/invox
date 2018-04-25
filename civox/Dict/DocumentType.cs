namespace civox.Dict {
    class DocumentType : Base {
        const string XML_NAME = "\\Dict\\F011-TPDOC.xml";

        static DocumentType instance = null;
        static object flock = new object();

        public static DocumentType Instance {
            get {
                if (instance == null) lock (flock) {
                    if (instance == null) {
                        instance = new DocumentType();
                        instance.Load(XML_NAME);
                    }
                }
                return instance;
            }
        }

        DocumentType() { }

        new public static string Get(string value) {
            return Instance.GetDefault(value, Options.DefaultDocument);
        }
    }
}
