﻿using System.Collections.Generic;
using System.Linq;
using System.Xml;
using onkobuf.lib;

namespace onkobuf.model {
    /// <summary>
    /// N004.xml nodulus stages classifier
    /// </summary>
    class Nodus {
        int id;
        string ds;
        string stageCode;
        string title;

        public int ID { get { return id; } }
        public string Diagnosis { get { return ds; } }
        public string Code { get { return stageCode; } }
        public string DiagnosisCode { get { return stageCode + " (" + (string.IsNullOrEmpty(ds) ? "все" : ds) + ")"; } }
        public string Title { get { return title; } }

        public Nodus(string anId, string aDS, string aCode, string aTitle) {
            id = 0;
            int.TryParse(anId, out id);
            ds = aDS;
            stageCode = aCode;
            title = aTitle;
        }
    }

    /// <summary>
    /// Dictionary for the Nodus neoplazma stages
    /// </summary>
    class Nodules {
        const string XML_NAME = "N004.xml";

        static Nodules instance;
        static object flock = new object();

        static Nodules Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new Nodules(onkobuf.Options.ResourceDirectory + XML_NAME);
                        }
                    }
                return instance;
            }
        }

        List<Nodus> nodules = null;

        /// <summary>
        /// Get all dictionary records
        /// </summary>
        public static List<Nodus> All { get { return Instance.nodules; } }

        Nodules(string xmlName) {
            nodules = new List<Nodus>();
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlName);
            XmlElement root = xml.DocumentElement;
            foreach (XmlNode node in root.SelectNodes("zap")) {
                if (!lib.DateHelper.ValidNode(node)) continue;

                string id = node.SelectSingleNode("ID_N").InnerText;
                string ds = node.SelectSingleNode("DS_N").InnerText;
                string code = node.SelectSingleNode("KOD_N").InnerText.Capitalized();
                string title = node.SelectSingleNode("N_NAME").InnerText;
                nodules.Add(new Nodus(id, ds, code, title));
            }
        }

        /// <summary>
        /// Select nodules for a diagnosis
        /// </summary>
        /// <param name="DS">Diagnosis to limit selection</param>
        /// <returns>List of Nodulus stages relevant to the diagnosis provided</returns>
        /// <remarks>If there is no such diagnosis in the dictionary
        /// list of "no-diagnosis" records is returned</remarks>
        public static List<Nodus> byDiagnosis(string DS) {
            Nodus[] ss = null;

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
