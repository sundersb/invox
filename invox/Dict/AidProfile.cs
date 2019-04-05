using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace invox.Dict {
    /// <summary>
    /// Привязка профиля медицинской помощи к внутреннему коду Релакс KMU.MSP
    /// </summary>
    class AidProfile : Base {
        const string XML_NAME = "\\Dict\\V002-SLMSP.xml";

        static AidProfile instance = null;
        static object flock = new object();

        public static AidProfile Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new AidProfile();
                            instance.Load(XML_NAME);
                        }
                    }
                return instance;
            }
        }

        AidProfile() { }

        new public string Get(string key) {
            return GetDefault(key, "97");
        }
    }

    /// <summary>
    /// Получение кода профиля медицинской помощи, исходя из специальности врача
    /// </summary>
    /// <remarks>Для входа в проверку ФОМС от 03.2019 - соответствие профиля специальности</remarks>
    class AidProfileBySpeciality {
        const string XML_NAME = "\\Dict\\PRVS-PROFILE.xml";

        List<KeyValuePair<string, string>> dict = null;

        static AidProfileBySpeciality instance = null;
        static object flock = new object();

        public static AidProfileBySpeciality Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new AidProfileBySpeciality();
                            instance.Load(XML_NAME);
                        }
                    }
                return instance;
            }
        }

        AidProfileBySpeciality() {
            dict = new List<KeyValuePair<string, string>>();
        }

        bool Load(string fName) {
            string fileName = Options.BaseDirectory + fName;

            if (!File.Exists(fileName)) return false;

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (!fs.CanRead) return false;

            XDocument x = XDocument.Load(fs);
            if (x.Root.Name != "dict") return false;

            foreach (var n in x.Root.Elements()) {
                if (n.Name != "item") return false;

                string ins = n.Attribute("in").Value;
                string outs = n.Attribute("out").Value;
                dict.Add(new KeyValuePair<string,string>(ins, outs));
            }
            return true;
        }

        /// <summary>
        /// Получить профиль медпомощи, анализируя KMU.MSP и федеральный код специальности врача
        /// </summary>
        /// <param name="msp">Код МСП из Релакс (KMU.MSP)</param>
        /// <param name="specialityCode">Код специальности врача по федеральному справочнику</param>
        /// <returns></returns>
        public string Get(string msp, string specialityCode) {
            // Из мультиплексора - отобрать варианты профилей МП в зависимости от специальности
            var found = Instance.dict.Where(p => p.Key == specialityCode).Select(p => p.Value);

            string result = AidProfile.Instance.Get(msp);
            if (found.Count() == 0) {
                Lib.Logger.Log(string.Format("Нет профиля МП для специальности {0}", specialityCode));
            } else {
                if (!found.Contains(result)) {
                    // Если код из КМУ подходит, он и остается, в противном случае берем первый подходящий код МП
                    result = found.First();
                }
            }
            return result;
        }
    }
}
