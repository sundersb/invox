﻿using System;
using System.Linq;
using invox.Lib;
using System.Collections.Generic;

namespace invox.Model {
    /// <summary>
    /// Диагностический блок
    /// Содержит сведения о проведенных исследованиях и их результатах
    /// </summary>
    class OnkologyDiagnosticType {
        public enum DiagnosticType : int {
            Histologycal = 1,
            Marker = 2
        }

        DateTime date;
        DiagnosticType type;
        string code;
        string result;

        /// <summary>
        /// Дата взятия материала
        /// Указывается дата взятия материала для проведения диагностики
        /// </summary>
        public DateTime Date { get { return date; } }

        /// <summary>
        /// Тип диагностического показателя
        /// 1 - гистологический признак; 2 - маркер (ИГХ)
        /// </summary>
        public DiagnosticType Type { get { return type; } }

        /// <summary>
        /// Код диагностического показателя
        /// При DIAG_TIP=1 заполняется в соответствии со справочником N007 Приложения А.
        /// При DIAG_TIP=2 заполняется в соответствии со справочником N010 Приложения А.
        /// </summary>
        public string Code { get { return code; } }

        /// <summary>
        /// Код результата диагностики
        /// При DIAG_TIP=1 заполняется в соответствии со справочником N008 Приложения А.
        /// При DIAG_TIP=2 заполняется в соответствии со справочником N011 Приложения А.
        /// </summary>
        public string Result { get { return result; } }

        public void Write(Lib.XmlExporter xml) {
            xml.Writer.WriteStartElement("B_DIAG");

            xml.Writer.WriteElementString("DIAG_DATE", date.AsXml());
            xml.Writer.WriteElementString("DIAG_TIP", ((int)type).ToString());
            xml.Writer.WriteElementString("DIAG_CODE", code);

            if (!string.IsNullOrEmpty(result)) {
                xml.Writer.WriteElementString("DIAG_RSLT", result);
                xml.Writer.WriteElementString("REC_RSLT", "1");
            }

            xml.Writer.WriteEndElement();
        }
    }
    
    /// <summary>
    /// Справочник противопоказаний и отказов от лечения онкологического заболевания
    /// </summary>
    enum N001 : int {
        Hystological = 0,        // Отказ от проведения гистологического исследования
        Surgical = 1,            // Противопоказания к проведению хирургического лечения
        Drug = 2,                // Противопоказания к проведению химиотерапевтического лечения	
        Ray = 3,                 // Противопоказания к проведению лучевой терапии	
        RefuseSurgical = 4,      // Отказ от проведения хирургического лечения
        RefuseDrug = 5,          // Отказ от проведения химиотерапевтического лечения
        RefuseRay = 6,           // Отказ от проведения лучевой терапии
        HystNotIndicated = 7,    // Гистологическое подтверждение диагноза не показано
        HystCounterindicated = 8 // Противопоказания к проведению гистологического исследования
    }

    /// <summary>
    /// Сведения об имеющихся противопоказаниях и отказах
    /// Заполняется в случае наличия противопоказаний к проведению определенных типов лечения или отказах
    /// пациента от проведения определенных типов лечения
    /// </summary>
    class OnkologyRefusal {
        N001 code;
        DateTime date;

        /// <summary>
        /// Код противопоказания или отказа
        /// Заполняется в соответствии со справочником N001 Приложения А.
        /// </summary>
        public N001 Code { get { return code; } }

        /// <summary>
        /// Дата регистрации противопоказания или отказа
        /// </summary>
        public DateTime Date { get { return date; } }

        public void Write(Lib.XmlExporter xml) {
            xml.Writer.WriteStartElement("B_PROT");

            xml.Writer.WriteElementString("PROT", ((int)code).ToString());
            xml.Writer.WriteElementString("D_PROT", date.AsXml());

            xml.Writer.WriteEndElement();
        }
    }

    /// <summary>
    /// N018 Повод обращения по поводу онкологического заболевания
    /// </summary>
    enum OnkologyReason : int {
        Primary = 0,               // Первичное лечение
        Relapse = 1,               // Лечение при рецидиве
        Progression = 2,           // Лечение при прогрессировании
        Supervision = 3,           // Динамическое наблюдение
        SupervisionRemission = 4,  // Диспансерное наблюдение (здоров/ремиссия)
        Diagnostic = 5,            // Диагностика (при отсутствии специфического лечения)
        SymptomaticalCure = 6      // Симптоматическое лечение
    }

    /// <summary>
    /// Приказ 59 от 30.03.2018 - Онкология ZL_LIST/ZAP/SL/Z_SL/ONK_SL
    /// <remarks>Сведения о случае лечения онкологического заболевания
    /// Обязательно для заполнения при установленном основном диагнозе злокачественного новообразования
    /// (первый символ кода диагноза по МКБ-10 - "С") и нейтропении (код диагноза по МКБ-10 D70
    /// с сопутствующим диагнозом C00-C80 или C97),
    /// если (USL_OK не равен 4 и REAB не равен 1 и DS_ONK не равен 1)
    ///</remarks>
    /// </summary>
    class OnkologyTreat {
        OnkologyReason reason;
        string stage;
        string tumor;
        string nodus;
        string mts;
        bool remoteMts;

        float beamLoad = -1;

        /// <summary>
        /// Повод обращения
        /// 1 - рецидив; 2 - прогрессирование
        /// </summary>
        public OnkologyReason Reason { get { return reason; } }

        /// <summary>
        /// Стадия заболевания
        /// Заполняется в соответствии со справочником N002 Приложения А
        /// </summary>
        public string Stage { get { return stage; } }

        /// <summary>
        /// Значение Tumor
        /// Заполняется в соответствии со справочником N003 Приложения А
        /// </summary>
        public string Tumor { get { return tumor; } }

        /// <summary>
        /// Значение Nodus
        /// Заполняется в соответствии со справочником N004 Приложения А
        /// </summary>
        public string Nodus { get { return nodus; } }

        /// <summary>
        /// Значение Metastasis
        /// Заполняется в соответствии со справочником N005 Приложения А
        /// </summary>
        public string Metastasis { get { return mts; } }

        /// <summary>
        /// Признак выявления отдаленных метастазов
        /// Подлежит заполнению значением 1 при выявлении отдаленных метастазов только при DS1_T=1 или DS1_T=2
        /// </summary>
        public bool RemoteMetastases { get { return remoteMts; } }

        /// <summary>
        /// Суммарная очаговая доза
        /// Обязательно для заполнения при проведении лучевой или химиолучевой терапии (USL_TIP=3 или USL_TIP=4)
        /// </summary>
        public float BeamLoad { get { return beamLoad; } }

        /// <summary>
        /// Список услуг по онкологии
        /// </summary>
        public IEnumerable<OncologyService> Services { get; set; }

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

            if (!string.IsNullOrEmpty(stage)) {
                int i = (int)stage.First() - (int)'0';
                if (i < 3)
                    reason = (OnkologyReason)i;
                else
                    reason = OnkologyReason.Primary;

                stage = stage.Substring(1);
            }
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
        public static bool IsOnkologyTreat(Recourse rec, Event e, Data.IInvoice pool) {
            if (rec.SuspectOncology) return false;

            if (e.MainDiagnosis.First() == 'C') return true;

            if (e.MainDiagnosis.Substring(0, 2) == "D0") return true;

            if (e.MainDiagnosis.Substring(0, 3) == "D70")
                return pool.LoadPersonDiagnoses().Any(IsSuppOnkology);

            return false;
        }

        public void Write(Lib.XmlExporter xml, Data.IInvoice pool) {
            xml.Writer.WriteStartElement("ONK_SL");

            xml.Writer.WriteElementString("DS1_T", ((int)reason).ToString());

            xml.WriteIfValid("STAD", stage);
            xml.WriteIfValid("ONK_T", tumor);
            xml.WriteIfValid("ONK_N", nodus);
            xml.WriteIfValid("ONK_M", mts);
            
            if (reason == OnkologyReason.Relapse || reason == OnkologyReason.Progression)
                xml.WriteBool("MTSTZ", remoteMts);

            if (beamLoad > 0)
                xml.Writer.WriteElementString("SOD", beamLoad.ToString("F2", Options.NumberFormat));

            foreach (OnkologyDiagnosticType dt in pool.LoadOnkologicalDiagnosticTypes())
                dt.Write(xml);

            foreach (OnkologyRefusal r in pool.LoadOnkologicalRefusal())
                r.Write(xml);

            if (Services != null) {
                foreach (OncologyService s in Services)
                    s.Write(xml, pool);
            }

            xml.Writer.WriteEndElement();
        }
    }
}
