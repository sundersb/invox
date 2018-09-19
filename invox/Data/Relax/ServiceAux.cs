using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Lib;

namespace invox.Data.Relax {
    class ServiceAux {
        const int DD_ANTROPOMETRY_CODE = 24001;
        const int[] INDEPENDED_TRANSFERS = { 1, 4, 7, 8, 9, 12, 13, 14, 16, 18 };

        public string ServiceId;
        public string AidProfile;
        public int ServiceCode;
        public string Result;
        public string BedProfile;
        public string InterventionKind;
        public int Quantity;
        public decimal Tariff;
        public decimal Total;
        public string SpecialityCode;
        public string DoctorCode;

        public Model.Service.IncompleteServiceReason Incomplete;         // ??? by REZOBR.SLIZ - Result - TODO
        public bool Refusal;            // ??? by REZOBR.SLIZ - Result - TODO

        public DateTime Date;
        
        public bool Newborn;
        public string DirectedFrom;
        public int BirthWeight;
        public string Outcome1;
        public Model.Transfer Transfer;
        public string Reason1;
        public int BedDays;
        public string PrimaryDiagnosis;
        public bool FirstIdentified;
        public string ConcurrentDiagnosis;
        public string ComplicationDiagnosis;
        public Model.DispensarySupervision DispensarySupervision;
        public string ConcurrentMesCode;

        public bool IsAntropometry() {
            return ServiceCode == DD_ANTROPOMETRY_CODE;
        }

        public Model.Transfer GetTransfer(int relaxCode) {
            if (INDEPENDED_TRANSFERS.Contains(relaxCode))
                return Model.Transfer.Independently;

            switch (relaxCode) {
                case 10:
                    return Model.Transfer.Emergency;

                case 15:
                    return Model.Transfer.Transferred;

                default:
                    return Model.Transfer.None;
            }
        }

        public Model.Service ToService(RecourseAux ra) {
            Model.Service result = new Model.Service();

            result.Identity = ServiceId;
            result.Unit = ra.Unit;
            result.InterventionKind = InterventionKind;
            result.Child = ra.Child;
            
            SetDates(result);

            result.Diagnosis = ra.MainDiagnosis;
            result.ServiceCode = ServiceCode;
            result.Quantity = Quantity;
            result.Tariff = Tariff;
            result.Total = Total;
            result.SpecialityCode = SpecialityCode;
            result.DoctorCode = DoctorCode;
            result.Incomplete = Incomplete;
            result.Refusal = Refusal;

            return result;
        }

        void SetDates(Model.Service service) {
            if (Quantity <= 1) {
                Quantity = 1;
                service.DateFrom = service.DateTill = Date;
            } else {
                service.DateTill = Date;
                service.DateFrom = Date.WorkingDaysBefore(Quantity);
            }
        }

    }
}
