using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Lib;

namespace invox.Model {
    /// <summary>
    /// Классификатор целей консилиума N019
    /// </summary>
    enum OncologyConsiliumReason : int {
        NotNeeded = 0,           // Отсутствует необходимость проведения консилиума
        Study = 1,               // Определена тактика обследования
        Treatment = 2,           // Определена тактика лечения
        TreatmentCorrection = 3, // Изменена тактика лечения
        Failed = 4               // Консилиум не проведен при наличии необходимости его проведения
    }

    /// <summary>
    /// Сведения о проведении консилиума
    /// Содержит сведения о проведении консилиума в целях определения тактики обследования или лечения.
    /// Обязательно к заполнению при установленном диагнозе злокачественного новообразования
    /// (первый символ кода основного диагноза - "C" или код основного диагноза входит в диапазон
    /// D00 - D09) и нейтропении (код основного диагноза - D70 с сопутствующим диагнозом C00 - C80 или C97).
    /// </summary>
    class OncologyConsilium {
        /// <summary>
        /// Цель проведения консилиума
        /// Классификатор целей консилиума N019 Приложения А
        /// </summary>
        public OncologyConsiliumReason Reason { get; set; }

        /// <summary>
        /// Дата проведения консилиума
        /// Обязательно к заполнению, если консилиум проведен (PR_CONS={1,2,3})
        /// </summary>
        public DateTime Date { get; set; }

        public OncologyConsilium(OncologyConsiliumReason reason, DateTime date) {
            Reason = reason;
            Date = date;
        }

        public void Write(Lib.XmlExporter xml) {
            xml.Writer.WriteStartElement("CONS");
            xml.Writer.WriteElementString("PR_CONS", ((int) Reason).ToString());
            if (Reason >= OncologyConsiliumReason.Study && Reason <= OncologyConsiliumReason.TreatmentCorrection)
                xml.Writer.WriteElementString("DT_CONS", Date.AsXml());
            xml.Writer.WriteEndElement();
        }
    }
}
