using System;

namespace civox.Data.Relax {
    class AdapterPerson : AdapterBase<Model.Person> {
        public override Model.Person Read(System.Data.Common.DbDataReader reader, int number) {
            Model.Person result = new Model.Person();

            result.ID = ReadInt(reader["RECID"]);
            result.Family = ReadString(reader["FAM"]);
            result.Name = ReadString(reader["IM"]);
            result.Patronymic = ReadString(reader["OT"]);
            result.Sex = (int)(Decimal)reader["W"];
            result.BirthDate = ReadDate(reader["DR"]);
            result.SNILS = ReadString(reader["SS"]);
            result.Address = ReadString(reader["ADRES"]);
            result.SocialPosition = ReadInt(reader["SP"]);
            result.ResidenceOKATO = "08401000000";

            string docType = Dict.DocumentType.Get(ReadString(reader["Q_PASP"]));
            string serial = ReadString(reader["SN_PASP"]);

            result.SetDocument(docType, serial);

            // SN_POL?
            return result;
        }
    }
}
