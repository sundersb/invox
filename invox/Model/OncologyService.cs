using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace invox.Model {
    /// <summary>
    /// Тип услуги (онкология)
    /// </summary>
    enum N013 : int {
        None = 0,
        Surgery = 1,         // Хирургическое лечение
        Medicamentous = 2,   // Лекарственная противоопухолевая терапия
        Ray = 3,             // Лучевая терапия
        CytostaticAndRay = 4 // Химиолучевая терапия
    }

    /// <summary>
    /// Тип хирургического лечения (онкология)
    /// </summary>
    enum N014 : int {
        None = 0,
        Primary = 1,     // Первичной опухоли, в том числе с удалением регионарных лимфатических узлов
        Metastases = 2,  // Метастазов
        Symptomatic = 3, // Симптоматическое
        Staging = 4,     // Выполнено хирургическое стадирование (может указываться при раке яичника вместо "1")
        Nodules = 5      // Регионарных лимфатических узлов без первичной опухоли
    }

    /// <summary>
    /// Линия лекарственной терапии
    /// </summary>
    enum N015 : int {
        None = 0,
        FirstLine = 1,     // Первая линия
        SecondLine = 2,    // Вторая линия
        ThirdLine = 3,     // Третья линия
        AfterThirdLine = 4 // Линия после третьей
    }

    /// <summary>
    /// Цикл лекарственной терапии
    /// </summary>
    enum N016 : int {
        None = 0,
        FirstCycle = 1,       // Первый цикл линии
        FollowingCycle = 2,   // Последующие циклы линии (кроме последнего)
        LastCycleAborted = 3, // Последний цикл линии (лечение прервано)
        LastCycleDone = 4     // Последний цикл линии (лечение завершено)
    }

    /// <summary>
    /// Тип лучевой терапии
    /// </summary>
    enum N017 {
        None = 0,
        Primary = 1,       // Первичной опухоли / ложа опухоли
        Metastases = 2,    // Метастазов
        Symptomatical = 3  // Симптоматическая
    }

    /// <summary>
    /// Сведения об услуге при лечении онкологического заболевания (ZL_LIST/ZAP/Z_SL/SL/USL/ONK_USL)
    /// <remarks>
    /// Вложен в
    ///     Service USL (Single)
    /// </remarks>
    /// </summary>
    class OncologyService {
        /// <summary>
        /// Сведения о проведении консилиума
        /// </summary>
        public enum ConsiliumType : int {
            None = 0,
            Examination = 1,   // определена тактика обследования,
            Curation = 2,      // определена тактика лечения,
            CurationUpdate = 3 // изменена тактика лечения
        }

        ConsiliumType consilium;
        N013 serviceType;
        N014 surgicalCure;
        N015 line;
        N016 cycle;
        N017 rayKind;

        /// <summary>
        /// Сведения о проведении консилиума
        /// Заполняется в случае проведения консилиума в целях определения тактики обследования или лечения
        /// </summary>
        public ConsiliumType Consilium { get { return consilium; } }

        /// <summary>
        /// Тип услуги
        /// Заполняется в соответствии со справочником N013 Приложения А.
        /// </summary>
        public N013 ServiceType { get { return serviceType; } }

        /// <summary>
        /// Тип хирургического лечения
        /// При USL_TIP=1 заполняется в соответствии со справочником N014 Приложения А.
        /// Не подлежит заполнению при USL_TIP <> 1.
        /// </summary>
        public N014 SurgicalCure { get { return surgicalCure; } }

        /// <summary>
        /// Линия лекарственной терапии
        /// Заполняется при лекарственной терапии в соответствии со справочником N015
        /// </summary>
        public N015 Line { get { return line; } }

        /// <summary>
        /// Цикл лекарственной терапии
        /// </summary>
        public N016 Cycle { get { return cycle; } }

        /// <summary>
        /// Тип лучевой терапии
        /// Заполняется при лучевой или химиолучевой терапии в соответствии со справочником N017 Приложения А.
        /// Не подлежит заполнению при USL_TIP=1.
        /// </summary>
        public N017 RayKind { get { return rayKind; } }

        public void Write(Lib.XmlExporter xml) {
            xml.Writer.WriteStartElement("ONK_USL");

            if (consilium != ConsiliumType.None)
                xml.Writer.WriteElementString("PR_CONS", ((int)consilium).ToString());

            xml.Writer.WriteElementString("USL_TIP", ((int)serviceType).ToString());

            if (surgicalCure != N014.None)
                xml.Writer.WriteElementString("HIR_TIP", ((int)surgicalCure).ToString());

            if (line != N015.None)
                xml.Writer.WriteElementString("LEK_TIP_L", ((int)line).ToString());

            if (cycle != N016.None)
                xml.Writer.WriteElementString("LEK_TIP_V", ((int)cycle).ToString());

            if (rayKind != N017.None)
                xml.Writer.WriteElementString("LUCH_TIP", ((int)rayKind).ToString());
            
            xml.Writer.WriteEndElement();
        }
    }
}
