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
        /// Заменить исход заболевания на валидный, если он не соответствует результату обращения
        /// </summary>
        /// <param name="resultCode">Результат обращения</param>
        /// <param name="outcomeCode">Исход заболевания</param>
        /// <returns>Исход заболевания - тот же или исправленный</returns>
        public string Repair(string resultCode, string outcomeCode) {
            var matches = items.Where(i => i.Result == resultCode).Select(i => i.Outcome);

            if (matches == null || matches.Count() == 0)
                return outcomeCode;

            if (matches.Contains(outcomeCode)) {
                return outcomeCode;
            } else {
                return matches.First();
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
