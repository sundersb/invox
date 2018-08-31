using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Lib;

namespace invox.Model {
    enum OncologyDirectionKind : int {
        None = 0,
        Onkologist = 1, // Направление к онкологу;
        Biopsy = 2,     // Направление на биопсию;
        Study = 3       // Направление на дообследование
    }
    
    enum OncologyDirectionMethod : int {
        None = 0,
        Laboratory = 1,
        Instrumental = 2,
        Ray = 3,
        RayExpensive = 4
    }

    /// <summary>
    /// Приказ 59 от 30.03.2018 (SL_LIST/ZAP/Z_SL/SL/USL/NAPR)
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
        /// Вид направления
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
                method = (OncologyDirectionMethod)charToEnum(tr[1], 4);
                serviceCode = tr.Substring(2);
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
            xml.Writer.WriteElementString("NAPR_V", ((int)kind).ToString());
            xml.Writer.WriteElementString("MET_ISSL", ((int)method).ToString());
            xml.Writer.WriteElementString("NAPR_USL", serviceCode);

            xml.Writer.WriteEndElement();
        }
    }
}
