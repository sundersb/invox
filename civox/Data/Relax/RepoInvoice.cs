using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace civox.Data.Relax {
    /// <summary>
    /// Relax repository for invoice
    /// </summary>
    class RepoInvoice : IInvoice {
        const string PERIOD_MARKER = "{period}";
        const string LPU_MARKER = "{lpu}";

        static string[] SELECT_RECOURSE_CASES_PARAMS = { "SN_POL" };
        static string[] SELECT_CASE_TREAT_PARAMS = { "SN_POL", "DS", "REASON" };
        static string[] SELECT_DISP_DIRECTIONS_PARAMS = { "RECID" };

        Provider provider;

        AdapterPerson aPerson;
        AdapterInvoice aInvoice;
        AdapterRecourse aRecourse;
        AdapterService aService;

        OleDbCommand selectPeople;
        OleDbCommand selectPeopleCount;
        OleDbCommand selectTotalToPay;
        OleDbCommand selectInvoiceRecs;
        OleDbCommand selectRecourses;
        OleDbCommand selectRecoursesCount;
        OleDbCommand selectService;
        OleDbCommand selectDispDirections;

        public RepoInvoice(Provider provider) {
            this.provider = provider;
            aPerson = new AdapterPerson();
            aInvoice = new AdapterInvoice();
            aRecourse = new AdapterRecourse();
            aService = new AdapterService();

            Func<string, OleDbCommand> helper =
                s => provider.CreateCommand(LocalizeQuery(s));

            Func<string, OleDbCommand> helperAlt =
                s => provider.CreateCommandAlt(LocalizeQuery(s));

            selectPeople = helper(Queries.SELECT_PEOPLE);
            selectPeopleCount = helper(Queries.SELECT_PEOPLE_COUNT);
            selectTotalToPay = helper(Queries.SELECT_TOTAL_TO_PAY);
            selectInvoiceRecs = helper(Queries.SELECT_INVOICE_RECS);
            selectRecoursesCount = helper(Queries.SELECT_RECOURSES_COUNT);

            selectRecourses = helperAlt(Queries.SELECT_RECOURSE_CASES);
            provider.AddStringParameters(selectRecourses, SELECT_RECOURSE_CASES_PARAMS);

            selectService = helperAlt(Queries.SELECT_CASE_TREAT);
            provider.AddStringParameters(selectService, SELECT_CASE_TREAT_PARAMS);

            selectDispDirections = helperAlt(Queries.SELECT_DISP_DIRECTIONS);
            provider.AddStringParameters(selectDispDirections, SELECT_DISP_DIRECTIONS_PARAMS);
        }

        /// <summary>
        /// Extend SQL macros
        /// </summary>
        /// <param name="sql">SQL with period and clinic macros</param>
        /// <returns>SQL with macros extended</returns>
        string LocalizeQuery(string sql) {
            return sql.Replace(PERIOD_MARKER, Options.PeriodLocation)
                .Replace(LPU_MARKER, Options.LocalLpuCode);
        }


        #region IInvoice realization
        // *************************

        public IEnumerable<Model.Person> LoadPeople() {
            return aPerson.Load(selectPeople);
        }

        public int GetRecourcesCount() {
            object result = provider.ExecuteScalar(selectRecoursesCount);
            return (int)(decimal)result;
        }

        public decimal TotalToPay() {
            return (decimal) provider.ExecuteScalar(selectTotalToPay);
        }

        public IEnumerable<Model.InvoiceRecord> LoadInvoiceRecords() {
            return aInvoice.Load(selectInvoiceRecs);
        }

        public IEnumerable<Model.Recourse> LoadRecourceCases(string policy) {
            selectRecourses.Parameters[0].Value = policy;
            return aRecourse.Load(selectRecourses);
        }

        public IEnumerable<Model.Service> LoadServices(string policy, string diagnosis, Model.Reason reason) {
            selectService.Parameters[0].Value = policy;
            selectService.Parameters[1].Value = diagnosis;
            selectService.Parameters[2].Value = ((int)reason).ToString();
            return aService.Load(selectService);
        }

        public int GetPeopleCount() {
            object result = provider.ExecuteScalar(selectPeopleCount);
            return (int)(decimal)result;
        }

        public List<Model.DispDirection> LoadDispanserisationRoute(long serviceId) {
            string id = string.Format("{0,6}", serviceId);
            selectDispDirections.Parameters[0].Value = id;
            List<Model.DispDirection> result = null;

            Action<System.Data.Common.DbDataReader> onRead = r => {
                string codes = (string)r["KSG"];
                string values = (string)r["KSG2"];
                result = Model.DispDirection.Make(codes.Trim(), values.Trim()).ToList();
            };

            provider.ExecuteReader(selectDispDirections, onRead);
            return result;
        }

        // **************************
        #endregion
    }
}
