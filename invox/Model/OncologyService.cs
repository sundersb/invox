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
        CytostaticAndRay = 4,// Химиолучевая терапия
        Unspecific = 5,      // Неспецифическое лечение 
        Diagnostics = 6      // Диагностика
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
        /// Тип услуги
        /// Заполняется в соответствии со справочником N013 Приложения А.
        /// </summary>
        public N013 ServiceType { get; set; }

        /// <summary>
        /// Тип хирургического лечения
        /// При USL_TIP=1 заполняется в соответствии со справочником N014 Приложения А.
        /// Не подлежит заполнению при USL_TIP <> 1.
        /// </summary>
        public N014 SurgicalCure { get; set; }

        /// <summary>
        /// Линия лекарственной терапии
        /// При USL_TIP=2 заполняется в соответствии со справочником N015 Приложения А.
        /// Не подлежит заполнению при USL_TIP не равном 2
        /// </summary>
        public N015 Line { get; set; }

        /// <summary>
        /// Цикл лекарственной терапии
        /// При USL_TIP=2 заполняется в соответствии со справочником N016 Приложеиня А.
        /// Не подлежит заполнению при USL_TIP не равном 2
        /// </summary>
        public N016 Cycle { get; set; }

        /// <summary>
        /// Признак проведения профилактики тошноты и рвотного рефлекса
        /// Указывается значение "1" в случае применения противорвотной терапии при проведении
        /// лекарственной противоопухолевой или химиолучевой терапии препаратом высоко-,
        /// средне- или низкоэметогенного потенциала
        /// </summary>
        public bool CounterVomitCure { get; set; }

        /// <summary>
        /// Тип лучевой терапии
        /// При USL_TIP=3 или USL_TIP=4 заполняется в соответствии со справочником N017 Приложения А.
        /// Не подлежит заполнению при USL_TIP не равном 3 или 4
        /// </summary>
        public N017 RayKind { get; set; }

        public IEnumerable<OncologyDrug> Drugs { get; set; }

        public void Write(Lib.XmlExporter xml, Data.IInvoice pool) {
            xml.Writer.WriteStartElement("ONK_USL");
            
            xml.Writer.WriteElementString("USL_TIP", ((int)ServiceType).ToString());

            if (SurgicalCure != N014.None)
                xml.Writer.WriteElementString("HIR_TIP", ((int)SurgicalCure).ToString());

            if (Line != N015.None)
                xml.Writer.WriteElementString("LEK_TIP_L", ((int)Line).ToString());

            if (Cycle != N016.None)
                xml.Writer.WriteElementString("LEK_TIP_V", ((int)Cycle).ToString());

            if (Drugs != null) {
                foreach (OncologyDrug d in Drugs)
                    d.Write(xml);
            }

            if (CounterVomitCure)
                xml.Writer.WriteElementString("PPTR", "1");

            if (RayKind != N017.None)
                xml.Writer.WriteElementString("LUCH_TIP", ((int)RayKind).ToString());
            
            xml.Writer.WriteEndElement();
        }
    }
}
