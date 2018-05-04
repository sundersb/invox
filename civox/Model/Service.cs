﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using civox.Lib;

namespace civox.Model {
    /// <summary>
    /// Service model
    /// </summary>
    /// <remarks>
    /// ФФОМС похоже плохо представляет себе, чем услуга
    /// отличается от законченного случая. Случаи обращения в их интерпретации
    /// обретают атрибуты услуг, а услуги - законченных случаев. Группировки полис/диагноз для вычленения
    /// обращений не хватает (для посещений с лечебной целью работает,
    /// для проф. и иных - нет), а при формировании коллекции услуг
    /// не хватает данных из случая (диагноз, отделение). Отсюда и причудливые запросы.
    /// По этой же причине Service не наследует Model: экспорт в XML реализует InvoiceRecord
    /// </remarks>
    class Service {
        // V008
        const string AID_KIND_PRIMARY = "1";
        const string AID_KIND_EMERGENCY = "2";
        const string AID_KIND_SPECIALIZED = "31";
        const string AID_KIND_HITECH = "32";
        
        // V014
        const string AID_FORM_URGENT = "1";
        const string AID_FORM_PRESSING = "2";
        const string AID_FORM_ORDINAL = "3";

        // TODO: Dispanserisation once in two years
        static int[] DISP_I_CODES = { 22, 24, 29 };
        static int[] DISP_II_CODES = { 25, 28 };
        static int[] DISP_I_RESULT_CODES = { 22, 29 };

        const int DISP_I_START_CODE = 24001;
        const int DISP_II_RESULT_CODE = 28;
        const int RECOURSE_RESULT_CODE = 50;
        const int DAY_HOSP_CODE = 30;
        const int PROPHYLAX_CODE = 27;

        DateTime beginDate;
        DateTime endDate;
        int quantity;

        public DateTime BeginDate { get { return beginDate; } }
        public DateTime EndDate { get { return endDate; } }
        public int Quantity { get { return quantity; } }

        /// <summary>
        /// Вид медицинской помощи V008
        /// </summary>
        public string AidKind { get { return GetAidKind(); } }

        /// <summary>
        /// Форма оказания МП V014
        /// </summary>
        public string AidForm { get { return GetAidForm(); } }

        public long ID;
        public int ServiceCode;
        public string CardNumber;
        public decimal Price;
        public string SpecialCase;
        public string DoctorCode;
        public string ResultCode;    // V009
        public string DispResultCode;// V017
        public string AidProfile;    // V002
        public string DoctorProfile; // V015
        public string PayKind;       // V010
        public string RecourseAim;   // CEL (territory)
        public bool Refusal;
        public bool UrgentHospitalization;
        public bool VisitingBrigade; // Выездная бригада

        public void SetDates(DateTime serviceDate, int days) {
            if (days < 0) return;

            if (days <= 1) {
                quantity = 1;
                beginDate = endDate = serviceDate;
            } else {
                quantity = days;
                endDate = serviceDate;
                beginDate = serviceDate.WorkingDaysBefore(days);
            }
        }

        /// <summary>
        /// Mark relevant services as resulting, first or last
        /// </summary>
        /// <param name="services">List of recourse services</param>
        public static RecourseLandmarks ArrangeServices(List<Service> services) {
            if (services == null) return null;
            RecourseLandmarks result = new RecourseLandmarks();

            if (services.Any(s => DISP_I_CODES.Contains(s.ServiceCode / 1000))) {
                // Dispanserisation I stage
                result.First = services.FirstOrDefault(s => s.ServiceCode == DISP_I_START_CODE);
                result.Last = services.FirstOrDefault(s => DISP_I_RESULT_CODES.Contains(s.ServiceCode / 1000));
                result.Resulting = result.Last;
            } else if (services.Any(s => DISP_II_CODES.Contains(s.ServiceCode / 1000))) {
                // Dispanserisation II stage
                DateTime date = services.Min(s => s.EndDate);
                result.First = services.FirstOrDefault(s => s.EndDate == date);
                result.Last = services.FirstOrDefault(s => s.ServiceCode / 1000 == DISP_II_RESULT_CODE);
                result.Resulting = result.Last;
            } else if (services.Any(s => s.ServiceCode / 100 == DAY_HOSP_CODE)) {
                // Day hospital
                result.Resulting = services.FirstOrDefault(s => s.ServiceCode / 100 == DAY_HOSP_CODE);

                DateTime date = services.Max(s => s.EndDate);
                result.Last = services.FirstOrDefault(s => s.EndDate == date);

                date = services.Min(s => s.EndDate);
                result.First = services.FirstOrDefault(s => s.EndDate == date);
            } else {
                // Set resulting service by code 50xxx, 27xxx
                result.Resulting = services.FirstOrDefault(s => {
                    int sc = s.ServiceCode / 1000;
                    return sc == RECOURSE_RESULT_CODE || sc == PROPHYLAX_CODE;
                });
                
                DateTime date = services.Max(s => s.EndDate);
                result.Last = services.FirstOrDefault(s => s.EndDate == date);

                date = services.Min(s => s.EndDate);
                result.First = services.FirstOrDefault(s => s.EndDate == date);
            }
            return result;
        }

        string GetAidKind() {
            switch (ServiceCode / 10000) {
                case 7:
                    return AID_KIND_HITECH;

                case 4:
                    return AID_KIND_EMERGENCY;

                default:
                    if (ServiceCode / 1000 == 98)
                        return AID_KIND_SPECIALIZED;
                    else
                        return AID_KIND_PRIMARY;
            }
        }

        string GetAidForm() {
            if (ServiceCode / 10000 == 4) return AID_FORM_URGENT;
            if (ServiceCode / 1000 == 7) return AID_FORM_PRESSING;
            return AID_FORM_ORDINAL;
        }

        /// <summary>
        /// XML-stream the service
        /// </summary>
        /// <param name="rec">Recourse to which this service is bound to</param>
        /// <param name="xml">XML streaming helper</param>
        /// <param name="repo">Data repository</param>
        public void Write(Recourse rec, Lib.XmlExporter xml, Data.IInvoice repo) {
            xml.Writer.WriteStartElement("USL");

            xml.Writer.WriteElementString("IDSERV", ID.ToString());

            xml.Writer.WriteElementString("LPU", Options.LpuCode);
            // LPU_1    У Подразделение МО лечения из регионального справочника

            if (rec.Section == AppendixSection.D3) {
                // Требует ФОМС, не по приказу
                xml.Writer.WriteElementString("PODR", rec.Department);
                // Doubling code in attitude to DATE_IN, DATE_OUT because of featured FOMS formal check
            } else {
                xml.Writer.WriteElementString("PODR", rec.Department);
                xml.Writer.WriteElementString("PROFIL", AidProfile);
                // VID_VME  У Вид медицинского вмешательства V001
                xml.WriteBool("DET", Options.Pediatric);
            }
            xml.Writer.WriteElementString("DATE_IN", BeginDate.AsXml());
            xml.Writer.WriteElementString("DATE_OUT", EndDate.AsXml());

            // ФОМС требует диагноз даже для ДД и проф. Не по приказу
            xml.Writer.WriteElementString("DS", rec.Diagnosis);

            // Признак отказа. ФОМС пропускает везде, но по приказу только ДД и проф. Теперь дает ошибку
            //xml.Writer.WriteElementString("P_OTK", s.Refusal ? "1" : string.Empty);
            // Порядок элементов для ФОМС имеет значение: если сунуть CODE_USL выше или ниже, БАРС выдаст ошибку:
            xml.Writer.WriteElementString("CODE_USL", ServiceCode.ToString("D6"));
            xml.Writer.WriteElementString("KOL_USL", Quantity.ToString());

            //xml.Writer.WriteElementString("TARIF", "1");          // TODO:
            xml.Writer.WriteElementString("SUMV_USL", string.Format(Options.NumberFormat, "{0:f2}", Price));

            xml.Writer.WriteElementString("PRVS", DoctorProfile);        // V015
            xml.Writer.WriteElementString("CODE_MD", DoctorCode);

            // NPL      У Неполный объем Только Д1
            //1 - документированный отказ больного,
            //2 - медицинские противопоказания,
            //3 - прочие причины (умер, переведен в другое отделение и пр.)
            //4 - ранее проведенные услуги в пределах установленных сроков
            //xml.Writer.WriteElementString("NPL", string.Empty);

            // Нате вам пасхалку
            xml.WriteIfValid("COMENTU", Options.ReadingBot.Read());
            //xml.Writer.WriteElementString("COMENTU", string.Empty);

            xml.Writer.WriteEndElement();
        }
    }

    class RecourseLandmarks {
        public Service Resulting;
        public Service First;
        public Service Last;

        public bool Valid { get { return Resulting != null && First != null && Last != null; } }
    }
}
