using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Data.Relax {
    class AdapterInvoice : AdapterBase<Model.InvoiceRecord> {
        public override Model.InvoiceRecord Read(System.Data.Common.DbDataReader reader, int number) {
            Model.InvoiceRecord result = new Model.InvoiceRecord();
            result.Identity = number;
            result.Revised = false;
            result.Person = new Model.InvoicePerson();
            Model.InvoicePerson p = result.Person;

            p.Identity = ReadString(reader["RECID"]);
            p.PolicyType = ReadInt(reader["T_POL"]);

            // Полис единого образца, если не указан
            if (p.PolicyType == 0) p.PolicyType = 3;

            p.Policy = ReadString(reader["SN_POL"]);
            p.SmoOkato = ReadString(reader["MSO_OKATO"]).Substring(0, 5);

            p.SmoCode = ReadString(reader["Q"]);
            if (p.SmoCode == "IN")
                p.SmoCode = string.Empty;
            else
                p.SmoCode = Dict.SMO.Get(p.SmoCode);

            p.SmoOgrn = ReadString(reader["MSO_OGRN"]);

#if FOMS
            // KHFOMS requires SMO_NAME
            p.SmoName = ReadString(reader["MSO_NAME"]);
#else
            if (string.IsNullOrEmpty(p.SmoCode)) {
                p.SmoOgrn = ReadString(reader["MSO_OGRN"]);
                if (string.IsNullOrEmpty(p.SmoOgrn))
                    p.SmoName = ReadString(reader["MSO_NAME"]);
            }
#endif

            // Обновление к релакс 20171215
            p.Disability = Model.Disability.NA;
            string kt = ReadString(reader["KT"]);
            if (!string.IsNullOrEmpty(kt)) {
                int i = 0;
                if (int.TryParse(kt, out i)) {
                    if (i < 5 && i >= 0)
                        p.Disability = (Model.Disability)i;
                    else if (i == 5)
                        p.DirectedToSE = true;
                }
            }

            if (ReadBool(reader["NOVOR"])) {
                // Newborn
                DateTime bd = ReadDate(reader["DR"]);
                int sex = ReadInt(reader["W"]);

                StringBuilder sb = new StringBuilder();
                sb.Append(sex);
                sb.Append(bd.Day);
                sb.Append(bd.Month.ToString("D2"));
                sb.Append((bd.Year % 100).ToString("D2"));
                // TODO: Ordinal number of a newborn - where to find?
                sb.Append('0');
                p.NewbornCode = sb.ToString();
            } else {
                p.NewbornCode = "0";
            }
            // TODO: Newborn weight???

            return result;
        }
    }
}
