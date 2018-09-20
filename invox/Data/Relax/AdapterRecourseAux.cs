using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Data.Relax {
    class AdapterRecourseAux : AdapterBase<RecourseAux> {
        public override RecourseAux Read(System.Data.Common.DbDataReader reader, int number) {
            RecourseAux result = new RecourseAux();

            result.OrdinalNumber = number;

            result.PersonId = ReadString(reader["PERSON_ID"]);
            result.ServiceId = ReadString(reader["SERVICE_ID"]);
            result.Unit = ReadString(reader["UNIT"]);
            result.AidProfile = Dict.AidProfile.Instance.Get(ReadString(reader["AID_PROFILE"]));
            result.AidConditions = Dict.Condition.Instance.Get(ReadString(reader["AID_CONDITIONS"]));
            result.ServiceCode = ReadInt(reader["SERVICE_CODE"]);
            result.Result = ReadInt(reader["RESULT"]);
            result.Outcome = Dict.Outcome.Get(result.AidConditions, ReadString(reader["OUTCOME"]).TrimStart('0'));
            result.PayKind = Dict.PayKind.Instance.Get(ReadString(reader["PAY_KIND"]));
            result.PayType = (Model.PayType) ReadInt(reader["PAY_TYPE"]);
            result.MobileBrigade = ReadBool(reader["MOBILE_BRIGADE"]);
            result.RecourseResult = ReadString(reader["RECOURSE_RESULT"]);
            result.BedProfile = Dict.BedProfile.Instance.Get(ReadString(reader["BED_PROFILE"]));
            result.Child = ReadBool(reader["DET"]);
            result.Reason = ReadString(reader["REASON"]);
            result.CardNumber = ReadString(reader["CARD_NUMBER"]);
            result.MainDiagnosis = ReadString(reader["DS_MAIN"]);
            result.Rehabilitation = ReadBool(reader["REHABILITATION"]);
            result.InterventionKind = ReadString(reader["INTERVENTION_KIND"]);
            result.Quantity = ReadInt(reader["QUANTITY"]);
            result.Tariff = (decimal) reader["TARIFF"];
            result.Total = (decimal) reader["TOTAL"];
            result.SpecialityCode = ReadString(reader["SPECIALITY_CODE"]);
            result.DoctorCode = ReadString(reader["DOCTOR_CODE"]);
            result.Date = ReadDate(reader["D_U"]);

            result.UpdateInternalReason();

            return result;
        }
    }
}
