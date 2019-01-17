using System;
using System.Linq;

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

        static int[] REFUSAL_RESULTS = { 302, 408, 417, 207 };

        // V008
        const int AID_KIND_PRIMARY = 1;
        const int AID_KIND_EMERGENCY = 2;
        const int AID_KIND_SPECIALIZED = 31;
        const int AID_KIND_HITECH = 32;

        // V014
        const int AID_FORM_URGENT = 1;
        const int AID_FORM_PRESSING = 2;
        const int AID_FORM_ORDINAL = 3;

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
        /// Результат обращения V009 (из REZOBR.SLIZ)
        /// </summary>
        public int Result;              // REZOBR.SLIZ

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
        public string MainDiagnosis;    // S.DS

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
            result.AidKind = GetAidKind();
            result.AidForm = GetAidForm();
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
        /// Создать событие и заполнить его доступными полями
        /// </summary>
        public Model.Event ToEvent(Model.Recourse rec) {
            Model.Event result = new Model.Event();

            // result.Services;                         - Pool.LoadEvents
            result.Identity = ServiceId;
            result.Unit = Unit;
            
            if (rec.IsHospitalization)
                result.BedProfile = BedProfile;
            
            result.Child = Child;
            result.Reason = InternalReasonHelper.ToVisitAim(InternalReason);

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

            result.Rehabilitation = Rehabilitation;
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
        public void UpdateInternalReason() {
            InternalReason = GetInternalReason(Department, ServiceCode, RecourseResult);
        }

        /// <summary>
        /// Получить повод обращения
        /// </summary>
        /// <param name="unit">Код отделения (S.OTD)</param>
        /// <param name="service">Код услуги (S.COD)</param>
        /// <param name="recourseResult">Код результата обращения (S.BE)</param>
        /// <returns>Повод обращения</returns>
        public static Relax.InternalReason GetInternalReason(string unit, int service, string recourseResult) {
            int u;
            if (!int.TryParse(unit, out u)) u = 1;

            switch (u) {
            case 0:
                    if (service == 50019 || service == 50021 || (service/1000 == 24))
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
                return InternalReason.Fluorography;

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
        int GetAidKind() {
            switch (ServiceCode / 100000) {
                case 7:
                    return AID_KIND_HITECH;

                case 4:
                    return AID_KIND_EMERGENCY;

                default:
                    int c = ServiceCode / 1000;
                    if (c == 98 || c == 3)
                        return AID_KIND_SPECIALIZED;
                    else
                        return AID_KIND_PRIMARY;
            }
        }

        /// <summary>
        /// Получить форму оказания медицинской помощи V014
        /// </summary>
        int GetAidForm() {
            if (ServiceCode / 10000 == 4) return AID_FORM_URGENT;
            if (ServiceCode / 1000 == 7) return AID_FORM_PRESSING;
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
