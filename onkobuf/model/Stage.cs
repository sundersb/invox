using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace onkobuf.model {
    /// <summary>
    /// N002.xml
    /// </summary>
    class Stage {
        int id;
        string ds;
        string stageCode;

        public int ID { get { return id; } }
        public string Diagnosis { get { return ds; } }
        public string Code { get { return stageCode; } }
        public string DiagnosisCode { get { return stageCode + " (" + (string.IsNullOrEmpty(ds) ? "все" : ds) + ")"; } }

        public Stage(string anId, string aDS, string aCode) {
            id = 0;
            int.TryParse(anId, out id);
            ds = aDS;
            stageCode = aCode;
        }
    }

    class Stages {
        const string XML_NAME = "N002.xml";

        static Stages instance;
        static object flock = new object();

        static Stages Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new Stages(onkobuf.Options.ResourceDirectory + XML_NAME);
                        }
                    }
                return instance;
            }
        }

        List<Stage> stages = null;

        public static List<Stage> All { get { return Instance.stages; } }

        Stages(string xmlName) {
            stages = new List<Stage>();
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlName);
            XmlElement root = xml.DocumentElement;
            foreach (XmlNode node in root.SelectNodes("zap")) {
                string id = node.SelectSingleNode("ID_St").InnerText;
                string ds = node.SelectSingleNode("DS_St").InnerText;
                string code = node.SelectSingleNode("KOD_St").InnerText;
                stages.Add(new Stage(id, ds, code));
            }
        }

        public static List<Stage> byDiagnosis(string DS) {
            Stage[] ss = null;

            if (!string.IsNullOrEmpty(DS)) {
                if (DS.Length == 3)
                    ss = Instance.stages.Where(s => s.Diagnosis == DS).ToArray();
                else
                    ss = Instance.stages.Where(s => s.Diagnosis.Contains(DS)).ToArray();
            }

            if (ss == null || ss.Length == 0)
                ss = Instance.stages.Where(s => string.IsNullOrEmpty(s.Diagnosis)).ToArray();

            return ss.ToList();
        }
    }
}
