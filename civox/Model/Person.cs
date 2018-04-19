using System;
using System.Collections.Generic;
using civox.Lib;

namespace civox.Model {
    //TODO: Representative fields

    /// <summary>
    /// A PERS record in the people XML
    /// </summary>
    class Person : Model {
        int sex;
        List<IdentityReliability> identityReliabilities = null;

        public long ID;
        public string Family;
        public string Name;
        public string Patronymic;

        /// <summary>
        /// Person's sex: 1 - male, 2 - female (V005) O
        /// </summary>
        public int Sex {
            get { return sex; }
            set {
                    if (value != 1 && value != 2)
                        throw new ArgumentException("Неправильный пол: " + value.ToString());
                    sex = value;
                }
        }

        /// <summary>
        /// Person's birthdate O
        /// </summary>
        public DateTime BirthDate;

        /// <summary>
        /// Person phone U (only for dispanserisation if presented)
        /// </summary>
        public string Phone;

        /// <summary>
        /// U Document type (F011)
        /// </summary>
        public long DocTypeId;

        public string DocumentSerial; // U
        public string DocumentNumber; // U

        public string SNILS;

        /// <summary>
        /// U
        /// </summary>
        public string Address;

        public string ResidenceOKATO; // U
        public string PresenceOKATO; // U

        public int SocialPosition;

        public Person() {
        }

        /// <summary>
        /// Add a DOST field (name or birthdate uncertainty)
        /// </summary>
        /// <param name="u">Which part of the patient's name abcent/invalid</param>
        public void AddPersonUncertainty(IdentityReliability u) {
            if (identityReliabilities == null) identityReliabilities = new List<IdentityReliability>();
            identityReliabilities.Add(u);
        }

        /// <summary>
        /// Save person to XML
        /// </summary>
        /// <param name="xml">XML export helper</param>
        public override void Write(Lib.XmlExporter xml, Data.IDataProvider provider) {
            if (!xml.OK) return;
            xml.Writer.WriteStartElement("PERS");

            xml.Writer.WriteElementString("ID_PAC", ID.ToString());

            if (string.IsNullOrEmpty(Family))
                AddPersonUncertainty(IdentityReliability.noFamily);
            else
                xml.Writer.WriteElementString("FAM", Family);

            if (string.IsNullOrEmpty(Name))
                AddPersonUncertainty(IdentityReliability.noName);
            else
                xml.Writer.WriteElementString("IM", Name);

            if (string.IsNullOrEmpty(Patronymic))
                AddPersonUncertainty(IdentityReliability.noPatronymic);
            else
                xml.Writer.WriteElementString("OT", Patronymic);

            xml.Writer.WriteElementString("W", sex.ToString());

            xml.Writer.WriteElementString("DR", BirthDate.AsXml());
            if (identityReliabilities != null) {
                foreach(IdentityReliability u in identityReliabilities)
                    xml.Writer.WriteElementString("DOST", ((int)u).ToString());
            }

            // Only for dispanserisation
            WriteIfValid("TEL", Phone, xml);
            WriteIfValid("MR", Address, xml);

            if (DocTypeId > 0)
                xml.Writer.WriteElementString("DOCTYPE", DocTypeId.ToString());

            WriteIfValid("DOCSER", DocumentSerial, xml);
            WriteIfValid("DOCNUM", DocumentNumber, xml);
            WriteIfValid("SNILS", SNILS, xml);
            WriteIfValid("OKATOG", ResidenceOKATO, xml);
            WriteIfValid("OKATOP", PresenceOKATO, xml);

            xml.Writer.WriteEndElement();
        }
    }
}
