using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using civox.Model;

namespace civox.Lib {
    /// <summary>
    /// Invoice file names helper
    /// </summary>
    // TODO: Extend InvoiceNames with invoice-to-SMO factory
    class InvoiceNames {
        string people;
        string invoice;
        string smoCode;

        InvoiceKind invoiceKind;

        /// <summary>
        /// Name of the file with personal data
        /// </summary>
        public string PeopleFileName { get { return people; } }

        /// <summary>
        /// Bills filename
        /// </summary>
        public string InvoiceFileName { get { return invoice; } }

        /// <summary>
        /// Federal code of the assurance company
        /// </summary>
        public string SmoCode { get { return smoCode; } }

        /// <summary>
        /// Invoice kind
        /// </summary>
        public InvoiceKind InvoiceKind { get { return invoiceKind; } }


        InvoiceNames(string people, string invoice) {
            this.people = people;
            this.invoice = invoice;

            // TODO: Invoice SMO - unnecessary
            smoCode = string.Empty;
        }

        static string GetAgentCode(AgentCode agent, string smoCode) {
            switch (agent) {
                case AgentCode.TerritoryFund: return string.Format("T{0}", Options.FomsCode);
                case AgentCode.AssuranceCompany: return string.Format("S{0}", smoCode);
                case AgentCode.Clinic: return string.Format("M{0}", Options.LpuCode);
            }
            Lib.Logger.Log("Упущен контрагент в InvoiceNames.GetAgentCode(): " + agent.ToString());
            return string.Empty;
        }

        static char GetProfCode(ProfKind kind) {
            switch (kind) {
                case ProfKind.Stage1: return 'P';
                case ProfKind.Stage2: return 'V';
                case ProfKind.Adults: return 'O';
                case ProfKind.Orphans: return 'S';
                case ProfKind.Adopted: return 'U';
                case ProfKind.Underage: return 'F';
                default:
                    Lib.Logger.Log("Упущен вид посещения с профцелью в InvoiceNames.GetProfCode(): " + kind.ToString());
                    return 'O';
            }
        }

        /// <summary>
        /// Get filenames for an invoice
        /// </summary>
        /// <param name="packetNumber">Packet number within the period</param>
        /// <param name="invoiceKind">Invoice kind</param>
        /// <param name="profKind">Prophylaxis kind in case invoiceKind is Prophylaxis</param>
        /// <returns>Names for the person data and invoice files</returns>
        public static InvoiceNames InvoiceToFoms(int packetNumber,
            InvoiceKind invoiceKind,
            ProfKind profKind = ProfKind.Adults) {

            StringBuilder b = new StringBuilder();
            b.Append(GetAgentCode(AgentCode.Clinic, string.Empty));
            b.Append(GetAgentCode(AgentCode.TerritoryFund, string.Empty));
            b.Append('_');
            b.Append(Options.Year % 100);
            b.Append(string.Format("{0:d2}", Options.Month));

            //b.Append(packetNumber % 10); // FOMS ignores "single digit" rule
            b.Append(packetNumber);
            
            string body = b.ToString();
            string invoice = null;
            string people = null;

            switch (invoiceKind) {
                case InvoiceKind.GeneralTreatment:
                    invoice = "H" + body;
                    people = "L" + body;
                    break;
                case InvoiceKind.HiTechAid:
                    invoice = "T" + body;
                    people = "LT" + body;
                    break;
                case InvoiceKind.Prophylaxis:
                    body = GetProfCode(profKind) + body;
                    invoice = "D" + body;
                    people = "L" + body;
                    break;
                default:
                    Lib.Logger.Log("Упущен вид счета в InvoiceNames.InvoiceToFoms(): " + invoiceKind.ToString());
                    break;
            }
            return new InvoiceNames(people, invoice) {
                invoiceKind = invoiceKind
            };
        }
    }
}
