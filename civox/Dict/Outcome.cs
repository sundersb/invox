namespace civox.Dict {
    class Outcome : Base {
        const string XML_NAME = "\\Dict\\V012-BOLEND.xml";
        const string DEFAULT_VALUE = "306"; // Осмотр

        static Outcome instance = null;
        static object flock = new object();

        public static Outcome Instance {
            get {
                if (instance == null) lock (flock) {
                    if (instance == null) {
                        instance = new Outcome();
                        instance.Load(XML_NAME);
                    }
                }
                return instance;
            }
        }

        public static string Get(string condition, string outcome) {
            return Instance.GetDefault(condition + '-' + outcome, DEFAULT_VALUE);
        }

        Outcome() { }
    }
}
