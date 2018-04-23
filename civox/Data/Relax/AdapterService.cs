using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Data.Relax {
    class AdapterService : AdapterBase<Model.Service> {
        public override Model.Service Read(System.Data.Common.DbDataReader reader, int number) {
            Model.Service result = new Model.Service() {
                ID = ReadInt(reader["RECID"]),
                Date = ReadDate(reader["D_U"]),
                ServiceCode = (int)(decimal)reader["COD"],
                CardNumber = ReadString(reader["C_I"]),
                Quantity = (int)(decimal)reader["K_U"],
                Price = (decimal) reader["S_ALL"],
                DoctorCode = ReadString(reader["TN1"]),
                ResultCode = Dict.LocalRezobr.FromLocal(ReadString(reader["BE"])),
                AidProfile = Dict.LocalAidKind.FromLocal(ReadString(reader["MSP"])),
                Profile = Dict.LocalProfile.FromLocal(ReadString(reader["PROFILE"]))
            };
            string sc = ReadString(reader["D_TYPE"]);
            if (!string.IsNullOrEmpty(sc) && sc != "0")
                result.SpecialCase = sc;
            return result;
        }
    }
}
