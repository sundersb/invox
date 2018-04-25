using System;
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
    /// не хватает данных из случая. Отсюда и причудливые запросы.
    /// </remarks>
    class Service {
        // TODO: Dispanserisation once in two years
        static int[] DISP_I_CODES = { 22, 24, 29 };
        static int[] DISP_II_CODES = { 25, 28 };
        static int[] DISP_I_RESULT_CODES = { 22, 29 };

        const int DISP_I_START_CODE = 24001;
        const int DISP_II_RESULT_CODE = 28;
        const int RECOURSE_RESULT_CODE = 50;
        const int DAY_HOSP_CODE = 30;

        DateTime beginDate;
        DateTime endDate;
        int quantity;

        public DateTime EndDate { get { return beginDate; } }
        public DateTime BeginDate { get { return endDate; } }
        public int Quantity { get { return quantity; } }

        public long ID;
        public int ServiceCode;
        public string CardNumber;
        public decimal Price;
        public string SpecialCase;
        public string DoctorCode;
        public string ResultCode;    // V009
        public string AidProfile;    // V002
        public string DoctorProfile; // V015
        public string PayKind;       // V010
        public string RecourseAim;   // CEL (territory)
        public bool Refusal;

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
                // Set resulting service by code 50xxx
                result.Resulting = services.FirstOrDefault(s => s.ServiceCode / 1000 == RECOURSE_RESULT_CODE);
                
                DateTime date = services.Max(s => s.EndDate);
                result.Last = services.FirstOrDefault(s => s.EndDate == date);

                date = services.Min(s => s.EndDate);
                result.First = services.FirstOrDefault(s => s.EndDate == date);
            }
            return result;
        }
    }

    class RecourseLandmarks {
        public Service Resulting;
        public Service First;
        public Service Last;

        public bool Valid { get { return Resulting != null && First != null && Last != null; } }
    }
}
