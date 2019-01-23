using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Security.Permissions;
using CambridgeSoft.COE.Framework.COEConfigurationService;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESecurityService;
using System.Reflection;


namespace CambridgeSoft.COE.Framework.Controls.WebParts
{

    [AspNetHostingPermission(SecurityAction.Demand,
        Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand,
      Level = AspNetHostingPermissionLevel.Minimal)]
    public class HomeWebPart : WebPart
    {

        string theme = string.Empty;
        public HomeWebPart()
        {
            
        }

        private Group _group;


        public Group Group
        {
            get { return _group; }

            set { _group = value; }
        }

       

        #region Web Part rendering methods and handlers



        public bool HasLinks(){
            bool linkFound = false;
            if (_group != null && _group.Enabled.ToLower()=="true")
            {
                string targetSection = _group.PageSectionTarget.ToUpper();
                if (targetSection == string.Empty) { targetSection = "PANEL"; }
                 switch (targetSection)
                    {   case "PANEL":
                                for (int i = 0; i < _group.LinksData.Count; i++)
                                {
                                    LinkData linkData = _group.LinksData.Get(i);
                                    if (UserHasPrivilege(linkData.PrivilegeRequired))
                                    {
                                        linkFound = true;

                                    }

                                }
                            break;
                        case "DASHBOARD":
                           
                            for (int i = 0; i < _group.CustomItems.Count; i++)
                            {
                                CustomItems customItem = _group.CustomItems.Get(i);
                                if (UserHasPrivilege(customItem.PrivilegeRequired))
                                {
                                    linkFound = true;

                                }

                            }
                            break;
                       
                    }
                
                
               
            }
            return linkFound;
        }

        protected override void CreateChildControls()
        {
           
            Controls.Clear();
            PagesSection pagesSection = (PagesSection)WebConfigurationManager.GetSection("system.web/pages");
            theme = pagesSection.StyleSheetTheme.ToLower();
            if (_group != null)
            {
               
                
                string color = _group.Color.ToString();
                string helpText = _group.HelpText.ToString();
                string displayName = _group.DisplayName.ToString();
                string pageTarget = _group.PageSectionTarget.ToUpper();
                if ((pageTarget == string.Empty) || (pageTarget == null)) { pageTarget = "PANEL"; }
                switch (pageTarget)
                {
                    case "PANEL":
                        #region panel
                        this.ScrollBars = ScrollBars.None;
                        this.ChromeType = PartChromeType.None;
                        this.ChromeState = PartChromeState.Normal;
                       
                        this.AllowClose = false;
                        this.AllowEdit = false;
                        this.AllowHide = false;
                        this.AllowMinimize = false;
                        this.AllowZoneChange = false;

                        //build panel shell

                        using (Literal startTags = new Literal())   // Coverity Fix CID : 11835 
                        {
                            startTags.Text = " <div class='apl_menu'><h1 class='" + color + "'>" + displayName + "</h1>";
                            startTags.Text = startTags.Text + "<a class='hid' href='#more' onclick='this.hideFocus=true' onblur='this.hideFocus=false'><span></span><em>";
                            startTags.Text = startTags.Text + helpText + "</em></a><h6 class='" + color + "_help'></h6><ul>";

                            this.Controls.Add(startTags);
                        }
                        AddLinkItems(_group);

                        using (Literal endTags = new Literal())     // Coverity Fix CID : 11835 
                        {
                            endTags.Text = "</div>";
                            this.Controls.Add(endTags);
                        }

                        break;
                       #endregion panel
                    case "DASHBOARD":
                        #region dashboard
                        this.ScrollBars = ScrollBars.None;
                        this.ChromeType = PartChromeType.None;
                        this.ChromeState = PartChromeState.Normal;
                        this.AllowClose = false;
                        this.AllowEdit = false;
                        this.AllowHide = false;
                        this.AllowMinimize = false;
                        this.AllowZoneChange = false;

                        //build dash shell
                       // string helpText = Group.HelpText.ToString();

                        

                        //startTags.Text = "<div id='dashboard'>";
                        using (Literal startTags = new Literal())   // Coverity Fix CID : 11835 
                        {
                            startTags.Text = "<h1 class='" + color + "'>" + displayName + "</h1>";
                            this.Controls.Add(startTags);
                        }
                        AddDashItems(_group);

                       
                       // endTags.Text = "</div>";
                        //this.Controls.Add(endTags);

                        break;
                        #endregion dashboard
                }
                       
            }
            
            ChildControlsCreated = true;
        }
        private void AddDashItems(Group group)
        {
            for (int i = 0; i < group.CustomItems.Count; i++)
            {
                CustomItems  customItems = group.CustomItems.Get(i);
                if (UserHasPrivilege(customItems.PrivilegeRequired))
                {

                    Literal Item = new Literal();
                    //paraOpen.Text = "<p>";

                    string value = string.Empty;
                    string text = string.Empty;
                    string id = string.Empty;
                    try
                    {
                        CustomItemLoader customItemLoader = new CustomItemLoader();
                        ICustomHomeItem customItem = customItemLoader.InstantiateCustomItem(customItems.AssemblyName, customItems.ClassName);
                        customItem.SetConfiguration(customItems.Configuration);
                        value = customItem.GetCustomItem();
                        id = customItems.Name;
                    }
                    catch (System.Exception ex)
                    {
                        value = ex.Message;
                    }

                    Item.ID = id;
                    Item.Text = value;
                    //spanClose.Text = value + " " + "</span>";
                    //paraClose.Text = text + "</p>";




                    this.Controls.Add(Item);
                    //this.Controls.Add(spanOpen);
                    //this.Controls.Add(spanClose);
                    //this.Controls.Add(paraClose);

                }
            }
        }

        private void AddLinkItems(Group  group)
        {
            for (int i = 0; i < group.LinksData.Count; i++)
            {
                LinkData linkData = group.LinksData.Get(i);
                string targetWindow = string.Empty;
                string _windowjs = string.Empty;

                //if (_group.NewWindow.ToUpper() == "TRUE")
                //{
                //    //targetWindow = group.Name;
                //    _windowjs = "window.open('" + linkData.URL + "','" + group.Name + "','location=1,status=1,scrollbars=1,width=800px,height=600px, top=100px, left=100px')";
                //}

                Literal linkTags=null;
                Literal div1Open=null;
                Literal div2Open=null;
                Literal div1Close=null;
                Literal div2Close=null;

                try
                {
                    linkTags = new Literal();
                    div1Open = new Literal();
                    div2Open = new Literal();
                    div1Close = new Literal();
                    div2Close = new Literal();

                    bool showLinksGraphic = linkData.LinkIconSize != Group.IconSize.none;

                    string imagePath = string.Empty;
                    if (showLinksGraphic == true)
                    {
                        imagePath = "/coecommonresources/" + linkData.LinkIconBasePath + "/" + linkData.LinkIconSize + "/" + linkData.LinkIconFileName;

                    }

                    //here we will figure out is user has rights to the link and only add it if they do
                    if (UserHasPrivilege(linkData.PrivilegeRequired))
                    {
                        div1Open.Text = "<li><div class='" + theme + "_" + linkData.LinkIconSize + "'>";
                        div2Open.Text = "<div class='icon_" + linkData.LinkIconSize + "' " + "style = 'background: transparent url(" + imagePath + ") no-repeat scroll;'>";
                        div1Close.Text = "</div></li>";
                        div2Close.Text = "</div>";
                        HyperLink _hyperLink = new HyperLink();
                        _hyperLink.Target = targetWindow;
                        _hyperLink.Text = GetSpacerText(linkData.LinkIconSize) + linkData.DisplayText;
                        //_hyperLink.NavigateUrl = linkData.URL;

                        if (_group.NewWindow.ToUpper() != "FALSE")
                        {
                            //targetWindow = group.Name;
                            //hard code an options list that would be similar to just launching a standard browser window
                            string _windowProps = "location=1,status=1,scrollbars=1,toolbar=1,resizable=1,width=1024px,height=768px,top=0px,left=0px";
                            if (_group.NewWindow.ToUpper() != "TRUE")
                            {
                                //if you are here then it means that value is neither true or false and thus shoudl be treated as an options list.
                                _windowProps = _group.NewWindow;
                            }
                            _windowjs = "if(event.which == 1){";                            
                            _windowjs += "window.open('" + this.Page.ResolveUrl(linkData.URL) + "','" + group.Name + "','" + _windowProps + "')";
                            _windowjs += ";event.returnValue = false; return false;}";
                            //_hyperLink.NavigateUrl = "#";
                            _hyperLink.Attributes.Add("onClick", _windowjs);
                            _hyperLink.NavigateUrl = this.Page.ResolveUrl(linkData.URL);
                            _hyperLink.Target = "_blank";
                        }
                        else
                        {
                            if (linkData.NewWindow.ToUpper() != "FALSE") //Ulises - In case there is just one link that needs to be opened in a new window.
                            {
                                string _windowProps = "location=1,status=1,scrollbars=1,toolbar=1,resizable=1,width=1024px,height=768px,top=100px,left=100px";
                                if (_group.NewWindow.ToUpper() != "TRUE")
                                {
                                    _windowProps = linkData.NewWindow;
                                }
                                _windowjs = "if(event.which == 1){";
                               _windowjs += "window.open('" + this.Page.ResolveUrl(linkData.URL) + "','" + group.Name + "','" + _windowProps + "')";
                            _windowjs += ";event.returnValue = false; return false;}";
                            //_hyperLink.NavigateUrl = "#";
                            _hyperLink.Attributes.Add("onClick", _windowjs);
                            _hyperLink.NavigateUrl = this.Page.ResolveUrl(linkData.URL);
                            _hyperLink.Target = "_blank";
                            }
                            else
                                _hyperLink.NavigateUrl = linkData.URL;
                        }


                        if (linkData.ToolTip != string.Empty)
                        {
                            _hyperLink.ToolTip = linkData.ToolTip;

                        }
                        _hyperLink.CssClass = theme + "_" + linkData.LinkIconSize;

                        if (showLinksGraphic == true)
                        {
                            HyperLink _hyperLink2 = new HyperLink();
                            _hyperLink.Attributes.Add("onmouseover", "this.hideFocus=true");
                            _hyperLink.Text = div2Open.Text + div2Close.Text + GetSpacerText(linkData.LinkIconSize) + linkData.DisplayText;
                            this.Controls.Add(div1Open);
                            this.Controls.Add(_hyperLink);
                            this.Controls.Add(div1Close);
                            // Coverity Fix CID - 10541 (from local server)
                            _hyperLink2.Dispose();
                        }
                        else
                        {
                            this.Controls.Add(_hyperLink);
                        }
                        // Coverity Fix CID - 10541 (from local server)
                        _hyperLink.Dispose();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    // Coverity Fix CID - 10541 (from local server)
                    if(linkTags!= null)
                        linkTags.Dispose();
                    if (div1Open != null)
                        div1Open.Dispose();
                    if (div2Open != null)
                        div2Open.Dispose();
                    if (div1Close != null)
                        div1Close.Dispose();
                    if (div2Close != null)
                        div2Close.Dispose(); 
                }
                               
            }
        }

        private string GetSpacerText(Group.IconSize size)
        {   string returnText = string.Empty;
            switch (size)
            {
                case Group.IconSize.none:
                   
                    break;
               
                case Group.IconSize.small:
                    returnText = "<br/><br/><br/>";
                    break;
                case Group.IconSize.medium:
                    returnText = "<br/><br/><br/><br/>";
                    break;
                case Group.IconSize.large:
                    returnText = "<br/><br/><br/><br/><br/>";
                    break;
                case Group.IconSize.xlarge:
                    returnText = "<br/><br/><br/><br/><br/><br/>";
                    break;
                case Group.IconSize.xxlarge:
                    returnText = "<br/><br/><br/><br/><br/><br/><br/>";
                    break;
               
            }

            return returnText;
        }

        public bool UserHasPrivilege(string privilegeName)
        {   
            COEPrincipal principal = (COEPrincipal)Csla.ApplicationContext.User;
            COEIdentity myIdentity = (COEIdentity)principal.Identity;


            bool hasAnyPriv = false;
            string[] myString = new string[1];
            myString[0] = "||";
            
            string[] privArray = privilegeName.Split(myString, StringSplitOptions.None);

            for (int i = 0; i < privArray.Length; i++)
            {
                if (privArray[i] == string.Empty)
                {
                    bool hasPriv = true;
                    hasAnyPriv = true;
                    break;
                }
                else
                {

                    bool hasPriv = myIdentity.IsInRole(privArray[i]);

                    if (hasPriv == true)
                    {
                        hasAnyPriv = true;
                        break;
                    }
                }
            }
            return hasAnyPriv;
        }     

        #endregion // Web Part Rendering methods and handlers





    }

    internal class CustomItemLoader
    {


        internal ICustomHomeItem InstantiateCustomItem(string assemblyName, string className)
        {
            ICustomHomeItem customItem;

            Assembly assembly = null;


            assembly = System.Reflection.Assembly.Load(assemblyName);

            customItem = (ICustomHomeItem)assembly.CreateInstance(className.Trim());


            return customItem;
        }

    }


}
