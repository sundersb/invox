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
        /// <summary>
        /// Диагноз сопутствующего заболевания
        /// Код из справочника МКБ до уровня подрубрики. Указывается в случае установления в соответствии с медицинской документацией.
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// Установлен впервые (сопутствующий)
        /// </summary>
        public bool FirstIdentified { get; set; }
        
        /// <summary>
        /// Диспансерное наблюдение
        /// Указываются сведения о диспансерном наблюдении по поводу сопутствующего заболевания:
        /// 1 - состоит,
        /// 2 - взят,
        /// 3 - не подлежит диспансерному наблюдению
        /// </summary>
        public DispensarySupervision DispensarySupervision { get; set; }

        public void Write(Lib.XmlExporter xml) {
            xml.Writer.WriteStartElement("DS2_N");

            xml.Writer.WriteElementString("DS2", Code);
            if (FirstIdentified) xml.Writer.WriteElementString("DS2_PR", "1");
            xml.Writer.WriteElementString("PR_DS2_N", ((int)DispensarySupervision).ToString());

            xml.Writer.WriteEndElement();
        }
    }
}
