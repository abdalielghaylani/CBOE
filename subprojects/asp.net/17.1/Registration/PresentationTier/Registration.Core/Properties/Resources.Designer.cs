﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CambridgeSoft.COE.Registration.Core.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CambridgeSoft.COE.Registration.Core.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
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
        ///   Looks up a localized string similar to 
        ///SELECT
        ///  t.LOG_ID
        ///  , t.DUPLICATE_ACTION
        ///  , t.Description
        ///  , t.USER_ID
        ///  , t.DATETIME_STAMP
        ///FROM regdb.log_bulkregistration_id t
        ///.
        /// </summary>
        internal static string GetLogHeaderSql {
            get {
                return ResourceManager.GetString("GetLogHeaderSql", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///UPDATE 
        ///   regdb.VW_PICKLISTDOMAIN set LOCKED = :locked where ID = :id
        ///.
        /// </summary>
        internal static string GetPickListDomainLockSql {
            get {
                return ResourceManager.GetString("GetPickListDomainLockSql", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///SELECT
        ///  rn.regnumber
        ///FROM vw_registrynumber rn
        ///  inner join vw_mixture m on m.regid = rn.regid
        ///  inner join vw_mixture_component mc on mc.mixtureid = m.mixtureid
        ///  inner join vw_compound c on c.compoundid = mc.compoundid
        ///.
        /// </summary>
        internal static string GetRecordsThatContainsStructure {
            get {
                return ResourceManager.GetString("GetRecordsThatContainsStructure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///SELECT
        ///  regnumber
        ///FROM regdb.vw_mixture_regnumber vmixr
        ///  JOIN regdb.vw_batch vb on vb.regid = vmixr.regid
        ///.
        /// </summary>
        internal static string GetRegNumberByBatchIdSql {
            get {
                return ResourceManager.GetString("GetRegNumberByBatchIdSql", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///SELECT
        ///  s.sequence_id as &quot;ID&quot;
        ///  , s.prefix
        ///  , s.prefix_delimiter
        ///  , s.next_in_sequence
        ///  , s.regnumber_length
        ///  , to_char(s.active) as active
        ///  , s.batchdelimeter
        ///  --, dup_check_local
        ///  --, suffix
        ///  , s.suffixdelimiter
        ///  , s.saltsuffixtype
        ///  --, objecttype
        ///  --, example
        ///  --, siteid -- too limiting! one site per Prefix?
        ///  , to_char(s.type) as type
        ///  , batchnumber_length
        ///FROM sequence s
        ///.
        /// </summary>
        internal static string GetRegNumberSequencerSql {
            get {
                return ResourceManager.GetString("GetRegNumberSequencerSql", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///SELECT
        ///  s.sequenceid as ID
        ///  , s.prefix
        ///  , s.prefixdelimiter
        ///  , s.regnumberlength
        ///  , s.suffixdelimiter
        ///  , s.saltsuffixtype
        ///  , s.batchnumlength
        ///  , s.nextinsequence
        ///  , s.example
        ///  , s.autoselcompdupchk
        ///  , to_char(s.active) as active
        ///  , to_char(s.type) as type
        ///FROM regdb.vw_sequence s
        ///.
        /// </summary>
        internal static string GetSequenceSql {
            get {
                return ResourceManager.GetString("GetSequenceSql", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///SELECT ID, TYPE, VALUE, MOLWEIGHT, FORMULA FROM (
        ///  SELECT
        ///    s.structureid as ID
        ///    , s.structureformat as TYPE
        ///    , s.structure as VALUE
        ///    , CSCartridge.MolWeight(s.structure) as MOLWEIGHT
        ///    , CSCartridge.Formula(s.structure, &apos;&apos;) as FORMULA
        ///  FROM regdb.vw_structure s
        ///  UNION ALL
        ///  SELECT
        ///    -1 * d.id as ID
        ///    , d.structureformat as TYPE
        ///    , d.default_drawing as VALUE
        ///    , 0 as MOLWEIGHT
        ///    , NULL as FORMULA
        ///  FROM drawing_type d
        ///) T
        ///.
        /// </summary>
        internal static string GetStructureSql {
            get {
                return ResourceManager.GetString("GetStructureSql", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///SELECT
        ///  count(*)
        ///FROM regdb.vw_temporarybatch vtb
        ///  INNER JOIN coedb.coesavedhitlist hl on hl.id = vtb.tempbatchid
        ///.
        /// </summary>
        internal static string GetTempRecordCount {
            get {
                return ResourceManager.GetString("GetTempRecordCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error occurs when trying to create Picklist from xml, please check the format of the xml..
        /// </summary>
        internal static string Picklist_InvalidXmlInput {
            get {
                return ResourceManager.GetString("Picklist_InvalidXmlInput", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to REGDB.
        /// </summary>
        internal static string RegistrationDatabaseName {
            get {
                return ResourceManager.GetString("RegistrationDatabaseName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SEQ_TEMPORARY_BATCH.
        /// </summary>
        internal static string Seq_Temporary_Batch {
            get {
                return ResourceManager.GetString("Seq_Temporary_Batch", resourceCulture);
            }
        }
    }
}