using System;
using System.Linq;
using System.Collections.Generic;

namespace invox.Data.Relax {
    /// <summary>
    /// Вспомогательный класс для выборки данных, объединяющий Recourse и Event
    /// </summary>
    /// <remarks>
    /// Необходимость связана с различием компоновки полей в риказе и Релакс
    /// </remarks>
    class RecourseAux {
        const string SUSP_NEO_DIAGNOSIS = "Z03.1";

        const int DD_ONCE_IN_TWO_YEARS_DONE = 98;

        static int[] SERVICE_KIND_LABORATORY = { 13, 34 };

        static string[] REFUSAL_RESULTS = { "302", "408", "417", "207" };

        static int[] SOUL_TARIFF_SERVICES = { 50002, 50004, 50010, 50014, 50019, 50020, 50023 };

        // V008
        const int AID_KIND_EMERGENCY = 2;
        const int AID_KIND_PRIMARY = 12;
        const int AID_KIND_PRIMARY_SPECIALIZED = 13;
        const int AID_KIND_SPECIALIZED = 31;
        const int AID_KIND_HITECH = 32;

        // V014
        const int AID_FORM_URGENT = 1;
        const int AID_FORM_PRESSING = 2;
        const int AID_FORM_ORDINAL = 3;

        /// <summary>
        /// Данные случай требует подушевой оплаты
        /// </summary>
        bool soul;

        /// <summary>
        /// Порядковый номер закрытого случая
        /// </summary>
        public int OrdinalNumber;

        /// <summary>
        /// Пациент P.RECID
        /// </summary>
        public string PersonId;         // P.RECID

        /// <summary>
        /// ID услуги, соответствующей событию
        /// </summary>
        public string ServiceId;        // S.RECID

        /// <summary>
        /// Подразделение ЛПУ (OTD)
        /// </summary>
        public string Department;       // S.OTD

        /// <summary>
        /// Подразделение ЛПУ (PODR)
        /// </summary>
        public string Unit;             // S.PODR

        /// <summary>
        /// Профиль медицинской помощи V002 (из KMU.MSP через мультиплексор)
        /// </summary>
        public string AidProfile;       // KMU.MSP

        /// <summary>
        /// Условия оказания медицинской помощи V006 (из SLUMP.SLUSL)
        /// </summary>
        public string AidConditions;    // SLUMP.SLUSL

        /// <summary>
        /// Код услуги (COD)
        /// </summary>
        public int ServiceCode;         // S.COD

        /// <summary>
        /// Вид услуги. Можно отличить лабораторные услуги от других
        /// </summary>
        public int ServiceKind;

        /// <summary>
        /// Результат обращения V009
        /// </summary>
        public string Result;           // REZOBR.SLIZ

        /// <summary>
        /// Исход заболевания V012 (из IG)
        /// </summary>
        public string Outcome;          // S.IG

        /// <summary>
        /// Способ оплаты V010 (из KMU.OPL)
        /// </summary>
        public string PayKind;          // KMU.OPL

        /// <summary>
        /// Тип оплаты для законченного случая
        /// </summary>
        public Model.PayType PayType;   // 1

        /// <summary>
        /// Признак выездной бригады. Пока просто false
        /// </summary>
        public bool MobileBrigade;      // ??? DD, No way to know yet

        /// <summary>
        /// Результат обращения для внутренних нужд пула данных (из BE)
        /// </summary>
        public string RecourseResult;   // S.BE

        /// <summary>
        /// Профиль койки V020 (из STRUCT.PROF через мультиплексор)
        /// </summary>
        public string BedProfile;       // STRUCT.PROF

        /// <summary>
        /// Признак услуги, оказанной ребенку
        /// </summary>
        public bool Child;              // KMU.LDET
        
        /// <summary>
        /// Номер амбулаторной карты/истории болезни
        /// </summary>
        public string CardNumber;       // S.C_I

        /// <summary>
        /// Основное заболевание
        /// </summary>
        public string MainDiagnosis;    // S.DS - ректифицирован
        public string InitialDiagnosis; // Диагноз по базе

        /// <summary>
        /// Признак реабилитации. Пока false
        /// </summary>
        public bool Rehabilitation;     // ???

        /// <summary>
        /// Вид вмешательства V001 (в работе)
        /// </summary>
        public string InterventionKind; // ??? v001

        /// <summary>
        /// Количество услуг в законченном случае
        /// </summary>
        public int Quantity;            // S.K_U

        /// <summary>
        /// Тариф
        /// </summary>
        public decimal Tariff;           // S.S_ALL

        /// <summary>
        /// Предъявлено к оплате
        /// </summary>
        public decimal Total;            // S.S_ALL

        /// <summary>
        /// Специальность врача V021 (из MEDPOST.CODEFSS пока без мультиплексора - уточнить)
        /// </summary>
        public string SpecialityCode;   // MEDPOST.CODEFSS

        /// <summary>
        /// Табельный номер врача
        /// </summary>
        public string DoctorCode;       // S.TN1

        /// <summary>
        /// Дата услуги, представляющей законченный случай
        /// </summary>
        public DateTime Date;           // S.D_U

        /// <summary>
        /// Повод обращения для вспомогательных нужд. Формируется по коду отделения и коду услуги
        /// </summary>
        public InternalReason InternalReason;

        /// <summary>
        /// Создать повод обращения и заполнить его доступными полями
        /// </summary>
        public Model.Recourse ToRecourse() {
            Model.Recourse result = new Model.Recourse();

            result.SuspectOncology = MainDiagnosis == SUSP_NEO_DIAGNOSIS;
            result.Department = Department;
            result.Profile = AidProfile;
            result.Identity = ServiceId;
            result.Conditions = AidConditions;
            //result.AidKind;                           - Pool.LoadEvents
            //result.AidForm;                           - Pool.LoadEvents
            //result.DirectedFrom;                      - Pool.LoadEvents

            // TODO: RecourseAux.DirectionDate - where to take from?
            //result.DirectionDate;

            //result.DateFrom;                          - Pool.LoadEvents
            result.DateTill = Date;
            //result.BedDays;                           - Pool.LoadEvents
            //result.BirthWeight;                       - Pool.LoadEvents
            result.Result = Result;
            result.Outcome = Outcome;
            //result.UnitShift                          - Pool.LoadEvents
            result.PayKind = PayKind;
            //result.Total                              - Pool.LoadEvents
            result.PayType = PayType;
            //result.AcceptedSum
            //result.DeniedSum
            result.MobileBrigade = MobileBrigade;
            result.DispanserisationRefusal = REFUSAL_RESULTS.Contains(Result);
            result.DispanserisationResult = Dict.DispResult.Get(RecourseResult);

            return result;
        }

        /// <summary>
        /// Обновить вид и форму помощи случая по закрывающей услуге
        /// </summary>
        /// <param name="rec">Законченный случай, для которого требуется обновить вид и форму МП</param>
        /// <param name="sa">Посещение, закрывающее случай</param>
        public void UpdateMedicalAid(Model.Recourse rec, ServiceAux sa) {
            rec.AidKind = GetAidKind(sa);
            rec.AidForm = GetAidForm(sa);
            
            // Код врача и специальности был взят из услуги, обозначающей закрытый случай и привязанной к открывающей записи,
            // но по приказу они должны соответствовать закрывающей записи
            SpecialityCode = sa.SpecialityCode;
            DoctorCode = sa.DoctorCode;
            Result = sa.Result;
            Outcome = sa.Outcome;
        }


        /// <summary>
        /// Загрузить сопутствующие заболевания для события
        /// </summary>
        /// <param name="evt">Событие, которому требуется установить сопутствующие заболевания</param>
        /// <param name="services">Услуги, оказанные в рамках события. Сопутствующие берутся из них</param>
        public void FindConcurrentDiagnoses(Model.Event evt, List<ServiceAux> services, Model.OrderSection section) {
            var d1 = services.Select(s => s.ConcurrentDiagnosis)
                .Where(d => !string.IsNullOrEmpty(d) && d != evt.MainDiagnosis)
                .Distinct();
            
            if (d1.Count() > 0)
                evt.ConcurrentDiagnoses = d1.ToList();
            else
                evt.ConcurrentDiagnoses = null;

            // Проверить соответствие диагноза цели обращения (только для разделов D1 и D4)
            if (section == Model.OrderSection.D1 || section == Model.OrderSection.D4) {
                DiagnosisKind kind = InternalReason.GetDiagnosisKind();

                if (!kind.Matches(MainDiagnosis)) {
                    // Если не соответствует
                    
                    // Переносим основное заболевание в сопутствующие
                    if (evt.ConcurrentDiagnoses == null) evt.ConcurrentDiagnoses = new List<string>();
                    evt.ConcurrentDiagnoses.Add(MainDiagnosis);

                    // Попытаться найти подходящий в услугах...
                    d1 = services.Select(s => s.PrimaryDiagnosis).Where(d => kind.Matches(d));
                    if (d1.Count() > 0) {
                        evt.MainDiagnosis = MainDiagnosis = d1.First();
                    } else {
                        // ...либо поставить диагноз по умолчанию
                        if (kind == DiagnosisKind.Treatment) {
                            Lib.Logger.Log(string.Format("Не найден подходящий диагноз для случая лечебной цели: RECID {0} (диагноз {1})",
                                evt.CardNumber,
                                MainDiagnosis));
                        }
                        evt.MainDiagnosis = MainDiagnosis = InternalReason.DefaultDiagnosis();
                        evt.Services.ForEach(s => s.Diagnosis = MainDiagnosis);
                    }
                }
            }
        }

        /// <summary>
        /// Создать событие и заполнить его доступными полями
        /// </summary>
        public Model.Event ToEvent(Model.Recourse rec, Model.OrderSection section) {
            Model.Event result = new Model.Event();

            // result.Services;                         - Pool.LoadEvents
            result.Identity = ServiceId;
            result.Unit = Unit;

            if (rec.IsHospitalization) {
                result.BedProfile = BedProfile;
                result.Transfer = Model.Transfer.Independently;
            } else {
                result.Transfer = Model.Transfer.None;
            }
            
            result.Child = Child;
            result.Reason = InternalReasonHelper.ToVisitAim(InternalReason);

#if FOMS
            result.LocalReason = InternalReason.ToFomsReason(soul);
#endif

            result.CardNumber = CardNumber;
            //result.Transfer;                          - Pool.LoadEvents
            //result.DateFrom;                          - Pool.LoadEvents
            result.DateTill = Date;
            //result.BedDays;                           - Pool.LoadEvents
            //result.PrimaryDiagnosis;                  - Pool.LoadEvents

            result.MainDiagnosis = MainDiagnosis;

            //result.FirstIdentified;                   - Pool.LoadEvents
            //result.ConcurrentDiagnosis;               - Pool.LoadEvents
            //result.ComplicationDiagnosis;             - Pool.LoadEvents
            //result.DispensarySupervision;             - Pool.LoadEvents
            //result.ConcurrentMesCode;                 - Pool.LoadEvents

            // TODO: RecourseAux - forming ClinicalGroup
            //result.ClinicalGroup;

            // Hack to avoid oncology treatment services in polyclinic
            result.Rehabilitation = Rehabilitation || (section == Model.OrderSection.D4 && !rec.SuspectOncology);

            result.Quantity = Quantity;
            result.SpecialityCode = SpecialityCode;
            result.DoctorCode = DoctorCode;

            //result.Tariff;                            - Pool.LoadEvents
            //result.Total;                             - Pool.LoadEvents

            // TODO: RecourseAux - HiTech data
            //result.HiTechKind;
            //result.HiTechMethod;
            //result.HiTechCheckDate;
            //result.HiTechCheckNumber;
            //result.HiTechPlannedHospitalizationDate;

            return result;
        }

        /// <summary>
        /// Обновить внутренний код повода обращения в соответствие кабинету, услуге и результату обращения
        /// </summary>
        public void Update() {
            int unit;
            if (!int.TryParse(Department, out unit)) unit = 1;

            soul = SOUL_TARIFF_SERVICES.Contains(ServiceCode);
            InternalReason = GetInternalReason(unit, ServiceCode, RecourseResult);
            InitialDiagnosis = MainDiagnosis;

            // 20190304 - Снова в конце месяца блять!
            if (InternalReason == Relax.InternalReason.Diagnostics) {
                Result = "304";
                Outcome = "304";
                if (SERVICE_KIND_LABORATORY.Contains(ServiceKind))
                    MainDiagnosis = "Z01.7";
                else
                    MainDiagnosis = "Z01.8";
            } else {
                Outcome = Dict.Outcome.Get(AidConditions, Outcome.TrimStart('0'));
            }

            PayKind = GetPayKind(unit, ServiceCode, soul);
        }

        /// <summary>
        /// Получить способ оплаты V010
        /// </summary>
        /// <param name="unit">Код отделения (S.OTD)</param>
        /// <param name="service">Код услуги (S.COD)</param>
        /// <param name="isSoul">Требует подушевой оплаты</param>
        /// <remarks>
        /// Использовался документ pravila-dlya-sbora-sluchaev-sposob-oplaty_16012018.xlsx с сайта ХКФОМС
        /// </remarks>
        /// <returns>Код способа оплаты</returns>
        static string GetPayKind(int unit, int service, bool isSoul) {
            switch (unit) {
                case 0:
                case 1:
                    if (isSoul)
                        return "25";
                    else
                        return "30";

                case 3: return "33";

                case 4:
                    if (isSoul)
                        return "25";
                    else
                        return "29";

                case 5: return "29";
                case 8: return "28";
                case 9:
                    if (service / 1000 == 27)
                        return "29";
                    else
                        return "30";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Получить повод обращения
        /// </summary>
        /// <param name="unit">Код отделения (S.OTD)</param>
        /// <param name="service">Код услуги (S.COD)</param>
        /// <param name="recourseResult">Код результата обращения (S.BE)</param>
        /// <returns>Повод обращения</returns>
        static Relax.InternalReason GetInternalReason(int unit, int service, string recourseResult) {
            switch (unit) {
            case 0:
                    if (service == 50019 || service == 50021 || service == 50023 || (service/1000 == 24))
                        return InternalReason.StrippedStage1;
                    else
                        return InternalReason.StrippedStage2;

            case 1:
                // Поликлиника, лечебная
                return Relax.InternalReason.AmbTreatment;

            case 3:
                // Дневной стационар
                if (service == 3001 || service == 3034)
                    return Relax.InternalReason.DayHosp;
                else
                    return Relax.InternalReason.SurgeryDayHosp;

            case 4:
                // Профилактика
                if (service == 50002 || service == 50001 || (service / 10000 == 6)) {
                    int r = 0;
                    if (int.TryParse(recourseResult, out r) && r == DD_ONCE_IN_TWO_YEARS_DONE)
                        return InternalReason.StrippedStage1;
                    else
                        return Relax.InternalReason.Other;
                } else {
                    return Relax.InternalReason.DispRegister;
                }

            case 5:
                // Неотложная помощь
                return Relax.InternalReason.Emergency;

            case 8:
                // ФОГК подросткам
                return InternalReason.Diagnostics;

            case 9:
                // Диспансеризация
                switch (service / 1000) {
                    case 27:
                        return Relax.InternalReason.Prof;

                    case 25:
                    case 28:
                        return Relax.InternalReason.Stage2;
                }
                return Relax.InternalReason.Stage1;

            default:
                return Relax.InternalReason.Unknown;
            }
        }

        /// <summary>
        /// Получить вид медицинской помощи V008
        /// </summary>
        int GetAidKind(ServiceAux sa) {
            switch (sa.ServiceCode / 100000) {
                case 7:
                    return AID_KIND_HITECH;

                case 4:
                    return AID_KIND_EMERGENCY;

                default:
                    int c = sa.ServiceCode / 1000;
                    if (c == 98 || c == 3) {
                        return AID_KIND_SPECIALIZED;
                    } else {
                        if (sa.AidProfile == "97")
                            return AID_KIND_PRIMARY;
                        else
                            return AID_KIND_PRIMARY_SPECIALIZED;
                    }
            }
        }

        /// <summary>
        /// Получить форму оказания медицинской помощи V014
        /// </summary>
        int GetAidForm(ServiceAux sa) {
            if (sa.ServiceCode / 10000 == 4) return AID_FORM_URGENT;
            if (sa.ServiceCode / 1000 == 7) return AID_FORM_PRESSING;
            return AID_FORM_ORDINAL;
        }

        /// <summary>
        /// Требуется ли направление для законченного случая
        /// </summary>
        /// <param name="rec">Законченный случай</param>
        public static bool NeedsDirection(Model.Recourse rec) {
            // Slimed by order 285
            //return rec.SuspectOncology
            //    // Плановая в круглосуточном стационаре или СДП
            //    || (rec.AidForm == AID_FORM_ORDINAL && rec.IsHospitalization)

            //    // Неотложная в круглосуточном стационаре
            //    || (rec.AidForm == AID_FORM_PRESSING && rec.Conditions == "1");

            return
                // В дневном стационаре
                (rec.Conditions == "2")
                // Плановая в круглосуточном стационаре
                || (rec.AidForm == AID_FORM_ORDINAL && rec.Conditions == "1");

        }
    }
}
