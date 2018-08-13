using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace civox.Model {
    enum OnkologyReason : int {
        None = 0,
        Relapse = 1,
        Progression = 2
    }

    /// <summary>
    /// Приказ 59 от 30.03.2018 - Онкология
    /// </summary>
    class OnkologyTreat: Model {
        OnkologyReason reason;
        string stage;
        string tumor;
        string nodus;
        string mts;
        bool remoteMts;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="mes1">Блянский код МЭС1 из релакса</param>
        public OnkologyTreat(string mes1) {
            string[] parts = mes1.Split('-');
            if (parts.Count() > 4) {
                stage = parts[1];
                tumor = parts[2];
                nodus = parts[3];
                mts = parts[4];
            } else if (parts.Count() == 4) {
                stage = parts[0];
                tumor = parts[1];
                nodus = parts[2];
                mts = parts[3];
            } else {
                stage = "00";
                tumor = "0";
                nodus = "0";
                mts = "0";
            }
            remoteMts = false;
            
            char i = stage[0];
            switch (i) {
                case '1':
                    reason = OnkologyReason.Relapse;
                    break;

                case '2':
                    reason = OnkologyReason.Progression;
                    break;

                default:
                    reason = OnkologyReason.None;
                    break;
            }
            stage = stage.Substring(1);
        }

        static bool IsSuppOnkology(string ds) {
            if (ds.First() == 'C') {
                int i;
                if (int.TryParse(ds.Substring(1, 2), out i))
                    return (i >= 0 && i <= 80) || (i == 97);
                else
                    return false;
            } else return false;
        }

        /// <summary>
        /// Нужно ли заполнять раздел SL.ONK_SL?
        /// </summary>
        public static bool IsOnkologyTreat(Recourse recourse, string policy, Data.IInvoice repo) {
            if (recourse.SuspNeo) return false;
            
            if (recourse.Diagnosis.First() == 'C') return true;
            
            if (recourse.Diagnosis.Substring(0, 3) == "D70")
                return repo.GetPersonDiagnoses(policy).Any(IsSuppOnkology);

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="resultingService"></param>
        /// <param name="repo"></param>
        public static void WriteTreat(Lib.XmlExporter xml, Service resultingService, Data.IInvoice repo) {
            OnkologyTreat treat = repo.GetOnkologyTreat(resultingService.ID);
            treat.Write(xml, repo);
        }

        public override void Write(Lib.XmlExporter xml, Data.IInvoice repo) {
            xml.Writer.WriteStartElement("ONK_SL");
            
            if (reason != OnkologyReason.None)
                xml.Writer.WriteElementString("DS1_T", ((int)reason).ToString());
            else
                xml.Writer.WriteElementString("DS1_T", string.Empty);

            xml.WriteIfValid("STAD", stage);
            xml.WriteIfValid("ONK_T", tumor);
            xml.WriteIfValid("ONK_N", nodus);
            xml.WriteIfValid("ONK_M", mts);
            if (remoteMts) xml.Writer.WriteElementString("MTSTZ", "1");

            // TODO:
            //<B_DIAG>	УМ	S	Диагностический блок	Содержит сведения о проведенных исследованиях и их результатах
            //<B_PROT>	УМ	S	Сведения об имеющихся противопоказаниях и отказах	Заполняется в случае наличия противопоказаний к проведению определенных типов лечения или отказах пациента от проведения определенных типов лечения
            //<SOD>	У	N(3.2)	Суммарная очаговая доза	Обязательно для заполнения при проведении лучевой или химиолучевой 

            xml.Writer.WriteEndElement();
        }
    }
}
