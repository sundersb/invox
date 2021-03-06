﻿using System;
using System.Collections.Generic;
using System.Data.Common;

namespace invox.Data.Relax {
    /// <summary>
    /// Helper to extract a Model from SQL data reader or Models' collection from dataset
    /// </summary>
    /// <typeparam name="T">Model subclass</typeparam>
    abstract class AdapterBase<T> {
        /// <summary>
        /// Read a Model from current dataset record
        /// </summary>
        /// <param name="reader">Data reader with Models' dataset</param>
        /// <param name="number">Record's number in the dataset</param>
        /// <returns>Model instance</returns>
        public abstract T Read(DbDataReader reader, int number);

        /// <summary>
        /// Get collection of Models provided by dataset from an SQL command
        /// </summary>
        /// <param name="command">SQL command with select statement</param>
        /// <returns>Models' collection</returns>
        public IEnumerable<T> Load(DbCommand command) {
            if (command.Connection.State == System.Data.ConnectionState.Closed)
                command.Connection.Open();

            DbDataReader r = null;
            try {
                r = command.ExecuteReader();
            } catch (Exception ex) {
                Lib.Logger.Log(ex.Message + "\r\n" + Pool.DescribeCommand(command));
                if (r != null) r.Close();
                command.Connection.Close();
                yield break;
            }

            int i = 0;
            while (r.Read()) {
                ++i;
                T record = Read(r, i);
                if (record != null)
                    yield return record;
            }
            r.Dispose();

            command.Connection.Close();
        }

        /// <summary>
        /// Get collection of Models by an SQL command composed on the fly
        /// </summary>
        /// <param name="connection">SQL connection to use</param>
        /// <param name="sql">Text of the SQL query</param>
        public IEnumerable<T> Load(DbConnection connection, string sql) {
            DbCommand command = connection.CreateCommand();
            command.CommandText = sql;

            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            try {

                DbDataReader r = null;
                try {
                    r = command.ExecuteReader();
                } catch (Exception ex) {
                    Lib.Logger.Log(ex.Message + "\r\n" + Pool.DescribeCommand(command));
                    if (r != null) r.Close();
                    yield break;
                }

                try {
                    int i = 0;
                    while (r.Read()) {
                        ++i;
                        T record = Read(r, i);
                        if (record != null)
                            yield return record;
                    }
                } finally {
                    r.Dispose();
                }
            } finally {
                connection.Close();
            }
        }

        protected int ReadInt(object value) {
            if (value == DBNull.Value) return 0;
            Type t = value.GetType();

            int result = 0;

            if (t == typeof(int)) return (int) value;
            if (t == typeof(decimal)) return (int)(decimal)value;
            if (t == typeof(string)) int.TryParse((string)value, out result);

            return result;
        }

        protected string ReadString(object value) {
            if (value == DBNull.Value) return string.Empty;

            string result = (string)value;
            return result.Trim();
        }

        protected DateTime ReadDate(object value) {
            if (value != DBNull.Value)
                return (DateTime)value;
            else
                return new DateTime(1900, 1, 1);
        }

        protected bool ReadBool(object value) {
            if (value == DBNull.Value) return false;

            if (value.GetType() == typeof(bool)) return (bool)value;
            if (value.GetType() == typeof(decimal)) return 0 != (decimal)value;
            if (value.GetType() == typeof(int)) return 0 != (int)value;
            if (value.GetType() == typeof(string)) {
                if (string.IsNullOrEmpty((string)value)) return false;
                int i;
                if (int.TryParse((string)value, out i)) return i != 0;
            }

            return false;
        }
    }
}
