using System;
using System.Collections.Generic;
using System.Data.Common;

namespace civox.Data.Relax {
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
            command.Connection.Open();
            try {
                DbDataReader r = null;
                try {
                    r = command.ExecuteReader();
                } catch (Exception ex) {
                    Lib.Logger.Log(ex.Message + "\r\n" + Provider.ShowCommand(command));
                    if (r != null) r.Close();
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
                command.Connection.Close();
            }
        }

        protected int ReadInt(object value) {
            int result = 0;
            int.TryParse((string)value, out result);
            return result;
        }

        protected string ReadString(object value) {
            string result = (string)value;
            return result.Trim();
        }

        protected DateTime ReadDate(object value) {
            if (value != DBNull.Value)
                return (DateTime)value;
            else
                return new DateTime(1900, 1, 1);
        }
    }
}
