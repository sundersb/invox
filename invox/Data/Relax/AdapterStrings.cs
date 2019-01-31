namespace invox.Data.Relax {
    class AdapterStrings : AdapterBase<string> {
        public override string Read(System.Data.Common.DbDataReader reader, int number) {
            return ReadString(reader[0]);
        }
    }
}
