using System;

namespace civox.Data.Relax {
    class AdapterInvoice : AdapterBase<Model.InvoiceRecord> {
        public override Model.InvoiceRecord Read(System.Data.Common.DbDataReader reader, int number) {
            Model.InvoiceRecord record = new Model.InvoiceRecord(number) {
                IsNewborn = false,
                IsUpdated = false,
                PersonId = ReadInt(reader["RECID"]),
                PolicyKind = (int)(decimal)reader["T_POL"],
                Policy = ReadString(reader["SN_POL"]),
                OKATO = Options.OKATO,
                Privilege = ReadString(reader["KT"]),
                Sex = (int)(Decimal)reader["W"],
                BirthDate = ReadDate(reader["DR"])
            };
            string smo = ReadString(reader["Q"]);
            record.SmoCode = Dict.SMO.Instance.Get(smo);
            
            return record;
        }
    }
}
