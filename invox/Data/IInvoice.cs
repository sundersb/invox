using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Data {
    interface IInvoice {
        bool Init();
        IEnumerable<Model.OnkologyDiagnosticType> LoadOnkologicalDiagnosticTypes();
        IEnumerable<Model.OnkologyRefusal> LoadOnkologicalRefusal();
        IEnumerable<Model.ComplexityQuotient> LoadComplexityQuotients();

        IEnumerable<string> LoadMesCodes(Model.InvoiceRecord irec, Model.Recourse rec, Model.Event evt);

        IEnumerable<string> LoadPersonDiagnoses();
        Model.OnkologyTreat GetOnkologyTreat();
        IEnumerable<Model.Sanction> LoadSanctions(Model.InvoiceRecord irec, Model.Recourse rec);
        IEnumerable<Model.OncologyDirection> LoadOncologyDirections(Model.Recourse rec, Model.Event evt);
        IEnumerable<Model.OncologyConsilium> LoadOncologyConsilium(Model.Recourse rec, Model.Event evt);
        Model.OncologyService GetOncologyService();

        IEnumerable<Model.ConcomitantDisease> GetConcomitantDiseases(Model.InvoiceRecord irec, Model.Event evt);
        IEnumerable<Model.DispAssignment> GetDispanserisationAssignments(Model.Recourse rec, Model.Event evt);

        int GetPeopleCount(Model.OrderSection section);
        IEnumerable<Model.Person> LoadPeople(Model.OrderSection section);

        int GetInvoiceRecordsCount(Model.OrderSection section);
        decimal Total(Model.OrderSection section);

        IEnumerable<Model.InvoiceRecord> LoadInvoiceRecords(Model.OrderSection section);
        IEnumerable<Model.Recourse> LoadRecourses(Model.InvoiceRecord irec, Model.OrderSection section);
        List<string> LoadInitErrors();
    }
}
