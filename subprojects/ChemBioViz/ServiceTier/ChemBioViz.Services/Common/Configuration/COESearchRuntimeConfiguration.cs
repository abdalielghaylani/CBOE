using System;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using CambridgeSoft.COE.Framework.Common.Messaging;

/// <summary>
/// Summary description for ChemBioVizSearchConfig
/// </summary>
public class COESearchRuntimeConfiguration : SerializableConfigurationSection
{
    #region variables
    public const string SectionName = "coeChemBioVizConfiguration";
    private const string _formGroupId = "FormGroupId";
    private const string _defaultSearchAction = "DefaultSearchAction";
    private const string _defaultQueryForm = "DefaultQueryForm";
    private const string _bufferSize = "BufferSize";
    private const string _gridPageSize = "GridPageSize";
    #endregion

    #region Properties
    [ConfigurationProperty(_formGroupId, IsRequired = true)]
    public int FormGroupId
    {
        get { return (int)base[_formGroupId]; }
        set { base[_formGroupId] = value; }
    }

    [ConfigurationProperty(_defaultSearchAction, IsRequired = false, DefaultValue = FormGroup.CurrentFormEnum.DetailForm)]
    public FormGroup.CurrentFormEnum DefaultSearchAction
    {
        get {
            return (FormGroup.CurrentFormEnum)Enum.Parse(typeof(FormGroup.CurrentFormEnum), base[_defaultSearchAction].ToString());
        }
        set { base[_defaultSearchAction] = value.ToString(); }
    }

    [ConfigurationProperty(_defaultQueryForm, IsRequired = false)]
    public int DefaultQueryForm
    {
        get { return (int)base[_defaultQueryForm]; }
        set { base[_defaultQueryForm] = value; }
    }

    [ConfigurationProperty(_bufferSize, IsRequired = false, DefaultValue=10)]
    public int BufferSize
    {
        get { return (int)base[_bufferSize]; }
        set { base[_bufferSize] = value; }
    }

    [ConfigurationProperty(_gridPageSize, IsRequired = false, DefaultValue = 10)]
    public int GridPageSize
    {
        get { return (int)base[_gridPageSize]; }
        set { base[_gridPageSize] = value; }
    }
    #endregion

}
