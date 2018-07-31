using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Data.Relax {
    class AdapterPrvsError: AdapterBase<string> {
        public override string Read(System.Data.Common.DbDataReader reader, int number) {
            return string.Format("Врач '{0}' не прописан в отделении '{1}'",
                ReadString(reader["TN1"]),
                ReadString(reader["OTD"]));
        }
    }
}
