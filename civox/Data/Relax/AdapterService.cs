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
                ResultCode = Dict.Rezobr.Instance.Get(ReadString(reader["BE"])),
                AidProfile = Dict.AidKind.Instance.Get(ReadString(reader["MSP"])),
                DoctorProfile = Dict.Profile.Instance.Get(ReadString(reader["PROFILE"])),
            };
            string dummy = ReadString(reader["D_TYPE"]);
            if (!string.IsNullOrEmpty(dummy) && dummy != "0")
                result.SpecialCase = dummy;

            dummy = ReadString(reader["OPL"]);
            result.PayKind = Dict.PayKind.Instance.Get(dummy);
            result.RecourseAim = Dict.RecourseAim.Instance.Get(dummy);

            return result;
        }
    }
}
