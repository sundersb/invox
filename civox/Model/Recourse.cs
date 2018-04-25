﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Model {
    /// <summary>
    /// Случай обращения за МП
    /// </summary>
    class Recourse {
        /// <summary>
        /// Повод обращение
        /// </summary>
        public Reason Reason;

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
