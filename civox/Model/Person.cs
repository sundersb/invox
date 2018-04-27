using System;
using System.Collections.Generic;
using civox.Lib;

namespace civox.Model {
    //TODO: Representative fields

    /// <summary>
    /// A PERS record in the people XML
    /// </summary>
    class Person : Model {
        const string DEFAULT_DOCUMENT = "14";
        static char[] SEPARATORS = " \r\n\t".ToCharArray();

        int sex;
        List<IdentityReliability> identityReliabilities = null;

        public long ID;
        public string Family;
        public string Name;
        public string Patronymic;

        /// <summary>
        /// Социальное положение
        /// </summary>
        public int SocialPosition;

        /// <summary>
        /// Категория льгот
        /// </summary>
        public string SocialFavour;

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
        public string DocTypeId;

        public string DocumentSerial; // U
        public string DocumentNumber; // U

        public string SNILS;

        /// <summary>
        /// U
        /// </summary>
        public string Address;

        public string ResidenceOKATO; // U
        public string PresenceOKATO; // U
        
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
        public override void Write(Lib.XmlExporter xml, Data.IInvoice repo) {
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
            xml.WriteIfValid("TEL", Phone);
            xml.WriteIfValid("MR", Address);
            xml.WriteIfValid("DOCTYPE", DocTypeId);

            // ХКФОМС чудит:
            //  Element 'DOCSER': This element is not expected. Expected is ( SOC ).
            //  Element 'DOCNUM': This element is not expected. Expected is ( SOC )
            //xml.WriteIfValid("DOCSER", DocumentSerial);
            //xml.WriteIfValid("DOCNUM", DocumentNumber);

            // SOC (О, Т2) Социальный статус: Справочник "Социальный статус"
            xml.Writer.WriteElementString("SOC", SocialPosition.ToString());

            // KT (У, Т2) "Категория льготности"
            //xml.WriteIfValid("KT", SocialFavour);

            // Опять от ФОМС (без KT ошибки нет):
            //  Element 'SNILS': This element is not expected. Expected is ( COMENTP )
            if (string.IsNullOrEmpty(SocialFavour))
                xml.WriteIfValid("SNILS", SNILS);

            xml.WriteIfValid("OKATOG", ResidenceOKATO);
            xml.WriteIfValid("OKATOP", PresenceOKATO);

            xml.Writer.WriteEndElement();
        }

        /// <summary>
        /// Set document's serial and number
        /// </summary>
        /// <param name="number">Serial and number separated by space</param>
        /// <remarks>Value may have consist of a single word, two or even more words.
        /// Single word is considered document's number.
        /// Two words - serial and number correspondinly.
        /// If there are three or more words, the last of them is number,
        /// while the former ones are joined together to form document's serial.</remarks>
        public void SetDocument(string type, string number) {
            number = number.Trim();
            if (string.IsNullOrEmpty(number)) {
                DocTypeId = string.Empty;
                DocumentNumber = string.Empty;
                DocumentSerial = string.Empty;
            } else {
                DocTypeId = string.IsNullOrEmpty(type) ? DEFAULT_DOCUMENT : type;

                string[] parts = number.Split(SEPARATORS);
                int l = parts.Length - 1;
                DocumentNumber = parts[l];

                if (l > 0) {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(parts[0]);
                    for (int i = 1; i < l; ++i)
                        sb.Append(parts[i]);
                    DocumentSerial = sb.ToString();
                } else {
                    DocumentSerial = string.Empty;
                }
            }
        }
    }
}
