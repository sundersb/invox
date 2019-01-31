using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Model;

namespace invox.Data.Relax {
    class AdapterEvent : AdapterBase<Event> {
        public override Event Read(System.Data.Common.DbDataReader reader, int number) {
            Event result = new Event();
            
            result.Identity = ReadString(reader["RECID"]);
            
            int service = ReadInt(reader["COD"]);
            
            // if hospitalization
            result.BedDays = ReadInt(reader["K_U"]);
            result.BedProfile = Dict.BedProfile.Instance.Get(ReadString(reader["BED_PROFILE"]));
            // endif

            result.Child = ReadBool(reader["DET"]);

            DateTime date = ReadDate(reader["D_U"]);
            result.CardNumber = ReadString(reader["C_I"]);

            return result;
        }
    }
}
