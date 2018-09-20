using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Model;

namespace invox.Data.Relax {
    class AdapterConcomitantDisease : AdapterBase<ConcomitantDisease> {
        public override ConcomitantDisease Read(System.Data.Common.DbDataReader reader, int number) {
            ConcomitantDisease result = new ConcomitantDisease();

            result.Code = ReadString(reader["DIAG2"]);
            result.DispensarySupervision = ServiceAux.GetDispensarySupervision(ReadInt(reader["HR2"]),
                ReadInt(reader["HRO2"]));
            
            StatisticCode code = (StatisticCode)ReadInt(reader["F"]);
            result.FirstIdentified = code == StatisticCode.Acute
                    || code == Relax.StatisticCode.ChronicalManifest;

            return result;
        }
    }
}
