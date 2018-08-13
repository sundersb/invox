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
        int GetRecourcesCount();

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
        IEnumerable<Model.Service> LoadServices(string policy, string diagnosis, Model.Reason reason);

        /// <summary>
        /// Load directions on dispanserisation resume
        /// </summary>
        /// <param name="serviceId">ID of the dispanserisation resulting service</param>
        /// <returns>Collection of disp routes</returns>
        List<Model.DispDirection> LoadDispanserisationRoute(long serviceId);

        /// <summary>
        /// Get list of doctors not included to the departaments
        /// </summary>
        /// <returns>List of error messages</returns>
        IEnumerable<string> LoadNoDeptDoctors();

        /// <summary>
        /// Get onkology treatment record
        /// </summary>
        /// <param name="serviceId">RECID of the service</param>
        Model.OnkologyTreat GetOnkologyTreat(long serviceId);

        /// <summary>
        /// Load all person's diagnoses in the period
        /// </summary>
        /// <param name="policy">Person policy number</param>
        List<string> GetPersonDiagnoses(string policy);

        /// <summary>
        /// Get direction information on onkology suspicion
        /// </summary>
        Model.OnkologyDirection GetOnkologyDirection(long serviceId, System.DateTime directionDate);
    }
}
