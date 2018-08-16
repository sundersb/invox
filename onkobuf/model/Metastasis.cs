using System.Collections.Generic;
using System.Linq;
using System.Xml;
using onkobuf.lib;

namespace onkobuf.model {
    /// <summary>
    /// N005.xml metastases stage record
    /// </summary>
    class Metastasis {
        int id;
        string ds;
        string stageCode;
        string title;

        public int ID { get { return id; } }
        public string Diagnosis { get { return ds; } }
        public string Code { get { return stageCode; } }
        public string DiagnosisCode { get { return stageCode + " (" + (string.IsNullOrEmpty(ds) ? "все" : ds) + ")"; } }
        public string Title { get { return title; } }

        public Metastasis(string anId, string aDS, string aCode, string aTitle) {
            id = 0;
            int.TryParse(anId, out id);
            ds = aDS;
            stageCode = aCode;
            title = aTitle;
        }
    }

    /// <summary>
    /// Metastases classification dictionary
    /// </summary>
    class Metastases {
        const string XML_NAME = "N005.xml";

        static Metastases instance;
        static object flock = new object();

        static Metastases Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new Metastases(onkobuf.Options.ResourceDirectory + XML_NAME);
                        }
                    }
                return instance;
            }
        }

        List<Metastasis> nodules = null;

        /// <summary>
        /// Get all articles of the dictionary
        /// </summary>
        public static List<Metastasis> All { get { return Instance.nodules; } }

        Metastases(string xmlName) {
            nodules = new List<Metastasis>();
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlName);
            XmlElement root = xml.DocumentElement;
            foreach (XmlNode node in root.SelectNodes("zap")) {
                string id = node.SelectSingleNode("ID_M").InnerText;
                string ds = node.SelectSingleNode("DS_M").InnerText;
                string code = node.SelectSingleNode("KOD_M").InnerText.Capitalized();
                string title = node.SelectSingleNode("M_NAME").InnerText;
                nodules.Add(new Metastasis(id, ds, code, title));
            }
        }

        /// <summary>
        /// List possible metastasis stages for a diagnosis
        /// </summary>
        /// <param name="DS">Nozology to look for</param>
        /// <returns>List of metastasis stages relevant to the diagnosis provided</returns>
        /// <remarks>If no such diagnosis found in the dictionary
        /// list of "no-diagnosis" records is returned</remarks>
        public static List<Metastasis> byDiagnosis(string DS) {
            Metastasis[] ss = null;

            if (!string.IsNullOrEmpty(DS)) {
                if (DS.Length == 3)
                    ss = Instance.nodules.Where(s => s.Diagnosis == DS).ToArray();
                else
                    ss = Instance.nodules.Where(s => s.Diagnosis.Contains(DS)).ToArray();
            }

            if (ss == null || ss.Length == 0)
                ss = Instance.nodules.Where(s => string.IsNullOrEmpty(s.Diagnosis)).ToArray();

            return ss.ToList();
        }

        public static void Clear() {
            instance = null;
        }
    }
}
