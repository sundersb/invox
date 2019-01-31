using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Lib;

namespace invox.Model {
    /// <summary>
    /// Сведения о введенном противоопухолевом лекарственном препарате
    /// </summary>
    class OncologyDrug {
        string code;
        string schema;
        DateTime date;

        /// <summary>
        /// Идентификатор лекарственного препарата, применяемого при проведении лекарственной противоопухолевой терапии
        /// Заполняется в соответствии с классификатором N020 Приложения А
        /// </summary>
        public string DrugCode { get { return code; } }
        
        /// <summary>
        /// Код схемы лекарственной терапии
        /// Заполняется в соответствии с классификатором V024 Приложения А
        /// </summary>
        public string Schema { get { return schema; } }
        
        /// <summary>
        /// Дата введения лекарственного препарата
        /// </summary>
        public DateTime Date { get { return date; } }

        public void Write(XmlExporter xml) {
            xml.Writer.WriteStartElement("LEK_PR");

            xml.Writer.WriteElementString("REGNUM", code);
            xml.Writer.WriteElementString("CODE_SH", schema);
            xml.Writer.WriteElementString("DATE_INJ", date.AsXml());

            xml.Writer.WriteEndElement();
        }
    }
}
