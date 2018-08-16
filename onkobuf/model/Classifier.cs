using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace onkobuf.model {
    /// <summary>
    /// Integral onkology stage classification record
    /// </summary>
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

        public Class(int anId, string aDS, int s, int t, int n, int m) {
            id = anId;
            ds = aDS;
            stage = s;
            tumor = t;
            nodus = n;
            metastasis = m;
        }

        public bool Matches(Stage s, Tumor t, Nodus n, Metastasis m) {
            return (s == null || s.ID == stage)
                && (t == null || t.ID == tumor)
                && (n == null || n.ID == nodus)
                && (m == null || m.ID == metastasis);
        }
    }

    /// <summary>
    /// Onkology stage dictionary
    /// </summary>
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

        /// <summary>
        ///  Get all dictionary records
        /// </summary>
        public static List<Class> All { get { return Instance.classifier; } }

        Classifier(string xmlName) {
            classifier = new List<Class>();
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlName);
            XmlElement root = xml.DocumentElement;
            foreach (XmlNode node in root.SelectNodes("zap")) {
                string id = node.SelectSingleNode("ID_gr").InnerText;
                string ds = node.SelectSingleNode("DS_gr").InnerText.ToUpper();
                string s = node.SelectSingleNode("ID_St").InnerText;
                string t = node.SelectSingleNode("ID_T").InnerText;
                string n = node.SelectSingleNode("ID_N").InnerText;
                string m = node.SelectSingleNode("ID_M").InnerText;
                classifier.Add(new Class(id, ds, s, t, n, m));
            }

            List<Stage> ss = Stages.byDiagnosis(string.Empty);
            List<Tumor> ts = Tumors.byDiagnosis(string.Empty);
            List<Nodus> ns = Nodules.byDiagnosis(string.Empty);
            List<Metastasis> ms = Metastases.byDiagnosis(string.Empty);

            int i = classifier.Max(c => c.ID);
            foreach (Stage stage in ss) {
                foreach (Tumor tumor in ts) {
                    foreach (Nodus nodus in ns) {
                        foreach (Metastasis mts in ms) {
                            ++i;
                            classifier.Add(new Class(i,
                                string.Empty,
                                stage.ID,
                                tumor.ID,
                                nodus.ID,
                                mts.ID));
                        }
                    }
                }
            }

            Stages.Clear();
            Tumors.Clear();
            Nodules.Clear();
            Metastases.Clear();
        }
    }
}
