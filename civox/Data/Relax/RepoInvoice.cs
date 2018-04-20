﻿using System;
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

        Provider provider;

        AdapterPerson aPerson;
        AdapterInvoice aInvoice;
        AdapterRecourse aRecourse;
        AdapterService aService;

        OleDbCommand selectPeople;
        OleDbCommand selectTotalToPay;
        OleDbCommand selectInvoiceRecs;
        OleDbCommand selectRecourses;
        OleDbCommand selectService;

        public RepoInvoice(Provider provider) {
            this.provider = provider;
            aPerson = new AdapterPerson();
            aInvoice = new AdapterInvoice();
            aRecourse = new AdapterRecourse();
            aService = new AdapterService();

            Func<string, OleDbCommand> helper =
                s => provider.CreateCommand(s.Replace(PERIOD_MARKER, Options.PeriodLocation));

            Func<string, OleDbCommand> helperAlt =
                s => provider.CreateCommandAlt(s.Replace(PERIOD_MARKER, Options.PeriodLocation));

            selectPeople = helper(Queries.SELECT_PEOPLE);
            selectTotalToPay = helper(Queries.SELECT_TOTAL_TO_PAY);
            selectInvoiceRecs = helper(Queries.SELECT_INVOICE_RECS);

            selectRecourses = helperAlt(Queries.SELECT_RECOURSE_CASES);
            selectRecourses.Parameters.Add(new OleDbParameter("?", OleDbType.VarChar));

            selectService = helperAlt(Queries.SELECT_CASE_TREAT);
            selectService.Parameters.Add(new OleDbParameter("?", OleDbType.VarChar));
            selectService.Parameters.Add(new OleDbParameter("?", OleDbType.VarChar));
        }


        #region IInvoice realization
        // *************************

        public IEnumerable<Model.Person> LoadPeople() {
            return aPerson.Load(selectPeople);
        }

        // TODO: int GetCasesNumber()
        public int GetCasesNumber() {
            throw new NotImplementedException();
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

        public IEnumerable<Model.Service> LoadServices(string policy, string diagnosis) {
            selectService.Parameters[0].Value = policy;
            selectService.Parameters[1].Value = diagnosis;
            return aService.Load(selectService);
        }

        // **************************
        #endregion
    }
}