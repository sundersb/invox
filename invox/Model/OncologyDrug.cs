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
        /// <summary>
        /// Идентификатор лекарственного препарата, применяемого при проведении лекарственной противоопухолевой терапии
        /// Заполняется в соответствии с классификатором N020 Приложения А
        /// </summary>
        public string DrugCode { get; set; }
        
        /// <summary>
        /// Код схемы лекарственной терапии
        /// Заполняется в соответствии с классификатором V024 Приложения А
        /// </summary>
        public string Schema { get; set; }
        
        /// <summary>
        /// Дата введения лекарственного препарата
        /// </summary>
        public DateTime Date { get; set; }

        public OncologyDrug(string drugCode, string schema) {
            DrugCode = drugCode;
            Schema = schema;
        }

        public void Write(XmlExporter xml) {
            xml.Writer.WriteStartElement("LEK_PR");

            xml.Writer.WriteElementString("REGNUM", DrugCode);
            xml.Writer.WriteElementString("CODE_SH", Schema);
            xml.Writer.WriteElementString("DATE_INJ", Date.AsXml());

            xml.Writer.WriteEndElement();
        }
    }
}
