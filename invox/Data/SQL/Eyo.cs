namespace invox.Data.SQL {
    static class Eyo {
        const string LIKE_EXPRESSION = "[её]";

        public static string CoalesceYo(this string name) {
            return name.Replace("е", LIKE_EXPRESSION).Replace("ё", LIKE_EXPRESSION);
        }
    }
}
