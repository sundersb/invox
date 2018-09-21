using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Model {
    enum OrderSection : int {
        D1,
        D2,
        D3
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
            }
            return string.Empty;
        }
    }
}
