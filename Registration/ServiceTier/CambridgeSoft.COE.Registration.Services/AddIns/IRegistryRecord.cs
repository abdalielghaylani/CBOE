using System;
using System.Collections.Generic;
using CambridgeSoft.COE.Registration.Services.Types;

namespace CambridgeSoft.COE.Registration.Services.AddIns {
    public interface IRegistryRecord {
        BatchList BatchList { get; }
        ComponentList ComponentList { get; }

        int ID { get; set; }
        void InitializeFromXml(string xml, bool isNew, bool isClean);
        bool IsDirty { get; }
        bool IsTemporal { get; }
        bool IsValid { get; }
        bool IsNew { get;}
        bool IsDirectReg { get;}
       
        DuplicateCheck CheckDuplicates { get;}
        RegistryRecord.DataAccessStrategy DataStrategy { get;}
        RegNumber RegNumber { get; }
        System.Collections.Generic.Dictionary<string, string[]> RegsAffectedByCompoundUpdate { get; }
        string StructureAggregation { get; set; }
        void UpdateXml();
        void SetDuplicateResponse(string duplicateResponseXml);
        void SetCustomDuplicateResponse(string customDuplicateResponseXml);
        string Xml { get; set; }        
        string XmlWithAddIns { get; }
        string XmlWithAddInsWithoutUpdate { get; }
        string FoundDuplicates { get; set; }
        bool SameBatchesIdentity { get;}
        bool AllowUnregisteredComponents { get;}

        bool SubmitCheckRedBoxWarning { get; set; }
        ChemDrawWarningChecker.ModuleName ModuleName { get; set; }
        bool RegisterCheckRedBoxWarning { get; set; }
        string RedBoxWarning { get; set; }
        void CheckValidationRedBoxWarningRule();

        List<DuplicateCheckResponse> FindDuplicates(RegistryRecord.DataAccessStrategy strategy);
    }
}
