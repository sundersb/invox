namespace civox.Data {
    /// <summary>
    /// Data provider for invoice to export
    /// </summary>
    interface IDataProvider {
        /// <summary>
        /// Get invoice repository
        /// </summary>
        /// <returns>Invoice repository</returns>
        IInvoice GetInvoiceRepository();
    }
}
