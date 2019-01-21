using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Model {
    enum OrderSection : int {
        D1,
        D2,
        D3,
        D4
    }

    enum ProphSubsection {
        None,                // 
        Stage1,              // в рамках первого этапа диспансеризации определенных групп взрослого населения
        Stage2,              // в рамках второго этапа диспансеризации определенных групп взрослого населения
        Prophylaxis,         // в рамках профилактических осмотров взрослого населения
        DispChildrenTight,   // в рамках диспансеризации пребывающих в стационарных учреждениях детей-сирот и детей, находящихся в трудной жизненной ситуации
        DispChildrenAdopted, // детей-сирот и детей, оставшихся без попечения родителей, в том числе усыновленных (удочеренных), принятых под опеку (попечительство), в приемную или патронатную семью
        ProphChildren        // в рамках профилактических медицинских осмотров несовершеннолетних
    }

    static class OrderSectionHelper {
        public static string AsString(OrderSection section, ProphSubsection subsection) {
            switch (section) {
                case OrderSection.D1:
                    return "с лечебной целью";
                case OrderSection.D2:
                    return "ВМП";
                case OrderSection.D3:
                    return "профилактики и диспансеризации ("
                        + ProphSubsectionHelper.AsString(subsection) + ")";
                case OrderSection.D4:
                    return "онкологии";
            }
            return string.Empty;
        }

        public static OrderSection[] FromString(string commandLineOption) {
            if (string.IsNullOrEmpty(commandLineOption) || commandLineOption.Any(c => c >= '4' || c < '0')) {
                return new OrderSection[] {
                    OrderSection.D1,
                    OrderSection.D2,
                    OrderSection.D3,
                    OrderSection.D4
                };
            } else {
                List<OrderSection> result = new List<OrderSection>();

                foreach (Char c in commandLineOption.ToArray()) {
                    int i = (int)(c - '0');
                    result.Add((OrderSection)i - 1);
                }

                return result.ToArray();
            }
        }
    }

    static class ProphSubsectionHelper {
        public static string AsString(ProphSubsection s) {
            switch (s) {
                case ProphSubsection.Stage1: return "I этап диспансесризации";
                case ProphSubsection.Stage2: return "II этап диспансеризации";
                case ProphSubsection.Prophylaxis: return "профилактика";
                case ProphSubsection.DispChildrenTight: return "диспансеризация несовершеннолетних в трудной жизненной ситуации";
                case ProphSubsection.DispChildrenAdopted: return "диспансеризация детей-сирот";
                case ProphSubsection.ProphChildren: return "профосмотры несовершеннолетних";
                default: return string.Empty;
            }
        }

        public static string GetCode(ProphSubsection s) {
            switch (s) {
                case ProphSubsection.Stage1: return "P";
                case ProphSubsection.Stage2: return "V";
                case ProphSubsection.Prophylaxis: return "O";
                case ProphSubsection.DispChildrenTight: return "S";
                case ProphSubsection.DispChildrenAdopted: return "U";
                case ProphSubsection.ProphChildren: return "F";
                default: return string.Empty;
            }
        }

        public static ProphSubsection[] GetSubsections(OrderSection section, bool pediatric) {
            if (section != OrderSection.D3) {
                return new ProphSubsection[] { ProphSubsection.None };
            } else {
                if (pediatric) {
                    return new ProphSubsection[] {
                        ProphSubsection.Stage1,
                        ProphSubsection.Stage2,
                        ProphSubsection.Prophylaxis,
                        ProphSubsection.DispChildrenTight,
                        ProphSubsection.DispChildrenAdopted,
                        ProphSubsection.ProphChildren
                    };
                } else {
                    return new ProphSubsection[] {
                        ProphSubsection.Stage1,
                        ProphSubsection.Stage2,
                        ProphSubsection.Prophylaxis
                    };
                }
            }
        }
    }
}
