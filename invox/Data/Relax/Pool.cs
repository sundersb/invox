﻿using System;
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
        static string[] SELECT_CONCOM_DISEASES = { "POLICY", "UNIT", "DIAGNOSIS" };
        static string[] SELECT_DISP_ASSIGNMENTS = { "EVENT_RECID" };
        
        const string PERIOD_MARKER = "{period}";
        const string LPU_MARKER = "{lpu}";
        const string APPENDIX_SECTION_MARKER = "{section}";
        const string RECOURSE_SERVICES_MARKER = "{recsvc}";

        // Чтобы избежать ошибок, связанных с забывчивостью при добавлении чего-нибудь в ответ на нововведения
        // ФОМС условие SQL для выборки услуг, обозначающих закрытый случай, вынесено из Queries
        // и помещено здесь
        const string RECOURSE_SERVICES_CRITERION = "floor(S.COD/1000) in (3, 27, 28, 29, 22, 50, 5)";

        
        // Условия для выборки услуг в рамках диспансеризации. Необходимость связана с тем, что
        // и 1, и 2 этап проходят в одном подразделении, а код МКБ может быть одинаковым, т.о.
        // в законченный случай 1 этапа могут войти все услуги и из 2-го (и наоборот):

        // 1) BE 98 однозначно определяет 1 этап "раз в два года"
        const string STAGE1_CRITERION = " and ((floor(S.COD/1000) in (22, 24, 29)) or (S.BE = '98'))";

        // 2) Достаточно для того, чтобы отличить 2 этап от прочих посещений (при условии, что кабинет
        // уже есть в условии запроса)
        const string STAGE2_CRITERION = " and ((S.COD in (50020, 50022)) or (floor(S.COD/1000) in (25, 28)))";

        // Добавить условие к выборке услуг законченного случая - по дате
        const string SERVICES_BY_DATE_CRITERION = " and (cast (S.D_U as varchar(10)) = ?)";

        /// <summary>
        /// Коды лечебных отделений
        /// </summary>
        /// <remarks>
        /// Подразделения: лечебная, СДП, иная цель, неотложка; не онкология; не ДД раз в 2 года
        /// </remarks>
        const string D1_SELECTION = "(S.OTD in ('0001', '0003', '0004', '0005')) and (S.DS <> 'Z03.1') and (left (S.DS, 1) <> 'C') and (S.BE <> '98')";

        /// <summary>
        /// Коды отделений, оказывающих услуги по ВМП
        /// </summary>
        const string D2_SELECTION = "S.OTD = '8000'";

        /// <summary>
        /// Коды отделений профилактики и диспансеризации
        /// </summary>
        const string D3_SELECTION = "(S.OTD in ('0000', '0009', '0008')) or (S.OTD = '0004' and S.BE = '98')";

        /// <summary>
        /// Выборка онкологии
        /// </summary>
        const string D4_SELECTION = "(S.DS = 'Z03.1') or (left (S.DS, 1) = 'C')";


        string period;
        string lpuCode;
        Model.OrderSection lastRecoursesSection;

        OleDbConnection connectionMain;
        OleDbConnection connectionAlt;

        OleDbCommand selectRecourses = null;
        OleDbCommand selectServicesTreatment;
        OleDbCommand selectServicesByDate;
        OleDbCommand selectServicesStage1;
        OleDbCommand selectServicesStage2;
        OleDbCommand selectConcomDiseases;
        OleDbCommand selectDispDirections;

        AdapterStrings aStrings;
        AdapterPerson aPerson;
        AdapterInvoice aInvoice;
        AdapterRecourseAux aRecourse;
        AdapterServiceAux aService;
        AdapterConcomitantDisease aConcomitantDisease;

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
            aConcomitantDisease = new AdapterConcomitantDisease();

            string cs = string.Format(CONNECTION_STRING, location);
            connectionMain = new OleDbConnection(cs);
            connectionAlt = new OleDbConnection(cs);

            selectServicesTreatment = connectionAlt.CreateCommand();
            selectServicesTreatment.CommandText = LocalizeQuery(Queries.SELECT_SERVICES);
            AddStringParameters(selectServicesTreatment, SELECT_SERVICES_TREATMENT);

            selectServicesByDate = connectionAlt.CreateCommand();
            selectServicesByDate.CommandText = LocalizeQuery(Queries.SELECT_SERVICES
                + SERVICES_BY_DATE_CRITERION);
            AddStringParameters(selectServicesByDate, SELECT_SERVICES_BY_DATE);

            selectServicesStage1 = connectionAlt.CreateCommand();
            selectServicesStage1.CommandText = LocalizeQuery(Queries.SELECT_SERVICES
                + STAGE1_CRITERION);
            AddStringParameters(selectServicesStage1, SELECT_SERVICES_TREATMENT);

            selectServicesStage2 = connectionAlt.CreateCommand();
            selectServicesStage2.CommandText = LocalizeQuery(Queries.SELECT_SERVICES
                + STAGE2_CRITERION);
            AddStringParameters(selectServicesStage2, SELECT_SERVICES_TREATMENT);

            selectConcomDiseases = connectionAlt.CreateCommand();
            selectConcomDiseases.CommandText = LocalizeQuery(Queries.SELECT_CONCOMITANT_DISEASES);
            AddStringParameters(selectConcomDiseases, SELECT_CONCOM_DISEASES);

            selectDispDirections = connectionAlt.CreateCommand();
            selectDispDirections.CommandText = LocalizeQuery(Queries.SELECT_DISP_ASSIGNMENTS);
            AddStringParameters(selectDispDirections, SELECT_DISP_ASSIGNMENTS);
        }

        public bool Init() {
            OleDbCommand selectSpecs = connectionMain.CreateCommand();
            selectSpecs.CommandText = Queries.SELECT_DOCTORS_SPECIALITY;

            ExecuteReader(selectSpecs, reader => {
                if (reader["OTD_TN1"] == DBNull.Value) return;
                if (reader["CODEFSS"] == DBNull.Value) return;
                string key = (string)reader["OTD_TN1"];
                string value = (string)reader["CODEFSS"];
                SpecialityDict.Append(key, value.Trim());
            });

            return true;
        }



        #region Helpers
        // ***********************************************************************************

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

        /// <summary>
        /// Подставить в запрос коды отделений в соответствие разделу приложения Д к приказу
        /// </summary>
        string SectionizeQuery(string sql, Model.OrderSection section) {
            switch (section) {
                case Model.OrderSection.D1:
                    return sql.Replace(APPENDIX_SECTION_MARKER, D1_SELECTION);

                case Model.OrderSection.D2:
                    return sql.Replace(APPENDIX_SECTION_MARKER, D2_SELECTION);

                case Model.OrderSection.D3:
                    return sql.Replace(APPENDIX_SECTION_MARKER, D3_SELECTION);

                case Model.OrderSection.D4:
                    return sql.Replace(APPENDIX_SECTION_MARKER, D4_SELECTION);
            }
            return sql;
        }

        /// <summary>
        /// Подставить в запрос условие выборки услуг, обозначающих законченный случай
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string RecoursizeQuery(string sql) {
            return sql.Replace(RECOURSE_SERVICES_MARKER, RECOURSE_SERVICES_CRITERION);
        }
        // ***********************************************************************************
        #endregion



        public IEnumerable<Model.OnkologyDiagnosticType> LoadOnkologicalDiagnosticTypes() {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.OnkologyRefusal> LoadOnkologicalRefusal() {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.ComplexityQuotient> LoadComplexityQuotients() {
            throw new NotImplementedException();
        }

        public IEnumerable<string> LoadComplicationDiagnoses() {
            throw new NotImplementedException();
        }

        public IEnumerable<string> LoadMesCodes(Model.InvoiceRecord irec, Model.Recourse rec, Model.Event evt) {
            // No sanctions yet - we're only presenting bills
            yield break;
        }

        public IEnumerable<string> LoadPersonDiagnoses() {
            throw new NotImplementedException();
        }

        public Model.OnkologyTreat GetOnkologyTreat() {
            throw new NotImplementedException();
        }

        public IEnumerable<Model.Sanction> LoadSanctions(Model.InvoiceRecord irec, Model.Recourse rec) {
            yield break;
        }

        public IEnumerable<Model.OncologyDirection> LoadOncologyDirections() {
            throw new NotImplementedException();
        }

        public Model.OncologyService GetOncologyService() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получить услуги для события
        /// </summary>
        IEnumerable<ServiceAux> LoadServices(RecourseAux ra, Model.Event evt) {
            DbCommand command = null;

            switch (ra.InternalReason) {
                case InternalReason.AmbTreatment:
                case InternalReason.DayHosp:
                case InternalReason.SurgeryDayHosp:
                    command = selectServicesTreatment;
                    break;

                case InternalReason.Stage1:
                case InternalReason.StrippedStage1:
                    command = selectServicesStage1;
                    break;

                case InternalReason.Stage2:
                case InternalReason.StrippedStage2:
                    command = selectServicesStage2;
                    break;

                case InternalReason.Other:
                case InternalReason.DispRegister:
                case InternalReason.Emergency:
                case InternalReason.Prof:
                case InternalReason.Fluorography:
                    command = selectServicesByDate;
                    string d = ra.Date.ToString("MM/dd/yyyy").Replace('.', '/');
                    command.Parameters[3].Value = d;
                    break;
            }

            if (command != null) {
                command.Parameters[0].Value = string.Format("{0,6}", ra.PersonId); ;
                command.Parameters[1].Value = ra.Department;
                command.Parameters[2].Value = ra.MainDiagnosis;
                return aService.Load(command);
            } else {
                return null;
            }
        }

        /// <summary>
        /// Загрузить события для законченного случая
        /// </summary>
        /// <remarks>
        /// В настоящий момент одному законченному случаю соответствует одно события (т.к. нет
        /// внутрибольничных переводов). Здесь также загружаются услуги для события и обновляются
        /// поля законченного случая данными, которые удалось получить на этом этапе.
        /// </remarks>
        List<Model.Event> LoadEvents(Model.Recourse rec, RecourseAux ra) {
            // Extract single event from the RecourseAux:
            List<Model.Event> result = new List<Model.Event>();
            Model.Event evt = ra.ToEvent(rec);
            result.Add(evt);

            // Load auxilliary service records and service models
            List<ServiceAux> ss = LoadServices(ra, evt).ToList();
            evt.Services = ss.Select(s => s.ToService(ra)).ToList();

            if (rec.IsHospitalization) {
                // Update profile-shifting transfer and total of the bed days
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
                // For DD1 - start of the recourse is an antropometry
                ServiceAux sa = ss.Where(s => s.IsAntropometry()).FirstOrDefault();
                if (sa != null)
                    evt.DateFrom = sa.Date;
                else
                    evt.DateFrom = ss.Min(s => s.Date);
            } else {
                // For other cases service with minimal date (mayhap hospitalization - reason why not ss.Min())
                evt.DateFrom = evt.Services.Min(s => s.DateFrom);
            }
            evt.DateTill = ss.Max(s => s.Date);

            // Statistic code
            if (!string.IsNullOrEmpty(ra.MainDiagnosis) && ra.MainDiagnosis.First() != 'Z') {
                StatisticCode[] sc = ss
                    .Select(s => s.StatisticCode)
                    .Where(s => s > StatisticCode.None && s < StatisticCode.Dispensary)
                    .ToArray();

                if (sc != null && sc.Count() > 0)
                    evt.StatisticsCode = (((int)sc.Max()) - 1).ToString();
                else
                    evt.StatisticsCode = Dict.StatisticsCode.Instance.Get(ra.MainDiagnosis);
            }

            // Other Event fields which can be taken from the services
            evt.Tariff = evt.Services.Sum(s => s.Tariff);
            evt.Total = evt.Services.Sum(s => s.Total);

            evt.PrimaryDiagnosis = ss.Max(s => s.PrimaryDiagnosis);
            evt.FirstIdentified = ss.Any(s => s.FirstIdentified);

            evt.ConcurrentDiagnoses = ServiceAux.GetConcurrentDiagnoses(ss);
            evt.ComplicationDiagnoses = ServiceAux.GetComplicationDiagnoses(ss);

            if (ra.InternalReason == InternalReason.DispRegister) {
                evt.DispensarySupervision = ss.Max(s => s.DispensarySupervision);
                if (evt.DispensarySupervision == Model.DispensarySupervision.None)
                    evt.DispensarySupervision = Model.DispensarySupervision.Observed;
            }

            evt.ConcurrentMesCode = ss.Max(s => s.ConcurrentMesCode);
            
            var d2 = ss.Where(s => s.Transfer != Model.Transfer.None);
            if (d2 != null && d2.Count() > 0)
                evt.Transfer = d2.Min(s => s.Transfer);
            else
                evt.Transfer = Model.Transfer.None;

            // Other Recourse fields -"-
            rec.UnitShift = ss.Any(s => s.Transfer == Model.Transfer.ProfileShift);
            rec.BirthWeight = ss.Max(s => s.BirthWeight);

            rec.DateFrom = evt.DateFrom;
            rec.DateTill = evt.DateTill;
            rec.Total = evt.Total;
            rec.BedDays = evt.BedDays;

            if (RecourseAux.NeedsDirection(rec)) {
                // Extract directed-from in case when it is needed
                rec.DirectedFrom = ss.Max(s => s.DirectedFrom);
                if (string.IsNullOrEmpty(rec.DirectedFrom)) {
                    // TODO: LPU code from local to federal
                    rec.DirectedFrom = Options.LpuCode;
                }
                // TODO: Direction date
            }

            return result;
        }

        public IEnumerable<Model.ConcomitantDisease> GetConcomitantDiseases(Model.InvoiceRecord irec, Model.Event evt) {
            selectConcomDiseases.Parameters[0].Value = irec.Person.Policy;
            selectConcomDiseases.Parameters[1].Value = evt.Unit;
            selectConcomDiseases.Parameters[2].Value = evt.MainDiagnosis;
            return aConcomitantDisease.Load(selectConcomDiseases);
        }

        public IEnumerable<Model.DispAssignment> GetDispanserisationAssignments(Model.Event evt) {
            string id = string.Format("{0,6}", evt.Identity);
            selectDispDirections.Parameters[0].Value = id;
            List<Model.DispAssignment> result = null;

            Action<System.Data.Common.DbDataReader> onRead = r => {
                string codes = (string)r["KSG"];
                string values = (string)r["KSG2"];
                result = Model.DispAssignment.Make(codes.Trim(), values.Trim()).ToList();
            };

            ExecuteReader(selectDispDirections, onRead);
            return result;
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
            sql = RecoursizeQuery(sql);
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
            sql = RecoursizeQuery(sql);
            return aInvoice.Load(connectionMain, LocalizeQuery(sql));
        }

        public List<string> LoadInitErrors() {
            string sql = LocalizeQuery(Queries.SELECT_NO_DOCTOR_DEPT);
            List<string> result = aStrings.Load(connectionMain, sql).ToList();
            result.AddRange(aStrings.Load(connectionMain, Queries.SELECT_NO_SPECIALITY));
            return result;
        }
    

        public IEnumerable<Model.Recourse>  LoadRecourses(Model.InvoiceRecord irec, Model.OrderSection section) {
            // Query is cached 
            if (section != lastRecoursesSection) {
                selectRecourses = null;
                lastRecoursesSection = section;
            }

            if (selectRecourses == null) {
                string sql = LocalizeQuery(Queries.SELECT_RECOURSES);
                sql = RecoursizeQuery(sql);
                selectRecourses = connectionAlt.CreateCommand();
                selectRecourses.CommandText = SectionizeQuery(sql, section);
                AddStringParameters(selectRecourses, SELECT_RECOURSE_CASES_PARAMS);
            }
            string id = string.Format("{0,6}", irec.Person.Identity);
            selectRecourses.Parameters[0].Value = id;
            
            List<RecourseAux> rs = aRecourse.Load(selectRecourses).ToList();

            foreach (RecourseAux ra in rs) {
                Model.Recourse rec = ra.ToRecourse();
                rec.Events = LoadEvents(rec, ra);
                yield return rec;
            }
        }
    }
}
