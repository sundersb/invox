using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Model {
    class Service {
        // TODO: Dispanserisation once in two years
        static int[] DISP_I_CODES = { 22, 24, 29 };
        static int[] DISP_II_CODES = { 25, 28 };
        static int[] DISP_I_START_CODES = { 24001, 24003 };
        static int[] DISP_I_RESULT_CODES = { 22, 29 };
        const int DISP_II_RESULT_CODE = 28;
        const int RECOURSE_RESULT_CODE = 50;

        public long ID;
        public DateTime Date;
        public int ServiceCode;
        public string CardNumber;
        public int Quantity;
        public decimal Price;
        public string SpecialCase;
        public string DoctorCode;
        public string ResultCode; // V009

        /// <summary>
        /// Mark relevant services as resulting, first or last
        /// </summary>
        /// <param name="services">List of recourse services</param>
        public static RecourseLandmarks ArrangeServices(List<Service> services) {
            if (services == null) return null;
            RecourseLandmarks result = new RecourseLandmarks();

            if (services.Any(s => DISP_I_CODES.Contains(s.ServiceCode / 1000))) {
                // Dispanserisation I stage
                result.First = services.FirstOrDefault(s => DISP_I_START_CODES.Contains(s.ServiceCode));
                result.Last = services.FirstOrDefault(s => DISP_I_RESULT_CODES.Contains(s.ServiceCode / 1000));
                result.Resulting = result.Last;
            } else if (services.Any(s => DISP_II_CODES.Contains(s.ServiceCode / 1000))) {
                // Dispanserisation II stage
                DateTime date = services.Min(s => s.Date);
                result.First = services.FirstOrDefault(s => s.Date == date);
                result.Last = services.FirstOrDefault(s => s.ServiceCode / 1000 == DISP_II_RESULT_CODE);
                result.Resulting = result.Last;
            } else {
                // Set resulting service by code 50xxx
                // TODO: Hospitalization resulting service
                result.Resulting = services.FirstOrDefault(s => s.ServiceCode / 1000 == RECOURSE_RESULT_CODE);
                
                DateTime date = services.Max(s => s.Date);
                result.Last = services.FirstOrDefault(s => s.Date == date);

                date = services.Min(s => s.Date);
                result.First = services.FirstOrDefault(s => s.Date == date);
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
