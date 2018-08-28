using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Model {
    enum Disability : int {
        NA = -1,
        None = 0,
        Group1,
        Group2,
        Group3,
        Child
    }

    class InvoicePerson : Base {
        string id;
        string policyType;
        string policySerial;
        string policyNumber;

        string assuranceOkato;
        string smoCode;
        string smoOgrn;
        string smoOkato;
        string smoName;

        Disability disability;
        bool directedToSE;

        string newbornCode;
        int newbornWeight;

        public override void Write(Lib.XmlExporter xml, Data.IInvoice pool) {
            xml.Writer.WriteStartElement("PACIENT");

            xml.Writer.WriteElementString("ID_PAC", id);

            xml.Writer.WriteElementString("VPOLIS", policyType);
            xml.WriteIfValid("SPOLIS", policySerial);
            xml.WriteIfValid("NPOLIS", policyNumber);

            xml.WriteIfValid("ST_OKATO", assuranceOkato);
            xml.WriteIfValid("SMO", smoCode);
            xml.WriteIfValid("SMO_OGRN", smoOgrn);
            xml.WriteIfValid("SMO_OK", smoOkato);
            xml.WriteIfValid("SMO_NAM", smoName);

            switch(Options.Section) {
                case OrderSection.D1:
                    WriteD1(xml);
                    break;
                case OrderSection.D2:
                    WriteD2(xml);
                    break;
                case OrderSection.D3:
                    if (!string.IsNullOrEmpty(newbornCode))
                        xml.Writer.WriteElementString("NOVOR", newbornCode);
                    break;
            }

            xml.Writer.WriteEndElement();
        }

        void WriteD1(Lib.XmlExporter xml) {
            if (disability != Disability.NA)
                xml.Writer.WriteElementString("INV", ((int)disability).ToString());

            if (directedToSE)
                xml.Writer.WriteElementString("MSE", "1");

            if (!string.IsNullOrEmpty(newbornCode)) {
                xml.Writer.WriteElementString("NOVOR", newbornCode);
                xml.Writer.WriteElementString("VNOV_D", newbornWeight.ToString("D4"));
            }
        }

        void WriteD2(Lib.XmlExporter xml) {
            if (directedToSE)
                xml.Writer.WriteElementString("MSE", "1");

            if (!string.IsNullOrEmpty(newbornCode)) {
                xml.Writer.WriteElementString("NOVOR", newbornCode);
                xml.Writer.WriteElementString("VNOV_D", newbornWeight.ToString("D4"));
            }
        }
    }
}
