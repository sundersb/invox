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

        /// <summary>
        /// 0 - сведения об оказанной медицинской помощи передаются впервые;
        /// 1 - запись передается повторно после исправления
        /// </summary>
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
            xml.WriteBool("PR_NOV", IsUpdated);

            // PATIENT record
            xml.Writer.WriteStartElement("PACIENT");
            xml.Writer.WriteElementString("ID_PAC", PersonId.ToString());
            xml.Writer.WriteElementString("VPOLIS", PolicyKind.ToString());
            xml.WriteIfValid("SPOLIS", PolicySerial);
            xml.Writer.WriteElementString("NPOLIS", PolicyNumber);
            xml.Writer.WriteElementString("ST_OKATO", OKATO);
            xml.Writer.WriteElementString("SMO", SmoCode);

            // TODO: INV -
            //0 - нет инвалидности;
            //1 - 1 группа;
            //2 - 2 группа;
            //3 - 3 группа;
            //4 - дети-инвалиды.
            //Заполняется только при впервые установленной инвалидности (1 - 4) или в случае отказа в признании лица инвалидом (0)
            //xml.WriteIfValid("INV", Disability);

            // TODO: NOVOR - Это не булево значение:
            //Указывается в случае оказания медицинской помощи ребенку до государственной регистрации рождения.
            //0 - признак отсутствует.
            //Если значение признака отлично от нуля, он заполняется по следующему шаблону:
            //ПДДММГГН, где
            //П - пол ребенка в соответствии с классификатором V005 Приложения А;
            //ДД - день рождения; ММ - месяц рождения;
            //ГГ - последние две цифры года рождения;
            //Н - порядковый номер ребенка (до двух знаков).
            xml.WriteBool("NOVOR", IsNewborn);

            // VNOV_D - Вес при рождении, г

            xml.Writer.WriteEndElement();

            // SLUCH records

            // ToList() soasto check lazyness and free the connection for subqueries
            List<Recourse> rs = repo.LoadRecourceCases(policyCompound).ToList();

            foreach (Recourse r in rs) {
                List<Service> services = repo.LoadServices(policyCompound, r.Diagnosis, r.Reason).ToList();

                if (ReasonHelper.IsSingleDay(r.Reason)) {
                    // List of services may contain several recourses (Emergency, Prof, Other etc.)
                    foreach (IGrouping<DateTime, Service> group in services.GroupBy(s => s.EndDate))
                        WriteRecourse(r, xml, group.ToList(), repo);
                } else {
                    WriteRecourse(r, xml, services, repo);
                }
            }

            xml.Writer.WriteEndElement();
        }

        /// <summary>
        /// Write SLUCH record
        /// </summary>
        void WriteRecourse(Recourse rec, Lib.XmlExporter xml, List<Service> services, Data.IInvoice repo) {
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

            bool isDisp = rec.IsDispanserisation();

            xml.Writer.WriteStartElement("SLUCH");

            xml.Writer.WriteElementString("IDCASE", marks.Resulting.ID.ToString());

            // V006 Условия оказания МП (нет при дисп и проф Д3):
            if (rec.Section != AppendixSection.D3)
                xml.Writer.WriteElementString("USL_OK", rec.Condition);

            // Вид - Первичная МСП V008
            xml.Writer.WriteElementString("VIDPOM", marks.Resulting.AidKind);
            
            // Форма - Плановая V014 (нет при проф и дисп Д3)
            if (rec.Section != AppendixSection.D3)
                xml.Writer.WriteElementString("FOR_POM", marks.Resulting.AidForm);

            // Только для ВМП (Д2): VID_HMP, METOD_HMP
            //xml.Writer.WriteElementString("VID_HMP", string.Empty);            // TODO: Вид ВМП - нет
            //xml.Writer.WriteElementString("METOD_HMP", string.Empty);          // TODO: Метод ВМП - нет

            // NPR_MO   Не для Д3 Код МО, направившего на лечение (диагностику, консультацию) F003 Приложения А. При отсутствии сведений может не заполняться

            // EXTR     Не для Д3 Направление (госпитализация), 1 - плановая; 2 - экстренная
            if (rec.Section != AppendixSection.D3 && ReasonHelper.IsHospitalization(rec.Reason))
                xml.Writer.WriteElementString("EXTR", marks.Resulting.UrgentHospitalization ? "2" : "1");

            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            // LPU_1    Подразделение МО лечения из регионального справочника

            // Выездная бригада - только Д3 (0/1):
            if (rec.Section == AppendixSection.D3 && marks.Resulting.VisitingBrigade)
                xml.Writer.WriteElementString("VBR", "1");

            // PODR     Не для Д3 Отделение МО лечения из регионального справочника
            if (rec.Section != AppendixSection.D3)
                xml.Writer.WriteElementString("PODR", rec.Department);

            // Профиль МП V002
            if (rec.Section != AppendixSection.D3) {
                xml.Writer.WriteElementString("PROFIL", marks.Last.AidProfile); // Не для Д3
                xml.WriteBool("DET", Options.Pediatric);                        // Не для Д3
            }

            // Только для ВМП (Д2): TAL_D, TAL_P
            // Дата талона ВМП
            //xml.Writer.WriteElementString("TAL_D", string.Empty);
            // Дата запланир. госпит.
            //xml.Writer.WriteElementString("TAL_P", string.Empty);

            xml.Writer.WriteElementString("NHISTORY", marks.Resulting.CardNumber);

            // Признак отказа - только Д3
            if (rec.Section == AppendixSection.D3)
                xml.WriteBool("P_OTK", services.Any(s => s.Refusal));

            // TODO: Признак поступления/перевода. Наш ФОМС игнорирует(?)
            // Обязательно для дневного и круглосуточного стационара. Только для Д1
            //1 – Самостоятельно
            //2 – СМП
            //3 – Перевод из другой МО
            //4 – Перевод внутри МО с другого профиля
            //xml.Writer.WriteElementString("P_PER", "3");

            if (rec.Reason == Reason.DayHosp || rec.Reason == Reason.SurgeryDayHosp) {
                xml.Writer.WriteElementString("DATE_1", marks.Resulting.BeginDate.AsXml());
                xml.Writer.WriteElementString("DATE_2", marks.Resulting.EndDate.AsXml());
            } else {
                xml.Writer.WriteElementString("DATE_1", marks.First.BeginDate.AsXml());
                xml.Writer.WriteElementString("DATE_2", marks.Last.EndDate.AsXml());
            }

            // DS0      Н Не для Д3 Диагноз первичный

            xml.Writer.WriteElementString("DS1", rec.Diagnosis);

            // DS2      УМ Не для Д3 Диагноз сопутствующего заболевания
            // DS3      УМ Не для Д3 Диагноз осложнения заболевания

            // V017
            if (isDisp) {
                // Диагноз первичный. Только для Д3
                if (rec.FirstRevealed)
                    xml.Writer.WriteElementString("DS1_PR", "1");

                // DS2_N - Сопутствующие заболевания
                //      DS2         Код из справочника МКБ до уровня подрубрики
                //      DS2_PR      Обязательно указывается «1», если данный сопутствующий диагноз выявлен впервые
                
                // V009 ФОМС требует это поле и для диспансеризации. Хоть и не по приказу
                xml.Writer.WriteElementString("RSLT", marks.Resulting.ResultCode);

                // V017
                xml.Writer.WriteElementString("RSLT_D", marks.Resulting.DispResultCode);

                // Направления по итогам диспансеризации
                foreach (DispDirection d in repo.LoadDispanserisationRoute(marks.Resulting.ID))
                    d.Write(xml, repo);

                // PR_D_N - сведения о диспансерном наблюдении по поводу основного заболевания: 0 - нет; 1 - да
            } else {
                // VNOV_M   УМ Вес при рождении, Указывается при оказании медицинской помощи недоношенным и маловесным детям.
                //             Поле заполняется, если в качестве пациента указана мать

                // CODE_MES1    УМ Код МЭС
                // CODE_MES2    У  Код МЭС сопутствующего заболевания

                // V009
                xml.Writer.WriteElementString("RSLT", marks.Resulting.ResultCode);

                // Исход заболевания V012
                xml.Writer.WriteElementString("ISHOD", rec.Outcome);

                // Специальность врача V015
                xml.Writer.WriteElementString("PRVS", marks.Resulting.DoctorProfile);
                xml.Writer.WriteElementString("VERS_SPEC", "V015");
                xml.Writer.WriteElementString("IDDOKT", services.OrderBy(s => s.EndDate).Last().DoctorCode);

                // Список особых случаев from D_TYPE
                // TODO: ХКФОМС вкладывает в OS_SLUCH свой особый смысл. Не тот, что ФФОМС:
                //1 - медицинская помощь оказана новорожденному ребенку до государственной регистрации рождения при многоплодных родах;
                //2 - в документе, удостоверяющем личность пациента/родителя (представителя) пациента, отсутствует отчество
                foreach (string sc in services.Select(s => s.SpecialCase).Where(s => !string.IsNullOrEmpty(s)).Distinct())
                    xml.Writer.WriteElementString("OS_SLUCH", sc);
            }

            // Способ оплаты V010
            xml.Writer.WriteElementString("IDSP", marks.Resulting.PayKind);

            // К-во единиц оплаты
            xml.Writer.WriteElementString("ED_COL", marks.Resulting.Quantity.ToString());

            // TODO: Тариф
            // UPDATE: В релаксе подушевые суммы по нулям. Где его брать?
            // PATU.TARIF Код тарифа услуги (1-себестоимость ОМС по ИП, 2- полная себестоимость по ИП, 3-цена по ИП, 4 –территориальный тариф)
            //xml.Writer.WriteElementString("TARIF", string.Empty);

            // Цель обращения
            xml.Writer.WriteElementString("CEL", marks.Resulting.RecourseAim);

            // Сумма к оплате
            xml.Writer.WriteElementString("SUMV", string.Format(Options.NumberFormat, "{0:f2}", services.Sum(s => s.Price)));

            // Оплата 0 - не принято решение об оплате
            //1 - полная;
            //2 - полный отказ;
            //3 - частичный отказ
            xml.Writer.WriteElementString("OPLATA", "1");

            foreach (Service s in services) {
                xml.Writer.WriteStartElement("USL");

                xml.Writer.WriteElementString("IDSERV", s.ID.ToString());

                xml.Writer.WriteElementString("LPU", Options.LpuCode);
                // LPU_1    У Подразделение МО лечения из регионального справочника

                if (isDisp) {
                    // Требует ФОМС, не по приказу
                    xml.Writer.WriteElementString("PODR", rec.Department);
                    // Doubling code in attitude to DATE_IN, DATE_OUT because of featured FOMS formal check
                } else {
                    xml.Writer.WriteElementString("PODR", rec.Department);
                    xml.Writer.WriteElementString("PROFIL", s.AidProfile);
                    // VID_VME  У Вид медицинского вмешательства V001
                    xml.WriteBool("DET", Options.Pediatric);
                }
                xml.Writer.WriteElementString("DATE_IN", s.BeginDate.AsXml());
                xml.Writer.WriteElementString("DATE_OUT", s.EndDate.AsXml());

                // ФОМС требует диагноз даже для ДД и проф. Не по приказу
                xml.Writer.WriteElementString("DS", rec.Diagnosis);

                // Признак отказа. ФОМС пропускает везде, но по приказу только ДД и проф. Теперь дает ошибку
                //xml.Writer.WriteElementString("P_OTK", s.Refusal ? "1" : string.Empty);
                // Порядок элементов для ФОМС имеет значение: если сунуть CODE_USL выше или ниже, БАРС выдаст ошибку:
                xml.Writer.WriteElementString("CODE_USL", s.ServiceCode.ToString("D6"));
                xml.Writer.WriteElementString("KOL_USL", s.Quantity.ToString());

                //xml.Writer.WriteElementString("TARIF", "1");          // TODO:
                xml.Writer.WriteElementString("SUMV_USL", string.Format(Options.NumberFormat, "{0:f2}", s.Price));

                xml.Writer.WriteElementString("PRVS", s.DoctorProfile);        // V015
                xml.Writer.WriteElementString("CODE_MD", s.DoctorCode);
                                
                // NPL      У Неполный объем Только Д1
                //1 - документированный отказ больного,
                //2 - медицинские противопоказания,
                //3 - прочие причины (умер, переведен в другое отделение и пр.)
                //4 - ранее проведенные услуги в пределах установленных сроков
                //xml.Writer.WriteElementString("NPL", string.Empty);

                // Нате вам пасхалку
                xml.WriteIfValid("COMENTU", Options.ReadingBot.Read());
                //xml.Writer.WriteElementString("COMENTU", string.Empty);

                xml.Writer.WriteEndElement();
            }

            xml.Writer.WriteEndElement();
        }
    }
}
