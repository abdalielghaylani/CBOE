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
using System.ComponentModel;
using System.Globalization;
using System.Security.Permissions;
using System.Collections.Generic;

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
        Search,
        Clear,
        RetrieveAll,
        HitList,
        Refine,
        NewQuery,
        MarkHit,
        None,
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
                this.TxtActionButton.OnClientClick = this.ActionImageButton.OnClientClick = this.ActionButton.OnClientClick = value;
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
            this.TxtActionButton.CausesValidation = this.ActionImageButton.CausesValidation = this.ActionButton.CausesValidation = value;
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
            this.ActionButton.CommandArgument = value == null ? string.Empty : value;
        }
    }

    ///// <summary>
    ///// CSSClass for the control container.
    ///// </summary>
    //public string CSSClass
    //{
    //    get { return this.ImageButtonDivContainer.Attributes["class"].ToString(); }
    //    set { this.ImageButtonDivContainer.Attributes["class"] = value; }
    //}

    /// <summary>
    /// Set submit behavior for this control
    /// </summary>
    public bool UseSubmitBehavior
    {
        set { this.ActionButton.UseSubmitBehavior = value; }
    }

    //public override string ClientID
    //{
    //    get
    //    {
    //        return this.ImageButtonDivContainer.ClientID; ;
    //    }
    //}

    public string PostBackURL
    {
        get { return this.ActionImageButton.PostBackUrl; }
        set { this.ActionButton.PostBackUrl = this.ActionImageButton.PostBackUrl = value;}
    }

    #endregion

    #region Event Handlers

    protected void Page_Load(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (Page.IsPostBack)
        {
            //Catch a click on the client, but in the div section.
            if (this.ClientID == Request.Params.Get("__EVENTARGUMENT"))
                this.ReThrowEventToListener((EventArgs)e);
        }
        else
            this.SetControlAttributes();
            
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void ActionImageButton_Click(object sender, ImageClickEventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.ReThrowEventToListener((EventArgs)e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected void ActionButton_Click(object sender, EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.ReThrowEventToListener(e);
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    protected override void OnPreRender(EventArgs e)
    {
        if(this.TypeOfButton != TypeOfButtons.None && _buttonMode != ButtonModes.Txt)
        {
            this.ActionImageButton.Attributes.Add("OnMouseOver", "ChangeImage(this.id,'" + this.GetFullHoverImageURL() + "');");
            this.ActionImageButton.Attributes.Add("OnMouseOut", "ChangeImage(this.id,'" + this.GetFullImageURL() + "');");
        }
        base.OnPreRender(e);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get all the controls inside the container to subscribe to events.
    /// </summary>
    /// <returns></returns>
    /// <remarks>The main purpose of this object if to subscribe to container events (div) not supported/launched for IE</remarks>
    public List<string> GetControlsInsideClientIDs()
    {
        List<string> retVal = new List<string>();
        switch(_buttonMode)
        {
            case ButtonModes.Img:
                retVal.Add(this.ActionImageButton.ClientID);
                break;
            case ButtonModes.ImgAndTxt:
                retVal.Add(this.ActionButton.ClientID);
                retVal.Add(this.ActionImageButton.ClientID);
                break;
            case ButtonModes.Txt:
                retVal.Add(this.TxtActionButton.ClientID);
                break;
        }
        
        return retVal;
    }

    /// <summary>
    /// Re throws the event to a page that is subscribed to this control. 
    /// </summary>
    /// <param name="e"></param>
    /// <remarks>The public event is just one, it doesn't matter id the user clicks the div, imagebutton or the button.</remarks>
    private void ReThrowEventToListener(EventArgs e)
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        if (ButtonClicked != null)
        {
            EventHandler<EventArgs> currentEventRaised = ButtonClicked; // Copy to a temporary variable to be thread-safe.
            currentEventRaised(this, e);
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }


    /// <summary>
    /// Get the full default image url for jscript use.
    /// </summary>
    /// <returns></returns>
    private string GetFullImageURL()
    {
        return Utilities.ImagesIconLibFullFolder(this.ImagesCollection, this.Page.StyleSheetTheme) + this.ImageName + "." + Utilities.ImagesIconLibFormat().ToLower();
    }

    /// <summary>
    /// Get the full hover image url for jscript use.
    /// </summary>
    /// <returns></returns>
    private string GetFullHoverImageURL()
    {
        return Utilities.ImagesIconLibFullFolder(this.ImagesCollection, this.Page.StyleSheetTheme) + this.ImageName + "_h" + "." + Utilities.ImagesIconLibFormat().ToLower();
    }


    /// <summary>
    /// Set enable or disable controls according the ButtonMode.
    /// </summary>
    private void SetControlAttributes()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        //Set button mode according variable.
        switch (_buttonMode)
        {
            case ButtonModes.ImgAndTxt:
                this.TxtAndImgContainer.Visible = true;
                this.OnlyTextContainer.Visible = false;
                break;
            case ButtonModes.Txt:
                this.TxtAndImgContainer.Visible = false;
                this.OnlyTextContainer.Visible = true;
                //define div class.
                break;
            case ButtonModes.Img:
                this.TxtAndImgContainer.Visible = true;
                this.OnlyTextContainer.Visible = false;

                this.ActionButton.Visible = false;
                this.ActionImageButton.Visible = true;
                //define div class.
                break;
            default:
                this.TxtAndImgContainer.Visible = true;
                this.OnlyTextContainer.Visible = false;
                //define div class.
                break;
        }
        //Set Image URL
        this.ActionImageButton.ImageUrl = this.GetURLImage();

        if (this.TypeOfButton != TypeOfButtons.None)
        {
            this.SetText();
            //this.SetCSSClass();
        }
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Set a CSS Class by default.
    /// </summary>
    /// <remarks>This value can be overwritten accesing directly to the property</remarks>
    private void SetCSSClass()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        this.ActionButton.CssClass = "ImageButton";
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    /// <summary>
    /// Set the text to display according the TypeOfButton
    /// </summary>
    /// <remarks>This value can be overwritten accesing directly to the property</remarks>
    private void SetText()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string controlText = String.Empty;
        switch (this.TypeOfButton)
        {
            case TypeOfButtons.Search:
                controlText = Resources.Resource.Search_Button_Text;
                //this.ImageButtonDivContainer.Attributes.Add("title", controlText);
                break;
            case TypeOfButtons.Clear:
                controlText = Resources.Resource.Clear_Button_Text;
                break;
            case TypeOfButtons.RetrieveAll:
                controlText = Resources.Resource.RetrieveAll_Button_Text;
                break;
            case TypeOfButtons.HitList:
                controlText = Resources.Resource.Hitlist_Button_Text;
                //this.ImageButtonDivContainer.Attributes.Add("title", controlText);
                break;
            case TypeOfButtons.NewQuery:
                controlText = Resources.Resource.NewQuery_Button_Text;
                //this.ImageButtonDivContainer.Attributes.Add("title", controlText);
                break;
            case TypeOfButtons.Refine:
                controlText = Resources.Resource.Refine_Button_Text;
                //this.ImageButtonDivContainer.Attributes.Add("title", controlText);
                break;
            case TypeOfButtons.MarkHit:
                controlText = Resources.Resource.MarkHit_Button_Text;
                break;
            default:
                break;
        }
        this.ActionImageButton.ToolTip = this.ActionButton.Text = this.ActionButton.ToolTip = controlText;
        this.TxtActionButton.ToolTip = this.TxtActionButton.Text = controlText;

        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.EndMethod, MethodBase.GetCurrentMethod().Name);
    }

    private string GetURLImage()
    {
        Utilities.WriteToAppLog(GUIShellTypes.LogMessageType.BeginMethod, MethodBase.GetCurrentMethod().Name);
        string retVal = this.ImagesFolder;
        string imagesExtension = "." + Utilities.ImagesIconLibFormat().ToLower();
        switch (this.TypeOfButton)
        {
            case TypeOfButtons.HitList:
                this.ImagesCollection = Constants.IconLibrary.Database_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Table";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.NewQuery:
                this.ImagesCollection = Constants.IconLibrary.Database_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Zoom_in";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Refine:
                this.ImagesCollection = Constants.IconLibrary.Database_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Filter_G";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.Search:
                this.ImagesCollection = Constants.IconLibrary.Database_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Zoom_in";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.MarkHit:
                this.ImagesCollection = Constants.IconLibrary.Database_Collection;
                retVal = this.ImagesFolder = Utilities.ImagesIconLibRelativeFolder(this.ImagesCollection);
                this.ImageName = "Plus";
                retVal += this.ImageName + imagesExtension;
                break;
            case TypeOfButtons.None:
                if (!string.IsNullOrEmpty(this.ImagesFolder) && !string.IsNullOrEmpty(this.ImageName))
                    retVal = this.ImagesFolder + this.ImageName;
                else if (!string.IsNullOrEmpty(this.ActionImageButton.ImageUrl))
                    retVal = this.ActionImageButton.ImageUrl;
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

    internal string GetButtonUniqueID()
    {
        switch(_buttonMode)
        {
            case ButtonModes.Txt:
                return this.TxtActionButton.UniqueID;
            case ButtonModes.ImgAndTxt:
                return this.TxtActionButton.UniqueID;
            case ButtonModes.Img:
                return this.ActionImageButton.UniqueID;
        }
        return this.UniqueID;
    }
    #endregion
}
