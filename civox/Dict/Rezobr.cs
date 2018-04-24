namespace civox.Dict {
    class Rezobr : Base {
        const string XML_NAME = "\\Dict\\V009-BE.xml";

        static Rezobr instance = null;
        static object flock = new object();

        public static Rezobr Instance {
            get {
                if (instance == null) lock (flock) {
                    if (instance == null) {
                        instance = new Rezobr();
                        instance.Load(XML_NAME);
                    }
                }
                return instance;
            }
        }
        
        Rezobr() {}
    }
}
