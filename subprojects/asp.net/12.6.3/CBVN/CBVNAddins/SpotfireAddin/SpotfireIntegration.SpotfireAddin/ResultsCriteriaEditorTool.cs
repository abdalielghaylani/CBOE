using System.Windows.Forms;
using System.Xml;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.Common;
using FormWizard;
using Spotfire.Dxp.Application;
using Spotfire.Dxp.Application.Extension;
using Spotfire.Dxp.Data;
using SpotfireIntegration.Common;
using SpotfireIntegration.SpotfireAddin.Properties;
using Spotfire.Dxp.Framework.ApplicationModel;

namespace SpotfireIntegration.SpotfireAddin
{
    public sealed class ResultsCriteriaEditorTool : CustomTool<Document>
    {
        public ResultsCriteriaEditorTool()
            : base("Edit ChemBioViz Results Criteria...") // Fixed CSBR-167220 Changed the sub menu name as requested
        {
        }

        protected override void ExecuteCore(Document context)
        {
            //EditResultsCriteria(context);
        }

        //static internal void EditResultsCriteria(Document context, COEHitList coeHitList)
        //{
        //    // Find the base table to edit.
        //    // TODO: Handle the case where a single Document contains multiple ResultsCriteria.
        //    DataTable baseDataTable = context.Data.Tables.GetCOEBaseTable();
        //    if (baseDataTable == null)
        //    {
        //        MessageBox.Show("No Results Criteria found to edit", "Results Criteria Editor");
        //        return;
        //    }

        //    // Get the COEService and make sure we are logged in.
        //    COEService service = context.Context.GetService<COEService>();
        //    //if (!service.IsAuthenticated)
        //    //{
        //    //    if (!service.Login())
        //    //    {
        //    //        MessageBox.Show("Unable to authenticate with the ChemOffice Enterprise server.",
        //    //            "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    //        return;
        //    //    }
        //    //}

        //    //COEHitList hitList;
        //    try
        //    {
        //        coeHitList = baseDataTable.GetCOEHitList();
        //    }
        //    catch (XmlException)
        //    {
        //        MessageBox.Show("Failed to load Results Criteria", "Results Criteria Editor");
        //        return;
        //    }
        //    ResultsCriteria resultsCriteria = coeHitList.ResultsCriteria;

        //    // Load the DataView.
        //    COEDataViewBO dataViewBO = service.GetDataViewBO(coeHitList);

        //    context.Context.GetService<ProgressService>().ExecuteWithProgress(
        //        "Updating tables", "The COE data tables in Spotfire are being updated for the new result criteria.",
        //        delegate
        //        {
        //            // Reload the tables using the revised ResultsCriteria.
        //            AnalysisApplication application = context.Context.GetService<AnalysisApplication>();
        //            application.UpdateSpotfireDataTable(form.resultsCriteria);
        //        });

        //    CBVNController.GetInstance().ResultsCriteriaChanged();
        //}
    }
}
