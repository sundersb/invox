namespace invox.Dict {
    class Condition : Base {
        const string XML_NAME = "\\Dict\\V006-SLUSL.xml";
        static Condition instance = null;
        static object flock = new object();

        public static Condition Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new Condition();
                            instance.Load(XML_NAME);
                        }
                    }
                return instance;
            }
        }

        Condition() { }

        new public string Get(string key) {
            return GetDefault(key, "3");
        }
    }
}
