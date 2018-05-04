using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Model {
    /// <summary>
    /// Случай обращения за МП
    /// </summary>
    class Recourse {
        Reason reason;

        /// <summary>
        /// Повод обращения
        /// </summary>
        public Reason Reason {
            get { return reason; }
            set {
                reason = value;
                switch (value) {
                    case Reason.AmbTreatment:
                    case Reason.DayHosp:
                    case Reason.SurgeryDayHosp:
                    case Reason.Emergency:
                    case Reason.DispRegister:
                        Section = AppendixSection.D1;
                        break;

                    case Reason.Prof:
                    case Reason.Stage1:
                    case Reason.Stage2:
                    case Reason.StrippedStage1:
                    case Reason.StrippedStage2:
                        Section = AppendixSection.D3;
                        break;

                    default:
                        Section = AppendixSection.D1;
                        break;
                }
            }
        }

        /// <summary>
        /// Раздел приложения к приказам
        /// </summary>
        public AppendixSection Section { get; private set; }

        public string Diagnosis;

        /// <summary>
        /// Признак впервые выявленного заболевания
        /// </summary>
        public bool FirstRevealed;

        /// <summary>
        /// Код подразделения ЛПУ
        /// </summary>
        public string Department;

        /// <summary>
        /// Условия оказания медицинской помощи V006
        /// </summary>
        public string Condition;

        /// <summary>
        /// Исход обращения (госпитализации) V012
        /// </summary>
        public string Outcome;

        /// <summary>
        /// Является ли данный случай диспансеризацией
        /// </summary>
        public bool IsDispanserisation() {
            return this.Reason == Reason.Stage1 ||
                this.Reason == Reason.Stage2 ||
                this.Reason == Reason.StrippedStage1 ||
                this.Reason == Reason.StrippedStage2;
        }
    }
}
