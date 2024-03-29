﻿using System;
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
            result.Department = ReadString(reader["DEPT"]);
            result.Unit = ReadString(reader["UNIT"]);

            //result.AidProfile = Dict.AidProfile.Instance.Get(ReadString(reader["AID_PROFILE"]));
            result.SpecialityCode = SpecialityDict.Get(ReadString(reader["SPECIALITY_ID"]));

            // TODO: Which is more precise?
            result.AidProfile = ReadString(reader["AID_PROFILE"]);
            //result.AidProfile = Dict.AidProfileBySpeciality.Instance.Get(ReadString(reader["MSP"]), result.SpecialityCode);

            result.PayKind = ReadString(reader["PAY_KIND"]);
            int dummy = ReadInt(ReadString(reader["AID_CONDITIONS"]));
            result.AidConditions = Dict.Condition.Instance.Get(dummy.ToString());

            result.ServiceCode = ReadInt(reader["SERVICE_CODE"]);
            result.Result = ReadString(reader["RESULT"]);
            result.Outcome = ReadString(reader["OUTCOME"]);
            result.PayType = (Model.PayType) ReadInt(reader["PAY_TYPE"]);
            result.MobileBrigade = ReadBool(reader["MOBILE_BRIGADE"]);
            result.RecourseResult = ReadString(reader["RECOURSE_RESULT"]);
            result.Child = ReadBool(reader["DET"]);
            result.CardNumber = ReadString(reader["CARD_NUMBER"]);
            result.MainDiagnosis = ReadString(reader["DS_MAIN"]);
            result.Rehabilitation = ReadBool(reader["REHABILITATION"]);
            result.InterventionKind = ReadString(reader["INTERVENTION_KIND"]);
            result.Quantity = ReadInt(reader["QUANTITY"]);
            result.Tariff = (decimal) reader["TARIFF"];
            result.Total = (decimal) reader["TOTAL"];
            result.DoctorCode = ReadString(reader["DOCTOR_CODE"]);
            result.Date = ReadDate(reader["D_U"]);
            result.ServiceKind = ReadInt(reader["SERVICE_KIND"]);

            // Дневной стационар у нас - поликлиническое отделение, не имеющее профиля койки.
            // Отсюда вручную прописываем терапию или неврологию в зависимости от диагноза
            if (dummy == 1 || dummy == 2) {
                result.BedProfile = Dict.BedProfile.Instance.Get(ReadString(reader["BED_PROFILE"]));
                if (result.BedProfile == "???") {
                    if (result.MainDiagnosis.StartsWith("I6"))
                        result.BedProfile = "34";
                    else
                        result.BedProfile = "71";
                }
            }

            result.Update();

            return result;
        }
    }
}
