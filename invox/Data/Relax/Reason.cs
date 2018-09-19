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
        Prof = 7,           // Профосмотр
        Stage1 = 8,         // Диспансеризация, 1 этап
        Stage2 = 9,         // Диспансеризация, 2 этап
        StrippedStage1 = 10,// Дисп. раз в два года - 1 этап
        StrippedStage2 = 11,// Дисп. раз в два года - 2 этап
        Fluorography = 12   // ФОГК подросткам
    }
}
