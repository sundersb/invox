using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Model {
    /// <summary>
    /// Telling simple single-day recourse from cases with multiple services
    /// </summary>
    class ReasonHelper {
        static Reason[] SINGLE_DAY = {
            Reason.DayHosp,
            Reason.SurgeryDayHosp,
            Reason.Other,
            Reason.Emergency,
            Reason.Prof,
            Reason.DispRegister
        };

        /// <summary>
        /// True if the reason provided is a single-day recourse
        /// </summary>
        /// <param name="reason">Recourse reason</param>
        public static bool IsSingleDay(Reason reason) {
            return SINGLE_DAY.Contains(reason);
        }
    }

    /// <summary>
    /// Cause of recourse (повод обращения)
    /// </summary>
    enum Reason : int {
        Unknown = 0,        // Неверный повод
        AmbTreatment = 1,   // Лечебная
        DayHosp = 2,        // СДП
        SurgeryDayHosp = 3, // СДП (хирургия)
        Other = 4,          // Иная цель
        DispRegister = 5,   // Д-учет
        Emergency = 6,      // Неотложная помощь
        Prof = 7,           // Профосмотр
        Stage1 = 8,         // Диспансеризация, 1 этап
        Stage2 = 9,         // Диспансеризация, 2 этап
        StrippedStage1 = 10,// Дисп. раз в два года - 1 этап
        StrippedStage2 = 11 // Дисп. раз в два года - 2 этап
    }
}
