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
            return result;
        }
    }
}
