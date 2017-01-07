using System;
using CambridgeSoft.COE.Framework.Common;
using Resources;

/// <summary>
/// Summary description for ResultsCriteriaHelper
/// </summary>
public class ResultsCriteriaHelper {
    public static ResultsCriteria BuildCriteriaForSubmitMixture() {
        ResultsCriteria resultsCriteria = new ResultsCriteria();
        #region VW_COMPOUND
        ResultsCriteria.ResultsCriteriaTable tableCompound = new ResultsCriteria.ResultsCriteriaTable();
        tableCompound.Id = 1;

        ResultsCriteria.Field idField = new ResultsCriteria.Field(10);
        idField.Alias = "CompoundID";
        tableCompound.Criterias.Add(idField);

        ResultsCriteria.Field structureIdField = new ResultsCriteria.Field(12);
        structureIdField.Alias = "BASE64_CDX";
        tableCompound.Criterias.Add(structureIdField);

        ResultsCriteria.Field regIdField = new ResultsCriteria.Field(11);
        regIdField.Alias = "RegID";
        tableCompound.Criterias.Add(regIdField);

        ResultsCriteria.Field regNumField = new ResultsCriteria.Field(15);
        regNumField.Alias = "RegNum";
        tableCompound.Criterias.Add(regNumField);

        resultsCriteria.Tables.Add(tableCompound);
        #endregion

        return resultsCriteria;
    }
}
