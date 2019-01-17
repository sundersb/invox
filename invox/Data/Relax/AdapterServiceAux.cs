using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Data.Relax {
    class AdapterServiceAux : AdapterBase<ServiceAux> {
        public override ServiceAux Read(System.Data.Common.DbDataReader reader, int number) {
            ServiceAux result = new ServiceAux();

            result.ServiceId = ReadString(reader["SERVICE_ID"]);
            result.AidProfile = Dict.AidProfile.Instance.Get(ReadString(reader["AID_PROFILE"]));
            result.ServiceCode = ReadInt(reader["SERVICE_CODE"]);
            result.Result = ReadString(reader["RESULT"]);
            result.BedProfile = ReadString(reader["BED_PROFILE"]);
            result.Quantity = ReadInt(reader["QUANTITY"]);
            result.Tariff = (decimal)reader["TARIFF"];
            result.Total = (decimal)reader["TOTAL"];
            result.SpecialityCode = ReadString(reader["SPECIALITY_CODE"]);
            result.DoctorCode = ReadString(reader["DOCTOR_CODE"]);
            result.Date = ReadDate(reader["D_U"]);
            result.Newborn = ReadBool(reader["NOVOR"]);
            result.DirectedFrom = ReadString(reader["DIRECTION_FROM"]);
            result.BirthWeight = ReadInt(reader["BIRTH_WEIGHT"]);
            
            result.Outcome1 = ReadString(reader["OUTCOME1"]);
            // Dict.Outcome.Get(result.AidConditions, ReadString(reader["OUTCOME"]).TrimStart('0'));

            result.Transfer = ServiceAux.GetTransfer(ReadInt(reader["TRANSFER"]));
            result.Reason1 = ReadString(reader["REASON1"]); //
            result.BedDays = ReadInt(reader["BED_DAYS"]);
            result.PrimaryDiagnosis = ReadString(reader["DS_PRIMARY"]);
            result.ConcurrentDiagnosis = ReadString(reader["DS_CONCURRENT"]);
            result.ComplicationDiagnosis = ReadString(reader["DS_COMPLICATION"]);
            result.ConcurrentMesCode = ReadString(reader["CONCURRENT_MES_CODE"]);
            result.StatisticCode = (StatisticCode)ReadInt(reader["CHARACTER"]);

            int dummy = ReadInt(reader["DISPENSARY_SUPERVISION"]);
            result.DispensarySupervision = ServiceAux.GetDispensarySupervision(dummy, ReadInt(reader["DCANCEL_REASON"]));

            return result;
        }
    }
}
