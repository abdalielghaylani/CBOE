using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEFormService;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using Csla.Web;
using CambridgeSoft.COE.ChemBioViz.Services;
using CambridgeSoft.COE.ChemBioViz.Services.COEChemBioVizService;
using Csla.Core;
using CambridgeSoft.COE.Framework.GUIShell;

/// <summary>
/// Summary description for BasePage
/// </summary>
public class BasePage : GUIShellPage {
    protected COEFormGroup _formGroup;
    protected CslaDataSource _cslaDataSource = new CslaDataSource();
    protected ResultsCriteria _resultsCriteria;
    protected PagingInfo _pagingInfo;

    private int FormGroupId
    {
        get {
            if (!string.IsNullOrEmpty(ConfigurationSectionName))
            {
                if(ConfigurationUtilities.GetFormGroupIDFromConfig(ConfigurationSectionName) != 0)
                    return ConfigurationUtilities.GetFormGroupIDFromConfig(ConfigurationSectionName);
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["FormGroupId"]))
                return int.Parse(ConfigurationManager.AppSettings["FormGroupId"]);

            return 0;
        }
    }

    private string ConfigurationSectionName
    {
        get
        {
            if(HttpContext.Current.Request.QueryString.GetValues("chembiovizconfig") != null)
                return HttpContext.Current.Request.QueryString.GetValues("chembiovizconfig")[0];

            return null;
        }
    }

    private string ApplicationName
    {
        get {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["AppName"]))
                return ConfigurationManager.AppSettings["AppName"];

            return null;

        }
    }

    public GenericBO BusinessObject
    {
        get {
            if(Session["BasePageBusinessObject"] == null)
                Session["BasePageBusinessObject"] = GenericBO.GetGenericBO(this.ApplicationName);

            return (GenericBO)Session["BasePageBusinessObject"];
        }
        set {
            Session["BasePageBusinessObject"] = value;
        }
    }

    public BasePage() {
        //
        // TODO: Add constructor logic here
        //
    }

    protected override void OnPreLoad(EventArgs e) {
        //this.LoadFormGenerator();
        base.OnPreLoad(e);
    }

    protected override void OnPreInit(EventArgs e)
    {
        this.LoadFormGenerator();
        base.OnPreInit(e);
    }

    protected override void OnInit(EventArgs e) {
        base.OnInit(e);
    }

    private void LoadFormGenerator() {
        COEFormBO coeFormService = COEFormBO.Get(FormGroupId);

        PlaceHolder placeHolder = GetFormGeneratorHolder();

        _formGroup = new COEFormGroup();
        _formGroup.ID = "FormGenerator";
        _formGroup.FormGroupDescription = coeFormService.COEFormGroup;


        placeHolder.Controls.Clear();
        placeHolder.Controls.Add(_formGroup);

    }

    private PlaceHolder GetFormGeneratorHolder()
    {
        PlaceHolder placeHolder = (PlaceHolder)this.FindControl("FormGeneratorHolder");

        if (placeHolder == null)
        {
            placeHolder = new PlaceHolder();
            placeHolder.ID = "FormGeneratorHolder";
            this.Controls.Add(placeHolder);
        }
        return placeHolder;
    }


}
