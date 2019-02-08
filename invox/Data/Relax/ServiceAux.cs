using System;
using System.Linq;
using invox.Lib;
using System.Collections.Generic;

namespace invox.Data.Relax {
    /// <summary>
    /// Признак статистического учета заболеваемости
    /// </summary>
    /// <remarks>
    /// Соответственные значения полей в DIAGNOZ
    ///     - - 1
    ///     О - 2
    ///     В - 3
    ///     Х - 4
    ///     Д - 5
    /// V027
    /// 1 - Острое
    /// 2 - Хроническое впервые установленное
    /// 3 - Ранее установленное хроническое
    /// </remarks>
    enum StatisticCode : int {
        None = 1,               // Признака нет
        Acute = 2,              // Острое
        ChronicalManifest = 3,  // Впервые в жизни выявленное хроническое
        ChronicalKnown = 4,     // Хроническое, выявленное ранее, при первом посещении в данном году
        Dispensary = 5          // Диспансерный учет
    }

    /// <summary>
    /// Вспомогательный класс для извлечения сведений об услугах, событии и законченном случае
    /// </summary>
    class ServiceAux {
        /// <summary>
        /// Код услуги, начинающей 1 этап диспансеризации
        /// </summary>
        const int DD_ANTROPOMETRY_CODE = 24001;
        
        /// <summary>
        /// Коды DIAGNOZ.INSTAC, соответствующие поступлению самотеком 
        /// </summary>
        static int[] INDEPENDED_TRANSFERS = { 1, 4, 7, 8, 9, 12, 13, 14, 16, 18 };

        /// <summary>
        /// Коды результата обращения V009, соответствующие отказу от лечения
        /// </summary>
        static int[] CURATION_REFUSAL_RESULTS = { 107, 110, 207, 302, 408, 417 };

        /// <summary>
        /// Коды результата обращения V009, соответствующие прерванному лечению по "другим причинам"
        /// </summary>
        static int[] CURATION_ABORTED_CODES = {102, 103, 104, 105, 106, 108, 202, 203, 204, 205, 206, 208, 303, 405, 406 };
        
        string result;
        Model.Service.IncompleteServiceReason incompleteness;

        
        /// <summary>
        /// ID услуги
        /// </summary>
        public string ServiceId;

        /// <summary>
        /// Профиль медицинской помощи V002 (из KMU.MSP через мультиплексор)
        /// </summary>
        public string AidProfile;

        /// <summary>
        /// Код услуги
        /// </summary>
        public int ServiceCode;

        /// <summary>
        /// Результат обращения V009 (из REZOBR.SLIZ)
        /// </summary>
        public string Result {
            get { return result; }
            set { SetResult(value); }
        }

        /// <summary>
        /// Профиль койки V020 (из STRUCT.PROF через мультиплексор)
        /// </summary>
        public string BedProfile;

        /// <summary>
        /// Вид вмешательства V001 (в настоящее время пусто - поле не обязательно)
        /// </summary>
        public string InterventionKind;

        /// <summary>
        /// Уоличество услуг
        /// </summary>
        public int Quantity;

        /// <summary>
        /// Тариф
        /// </summary>
        public decimal Tariff;

        /// <summary>
        /// Стоимость услуги
        /// </summary>
        public decimal Total;

        /// <summary>
        /// Специальность врача V021 (из MEDPOST.CODEFSS пока без мультиплексора - уточнить)
        /// </summary>
        public string SpecialityCode;

        /// <summary>
        /// Табельный номер врача
        /// </summary>
        public string DoctorCode;

        /// <summary>
        /// Причина прерывания лечения (на основе результата обращения)
        /// </summary>
        public Model.Service.IncompleteServiceReason Incomplete {
            get { return incompleteness; }
        }

        /// <summary>
        /// Отказ от госпитализации (на основе результата обращения)
        /// </summary>
        public bool Refusal {
            get { return incompleteness == Model.Service.IncompleteServiceReason.Refusal; }
        }

        /// <summary>
        /// Дата услуги
        /// </summary>
        public DateTime Date;
        
        /// <summary>
        /// Признак новорожденного
        /// </summary>
        public bool Newborn;

        /// <summary>
        /// Код ЛПУ, направившего на лечение DIAGNOZS.SRZ_MCOD. Пока территориальный
        /// </summary>
        public string DirectedFrom;

        /// <summary>
        /// Масса тела при рождении
        /// </summary>
        public int BirthWeight;

        /// <summary>
        /// Исход госпитализации (второй вариант - из DIAGNOZS.BOLEND)
        /// </summary>
        public string Outcome1;

        /// <summary>
        /// Признак перевода (из DIAGNOZS.INSTAC)
        /// </summary>
        public Model.Transfer Transfer;

        /// <summary>
        /// Повод обращения, второй вариант (из DIAGNOZ.OBR)
        /// </summary>
        public string Reason1;

        /// <summary>
        /// Койко-дни
        /// </summary>
        public int BedDays;

        /// <summary>
        /// Диагноз при поступлении
        /// </summary>
        public string PrimaryDiagnosis;

        /// <summary>
        /// Сопутствующее заболевание
        /// </summary>
        public string ConcurrentDiagnosis;

        /// <summary>
        /// Осложнение
        /// </summary>
        public string ComplicationDiagnosis;

        /// <summary>
        /// Признак диспансерного наблюдения (берется из DIAGNOZ.HR)
        /// </summary>
        public Model.DispensarySupervision DispensarySupervision;

        /// <summary>
        /// Код МЭС конкурирующего заболевания (навскидку из DIAGNOZ/DIAGNOZS.KSG2)
        /// </summary>
        public string ConcurrentMesCode;

        /// <summary>
        /// Код статистического учета для внутренних нужд пула данных
        /// </summary>
        public StatisticCode StatisticCode;

        /// <summary>
        /// Признак впервые выявленного заболевания
        /// </summary>
        public bool FirstIdentified {
            get {
                return StatisticCode == Relax.StatisticCode.Acute
                    || StatisticCode == Relax.StatisticCode.ChronicalManifest;
            }
        }

        /// <summary>
        /// Является ли данная услуга началом 1 этапа диспансеризации?
        /// </summary>
        /// <returns></returns>
        public bool IsAntropometry() {
            return ServiceCode == DD_ANTROPOMETRY_CODE;
        }

        /// <summary>
        /// Получить признак перевода по коду перевода
        /// </summary>
        /// <param name="relaxCode">Код перевода DIAGNOZS.INSTAC</param>
        public static Model.Transfer GetTransfer(int relaxCode) {
            if (INDEPENDED_TRANSFERS.Contains(relaxCode))
                return Model.Transfer.Independently;

            switch (relaxCode) {
                case 10:
                    return Model.Transfer.Emergency;

                case 15:
                    return Model.Transfer.Transferred;

                default:
                    return Model.Transfer.None;
            }
        }

        /// <summary>
        /// Преобразовать данный объект в модель услуги
        /// </summary>
        /// <param name="ra">Вспомогательный объект законченного случая</param>
        public Model.Service ToService(RecourseAux ra) {
            Model.Service result = new Model.Service();

            result.Identity = ServiceId;
            result.Unit = ra.Unit;
            result.InterventionKind = InterventionKind;
            result.Child = ra.Child;

            SetDates(result, ra);

            result.Diagnosis = ra.MainDiagnosis;
            result.ServiceCode = ServiceCode;
            result.Quantity = Quantity;
            result.Tariff = Tariff;
            result.Total = Total;
            result.SpecialityCode = SpecialityCode;
            result.DoctorCode = DoctorCode;
            result.Incomplete = Incomplete;
            result.Refusal = Refusal;

            return result;
        }

        /// <summary>
        /// Установить даты начала и окончания услуги и количество услуг
        /// </summary>
        /// <param name="service">Услуга, которую требуется обновить</param>
        void SetDates(Model.Service service, RecourseAux ra) {
            if (BedDays <= 1) {
                Quantity = 1;
                service.DateFrom = service.DateTill = Date;
            } else {
                service.DateTill = Date;
#if GKP3
                // Since we are apriori polyclinic
                service.DateFrom = Date.WorkingDaysBefore(BedDays, true);
#else
                bool dayHospital = ra.InternalReason == InternalReason.DayHosp || ra.InternalReason == InternalReason.SurgeryDayHosp;
                service.DateFrom = Date.WorkingDaysBefore(Quantity, dayHospital);
#endif
            }
        }
        
        /// <summary>
        /// Установить результат обращения (V009). Побочно - определяет прерванное лечение и отказ
        /// </summary>
        void SetResult(string value) {
            if (value != result) {
                result = value;
                int code;
                if (int.TryParse(value, out code)) {
                    if (CURATION_REFUSAL_RESULTS.Contains(code)) {
                        incompleteness = Model.Service.IncompleteServiceReason.Refusal;
                    } else if (CURATION_ABORTED_CODES.Contains(code)) {
                        incompleteness = Model.Service.IncompleteServiceReason.Other;
                    } else {
                        incompleteness = Model.Service.IncompleteServiceReason.None;
                    }
                } else {
                    incompleteness = Model.Service.IncompleteServiceReason.None;
                }
            }
        }

        /// <summary>
        /// Extract not zero concurrent diseases from the services of an event
        /// </summary>
        public static List<string> GetConcurrentDiagnoses(List<ServiceAux> services) {
            var d1 = services.Select(s => s.ConcurrentDiagnosis).Where(d => !string.IsNullOrEmpty(d));
            if (d1.Count() > 0)
                return d1.ToList();
            else
                return null;
        }

        /// <summary>
        /// Extract not zero complications from the services of an event
        /// </summary>
        public static List<string> GetComplicationDiagnoses(List<ServiceAux> services) {
            var d1 = services.Select(s => s.ComplicationDiagnosis).Where(d => !string.IsNullOrEmpty(d));
            if (d1.Count() > 0)
                return d1.ToList();
            else
                return null;
        }
    
        /// <summary>
        /// Получить код диспансерного наблюдения по коду Релакс
        /// </summary>
        /// <param name="sldispCode">Код DIAGNOZ.HR для данной услуги</param>
        /// <remarks>
        /// DIAGNOZ.HR -> SLDISP
        /// 1 - Состоит
        /// 2 - Взят
        /// 3 - Снят
        /// 
        /// DIAGNOZ.HRO -> SLDISPOF
        /// 1 - Выздоровление
        /// 2 - Переезд
        /// 3 - Смерть
        /// 4 - Умер, 6 мес.
        /// </remarks>
        public static Model.DispensarySupervision GetDispensarySupervision(int sldispCode, int dispofCode) {
            switch (sldispCode) {
                case 1:
                    return Model.DispensarySupervision.Observed;

                case 2:
                    return Model.DispensarySupervision.Taken;

                case 3:
                    if (dispofCode == 1)
                        return Model.DispensarySupervision.Recovery;
                    else
                        return Model.DispensarySupervision.Cancelled;

                default:
                    return Model.DispensarySupervision.None;
            }
        }
    }
}
