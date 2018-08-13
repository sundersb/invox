using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using civox.Lib;

namespace civox.Model {
    enum OnkologyDirectionKind : int {
        None = 0,
        Onkologist = 1,
        Biopsy = 2,
        Study = 3
    }

    enum OnkologyDirectionMethod : int {
        None = 0,
        Laboratory = 1,
        Instrumental = 2,
        Ray = 3,
        RayExpensive = 4
    }

    /// <summary>
    /// Приказ 59 от 30.03.2018
    /// </summary>
    class OnkologyDirection : Model {
        DateTime date;
        OnkologyDirectionKind kind;
        OnkologyDirectionMethod method;
        string serviceCode;

        public OnkologyDirection(string tr, DateTime directionDate) {
            date = directionDate;
            if (tr.Length < 5) {
                kind = OnkologyDirectionKind.None;
                method = OnkologyDirectionMethod.None;
                serviceCode = "0";
            } else {
                kind = (OnkologyDirectionKind)((int)tr[0] - (int)'0');
                method = (OnkologyDirectionMethod)((int)tr[1] - (int)'0');
                serviceCode = tr.Substring(2);
            }
        }

        public override void Write(Lib.XmlExporter xml, Data.IInvoice repo) {
            if (kind == OnkologyDirectionKind.None || method == OnkologyDirectionMethod.None) return;

            xml.Writer.WriteStartElement("NAPR");

            xml.Writer.WriteElementString("NAPR_DATE", date.AsXml());
            xml.Writer.WriteElementString("NAPR_V", ((int)kind).ToString());
            xml.Writer.WriteElementString("MET_ISSL", ((int)method).ToString());
            xml.Writer.WriteElementString("NAPR_USL", serviceCode);

            xml.Writer.WriteEndElement();
        }
    }
}
