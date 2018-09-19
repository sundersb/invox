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
            result.Result = ReadString(reader["RESULT"]);
            result.Outcome = Dict.Outcome.Get(result.AidConditions, ReadString(reader["OUTCOME"]).TrimStart('0'));
            result.PayKind = Dict.PayKind.Instance.Get(ReadString(reader["PAY_KIND"]));
            result.PayType = (Model.PayType) ReadInt(reader["PAY_TYPE"]);
            result.MobileBrigade = ReadBool(reader["MOBILE_BRIGADE"]);
            result.RecourseResult = ReadString(reader["RECOURSE_RESULT"]);
            result.BedProfile = ReadString(reader["BED_PROFILE"]);
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

            //public bool Incomplete;         // ??? by REZOBR.SLIZ
            //public bool Refusal;            // ??? by REZOBR.SLIZ


            //int service = ReadInt(reader["SERVICE_CODE"]);
            //result.AidKind = GetAidKind(service);
            //result.AidForm = GetAidForm(service);

            //string ds = ReadString(reader["DS_MAIN"]);
            //result.SuspectOncology = ds == SUSP_NEO_DIAGNOSIS;

            //// TODO: Directed from outer MO?
            //// TODO: On loading events
            //if (NeedsDirection(result)) {
            //    result.DirectedFrom = Options.LpuCode;
            //    //result.DirectionDate = ???
            //}

            ////BirthWeight
            
            //result.DispanserisationRefusal = REFUSAL_RESULTS.Contains(result.Result);
            //result.DispanserisationResult = Dict.DispResult.Instance.Get(ReadString(reader["RECOURSE_RESULT"]));

            result.UpdateInternalReason();

            return result;
        }
    }
}
