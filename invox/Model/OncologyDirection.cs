using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Lib;

namespace invox.Model {
    /// <summary>
    /// V028 Классификатор видов направления
    /// </summary>
    enum OncologyDirectionKind : int {
        None = 0,
        Onkologist = 1, // Направление к онкологу;
        Biopsy = 2,     // Направление на биопсию;
        Study = 3,      // Направление на дообследование
        Tactics = 4     // Направление для определения тактики обследования и/или тактики лечения
    }
    
    /// <summary>
    /// V029 Классификатор методов диагностического исследования
    /// </summary>
    enum OncologyDirectionMethod : int {
        None = 0,
        Laboratory = 1,
        Instrumental = 2,
        Ray = 3,
        RayExpensive = 4
    }

    /// <summary>
    /// Приказ 59 от 30.03.2018 (ZL_LIST/ZAP/Z_SL/SL/USL/NAPR)
    /// <remarks>
    /// Вложен в
    ///     Service USL (Multiple)
    /// </remarks>
    /// </summary>
    class OncologyDirection {
        DateTime date;
        OncologyDirectionKind kind;
        OncologyDirectionMethod method;
        string serviceCode;

        /// <summary>
        /// Дата направления
        /// </summary>
        public DateTime Date { get { return date; } }

        /// <summary>
        /// Код МО, куда оформлено направление
        /// Код МО - юридического лица. Заполняется в соответствии со справочником F003 Приложения А.
        /// Заполнение обязательно в случаях оформления направления в другую МО
        /// </summary>
        public string TargetClinic { get; set; }

        /// <summary>
        /// Вид направления
        /// Классификатор видов направления V028 Приложения А
        /// </summary>
        public OncologyDirectionKind Kind { get { return kind; } }

        /// <summary>
        /// Метод диагностического исследования
        /// Если NAPR_V=3
        /// </summary>
        public OncologyDirectionMethod Method { get { return method; } }

        /// <summary>
        /// Медицинская услуга (код), указанная в направлении
        /// Указывается в соответствии с номенклатурой медицинских услуг (V001). Обязательно к заполнению при заполненном MET_ISSL
        /// </summary>
        public string ServiceCode { get { return serviceCode; } }

        public OncologyDirection(string tr, DateTime directionDate) {
            date = directionDate;
            if (tr.Length < 5) {
                kind = OncologyDirectionKind.None;
                method = OncologyDirectionMethod.None;
                serviceCode = "0";
            } else {
                kind = (OncologyDirectionKind)charToEnum(tr[0], 3);
                if (kind == OncologyDirectionKind.Study) {
                    method = (OncologyDirectionMethod)charToEnum(tr[1], 4);
                    serviceCode = tr.Substring(2);
                } else {
                    method = OncologyDirectionMethod.None;
                }
            }
        }

        int charToEnum(char c, int max) {
            int result = (int)c - (int)'0';
            if (result > max)
                return 0;
            else
                return result;
        }

        public void Write(Lib.XmlExporter xml) {
            if (kind == OncologyDirectionKind.None || method == OncologyDirectionMethod.None) return;

            xml.Writer.WriteStartElement("NAPR");

            xml.Writer.WriteElementString("NAPR_DATE", date.AsXml());
            xml.WriteIfValid("NAPR_MO", TargetClinic);
            xml.Writer.WriteElementString("NAPR_V", ((int)kind).ToString());
            if (method != OncologyDirectionMethod.None) {
                xml.Writer.WriteElementString("MET_ISSL", ((int)method).ToString());
                xml.Writer.WriteElementString("NAPR_USL", serviceCode);
            }

            xml.Writer.WriteEndElement();
        }
    }
}
