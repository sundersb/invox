using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Data {
    interface IInvoice {
        IEnumerable<Model.OnkologyDiagnosticType> LoadOnkologicalDiagnosticTypes();
        IEnumerable<Model.OnkologyRefusal> LoadOnkologicalRefusal();
        IEnumerable<Model.ComplexityQuotient> LoadComplexityQuotients();

        IEnumerable<string> LoadConcurrentDiagnoses();
        IEnumerable<string> LoadComplicationDiagnoses();
        IEnumerable<string> LoadMesCodes();

        IEnumerable<string> LoadPersonDiagnoses();
        Model.OnkologyTreat GetOnkologyTreat();
        IEnumerable<Model.Sanction> LoadSanctions();
        IEnumerable<Model.Service> LoadServices();
        IEnumerable<Model.OncologyDirection> LoadOncologyDirections();
        Model.OncologyService GetOncologyService();
        IEnumerable<Model.Event> LoadEvents(Model.InvoiceRecord irec, Model.Recourse rec);
        IEnumerable<Model.ConcomitantDisease> GetConcomitantDiseases();
        IEnumerable<Model.DispAssignment> GetDispanserisationAssignmetns();

        int GetPeopleCount(Model.OrderSection section);
        IEnumerable<Model.Person> LoadPeople(Model.OrderSection section);

        int GetInvoiceRecordsCount(Model.OrderSection section);
        decimal Total(Model.OrderSection section);

        IEnumerable<Model.InvoiceRecord> LoadInvoiceRecords(Model.OrderSection section);
        IEnumerable<Model.Recourse> LoadRecourses(Model.InvoiceRecord irec, Model.OrderSection section);
        IEnumerable<string> LoadNoDeptDoctors();
    }
}
