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
        Model.OnkologyTreat GetOnkologyTreat(Model.Recourse rec, Model.Event evt);
        IEnumerable<Model.Sanction> LoadSanctions(Model.InvoiceRecord irec, Model.Recourse rec);
        IEnumerable<Model.OncologyDirection> LoadOncologyDirections(Model.Recourse rec, Model.Event evt);
        IEnumerable<Model.OncologyConsilium> LoadOncologyConsilium(Model.Recourse rec, Model.Event evt);
        IEnumerable<Model.OncologyService> LoadOncologyServices();
        IEnumerable<Model.OncologyDrug> LoadOncologyDrugs();

        IEnumerable<Model.ConcomitantDisease> GetConcomitantDiseases(Model.InvoiceRecord irec, Model.Event evt);
        IEnumerable<Model.DispAssignment> GetDispanserisationAssignments(Model.Recourse rec, Model.Event evt);

        int GetPeopleCount(Model.OrderSection section, Model.ProphSubsection subsection);
        IEnumerable<Model.Person> LoadPeople(Model.OrderSection section, Model.ProphSubsection subsection);

        int GetInvoiceRecordsCount(Model.OrderSection section, Model.ProphSubsection subsection);
        decimal Total(Model.OrderSection section, Model.ProphSubsection subsection);

        IEnumerable<Model.InvoiceRecord> LoadInvoiceRecords(Model.OrderSection section, Model.ProphSubsection subsection);
        IEnumerable<Model.Recourse> LoadRecourses(Model.InvoiceRecord irec, Model.OrderSection section, Model.ProphSubsection subsection);
        List<string> LoadInitErrors();
    }
}
