using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data.Common;

namespace invox.Data.Relax {
    class Pool : IInvoice {
        const string CONNECTION_STRING = "Provider=vfpoledb;Data Source={0};Collating Sequence=machine;Mode=ReadWrite|Share Deny None;";
        const string PERIOD_MARKER = "{period}";
        const string LPU_MARKER = "{lpu}";

        string period;
        OleDbConnection connectionMain;

        AdapterStrings aStrings;

        public Pool(string location, string period) {
            this.period = period;
            string cs = string.Format(CONNECTION_STRING, location);
            connectionMain = new OleDbConnection(cs);
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
            command.Connection.Open();
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

        public IEnumerable<Model.Event> LoadEvents() {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.ConcomitantDisease> GetConcomitantDiseases() {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.DispAssignment> GetDispanserisationAssignmetns() {
            throw new NotImplementedException();
        }

        public int GetPeopleCount(Model.OrderSection section) {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.Person> LoadPeople(Model.OrderSection section) {
            throw new NotImplementedException();
        }

        public int GetInvoiceRecordsCount(Model.OrderSection section) {
            throw new NotImplementedException();
        }

        public float Total(Model.OrderSection section) {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.InvoiceRecord> LoadInvoiceRecords(Model.OrderSection section) {
            throw new NotImplementedException();
        }

        public IEnumerable<string> LoadNoDeptDoctors() {
            return aStrings.Load(connectionMain, "");
        }
    }
}
