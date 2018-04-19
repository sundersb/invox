namespace civox.Data.Relax {
    class AdapterInvoice : AdapterBase<Model.InvoiceRecord> {
        public override Model.InvoiceRecord Read(System.Data.Common.DbDataReader reader, int number) {
            Model.InvoiceRecord record = new Model.InvoiceRecord(number) {
                IsNewborn = false,
                IsUpdated = false,
                PersonId = ReadInt(reader["RECID"]),
                PolicyKind = (int)(decimal)reader["T_POL"],
                Policy = ReadString(reader["SN_POL"])
            };
            string smo = ReadString(reader["Q"]);
            record.SmoCode = Dict.LocalSMO.FromLocal(smo);

            return record;
        }
    }
}
