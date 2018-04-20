using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using civox.Lib;

namespace civox.Model {
    /// <summary>
    /// Implements ZAP record of the invoice file
    /// </summary>
    class InvoiceRecord : Model {
        int number;

        // Original SN_POL value
        string policyCompound = string.Empty;

        public bool IsUpdated;
        public long PersonId;
        
        // T_POL values (1, 3) seems to match F008
        public int PolicyKind;

        public string PolicySerial { get; private set; }
        public string PolicyNumber { get; private set; }
        public string SmoCode;
        public bool IsNewborn;

        /// <summary>
        /// Get/set policy serial and number separated by space
        /// </summary>
        public string Policy {
            get {
                if (string.IsNullOrEmpty(PolicySerial))
                    return PolicyNumber;
                else
                    return PolicySerial + " " + PolicyNumber;
            }
            set { SetPolicy(value); }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="number">Ordinal number of the record in succession</param>
        public InvoiceRecord(int number) {
            this.number = number;
        }

        void SetPolicy(string value) {
            policyCompound = value;
            if (Lib.UniqueNumber.Valid(value)) {
                PolicySerial = string.Empty;
                PolicyNumber = new string(value.Where(c => c != ' ').ToArray());
            } else {
                string[] parts = value.Split(' ');
                if (parts.Length != 2) return;
                PolicySerial = parts[0];
                PolicyNumber = parts[1];
            }
        }
        
        /// <summary>
        /// Save record to XML exporter
        /// </summary>
        /// <param name="xml">XML exporter this record to write to</param>
        /// <param name="provider">Data provider</param>
        public override void Write(Lib.XmlExporter xml, Data.IDataProvider provider) {
            xml.Writer.WriteStartElement("ZAP");
            
            xml.Writer.WriteElementString("N_ZAP", number.ToString());
            WriteBool("PR_NOV", IsUpdated, xml);

            // PATIENT record
            xml.Writer.WriteStartElement("PACIENT");
            xml.Writer.WriteElementString("ID_PAC", PersonId.ToString());
            xml.Writer.WriteElementString("VPOLIS", PolicyKind.ToString());
            WriteIfValid("SPOLIS", PolicySerial, xml);
            xml.Writer.WriteElementString("NPOLIS", PolicyNumber);
            xml.Writer.WriteElementString("OKATO", Options.OKATO);
            xml.Writer.WriteElementString("SMO", SmoCode);
            WriteBool("NOVOR", IsNewborn, xml);
            xml.Writer.WriteEndElement();

            // SLUCH records
            List<Recourse> rs = provider.GetInvoiceRepository().LoadRecourceCases(policyCompound).ToList();
            // Just testing
            foreach (Recourse r in rs) {
                switch (r.Reason) {
                    case Reason.AmbTreatment:
                        WriteTreatment(r, xml, provider);
                        break;
                }
            }

            xml.Writer.WriteEndElement();
        }

        void WriteTreatment(Recourse rec, Lib.XmlExporter xml, Data.IDataProvider provider) {
            List<Service> ss = provider.GetInvoiceRepository().LoadServices(policyCompound, rec.Diagnosis).ToList();
            if (ss.Count == 0) return;

            RecourseLandmarks marks = Service.ArrangeServices(ss);
            if (!marks.Valid) {
                Lib.Logger.Log(string.Format("Услуги не формируют случай обращения:\r\n\t'{0}', DS {1}, отделение {2}, повод обращения {3}",
                    policyCompound,
                    rec.Diagnosis,
                    rec.Department,
                    rec.Reason));
                return;
            }

            xml.Writer.WriteStartElement("SLUCH");

            xml.Writer.WriteElementString("IDSERV", marks.Resulting.ID.ToString());
            xml.Writer.WriteElementString("USL_OK", "1"); // Условия оказания МП - Амбулаторая
            xml.Writer.WriteElementString("VIDPOM", "1"); // Вид - Первичная МСП
            xml.Writer.WriteElementString("FOR_POM", "3"); // Форма - Плановая

            xml.Writer.WriteElementString("VID_HMP", string.Empty);   // Вид ВМП - нет
            xml.Writer.WriteElementString("METOD_HMP", string.Empty); // Метод ВМП - нет
            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            xml.Writer.WriteElementString("VBR", string.Empty);                // Выездная бригада - нет

            xml.Writer.WriteElementString("PROFIL", marks.Last.AidKind);       // Профиль МП V002

            // Педиатрический профиль
            if (Options.Pediatric)
                xml.Writer.WriteElementString("DET", "1");
            else
                xml.Writer.WriteElementString("DET", string.Empty);
            
            xml.Writer.WriteElementString("TAL_D", string.Empty);           // Дата талона ВМП
            xml.Writer.WriteElementString("TAL_P", string.Empty);           // Дата запланир. госпит.

            // Номер амб карты/истории болезни
            xml.Writer.WriteElementString("NHISTORY", marks.Resulting.CardNumber);

            xml.Writer.WriteElementString("P_OTK", string.Empty);           // Признак отказа

            xml.Writer.WriteElementString("DATE_1", marks.First.Date.AsXml());
            xml.Writer.WriteElementString("DATE_2", marks.Last.Date.AsXml());
            xml.Writer.WriteElementString("DS1", rec.Diagnosis);
            
            // Диагноз первичный
            if (rec.FirstRevealed)
                xml.Writer.WriteElementString("DS1_PR", "1");

            xml.Writer.WriteElementString("RSLT", marks.Resulting.ResultCode);     // V009
            xml.Writer.WriteElementString("RSLT_D", string.Empty);         // TODO: 
            xml.Writer.WriteElementString("ISHOD", string.Empty);        // TODO: Исход заболевания V012
            xml.Writer.WriteElementString("PRVS", string.Empty);         // TODO: Специальность врача V015
            xml.Writer.WriteElementString("VERS_SPEC", "V015");          // Имя справочника специальностей

            xml.Writer.WriteElementString("IDDOKT", ss.OrderBy(s => s.Date).Last().DoctorCode);

            // TODO: Список особых случаев from D_TYPE
            foreach (string sc in ss.Select(s => s.SpecialCase).Where(s => !string.IsNullOrEmpty(s)).Distinct())
                xml.Writer.WriteElementString("OS_SLUCH", sc);

            xml.Writer.WriteElementString("IDSP", string.Empty);         // TODO: Способ оплаты V010
            xml.Writer.WriteElementString("ED_COL", "1");                // К-во единиц оплаты

            // TODO: Цель обращения кроме лечебной
            if (marks.Resulting.ServiceCode % 2 == 0)
                xml.Writer.WriteElementString("CEL", "17"); // Лечебная цель, подушевой тариф
            else
                xml.Writer.WriteElementString("CEL", "16"); // Лечебная цель, самостоятельный тариф

            // TODO: Тариф
            // UPDATE: Хуй там! В релаксе подушевые суммы по нулям
            xml.Writer.WriteElementString("TARIF", string.Empty);
            // TODO: Сумма к оплате - запятую на точку
            xml.Writer.WriteElementString("SUMV", string.Format("{0:f2}", ss.Sum(s => s.Price)));

            xml.Writer.WriteElementString("OPLATA", "1");                // Оплата - 1 - полная

            foreach (Service s in ss) {
                xml.Writer.WriteStartElement("USL");
                xml.Writer.WriteElementString("IDSERV", s.ID.ToString());
                xml.Writer.WriteElementString("LPU", Options.LpuCode);
                xml.Writer.WriteElementString("PODR", rec.Department);

                //PROFIL		Профиль МП. Классификатор V002 Приложения А
                //DET		Принак детского профиля. 0 - нет, 1 - да
                xml.Writer.WriteElementString("DATE_IN", s.Date.AsXml());
                xml.Writer.WriteElementString("DATE_OUT", s.Date.AsXml());
                xml.Writer.WriteElementString("DS", rec.Diagnosis);
                xml.Writer.WriteElementString("P_OTK", string.Empty);            // Признак отказа
                xml.Writer.WriteElementString("CODE_USL", s.ServiceCode.ToString()); // Код услуги
                xml.Writer.WriteElementString("KOL_USL", s.Quantity.ToString()); // Кратность услуги
                xml.Writer.WriteElementString("TARIF", string.Empty);            // TODO:
                xml.Writer.WriteElementString("SUMV_USL", string.Format("{0:f2}", s.Price));
                xml.Writer.WriteElementString("PRVS", string.Empty);         // TODO: Специальность врача V015
                xml.Writer.WriteElementString("CODE_MD", s.DoctorCode);          // Код медицинского работника

                xml.Writer.WriteEndElement();
            }

            xml.Writer.WriteEndElement();
        }
    }
}
