using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using invox.Lib;

namespace invox.Model {
    /// <summary>
    /// Параметры направления на обследование при подозрении на онкологию
    /// </summary>
    class NeoSuspectDirection {
        /// <summary>
        /// True если структура содержит валидные данные
        /// </summary>
        public bool Suspected;

        /// <summary>
        /// Медицинская услуга (код), указанная в направлении
        /// Заполняется, в соответствии с номенклатурой медицинских услуг (V001) только
        /// при направлении на обследование в случае подозрения на ЗНО (NAZ_R=3 и DS_ONK=1)
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// Дата направления
        /// Заполнение обязательно только в случаях оформления направления в случае подозрения
        /// на ЗНО: на консультацию в другую МО или на обследование (NAZ_R={2,3} и DS_ONK=1)
        /// </summary>
        public DateTime DirectionDate { get; set; }

        /// <summary>
        /// Код МО, куда оформлено направление
        /// Код МО - юридического лица. Заполняется в соответствии со справочником F003 Приложения А.
        /// Заполнение обязательно только в случаях оформления направления в случае
        /// подозрения на ЗНО: на консультацию в другую МО или на обследование (NAZ_R={2,3} и DS_ONK=1)
        /// </summary>
        public string TargetClinic;
    }

    /// <summary>
    /// Направление по итогам диспансеризации для D3 (ZL_LIST/ZAP/Z_SL/SL/NAZ)
    /// <remarks>
    /// Вложен в
    ///     Event SL
    /// </remarks>
    /// </summary>
    class DispAssignment {
        /// <summary>
        /// Номер по порядку
        /// </summary>
        public int Index { get; set; }
        
        /// <summary>
        /// Вид назначения
        /// Заполняется при присвоении группы здоровья, кроме I и II.
        /// 1 - направлен на консультацию в медицинскую организацию по месту прикрепления;
        /// 2 - направлен на консультацию в иную медицинскую организацию;
        /// 3 - направлен на обследование;
        /// 4 - направлен в дневной стационар;
        /// 5 - направлен на госпитализацию;
        /// 6 - направлен в реабилитационное отделение.
        /// </summary>
        public int RouteCode { get; set; }
        
        /// <summary>
        /// Специальность врача
        /// Заполняется, если в поле NAZ_R проставлены коды 1 или 2.
        /// Классификатор V021.
        /// </summary>
        public string DoctorSpeciality { get; set; }
        
        /// <summary>
        /// Вид обследования
        /// Заполняется, если в поле NAZ_R проставлен код 3.
        /// 1 - лабораторная диагностика;
        /// 2 - инструментальная диагностика;
        /// 3 - методы лучевой диагностики, за исключением дорогостоящих;
        /// 4 - дорогостоящие методы лучевой диагностики (КТ, МРТ, ангиография)
        /// </summary>
        public string StudyKind { get; set; }

        /// <summary>
        /// Медицинская услуга (код), указанная в направлении
        /// Заполняется, в соответствии с номенклатурой медицинских услуг (V001) только
        /// при направлении на обследование в случае подозрения на ЗНО (NAZ_R=3 и DS_ONK=1)
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// Дата направления
        /// Заполнение обязательно только в случаях оформления направления в случае подозрения
        /// на ЗНО: на консультацию в другую МО или на обследование (NAZ_R={2,3} и DS_ONK=1)
        /// </summary>
        public DateTime DirectionDate { get; set; }

        /// <summary>
        /// Код МО, куда оформлено направление
        /// Код МО - юридического лица. Заполняется в соответствии со справочником F003 Приложения А.
        /// Заполнение обязательно только в случаях оформления направления в случае
        /// подозрения на ЗНО: на консультацию в другую МО или на обследование (NAZ_R={2,3} и DS_ONK=1)
        /// </summary>
        public string TargetClinic { get; set; }

        /// <summary>
        /// Профиль медицинской помощи
        /// Заполняется, если в поле NAZ_R проставлены коды 4 или 5.
        /// Классификатор V002.
        /// </summary>
        public string AidProfile { get; set; }
        
        /// <summary>
        /// Профиль койки
        /// Заполняется, если в поле NAZ_R проставлен код 6.
        /// Классификатор V020.
        /// </summary>
        public string BedProfile { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="number">Порядковый номер направления</param>
        /// <param name="code">Код направления NAZ_R</param>
        /// <param name="value">Код назначения, определяемый значением NAZ_R</param>
        DispAssignment(int number, int code, string value, NeoSuspectDirection neoSuspection) {
            Index = number;
            RouteCode = code;

            switch (code) {
                case 1:
                case 2:
                    DoctorSpeciality = value;
                    break;

                case 3:
                    StudyKind = value;
                    break;

                case 4:
                case 5:
                    AidProfile = value;
                    break;

                case 6:
                    BedProfile = value;
                    break;

                default:
                    Lib.Logger.Log("Неверный код направления по результату диспансеризации: " + code.ToString());
                    break;
            }

            if (neoSuspection != null && neoSuspection.Suspected) {
                switch (RouteCode) {
                    case 2:
                        DirectionDate = neoSuspection.DirectionDate;
                        TargetClinic = neoSuspection.TargetClinic;
                        ServiceCode = neoSuspection.ServiceCode;
                        break;

                    case 3:
                        TargetClinic = string.Empty;
                        ServiceCode = neoSuspection.ServiceCode;
                        break;

                    default:
                        TargetClinic = string.Empty;
                        ServiceCode = string.Empty;
                        break;
                }
            }
        }

        public void Write(Lib.XmlExporter xml) {
            xml.Writer.WriteStartElement("NAZ");
#if FOMS
            xml.Writer.WriteElementString("NAZ_N", Index.ToString());
#else
            // Здесь опечатка? В приказе тут есть пробел: NAZ N
            xml.Writer.WriteElementString("NAZN", Index.ToString());
#endif
            xml.Writer.WriteElementString("NAZ_R", RouteCode.ToString());
            xml.WriteIfValid("NAZ_SP", DoctorSpeciality);
            xml.WriteIfValid("NAZ_V", StudyKind);

            xml.WriteIfValid("NAZ_USL", ServiceCode);

            if (!string.IsNullOrEmpty(TargetClinic)) {
                xml.Writer.WriteElementString("NAPR_DATE", DirectionDate.AsXml());
                xml.Writer.WriteElementString("NAPR_MO", TargetClinic);
            }

            xml.WriteIfValid("NAZ_PMP", AidProfile);
            xml.WriteIfValid("NAZ_PK", BedProfile);

            xml.Writer.WriteEndElement();
        }

        /// <summary>
        /// Get collection of dispanserisation bound directions
        /// </summary>
        /// <param name="KSG">Direction code (МЭС1, PAT.KSG)</param>
        /// <param name="KSG2">Direction elaboration (МЭС2, KSG2)</param>
        /// <param name="neoSuspection">Onkology suspection direction's parameters</param>
        public static IEnumerable<DispAssignment> Make(string KSG, string KSG2, NeoSuspectDirection neoSuspection) {
            int number = 0;
            int code;

            string[] codes = KSG.Split('-');
            string[] values = KSG2.Split('-');

            int j = Math.Min(codes.Length, values.Length);
            for (int i = 0; i < j; ++i) {
                if (int.TryParse(codes[i], out code)) {
                    ++number;
                    yield return new DispAssignment(number, code, values[i], neoSuspection);
                }
            }
        }
    }
}
