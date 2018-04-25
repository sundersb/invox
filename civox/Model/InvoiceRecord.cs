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
        
        // T_POL values (1, 3) seem to match F008
        public int PolicyKind;

        public string PolicySerial { get; private set; }
        public string PolicyNumber { get; private set; }
        public string SmoCode;
        public string OKATO;
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
        /// Save record
        /// </summary>
        /// <param name="xml">XML exporter this record to write to</param>
        /// <param name="provider">Data provider</param>
        public override void Write(Lib.XmlExporter xml, Data.IInvoice repo) {
            xml.Writer.WriteStartElement("ZAP");
            
            xml.Writer.WriteElementString("N_ZAP", number.ToString());
            WriteBool("PR_NOV", IsUpdated, xml);

            // PATIENT record
            xml.Writer.WriteStartElement("PACIENT");
            xml.Writer.WriteElementString("ID_PAC", PersonId.ToString());
            xml.Writer.WriteElementString("VPOLIS", PolicyKind.ToString());
            WriteIfValid("SPOLIS", PolicySerial, xml);
            xml.Writer.WriteElementString("NPOLIS", PolicyNumber);
            xml.Writer.WriteElementString("OKATO", OKATO);
            xml.Writer.WriteElementString("SMO", SmoCode);
            WriteBool("NOVOR", IsNewborn, xml);
            xml.Writer.WriteEndElement();

            // SLUCH records

            // ToList() soasto check lazyness and free the connection for subqueries
            List<Recourse> rs = repo.LoadRecourceCases(policyCompound).ToList();

            foreach (Recourse r in rs) {
                List<Service> services = repo.LoadServices(policyCompound, r.Diagnosis, r.Department).ToList();

                if (ReasonHelper.IsSingleDay(r.Reason)) {
                    // List of services may contain several recourses (Emergency, Prof, Other etc.)
                    foreach (IGrouping<DateTime, Service> group in services.GroupBy(s => s.EndDate))
                        WriteRecourse(r, xml, group.ToList());
                } else {
                    WriteRecourse(r, xml, services);
                }
            }

            xml.Writer.WriteEndElement();
        }

        /// <summary>
        /// Write SLUCH record
        /// </summary>
        void WriteRecourse(Recourse rec, Lib.XmlExporter xml, List<Service> services) {
            if (services.Count == 0) return;

            RecourseLandmarks marks = Service.ArrangeServices(services);
            if (!marks.Valid) {
                Lib.Logger.Log(string.Format("Услуги не формируют случай обращения:\r\n\t'{0}', DS {1}, отделение {2}, повод обращения {3}",
                    policyCompound,
                    rec.Diagnosis,
                    rec.Department,
                    rec.Reason));
                return;
            }

            // В приказе этот узел зовется Z_SL. У нас вот так:
            xml.Writer.WriteStartElement("SLUCH");

            xml.Writer.WriteElementString("IDSERV", marks.Resulting.ID.ToString());

            // V006 Условия оказания МП
            xml.Writer.WriteElementString("USL_OK", rec.Condition);

            // Вид - Первичная МСП V008
            xml.Writer.WriteElementString("VIDPOM", marks.Resulting.AidKind);
            
            // Форма - Плановая V014
            xml.Writer.WriteElementString("FOR_POM", marks.Resulting.AidForm);

            // TODO: Признак поступления/перевода. Наш ФОМС игнорирует(?)
            // Обязательно для дневного и круглосуточного стационара.
            //1 – Самостоятельно
            //2 – СМП
            //3 – Перевод из другой МО
            //4 – Перевод внутри МО с другого профиля
            xml.Writer.WriteElementString("P_PER", "3");

            xml.Writer.WriteElementString("VID_HMP", string.Empty);            // TODO: Вид ВМП - нет
            xml.Writer.WriteElementString("METOD_HMP", string.Empty);          // TODO: Метод ВМП - нет
            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            xml.Writer.WriteElementString("VBR", string.Empty);                // TODO: Выездная бригада - нет

            // Профиль МП V002
            xml.Writer.WriteElementString("PROFIL", marks.Last.AidProfile);

            // Педиатрический
            WriteBool("DET", Options.Pediatric, xml);

            // TODO: Дата талона ВМП
            xml.Writer.WriteElementString("TAL_D", string.Empty);

            // TODO: Дата запланир. госпит.
            xml.Writer.WriteElementString("TAL_P", string.Empty);

            xml.Writer.WriteElementString("NHISTORY", marks.Resulting.CardNumber);

            // Признак отказа
            WriteBool("P_OTK", services.Any(s => s.Refusal), xml);

            if (rec.Reason == Reason.DayHosp || rec.Reason == Reason.SurgeryDayHosp) {
                xml.Writer.WriteElementString("DATE_1", marks.Resulting.BeginDate.AsXml());
                xml.Writer.WriteElementString("DATE_2", marks.Resulting.EndDate.AsXml());
            } else {
                xml.Writer.WriteElementString("DATE_1", marks.First.BeginDate.AsXml());
                xml.Writer.WriteElementString("DATE_2", marks.Last.EndDate.AsXml());
            }
            xml.Writer.WriteElementString("DS1", rec.Diagnosis);
            
            // Диагноз первичный
            if (rec.FirstRevealed)
                xml.Writer.WriteElementString("DS1_PR", "1");

            // V009
            xml.Writer.WriteElementString("RSLT", marks.Resulting.ResultCode);
            if (rec.IsDispanserisation()) {
                // TODO: Результат диспансеризации
                xml.Writer.WriteElementString("RSLT_D", string.Empty);
            }

            // Исход заболевания V012
            xml.Writer.WriteElementString("ISHOD", rec.Outcome);

            // Специальность врача V015
            xml.Writer.WriteElementString("PRVS", marks.Resulting.DoctorProfile);
            xml.Writer.WriteElementString("VERS_SPEC", "V015");

            xml.Writer.WriteElementString("IDDOKT", services.OrderBy(s => s.EndDate).Last().DoctorCode);

            // Список особых случаев from D_TYPE
            // TODO: ХКФОМС вкладывает в OS_SLUCH свой особый смысл. Не тот, что ФФОМС.
            foreach (string sc in services.Select(s => s.SpecialCase).Where(s => !string.IsNullOrEmpty(s)).Distinct())
                xml.Writer.WriteElementString("OS_SLUCH", sc);

            // Способ оплаты V010
            xml.Writer.WriteElementString("IDSP", marks.Resulting.PayKind);

            // К-во единиц оплаты
            xml.Writer.WriteElementString("ED_COL", marks.Resulting.Quantity.ToString());
            // Продолжительность госпитализации (койко-дни/пациенто-дни). ХКФОМС игнорирует(?)
            if (marks.Resulting.Quantity > 1)
                xml.Writer.WriteElementString("KD_Z", marks.Resulting.Quantity.ToString());

            // Цель обращения
            xml.Writer.WriteElementString("CEL", marks.Resulting.RecourseAim);

            // TODO: Тариф
            // UPDATE: Хуй там! В релаксе подушевые суммы по нулям
            xml.Writer.WriteElementString("TARIF", string.Empty);
            
            // Сумма к оплате
            xml.Writer.WriteElementString("SUMV", string.Format(Options.NumberFormat, "{0:f2}", services.Sum(s => s.Price)));
            
            // Оплата - 1 - полная
            xml.Writer.WriteElementString("OPLATA", "1");

            foreach (Service s in services) {
                xml.Writer.WriteStartElement("USL");
                xml.Writer.WriteElementString("IDSERV", s.ID.ToString());
                xml.Writer.WriteElementString("LPU", Options.LpuCode);
                xml.Writer.WriteElementString("PODR", rec.Department);
                xml.Writer.WriteElementString("PROFIL", s.AidProfile);

                WriteBool("DET", Options.Pediatric, xml);

                xml.Writer.WriteElementString("DATE_IN", s.BeginDate.AsXml());
                xml.Writer.WriteElementString("DATE_OUT", s.EndDate.AsXml());

                xml.Writer.WriteElementString("DS", rec.Diagnosis);
                
                // Признак отказа
                WriteBool("P_OTK", s.Refusal, xml);

                xml.Writer.WriteElementString("CODE_USL", s.ServiceCode.ToString());
                xml.Writer.WriteElementString("KOL_USL", s.Quantity.ToString());

                xml.Writer.WriteElementString("TARIF", string.Empty);          // TODO:
                xml.Writer.WriteElementString("SUMV_USL", string.Format(Options.NumberFormat, "{0:f2}", s.Price));
                
                xml.Writer.WriteElementString("PRVS", s.DoctorProfile);        // V015
                xml.Writer.WriteElementString("CODE_MD", s.DoctorCode);

                xml.Writer.WriteEndElement();
            }

            xml.Writer.WriteEndElement();
        }
    }
}
