﻿using System;
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
                Lib.Logger.Log(ex.Message + "\n" + command.CommandText);
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
                Lib.Logger.Log(ex.Message + "\n" + command.CommandText);
            } finally {
                command.Connection.Close();
            }
            return result;
        }
    }
}