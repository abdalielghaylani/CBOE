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
using System.Reflection;

public partial class Forms_Public_UserControls_ImageButton : System.Web.UI.UserControl
{
    #region Variables

    private ButtonModes _buttonMode = ButtonModes.ImgAndTxt;
    
    #endregion

    #region Constants

    /// <summary>
    /// Supported buttons for easy implementation.
    /// </summary>
    public enum TypeOfButtons
    {
        Back,
        Cancel,
        Next,
        Save,
        Submit,
        UpdateFields,
        UpdateTable,
        Clear,
        Update,
        AddUser,
        RemoveUser,
        AddRole,
        RemoveRole,
        Remove,
        Delete,
        Invalid,
        Valid,
        Resolve,
        None,
        CreateAlias,
        NewRelationship,
        AddSchema,
        Publish,
        Edit,
        EditRoleRoles,
        EditRoleUsers,
        Close,
        Done,
        Ok,
        GoBack,
        GroupAdd
    }

    /// <summary>
    /// Modes of display of the control
    /// </summary>
    public enum ButtonModes
    {
        ImgAndTxt,
        Img,
        Txt,
    }

    #endregion

    #region Events
    public event EventHandler<EventArgs> ButtonClicked = null;
    #endregion

    #region Properties

    private Constants.IconLibrary ImagesCollection
    {
        get
        {
            return ViewState[Constants.ImagesCollection] != null ? (Constants.IconLibrary)ViewState[Constants.ImagesCollection] : Constants.IconLibrary.None;
        }
        set
        {
            ViewState[Constants.ImagesCollection] = value;
        }
    }

    /// <summary>
    /// Set a directory to easy access to each files inside of it.
    /// </summary>
    public string ImagesFolder
    {
        get 
        {
            return ViewState[Constants.ImagesFolder] != null ? (string)ViewState[Constants.ImagesFolder] : Utilities.ImagesBaseRelativeFolder();
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                ViewState[Constants.ImagesFolder] = value;
        }
    }

    /// <summary>
    /// Image Src Name. If you have set the ImagesFolder you just have to add the image name.
    /// </summary>
    public string ImageName
    {
        get
        {
            return ViewState[Constants.ImageName] != null ? (string)ViewState[Constants.ImageName] : String.Empty;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
                ViewState[Constants.ImageName] = value;
        }
    }

    /// <summary>
    /// Set the relative path to the Image.
    /// </summary>
    public string ImageURL
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                this.ActionImageButton.ImageUrl = value;
        }
        get
        {
            return this.ActionImageButton.ImageUrl;
        }
    }

    /// <summary>
    /// Set the relative path to the hover Image.
    /// </summary>
    public string HoverImageURL
    {
        get
        {
            return (string) ViewState["HoverImageURL"];
        }
        set
        {
            ViewState["HoverImageURL"] = value;
        }
    }

    /// <summary>
    /// Tooltip text to display on the image
    /// </summary>
    public string ImageToolTip
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                this.ActionImageButton.ToolTip = value;
        }
    }

    /// <summary>
    /// Image CSS Class
    /// </summary>
    public string ImageCssClass
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                this.ActionImageButton.CssClass = value;
        }
    }

    /// <summary>
    /// Text to display in the button
    /// </summary>
    public string ButtonText
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                this.ActionButton.Text = value;
        }
    }

    /// <summary>
    /// Tooltip text to display on the button
    /// </summary>
    public string ButtonToolTip
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                this.ActionButton.ToolTip = value;
        }
    }

    /// <summary>
    /// Button CSS Class
    /// </summary>
    public string ButtonCssClass
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                this.ActionButton.CssClass = value;
        }
    }

    /// <summary>
    /// Button Mode. Check ButtonModesEnum to see all the options.
    /// </summary>
    public ButtonModes ButtonMode
    {
        set
        {
            _buttonMode = value;
        }
    }

    /// <summary>
    /// Type of the Button. Check TypeOfButtons Enum to see all the options   
    /// </summary>
    public TypeOfButtons TypeOfButton
    {
        set
        {
            ViewState[Constants.TypeOfButton] = value;
        }
        get
        {
            return (TypeOfButtons)ViewState[Constants.TypeOfButton];
        }
        
    }

    /// <summary>
    /// Set the jscript code to run on Client Click.
    /// </summary>
    public string OnClientClick
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                this.ActionImageButton.OnClientClick = this.ActionButton.OnClientClick = value;
        }
    }

    /// <summary>
    /// Set the jscript code to run on Client Click.
    /// </summary>
    public string PostBackURL
    {
        set
        {
            if(!string.IsNullOrEmpty(value))
                this.ActionImageButton.PostBackUrl = this.ActionButton.PostBackUrl = value;
        }
    }

    /// <summary>
    /// Display as enable/disable the button 
    /// </summary>
    public bool Enabled
    {
        set
        {
            this.ActionButton.Enabled = this.ActionImageButton.Enabled = value;
        }
    }

    /// <summary>
    /// Set to false if the submit of this button doesn't require validation.
    /// </summary>
    public bool CausesValidation
    {
        set
        {
            this.ActionImageButton.CausesValidation = this.ActionButton.CausesValidation = value;
        }
    }

    /// <summary>
    /// Add parameteres as you desire here.
    /// </summary>
    public string CommandArgument
    {
        get
        {
            return this.ActionButton.CommandArgument;
        }
        set
        {
            this.ActionButton.CommandArgument = this.ActionImageButton.CommandArgument = value == null ? string.Empty : value;
        }
    }

    /// <summary>
    /// Add parameteres as you desire here.
    /// </summary>
    public string CommandName
    {
        get
        {
            return this.ActionButton.CommandName;
        }
        set
        {
            this.ActionButton.CommandName = this.ActionImageButton.CommandName = value == null ? string.Empty : value;
        }
    }

    public string ValidationGroup
    {
        get
        {
            return this.ActionButton.ValidationGroup;
        }
        set
        {
            if(!string.IsNullOrEmpty(value))
                this.ActionButton.ValidationGroup = this.ActionImageButton.ValidationGroup = value;
        }
    }

    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //if (!Page.IsPostBack)
        //{
            this.SetControlAttributes();
        //}
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void ActionImageButton_Click(object sender, ImageClickEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (ButtonClicked != null)
        {
            // Copy to a temporary variable to be thread-safe.
            EventHandler<EventArgs> currentEventRaised = ButtonClicked;
            currentEventRaised(this, new EventArgs());
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void ActionButton_Click(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (ButtonClicked != null)
        {
            // Copy to a temporary variable to be thread-safe.
            EventHandler<EventArgs> currentEventRaised = ButtonClicked;
            currentEventRaised(this, new EventArgs());
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    protected override void OnPreRender(EventArgs e)
    {
        if (this.TypeOfButton != TypeOfButtons.None)
        {
            this.ActionImageButton.Attributes.Add("OnMouseOver", "ChangeImage(this.id,'" + this.Page.ResolveClientUrl(this.GetFullHoverImageURL()) + "');");
            this.ActionImageButton.Attributes.Add("OnMouseOut", "ChangeImage(this.id,'" + this.Page.ResolveClientUrl(this.GetFullImageURL()) + "');");
        }
        base.OnPreRender(e);
    }

    #endregion

    #region Methods

    private string GetFullImageURL()
    {
        if(string.IsNullOrEmpty(this.ImageURL))
            return Utilities.ImagesIconLibFullFolder(this.ImagesCollection, this.Page.StyleSheetTheme) + this.ImageName + "." + Utilities.ImagesIconLibFormat().ToLower();
        else
            return this.ImageURL;
    }

    private string GetFullHoverImageURL()
    {
        if(string.IsNullOrEmpty(ImageURL))
            return Utilities.ImagesIconLibFullFolder(this.ImagesCollection, this.Page.StyleSheetTheme) + this.ImageName + "_h" + "." + Utilities.ImagesIconLibFormat().ToLower();
        else
        {
            if(string.IsNullOrEmpty(this.HoverImageURL))
                return this.ImageURL;
            else
                return this.HoverImageURL;
        }
    }


    private void SetControlAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Set button mode according variable.
        switch (_buttonMode)
        {
            case ButtonModes.ImgAndTxt:
                //TODO: define div class.
                break;
            case ButtonModes.Txt:
                this.ActionImageButton.Visible = false;
                this.ActionButton.Visible = true;
                //define div class.
                break;
            case ButtonModes.Img:
                this.ActionButton.Visible = false;
                this.ActionImageButton.Visible = true;
                //define div class.
                break;
            default:
                //define div class.
                break;
        }
        //Set Image URL
        if(string.IsNullOrEmpty(this.ImageURL))
            this.ActionImageButton.ImageUrl = this.GetURLImage();
        else
            this.ActionImageButton.ImageUrl = this.ImageURL;

        if (this.TypeOfButton != TypeOfButtons.None)
        {
            this.SetText();
            this.SetCSSClass();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void SetCSSClass()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if(string.IsNullOrEmpty(this.ActionButton.CssClass))
            this.ActionButton.CssClass = "ImageButton";
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private void SetText()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string controlText = String.Empty;
        switch (this.TypeOfButton)
        {
            case TypeOfButtons.Back:
                controlText = Resources.Resource.Back_Button_Text;
                break;
            case TypeOfButtons.Cancel:
                controlText = Resources.Resource.Cancel_Button_Text;
                break;
            //case TypeOfButtons.Help:
            //    controlText = Resources.Resource.Help_Button_Text;
            //    break;
            //case TypeOfButtons.Home:
            //    controlText = Resources.Resource.Home_Button_Text;
            //    break;
            case TypeOfButtons.Next:
                controlText = Resources.Resource.Next_Button_Text;
                break;
            //case TypeOfButtons.Refresh:
            //    controlText = Resources.Resource.Refresh_Button_Text;
            //    break;
            case TypeOfButtons.Save:
                controlText = Resources.Resource.Save_Button_Text;
                break;
            //case TypeOfButtons.Search:
            //    controlText = Resources.Resource.Search_Button_Text;
            //    break;
            case TypeOfButtons.Submit:
                controlText = Resources.Resource.Submit_Button_Text;
                break;
            //SM- CSBR-142993: Adding Ok event 
            case TypeOfButtons.Ok:
                controlText = Resources.Resource.Submit_Button_Text;
                break;
            case TypeOfButtons.GoBack:
                controlText = Resources.Resource.Submit_Button_Text;
                break;
            case TypeOfButtons.UpdateFields:
                controlText = Resources.Resource.UpdateFields_Button_Text;
                break;
            case TypeOfButtons.UpdateTable:
                controlText = Resources.Resource.UpdateTable_Button_Text;
                break;
            case TypeOfButtons.Clear:
                controlText = Resources.Resource.Clear_Button_Text;
                break;
            case TypeOfButtons.Delete:
                controlText = Resources.Resource.Delete_Button_Text;
                break;
            case TypeOfButtons.Update:
                controlText = Resources.Resource.Update_Button_Text;
                break;
            case TypeOfButtons.AddUser:
                controlText = Resources.Resource.AddUser_Button_Text;
                break;
            case TypeOfButtons.RemoveUser:
                controlText = Resources.Resource.RemoveUser_Button_Text;
                break;
            case TypeOfButtons.AddRole:
                controlText = Resources.Resource.AddRole_Button_Text;
                break;
            case TypeOfButtons.RemoveRole:
                controlText = Resources.Resource.RemoveRole_Button_Text;
                break;
            case TypeOfButtons.Remove:
                controlText = Resources.Resource.Remove_Label_Text;
                break;
            //case TypeOfButtons.Add:
            //    controlText = Resources.Resource.Add_Button_Text;
            //    break;
            //case TypeOfButtons.Validate:
            //    controlText = Resources.Resource.Validate_Button_Text;
            //    break;
            case TypeOfButtons.Invalid:
                controlText = Resources.Resource.Invalid_Button_Text;
                break;
            case TypeOfButtons.Valid:
                controlText = Resources.Resource.Valid_Button_Text;
                break;
            case TypeOfButtons.Resolve:
                controlText = Resources.Resource.Resolve_Button_Text;
                break;
            case TypeOfButtons.CreateAlias:
                controlText = Resources.Resource.CreateAlias_Button_Text;
                break;
            case TypeOfButtons.NewRelationship:
                controlText = Resources.Resource.NewRelationship_Button_Text;
                break;
            case TypeOfButtons.AddSchema:
                controlText = Resources.Resource.AddSchema_Button_Text;
                break;
            case TypeOfButtons.Publish:
                controlText = Resources.Resource.Publish_Button_Text;
                break;
            case TypeOfButtons.Edit:
                controlText = Resources.Resource.Publish_Button_Edit;
                break;
            case TypeOfButtons.EditRoleUsers:
                controlText = Resources.Resource.Publish_Button_EditRoleUsers;
                break;
            case TypeOfButtons.EditRoleRoles:
                controlText = Resources.Resource.Publish_Button_EditRoleRoles;
                break;
            case TypeOfButtons.Close:
                controlText = Resources.Resource.Publish_Button_Close;
                break;
            case TypeOfButtons.Done:
                controlText = Resources.Resource.Publish_Button_Done;
                break;
            default:
                break;
        }
        if(string.IsNullOrEmpty(this.ActionButton.Text))
            this.ActionImageButton.ToolTip = this.ActionButton.Text = this.ActionButton.ToolTip = controlText;
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private string GetURLImage()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string retVal = this.ImagesFolder;
        string imagesExtension = "." + Utilities.ImagesIconLibFormat().ToLower();
        switch (this.TypeOfButton)
        {
            case TypeOfButtons.Back:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Arrow_Left_B";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Cancel:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Cross_R";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Next:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Arrow_Right_B";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Save:
                this.ImagesCollection = Constants.IconLibrary.Vista_Business_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Save";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Submit:
                this.ImagesCollection = Constants.IconLibrary.Vista_Business_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Save_As";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Ok:
                this.ImagesCollection = Constants.IconLibrary.Vista_Business_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Ok_Dialog_Btn";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.GoBack:
                this.ImagesCollection = Constants.IconLibrary.Vista_Business_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Ok_Btn";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.GroupAdd:
                this.ImagesCollection = Constants.IconLibrary.Vista_Business_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Add";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.UpdateFields:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Edit";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.UpdateTable:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Edit";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Clear:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Clean_Up";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Update:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Edit";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Edit:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Edit";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.EditRoleUsers:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Edit";
                retVal += this.ImageName + imagesExtension;
                break;

            case TypeOfButtons.EditRoleRoles:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Edit";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.AddUser:
                this.ImagesCollection = Constants.IconLibrary.Database_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "User_Add";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.RemoveUser:
                this.ImagesCollection = Constants.IconLibrary.Database_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "User_Drop";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.AddRole:
                this.ImagesCollection = Constants.IconLibrary.Network_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Forward_3";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.RemoveRole:
                this.ImagesCollection = Constants.IconLibrary.Network_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Reply_3";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Remove:
                this.ImagesCollection = Constants.IconLibrary.Database_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Delete_To_Bin";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Delete:
                this.ImagesCollection = Constants.IconLibrary.Vista_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Delete";
                retVal += this.ImageName + imagesExtension;
                break;
            //case TypeOfButtons.Add:
            //    retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
            //    this.ImageName += "Add_Green";
            //    retVal += this.ImageName + imagesExtension;
            //    break;
            case TypeOfButtons.Invalid:
                this.ImagesCollection = Constants.IconLibrary.Vista_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Wrong";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Valid:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Check";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Resolve:
                this.ImagesCollection = Constants.IconLibrary.Vista_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Accessibility_Warning";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.CreateAlias:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Add_Green";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.NewRelationship:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Add_Green";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Publish:
                this.ImagesCollection = Constants.IconLibrary.Database_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Database_Server";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.AddSchema:
                this.ImagesCollection = Constants.IconLibrary.Database_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Database_Register";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.None:
                if (!string.IsNullOrEmpty(this.ImagesFolder) && !string.IsNullOrEmpty(this.ImageName))
                    retVal = this.ImagesFolder + this.ImageName;
                else if (!string.IsNullOrEmpty(this.ActionImageButton.ImageUrl))
                    retVal = this.ActionImageButton.ImageUrl;
                break;

            case TypeOfButtons.Done:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Cross_R";
                retVal += this.ImageName + imagesExtension;
                break;

            case TypeOfButtons.Close:
                this.ImagesCollection = Constants.IconLibrary.Core_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Cross_R";
                retVal += this.ImageName + imagesExtension;
                break;
            default:
                if (!string.IsNullOrEmpty(this.ImagesFolder) && !string.IsNullOrEmpty(this.ImageName))
                    retVal = this.ImagesFolder + this.ImageName;
                else if (!string.IsNullOrEmpty(this.ActionImageButton.ImageUrl))
                    retVal = this.ActionImageButton.ImageUrl;
                break;
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
        return retVal;
    }

    #endregion

    #region Public Methods

    public static Forms_Public_UserControls_ImageButton NewImageButton(TypeOfButtons type)
    {
        return new Forms_Public_UserControls_ImageButton(type);
    }

    public static Forms_Public_UserControls_ImageButton NewImageButton()
    {
        return new Forms_Public_UserControls_ImageButton();
    }

    public string GetButtonUniqueID()
    {
        return this.ActionButton.UniqueID;
    }

    public string GetButtonClientID()
    {
        return this.ActionButton.ClientID;
    }
    #endregion

    #region Constructors

    private Forms_Public_UserControls_ImageButton(TypeOfButtons type)
    {
        this.TypeOfButton = type;
    }

    public Forms_Public_UserControls_ImageButton()
    {

    }

    #endregion
}
