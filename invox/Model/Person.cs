using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Lib;

namespace invox.Model {
    class Representative : Base {
        string family;
        string name;
        string patronymic;
        int sex;

        DateTime birthDate;
        bool wrongDate;
        bool wrongMonth;

        void WritePersonIdentityError(Lib.XmlExporter xml) {
            if (string.IsNullOrEmpty(family)) xml.Writer.WriteElementString("DOST_P", "2");
            if (string.IsNullOrEmpty(name)) xml.Writer.WriteElementString("DOST_P", "3");
            if (string.IsNullOrEmpty(patronymic)) xml.Writer.WriteElementString("DOST_P", "1");
            if (wrongDate) {
                xml.Writer.WriteElementString("DOST_P", "6");
                xml.Writer.WriteElementString("DOST_P", "4");
            } else if (wrongMonth) {
                xml.Writer.WriteElementString("DOST_P", "6");
                xml.Writer.WriteElementString("DOST_P", "5");
            }
        }

        public override void Write(XmlExporter xml, Data.IInvoice pool) {
            xml.WriteIfValid("FAM_P", family);
            xml.WriteIfValid("IM_P", name);
            xml.WriteIfValid("OT_P", patronymic);
            xml.Writer.WriteElementString("W_P", sex.ToString());
            xml.Writer.WriteElementString("DR_P", birthDate.AsXml());

            WritePersonIdentityError(xml);
        }
    }

    class Person : Base {
        string id;
        string family;
        string name;
        string patronymic;
        int sex;

        DateTime birthDate;
        bool wrongDate;
        bool wrongMonth;

        string phone;

        Representative representative;

        string birthPlace;

        string doctype;
        string docSerial;
        string docNumber;

        string snils;
        string residenceOkato;
        string presenceOkato;
        string comment;

        void WritePersonIdentityError(Lib.XmlExporter xml) {
            if (string.IsNullOrEmpty(family)) xml.Writer.WriteElementString("DOST", "2");
            if (string.IsNullOrEmpty(name)) xml.Writer.WriteElementString("DOST", "3");
            if (string.IsNullOrEmpty(patronymic)) xml.Writer.WriteElementString("DOST", "1");
            if (wrongDate) {
                xml.Writer.WriteElementString("DOST", "6");
                xml.Writer.WriteElementString("DOST", "4");
            } else if (wrongMonth) {
                xml.Writer.WriteElementString("DOST", "6");
                xml.Writer.WriteElementString("DOST", "5");
            }
        }

        public override void Write(Lib.XmlExporter xml, Data.IInvoice pool) {
            xml.Writer.WriteStartElement("PERS");

            xml.WriteIfValid("ID_PAC", id);
            xml.WriteIfValid("FAM", family);
            xml.WriteIfValid("IM", name);
            xml.WriteIfValid("OT", patronymic);
            xml.Writer.WriteElementString("W", sex.ToString());
            xml.Writer.WriteElementString("DR", birthDate.AsXml());

            WritePersonIdentityError(xml);

            if (Options.Section == OrderSection.D3) xml.WriteIfValid("TEL", phone);

            if (representative != null) representative.Write(xml, pool);

            xml.WriteIfValid("MR", birthPlace);
            xml.WriteIfValid("DOCTYPE", doctype);
            xml.WriteIfValid("DOCSER", doctype);
            xml.WriteIfValid("DOCNUM", doctype);

            xml.WriteIfValid("SNILS", snils);
            xml.WriteIfValid("OKATOG", residenceOkato);
            xml.WriteIfValid("OKATOP", presenceOkato);
            xml.WriteIfValid("COMENTP", comment);

            xml.Writer.WriteEndElement();
        }
    }
}
