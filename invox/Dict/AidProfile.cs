namespace invox.Dict {
    class AidProfile : Base {
        const string XML_NAME = "\\Dict\\V002-SLMSP.xml";

        static AidProfile instance = null;
        static object flock = new object();

        public static AidProfile Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new AidProfile();
                            instance.Load(XML_NAME);
                        }
                    }
                return instance;
            }
        }

        AidProfile() { }
    }
}
