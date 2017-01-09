using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for Constants
/// </summary>
namespace PerkinElmer.CBOE.Registration.Client
{
    public class Constants
    {
        public const string ReviewRegisterMixturePage = "ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX";
        //public const string ReviewRegisterMixturePage = "ASP.FORMS_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX";
        public const string PublicCommonJScriptsPath = "~/Forms/Public/JScripts/CommonUtilities.js";
        public const string AppName = "AppName";
        public const string AppPagesTitle = "AppPagesTitle";
        public const string InvHasRegIdInGroupingField = "InvHasRegIdInGroupingField";
        public const string InvHasRegBatchIdInGroupingField = "InvHasRegBatchIdInGroupingField";
        // Column Names

        public const string DeleteColumn = "DeleteColumn";
        public const string IdColumn = "IDColumn";

        // Control IDs

        public const string ContentPlaceHolderID = "ContentPlaceHolder";
        public const string MessagesAreaUCID = "MessagesAreaUserControl";
        public const string ErrorAreaUCID = "ErrorsWebPanel";
        public const string ErrorYUIPanelID = "ErrorControlYUI";

        // Exception Policies

        public const string REG_GUI_POLICY = "RegUIExceptionPolicy";
        public const string REG_OTHER_POLICY = "RegUnknownExceptionPolicy";
        public const string REG_LOGANDTHROW_POLICY = "LogAndThrowNewException";
        public const string REG_LOGONLY_POLICY = "LogOnly";

        // Fields length

        public const int DescriptionLongSummaryFieldLimit = 25;
        public const int NameLongSummaryFieldLimit = 25;

        // Query-string Keys

        public const string FullRegNum_UrlParameter = "BatchRegNum";
        public const string RegNum_UrlParameter = "RegNum";
        public const string SavedObjectId_UrlParameter = "SavedObjectId";
        public const string SubmittedObjectId_UrlParameter = "SubmittedObjectId";
        public const string RegisteredObjectId_UrlParameter = "RegisteredObjectId";
        public const string RegisteredCompoundId_UrlParameter = "RegisteredCompoundId";
        public const string RegisteredRegId_UrlParameter = "RegisteredRegId";
        public const string FormGroup_UrlParameter = "FormGroup";
        public const string CurrentPageState_UrlParameter = "CurrentPageState";
        public const string CurrentBatchIndex_UrlParameter = "CurrentBatchIndex";
        public const string Ticket_UrlParameter = "ticket";
        public const string AllowedActionsParameter = "AllowedActions";
        public const string AllowedActions_All = "AllowedActions_All";
        public const string AllowedActions_AddBatch = "AllowedActions_AddBatch";
        public const string MustImportCustomization_URLParameter = "MustImportCustomization";
        public const string RegistryTypeParameter = "RegistryType";
        public const string ShowUserPreference_UrlParameter = "ShowUserPreference";

        // Session Keys

        public const string MultiCompoundObject_Session = "MultiCompoundBusinessObject";
        public const string DuplicateIdsList_Session = "DuplicateSingleCompoundRegistryRecordIdList";
        public const string DuplicateMultiCompounds_Session = "DuplicateMultiCompoundRegistryRecordList";
        public const string DuplicateCompoundIdsList_Session = "DuplicateCompoundRegistryRecordIdList";
        public const string DuplicateCompoundObjects_Session = "DuplicateCompoundRegistryRecordList";
        public const string CompoundsToResolve_Session = "CompoundsToResolveList";
        public const string PermSearchedInfo = "PermSearchedInfo";
        public const string TempSearchedInfo = "TempSearchedInfo";
        public const string AvailableRecord = "AvailableRecord";

        // ViewState Names

        public const string SelectedNode_ViewState = "SelectedNode";
        public const string CurrentPageState_ViewState = "CurrentPageState";
        public const string LastNodeEditted_ViewState = "LastNodeEditted";

        //Cache Keys

        public const string ClearCache = "ClearCache";


    }
}
