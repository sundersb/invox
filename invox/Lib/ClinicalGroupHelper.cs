using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using invox.Model;

namespace invox.Lib {
    /// <summary>
    /// Вспомогательный класс для загрузки клинических групп
    /// </summary>
    static class ClinicalGroupHelper {
        const string XML_NAME = "\\Lib\\statq.xml";
        const decimal DEFAULT_DIFFERENTIAL_QUOTIENT = 1.4m;
        const decimal DEFAULT_GROUP_LEVEL = 1.0m;

        /// <summary>
        /// Индексатор клинико-статистических групп по коду
        /// </summary>
        public static GroupsCollection Groups { get; private set; }

        /// <summary>
        /// Загрузить справочник КСГ
        /// </summary>
        /// <returns></returns>
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
                Groups = new GroupsCollection(x.Root, year);
            }

            return true;
        }

        public class GroupsCollection {
            Dictionary<string, ClinicalGroup> items;

            /// <summary>
            /// Получить объект КСГ по коду КСГ
            /// </summary>
            /// <param name="index">Код КСГ</param>
            /// <returns>Клиническая группа</returns>
            public ClinicalGroup this[string index] {
                get {
                    ClinicalGroup item = null;
                    items.TryGetValue(index, out item);
                    return item;
                }
            }

            public GroupsCollection(XElement xmlRoot, int year) {
                items = new Dictionary<string, ClinicalGroup>();
                foreach (var n in xmlRoot.Elements()) {
                    if (n.Name != "item") {
                        Logger.Log("Ошибочный элемент в справочнике " + XML_NAME);
                        continue;
                    }
                    ClinicalGroup item = ReadItem(n);
                    item.Version = year;
                    items.Add(item.KsgNumber, item);
                }
            }

            static ClinicalGroup ReadItem(XElement node) {
                ClinicalGroup result = new ClinicalGroup() {
                    KsgNumber = node.Attribute("code").Value,
                    KpgNumber = null,
                    SubgroupUsed = false,
                    BaseRate = AsDecimal(node.Attribute("price").Value),
                    QuotExpense = AsDecimal(node.Attribute("rmq").Value),
                    QuotManagement = AsDecimal(node.Attribute("mq").Value),
                    QuotDifference = DEFAULT_DIFFERENTIAL_QUOTIENT,
                    QuotGroupLevel = DEFAULT_GROUP_LEVEL,
                    KslpUsed = false
                };
                //Title = node.Attribute("title").Value;

                return result;
            }

            static decimal AsDecimal(string value) {
                decimal result = 1.0m;
                decimal.TryParse(value, System.Globalization.NumberStyles.Number, invox.Options.NumberFormat, out result);
                return result;
            }
        }
    }
}
