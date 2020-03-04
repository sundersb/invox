namespace invox.Data.SQL {
    static class Eyo {
        const string LIKE_EXPRESSION = "[её]";
        const string TEMP_PATTERN = "#";

        public static string CoalesceYo(this string name) {
            return name
                .Replace("е", TEMP_PATTERN)
                .Replace("ё", TEMP_PATTERN)
                .Replace(TEMP_PATTERN, LIKE_EXPRESSION);
        }
    }
}
