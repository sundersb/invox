using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace onkobuf.model {
    class Class {
        int id;
        string ds;
        int stage;
        int tumor;
        int nodus;
        int metastasis;

        public int ID { get { return id; } }
        public string Diagnosis { get { return ds; } }

        public int Stage { get { return stage; } }
        public int Tumor { get { return tumor; } }
        public int Nodus { get { return nodus; } }
        public int Metastasis { get { return metastasis; } }

        public Class(string anId, string aDS, string s, string t, string n, string m) {
            id = 0;
            int.TryParse(anId, out id);
            ds = aDS;
            stage = 0;
            int.TryParse(s, out stage);
            tumor = 0;
            int.TryParse(t, out tumor);
            nodus = 0;
            int.TryParse(n, out nodus);
            metastasis = 0;
            int.TryParse(m, out metastasis);
        }

        public bool Matches(Stage s, Tumor t, Nodus n, Metastasis m) {
            return (s == null || s.ID == stage)
                && (t == null || t.ID == tumor)
                && (n == null || n.ID == nodus)
                && (m == null || m.ID == metastasis);
        }
    }

    class Classifier {
        const string XML_NAME = "N006.xml";

        static Classifier instance;
        static object flock = new object();

        static Classifier Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new Classifier(onkobuf.Options.ResourceDirectory + XML_NAME);
                        }
                    }
                return instance;
            }
        }

        List<Class> classifier = null;

        public static List<Class> All { get { return Instance.classifier; } }

        Classifier(string xmlName) {
            classifier = new List<Class>();
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlName);
            XmlElement root = xml.DocumentElement;
            foreach (XmlNode node in root.SelectNodes("zap")) {
                string id = node.SelectSingleNode("ID_gr").InnerText;
                string ds = node.SelectSingleNode("DS_gr").InnerText;
                string s = node.SelectSingleNode("ID_St").InnerText;
                string t = node.SelectSingleNode("ID_T").InnerText;
                string n = node.SelectSingleNode("ID_N").InnerText;
                string m = node.SelectSingleNode("ID_M").InnerText;
                classifier.Add(new Class(id, ds, s, t, n, m));
            }
        }

        //public static int Get(int s, int t, int n, int m) {
        //    Class found = Instance.classifier.FirstOrDefault(c => {
        //        return c.Stage == s
        //            && c.Tumor == t
        //            && c.Nodus == n
        //            && c.Metastasis == m;
        //    });

        //    if (found != null)
        //        return found.ID;
        //    else
        //        return 0;
        //}

        public static List<Class> Get(Stage s, Tumor t, Nodus n, Metastasis m) {
            var selection = from c in Instance.classifier
                            where c.Matches(s, t, n, m)
                            select c;
            return selection.ToList();
        }
    }
}
