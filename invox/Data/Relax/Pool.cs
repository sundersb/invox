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
        static string[] SELECT_SERVICES_TREATMENT = { "PERSON_RECID", "UNIT", "DS" };
        static string[] SELECT_SERVICES_BY_DATE = { "PERSON_RECID", "UNIT", "DS", "DU" };
        
        const string PERIOD_MARKER = "{period}";
        const string LPU_MARKER = "{lpu}";
        const string APPENDIX_SECTION_MARKER = "{section}";
        
        const string D1_DEPARTMENTS = "'0001', '0003', '0004', '0005'";
        const string D2_DEPARTMENTS = "'8000'";
        const string D3_DEPARTMENTS = "'0000', '0009', '0008'";

        string period;
        string lpuCode;
        Model.OrderSection lastRecoursesSection;

        OleDbConnection connectionMain;
        OleDbConnection connectionAlt;

        OleDbCommand selectRecourses = null;
        OleDbCommand selectServicesTreatment;
        OleDbCommand selectServicesByDate;

        AdapterStrings aStrings;
        AdapterPerson aPerson;
        AdapterInvoice aInvoice;
        AdapterRecourseAux aRecourse;
        AdapterServiceAux aService;

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
            aRecourse = new AdapterRecourseAux();
            aService = new AdapterServiceAux();

            string cs = string.Format(CONNECTION_STRING, location);
            connectionMain = new OleDbConnection(cs);
            connectionAlt = new OleDbConnection(cs);

            selectServicesTreatment = connectionAlt.CreateCommand();
            selectServicesTreatment.CommandText = Queries.SELECT_SERVICES_TREATMENT;
            AddStringParameters(selectServicesTreatment, SELECT_SERVICES_TREATMENT);

            selectServicesByDate = connectionAlt.CreateCommand();
            selectServicesByDate.CommandText = Queries.SELECT_SERVICES_OTHER;
            AddStringParameters(selectServicesByDate, SELECT_SERVICES_BY_DATE);
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

        public IEnumerable<Model.OncologyDirection> LoadOncologyDirections() {
            throw new NotImplementedException();
        }

        public Model.OncologyService GetOncologyService() {
            throw new NotImplementedException();
        }

        IEnumerable<ServiceAux> LoadServices(RecourseAux ra, Model.Event evt) {
            DbCommand command = null;

            switch (ra.InternalReason) {
                case InternalReason.AmbTreatment:
                case InternalReason.DayHosp:
                case InternalReason.SurgeryDayHosp:
                    command = selectServicesTreatment;
                    break;

                case InternalReason.Stage1:
                case InternalReason.Stage2:
                case InternalReason.StrippedStage1:
                case InternalReason.StrippedStage2:
                    command = selectServicesTreatment;
                    break;

                case InternalReason.Other:
                case InternalReason.DispRegister:
                case InternalReason.Emergency:
                case InternalReason.Prof:
                case InternalReason.Fluorography:
                    command = selectServicesByDate;
                    command.Parameters[3].Value = ra.Date;
                    break;
            }

            if (command != null) {
                command.Parameters[0].Value = ra.PersonId;
                command.Parameters[1].Value = ra.Unit;
                command.Parameters[2].Value = ra.MainDiagnosis;
                return aService.Load(command);
            } else {
                return null;
            }
        }

        List<Model.Event> LoadEvents(Model.Recourse rec, RecourseAux ra) {
            List<Model.Event> result = new List<Model.Event>();
            Model.Event evt = ra.ToEvent();
            result.Add(evt);

            List<ServiceAux> ss = LoadServices(ra, evt).ToList();
            evt.Services = ss.Select(s => s.ToService(ra)).ToList();

            //foreach (Service s in pool.LoadServices()) {
            //    s.Oncology = evt.isOncology;
            //}
            
            // Fill event's empty fields from services list:
            if (rec.IsHospitalization) {
                ServiceAux serv = ss.Where(s => s.Transfer == Model.Transfer.Transferred).FirstOrDefault();
                
                // Turn transfer to ProfileShift if there has been bed profile change:
                if (serv != null
                    && ss.GroupBy(s => s.BedProfile).Count() > 1) {
                    
                    serv.Transfer = Model.Transfer.ProfileShift;
                }

                evt.BedDays = ss.Select(s => s.BedDays).Sum();
            }

            // Event dates
            if (ra.InternalReason == InternalReason.Stage1) {
                ServiceAux sa = ss.Where(s => s.IsAntropometry()).FirstOrDefault();
                if (sa != null)
                    evt.DateFrom = sa.Date;
                else
                    evt.DateFrom = ss.Min(s => s.Date);
            } else {
                evt.DateFrom = ss.Min(s => s.Date);
            }
            evt.DateTill = ss.Max(s => s.Date);

            evt.Tariff = evt.Services.Sum(s => s.Tariff);
            evt.Total = evt.Services.Sum(s => s.Total);

            evt.PrimaryDiagnosis = ss.Max(s => s.PrimaryDiagnosis);
            evt.FirstIdentified = ss.Any(s => s.FirstIdentified);
            evt.ConcurrentDiagnosis = ss.Max(s => s.ConcurrentDiagnosis);
            evt.ComplicationDiagnosis = ss.Max(s => s.ComplicationDiagnosis);
            evt.DispensarySupervision = ss.Max(s => s.DispensarySupervision);
            evt.ConcurrentMesCode = ss.Max(s => s.ConcurrentMesCode);
            evt.Transfer = ss.Where(s => s.Transfer != Model.Transfer.None).Min(s=>s.Transfer);

            // Fill empty recourse fields:
            rec.UnitShift = ss.Any(s => s.Transfer == Model.Transfer.ProfileShift);
            rec.BirthWeight = ss.Max(s => s.BirthWeight);

            rec.DateFrom = evt.DateFrom;
            rec.DateTill = evt.DateTill;
            rec.Total = evt.Total;
            rec.BedDays = evt.BedDays;

            return result;
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
            
            List<RecourseAux> rs = aRecourse.Load(selectRecourses).ToList();

            foreach (RecourseAux ra in rs) {
                Model.Recourse rec = ra.ToRecourse();
                rec.Events = LoadEvents(rec, ra);
                yield return rec;
            }
        }
    }
}
