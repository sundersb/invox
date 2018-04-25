namespace civox.Model {
    /// <summary>
    /// Код котрагента для формирования имени файла счета
    /// </summary>
    enum AgentCode {
        TerritoryFund,      // ХКФОМС
        AssuranceCompany,   // СМО
        Clinic              // ЛПУ
    }

    /// <summary>
    /// Вид счета для формирования имени файла счета
    /// </summary>
    enum InvoiceKind {
        GeneralTreatment,  // Посещения с лечебной целью
        HiTechAid,         // ВМП
        Prophylaxis        // Профилактические, диспансеризация
    }

    /// <summary>
    /// Вид счета на случаи с профцелью
    /// </summary>
    enum ProfKind {
        Stage1,            // Диспансеризация 1 этап
        Stage2,            // Диспансеризация 2 этап
        Adults,            // Профосмотры взрослых
        Orphans,           // Профосмотры детей-сирот
        Adopted,           // Профосмотры на усыновления
        Underage           // Профосмотры несовершеннолетних
    }
}
