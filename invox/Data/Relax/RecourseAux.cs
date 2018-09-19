using System;
using System.Linq;

namespace invox.Data.Relax {
    class RecourseAux {
        const string SUSP_NEO_DIAGNOSIS = "Z03.1";
        static int[] REFUSAL_RESULTS = { 302, 408, 417, 207 };
        const int DD_ONCE_IN_TWO_YEARS_DONE = 98;

        // V008
        const int AID_KIND_PRIMARY = 1;
        const int AID_KIND_EMERGENCY = 2;
        const int AID_KIND_SPECIALIZED = 31;
        const int AID_KIND_HITECH = 32;

        // V014
        const int AID_FORM_URGENT = 1;
        const int AID_FORM_PRESSING = 2;
        const int AID_FORM_ORDINAL = 3;

        public int OrdinalNumber;
        public string PersonId;         // P.RECID
        public string ServiceId;        // S.RECID
        public string Unit;             // S.OTD
        public string AidProfile;       // KMU.MSP
        public string AidConditions;    // SLUMP.SLUSL
        public int ServiceCode;         // S.COD
        public string Result;              // REZOBR.SLIZ
        public string Outcome;          // S.IG
        public string PayKind;          // KMU.OPL
        public Model.PayType PayType;   // 1
        public bool MobileBrigade;      // ??? DD, No way to know yet
        public string RecourseResult;   // S.BE
        public string BedProfile;       // STRUCT.PROF
        public bool Child;              // KMU.LDET
        public string Reason;           // ??? V025 SLOBR-SLUMP-SLPOS
        public string CardNumber;       // S.C_I
        public string MainDiagnosis;    // S.DS
        public bool Rehabilitation;     // ???
        public string InterventionKind; // ??? v001
        public int Quantity;            // S.K_U
        public decimal Tariff;           // S.S_ALL
        public decimal Total;            // S.S_ALL
        public string SpecialityCode;   // MEDPOST.CODEFSS
        public string DoctorCode;       // S.TN1
        public DateTime Date;           // S.D_U
        public InternalReason InternalReason;

        /// <summary>
        /// Создать повод обращения и заполнить его доступными полями
        /// </summary>
        public Model.Recourse ToRecourse() {
            Model.Recourse result = new Model.Recourse();

            result.SuspectOncology = MainDiagnosis == SUSP_NEO_DIAGNOSIS;
            result.Department = Unit;
            result.Profile = AidProfile;
            result.Identity = ServiceId;
            result.AidKind = GetAidKind();
            result.AidForm = GetAidForm();
            //result.DirectedFrom;                      - Pool.LoadEvents
            //result.DirectionDate;                                        !!!! nowhere to be found !!!!
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
            result.DispanserisationResult = Dict.DispResult.Instance.Get(RecourseResult);

            return result;
        }

        /// <summary>
        /// Создать событие и заполнить его доступными полями
        /// </summary>
        public Model.Event ToEvent() {
            Model.Event result = new Model.Event();

            // result.Services;                         - Pool.LoadEvents
            result.Identity = ServiceId;
            result.Unit = Unit;
            result.BedProfile = BedProfile;
            result.Child = Child;
            result.Reason = Reason;
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
            //result.ClinicalGroup;

            result.Rehabilitation = Rehabilitation;
            result.Quantity = Quantity;
            result.SpecialityCode = SpecialityCode;
            result.DoctorCode = DoctorCode;

            //result.Tariff;                            - Pool.LoadEvents
            //result.Total;                             - Pool.LoadEvents
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
            InternalReason = GetInternalReason(Unit, ServiceCode, RecourseResult);
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

        int GetAidKind() {
            switch (ServiceCode / 100000) {
                case 7:
                    return AID_KIND_HITECH;

                case 4:
                    return AID_KIND_EMERGENCY;

                default:
                    if (ServiceCode / 1000 == 98)
                        return AID_KIND_SPECIALIZED;
                    else
                        return AID_KIND_PRIMARY;
            }
        }

        int GetAidForm() {
            if (ServiceCode / 10000 == 4) return AID_FORM_URGENT;
            if (ServiceCode / 1000 == 7) return AID_FORM_PRESSING;
            return AID_FORM_ORDINAL;
        }


    }
}
