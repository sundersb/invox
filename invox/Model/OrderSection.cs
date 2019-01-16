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

    static class OrderSectionHelper {
        public static string AsString(OrderSection section) {
            switch (section) {
                case OrderSection.D1:
                    return "с лечебной целью";
                case OrderSection.D2:
                    return "ВМП";
                case OrderSection.D3:
                    return "профилактики и диспансеризации";
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
}
