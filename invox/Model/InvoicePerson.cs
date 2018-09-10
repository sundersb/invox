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

    /// <summary>
    /// Сведения о пациенте (ZL_LIST/ZAP/PACIENT)
    /// <remarks>
    /// Вложен в
    ///     InvoiceRecord ZAP (Single)
    /// Содержит
    /// </remarks>
    /// D1, D2, D3 - OK
    /// </summary>
    class InvoicePerson {
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

        /// <summary>
        /// Save person data to invoice XML
        /// </summary>
        /// <param name="xml">XML exporter to write to</param>
        /// <param name="section">Order #59 section</param>
        public void Write(Lib.XmlExporter xml, OrderSection section) {
            switch(section) {
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
        }

        /// <summary>
        /// Save person data to treatment invoice
        /// </summary>
        /// <param name="xml">XML exporter to use</param>
        public void WriteD1(Lib.XmlExporter xml) {
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

            if (disability != Disability.NA)
                xml.Writer.WriteElementString("INV", ((int)disability).ToString());

            if (directedToSE)
                xml.Writer.WriteElementString("MSE", "1");

            if (!string.IsNullOrEmpty(newbornCode)) {
                xml.Writer.WriteElementString("NOVOR", newbornCode);
                xml.Writer.WriteElementString("VNOV_D", newbornWeight.ToString("D4"));
            }

            xml.Writer.WriteEndElement();
        }

        /// <summary>
        /// Save person data to hi-tech invoice
        /// </summary>
        /// <param name="xml">XML exporter to use</param>
        public void WriteD2(Lib.XmlExporter xml) {
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

            if (directedToSE)
                xml.Writer.WriteElementString("MSE", "1");

            if (!string.IsNullOrEmpty(newbornCode)) {
                xml.Writer.WriteElementString("NOVOR", newbornCode);
                xml.Writer.WriteElementString("VNOV_D", newbornWeight.ToString("D4"));
            }

            xml.Writer.WriteEndElement();
        }

        /// <summary>
        /// Save person data to dispanserisation invoice
        /// </summary>
        /// <param name="xml">XML exporter to use</param>
        public void WriteD3(Lib.XmlExporter xml) {
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
            if (!string.IsNullOrEmpty(newbornCode))
                xml.Writer.WriteElementString("NOVOR", newbornCode);

            xml.Writer.WriteEndElement();
        }
    }
}
