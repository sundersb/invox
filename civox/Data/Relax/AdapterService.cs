using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Data.Relax {
    class AdapterService : AdapterBase<Model.Service> {
        static string[] REFUSAL_RESULTS = { "302", "408", "417", "207" };

        public override Model.Service Read(System.Data.Common.DbDataReader reader, int number) {
            Model.Service result = new Model.Service() {
                ID = ReadInt(reader["RECID"]),
                ServiceCode = (int)(decimal)reader["COD"],
                CardNumber = ReadString(reader["C_I"]),
                Price = (decimal) reader["S_ALL"],
                DoctorCode = ReadString(reader["TN1"]),
                AidProfile = Dict.AidKind.Instance.Get(ReadString(reader["MSP"])),
                DoctorProfile = Dict.Profile.Instance.Get(ReadString(reader["PROFILE"])),
            };
            string dummy = ReadString(reader["D_TYPE"]);
            if (!string.IsNullOrEmpty(dummy) && dummy != "0")
                result.SpecialCase = dummy;

            dummy = ReadString(reader["OPL"]);
            result.PayKind = Dict.PayKind.Instance.Get(dummy);
            result.RecourseAim = Dict.RecourseAim.Instance.Get(dummy);

            result.SetDates(ReadDate(reader["D_U"]), (int)(decimal)reader["K_U"]);

            result.Refusal = REFUSAL_RESULTS.Contains(result.ResultCode);

            dummy = ReadString(reader["BE"]);
            result.ResultCode = Dict.Rezobr.Instance.Get(dummy);
            result.DispResultCode = Dict.DispResult.Instance.Get(dummy);

            dummy = ReadString(reader["EXTR"]);

            // PATU.EXTR: 1 - планово, 2 - экстренно
            result.UrgentHospitalization = dummy == "2";

            return result;
        }
    }
}
