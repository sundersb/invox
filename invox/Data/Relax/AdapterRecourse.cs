using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Model;

namespace invox.Data.Relax {
    class AdapterRecourse : AdapterBase<Recourse> {
        const string SUSP_NEO_DIAGNOSIS = "Z03.1";
        static int[] REFUSAL_RESULTS = { 302, 408, 417, 207 };

        // V008
        const int AID_KIND_PRIMARY = 1;
        const int AID_KIND_EMERGENCY = 2;
        const int AID_KIND_SPECIALIZED = 31;
        const int AID_KIND_HITECH = 32;

        // V014
        const int AID_FORM_URGENT = 1;
        const int AID_FORM_PRESSING = 2;
        const int AID_FORM_ORDINAL = 3;
        
        public override Recourse Read(System.Data.Common.DbDataReader reader, int number) {
            Recourse result = new Recourse();

            result.Identity = ReadString(reader["RECID"]);
            result.Conditions = Dict.Condition.Instance.Get(ReadString(reader["COND"]));

            result.Department =  ReadString(reader["OTD"]);
            result.Profile = Dict.AidProfile.Instance.Get(ReadString(reader["MSP"]));
            
            int service = ReadInt(reader["COD"]);
            result.AidKind = GetAidKind(service);
            result.AidForm = GetAidForm(service);

            string ds = ReadString(reader["DS"]);
            result.SuspectOncology = ds == SUSP_NEO_DIAGNOSIS;
                        
            // TODO: Directed from outer MO?
            if (NeedsDirection(result)) {
                result.DirectedFrom = Options.LpuCode;
                //result.DirectionDate = ???
            }
            
            //BirthWeight

            result.Result = ReadInt(reader["RESCODE"]);

            result.Outcome = Dict.Outcome.Get(result.Conditions, ReadString(reader["IG"]).TrimStart('0'));
            
            result.PayKind = Dict.PayKind.Instance.Get(ReadString(reader["OPL"]));

            result.PayType = PayType.Full;
            result.MobileBrigade = false;
            result.DispanserisationRefusal = REFUSAL_RESULTS.Contains(result.Result);
            result.DispanserisationResult = Dict.DispResult.Instance.Get(ReadString(reader["BE"]));

            return result;
        }
    
        int GetAidKind(int serviceCode) {
            switch (serviceCode / 100000) {
                case 7:
                    return AID_KIND_HITECH;

                case 4:
                    return AID_KIND_EMERGENCY;

                default:
                    if (serviceCode / 1000 == 98)
                        return AID_KIND_SPECIALIZED;
                    else
                        return AID_KIND_PRIMARY;
            }
        }

        int GetAidForm(int serviceCode) {
            if (serviceCode / 10000 == 4) return AID_FORM_URGENT;
            if (serviceCode / 1000 == 7) return AID_FORM_PRESSING;
            return AID_FORM_ORDINAL;
        }

        bool NeedsDirection(Recourse rec) {
            return rec.SuspectOncology
                // Плановая в круглосуточном стационаре или СДП
                || (rec.AidForm == AID_FORM_ORDINAL && rec.IsHospitalization)

                // Неотложная в круглосуточном стационаре
                || (rec.AidForm == AID_FORM_PRESSING && rec.Conditions == "1");
        }
    }
}
