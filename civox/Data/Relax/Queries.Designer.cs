﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace civox.Data.Relax {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Queries {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Queries() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("civox.Data.Relax.Queries", typeof(Queries).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select distinct
        ///  S.RECID,
        ///  S.D_U,
        ///  S.COD,
        ///  S.C_I,
        ///  S.K_U,
        ///  S.S_ALL,
        ///  S.D_TYPE,
        ///  S.TN1,
        ///  S.BE,
        ///  nvl(RO.SLIZ, &apos;xxx&apos;) RESCODE,
        ///  K.MSP,
        ///  nvl(MP.CODEFSS, &apos;xxx&apos;) PROFILE,
        ///  K.OPL,
        ///  S.EXTR
        /// from {period}S{lpu} S
        ///  left outer join BASE/COMMON/KMU K on cast (K.CODE as int) = S.COD
        ///  left outer join BASE/DESCR/STRUCT X on X.BUXC = S.OTD
        ///  left outer join BASE/DESCR/MEDPERS M on M.PODR + M.CODE = X.CODE + S.TN1
        ///  left outer join BASE/COMMON/MEDPOST MP on MP.CODE = M.POST
        ///  left outer j [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string SELECT_CASE_TREAT {
            get {
                return ResourceManager.GetString("SELECT_CASE_TREAT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select distinct DIAGIN from {period}DIAGNOZ where SN_POL = ?.
        /// </summary>
        internal static string SELECT_DIAGNOSES {
            get {
                return ResourceManager.GetString("SELECT_DIAGNOSES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select KSG, KSG2
        /// from {period}S{lpu}
        /// where RECID = ?.
        /// </summary>
        internal static string SELECT_DISP_DIRECTIONS {
            get {
                return ResourceManager.GetString("SELECT_DISP_DIRECTIONS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select distinct
        ///  S.OTD,
        ///  S.TN1
        /// from {period}S{lpu} S
        ///  left outer join BASE/DESCR/STRUCT X on X.BUXC = S.OTD
        ///  left outer join BASE/DESCR/MEDPERS M on M.PODR + M.CODE = X.CODE + S.TN1
        ///  left outer join BASE/COMMON/MEDPOST MP on MP.CODE = M.POST
        /// where MP.CODEFSS is null.
        /// </summary>
        internal static string SELECT_ERROR_PRVS {
            get {
                return ResourceManager.GetString("SELECT_ERROR_PRVS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select distinct
        ///  P.RECID,
        ///  P.T_POL,
        ///  cpconvert(866,1251,P.SN_POL) SN_POL,
        ///  P.Q,
        ///  P.KT,
        ///  P.W,
        ///  P.DR,
        ///  P.NOVOR
        /// from {period}P{lpu} P
        ///  join {period}S{lpu} S on S.SN_POL = P.SN_POL
        /// order by 3.
        /// </summary>
        internal static string SELECT_INVOICE_RECS {
            get {
                return ResourceManager.GetString("SELECT_INVOICE_RECS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select distinct
        ///  cpconver(866, 1251, P.SN_POL) SN_POL,
        ///  P.RECID,
        ///  cpconvert(866, 1251, P.FAM) FAM,
        ///  cpconvert(866, 1251, P.IM) IM,
        ///  cpconvert(866, 1251, P.OT) OT,
        ///  P.W,
        ///  P.DR,
        ///  P.SS,
        ///  P.SP,
        ///  P.KT,
        ///  cpconvert(866, 1251, P.SN_POL) SN_POL,
        ///  cpconvert(866, 1251, P.ADRES) ADRES,
        ///  PP.Q_PASP,
        ///  PP.SN_PASP
        /// from {period}P{lpu} P
        ///  join {period}S{lpu} S on S.SN_POL = P.SN_POL
        ///  left outer join {period}PAT PP on PP.SN_POL = P.SN_POL
        /// order by 1.
        /// </summary>
        internal static string SELECT_PEOPLE {
            get {
                return ResourceManager.GetString("SELECT_PEOPLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select count(distinct P.SN_POL)
        /// from {period}P{lpu} P
        ///  join {period}S{lpu} S on S.SN_POL = P.SN_POL.
        /// </summary>
        internal static string SELECT_PEOPLE_COUNT {
            get {
                return ResourceManager.GetString("SELECT_PEOPLE_COUNT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select distinct
        ///  S.OTD,
        ///  icase(S.OTD = &apos;0001&apos;, 1,
        ///   S.OTD = &apos;0003&apos;, iif(S.COD = 3001 or S.COD = 3034, 2, 3),
        ///   S.OTD = &apos;0004&apos;, iif(S.COD = 50002 or S.COD = 50001 or floor(S.COD / 10000) = 6, 4, 5),
        ///   S.OTD = &apos;0005&apos;, 6,
        ///   S.OTD = &apos;0009&apos;, icase(floor(S.COD / 1000) = 27, 7, floor(S.COD / 1000) = 25 or floor(COD / 1000) = 28, 9, 8),
        ///   0) REASON,
        ///  S.DS,
        ///  max(D.F) F,
        ///  max(UMP.SLUSL) COND,
        ///  max(S.IG) IG
        /// from {period}S{lpu} S
        ///  left outer join {period}DIAGNOZ D on (D.SN_POL = S.SN_POL) and  [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string SELECT_RECOURSE_CASES {
            get {
                return ResourceManager.GetString("SELECT_RECOURSE_CASES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select count(*)
        /// from {period}S{lpu}
        /// where floor(COD/1000) in (3, 27, 28, 29, 22, 50).
        /// </summary>
        internal static string SELECT_RECOURSES_COUNT {
            get {
                return ResourceManager.GetString("SELECT_RECOURSES_COUNT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select sum(S_ALL) TOTAL from {period}S{lpu}.
        /// </summary>
        internal static string SELECT_TOTAL_TO_PAY {
            get {
                return ResourceManager.GetString("SELECT_TOTAL_TO_PAY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select TR from {period}S{lpu} where RECID = ?.
        /// </summary>
        internal static string SELECT_TRAUMA_CODE {
            get {
                return ResourceManager.GetString("SELECT_TRAUMA_CODE", resourceCulture);
            }
        }
    }
}
