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
        int sex;

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
        public DateTime BirthDate;

        public int Sex {
            get { return sex; }
            set {
                if (value != 1 && value != 2)
                    throw new ArgumentException("Неправильный пол: " + value.ToString());
                sex = value;
            }
        }


        /// <summary>
        /// Код льготы
        /// </summary>
        public string Privilege;

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

        string GetDisability() {
            if (Privilege.Length == 2
                && Privilege[0] == '0'
                && Privilege[1] >= '1'
                && Privilege[1] <= '4') {
                return Privilege.Substring(1);
            } else return string.Empty;
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

            //0 - нет инвалидности;
            //1 - 1 группа;
            //2 - 2 группа;
            //3 - 3 группа;
            //4 - дети-инвалиды.
            //Заполняется только при впервые установленной инвалидности (1 - 4) или в случае отказа в признании лица инвалидом (0)
            xml.WriteIfValid("INV", GetDisability());

            // NOVOR
            if (IsNewborn) {
                //Указывается в случае оказания медицинской помощи ребенку до государственной регистрации рождения.
                //0 - признак отсутствует.
                //Если значение признака отлично от нуля, он заполняется по следующему шаблону:
                //ПДДММГГН, где
                //П - пол ребенка в соответствии с классификатором V005 Приложения А;
                //ДД - день рождения; ММ - месяц рождения;
                //ГГ - последние две цифры года рождения;
                //Н - порядковый номер ребенка (до двух знаков).
                StringBuilder sb = new StringBuilder();
                
                sb.Append(sex);
                sb.Append(BirthDate.Day.ToString("D2"));
                sb.Append(BirthDate.Month.ToString("D2"));
                sb.Append((BirthDate.Year % 100).ToString("D2"));
                //TODO: порядковый номер ребенка (до двух знаков).
                sb.Append("1");

                xml.Writer.WriteElementString("NOVOR", sb.ToString());
            } else {
                xml.Writer.WriteElementString("NOVOR", "0");
            }

            // VNOV_D - Вес при рождении, г

            xml.Writer.WriteEndElement();

            // ToList() soasto check lazyness and free the connection for subqueries
            List<Recourse> rs = repo.LoadRecourceCases(policyCompound).ToList();

            foreach (Recourse r in rs) {
                var writer = GetWriter(r);
                List<Service> services = repo.LoadServices(policyCompound, r.Diagnosis, r.Reason).ToList();

                if (ReasonHelper.IsSingleDay(r.Reason)) {
                    // List of services may contain several recourses (Emergency, Prof, Other etc.)
                    foreach (IGrouping<DateTime, Service> group in services.GroupBy(s => s.EndDate)) {
                        writer(r, xml, group.ToList(), repo);
                    }
                } else {
                    writer(r, xml, services, repo);
                }
            }

            xml.Writer.WriteEndElement();
        }

        Action<Recourse, Lib.XmlExporter, List<Service>, Data.IInvoice> GetWriter(Recourse rec) {
            switch (rec.Section) {
                case AppendixSection.D1: return WriteD1;
                case AppendixSection.D2: return WriteD2;
                case AppendixSection.D3: return WriteD3;
                default: throw new ArgumentException("Неверный раздел для повода обращения: " + rec.Section.ToString());
            }
        }

        void InvalidRecourse(Recourse rec) {
            Lib.Logger.Log(string.Format("Услуги не формируют случай обращения:\r\n\t'{0}', DS {1}, отделение {2}, повод обращения {3}",
                policyCompound,
                rec.Diagnosis,
                rec.Department,
                rec.Reason));
        }
        
        bool NeedsDirection(Recourse rec, Service service) {
            return rec.SuspNeo
                
                // Плановая в круглосуточном стационаре или СДП
                || (service.AidForm == "3" && (rec.Condition == "1" || rec.Condition == "2"))

                // Неотложная в круглосуточном стационаре
                || (service.AidForm == "2" && rec.Condition == "1");
        }

        /// <summary>
        /// Write SLUCH record for appendix D1 recourse
        /// </summary>
        void WriteD1(Recourse rec, Lib.XmlExporter xml, List<Service> services, Data.IInvoice repo) {
            if (services.Count == 0) return;

            RecourseLandmarks marks = Service.ArrangeServicesD12(services);
            if (!marks.Valid) {
                InvalidRecourse(rec);
                return;
            }

            xml.Writer.WriteStartElement("SLUCH");

            xml.Writer.WriteElementString("IDCASE", marks.Resulting.ID.ToString());

            // V006 Условия оказания МП
            xml.Writer.WriteElementString("USL_OK", rec.Condition);

            // Вид - Первичная МСП V008
            xml.Writer.WriteElementString("VIDPOM", marks.Resulting.AidKind);
            
            // Форма - Плановая V014
            xml.Writer.WriteElementString("FOR_POM", marks.Resulting.AidForm);

            if (NeedsDirection(rec, marks.Resulting)) {
                // NPR_MO   У Код МО, направившего на лечение (диагностику, консультацию) F003 Приложения А. При отсутствии сведений может не заполняться
                // TODO: Мы направляем в СДП себе только сами. Где другим ЛПУ брать код направившей МО?
                xml.Writer.WriteElementString("NPR_MO", Options.LpuCode);

                // TODO: NPR_DATE
                //xml.Writer.WriteElementString("NPR_DATE", ???);
            }

            // EXTR     У Направление (госпитализация), 1 - плановая; 2 - экстренная
            if (ReasonHelper.IsHospitalization(rec.Reason))
                xml.Writer.WriteElementString("EXTR", marks.Resulting.UrgentHospitalization ? "2" : "1");

            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            // LPU_1    У Подразделение МО лечения из регионального справочника

            // PODR     У Отделение МО лечения из регионального справочника
            xml.Writer.WriteElementString("PODR", rec.Department);

            // О Профиль МП V002
            xml.Writer.WriteElementString("PROFIL", marks.Last.AidProfile);

            // О Детский профиль 0 - нет, 1 - да
            xml.WriteBool("DET", Options.Pediatric);

            xml.Writer.WriteElementString("NHISTORY", marks.Resulting.CardNumber);

            if (ReasonHelper.IsHospitalization(rec.Reason)) {
                // TODO: Признак поступления/перевода. Наш ФОМС игнорирует(?)
                // У Обязательно для дневного и круглосуточного стационара
                //1 – Самостоятельно
                //2 – СМП
                //3 – Перевод из другой МО
                //4 – Перевод внутри МО с другого профиля
                //xml.Writer.WriteElementString("P_PER", "1");

                xml.Writer.WriteElementString("DATE_1", marks.Resulting.BeginDate.AsXml());
                xml.Writer.WriteElementString("DATE_2", marks.Resulting.EndDate.AsXml());
            } else {
                xml.Writer.WriteElementString("DATE_1", marks.First.BeginDate.AsXml());
                xml.Writer.WriteElementString("DATE_2", marks.Last.EndDate.AsXml());
            }

            // DS0      Н Диагноз первичный

            xml.Writer.WriteElementString("DS1", rec.Diagnosis);

            // DS2      УМ Диагноз сопутствующего заболевания
            // DS3      УМ Диагноз осложнения заболевания
            
#if !NO59
            // Приказ 59 30.03.2018
            if (rec.SuspNeo) xml.Writer.WriteElementString("DS_ONK", "1");
#endif
            // VNOV_M   УМ Вес при рождении, Указывается при оказании медицинской помощи недоношенным и маловесным детям.
            //             Поле заполняется, если в качестве пациента указана мать

            // CODE_MES1    УМ Код МЭС
            // CODE_MES2    У  Код МЭС сопутствующего заболевания

#if !NO59
            // GY{ уроды, когда они договорятся по какому приказу работать?!!

            // Приказ 59 от 30.03.2018 - онкология
            if (OnkologyTreat.IsOnkologyTreat(rec, policyCompound, repo)) {
                OnkologyTreat treat = repo.GetOnkologyTreat(marks.Resulting.ID);
                treat.Write(xml, repo);
            }
#endif

            // V009
            xml.Writer.WriteElementString("RSLT", marks.Resulting.ResultCode);

            // Исход заболевания V012
            xml.Writer.WriteElementString("ISHOD", rec.Outcome);

            // Специальность врача V015 (V021 с 20180518)
            xml.Writer.WriteElementString("PRVS", marks.Resulting.DoctorProfile);
            xml.Writer.WriteElementString("VERS_SPEC", "V021");
            xml.Writer.WriteElementString("IDDOKT", services.OrderBy(s => s.EndDate).Last().DoctorCode);

            // Список особых случаев from D_TYPE
            // TODO: ХКФОМС вкладывает в OS_SLUCH свой особый смысл. Не тот, что ФФОМС:
            //1 - медицинская помощь оказана новорожденному ребенку до государственной регистрации рождения при многоплодных родах;
            //2 - в документе, удостоверяющем личность пациента/родителя (представителя) пациента, отсутствует отчество
            foreach (string sc in services.Select(s => s.SpecialCase).Where(s => !string.IsNullOrEmpty(s)).Distinct())
                xml.Writer.WriteElementString("OS_SLUCH", sc);

            // О Способ оплаты V010
            xml.Writer.WriteElementString("IDSP", marks.Resulting.PayKind);

            // У К-во единиц оплаты
            xml.Writer.WriteElementString("ED_COL", marks.Resulting.Quantity.ToString());

            // TODO: У Тариф
            // UPDATE: В релаксе подушевые суммы по нулям. Где его брать?
            // PATU.TARIF Код тарифа услуги (1-себестоимость ОМС по ИП, 2- полная себестоимость по ИП, 3-цена по ИП, 4 –территориальный тариф)
            //xml.Writer.WriteElementString("TARIF", string.Empty);

            // Цель обращения (приказ ХКФОМС)
            xml.Writer.WriteElementString("CEL", marks.Resulting.RecourseAim);

            // О Сумма к оплате
            xml.Writer.WriteElementString("SUMV", string.Format(Options.NumberFormat, "{0:f2}", services.Sum(s => s.Price)));

            // Оплата 0 - не принято решение об оплате
            //1 - полная;
            //2 - полный отказ;
            //3 - частичный отказ
            xml.Writer.WriteElementString("OPLATA", "1");

            foreach (Service s in services) s.WriteD12(rec, xml, repo);

            xml.Writer.WriteEndElement();
        }

        void WriteD2(Recourse rec, Lib.XmlExporter xml, List<Service> services, Data.IInvoice repo) {
            if (services.Count == 0) return;

            RecourseLandmarks marks = Service.ArrangeServicesD12(services);
            if (!marks.Valid) {
                InvalidRecourse(rec);
                return;
            }

            xml.Writer.WriteStartElement("SLUCH");

            xml.Writer.WriteElementString("IDCASE", marks.Resulting.ID.ToString());

            // V006 Условия оказания МП:
            xml.Writer.WriteElementString("USL_OK", rec.Condition);

            // Вид - Первичная МСП V008
            xml.Writer.WriteElementString("VIDPOM", marks.Resulting.AidKind);

            // Форма - Плановая V014 (нет при проф и дисп Д3)
            xml.Writer.WriteElementString("FOR_POM", marks.Resulting.AidForm);

            // Только для ВМП (Д2): VID_HMP, METOD_HMP
            xml.Writer.WriteElementString("VID_HMP", string.Empty);            // TODO: Вид ВМП - нет
            xml.Writer.WriteElementString("METOD_HMP", string.Empty);          // TODO: Метод ВМП - нет

            if (NeedsDirection(rec, marks.Resulting)) {
                // NPR_MO   У Код МО, направившего на лечение (диагностику, консультацию) F003 Приложения А. При отсутствии сведений может не заполняться
                // TODO: Мы направляем в СДП себе только сами. Где другим ЛПУ брать код направившей МО?
                xml.Writer.WriteElementString("NPR_MO", Options.LpuCode);
            }

            // EXTR     У Направление (госпитализация), 1 - плановая; 2 - экстренная
            if (ReasonHelper.IsHospitalization(rec.Reason))
                xml.Writer.WriteElementString("EXTR", marks.Resulting.UrgentHospitalization ? "2" : "1");

            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            // LPU_1    Подразделение МО лечения из регионального справочника

            // PODR     Отделение МО лечения из регионального справочника
            xml.Writer.WriteElementString("PODR", rec.Department);

            // Профиль МП V002
            xml.Writer.WriteElementString("PROFIL", marks.Last.AidProfile);
            xml.WriteBool("DET", Options.Pediatric);

            // О Дата талона ВМП
            xml.Writer.WriteElementString("TAL_D", string.Empty);
            // О Дата запланир. госпит.
            xml.Writer.WriteElementString("TAL_P", string.Empty);

            xml.Writer.WriteElementString("NHISTORY", marks.Resulting.CardNumber);

            // TODO: Признак поступления/перевода. Наш ФОМС игнорирует(?)
            // Обязательно для дневного и круглосуточного стационара. Только для Д1
            //1 – Самостоятельно
            //2 – СМП
            //3 – Перевод из другой МО
            //4 – Перевод внутри МО с другого профиля
            //xml.Writer.WriteElementString("P_PER", "3");

            if (ReasonHelper.IsHospitalization(rec.Reason)) {
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

#if !NO59
            // Приказ 59 30.03.2018
            if (rec.SuspNeo) xml.Writer.WriteElementString("DS_ONK", "1");
#endif
            // VNOV_M   УМ Вес при рождении, Указывается при оказании медицинской помощи недоношенным и маловесным детям.
            //             Поле заполняется, если в качестве пациента указана мать

            // CODE_MES1    УМ Код МЭС
            // CODE_MES2    У  Код МЭС сопутствующего заболевания

#if !NO59
            // Приказ 59 от 30.03.2018 - онкология
            if (OnkologyTreat.IsOnkologyTreat(rec, policyCompound, repo)) {
                OnkologyTreat treat = repo.GetOnkologyTreat(marks.Resulting.ID);
                treat.Write(xml, repo);
            }
#endif
            // V009
            xml.Writer.WriteElementString("RSLT", marks.Resulting.ResultCode);

            // Исход заболевания V012
            xml.Writer.WriteElementString("ISHOD", rec.Outcome);

            // Специальность врача V021
            xml.Writer.WriteElementString("PRVS", marks.Resulting.DoctorProfile);
            xml.Writer.WriteElementString("VERS_SPEC", "V021");
            xml.Writer.WriteElementString("IDDOKT", services.OrderBy(s => s.EndDate).Last().DoctorCode);

            // Список особых случаев from D_TYPE
            // TODO: ХКФОМС вкладывает в OS_SLUCH свой особый смысл. Не тот, что ФФОМС:
            //1 - медицинская помощь оказана новорожденному ребенку до государственной регистрации рождения при многоплодных родах;
            //2 - в документе, удостоверяющем личность пациента/родителя (представителя) пациента, отсутствует отчество
            foreach (string sc in services.Select(s => s.SpecialCase).Where(s => !string.IsNullOrEmpty(s)).Distinct())
                xml.Writer.WriteElementString("OS_SLUCH", sc);

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

            foreach (Service s in services) s.WriteD12(rec, xml, repo);

            xml.Writer.WriteEndElement();
        }

        void WriteD3(Recourse rec, Lib.XmlExporter xml, List<Service> services, Data.IInvoice repo) {
            if (services.Count == 0) return;

            RecourseLandmarks marks = Service.ArrangeServicesD3(services);
            if (!marks.Valid) {
                InvalidRecourse(rec);
                return;
            }

            xml.Writer.WriteStartElement("SLUCH");

            xml.Writer.WriteElementString("IDCASE", marks.Resulting.ID.ToString());

            // V006 Условия оказания МП
            xml.Writer.WriteElementString("USL_OK", rec.Condition);

            // Вид - Первичная МСП V008
            xml.Writer.WriteElementString("VIDPOM", marks.Resulting.AidKind);

            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            // LPU_1    Подразделение МО лечения из регионального справочника

            // Выездная бригада (0/1):
            if (marks.Resulting.VisitingBrigade)
                xml.Writer.WriteElementString("VBR", "1");

            xml.Writer.WriteElementString("NHISTORY", marks.Resulting.CardNumber);

            // О Признак отказа
            xml.WriteBool("P_OTK", services.Any(s => s.Refusal));

            xml.Writer.WriteElementString("DATE_1", marks.First.BeginDate.AsXml());
            xml.Writer.WriteElementString("DATE_2", marks.Last.EndDate.AsXml());

            xml.Writer.WriteElementString("DS1", rec.Diagnosis);

            // Диагноз первичный
            if (rec.FirstRevealed)
                xml.Writer.WriteElementString("DS1_PR", "1");

#if !NO59
            // Приказ 59 30.03.2018
            if (rec.SuspNeo) xml.Writer.WriteElementString("DS_ONK", "1");
#endif
            // DS2_N - УМ Сопутствующие заболевания
            //      DS2         О Код из справочника МКБ до уровня подрубрики
            //      DS2_PR      У Обязательно указывается «1», если данный сопутствующий диагноз выявлен впервые

            // V009
            // TODO: Убрать, когда В БАРС исправят. Сейчас требуют RSLT и RSLT_D одновременно
            xml.Writer.WriteElementString("RSLT", marks.Resulting.ResultCode);

            // V017 - По приказу наличие поля обязательно, но ХКФОМС для не-диспансеризации банит
            if (rec.IsDispanserisation())
                xml.Writer.WriteElementString("RSLT_D", marks.Resulting.DispResultCode);

            // Направления по итогам диспансеризации
            // 20180731 - ФОМС банит случаи, где направлений больше одного на этапе ФЛК!
            
            //foreach (DispDirection d in repo.LoadDispanserisationRoute(marks.Resulting.ID))
            //    d.Write(xml, repo);

            // Inde irae:
            DispDirection d = repo.LoadDispanserisationRoute(marks.Resulting.ID).FirstOrDefault();
            if (d != null) d.Write(xml, repo);

            // PR_D_N - сведения о диспансерном наблюдении по поводу основного заболевания: 0 - нет; 1 - да

            // Самодеятельность ХКФОМС - приказ 169 не требует
            xml.Writer.WriteElementString("IDDOKT", marks.Resulting.DoctorCode);

            // Способ оплаты V010
            xml.Writer.WriteElementString("IDSP", marks.Resulting.PayKind);

            // К-во единиц оплаты
            xml.Writer.WriteElementString("ED_COL", marks.Resulting.Quantity.ToString());

            // TODO: Тариф
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

            foreach (Service s in services) s.WriteD3(rec, xml, repo);

            xml.Writer.WriteEndElement();
        }
    }
}
