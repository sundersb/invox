﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data.Common;
using invox.Lib;

namespace invox.Data.Relax {
    class Pool : IInvoice {
        const string CONNECTION_STRING = "Provider=vfpoledb;Data Source={0};Collating Sequence=machine;Mode=ReadWrite|Share Deny None;";

        static string[] SELECT_RECOURSE_CASES_PARAMS = { "PERSON_RECID" };
        static string[] SELECT_SERVICES_TREATMENT = { "PERSON_RECID", "UNIT", "DS" };
        static string[] SELECT_SERVICES_BY_DATE = { "PERSON_RECID", "UNIT", "DS", "DU" };
        static string[] SELECT_CONCOM_DISEASES = { "POLICY", "UNIT", "DIAGNOSIS" };
        static string[] PARAMETER_EVENT_RECID = { "EVENT_RECID" };
        
        const string PERIOD_MARKER = "{period}";
        const string LPU_MARKER = "{lpu}";
        const string APPENDIX_SECTION_MARKER = "{section}";
        const string RECOURSE_SERVICES_MARKER = "{recsvc}";

        // Чтобы избежать ошибок, связанных с забывчивостью при добавлении чего-нибудь в ответ на нововведения
        // ФОМС условие SQL для выборки услуг, обозначающих закрытый случай, вынесено из Queries
        // и помещено здесь
        // СДП, проф, ДД2, ДД1, подушевые, ФОГК, диагностика
        const string RECOURSE_SERVICES_CRITERION = "floor(S.COD/1000) in (3, 27, 28, 22, 50, 4, 5, 6)";

        
        // Условия для выборки услуг в рамках диспансеризации. Необходимость связана с тем, что
        // и 1, и 2 этап проходят в одном подразделении, а код МКБ может быть одинаковым, т.о.
        // в законченный случай 1 этапа могут войти все услуги и из 2-го (и наоборот):

        // 1) Услуги 1 этапа диспансеризации
        //const string STAGE1_CRITERION = " and ((S.COD in (50019, 50023)) or (floor(S.COD/1000) in (22, 24, 29)))";

        // 2) Достаточно для того, чтобы отличить 2 этап от прочих посещений (при условии, что кабинет
        // уже есть в условии запроса)
        //const string STAGE2_CRITERION = " and ((S.COD in (50020, 50022)) or (floor(S.COD/1000) in (25, 28)))";

        // Добавить условие к выборке услуг законченного случая - по дате
        const string SERVICES_BY_DATE_CRITERION = " and (cast (S.D_U as varchar(10)) = ?)";

        /// <summary>
        /// Коды лечебных отделений
        /// </summary>
        /// <remarks>
        /// Подразделения: лечебная, СДП, иная цель, неотложка; не онкология; не ДД раз в 2 года
        /// </remarks>
#if FOMS
        const string D1_SELECTION = "(S.OTD in ('0001', '0003', '0004', '0005', '0008')) and (S.DS <> 'Z03.1') and (left (S.DS, 1) <> 'C') and (S.DS not like 'D0%')";
#else
        const string D1_SELECTION = "(S.OTD in ('0001', '0003', '0004', '0005')) and (S.DS <> 'Z03.1') and (left (S.DS, 1) <> 'C')";
#endif
        /// <summary>
        /// Коды отделений, оказывающих услуги по ВМП
        /// </summary>
        const string D2_SELECTION = "S.OTD = '8000'";

        /// <summary>
        /// Коды отделений профилактики и диспансеризации
        /// </summary>

        //const string D3_SELECTION_STAGE1 = "((floor(S.COD/1000) in (22, 24, 29)) or (S.COD in (50019, 50021, 50023)) or (S.BE = '98'))";
        //const string D3_SELECTION_STAGE2 = "((floor(S.COD/1000) in (25, 28)) or (S.COD in (50020, 50022)))";
        //const string D3_SELECTION_PROF = "floor(S.COD/1000) = 27";

        // Changed 2019-06-24
        const string D3_SELECTION_STAGE1 = "floor(S.COD/1000) in (22, 24)";
        const string D3_SELECTION_STAGE2 = "floor(S.COD/1000) in (25, 28)";
        const string D3_SELECTION_PROF = "floor(S.COD/1000) in (24, 27)";

        /// <summary>
        /// Выборка онкологии
        /// </summary>
#if FOMS
        const string D4_SELECTION = "(S.OTD not in ('0000', '0008', '0009')) and ((S.DS = 'Z03.1') or (left (S.DS, 1) = 'C') or (S.DS like 'D0%'))";
#else
        const string D4_SELECTION = "(S.OTD not in ('0000', '0008', '0009')) and ((S.DS = 'Z03.1') or (left (S.DS, 1) = 'C'))";
#endif

        string period;
        string lpuCode;
        SQL.Medialog medialog;
        Model.OrderSection lastRecoursesSection;
        Model.ProphSubsection lastRecourceSubsection;

        OleDbConnection connectionMain;
        OleDbConnection connectionAlt;

        OleDbCommand selectRecourses = null;
        OleDbCommand selectServicesTreatment;
        OleDbCommand selectServicesByDate;
        OleDbCommand selectServicesStage1;
        OleDbCommand selectServicesStage2;
        OleDbCommand selectServicesProf;
        OleDbCommand selectConcomDiseases;
        OleDbCommand selectDispDirections;
        OleDbCommand selectOnkologyDirections;
        OleDbCommand selectOnkologyTreat;

        AdapterStrings aStrings;
        AdapterPerson aPerson;
        AdapterInvoice aInvoice;
        AdapterRecourseAux aRecourse;
        AdapterServiceAux aService;
        AdapterConcomitantDisease aConcomitantDisease;
        AdapterOncoDirection aOncologyDirection;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="location">Каталог Релакс</param>
        /// <param name="lpuCode">Код ЛПУ (релаксовский)</param>
        /// <param name="period">Каталог текущего периода (от корневого каталога Релакс)</param>
        public Pool(string location, string lpuCode, string period, SQL.Medialog medialog) {
            this.period = period;
            this.lpuCode = lpuCode;
            lastRecourceSubsection = Model.ProphSubsection.None;
            this.medialog = medialog;

            aStrings = new AdapterStrings();
            aPerson = new AdapterPerson(medialog);
            aInvoice = new AdapterInvoice();
            aRecourse = new AdapterRecourseAux();
            aService = new AdapterServiceAux();
            aConcomitantDisease = new AdapterConcomitantDisease();
            aOncologyDirection = new AdapterOncoDirection();

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
                + "and (" + D3_SELECTION_STAGE1 + ")");
            AddStringParameters(selectServicesStage1, SELECT_SERVICES_TREATMENT);

            selectServicesStage2 = connectionAlt.CreateCommand();
            selectServicesStage2.CommandText = LocalizeQuery(Queries.SELECT_SERVICES
                + "and (" + D3_SELECTION_STAGE2 + ")");
            AddStringParameters(selectServicesStage2, SELECT_SERVICES_TREATMENT);

            selectServicesProf = connectionAlt.CreateCommand();
            selectServicesProf.CommandText = LocalizeQuery(Queries.SELECT_SERVICES
                + "and (" + D3_SELECTION_PROF + ")");
            AddStringParameters(selectServicesProf, SELECT_SERVICES_TREATMENT);

            selectConcomDiseases = connectionAlt.CreateCommand();
            selectConcomDiseases.CommandText = LocalizeQuery(Queries.SELECT_CONCOMITANT_DISEASES);
            AddStringParameters(selectConcomDiseases, SELECT_CONCOM_DISEASES);

            selectDispDirections = connectionAlt.CreateCommand();
            selectDispDirections.CommandText = LocalizeQuery(Queries.SELECT_DISP_ASSIGNMENTS);
            AddStringParameters(selectDispDirections, PARAMETER_EVENT_RECID);

            selectOnkologyDirections = connectionAlt.CreateCommand();
            selectOnkologyDirections.CommandText = LocalizeQuery(Queries.SELECT_ONCO_DIRECTIONS);
            AddStringParameters(selectOnkologyDirections, PARAMETER_EVENT_RECID);

            selectOnkologyTreat = connectionAlt.CreateCommand();
            selectOnkologyTreat.CommandText = LocalizeQuery(Queries.SELECT_ONKO_TREAT);
            AddStringParameters(selectOnkologyTreat, PARAMETER_EVENT_RECID);
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

            selectSpecs.CommandText = LocalizeQuery(Queries.UPDATE_ONKO_OTHER_AIM_COMMON);
            ExecuteCommand(selectSpecs);

            selectSpecs.CommandText = LocalizeQuery(Queries.UPDATE_ONKO_OTHER_AIM_MAIN);
            ExecuteCommand(selectSpecs);

            var fixes = LocalizeQuery(Queries.UPDATE_DAYOFF_DISPANSERIZATION)
                .Split(new char[] { ';' })
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s));

            foreach (string fix in fixes) {
                selectSpecs.CommandText = fix;
                ExecuteCommand(selectSpecs);
            }

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
        string SectionizeQuery(string sql, Model.OrderSection section, Model.ProphSubsection subsection) {
            switch (section) {
                case Model.OrderSection.D1:
                    return sql.Replace(APPENDIX_SECTION_MARKER, D1_SELECTION);

                case Model.OrderSection.D2:
                    return sql.Replace(APPENDIX_SECTION_MARKER, D2_SELECTION);

                case Model.OrderSection.D3:
                    switch(subsection) {
                        case Model.ProphSubsection.Stage1:
                            return sql.Replace(APPENDIX_SECTION_MARKER, D3_SELECTION_STAGE1);
                        case Model.ProphSubsection.Stage2:
                            return sql.Replace(APPENDIX_SECTION_MARKER, D3_SELECTION_STAGE2);
                        case Model.ProphSubsection.Prophylaxis:
                            return sql.Replace(APPENDIX_SECTION_MARKER, D3_SELECTION_PROF);
                        default: throw new NotImplementedException();
                    }

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
            yield break;
            //throw new NotImplementedException();
        }

        public IEnumerable<Model.OnkologyRefusal> LoadOnkologicalRefusal() {
            yield break;
            //throw new NotImplementedException();
        }

        public IEnumerable<Model.ComplexityQuotient> LoadComplexityQuotients() {
            yield break;
            //throw new NotImplementedException();
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

        public Model.OnkologyTreat GetOnkologyTreat(Model.Recourse rec, Model.Event evt) {
            string id = string.Format("{0,6}", evt.Identity);
            selectOnkologyTreat.Parameters[0].Value = id;

            Model.OnkologyTreat result = null;

            ExecuteReader(selectOnkologyTreat, r => {
                string mes1 = aStrings.Read(r, 0);
                result = new Model.OnkologyTreat(mes1);
            });

            if (result != null) {
                var drugService = evt.Services.FirstOrDefault(s => s.ServiceCode / 10000 == 52);
                if (drugService != null) {
                    var oncoService = Lib.OncoTherapyHelper.Service[drugService.ServiceCode];
                    if (oncoService != null) {
                        result.Services = new invox.Model.OncologyService[] { oncoService };
                        evt.ClinicalGroup = Lib.OncoTherapyHelper.Service.ClinicalCroupForService(drugService.ServiceCode);
                        oncoService.Drugs.First().Date = drugService.DateTill;
                    }
                }
            }

            return result;
        }

        public IEnumerable<Model.Sanction> LoadSanctions(Model.InvoiceRecord irec, Model.Recourse rec) {
            yield break;
        }

        public IEnumerable<Model.OncologyDirection> LoadOncologyDirections(Model.Recourse rec, Model.Event evt) {
            string id = string.Format("{0,6}", evt.Identity);
            selectOnkologyDirections.Parameters[0].Value = id;
            return aOncologyDirection.Load(selectOnkologyDirections);
        }

        public IEnumerable<Model.OncologyConsilium> LoadOncologyConsilium(Model.Recourse rec, Model.Event evt) {
            // У нас не проводятся консилиумы
            yield return new Model.OncologyConsilium(Model.OncologyConsiliumReason.NotNeeded, DateTime.Today);
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
                    command = selectServicesStage1;
                    break;

                case InternalReason.Stage2:
                    command = selectServicesStage2;
                    break;

                case InternalReason.Prof:
                    command = selectServicesProf;
                    break;

                case InternalReason.Other:
                case InternalReason.BriefTreatment:
                case InternalReason.DispRegister:
                case InternalReason.Emergency:
                case InternalReason.Diagnostics:
                    command = selectServicesByDate;
                    string d = ra.Date.ToString("MM/dd/yyyy").Replace('.', '/');
                    command.Parameters[3].Value = d;
                    break;
            }

            if (command != null) {
                command.Parameters[0].Value = string.Format("{0,6}", ra.PersonId);
                command.Parameters[1].Value = ra.Department;
                command.Parameters[2].Value = ra.InitialDiagnosis;
                return aService.Load(command);
            } else {
                return null;
            }
        }

        /// <summary>
        /// Получить вид вмешательства в зависимости от кода услуги операции
        /// </summary>
        /// <param name="serviceCode">Код услуги</param>
        /// <returns>Вид медицинского вмешательства V001</returns>
        string GetInterventionKind(int serviceCode) {
            object o = ExecuteScalar(connectionAlt, Queries.SELECT_SERVICE_NAME + "'" + serviceCode.ToString() + "'");
            if (o == null)
                return null;

            string result = (string)o;
            result = result.Trim();
            int pos = result.IndexOf(' ');
            result = result.Substring(0, pos);
            return result;
        }

        /// <summary>
        /// Загрузить события для законченного случая
        /// </summary>
        /// <remarks>
        /// В настоящий момент одному законченному случаю соответствует одно события (т.к. нет
        /// внутрибольничных переводов). Здесь также загружаются услуги для события и обновляются
        /// поля законченного случая данными, которые удалось получить на этом этапе.
        /// </remarks>
        List<Model.Event> LoadEvents(Model.Recourse rec, RecourseAux ra, Model.OrderSection section) {
            // Extract single event from the RecourseAux:
            List<Model.Event> result = new List<Model.Event>();
            Model.Event evt = ra.ToEvent(rec, section);
            result.Add(evt);
            
            // Load auxilliary service records and service models
            List<ServiceAux> ss = LoadServices(ra, evt).Distinct(new ServiceComparer()).OrderBy(s => s.Date).ToList();

            if (ra.InternalReason == InternalReason.SurgeryDayHosp
                && ss.Count(s => s.ServiceCode == ra.ServiceCode) > 1) {
                // Fix for:
                // Два случая ЦАХ у одного пациента с одним и тем же диагнозом
                //  будут выгружены как два случая, но в каждом ВСЕ услуги из обеих госпитализаций

                // Get main service
                ServiceAux sa = ss.FirstOrDefault(s => s.Date == ra.Date && s.ServiceCode == ra.ServiceCode);

                if (sa != null) {
                    // Get hospitalization dates
                    DateTime till = sa.Date;
                    DateTime from = till.WorkingDaysBefore(sa.BedDays, true);

                    // Select only services with relevant dates
                    IEnumerable<ServiceAux> sas = ss.Where(s => s.Date <= till && s.Date >= from);
                    ra.UpdateMedicalAid(rec, sas.Last());
                    evt.Services = sas.Select(s => s.ToService(ra)).ToList();
                    
                    // ...supposedly at least one services is selected: the main one
                } else {
                    ra.UpdateMedicalAid(rec, ss.Last());
                    evt.Services = ss.Select(s => s.ToService(ra)).ToList();
                }
            } else {
                ra.UpdateMedicalAid(rec, ss.Last());
                evt.Services = ss.Select(s => s.ToService(ra)).ToList();
            }

            // 20190902 - Блядский вид медицинского вмешательства
            if (ra.InternalReason == InternalReason.SurgeryDayHosp) {
                Model.Service service = evt.Services.FirstOrDefault(s => s.ServiceCode / 100000 == 3);
                if (service != null) {
                    string interventionKind = GetInterventionKind(service.ServiceCode);
                    if (!string.IsNullOrEmpty(interventionKind))
                        evt.Services.ForEach(s => s.InterventionKind = interventionKind);
                }
            }
            
            // Код врача и специальности должны соответствовать закрывающей записи,
            // приводим в соответствие из ra (где было изменено в UpdateMedicalAid())
            evt.SpecialityCode = ra.SpecialityCode;
            evt.DoctorCode = ra.DoctorCode;

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
            if (ra.InternalReason == InternalReason.Stage1 || ra.InternalReason == InternalReason.Prof) {
                evt.DateTill = evt.Services.Max(s => s.DateTill);

                // For DD1 - start of the recourse is an antropometry
                ServiceAux sa = ss.Where(s => s.IsAntropometry()).FirstOrDefault();
                if (sa != null) {
                    evt.DateFrom = sa.Date;
                } else {
                    evt.DateFrom = ss.Min(s => s.Date);
                }

#if FOMS1
            } else if (ra.InternalReason == InternalReason.Prof) {
                // Work around error "Код способа оплаты не соответствует периоду лечения;
                // Цель обращения не соответствует способу оплаты медицинской помощи"
                // Prof events within a single day are discarded with this error
                evt.DateFrom = ra.Date.ShiftDays(-1);

                if (evt.DateFrom == ra.Date) {
                    evt.DateTill = ra.Date.ShiftDays(1);
                    Model.Service s = evt.Services.First();
                    if (s != null) {
                        s.DateFrom = evt.DateTill;
                        s.DateTill = evt.DateTill;
                    }
                } else {
                    evt.DateTill = ra.Date;
                }
#endif
            } else {
                // For other cases service with minimal date (mayhap hospitalization - reason why not ss.Min())
                // 2 этап - пустая выборка
                try {
                    evt.DateFrom = evt.Services.Min(s => s.DateFrom);
                } catch (Exception e) {
                    Logger.Log(e.Message + string.Format(" - повод {0}, услуга {1}, S.RECID {2}", ra.InternalReason, ra.ServiceCode, ra.ServiceId));
                }
                evt.DateTill = evt.Services.Max(s => s.DateTill);
            }

#if FOMS
            // Задолбал ФОМС с их дурацкими ошибками: дд раз в 2 года ставит "неправильные даты", если дата начала и окончания не совпадают
            // В подушевой услуге ставим даты начала и окончания как во всем закрытом случае

            // 20190417 Хрен там, теперь не только обрезанная ДД
            Model.Service ser = evt.Services.FirstOrDefault(s => s.ServiceCode / 10000 == 5);
            if (ser != null) {
                ser.DateFrom = evt.DateFrom;
                ser.DateTill = evt.DateTill;
            }
#endif

            // May change evt and ra's MainDiagnosis:
            ra.FindConcurrentDiagnoses(evt, ss, section);

            // Service diagnosis as recorse's in case of diagnostics
            if (ra.InternalReason == InternalReason.Diagnostics)
                evt.Services.ForEach(s => s.Diagnosis = ra.MainDiagnosis);

            evt.ComplicationDiagnoses = ServiceAux.GetComplicationDiagnoses(ss);

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

            // Профиль МП случая - по профилю закрывающей записи
            rec.Profile = ss.OrderBy(s => s.Date).Last().AidProfile;

            // Other Event fields which can be taken from the services
            evt.Tariff = evt.Services.Sum(s => s.Tariff);
            evt.Total = evt.Services.Sum(s => s.Total);

            evt.PrimaryDiagnosis = ss.Max(s => s.PrimaryDiagnosis);
            evt.FirstIdentified = ss.Any(s => s.FirstIdentified);

            if (ra.InternalReason == InternalReason.DispRegister) {
                evt.DispensarySupervision = ss.Max(s => s.DispensarySupervision);
                if (evt.DispensarySupervision == Model.DispensarySupervision.None)
                    evt.DispensarySupervision = Model.DispensarySupervision.Observed;
            } else if (section == Model.OrderSection.D3) {
                // 20190430 - D3 section: disp supervision is explicit
                evt.DispensarySupervision = ss.Max(s => s.DispensarySupervision);
                // Диагнозы на R ФОМС считает подлежащими Д-учету
                if (evt.DispensarySupervision < Model.DispensarySupervision.Observed
                    || evt.DispensarySupervision > Model.DispensarySupervision.NotSubject) {
                        if ("ZY".Contains(evt.MainDiagnosis.First())) {
                            evt.DispensarySupervision = Model.DispensarySupervision.NotSubject;
                        } else {
                            if (evt.FirstIdentified)
                                evt.DispensarySupervision = Model.DispensarySupervision.Taken;
                            else
                                evt.DispensarySupervision = Model.DispensarySupervision.Observed;
                        }
                }
            }

            evt.ConcurrentMesCode = ss.Max(s => s.ConcurrentMesCode);
            
            var d2 = ss.Where(s => s.Transfer != Model.Transfer.None);
            if (d2 != null && d2.Count() > 0)
                evt.Transfer = d2.Min(s => s.Transfer);

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
                rec.DirectionDate = rec.DateFrom;
            }

            return result;
        }

        public IEnumerable<Model.ConcomitantDisease> GetConcomitantDiseases(Model.InvoiceRecord irec, Model.Event evt) {
            selectConcomDiseases.Parameters[0].Value = irec.Person.Policy;
            selectConcomDiseases.Parameters[1].Value = evt.Unit;
            selectConcomDiseases.Parameters[2].Value = evt.MainDiagnosis;
            return aConcomitantDisease.Load(selectConcomDiseases);
        }

        public IEnumerable<Model.DispAssignment> GetDispanserisationAssignments(Model.Recourse rec, Model.Event evt) {
            string id = string.Format("{0,6}", evt.Identity);
            selectDispDirections.Parameters[0].Value = id;
            List<Model.DispAssignment> result = null;

            Action<System.Data.Common.DbDataReader> onRead = r => {
                string codes = (string)r["KSG"];
                string values = (string)r["KSG2"];

                Model.NeoSuspectDirection susp = null;
                if (rec.SuspectOncology) {
                    // TODO: Target clinic and service code for direction if oncology suspected
                    susp = new Model.NeoSuspectDirection() {
                        Suspected = true,
                        DirectionDate = evt.DateTill,
                        TargetClinic = "XE3",
                        ServiceCode = "XE3"
                    };
                }

                result = Model.DispAssignment.Make(codes.Trim(), values.Trim(), susp).ToList();
            };

            ExecuteReader(selectDispDirections, onRead);
            return result;
        }

        public int GetPeopleCount(Model.OrderSection section, Model.ProphSubsection subsection) {
            string sql = SectionizeQuery(Queries.SELECT_PEOPLE_COUNT, section, subsection);
            sql = RecoursizeQuery(sql);
            object result = ExecuteScalar(connectionMain, LocalizeQuery(sql));
            return result != null && result != DBNull.Value ? (int)(decimal)result : 0;
        }

        public IEnumerable<Model.Person> LoadPeople(Model.OrderSection section, Model.ProphSubsection subsection) {
            string sql = SectionizeQuery(Queries.SELECT_PEOPLE, section, subsection);
            sql = RecoursizeQuery(sql);
            return aPerson.Load(connectionMain, LocalizeQuery(sql));
        }

        public int GetInvoiceRecordsCount(Model.OrderSection section, Model.ProphSubsection subsection) {
            string sql = SectionizeQuery(Queries.SELECT_INVOICE_RECORDS_COUNT, section, subsection);
            sql = RecoursizeQuery(sql);
            object result = ExecuteScalar(connectionMain, LocalizeQuery(sql));
            return result != null && result != DBNull.Value ? (int)(decimal)result : 0;
        }

        public decimal Total(Model.OrderSection section, Model.ProphSubsection subsection) {
            string sql = SectionizeQuery(Queries.SELECT_TOTAL, section, subsection);
            object result = ExecuteScalar(connectionMain, LocalizeQuery(sql));
            return result != null && result != DBNull.Value ? (decimal)result : 0;
        }

        public IEnumerable<Model.InvoiceRecord> LoadInvoiceRecords(Model.OrderSection section, Model.ProphSubsection subsection) {
            string sql = SectionizeQuery(Queries.SELECT_INVOICE_PEOPLE, section, subsection);
            sql = RecoursizeQuery(sql);
            return aInvoice.Load(connectionMain, LocalizeQuery(sql));
        }

        public List<string> LoadInitErrors() {
            string sql = LocalizeQuery(Queries.SELECT_NO_DOCTOR_DEPT);
            List<string> result = aStrings.Load(connectionMain, sql).ToList();
            result.AddRange(aStrings.Load(connectionMain, Queries.SELECT_NO_SPECIALITY));
            return result;
        }


        public IEnumerable<Model.Recourse> LoadRecourses(Model.InvoiceRecord irec, Model.OrderSection section, Model.ProphSubsection subsection) {
            // Query is cached 
            if (section != lastRecoursesSection || lastRecourceSubsection != subsection) {
                selectRecourses = null;
                lastRecoursesSection = section;
                lastRecourceSubsection = subsection;
            }

            if (selectRecourses == null) {
                string sql = LocalizeQuery(Queries.SELECT_RECOURSES);
                sql = RecoursizeQuery(sql);
                selectRecourses = connectionAlt.CreateCommand();
                selectRecourses.CommandText = SectionizeQuery(sql, section, subsection);
                AddStringParameters(selectRecourses, SELECT_RECOURSE_CASES_PARAMS);
            }
            string id = string.Format("{0,6}", irec.Person.Identity);
            selectRecourses.Parameters[0].Value = id;
            
            List<RecourseAux> rs = aRecourse.Load(selectRecourses).ToList();

            foreach (RecourseAux ra in rs) {
                Model.Recourse rec = ra.ToRecourse();
                invox.Dict.ResultOutcome.Instance.Repair(rec); 
                rec.Events = LoadEvents(rec, ra, section);
                yield return rec;
            }
        }
    }
}
