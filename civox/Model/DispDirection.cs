using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Model {
    /// <summary>
    /// Dispanserisation result bound direction
    /// </summary>
    class DispDirection : Model {
        int index;
        int routeCode;
        string doctorSpec;
        string studyKind;
        string aidProfile;
        string bedProfile;

        DispDirection(int number, int code, string value) {
            index = number;
            routeCode = code;

            switch (code) {
                case 1:
                case 2:
                    doctorSpec = value;
                    break;

                case 3:
                    studyKind = value;
                    break;

                case 4:
                case 5:
                    aidProfile = value;
                    break;

                case 6:
                    bedProfile = value;
                    break;

                default:
                    Lib.Logger.Log("Неверный код направления по результату диспансеризации: " + code.ToString());
                    break;
            }
        }

        public override void Write(Lib.XmlExporter xml, Data.IInvoice repo) {
            //      NAZ_R       Заполняется при присвоении группы здоровья, кроме I и II.
            //                  1 – направлен на консультацию в медицинскую организацию по месту прикрепления;
            //                  2 – направлен на консультацию в иную медицинскую организацию;
            //                  3 – направлен на обследование;
            //                  4 – направлен в дневной стационар;
            //                  5 – направлен на госпитализацию;
            //                  6 – направлен в реабилитационное отделение
            xml.Writer.WriteElementString("NAZ_R", routeCode.ToString());

            //      NAZ_SP      Специальность врача, если в поле NAZ_R проставлены коды 1 или 2. Классификатор V021
            xml.WriteIfValid("NAZ_SP", doctorSpec);

            //      NAZ_V       Вид обследования, если в поле NAZ_R проставлен код 3.
            //                  1 – лабораторная диагностика;
            //                  2 – инструментальная диагностика;
            //                  3 – методы лучевой диагностики, за исключением дорогостоящих;
            //                  4 – дорогостоящие методы лучевой диагностики (КТ, МРТ, ангиография)
            xml.WriteIfValid("NAZ_V", studyKind);

            //      NAZ_PMP     Профиль медицинской помощи, если в поле NAZ_R проставлены коды 4 или 5 Классификатор V002
            xml.WriteIfValid("NAZ_PMP", aidProfile);

            //      NAZ_PK      Профиль койки, если в поле NAZ_R проставлен код 6. Классификатор V020
            xml.WriteIfValid("NAZ_PK", bedProfile);
        }

        /// <summary>
        /// Get collection of dispanserisation bound directions
        /// </summary>
        /// <param name="KSG">Direction code (МЭС1, PAT.KSG)</param>
        /// <param name="KSG2">Direction elaboration (МЭС2, KSG2)</param>
        public static IEnumerable<DispDirection> Make(string KSG, string KSG2) {
            int number = 0;
            int code;

            string[] codes = KSG.Split('-');
            string[] values = KSG2.Split('-');

            int j = Math.Min(codes.Length, values.Length);
            for (int i = 0; i < j; ++i) {
                if (int.TryParse(codes[i], out code)) {
                    ++number;
                    yield return new DispDirection(number, code, values[i]);
                }
            }
        }
    }
}
