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
        Fluorography = 12   // ФОГК подросткам
    }

    static class InternalReasonHelper {
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

                case InternalReason.Fluorography:
                    return "2.1";

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
        /// <param name="reason"></param>
        /// <param name="isSoul"></param>
        /// <returns></returns>
        public static string ToFomsReason(InternalReason reason, bool isSoul) {
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

                case InternalReason.Fluorography: return "21";

                default: return string.Empty;
            }
        }
#endif
    }
}
