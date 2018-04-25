using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data.Common;

namespace civox.Data.Relax {
    class Provider : IDataProvider {
        const string CONNECTION_STRING = "Provider=vfpoledb;Data Source={0};Collating Sequence=machine;Mode=ReadWrite|Share Deny None;";

        // Main connection
        OleDbConnection connectionMain;

        // Alternative connection. Use when connectionMain is still opened
        OleDbConnection connectionAlt;

        RepoInvoice repoInvoice = null;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="location">Full path to relax directory</param>
        public Provider(string location) {
            string cs = string.Format(CONNECTION_STRING, location);
            connectionMain = new OleDbConnection(cs);
            connectionAlt = new OleDbConnection(cs);
        }

        /// <summary>
        /// Create SQL command
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <returns>Command to execute</returns>
        public OleDbCommand CreateCommand(string sql) {
            OleDbCommand result = connectionMain.CreateCommand();
            result.CommandText = sql;
            return result;
        }

        /// <summary>
        /// Create SQL command by alternative connection
        /// </summary>
        /// <param name="sql">SQL query</param>
        /// <returns>Command to run</returns>
        public OleDbCommand CreateCommandAlt(string sql) {
            OleDbCommand result = connectionAlt.CreateCommand();
            result.CommandText = sql;
            return result;
        }

        /// <summary>
        /// Add varchar parameters to an SQL command
        /// </summary>
        /// <param name="command">SQL command to add parameters to</param>
        /// <param name="parameters">Parameter names</param>
        public void AddStringParameters(OleDbCommand command, string[] parameters) {
            foreach (string param in parameters)
                command.Parameters.Add(new OleDbParameter(param, OleDbType.VarChar));
        }

        /// <summary>
        /// Get SQL command query and parameter values
        /// </summary>
        /// <param name="command">Command to describe</param>
        /// <returns>Command's SQL text and parameters</returns>
        public static string ShowCommand(DbCommand command) {
            StringBuilder sb = new StringBuilder(command.CommandText);
            if (command.Parameters.Count > 0) {
                sb.Append("\r\n\r\nПараметры:");
                foreach (DbParameter p in command.Parameters) {
                    sb.Append(string.Format("\r\n\t{0} = '{1}'", p.ParameterName, p.Value));
                }
            }
            return sb.ToString();
        }

        public IInvoice GetInvoiceRepository() {
            if (repoInvoice == null) repoInvoice = new RepoInvoice(this);
            return repoInvoice;
        }

        /// <summary>
        /// Execute a non-select query
        /// </summary>
        public void ExecuteCommand(DbCommand command) {
            if (command.Connection.State != System.Data.ConnectionState.Open)
                command.Connection.Open();
            try {
                command.ExecuteNonQuery();
            } catch (Exception ex) {
                Lib.Logger.Log(ex.Message + "\r\n" + ShowCommand(command));
            } finally {
                command.Connection.Close();
            }
        }

        /// <summary>
        /// Execute SQL select command to get a single value
        /// </summary>
        public object ExecuteScalar(DbCommand command) {
            if (command.Connection.State != System.Data.ConnectionState.Open)
                command.Connection.Open();
            object result = null;

            try {
                result = command.ExecuteScalar();
            } catch (Exception ex) {
                Lib.Logger.Log(ex.Message + "\r\n" + ShowCommand(command));
            } finally {
                command.Connection.Close();
            }
            return result;
        }
    }
}
