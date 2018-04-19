using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Model {
    /// <summary>
    /// DOST field reference for people export
    /// </summary>
    enum IdentityReliability : int {
        noPatronymic = 1,
        noFamily,
        noName,
        yearMonth,
        onlyYear,
        wrongBirthDate
    }
}
