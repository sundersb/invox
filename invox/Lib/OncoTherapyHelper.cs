using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using invox.Model;

namespace invox.Lib {
    class OncoTherapyHelper {
        const string XML_NAME = "\\Lib\\onco_drug_therapy.xml";

        const N013 DEFAULT_SERVICE_TYPE = N013.Medicamentous;
        const N014 DEFAULT_SURGICAL_CURE = N014.None;
        const N016 DEFAULT_CURE_CYCLE = N016.FollowingCycle;
        const bool DEFAULT_VOMIT_PROPHYLAX = false;
        const N017 DEFAULT_RAY_TREAT = N017.None;

        /// <summary>
        /// Коллекция онкологических услуг. После получения необходимо установить дату услуги!
        /// </summary>
        public static ServicesCollection Service { get; private set; }

        public static bool Init() {
            string fileName = invox.Options.BaseDirectory + XML_NAME;

            if (!File.Exists(fileName))
                return false;

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                if (!fs.CanRead)
                    return false;

                XDocument x = XDocument.Load(fs);
                if (x.Root.Name != "items") {
                    Logger.Log("Неверное имя корневого элемента в справочнике " + XML_NAME);
                    return false;
                }
                int year = DateTime.Today.Year;
                Service = new ServicesCollection(x.Root);
            }

            return true;
        }

        public class ServicesCollection {
            Dictionary<int, OncologyService> items;
            Dictionary<int, ClinicalGroup> groups;

            /// <summary>
            /// Онкологическая услуга
            /// </summary>
            /// <param name="index">Код услуги в Релакс КМУ</param>
            /// <returns>Услуга по онкологии. Для нее необходимо установить дату введения ЛС!</returns>
            public OncologyService this[int index] {
                get {
                    OncologyService item = null;
                    items.TryGetValue(index, out item);
                    return item;
                }
            }

            public ClinicalGroup ClinicalCroupForService(int serviceCode) {
                ClinicalGroup item = null;
                groups.TryGetValue(serviceCode, out item);
                return item;
            }

            public ServicesCollection(XElement xmlRoot) {
                items = new Dictionary<int, OncologyService>();
                groups = new Dictionary<int, ClinicalGroup>();

                foreach (var node in xmlRoot.Elements()) {
                    if (node.Name != "item") {
                        Logger.Log("Ошибочный элемент в справочнике " + XML_NAME);
                        continue;
                    }

                    int index = 0;
                    if (int.TryParse(node.Attribute("serv").Value, out index)) {
                        OncologyService item = ReadItem(node);
                        items.Add(index, item);

                        string ksgCode = node.Attribute("ksg").Value;
                        groups.Add(index, ClinicalGroupHelper.Groups[ksgCode]);
                    }
                }
            }

            OncologyService ReadItem(XElement node) {
                //string title = node.Value;

                OncologyService result = new OncologyService() {
                    ServiceType = DEFAULT_SERVICE_TYPE,
                    SurgicalCure = DEFAULT_SURGICAL_CURE,
                    Line = (N015) GetInt(node, "line"),
                    Cycle = DEFAULT_CURE_CYCLE,
                    CounterVomitCure = DEFAULT_VOMIT_PROPHYLAX,
                    RayKind = DEFAULT_RAY_TREAT,
                    Drugs = new OncologyDrug[] {
                        new OncologyDrug(node.Attribute("idls").Value, node.Attribute("schema").Value)
                    }
                };

                return result;
            }

            int GetInt(XElement node, string attr) {
                int result = 0;
                int.TryParse(node.Attribute(attr).Value, out result);
                return result;
            }
        }
    }
}
