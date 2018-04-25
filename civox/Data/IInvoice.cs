using System.Collections.Generic;

namespace civox.Data {
    /// <summary>
    /// Invoice repository interface
    /// </summary>
    interface IInvoice {
        /// <summary>
        /// Get people collection
        /// </summary>
        /// <returns>Collection of Model.Person</returns>
        IEnumerable<Model.Person> LoadPeople();

        /// <summary>
        /// Get amount of records in people dataset
        /// </summary>
        int GetPeopleCount();

        /// <summary>
        /// Get invoice records
        /// </summary>
        /// <returns>Collection of InvoiceRecord</returns>
        IEnumerable<Model.InvoiceRecord> LoadInvoiceRecords();

        /// <summary>
        /// Get quantity of treatment cases
        /// </summary>
        int GetCasesNumber();

        /// <summary>
        /// Get total price for the FOMS to pay
        /// </summary>
        decimal TotalToPay();

        /// <summary>
        /// Load recource cases
        /// </summary>
        /// <param name="policy">Person's policy serial and number</param>
        /// <returns>Collection of Model.Recourse</returns>
        IEnumerable<Model.Recourse> LoadRecourceCases(string policy);

        /// <summary>
        /// Load a treatment case
        /// </summary>
        /// <param name="policy">Person's policy number</param>
        /// <param name="diagnosis">ICD code</param>
        /// <returns>Collection of services</returns>
        IEnumerable<Model.Service> LoadServices(string policy, string diagnosis, string dept);
    }
}
