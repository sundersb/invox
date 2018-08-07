﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace onkobuf.model {
    /// <summary>
    /// N003.xml
    /// </summary>
    class Tumor {
        int id;
        string ds;
        string stageCode;
        string title;

        public int ID { get { return id; } }
        public string Diagnosis { get { return ds; } }
        public string Code { get { return stageCode; } }
        public string DiagnosisCode { get { return stageCode + " (" + (string.IsNullOrEmpty(ds) ? "все" : ds) + ")"; } }
        public string Title { get { return title; } }

        public Tumor(string anId, string aDS, string aCode, string aTitle) {
            id = 0;
            int.TryParse(anId, out id);
            ds = aDS;
            stageCode = aCode;
            title = aTitle;
        }
    }

    class Tumors {
        const string XML_NAME = "N003.xml";

        static Tumors instance;
        static object flock = new object();

        static Tumors Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new Tumors(onkobuf.Options.ResourceDirectory + XML_NAME);
                        }
                    }
                return instance;
            }
        }

        List<Tumor> tumors = null;

        public static List<Tumor> All { get { return Instance.tumors; } }

        Tumors(string xmlName) {
            tumors = new List<Tumor>();
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlName);
            XmlElement root = xml.DocumentElement;
            foreach (XmlNode node in root.SelectNodes("zap")) {
                string id = node.SelectSingleNode("ID_T").InnerText;
                string ds = node.SelectSingleNode("DS_T").InnerText;
                string code = node.SelectSingleNode("KOD_T").InnerText;
                string title = node.SelectSingleNode("T_NAME").InnerText;
                tumors.Add(new Tumor(id, ds, code, title));
            }
        }

        public static List<Tumor> byDiagnosis(string DS) {
            Tumor[] ss = null;

            if (!string.IsNullOrEmpty(DS)) {
                if (DS.Length == 3)
                    ss = Instance.tumors.Where(s => s.Diagnosis == DS).ToArray();
                else
                    ss = Instance.tumors.Where(s => s.Diagnosis.Contains(DS)).ToArray();
            }

            if (ss == null || ss.Length == 0)
                ss = Instance.tumors.Where(s => string.IsNullOrEmpty(s.Diagnosis)).ToArray();

            return ss.ToList();
        }
    }
}
