using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using CambridgeSoft.COE.Framework.Common;
using System.Xml;
using System.Web.UI;
using CambridgeSoft.COE.Framework.Common.Utility;
using System.Collections;
using System.Web;


namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class is used to query structures. Basic configuration displays a search type drop down, a chemdraw control and an options
    /// tab, that when clicked pops the structure options. Those options allows general and stereochemical preferences.
    /// </para>
    /// <para>
    /// The Search options can be fine tune configured using the SearchTypes tag:
    /// </para>
    /// <code language="xml">
    /// &lt;SearchTypes visible="" defaultType="" substructureSearch="" exactSearch="" fullSearch="" similaritySearch=""/&gt;
    /// </code>
    /// <list type="bullet">
    /// <item><b>visible:</b> Indicates if the dropdown with the search types will be shown to the user.</item>
    /// <item><b>defaultType:</b> Defines the search type to be used if none is selected by the user. That could happen when the search type
    /// configured to be hidden. Allowed values are: Similarity, FullStructure, Exact, Substructure.</item>
    /// <item><b>substructureSearch:</b> Specifies if substructure search is available. Allowed values are yes/no.</item>
    /// <item><b>exactSearch:</b> Specifies if exact search is available. Allowed values are yes/no.</item>
    /// <item><b>fullSearch:</b> Specifies if full search is available. Allowed values are yes/no.</item>
    /// <item><b>similaritySearch:</b> Specifies if similarity search is available. Allowed values are yes/no.</item>
    /// </list>
    /// <para>
    /// It is also posible to disallow the edition of structure search options using the EnableStructurePreferences tag:
    /// </para>
    /// <code language="xml">
    /// &lt;EnableStructurePreferences&gt;no&lt;/EnableStructurePreferences&gt;
    /// </code>
    /// <para>
    /// A general sample for this control may be the following:
    /// </para>
    /// <code language="xml">
    /// &lt;formElement&gt;
    ///   &lt;bindingExpression&gt;SearchCriteria.Items[0].Criterium&lt;/bindingExpression&gt;
    ///   &lt;searchCriteriaItem id="1" fieldid="205" tableid="203"&gt;
    ///       &lt;structureCriteria negate="NO" implementation="CsCartridge" cartridgeSchema="CSCartridge" absoluteHitsRel="NO" relativeTetStereo="NO" tetrahedralStereo="YES" simThreshold="100" reactionCenter="YES" fullSearch="NO" tautometer="NO" fragmentsOverlap="NO" permitExtraneousFragmentsIfRXN="NO" permitExtraneousFragments="NO" doubleBondStereo="YES" hitAnyChargeHetero="YES" identity="NO" hitAnyChargeCarbon="YES" similar="NO" format="base64cdx"/&gt;
    ///   &lt;/searchCriteriaItem&gt;
    ///   &lt;configInfo&gt;
    ///       &lt;fieldConfig&gt;
    ///         &lt;Links&gt;
    ///             &lt;CSSClass&gt;LinkButton&lt;/CSSClass&gt;
    ///         &lt;/Links&gt;
    ///         &lt;Labels&gt;
    ///             &lt;CSSClass&gt;COELabel&lt;/CSSClass&gt;
    ///         &lt;/Labels&gt;
    ///         &lt;ChemDraw&gt;
    ///             &lt;CSSClass&gt;COEChemDraw&lt;/CSSClass&gt;
    ///             &lt;ID&gt;Structure&lt;/ID&gt;
    ///         &lt;/ChemDraw&gt;
    ///         &lt;SearchTypes visible="yes" defaultType="Substructure" substructureSearch="yes" exactSearch="yes" fullSearch="yes" similaritySearch="yes" /&gt;
    ///         &lt;EnableStructurePreferences&gt;yes&lt;/EnableStructurePreferences&gt;
    ///       &lt;/fieldConfig&gt;
    ///   &lt;/configInfo&gt;
    ///   &lt;Id&gt;StructureQueryControl&lt;/Id&gt;
    ///       &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEStructureQuery&lt;/type&gt;
    ///   &lt;/displayInfo&gt;
    /// &lt;/formElement&gt;
    /// </code>
    /// </summary>
    [ValidationPropertyAttribute("Value")]
    [ToolboxData("<{0}:COEStructureQuery runat=server></{0}:COEStructureQuery>")]
    public class COEStructureQuery : CompositeControl, ICOEGenerableControl
    {

        #region Variables
        private SearchCriteria.StructureCriteria _criteria;
        private COEChemDraw _chemDrawControl;
        private DropDownList _searchTypeControl;
        private HtmlAnchor _clearStructureControl;
        private string _labelsClass;
        private string _linksClass;
        private HtmlImage _pluginDownloadImage;
        private string _downloadChemDrawImageSrc = string.Empty;
        private string _xmlDataAsString;
        private HtmlAnchor _preferencesLink;
        private StructureQueryPreferences _preferencesControl;
        private bool _showSearchTypes = true;
        private string _defaultType = "Substructure";
        private bool _substructureSearch = true;
        private bool _exactSearch = true;
        private bool _fullSearch = true;
        private bool _similaritySearch = true;
        #endregion

        #region Constructor
        public COEStructureQuery()
        {
            _criteria = new SearchCriteria.StructureCriteria();
            _criteria.Implementation = "CsCartridge";
            _criteria.FullSearch = SearchCriteria.COEBoolean.Yes;
            _criteria.Identity = SearchCriteria.COEBoolean.No;
            _criteria.Similar = SearchCriteria.COEBoolean.No;
            _chemDrawControl = new COEChemDraw();
            _clearStructureControl = new HtmlAnchor();
            _searchTypeControl = new DropDownList();
            _preferencesLink = new HtmlAnchor();
            _preferencesControl = new StructureQueryPreferences(_criteria);
        }

        private void InitializeControls()
        {
            if(_substructureSearch)
                _searchTypeControl.Items.Add(new ListItem("Substructure", "Substructure"));
            if(_fullSearch)
                _searchTypeControl.Items.Add(new ListItem("Full Structure", "FullStructure"));
            if(_exactSearch)
                _searchTypeControl.Items.Add(new ListItem("Exact", "Exact"));
            if(_similaritySearch)
                _searchTypeControl.Items.Add(new ListItem("Similarity", "Similarity"));

            if(!string.IsNullOrEmpty(_defaultType) && string.IsNullOrEmpty(SelectedType))
            {
                switch(_defaultType)
                {
                    case "Substructure":
                        _criteria.FullSearch = SearchCriteria.COEBoolean.No;
                        _criteria.Identity = SearchCriteria.COEBoolean.No;
                        _criteria.Similar = SearchCriteria.COEBoolean.No;
                        break;
                    case "FullStructure":
                        _criteria.FullSearch = SearchCriteria.COEBoolean.Yes;
                        _criteria.Identity = SearchCriteria.COEBoolean.No;
                        _criteria.Similar = SearchCriteria.COEBoolean.No;
                        break;
                    case "Exact":
                        _criteria.FullSearch = SearchCriteria.COEBoolean.No;
                        _criteria.Identity = SearchCriteria.COEBoolean.Yes;
                        _criteria.Similar = SearchCriteria.COEBoolean.No;
                        break;
                    case "Similarity":
                        _criteria.FullSearch = SearchCriteria.COEBoolean.Yes;
                        _criteria.Identity = SearchCriteria.COEBoolean.No;
                        _criteria.Similar = SearchCriteria.COEBoolean.Yes;
                        break;
                }
                _searchTypeControl.SelectedValue = SelectedType = _defaultType;
            }

            _clearStructureControl.InnerText = "Clear Structure";
            _preferencesLink.InnerText = "O P T I O N S";
        }
        #endregion

        #region Properties
        public string Value
        {
            get
            {
                if(!ShowPluginDownload)
                    return ((SearchCriteria.StructureCriteria) GetData()).Value;
                else
                    return _criteria.Value;
            }
        }

        private bool IsCDP
        {
            get
            {
                bool isCDP = false;
                if(Page != null)
                    isCDP = (Page.Session["isCDP"] != null && (bool) Page.Session["isCDP"]);
                else
                    isCDP = (((Page) HttpContext.Current.CurrentHandler).Session["isCDP"] != null && (bool) ((Page) HttpContext.Current.CurrentHandler).Session["isCDP"]);

                return isCDP;
            }
        }

        private bool ShowPluginDownload
        {
            get
            {
                bool show = false;
                if(Page != null)
                    show = (Page.Session["ShowPluginDownload"] != null && (bool) Page.Session["ShowPluginDownload"]);
                else
                    show = (((Page) HttpContext.Current.CurrentHandler).Session["ShowPluginDownload"] != null && (bool) ((Page) HttpContext.Current.CurrentHandler).Session["ShowPluginDownload"]);

                return show;
            }
        }

        private string PluginDownloadURL
        {
            get
            {
                string url = string.Empty;
                if(Page != null)
                {
                    if(Page.Session["PluginDownloadURL"] != null)
                        url = (string) Page.Session["PluginDownloadURL"];
                }
                else
                {
                    if(((Page) HttpContext.Current.CurrentHandler).Session["PluginDownloadURL"] != null)
                        url = (string) ((Page) HttpContext.Current.CurrentHandler).Session["PluginDownloadURL"];
                }

                return url;
            }
        }

        private string DownloadChemDrawImageSrc
        {
            get
            {
                if(string.IsNullOrEmpty(_downloadChemDrawImageSrc))
                    return Page.ClientScript.GetWebResourceUrl(typeof(COEStructureQuery), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.DownloadChemDraw.jpg");
                else
                    return _downloadChemDrawImageSrc;
            }
            set
            {
                _downloadChemDrawImageSrc = value;
            }
        }

        private bool EnableStructurePreferences
        {
            get
            {
                if(ViewState["EnableStructurePreferences"] == null)
                    ViewState["EnableStructurePreferences"] = true;

                return (bool) ViewState["EnableStructurePreferences"];
            }
            set
            {
                ViewState["EnableStructurePreferences"] = value;
            }
        }

        private string SelectedType
        {
            get
            {
                return ViewState["SelectedType"] as string;
            }
            set
            {
                ViewState["SelectedType"] = value;
            }
        }
        #endregion

        #region ICOEGenerableControl Members

        public object GetData()
        {
            if(IsCDP)
            {
                UpdateStructureCriteria();
            }
            return _criteria;
        }

        private void UpdateStructureCriteria()
        {
            if(EnableStructurePreferences)
                _criteria = _preferencesControl.UpdateStructureCriteria();

            if(string.IsNullOrEmpty(_searchTypeControl.SelectedValue) && !string.IsNullOrEmpty(_defaultType))
                SelectedType = _defaultType;
            else if(!string.IsNullOrEmpty(_searchTypeControl.SelectedValue) && _showSearchTypes)
                SelectedType = _searchTypeControl.SelectedValue;
            //1) Substructure - IDENTITY=NO FullSearch=NO
            //2) EXACT - IDENTITY=YES FullSearch=NO
            //3) FULL = FullSearch=YES IDENTITY=NO
            switch(SelectedType)
            {
                case "Substructure":
                    _criteria.FullSearch = SearchCriteria.COEBoolean.No;
                    _criteria.Identity = SearchCriteria.COEBoolean.No;
                    _criteria.Similar = SearchCriteria.COEBoolean.No;
                    break;
                case "FullStructure":
                    _criteria.FullSearch = SearchCriteria.COEBoolean.Yes;
                    _criteria.Identity = SearchCriteria.COEBoolean.No;
                    _criteria.Similar = SearchCriteria.COEBoolean.No;
                    break;
                case "Exact":
                    _criteria.FullSearch = SearchCriteria.COEBoolean.No;
                    _criteria.Identity = SearchCriteria.COEBoolean.Yes;
                    _criteria.Similar = SearchCriteria.COEBoolean.No;
                    break;
                case "Similarity":
                    _criteria.FullSearch = SearchCriteria.COEBoolean.Yes;
                    _criteria.Identity = SearchCriteria.COEBoolean.No;
                    _criteria.Similar = SearchCriteria.COEBoolean.Yes;
                    break;
            }

            _criteria.Structure = _chemDrawControl.GetData().ToString();
        }

        public void PutData(object data)
        {
            if(!(data is SearchCriteria.StructureCriteria))
            {
                if(data is string)
                {
                    try
                    {
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(data.ToString());
                        _criteria = new SearchCriteria.StructureCriteria(xmlDocument.FirstChild);
                    }
                    catch(Exception)
                    {
                        throw new Exception("COEStructureQuery expects to be bound to a " + typeof(SearchCriteria.StructureCriteria).FullName + " object type");
                    }
                }
                else
                    throw new Exception("COEStructureQuery expects to be bound to a " + typeof(SearchCriteria.StructureCriteria).FullName + " object type");
            }
            else
            {
                _criteria = (SearchCriteria.StructureCriteria) data;
            }

            UpdateSelectedType();

            if(EnableStructurePreferences)
                _preferencesControl.Criteria = _criteria;
        }

        private void UpdateSelectedType()
        {
            if(_criteria.Similar == SearchCriteria.COEBoolean.Yes)
            {
                _searchTypeControl.SelectedValue = this.SelectedType = "Similarity";
            }
            else if(_criteria.FullSearch == SearchCriteria.COEBoolean.Yes)
            {
                _searchTypeControl.SelectedValue = this.SelectedType = "FullStructure";
            }
            else if(_criteria.Identity == SearchCriteria.COEBoolean.Yes)
            {
                _searchTypeControl.SelectedValue = this.SelectedType = "Exact";
            }
            else
            {
                _searchTypeControl.SelectedValue = this.SelectedType = "Substructure";
            }
        }

        public void LoadFromXml(string xmlDataAsString)
        {
            _xmlDataAsString = xmlDataAsString;
        }

        public void LoadFromXml()
        {
            /*
                <ChemDrawID>Structure</ChemDrawID>
                <Height>350px</Height>
                <Width>350px</Width>
            */
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(_xmlDataAsString);

            XmlNamespaceManager manager = new XmlNamespaceManager(xmlDocument.NameTable);
            manager.AddNamespace("COE", xmlDocument.DocumentElement.NamespaceURI);

            XmlNode xmlData = xmlDocument.SelectSingleNode("//COE:fieldConfig", manager);

            XmlNode width = xmlData.SelectSingleNode("./COE:Width", manager);
            if(width != null && width.InnerText.Length > 0)
            {
                this.Width = new Unit(width.InnerText);
            }

            XmlNode cssClass = xmlData.SelectSingleNode("./COE:CSSClass", manager);
            if(cssClass != null && cssClass.InnerText.Length > 0)
            {
                this.CssClass = cssClass.InnerText;
            }

            XmlNode height = xmlData.SelectSingleNode("./COE:Height", manager);
            if(height != null && height.InnerText.Length > 0)
            {
                this.Height = new Unit(height.InnerText);
            }

            XmlNode downloadChemDrawImageSrc = xmlData.SelectSingleNode("./COE:DownloadChemDrawImageSrc", manager);
            if(downloadChemDrawImageSrc != null && downloadChemDrawImageSrc.InnerText.Length > 0)
            {
                _downloadChemDrawImageSrc = downloadChemDrawImageSrc.InnerText;
            }
            else if(Page.Session["DownloadChemDrawImageSrc"] != null)
                _downloadChemDrawImageSrc = (string) Page.Session["DownloadChemDrawImageSrc"];

            XmlNode chemDraw = xmlData.SelectSingleNode("./COE:ChemDraw", manager);
            _chemDrawControl.LoadFromXml(chemDraw.OuterXml);

            XmlNode labels = xmlData.SelectSingleNode("./COE:Labels", manager);
            if(labels != null && labels.InnerText.Length > 0)
            {
                cssClass = labels.SelectSingleNode("./COE:CSSClass", manager);
                if(cssClass != null && cssClass.InnerText.Length > 0)
                {
                    _labelsClass = cssClass.InnerText;
                }
            }

            XmlNode links = xmlData.SelectSingleNode("./COE:Links", manager);
            if(links != null && links.InnerText.Length > 0)
            {
                cssClass = links.SelectSingleNode("./COE:CSSClass", manager);
                if(cssClass != null && cssClass.InnerText.Length > 0)
                {
                    _linksClass = cssClass.InnerText;
                }
            }

            XmlNode enableStructurePreferences = xmlData.SelectSingleNode("./COE:EnableStructurePreferences", manager);
            if(enableStructurePreferences != null && !string.IsNullOrEmpty(enableStructurePreferences.InnerText))
                EnableStructurePreferences = enableStructurePreferences.InnerText.ToLower() == "true" || enableStructurePreferences.InnerText.ToLower() == "yes";

            // <SearchTypes visible="no" fullSearch="no" substructureSearch="no" exactSearch="no" defaultType="FullStructure" />
            // <!-- defaultTypes allowable values: Substructure, FullStructure, Exact -->
            XmlNode searchTypes = xmlData.SelectSingleNode("./COE:SearchTypes", manager);
            if(searchTypes != null)
            {
                if(searchTypes.Attributes["visible"] != null && !string.IsNullOrEmpty(searchTypes.Attributes["visible"].Value))
                    _showSearchTypes = searchTypes.Attributes["visible"].Value.ToLower() == "true" || searchTypes.Attributes["visible"].Value.ToLower() == "yes";
                if(searchTypes.Attributes["defaultType"] != null && !string.IsNullOrEmpty(searchTypes.Attributes["defaultType"].Value))
                    _defaultType = searchTypes.Attributes["defaultType"].Value;

                if(searchTypes.Attributes["substructureSearch"] != null && !string.IsNullOrEmpty(searchTypes.Attributes["substructureSearch"].Value))
                    _substructureSearch = searchTypes.Attributes["substructureSearch"].Value.ToLower() == "true" || searchTypes.Attributes["substructureSearch"].Value.ToLower() == "yes";

                if(searchTypes.Attributes["exactSearch"] != null && !string.IsNullOrEmpty(searchTypes.Attributes["exactSearch"].Value))
                    _exactSearch = searchTypes.Attributes["exactSearch"].Value.ToLower() == "true" || searchTypes.Attributes["exactSearch"].Value.ToLower() == "yes";

                if(searchTypes.Attributes["fullSearch"] != null && !string.IsNullOrEmpty(searchTypes.Attributes["fullSearch"].Value))
                    _fullSearch = searchTypes.Attributes["fullSearch"].Value.ToLower() == "true" || searchTypes.Attributes["fullSearch"].Value.ToLower() == "yes";

                if(searchTypes.Attributes["similaritySearch"] != null && !string.IsNullOrEmpty(searchTypes.Attributes["similaritySearch"].Value))
                    _similaritySearch = searchTypes.Attributes["similaritySearch"].Value.ToLower() == "true" || searchTypes.Attributes["similaritySearch"].Value.ToLower() == "yes";
            }
        }

        public string DefaultValue
        {
            get
            {
                return string.Empty;
            }
            set
            {
                ;
            }
        }

        #endregion

        #region CompositeControl Methods
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            LoadFromXml();
        }
        
        protected override void CreateChildControls()
        {
            if(IsCDP)
            {
                this.InitializeControls();
                #region Preferences Control
                if(EnableStructurePreferences)
                {
                    _preferencesControl.ID = "PreferencesControl";
                    this.Controls.Add(_preferencesControl);
                }
                #endregion

                #region Search Type

                Label searchTypeLabel = new Label();
                Panel dropDownContainer = new Panel();
                dropDownContainer.ID = "SearchTypeDiv";
                this.Controls.Add(dropDownContainer);

                if(!_showSearchTypes)
                    dropDownContainer.Style.Add(HtmlTextWriterStyle.Display, "none");
                else
                    dropDownContainer.Style.Add(HtmlTextWriterStyle.Margin, "2px");

                dropDownContainer.Controls.Add(searchTypeLabel);
                dropDownContainer.Controls.Add(_searchTypeControl);
                searchTypeLabel.Text = "Search Type";
                if(!string.IsNullOrEmpty(_labelsClass))
                    searchTypeLabel.CssClass = _labelsClass;
                searchTypeLabel.Style.Add(HtmlTextWriterStyle.Margin, "2px");
                _searchTypeControl.Style.Add(HtmlTextWriterStyle.Margin, "2px");

                #endregion

                #region ChemDraw
                Panel chemDrawContainer = new Panel();
                chemDrawContainer.ID = "StructureDiv";
                this.Controls.Add(chemDrawContainer);
                chemDrawContainer.Attributes.Add("class", "COEStructureQueryCDContainer");
                chemDrawContainer.Controls.Add(_chemDrawControl);
                _chemDrawControl.Style.Add(HtmlTextWriterStyle.Margin, "2px");
                chemDrawContainer.Style.Add(HtmlTextWriterStyle.Margin, "2px");
                #endregion

                #region Preferences Tab
                if(EnableStructurePreferences)
                {
                    Panel preferencesTab = new Panel();
                    preferencesTab.ID = "PreferencesTab";
                    this.Controls.Add(preferencesTab);
                    _preferencesLink.ID = "PreferencesLink";
                    preferencesTab.Controls.Add(_preferencesLink);
                    _preferencesLink.Attributes.Add("class", "cdPreferencesLink");
                    preferencesTab.Attributes.Add("class", "cdPreferencesTab");
                    _preferencesLink.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                    _preferencesControl.ShowingControlsIds.Add(preferencesTab.ClientID);
                    _preferencesControl.ChemDrawControlsToHide.Add(_chemDrawControl.ClientID);
                }
                #endregion

                #region ClearStructure
                Panel clearStructureContainer = new Panel();
                clearStructureContainer.Style.Add("clear", "both");
                clearStructureContainer.ID = "CleanStructureDiv";
                this.Controls.Add(clearStructureContainer);
                clearStructureContainer.Controls.Add(_clearStructureControl);
                clearStructureContainer.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
                if(!string.IsNullOrEmpty(_linksClass))
                    _clearStructureControl.Attributes["class"] = _linksClass;
                _clearStructureControl.Style.Add(HtmlTextWriterStyle.Margin, "4px");
                _clearStructureControl.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");

                clearStructureContainer.Style.Add(HtmlTextWriterStyle.Margin, "4px");
                #endregion
            }
            else if(ShowPluginDownload)
            {
                _pluginDownloadImage = new HtmlImage();
                _pluginDownloadImage.ID = "DownloadPluginImage";
                this.Controls.Add(_pluginDownloadImage);
                _pluginDownloadImage.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
                _pluginDownloadImage.Src = Page.ResolveClientUrl(DownloadChemDrawImageSrc);
                _pluginDownloadImage.Attributes.Add("onclick", "window.open('" + PluginDownloadURL + "')");
            }
            //base.CreateChildControls();
            ChildControlsCreated = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if(IsCDP)
            {
                _chemDrawControl.PutData(_criteria.Structure);
                if(string.IsNullOrEmpty(SelectedType))
                {
                    UpdateSelectedType();
                }
                else
                    _searchTypeControl.SelectedValue = SelectedType;
                _clearStructureControl.Attributes.Add("onclick", "cd_clear(\"" + _chemDrawControl.ClientID + "\");");
            }
        }

        #endregion
    }

    class StructureQueryPreferences : CompositeControl
    {

        #region Constants
        private const string YAHOODOMEVENTS = "yahoo-dom-event";
        private const string DRAGDROPMIN = "dragdrop-min";
        private const string CONTAINERMIN = "container-min";
        #endregion

        #region Variables
        private string _contentsCssClass = "contents";
        private string _labelCssClass = "labelCell";
        private string _valuesCssClass = "valueCell";

        private string _title = string.Empty;
        private string _footer = string.Empty;
        private List<string> _showingControlsIds = new List<string>();
        private List<string> _chemDrawControlsToHide = new List<string>();

        private CheckBox _hitAnyChargeCheckBox;
        private CheckBox _reactionQueryCheckBox;
        private CheckBox _hitAnyChargeOnCarbonCheckBox;
        private CheckBox _permitExtraneousFragmentsInFullCheckBox;
        private CheckBox _permitExtraneousFragmentsInReactionFullCheckBox;
        private CheckBox _fragmentsOverlapCheckBox;
        private CheckBox _tautomericCheckBox;
        private TextBox _similarityTextBox;
        private CheckBox _fullStructureSimilarityCheckBox;
        private CheckBox _matchStereochemistry;
        private RadioButton _matchTetrahedralStereoSame;
        private RadioButton _matchTetrahedralStereoEither;
        private RadioButton _matchTetrahedralStereoAny;
        private RadioButton _matchDoubleBondStereoSame;
        private RadioButton _matchDoubleBondStereoAny;
        private CheckBox _thickBondsRelCheckBox;
        private HtmlInputHidden _selectedTab;
        private SearchCriteria.StructureCriteria _criteria;
        #endregion

        #region Properties
        public List<string> ShowingControlsIds
        {
            get
            {
                return _showingControlsIds;
            }
        }

        public List<string> ChemDrawControlsToHide
        {
            get
            {
                return _chemDrawControlsToHide;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        public string Footer
        {
            get
            {
                return _footer;
            }
            set
            {
                _footer = value;
            }
        }

        public string ContentsCssClass
        {
            set
            {
                _contentsCssClass = value;
            }
        }

        public SearchCriteria.StructureCriteria Criteria
        {
            set
            {
                _criteria = value;
                InitializeValues();
            }
            get
            {
                return _criteria;
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }
        #endregion

        #region Constructors
        public StructureQueryPreferences(SearchCriteria.StructureCriteria criteria)
        {
            _criteria = criteria;
            _criteria.Implementation = "CsCartridge";
            _criteria.FullSearch = SearchCriteria.COEBoolean.Yes;
            _criteria.Identity = SearchCriteria.COEBoolean.No;

            _hitAnyChargeCheckBox = new CheckBox();
            _hitAnyChargeCheckBox.ID = "HitAnyChargeHeteroCheck";
            _reactionQueryCheckBox = new CheckBox();
            _reactionQueryCheckBox.ID = "ReactionQueryCheck";
            _hitAnyChargeOnCarbonCheckBox = new CheckBox();
            _hitAnyChargeOnCarbonCheckBox.ID = "HitAnyChargeCarbonCheck";
            _permitExtraneousFragmentsInFullCheckBox = new CheckBox();
            _permitExtraneousFragmentsInFullCheckBox.ID = "PermitExtraneousFragmentCheck";
            _permitExtraneousFragmentsInReactionFullCheckBox = new CheckBox();
            _permitExtraneousFragmentsInReactionFullCheckBox.ID = "PermitExtraneousFragmentIdRXNCheck";
            _fragmentsOverlapCheckBox = new CheckBox();
            _fragmentsOverlapCheckBox.ID = "FragmentsOverlapCheck";
            _tautomericCheckBox = new CheckBox();
            _tautomericCheckBox.ID = "TautomerCheck";
            _similarityTextBox = new TextBox();
            _similarityTextBox.ID = "SimilarityTextBox";
            _fullStructureSimilarityCheckBox = new CheckBox();
            _fullStructureSimilarityCheckBox.ID = "FullStructureSimilarityCheck";
            _matchStereochemistry = new CheckBox();
            _matchStereochemistry.ID = "MatchStereochemistryCheck";
            _matchTetrahedralStereoSame = new RadioButton();
            _matchTetrahedralStereoSame.ID = "MatchTetrahedralSameCheck";
            _matchTetrahedralStereoEither = new RadioButton();
            _matchTetrahedralStereoEither.ID = "MatchTetrahedralEitherCheck";
            _matchTetrahedralStereoAny = new RadioButton();
            _matchTetrahedralStereoAny.ID = "MatchTetrahedralAnyCheck";
            _matchDoubleBondStereoSame = new RadioButton();
            _matchDoubleBondStereoSame.ID = "MatchDoubleBondStereoSameCheck";
            _matchDoubleBondStereoAny = new RadioButton();
            _matchDoubleBondStereoAny.ID = "MatchDoubleBondStereoAnyCheck";
            _thickBondsRelCheckBox = new CheckBox();
            _thickBondsRelCheckBox.ID = "ThickBondsRelCheck";
            _selectedTab = new HtmlInputHidden();
            _selectedTab.ID = "SelectedTab";
            _selectedTab.Value = "0";
            _matchTetrahedralStereoSame.GroupName = "TetrahedralStereoGroup";
            _matchTetrahedralStereoSame.Text = SearchCriteria.TetrahedralStereoMatching.Same.ToString();
            _matchTetrahedralStereoEither.GroupName = "TetrahedralStereoGroup";
            _matchTetrahedralStereoEither.Text = SearchCriteria.TetrahedralStereoMatching.Either.ToString();
            _matchTetrahedralStereoAny.GroupName = "TetrahedralStereoGroup";
            _matchTetrahedralStereoAny.Text = SearchCriteria.TetrahedralStereoMatching.Any.ToString();
            _matchDoubleBondStereoSame.GroupName = "DoubleBondsGroup";
            _matchDoubleBondStereoSame.Text = SearchCriteria.TetrahedralStereoMatching.Same.ToString();
            _matchDoubleBondStereoAny.GroupName = "DoubleBondsGroup";
            _matchDoubleBondStereoAny.Text = SearchCriteria.TetrahedralStereoMatching.Any.ToString();
            InitializeValues();
        }
        #endregion

        #region Life cycle Events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.RegisterRequiresControlState(this);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.YAHOODOMEVENTS);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.DRAGDROPMIN);
            FrameworkUtils.RegisterYUIScript(this.Page, FrameworkConstants.YUI_JS.CONTAINERMIN);
            CambridgeSoft.COE.Framework.Common.FrameworkUtils.AddYUICSSReference(this.Page,
                CambridgeSoft.COE.Framework.Common.FrameworkConstants.YUI_CSS.CONTAINER);

        }

        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            this.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
            this.Style.Add(HtmlTextWriterStyle.Display, "none");

            this.Controls.Add(_hitAnyChargeCheckBox);
            this.Controls.Add(_reactionQueryCheckBox);
            this.Controls.Add(_hitAnyChargeOnCarbonCheckBox);
            this.Controls.Add(_permitExtraneousFragmentsInFullCheckBox);
            this.Controls.Add(_permitExtraneousFragmentsInReactionFullCheckBox);
            this.Controls.Add(_fragmentsOverlapCheckBox);
            this.Controls.Add(_tautomericCheckBox);
            this.Controls.Add(_similarityTextBox);
            this.Controls.Add(_fullStructureSimilarityCheckBox);
            this.Controls.Add(_matchStereochemistry);
            this.Controls.Add(_matchTetrahedralStereoSame);
            this.Controls.Add(_matchTetrahedralStereoEither);
            this.Controls.Add(_matchTetrahedralStereoAny);
            this.Controls.Add(_matchDoubleBondStereoSame);
            this.Controls.Add(_matchDoubleBondStereoAny);
            this.Controls.Add(_thickBondsRelCheckBox);
            this.Controls.Add(_selectedTab);

            _similarityTextBox.MaxLength = 3;
            _similarityTextBox.Width = new Unit("40px");

            ChildControlsCreated = true;
        }

        private void RenderScripts(HtmlTextWriter writer)
        {
            string changeSelectedTabStr = @"
        function ChangeSelectedTab" + this.ClientID + @"(selectedTab)
        {
            document.getElementById('" + _selectedTab.ClientID + @"').value = selectedTab; 
            document.getElementById('tab' + selectedTab).className = 'COEStructureQueryTabSelected'; 
            document.getElementById('contentsTable' + selectedTab).style.display = 'block'; 
            var inactiveTab = (selectedTab == '0')? '1' : '0';
            document.getElementById('tab' + inactiveTab).className = 'COEStructureQueryTab'; 
            document.getElementById('contentsTable' + inactiveTab).style.display = 'none';
        }
        function SetStereochemistryOptionsEnabled" + this.ClientID + @"(enabled)
        {
            document.getElementById('" + _matchTetrahedralStereoSame.ClientID + @"').disabled = !enabled;
            document.getElementById('" + _matchTetrahedralStereoEither.ClientID + @"').disabled = !enabled;
            document.getElementById('" + _matchTetrahedralStereoAny.ClientID + @"').disabled = !enabled;
            document.getElementById('" + _matchDoubleBondStereoSame.ClientID + @"').disabled = !enabled;
            document.getElementById('" + _matchDoubleBondStereoAny.ClientID + @"').disabled = !enabled;
            document.getElementById('" + _thickBondsRelCheckBox.ClientID + @"').disabled = !enabled;
        }";
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            writer.RenderBeginTag(HtmlTextWriterTag.Script);
            writer.Write(changeSelectedTabStr);
            writer.RenderEndTag();
        }

        protected void RenderHeader(HtmlTextWriter writer)
        {
            writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "1em");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "hd");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(string.IsNullOrEmpty(_title) ? "Search Preferences" : _title);
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        protected void RenderFooter(HtmlTextWriter writer)
        {
            writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "1em");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "ft");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(string.IsNullOrEmpty(_footer) ? "Please select your search options." : _footer);
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        protected void RenderBody(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "bd");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "0px");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryTabsContainer");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            _selectedTab.RenderControl(writer);
            if(_selectedTab.Value == "0")
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryTabSelected");
            else
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryTab");
            writer.AddAttribute("id", "tab0");
            writer.AddAttribute("onclick", "ChangeSelectedTab" + this.ClientID + @"('0');");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write("General");
            writer.RenderEndTag();

            if(_selectedTab.Value == "1")
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryTabSelected");
            else
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryTab");
            writer.AddAttribute("id", "tab1");
            writer.AddAttribute("onclick", "ChangeSelectedTab" + this.ClientID + @"('1');");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write("Stereochemistry");
            writer.RenderEndTag();

            this.RenderGeneralTabContents(writer);
            this.RenderStereochemistryTabContents(writer);
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        private void RenderGeneralTabContents(HtmlTextWriter writer)
        {

            if(_selectedTab.Value == "0")
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "block");
            }
            else
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "contentsTable0");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, _contentsCssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            #region Hit any charge on heteroatom
            RenderPreferenceRow("Hit any charge on heteroatom", _hitAnyChargeCheckBox, writer);
            #endregion

            #region Reaction query must hit reaction center
            RenderPreferenceRow("Reaction query must hit reaction center", _reactionQueryCheckBox, writer);
            #endregion

            #region Hit any charge on carbon
            RenderPreferenceRow("Hit any charge on carbon", _hitAnyChargeOnCarbonCheckBox, writer);
            #endregion

            #region Permit extraneous fragments in Full Structure Searches
            RenderPreferenceRow("Permit extraneous fragments in Full Structure Searches", _permitExtraneousFragmentsInFullCheckBox, writer);
            #endregion

            #region Permit extraneous fragments in Reaction Full Structure Searches
            RenderPreferenceRow("Permit extraneous fragments in Reaction Full Structure Searches", _permitExtraneousFragmentsInReactionFullCheckBox, writer);
            #endregion

            #region Query fragments can overlap in target
            RenderPreferenceRow("Query fragments can overlap in target", _fragmentsOverlapCheckBox, writer);
            #endregion

            #region Tautomeric
            RenderPreferenceRow("Tautomeric", _tautomericCheckBox, writer);
            #endregion

            #region Similarity search (20-100%)
            using (Label percentageLabel = new Label())     //Coverity Fix CID : 11831 
            {
                percentageLabel.Text = " %";
                RenderPreferenceRow("Similarity search (20-100%)", _similarityTextBox, percentageLabel, writer);
            }
            #endregion

            #region Full structure similarity
            RenderPreferenceRow("Full structure similarity", _fullStructureSimilarityCheckBox, writer);
            #endregion

            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        private void RenderStereochemistryTabContents(HtmlTextWriter writer)
        {

            if(_selectedTab.Value == "1")
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "block");
            }
            else
            {
                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "contentsTable1");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            this.RenderMatchStereoChemistryPanel(writer);
            this.RenderTetrahedralStereoPanel(writer);
            this.RenderThickBondsPanel(writer);
            this.RenderDoubleBondsPanel(writer);

            writer.RenderEndTag();
        }

        private void RenderMatchStereoChemistryPanel(HtmlTextWriter writer)
        {
            _matchStereochemistry.Text = "Match Stereochemistry";
            _matchStereochemistry.InputAttributes.Add("onclick", "SetStereochemistryOptionsEnabled" + this.ClientID + "(this.checked);");
            //matchStereochemistry Panel 
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryGroupPanel");
            writer.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "center");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "20px");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            _matchStereochemistry.RenderControl(writer);
            writer.RenderEndTag();
        }

        private void RenderTetrahedralStereoPanel(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryGroupPanel");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.Fieldset);
            writer.RenderBeginTag(HtmlTextWriterTag.Legend);
            writer.Write("Tetrahedral stereo center hits: ");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, "1");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            _matchTetrahedralStereoSame.RenderControl(writer);
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.st_up.GIF"));
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.st_up.GIF"));
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            _matchTetrahedralStereoEither.RenderControl(writer);
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.st_up.GIF"));
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.st_down.GIF"));
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            _matchTetrahedralStereoAny.RenderControl(writer);
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.st_up.GIF"));
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.st_down.GIF"));
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.st_any.GIF"));
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        private void RenderThickBondsPanel(HtmlTextWriter writer)
        {
            _thickBondsRelCheckBox.Text = "Thick bonds represent relative stereochemistry";
            //thickbonds panel
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryGroupPanel");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            //images container
            writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "20px");
            writer.AddStyleAttribute(HtmlTextWriterStyle.MarginTop, "10px");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            //image st_up_thick
            writer.AddAttribute("src", Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.st_up_thick.GIF"));
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            //image st_down_thick
            writer.AddAttribute("src", Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.st_down_thick.GIF"));
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.RenderEndTag();
            _thickBondsRelCheckBox.RenderControl(writer);
            writer.RenderEndTag();
        }

        private void RenderDoubleBondsPanel(HtmlTextWriter writer)
        {
            //doubleBonds Panel
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryGroupPanel");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.Fieldset);
            writer.RenderBeginTag(HtmlTextWriterTag.Legend);
            writer.Write("Double bonds hits: ");
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute(HtmlTextWriterAttribute.Rowspan, "1");
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            _matchDoubleBondStereoSame.RenderControl(writer);
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.std_cis.GIF"));
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.std_cis.GIF"));
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            _matchDoubleBondStereoAny.RenderControl(writer);
            writer.RenderEndTag();
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.std_cis.GIF"));
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.std_either.GIF"));
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "COEStructureQueryImage32x32");
            writer.AddAttribute(HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl(typeof(StructureQueryPreferences), "CambridgeSoft.COE.Framework.ServerControls.FormGenerator.Images.std_trans.GIF"));
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        private void RenderPreferenceRow(string labelText, WebControl valueControl, HtmlTextWriter writer)
        {
            this.RenderPreferenceRow(labelText, valueControl, null, writer);
        }

        private void RenderPreferenceRow(string labelText, WebControl valueControl, WebControl labelForValueControl, HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, _labelCssClass);

            if(valueControl == null)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
                writer.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, "bold");
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Td);
            writer.Write(labelText);
            writer.RenderEndTag();

            if(valueControl != null)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, _valuesCssClass);
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                valueControl.RenderControl(writer);
                if(labelForValueControl != null)
                {
                    labelForValueControl.RenderControl(writer);
                }
                writer.RenderEndTag();
            }

            writer.RenderEndTag();
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            this.RenderScripts(writer);
            this.RenderHeader(writer);
            this.RenderBody(writer);
            this.RenderFooter(writer);
        }

        protected override object SaveControlState()
        {
            object[] states = new object[2];
            states[0] = base.SaveControlState();
            states[1] = _criteria;
            return states;
        }

        protected override void LoadControlState(object savedState)
        {

            object[] states = savedState as object[];
            if(states != null && states.Length == 2)
            {
                base.LoadControlState(states[0]);
                _criteria = (SearchCriteria.StructureCriteria) states[1];
            }
            else
                base.LoadControlState(savedState);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            string initScript = @"
<script type='text/javascript'>
    YAHOO.namespace('COEFormGenerator');

    function showControls" + this.ClientID + @"() {
        document.getElementById('" + this.ClientID + @"').style.visibility='hidden';
        document.getElementById('" + this.ClientID + @"').style.display='none';
        ";
            foreach(string controlId in _chemDrawControlsToHide)
            {
                initScript += @"
        if(cd_getSpecificObject('" + controlId + @"') != 'undefined' 
            && cd_getSpecificObject('" + controlId + @"') != null) {
            cd_getSpecificObject('" + controlId + @"').style.visibility = 'visible';
            cd_getSpecificObject('" + controlId + @"').style.display = 'block';
        }";
            }
            initScript += @"
    }

    function hideControls" + this.ClientID + @"() {
        document.getElementById('" + this.ClientID + @"').style.visibility='visible';
        document.getElementById('" + this.ClientID + @"').style.display='block';
        ";
            foreach(string controlId in _chemDrawControlsToHide)
            {
                initScript += @"
        if(cd_getSpecificObject('" + controlId + @"') != 'undefined' 
            && cd_getSpecificObject('" + controlId + @"') != null) {
            cd_getSpecificObject('" + controlId + @"').style.visibility = 'hidden';
            cd_getSpecificObject('" + controlId + @"').style.display = 'none';
        }";
            }
            initScript += @"
    }

    function initSearchPref() {
	    YAHOO.COEFormGenerator.searchPreferences" + this.ClientID + @" = new YAHOO.widget.Panel('" + this.ClientID + @"', { width:'400px', 
                                                                                                      modal:false, 
                                                                                                      draggable:false,
                                                                                                      visible:false, 
                                                                                                      constraintoviewport:true,
                                                                                                      context:['" + Parent.ClientID + @"','tl','tl']} );
	    YAHOO.COEFormGenerator.searchPreferences" + this.ClientID + @".render(document.body.form);

        YAHOO.COEFormGenerator.searchPreferences" + this.ClientID + @".beforeHideEvent.subscribe(showControls" + this.ClientID + @");
        YAHOO.COEFormGenerator.searchPreferences" + this.ClientID + @".beforeShowEvent.subscribe(hideControls" + this.ClientID + @");
		";
            foreach(string showingControlId in _showingControlsIds)
            {
                initScript += @"
        YAHOO.util.Event.addListener('" + showingControlId + "', 'click', YAHOO.COEFormGenerator.searchPreferences" + this.ClientID + @".show, YAHOO.COEFormGenerator.searchPreferences" + this.ClientID + @", true);";
            }

            initScript += @"

	}

    YAHOO.util.Event.addListener(window, 'load', initSearchPref);
</script>";

            writer.Write(initScript);
        }

        #endregion

        #region Methods

        internal SearchCriteria.StructureCriteria UpdateStructureCriteria()
        {
            EnsureChildControls();
            Criteria.HitAnyChargeHetero = COEConvert.ToCOEBoolean(_hitAnyChargeCheckBox.Checked.ToString());
            Criteria.ReactionCenter = COEConvert.ToCOEBoolean(_reactionQueryCheckBox.Checked.ToString());
            Criteria.HitAnyChargeCarbon = COEConvert.ToCOEBoolean(_hitAnyChargeOnCarbonCheckBox.Checked.ToString());
            Criteria.PermitExtraneousFragments = COEConvert.ToCOEBoolean(_permitExtraneousFragmentsInFullCheckBox.Checked.ToString());
            Criteria.PermitExtraneousFragmentsIfRXN = COEConvert.ToCOEBoolean(_permitExtraneousFragmentsInReactionFullCheckBox.Checked.ToString());
            Criteria.FragmentsOverlap = COEConvert.ToCOEBoolean(_fragmentsOverlapCheckBox.Checked.ToString());
            Criteria.Tautometer = COEConvert.ToCOEBoolean(_tautomericCheckBox.Checked.ToString());
            // TODO: Criteria.AbsoluteHitsRel is not configurable
            if(!string.IsNullOrEmpty(_similarityTextBox.Text) && Convert.ToInt32(_similarityTextBox.Text) > 19 && Convert.ToInt32(_similarityTextBox.Text) < 101)
            {
                Criteria.SimThreshold = Convert.ToInt32(_similarityTextBox.Text);
            }
            else
            {
                Criteria.SimThreshold = 90;
            }
            Criteria.FullSearch = COEConvert.ToCOEBoolean(_fullStructureSimilarityCheckBox.Checked.ToString());
            if(_matchStereochemistry.Checked)
            {
                Criteria.TetrahedralStereo = FillFromCheckBoxes();
                Criteria.DoubleBondStereo = _matchDoubleBondStereoAny.Checked ? SearchCriteria.COEBoolean.No : SearchCriteria.COEBoolean.Yes;
                Criteria.RelativeTetStereo = COEConvert.ToCOEBoolean(_thickBondsRelCheckBox.Checked.ToString());
            }
            else
            {
                Criteria.TetrahedralStereo = SearchCriteria.TetrahedralStereoMatching.Any;
                Criteria.DoubleBondStereo = SearchCriteria.COEBoolean.No;
                Criteria.RelativeTetStereo = SearchCriteria.COEBoolean.No;
            }
            return Criteria;
        }

        private SearchCriteria.TetrahedralStereoMatching FillFromCheckBoxes()
        {
            if(_matchTetrahedralStereoSame.Checked)
                return SearchCriteria.TetrahedralStereoMatching.Same;
            if(_matchTetrahedralStereoEither.Checked)
                return SearchCriteria.TetrahedralStereoMatching.Either;
            if(_matchTetrahedralStereoAny.Checked)
                return SearchCriteria.TetrahedralStereoMatching.Any;

            return SearchCriteria.TetrahedralStereoMatching.Same;
        }

        private void InitializeValues()
        {
            _hitAnyChargeCheckBox.Checked = ToBoolean(Criteria.HitAnyChargeHetero);
            _reactionQueryCheckBox.Checked = ToBoolean(Criteria.ReactionCenter);
            _hitAnyChargeOnCarbonCheckBox.Checked = ToBoolean(Criteria.HitAnyChargeCarbon);
            _permitExtraneousFragmentsInFullCheckBox.Checked = ToBoolean(Criteria.PermitExtraneousFragments);
            _permitExtraneousFragmentsInReactionFullCheckBox.Checked = ToBoolean(Criteria.PermitExtraneousFragmentsIfRXN);
            _fragmentsOverlapCheckBox.Checked = ToBoolean(Criteria.FragmentsOverlap);
            _tautomericCheckBox.Checked = ToBoolean(Criteria.Tautometer);
            _similarityTextBox.Text = Criteria.SimThreshold.ToString();
            _fullStructureSimilarityCheckBox.Checked = ToBoolean(Criteria.FullSearch);

            _matchTetrahedralStereoSame.Checked = (Criteria.TetrahedralStereo == SearchCriteria.TetrahedralStereoMatching.Same);
            _matchTetrahedralStereoEither.Checked = (Criteria.TetrahedralStereo == SearchCriteria.TetrahedralStereoMatching.Either);
            _matchTetrahedralStereoAny.Checked = (Criteria.TetrahedralStereo == SearchCriteria.TetrahedralStereoMatching.Any);
            if(Criteria.TetrahedralStereo == SearchCriteria.TetrahedralStereoMatching.Yes)
                _matchTetrahedralStereoSame.Checked = true;
            else if(Criteria.TetrahedralStereo == SearchCriteria.TetrahedralStereoMatching.No)
                _matchTetrahedralStereoAny.Checked = true;

            _matchDoubleBondStereoAny.Checked = !ToBoolean(Criteria.DoubleBondStereo);
            _matchDoubleBondStereoSame.Checked = ToBoolean(Criteria.DoubleBondStereo);
            _thickBondsRelCheckBox.Checked = ToBoolean(Criteria.RelativeTetStereo);

            _matchStereochemistry.Checked = true;
        }

        private bool ToBoolean(SearchCriteria.COEBoolean coeBoolean)
        {
            if(coeBoolean == SearchCriteria.COEBoolean.No)
                return false;
            else
                return true;
        }
        #endregion

    }
}
