using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace invox.Data.SQL {
    class Medialog {
        const string CONNECTION_STRING = "Server={0};Database={1};User Id={2};Password={3};";
        const string CONNECTION_STRING_TRUSTED = "Server={0};Database={1};Trusted_Connection=True;";

        long userId;
        protected bool authorized;
        SqlConnection connection = null;
        SqlCommand selectPassBySnils;
        SqlCommand selectPassByName;

        public SqlConnection Connection { get { return connection; } }

        public Medialog() {
            authorized = false;
            connection = new SqlConnection();

            selectPassBySnils = new SqlCommand(Queries.QUERY_SELECT_PASSPORT_BY_SNILS, connection);
            selectPassBySnils.Parameters.AddWithValue("snils", string.Empty);

            selectPassByName = new SqlCommand(Queries.QUERY_SELECT_PASSPORT_BY_NAME, connection);
            selectPassByName.Parameters.AddWithValue("family", string.Empty);
            selectPassByName.Parameters.AddWithValue("name", string.Empty);
            selectPassByName.Parameters.AddWithValue("patr", string.Empty);
            selectPassByName.Parameters.AddWithValue("bdate", DateTime.Today);
        }

        public bool Authorize(string server, string database, string user, string password) {
            if (authorized) return true;

            try {
                connection.ConnectionString = string.Format(CONNECTION_STRING,
                    server,
                    database,
                    user,
                    password);
                userId = GetUserId(Options.MedialogUser);
            } catch (Exception ex) {
                Lib.Logger.Log(ex.Message);
            }
            authorized = userId != 0;
            return authorized;
        }

        public bool Authorize() {
            if (authorized) return true;
            return Authorize(Options.MedialogServer,
                    Options.MedialogDatabase,
                    Options.MedialogUser,
                    Options.MedialogPassword);

            //try {
            //    connection.ConnectionString = string.Format(CONNECTION_STRING,
            //        Options.MedialogServer,
            //        Options.MedialogDatabase,
            //        Options.MedialogUser,
            //        Options.MedialogPassword);
            //    userId = GetUserId(Options.MedialogUser);
            //} catch (Exception ex) {
            //    Lib.Logger.Log(ex.Message);
            //}
            //authorized = userId != 0;
            //return authorized;
        }

        long GetUserId(string userName) {
            object result = DBNull.Value;
            connection.Open();
            try {
                using (SqlCommand command = connection.CreateCommand()) {
                    command.CommandText = Queries.QUERY_SELECT_USER_ID;
                    command.Parameters.AddWithValue("name", userName);
                    result = command.ExecuteScalar();
                }
                return result != DBNull.Value ? (long)result : 0L;
            } finally {
                connection.Close();
            }
        }

        public IEnumerable<T> GetRecords<T>(SqlCommand command, Func<SqlDataReader, T> onRecord) {
            command.Connection.Open();
            try {
                SqlDataReader r = null;
                try {
                    r = command.ExecuteReader();
                } catch (Exception ex) {
                    Lib.Logger.Log(ex.Message + "\n" + command.CommandText);
                    if (r != null) r.Close();
                    yield break;
                }

                try {
                    while (r.Read()) {
                        T record = onRecord(r);
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

        public bool Execute(SqlCommand command, Action<SqlDataReader> onRecord) {
            command.Connection.Open();
            bool result = false;
            try {
                SqlDataReader r = null;
                try {
                    r = command.ExecuteReader();
                } catch (Exception ex) {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(ex.Message);
                    sb.AppendLine(command.CommandText);
                    foreach (SqlParameter param in command.Parameters)
                        sb.AppendLine(string.Format("\t{0}: {1}", param.ParameterName, param.Value));

                    Lib.Logger.Log(sb.ToString());
                    if (r != null) r.Close();
                    return result;
                }

                try {
                    while (r.Read()) {
                        onRecord(r);
                        result = true;
                    }
                } finally {
                    r.Dispose();
                }
            } finally {
                command.Connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Обновить дату выдачи документа и выдавший орган из Медиалога
        /// </summary>
        /// <param name="person">Пациент</param>
        /// <returns>True, если данные обновлены</returns>
        public bool UpdatePersonDocument(Model.Person person) {
            Action<SqlDataReader> onPassRecord = reader => {
                object d = reader[0];
                if (d != DBNull.Value)
                    person.DocumentDate = (DateTime)d;

                d = reader[1];
                if (d != DBNull.Value)
                    person.DocumentOrganization = (string)d;

                d = reader[2];
                if (d != DBNull.Value)
                    Lib.PassportChecker.UpdatePassport(person, string.Empty, (string)d);
            };

            SqlCommand command = null;
            if (Lib.SnilsChecker.Valid(person.Snils)) {
                command = selectPassBySnils;
                selectPassBySnils.Parameters["snils"].Value = person.Snils;
                //if (Execute(selectPassBySnils, onPassRecord)
                //    && person.DocumentDate.HasValue)
                //    return true;
            } else {
                command = selectPassByName;
                selectPassByName.Parameters["family"].Value = person.Family.CoalesceYo();
                selectPassByName.Parameters["name"].Value = person.Name.CoalesceYo();
                selectPassByName.Parameters["patr"].Value = person.Patronymic.CoalesceYo();
                selectPassByName.Parameters["bdate"].Value = person.BirthDate;
            }

            return Execute(command, onPassRecord)
                && person.DocumentDate.HasValue;
        }
    }
}
