using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Data.Relax {
    /// <summary>
    /// Повод обращения - для внутренних нужд
    /// </summary>
    enum InternalReason {
        Unknown = 0,        // Неверный повод
        AmbTreatment = 1,   // Лечебная
        DayHosp = 2,        // СДП
        SurgeryDayHosp = 3, // СДП (хирургия)
        Other = 4,          // Иная цель
        DispRegister = 5,   // Д-учет
        Emergency = 6,      // Неотложная помощь
        Prof = 7,           // Профосмотр старше 18 лет
        Stage1 = 8,         // Диспансеризация, 1 этап
        Stage2 = 9,         // Диспансеризация, 2 этап
        StrippedStage1 = 10,// Дисп. раз в два года - 1 этап
        StrippedStage2 = 11,// Дисп. раз в два года - 2 этап
        Diagnostics = 12    // ФОГК подросткам
    }

    /// <summary>
    /// Сопоставление цели посещения диагнозу для разделов D1, D4 (ФОМС)
    /// </summary>
    enum DiagnosisKind {
        Treatment,
        Prophylax,
        NeverMind
    }

    static class InternalReasonHelper {
        const string DEFAULT_DIAGNOSIS_TREATMENT = "I13.2";
        const string DEFAULT_DIAGNOSIS_DIAGNOSTICS = "Z01.9";
        const string DEFAULT_DIAGNOSIS_PROPHYLAX = "Z00.0";
        const string DEFAULT_DIAGNOSIS_OTHER = "Z76.8";

        /// <summary>
        /// Преобразовать InternalReason в коды V025 Классификатор целей посещения (KPC)
        /// </summary>
        /// <param name="reason">Цель обращения</param>
        /// <returns>Код цели посещения V025</returns>
        public static string ToVisitAim(InternalReason reason) {
            switch (reason) {
                case InternalReason.AmbTreatment:
                case InternalReason.DayHosp:
                case InternalReason.SurgeryDayHosp:
                    return "3.0";

                case InternalReason.Other:
                    return "2.6";

                case InternalReason.DispRegister:
                    return "1.3";

                case InternalReason.Emergency:
                    return "1.1";

                case InternalReason.Prof:
                    return "3.1";

                case InternalReason.Stage1:
                case InternalReason.Stage2:
                case InternalReason.StrippedStage1:
                case InternalReason.StrippedStage2:
                    return "2.2";

                case InternalReason.Diagnostics:
                    // Письмо ФОМС 1.03.2019 №10-673/9
                    return "2.6";

                default: return "2.6";
            }
        }
/*
 * V025 Классификатор целей посещения (KPC)
1.0	Посещение по заболеванию
1.1	Посещениe в неотложной форме
1.2	Aктивное посещение
1.3	Диспансерное наблюдение
2.1	Медицинский осмотр
2.2	Диспансеризация
2.3	Комплексное обследование
2.5	Патронаж
2.6	Посещение по другим обстоятельствам
3.0	Обращение по заболеванию
3.1	Обращение с профилактической целью
 */
#if FOMS
        /// <summary>
        /// Получить код цели обращения по локальному справочнику ХКФОМС
        /// </summary>
        /// <param name="reason">Повод обращения</param>
        /// <param name="isSoul">Подушевое финансирование?</param>
        /// <returns>Значение поля CEL законченного случая</returns>
        public static string ToFomsReason(this InternalReason reason, bool isSoul) {
            switch (reason) {
                case InternalReason.AmbTreatment:
                    return isSoul ? "17" : "16";
                
                case InternalReason.DayHosp:
                case InternalReason.SurgeryDayHosp:
                    return "3";

                case InternalReason.Other:
                case InternalReason.DispRegister:
                    return isSoul ? "5" : "4";

                case InternalReason.Emergency: return "18";
                case InternalReason.Prof: return "15";
                case InternalReason.Stage1: return "8";
                case InternalReason.Stage2: return "9";
                
                case InternalReason.StrippedStage1: return isSoul ? "33" : "35";
                case InternalReason.StrippedStage2: return isSoul ? "34" : "36";

                case InternalReason.Diagnostics: return "21";

                default: return string.Empty;
            }
        }
#endif

        /// <summary>
        /// Получить требуемый проверкой тип диагноза в зависимости от цели обращения
        /// </summary>
        /// <returns>Тип диагноза, сопоставляющий его с требуемой целью обращения</returns>
        public static DiagnosisKind GetDiagnosisKind(this InternalReason reason) {
            switch (reason) {
                case InternalReason.Diagnostics:
                case InternalReason.Other:
                case InternalReason.Prof:
                    return DiagnosisKind.Prophylax;

                case InternalReason.AmbTreatment:
                case InternalReason.DayHosp:
                case InternalReason.SurgeryDayHosp:
                case InternalReason.DispRegister:
                case InternalReason.Emergency:
                    return DiagnosisKind.Treatment;

                default:
                    return DiagnosisKind.NeverMind;
            }
        }

        /// <summary>
        /// Получить диагноз "по умолчанию" для данного повода обращения. Только для разделов D1 и D4 приказа!
        /// </summary>
        /// <param name="reason">Повод обращения</param>
        /// <returns>Соответствующий диагноз</returns>
        public static string DefaultDiagnosis(this InternalReason reason) {
            switch (reason) {
                case InternalReason.Diagnostics:
                    return DEFAULT_DIAGNOSIS_DIAGNOSTICS;

                case InternalReason.Prof:
                    return DEFAULT_DIAGNOSIS_PROPHYLAX;

                case InternalReason.Other:
                    return DEFAULT_DIAGNOSIS_OTHER;

                default:
                    return DEFAULT_DIAGNOSIS_TREATMENT;
            }
        }
    }

    //Коллеги, у ТФОМС вышла новая проверка 
    //"Соответствие цели посещения диагнозу"
    //Для пакетов СМ, НМ
    //Для P_CEL (V025)=1.0; 1.1; 1.2; 1.3; 3.0   DS1=(A00-T98)
    //P_CEL (V025)=2.3; 2.5; 2.6; 3.1  DS1=(Z00-Z99)
    /// <summary>
    /// Сопоставление диагноза цели посещения
    /// </summary>
    static class DiagnosisKindHelper {
        /// <summary>
        /// Проверка соответствия диагноза требуемому целью обращения
        /// </summary>
        /// <param name="kind">Ожидаемый тип диагноза в зависимости от цели обращения</param>
        /// <param name="diagnosis">Диагноз</param>
        public static bool Matches(this DiagnosisKind kind, string diagnosis) {
            if (string.IsNullOrEmpty(diagnosis)) return false;

            switch (kind) {
                case DiagnosisKind.Prophylax:
                    return diagnosis[0] == 'Z';

                case DiagnosisKind.Treatment:
                    return diagnosis[0] >= 'A' & diagnosis[0] <= 'T';

                default: return true;
            }
        }
    }
}
