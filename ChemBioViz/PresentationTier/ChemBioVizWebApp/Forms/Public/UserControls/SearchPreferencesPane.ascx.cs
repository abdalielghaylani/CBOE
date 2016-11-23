using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.GUIShell;
using Resources;

namespace CambridgeSoft.COE.ChemBioVizWebApp.Forms.Public.UserControls
{
    public partial class SearchPreferencesPane : System.Web.UI.UserControl, ICOENavigationPanelControl
    {

        #region Page Lyfe Cycle Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
                this.SetControlsAttributtes();
        }
        #endregion

        #region Private Methods
        protected void SetControlsAttributtes()
        {
            this.HitAnyChargeTD.InnerText = Resource.HitAnyChargeOnHetero_Label_Text;
            this.ReactionQueryTD.InnerText = Resource.ReactionQuery_Label_Text;
            this.HitAnyChargeOnCarbonTD.InnerText = Resource.HitAnyChargeOnCarbon_Label_Text;
            this.PermitExtraneousFragmentsInFullTD.InnerText = Resource.PermitExtraneousFragmentsInFull_Label_Text;
            this.PermitExtraneousFragmentsInReactionFullTD.InnerText = Resource.PermitExtraneousFragmentsInReactionFull_Label_Text;
            this.FragmentsOverlapTD.InnerText = Resource.FragmentsOverlap_Label_Text;
            this.TautomericTD.InnerText = Resource.Tautomeric_Label_Text;
            this.IgnoreImplicitHydrogensTD.InnerText = Resource.IgnoreImplicitHydrogens_Label_Text;
            this.SimilarityTD.InnerText = Resource.Similarity_Label_Text;
            this.FullStructureSimilarityTD.InnerText = Resource.FullStructureSimilarity_Label_Text;
            this.StereochemistryOptionsTD.InnerText = Resource.StereochemistryOptions_Label_Text;
            this.MatchTetrahedralStereoTD.InnerText = Resource.MatchTetrahedralStereo_Label_Text;
            this.MatchDoubleBondStereoTD.InnerText = Resource.MatchDoubleBondStereo_Label_Text;
            this.ThickBondsRelTD.InnerText = Resource.ThickBondsRel_Label_Text;
        }
        #endregion

        #region ICOENavigationPanelControl Members


        public event EventHandler<COENavigationPanelControlEventArgs> CommandRaised;

        #endregion
    }
}