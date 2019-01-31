using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Data.Relax {
    /// <summary>
    /// Извлечение кода специальности V021 по коду подразделения и табельному номеру врача
    /// </summary>
    class SpecialityDict {
        static SpecialityDict instance = null;

        Dictionary<string, string> dict = new Dictionary<string, string>();
        static object flock = new object();

        static SpecialityDict Instance {
            get {
                if (instance == null) lock (flock) {
                        if (instance == null) {
                            instance = new SpecialityDict();
                        }
                    }
                return instance;
            }
        }

        SpecialityDict() { }

        /// <summary>
        /// Получить код специальности врача
        /// </summary>
        /// <param name="key">Код подразделения и код врача (S.OTD + S.TN1)</param>
        /// <returns>Код специальности V021</returns>
        static public string Get(string key) {
            return Instance.dict.ContainsKey(key) ? Instance.dict[key] : string.Empty;
        }

        /// <summary>
        /// Добавить значение в справочник
        /// </summary>
        static public void Append(string deptAndDoctor, string Code) {
            Instance.dict[deptAndDoctor] = Code;
        }
    }
}
