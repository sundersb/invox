using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Model {
    /// <summary>
    /// Сопутствующее заболевание для D3 (ZL_LIST/ZAP/Z_SL/SL/DS2_N)
    /// <remarks>
    /// Вложен в
    ///     Event SL
    /// </remarks>
    /// </summary>
    class ConcomitantDisease {
        string code;
        bool firstIdentified;
        DispensarySupervision dispSupervision;

        /// <summary>
        /// Диагноз сопутствующего заболевания
        /// Код из справочника МКБ до уровня подрубрики. Указывается в случае установления в соответствии с медицинской документацией.
        /// </summary>
        public string Code { get { return code; } }
        
        /// <summary>
        /// Установлен впервые (сопутствующий)
        /// </summary>
        public bool FirstIdentified { get { return firstIdentified; } }
        
        /// <summary>
        /// Диспансерное наблюдение
        /// Указываются сведения о диспансерном наблюдении по поводу сопутствующего заболевания:
        /// 1 - состоит,
        /// 2 - взят,
        /// 3 - не подлежит диспансерному наблюдению
        /// </summary>
        public DispensarySupervision DispensarySupervision { get { return dispSupervision; } }

        public void Write(Lib.XmlExporter xml) {
            xml.Writer.WriteStartElement("DS2_N");

            xml.Writer.WriteElementString("DS2", code);
            if (firstIdentified) xml.Writer.WriteElementString("DS2_PR", "1");
            xml.Writer.WriteElementString("PR_DS2_N", ((int)dispSupervision).ToString());

            xml.Writer.WriteEndElement();
        }
    }
}
