using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Model {
    /// <summary>
    /// Коэффициенты сложности лечения пациента  (ZL_LIST/ZAP/Z_SL/SL/KSG_KPG/SL_KOEF)
    /// Сведения о примененных коэффициентах сложности лечения пациента.
    /// Указывается при наличии IT_SL.
    /// </summary>
    class ComplexityQuotient {
        string code;
        double quotient;

        /// <summary>
        /// Номер коэффициента сложности лечения пациента
        /// В соответствии с региональным справочником.
        /// </summary>
        public string Code { get { return code; } }

        /// <summary>
        /// Значение коэффициента сложности лечения пациента
        /// </summary>
        public double Quotient { get { return quotient; } }

        public void Write(Lib.XmlExporter xml, Data.IInvoice pool) {
            xml.Writer.WriteStartElement("SL_KOEF");

            xml.Writer.WriteElementString("IDSL", code);
            xml.Writer.WriteElementString("Z_SL", quotient.ToString("F5", Options.NumberFormat));

            xml.Writer.WriteEndElement();
        }
    }

    /// <summary>
    /// Сведения о КСГ/КПГ (ZL_LIST/ZAP/Z_SL/SL/KSG_KPG)
    /// Заполняется при оплате случая лечения по КСГ или КПГ
    /// <remarks>
    /// Вложен в
    ///     Event Z_SL (Single)
    /// Содержит
    ///     ComplexityQuotient SL_KOEF (Multiple)
    /// </remarks>
    /// D1 - OK, D2, D3 - absent
    /// </summary>
    class ClinicalGroup {
        string ksgNumber;
        int version;
        bool subgroupUsed;
        string kpgNumber;

        double qexpenece;
        double qmanagement;
        double baseRate;
        double qdiff;
        double qlevel;

        string crit;

        bool kslpUsed;

        /// <summary>
        /// Номер КСГ
        /// Номер КСГ (V023) с указанием подгруппы (в случае использования).
        /// Заполняется при оплате случая лечения по КСГ. Не подлежит заполнению при заполненном N_KPG
        /// </summary>
        public string KsgNumber { get { return ksgNumber; } }

        /// <summary>
        /// Модель определения КСГ
        /// Указывается версия модели определения КСГ (год)
        /// </summary>
        public int Version { get { return version; } }

        /// <summary>
        /// Номер КПГ
        /// Номер КПГ (V026).
        /// Заполняется при оплате случая лечения по КПГ.
        /// Не подлежит заполнению при заполненном N_KSG
        /// </summary>
        public string KpgNumber { get { return kpgNumber; } }

        /// <summary>
        /// Признак использования подгруппы КСГ
        /// </summary>
        public bool SubgroupUsed { get { return subgroupUsed; } }

        /// <summary>
        /// Коэффициент затратоемкости
        /// Значение коэффициента затратоемкости группы/подгруппы КСГ или КПГ
        /// </summary>
        public double QuotExpense { get { return qexpenece; } }

        /// <summary>
        /// Управленческий коэффициент
        /// Значение управленческого коэффициента для КСГ или КПГ.
        /// При отсутствии указывается "1
        /// </summary>
        public double QuotManagement { get { return qmanagement; } }
        
        /// <summary>
        /// Базовая ставка
        /// Значение базовой ставки, указывается в рублях
        /// </summary>
        public double BaseRate { get { return baseRate; } }

        /// <summary>
        /// Коэффициент дифференциации
        /// </summary>
        public double QuotDifference { get { return qdiff; } }

        /// <summary>
        /// Коэффициент уровня/подуровня оказания медицинской помощи
        /// </summary>
        public double QuotGroupLevel { get { return qlevel; } }

        /// <summary>
        /// Признак использования КСЛП
        /// </summary>
        public bool KslpUsed { get { return kslpUsed; } }

        /// <summary>
        /// Классификационный критерий
        /// Классификационный критерий (V024), в том числе установленный субъектом Российской Федерации.
        /// Обязателен к заполнению:
        /// - в случае применения при оплате случая лечения по КСГ;
        /// - в случае применения при оплате случая лечения по КПГ, если применен региональный классификационный критерий
        /// </summary>
        public string AuxCriterion { get { return crit; } }

        public void Write(Lib.XmlExporter xml, Data.IInvoice pool, Event evt) {
            xml.Writer.WriteStartElement("KSG_KPG");

            xml.WriteIfValid("N_KSG", ksgNumber);
            xml.Writer.WriteElementString("VER_KSG", version.ToString());
            xml.WriteBool("KSG_PG", subgroupUsed);
            xml.WriteIfValid("N_KPG", kpgNumber);

            xml.Writer.WriteElementString("KOEF_Z", qexpenece.ToString("F5", Options.NumberFormat));
            xml.Writer.WriteElementString("KOEF_UP", qmanagement.ToString("F5", Options.NumberFormat));
            xml.Writer.WriteElementString("BZTSZ", baseRate.ToString("C2", Options.NumberFormat));
            xml.Writer.WriteElementString("KOEF_D", qdiff.ToString("F5", Options.NumberFormat));
            xml.Writer.WriteElementString("KOEF_U", qlevel.ToString("F5", Options.NumberFormat));

            xml.WriteIfValid("CRIT", crit);

            List<ComplexityQuotient> qs = pool.LoadComplexityQuotients().ToList();
            if (qs.Count() > 0) {
                double acc = qs.Select(q => q.Quotient).Aggregate(1.0, (a, b) => { return a * b; });
                if (acc > 0) {
                    xml.Writer.WriteElementString("IT_SL", acc.ToString("F5", Options.NumberFormat));
                    foreach (ComplexityQuotient q in qs)
                        q.Write(xml, pool);
                }
            }

            xml.Writer.WriteEndElement();
        }
    }
}
