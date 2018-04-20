using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Data.Relax {
    class AdapterRecourse : AdapterBase<Model.Recourse> {
        public override Model.Recourse Read(System.Data.Common.DbDataReader reader, int number) {
            Model.Recourse result = new Model.Recourse() {
                Reason = (Model.Reason)(int)(decimal) reader["REASON"],
                Diagnosis = ReadString(reader["DS"]),
                Department = ReadString(reader["OTD"])
            };
            int fr = ReadInt(reader["F"]);
            // DIAGNOZIS.F:
            // 1 - Прочее
            // 2 - Острое
            // 3 - Хроническое, впервые выявленное
            // 4 - Хроническое, выявленное ранее
            // 5 - Д-учет
            result.FirstRevealed = fr == 2 || fr == 3;
            return result;
        }
    }
}
