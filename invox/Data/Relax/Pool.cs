using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data.Common;

namespace invox.Data.Relax {
    class Pool : IInvoice {
        const string CONNECTION_STRING = "Provider=vfpoledb;Data Source={0};Collating Sequence=machine;Mode=ReadWrite|Share Deny None;";

        static string[] SELECT_RECOURSE_CASES_PARAMS = { "PERSON_RECID" };
        
        const string PERIOD_MARKER = "{period}";
        const string LPU_MARKER = "{lpu}";
        const string APPENDIX_SECTION_MARKER = "{section}";
        
        const string D1_DEPARTMENTS = "'0001', '0003', '0004', '0005'";
        const string D2_DEPARTMENTS = "'8000'";
        const string D3_DEPARTMENTS = "'0000', '0009'";

        string period;
        string lpuCode;
        Model.OrderSection lastRecoursesSection;

        OleDbConnection connectionMain;
        OleDbConnection connectionAlt;

        OleDbCommand selectRecourses = null;

        AdapterStrings aStrings;
        AdapterPerson aPerson;
        AdapterInvoice aInvoice;
        AdapterRecourse aRecourse;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="location">Каталог Релакс</param>
        /// <param name="lpuCode">Код ЛПУ (релаксовский)</param>
        /// <param name="period">Каталог текущего периода (от корневого каталога Релакс)</param>
        public Pool(string location, string lpuCode, string period) {
            this.period = period;
            this.lpuCode = lpuCode;

            aStrings = new AdapterStrings();
            aPerson = new AdapterPerson();
            aInvoice = new AdapterInvoice();
            aRecourse = new AdapterRecourse();

            string cs = string.Format(CONNECTION_STRING, location);
            connectionMain = new OleDbConnection(cs);
            connectionAlt = new OleDbConnection(cs);
        }

        /// <summary>
        /// Add varchar parameters to an SQL command
        /// </summary>
        /// <param name="command">SQL command to add parameters to</param>
        /// <param name="parameters">Parameter names</param>
        void AddStringParameters(OleDbCommand command, string[] parameters) {
            foreach (string param in parameters)
                command.Parameters.Add(new OleDbParameter(param, OleDbType.VarChar));
        }

        /// <summary>
        /// Get SQL command query and parameter values
        /// </summary>
        /// <param name="command">Command to describe</param>
        /// <returns>Command's SQL text and parameters</returns>
        public static string DescribeCommand(DbCommand command) {
            StringBuilder sb = new StringBuilder(command.CommandText);
            if (command.Parameters.Count > 0) {
                sb.Append("\r\n\r\nПараметры:");
                foreach (DbParameter p in command.Parameters) {
                    sb.Append(string.Format("\r\n\t{0} = '{1}'", p.ParameterName, p.Value));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Execute a non-select query
        /// </summary>
        static void ExecuteCommand(DbCommand command) {
            if (command.Connection.State != System.Data.ConnectionState.Open)
                command.Connection.Open();
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                Lib.Logger.Log(ex.Message + "\r\n" + DescribeCommand(command));
            } finally {
                command.Connection.Close();
            }
        }
        
        /// <summary>
        /// Execute SQL select command to get a single value
        /// </summary>
        static object ExecuteScalar(DbCommand command) {
            if (command.Connection.State != System.Data.ConnectionState.Open)
                command.Connection.Open();
            object result = null;

            try {
                result = command.ExecuteScalar();
            } catch (Exception ex) {
                Lib.Logger.Log(ex.Message + "\r\n" + DescribeCommand(command));
            } finally {
                command.Connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Execute SQL select command to get a single value
        /// </summary>
        static object ExecuteScalar(DbConnection connection, string sql) {
            DbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
            
            object result = null;
            try {
                result = command.ExecuteScalar();
            } catch (Exception ex) {
                Lib.Logger.Log(ex.Message + "\r\n" + DescribeCommand(command));
            } finally {
                connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Execute arbitrary select query
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <param name="onRead">Action to perform on records in the dataset</param>
        static void ExecuteReader(DbCommand command, Action<DbDataReader> onRead) {
            command.Connection.Open();
            try {
                DbDataReader r = null;
                try {
                    r = command.ExecuteReader();
                } catch (Exception ex) {
                    Lib.Logger.Log(ex.Message + "\r\n" + DescribeCommand(command));
                    if (r != null) r.Close();
                }

                try {
                    while (r.Read())
                        onRead(r);
                } finally {
                    r.Dispose();
                }
            } finally {
                command.Connection.Close();
            }
        }

        /// <summary>
        /// Execute arbitrary select query
        /// </summary>
        /// <param name="connection">Connection to use</param>
        /// <param name="sql">Command to execute</param>
        /// <param name="onRead">Action to perform on records in the dataset</param>
        static void ExecuteReader(DbConnection connection, string sql, Action<DbDataReader> onRead) {
            DbCommand command = connection.CreateCommand();
            command.CommandText = sql;

            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
            try {
                DbDataReader r = null;
                try {
                    r = command.ExecuteReader();
                } catch (Exception ex) {
                    Lib.Logger.Log(ex.Message + "\r\n" + DescribeCommand(command));
                    if (r != null) r.Close();
                }

                try {
                    while (r.Read())
                        onRead(r);
                } finally {
                    r.Dispose();
                }
            } finally {
                connection.Close();
            }
        }

        /// <summary>
        /// Extend SQL macros
        /// </summary>
        /// <param name="sql">SQL with period and clinic macros</param>
        /// <returns>SQL with macros extended</returns>
        string LocalizeQuery(string sql) {
            return sql.Replace(PERIOD_MARKER, period)
                .Replace(LPU_MARKER, lpuCode);
        }

        string SectionizeQuery(string sql, Model.OrderSection section) {
            switch (section) {
                case Model.OrderSection.D1:
                    return sql.Replace(APPENDIX_SECTION_MARKER, D1_DEPARTMENTS);

                case Model.OrderSection.D2:
                    return sql.Replace(APPENDIX_SECTION_MARKER, D2_DEPARTMENTS);

                case Model.OrderSection.D3:
                    return sql.Replace(APPENDIX_SECTION_MARKER, D3_DEPARTMENTS);
            }
            return sql;
        }

        public IEnumerable<Model.OnkologyDiagnosticType> LoadOnkologicalDiagnosticTypes() {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.OnkologyRefusal> LoadOnkologicalRefusal() {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.ComplexityQuotient> LoadComplexityQuotients() {
            throw new NotImplementedException();
        }

        public IEnumerable<string> LoadConcurrentDiagnoses() {
            throw new NotImplementedException();
        }

        public IEnumerable<string> LoadComplicationDiagnoses() {
            throw new NotImplementedException();
        }

        public IEnumerable<string> LoadMesCodes() {
            throw new NotImplementedException();
        }

        public IEnumerable<string> LoadPersonDiagnoses() {
            throw new NotImplementedException();
        }

        public Model.OnkologyTreat GetOnkologyTreat() {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.Sanction> LoadSanctions() {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.Service> LoadServices() {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.OncologyDirection> LoadOncologyDirections() {
            throw new NotImplementedException();
        }

        public Model.OncologyService GetOncologyService() {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.Event> LoadEvents(Model.InvoiceRecord irec, Model.Recourse rec) {
            yield break;
            throw new NotImplementedException();
        }

        public IEnumerable<Model.ConcomitantDisease> GetConcomitantDiseases() {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.DispAssignment> GetDispanserisationAssignmetns() {
            throw new NotImplementedException();
        }

        public int GetPeopleCount(Model.OrderSection section) {
            string sql = SectionizeQuery(Queries.SELECT_PEOPLE_COUNT, section);
            object result = ExecuteScalar(connectionMain, LocalizeQuery(sql));
            return result != DBNull.Value ? (int)(decimal)result : 0;
        }

        public IEnumerable<Model.Person> LoadPeople(Model.OrderSection section) {
            string sql = SectionizeQuery(Queries.SELECT_PEOPLE, section);
            return aPerson.Load(connectionMain, LocalizeQuery(sql));
        }

        public int GetInvoiceRecordsCount(Model.OrderSection section) {
            string sql = SectionizeQuery(Queries.SELECT_INVOICE_RECORDS_COUNT, section);
            object result = ExecuteScalar(connectionMain, LocalizeQuery(sql));
            return result != DBNull.Value ? (int)(decimal)result : 0;
        }

        public decimal Total(Model.OrderSection section) {
            string sql = SectionizeQuery(Queries.SELECT_TOTAL, section);
            object result = ExecuteScalar(connectionMain, LocalizeQuery(sql));
            return result != DBNull.Value ? (decimal)result : 0;
        }

        public IEnumerable<Model.InvoiceRecord> LoadInvoiceRecords(Model.OrderSection section) {
            string sql = SectionizeQuery(Queries.SELECT_INVOICE_PEOPLE, section);
            return aInvoice.Load(connectionMain, LocalizeQuery(sql));
        }

        public IEnumerable<string> LoadNoDeptDoctors() {
            string sql = LocalizeQuery(Queries.SELECT_NO_DOCTOR_DEPT);
            return aStrings.Load(connectionMain, sql);
        }
    

        public IEnumerable<Model.Recourse>  LoadRecourses(Model.InvoiceRecord irec, Model.OrderSection section) {
            if (section != lastRecoursesSection) {
                selectRecourses = null;
                lastRecoursesSection = section;
            }

            if (selectRecourses == null) {
                string sql = LocalizeQuery(Queries.SELECT_RECOURSES);
                selectRecourses = connectionAlt.CreateCommand();
                selectRecourses.CommandText = SectionizeQuery(sql, section);
                AddStringParameters(selectRecourses, SELECT_RECOURSE_CASES_PARAMS);
            }
            selectRecourses.Parameters[0].Value = irec.Person.Identity;
            return aRecourse.Load(selectRecourses);
        }
    }
}
