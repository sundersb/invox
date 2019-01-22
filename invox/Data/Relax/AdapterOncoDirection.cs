using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Data.Relax {
    class AdapterOncoDirection : AdapterBase<Model.OncologyDirection> {
        public override Model.OncologyDirection Read(System.Data.Common.DbDataReader reader, int number) {
            DateTime date = ReadDate(reader["DIRECTION_DATE"]);
            string value = ReadString(reader["DIRECTION"]);
            return new Model.OncologyDirection(value, date);
        }
    }
}
