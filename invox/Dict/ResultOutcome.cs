using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace invox.Dict {
    /// <summary>
    /// Соответствие результата обращения V009 исходу заболевания V012
    /// </summary>
    class ResultOutcome {
        const string XML_NAME = "\\Dict\\ResultOutcome.xml";
        static string[] RESULTS_FOR_OUTCOME_306 = {
            "301", "305", "308", "314", "315"
        };

        ResultOutcomeItem[] items;

        static ResultOutcome instance = null;
        static object flock = new object();

        public static ResultOutcome Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new ResultOutcome();
                        }
                    }
                return instance;
            }
        }

        ResultOutcome() {
            var serializer = new XmlSerializer(typeof(ResultOutcomeItem[]));
            XmlReaderSettings settings = new XmlReaderSettings();

            string xml = File.ReadAllText(Options.BaseDirectory + XML_NAME);

            using (StringReader sw = new StringReader(xml))
            using (XmlReader xw = XmlReader.Create(sw, settings)) {
                items = (ResultOutcomeItem[])serializer.Deserialize(xw);
            }
        }

        /// <summary>
        /// Исправить результат обращения или исход заболевания, если они не бьют друг другу
        /// </summary>
        /// <param name="rec"></param>
        public void Repair(invox.Model.Recourse rec) {
            if (rec.Outcome == "306") {
                if (!RESULTS_FOR_OUTCOME_306.Contains(rec.Result))
                    rec.Outcome = "304";
            } else {
                var matches = items.Where(i => i.Result == rec.Result).Select(i => i.Outcome);
                if (matches == null || matches.Count() == 0) return;

                if (!matches.Contains(rec.Outcome))
                    rec.Outcome = matches.First();
            }
        }
    }

    [Serializable]
    public class ResultOutcomeItem {
        [XmlAttribute("result")]
        public string Result { get; set; }

        [XmlAttribute("outcome")]
        public string Outcome { get; set; }

        public ResultOutcomeItem() { }
    }
}
