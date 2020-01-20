﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace invox.Data.Relax {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("invox.Data.Relax.Queries", typeof(Queries).Assembly);
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
        ///   Ищет локализованную строку, похожую на select DIAG2, HR2, HRO2
        /// from {period}DIAGNOZ
        /// where SN_POL = ?
        ///  and OTD = ?
        ///  and DIAGOUT = ?
        ///  and DIAG2 &lt;&gt; &apos;&apos;.
        /// </summary>
        internal static string SELECT_CONCOMITANT_DISEASES {
            get {
                return ResourceManager.GetString("SELECT_CONCOMITANT_DISEASES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select KSG, KSG2
        /// from {period}S{lpu}
        /// where RECID = ?.
        /// </summary>
        internal static string SELECT_DISP_ASSIGNMENTS {
            get {
                return ResourceManager.GetString("SELECT_DISP_ASSIGNMENTS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select ST.BUXC + M.CODE OTD_TN1, max(MP.CODEFSS) CODEFSS
        /// from BASE/DESCR/STRUCT ST
        ///  join BASE/DESCR/MEDPERS M on M.PODR = ST.CODE
        ///  join BASE/COMMON/MEDPOST MP on MP.CODE = M.POST
        /// where MP.CODEFSS &lt;&gt; &apos;&apos;
        /// group by 1.
        /// </summary>
        internal static string SELECT_DOCTORS_SPECIALITY {
            get {
                return ResourceManager.GetString("SELECT_DOCTORS_SPECIALITY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select distinct
        ///  P.RECID,
        ///  P.T_POL,
        ///  cpconvert (866, 1251, P.SN_POL) SN_POL,
        ///  P.Q,
        ///  P.KT,
        ///  P.W,
        ///  P.DR,
        ///  P.NOVOR,
        ///  cpconvert (866, 1251, MSO.NAME) MSO_NAME,
        ///  MSO.OGRN MSO_OGRN,
        ///  MSO.OKATO MSO_OKATO
        /// from {period}S{lpu} S
        ///  join {period}P{lpu} P on P.SN_POL = S.SN_POL
        ///  join BASE/COMMON/SLMSO MSO on MSO.CODE = upper(P.Q)
        /// where ({section})
        ///  and ({recsvc}).
        /// </summary>
        internal static string SELECT_INVOICE_PEOPLE {
            get {
                return ResourceManager.GetString("SELECT_INVOICE_PEOPLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select distinct
        ///  P.RECID,
        ///  P.T_POL,
        ///  cpconvert (866, 1251, P.SN_POL) SN_POL,
        ///  P.Q,
        ///  PAT.KT,
        ///  P.W,
        ///  P.DR,
        ///  P.NOVOR,
        ///  cpconvert (866, 1251, MSO.FULLNAME) MSO_NAME,
        ///  MSO.OGRN MSO_OGRN,
        ///  MSO.OKATO MSO_OKATO
        /// from {period}C{lpu} S
        ///  join {period}I{lpu} P on P.SN_POL = S.SN_POL
        ///  join {period}PAT on PAT.SN_POL = I.SN_POL
        ///  left outer join BASE/COMMON/MSOIN MSO on MSO.CODE = P.Q_VCODE
        /// where ({section})
        ///  and ({recsvc}).
        /// </summary>
        internal static string SELECT_INVOICE_PEOPLE_FOREIGN {
            get {
                return ResourceManager.GetString("SELECT_INVOICE_PEOPLE_FOREIGN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select count(*)
        /// from {period}S{lpu} S
        /// where ({recsvc})
        ///  and ({section}).
        /// </summary>
        internal static string SELECT_INVOICE_RECORDS_COUNT {
            get {
                return ResourceManager.GetString("SELECT_INVOICE_RECORDS_COUNT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select distinct
        ///  &apos;Врач &apos; + ltrim(S.TN1) + &apos; не прописан в отделении &apos; + S.OTD EMSG
        /// from {period}S{lpu} S
        ///  left outer join BASE/DESCR/STRUCT X on X.BUXC = S.OTD
        ///  left outer join BASE/DESCR/MEDPERS M on M.PODR + M.CODE = X.CODE + S.TN1
        ///  left outer join BASE/COMMON/MEDPOST MP on MP.CODE = M.POST
        /// where MP.CODEFSS is null.
        /// </summary>
        internal static string SELECT_NO_DOCTOR_DEPT {
            get {
                return ResourceManager.GetString("SELECT_NO_DOCTOR_DEPT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select
        ///  &apos;Специальность: &quot;&apos;
        ///   + rtrim(NAME) + &apos;&quot; (&apos;
        ///   + cast (CNT as varchar(5)) + &apos; врачей)&apos;
        /// from (
        ///select
        ///  cpconvert(866,1251,MP.NAME) NAME, count(*) CNT
        /// from BASE/DESCR/MEDPERS M
        ///  join BASE/COMMON/MEDPOST MP on MP.CODE = M.POST
        /// where MP.CODEFSS = &apos;&apos;
        /// group by 1) A.
        /// </summary>
        internal static string SELECT_NO_SPECIALITY {
            get {
                return ResourceManager.GetString("SELECT_NO_SPECIALITY", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select S.BLOPEN DIRECTION_DATE, S.TR DIRECTION from {period}S{lpu} S where S.RECID = ?.
        /// </summary>
        internal static string SELECT_ONCO_DIRECTIONS {
            get {
                return ResourceManager.GetString("SELECT_ONCO_DIRECTIONS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select S.KSG from {period}S{lpu} S where S.RECID = ?.
        /// </summary>
        internal static string SELECT_ONKO_TREAT {
            get {
                return ResourceManager.GetString("SELECT_ONKO_TREAT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select distinct
        ///  P.RECID,
        ///  cpconvert(866, 1251, P.FAM) FAM,
        ///  cpconvert(866, 1251, P.IM) IM,
        ///  cpconvert(866, 1251, P.OT) OT,
        ///  P.W,
        ///  P.DR,
        ///  P.SS,
        ///  cpconver(866, 1251, P.SN_POL) SN_POL,
        ///  PP.Q_PASP,
        ///  cpconvert(866, 1251, PP.SN_PASP) SN_PASP,
        ///  cpconvert(866, 1251, P.PR) BP,
        ///  cpconvert(866, 1251, P.ADRES) ADRES,
        ///  P.SP,
        ///  P.KT,
        ///  cpconvert(866, 1251, P.FAM1) FAMP,
        ///  cpconvert(866, 1251, P.IM1) IMP,
        ///  cpconvert(866, 1251, P.OT1) OTP,
        ///  P.WP,
        ///  P.DRP
        /// from {period}P{lpu} P
        ///  join {p [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string SELECT_PEOPLE {
            get {
                return ResourceManager.GetString("SELECT_PEOPLE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select count(distinct SN_POL)
        /// from {period}S{lpu} S
        /// where ({section}) and ({recsvc}).
        /// </summary>
        internal static string SELECT_PEOPLE_COUNT {
            get {
                return ResourceManager.GetString("SELECT_PEOPLE_COUNT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select
        ///  P.RECID PERSON_ID,
        ///  S.RECID SERVICE_ID,
        ///  S.OTD DEPT, S.PODR UNIT,
        ///  K.MSP MSP,
        ///  K.PRUPP AID_PROFILE,
        ///  K.UMP PAY_KIND,
        ///  UMP.SLUSL AID_CONDITIONS,
        ///  S.COD SERVICE_CODE,
        ///  nvl(RO.SLIZ, &apos;xxx&apos;) RESULT,
        ///  S.IG OUTCOME,
        ///  K.OPL PAY_KIND,
        ///  1 PAY_TYPE,
        ///  .F. MOBILE_BRIGADE,
        ///  S.BE RECOURSE_RESULT,
        ///  ST.PROF BED_PROFILE,
        ///  K.LDET DET,
        ///  S.C_I CARD_NUMBER,
        ///  S.DS DS_MAIN,
        ///  .F. REHABILITATION,
        ///  &apos;V001&apos; INTERVENTION_KIND,
        ///  S.K_U QUANTITY,
        ///  S.S_ALL TARIFF,
        ///  S.S_ALL TOTAL,
        ///  S.OT [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string SELECT_RECOURSES {
            get {
                return ResourceManager.GetString("SELECT_RECOURSES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select cpconvert(866, 1251, NAME) from BASE/COMMON/KMU where CODE = .
        /// </summary>
        internal static string SELECT_SERVICE_NAME {
            get {
                return ResourceManager.GetString("SELECT_SERVICE_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select
        ///  S.RECID SERVICE_ID,
        ///  K.MSP AID_PROFILE,
        ///  S.COD SERVICE_CODE,
        ///  nvl(RO.SLIZ, &apos;xxx&apos;) RESULT, ST.PROF BED_PROFILE, S.IG OUTCOME,
        ///  1 QUANTITY,
        ///  S.S_ALL TARIFF,
        ///  S.S_ALL TOTAL,
        ///  S.OTD + S.TN1 SPECIALITY_ID,
        ///  S.TN1 DOCTOR_CODE,
        ///  S.D_U,
        ///  nvl(D.DBEG, DS.DBEG) DATE_FROM,
        ///  nvl(D.DEND, DS.DEND) DATE_TILL,
        ///  nvl(D.NOVOR, .F.) or nvl(DS.NOVOR, .F.) NOVOR,
        ///  DS.SRZ_MCOD DIRECTION_FROM,
        ///  DS.VESR BIRTH_WEIGHT,
        ///  DS.BOLEND OUTCOME1,
        ///  DS.INSTAC TRANSFER,
        ///  D.OBR REASON1,
        ///  nvl(DS.KD, S [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string SELECT_SERVICES {
            get {
                return ResourceManager.GetString("SELECT_SERVICES", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на select sum(S_ALL) TOTAL from {period}S{lpu} S where {section}.
        /// </summary>
        internal static string SELECT_TOTAL {
            get {
                return ResourceManager.GetString("SELECT_TOTAL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на update {period}PATU set DS2 = DS, DS = &apos;Z02.7&apos;
        /// where (DS like &apos;C%&apos;)
        ///  and (OTD = &apos;0004&apos;)
        ///  and ((COD in (50301, 50302, 50401, 50402))
        ///   or (floor(COD / 10000) = 6)).
        /// </summary>
        internal static string UPDATE_ONKO_OTHER_AIM_COMMON {
            get {
                return ResourceManager.GetString("UPDATE_ONKO_OTHER_AIM_COMMON", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на update {period}S{lpu} set DS2 = DS, DS = &apos;Z02.7&apos;
        /// where (DS like &apos;C%&apos;)
        ///  and (OTD = &apos;0004&apos;)
        ///  and ((COD in (50301, 50302, 50401, 50402))
        ///   or (floor(COD / 10000) = 6)).
        /// </summary>
        internal static string UPDATE_ONKO_OTHER_AIM_MAIN {
            get {
                return ResourceManager.GetString("UPDATE_ONKO_OTHER_AIM_MAIN", resourceCulture);
            }
        }
    }
}
