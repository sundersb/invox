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
        IEnumerable<Model.Event> LoadEvents();
    }
}
