using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Model;

namespace invox.Data.Relax {
    class AdapterPerson : AdapterBase<Person> {
        const string DEFAULT_OKATO = "08401000000";

        public override Person Read(System.Data.Common.DbDataReader reader, int number) {
            Person result = new Person();
            result.ID = ReadString(reader["RECID"]);
            result.Family = ReadString(reader["FAM"]);
            result.Name = ReadString(reader["IM"]);
            result.Patronymic = ReadString(reader["OT"]);
            result.Sex = ReadInt(reader["W"]);
            result.BirthDate = ReadDate(reader["DR"]);
            result.Snils = ReadString(reader["SS"]);

            result.DocumentType = Dict.DocumentType.Get(ReadString(reader["Q_PASP"]));

            result.SetDocument(ReadString(reader["SN_PASP"]));

            result.BirthPlace = ReadString(reader["BP"]);

            result.Address = ReadString(reader["ADRES"]);
            result.SocialPosition = ReadString(reader["SP"]);
            result.SocialFavour = ReadString(reader["KT"]);
            result.ResidenceOkato = DEFAULT_OKATO;

            result.Representative = GetRepresentative(reader);

            return result;
        }

        Representative GetRepresentative(System.Data.Common.DbDataReader reader) {
            string family = ReadString(reader["FAMP"]);
            string name = ReadString(reader["IMP"]);
            string patronymic = ReadString(reader["OTP"]);

            if (string.IsNullOrEmpty(family)
                && string.IsNullOrEmpty(name)
                && string.IsNullOrEmpty(patronymic)) return null;

            Representative result = new Representative();

            result.Family = family;
            result.Name = name;
            result.Patronymic = patronymic;

            return result;
        }
    }
}
