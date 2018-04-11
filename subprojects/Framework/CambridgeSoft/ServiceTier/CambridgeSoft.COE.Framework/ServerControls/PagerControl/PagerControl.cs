using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using CambridgeSoft.COE.Framework.Properties;


namespace CambridgeSoft.COE.Framework.Controls
{

    [DefaultProperty("CurrentPage")]
    [ToolboxData("<{0}:PagerControl runat=server></{0}:PagerControl>")]
    [Description("Displays a pager control.")]
    public class PagerControl : CompositeControl
    {
        #region Variables

        private ImageButton _firstLink;// = new ImageButton();
        private ImageButton _lastLink;// = new ImageButton();
        private ImageButton _nextLink;// = new ImageButton();
        private ImageButton _previousLink;// = new ImageButton();
        private LinkButton _recNumber;
        private Label _recordCount = new Label();
        //private DropDownList _currentDropDown = new DropDownList();
        private const string _objectCleanerScripts = "cleanCDPObjects";

        private int _pageSize;
        private int _pagesCount;
        private int _currentPage;
        private bool _stillGettingRecords = true;

        #endregion

        #region Events Delegates

        public delegate void CurrentPageChangedHandler(object sender, PageChangedEventArgs eventArgs);
        public event CurrentPageChangedHandler CurrentPageChanged;

        #endregion

        #region Properties
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        public int PageSize
        {
            get
            {
                if (Page.Session["PageSize"] != null)
                    return (int)Page.Session["PageSize"];
                else
                    return 1;
            }
            set
            {
                Page.Session["PageSize"] = value;
            }
        }

        public int RecordCount
        {
            get
            {
                if (Page.Session["RecordCount"] != null)
                    return (int)Page.Session["RecordCount"];
                else
                    return 1;
            }
            set
            {
                Page.Session["RecordCount"] = value;
                CreateChildControls();
            }
        }

        private int PagesCount
        {
            get
            {
                if (Page.Session["PagesCount"] != null)
                    return (int)Page.Session["PagesCount"];
                else
                    return 0;
            }
            set
            {
                Page.Session["PagesCount"] = value;
            }
        }

        public int CurrentPage
        {
            get
            {
                if (Page.Session["CurrentPage"] == null)
                    Page.Session["CurrentPage"] = 1;

                return (int)Page.Session["CurrentPage"];
            }
            set
            {
                if (Page.Session["CurrentPage"] == null || (int)Page.Session["CurrentPage"] != value)
                {
                    int previous = CurrentPage;
                    Page.Session["CurrentPage"] = value;
                    //_currentDropDown.SelectedValue = value.ToString();
                    if (CurrentPageChanged != null)
                        CurrentPageChanged(this, new PageChangedEventArgs(previous, value));
                }
            }
        }

        public string FirstImageURL
        {
            get
            {
                if (Page.Session["FirstImageURL"] != null)
                    return (string)Page.Session["FirstImageURL"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(PagerControl), "CambridgeSoft.COE.Framework.ServerControls.PagerControl.First_Track_1.png");
            }
            set
            {
                Page.Session["FirstImageURL"] = value;
            }
        }

        public string FirstImageURL_Deselected
        {
            get
            {
                if (Page.Session["FirstImageURL_Deselected"] != null)
                    return (string)Page.Session["FirstImageURL_Deselected"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(PagerControl), "CambridgeSoft.COE.Framework.ServerControls.PagerControl.First_Track_1_d.png");
            }
            set
            {
                Page.Session["FirstImageURL_Deselected"] = value;
            }
        }

        public string PreviousImageURL
        {
            get
            {
                if (Page.Session["PreviousImageURL"] != null)
                    return (string)Page.Session["PreviousImageURL"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(PagerControl), "CambridgeSoft.COE.Framework.ServerControls.PagerControl.Previous_B.png");
            }
            set
            {
                Page.Session["PreviousImageURL"] = value;
            }
        }

        public string PreviousImageURL_Deselected
        {
            get
            {
                if (Page.Session["PreviousImageURL_Deselected"] != null)
                    return (string)Page.Session["PreviousImageURL_Deselected"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(PagerControl), "CambridgeSoft.COE.Framework.ServerControls.PagerControl.Previous_B_d.png");
            }
            set
            {
                Page.Session["PreviousImageURL_Deselected"] = value;
            }
        }

        public string NextImageURL
        {
            get
            {
                if (Page.Session["NextImageURL"] != null)
                    return (string)Page.Session["NextImageURL"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(PagerControl), "CambridgeSoft.COE.Framework.ServerControls.PagerControl.Next_B.png");
            }
            set
            {
                Page.Session["NextImageURL"] = value;
            }
        }

        public string NextImageURL_Deselected
        {
            get
            {
                if (Page.Session["NextImageURL_Deselected"] != null)
                    return (string)Page.Session["NextImageURL_Deselected"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(PagerControl), "CambridgeSoft.COE.Framework.ServerControls.PagerControl.Next_B_d.png");
            }
            set
            {
                Page.Session["NextImageURL_Deselected"] = value;
            }
        }

        public string LastImageURL
        {
            get
            {
                if (Page.Session["LastImageURL"] != null)
                    return (string)Page.Session["LastImageURL"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(PagerControl), "CambridgeSoft.COE.Framework.ServerControls.PagerControl.Last_Track_1.png");
            }
            set
            {
                Page.Session["LastImageURL"] = value;
            }
        }

        public string LastImageURL_Deselected
        {
            get
            {
                if (Page.Session["LastImageURL_Deselected"] != null)
                    return (string)Page.Session["LastImageURL_Deselected"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(PagerControl), "CambridgeSoft.COE.Framework.ServerControls.PagerControl.Last_Track_1_d.png");
            }
            set
            {
                Page.Session["LastImageURL_Deselected"] = value;
            }
        }

        public string InProgressImageURL
        {
            get
            {
                if (Page.Session["InProgressImageURL"] != null)
                    return (string)Page.Session["InProgressImageURL"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(PagerControl), "CambridgeSoft.COE.Framework.ServerControls.PagerControl.InProgress.gif");
            }
            set
            {
                Page.Session["InProgressImageURL"] = value;
            }
        }
        public string RecImageURL
        {
            get
            {
                if (Page.Session["RecImageURL"] != null)
                    return (string)Page.Session["RecImageURL"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(PagerControl), "CambridgeSoft.COE.Framework.ServerControls.PagerControl.Rec_B.png");
            }
            set
            {
                Page.Session["RecImageURL"] = value;
            }
        }

        public string RecImageURL_Deselected
        {
            get
            {
                if (Page.Session["RecImageURL_Deselected"] != null)
                    return (string)Page.Session["RecImageURL_Deselected"];
                else
                    return Page.ClientScript.GetWebResourceUrl(typeof(PagerControl), "CambridgeSoft.COE.Framework.ServerControls.PagerControl.Rec_B_d.png");
            }
            set
            {
                Page.Session["RecImageURL_Deselected"] = value;
            }
        }

        public ImageButton FirstLink
        {
            get
            {
                if (_firstLink == null)
                {
                    _firstLink = new ImageButton();
                    _firstLink.ID = "FirstImageButton";
                    _firstLink.ImageUrl = FirstImageURL;
                    _firstLink.Click += new ImageClickEventHandler(firstLink_Click);
                }

                return _firstLink;
            }
        }

        public ImageButton PreviousLink
        {
            get
            {
                if (_previousLink == null)
                {
                    _previousLink = new ImageButton();
                    _previousLink.ID = "PreviousImageButton";
                    _previousLink.ImageUrl = PreviousImageURL;
                    _previousLink.Click += new ImageClickEventHandler(previousLink_Click);
                }

                return _previousLink;
            }
        }

        public ImageButton NextLink
        {
            get
            {
                if (_nextLink == null)
                {
                    _nextLink = new ImageButton();
                    _nextLink.ID = "NextImageButton";
                    _nextLink.ImageUrl = NextImageURL;
                    _nextLink.Click += new ImageClickEventHandler(nextLink_Click);
                }

                return _nextLink;
            }
        }

        public ImageButton LastLink
        {
            get
            {
                if (_lastLink == null)
                {
                    _lastLink = new ImageButton();
                    _lastLink.ID = "LastImageButton";
                    _lastLink.Click += new ImageClickEventHandler(lastLink_Click);
                }

                string imageURL = _stillGettingRecords ? InProgressImageURL : LastImageURL;
                _lastLink.ImageUrl = imageURL;

                return _lastLink;
            }
        }

        public LinkButton RecNumber
        {
            get
            {
                if (_recNumber == null)
                {
                    _recNumber = new LinkButton();
                    _recNumber.ID = "RecNumLinkButton";
                    _recNumber.Style.Add(HtmlTextWriterStyle.BorderStyle, "outset");
                    _recNumber.Style.Add(HtmlTextWriterStyle.BorderWidth, "0px");
                    _recNumber.Style.Add(HtmlTextWriterStyle.PaddingTop, "4px");
                    _recNumber.Style.Add(HtmlTextWriterStyle.PaddingRight, "7px");
                    _recNumber.Style.Add(HtmlTextWriterStyle.PaddingBottom, "0px");
                    _recNumber.Style.Add(HtmlTextWriterStyle.PaddingLeft, "5px");
                    _recNumber.Style.Add(HtmlTextWriterStyle.BackgroundImage, RecImageURL);
                    _recNumber.Style.Add(HtmlTextWriterStyle.BorderColor, "gray");
                    _recNumber.Style.Add(HtmlTextWriterStyle.Color, "#FCFCFC");                    

                }

                //_recNumber.Text = Resources.RecNumber_Label_Text;
                _recNumber.Width = Unit.Parse("35px");
                _recNumber.Height = Unit.Parse("15px");
                return _recNumber;
            }
        }

        /// <summary>
        /// Enables the RecNumber in the middle of the pager. Default value is true. If disabled the text info is put in the middle of the
        /// pager, otherwise is displayed on the right.
        /// </summary>
        public bool DisplayRecNumber
        {
            get
            {
                if (Page.Session["DisplayRecNumber"] == null)
                    Page.Session["DisplayRecNumber"] = true;

                return (bool)Page.Session["DisplayRecNumber"];
            }
            set
            {
                Page.Session["DisplayRecNumber"] = value;
            }
        }

        /// <summary>
        /// The string that represents the way you want to show current pager information when there are multiple pages. Specific tokens can bu used for
        /// current page: {currentpage}, quantity of pages: {pagescount} and Total quantity of records: {totalrecordcount}.
        /// For example, the default format is: Records {currentpage} - {pagescount} of {totalrecordcount} records matching your query
        /// </summary>
        public string MultiplePagesInformationFormat
        {
            get
            {
                if (Page.Session["MultiplePagesInformationFormat"] == null)
                {
                    Page.Session["MultiplePagesInformationFormat"] = "Records {currentpage} - {pagescount} of {totalrecordcount} records matching your query";
                }


                return ((string)Page.Session["MultiplePagesInformationFormat"]).Replace("{currentpage}", "{0}").Replace("{pagescount}", "{1}").Replace("{totalrecordcount}", "{2}");
            }
            set
            {
                Page.Session["MultiplePagesInformationFormat"] = value;
            }
        }

        /// <summary>
        /// The string that represents the way you want to show current pager information when there is only one page. Specific tokens can bu used for
        /// current page: {currentpage}, quantity of pages: {pagescount} and Total quantity of records: {totalrecordcount}.
        /// For example, the default format is: Record {currentpage} of {totalrecordcount} records matching your query
        /// </summary>
        public string SinglePageInformationFormat
        {
            get
            {
                if (Page.Session["SinglePageInformationFormat"] == null)
                {
                    Page.Session["SinglePageInformationFormat"] = "Record {currentpage} of {totalrecordcount} records matching your query";
                }


                return ((string)Page.Session["SinglePageInformationFormat"]).Replace("{currentpage}", "{0}").Replace("{pagescount}", "{1}").Replace("{totalrecordcount}", "{2}");
            }
            set
            {
                Page.Session["SinglePageInformationFormat"] = value;
            }
        }

        public bool IsStillGettingResults
        {
            set { _stillGettingRecords = value; CreateChildControls(); }
        }
        #endregion

        #region Events
        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            this.Controls.Add(FirstLink);
            this.Controls.Add(PreviousLink);
            if (DisplayRecNumber)
            {
                this.Controls.Add(RecNumber);
            }
            else
                this.Controls.Add(_recordCount);
            this.Controls.Add(NextLink);
            this.Controls.Add(LastLink);
            if (DisplayRecNumber)
                this.Controls.Add(_recordCount);
            AddStyles();

            PagesCount = (int)Math.Ceiling((double)RecordCount / PageSize);
            if (CurrentPage > PagesCount)
            {
                CurrentPage = PagesCount;
            }
            this.ChildControlsCreated = true;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            int finalRecordsDisplayedInPage = CurrentPage * PageSize > RecordCount ? RecordCount : CurrentPage * PageSize;
            int startRecordsDisplayedInPage = (CurrentPage * PageSize) - PageSize + 1;

            string format = string.Empty;
            if (startRecordsDisplayedInPage >= finalRecordsDisplayedInPage)
                format = SinglePageInformationFormat;
            else
                format = MultiplePagesInformationFormat;

            _recordCount.Text = string.Format(format, startRecordsDisplayedInPage, finalRecordsDisplayedInPage >= 0 ? finalRecordsDisplayedInPage : 1, RecordCount >= 0 ? RecordCount : 1);

            if (CurrentPage >= PagesCount)
            {
                _nextLink.Enabled = _lastLink.Enabled = false;
                _nextLink.ImageUrl = this.NextImageURL_Deselected;
                _lastLink.ImageUrl = this.LastImageURL_Deselected;
            }
            else
            {
                _nextLink.Enabled = true;
                _nextLink.ImageUrl = this.NextImageURL;
                if (!_stillGettingRecords)
                {
                    _lastLink.ImageUrl = this.LastImageURL;
                    _lastLink.Enabled = true;
                }
                else
                {
                    _lastLink.ImageUrl = this.InProgressImageURL;
                    _lastLink.Enabled = false;
                }
            }

            if (CurrentPage <= 1)
            {
                _firstLink.Enabled = _previousLink.Enabled = false;
                _firstLink.ImageUrl = this.FirstImageURL_Deselected;
                _previousLink.ImageUrl = this.PreviousImageURL_Deselected;
            }
            else
            {
                _firstLink.Enabled = _previousLink.Enabled = true;
                _firstLink.ImageUrl = this.FirstImageURL;
                _previousLink.ImageUrl = this.PreviousImageURL;
            }

            //This is for disabling the Rec# button if the output has only one page result.
            if (PagesCount <= 1)
            {
                _recNumber.Enabled = false;
                _recNumber.Style.Add(HtmlTextWriterStyle.BackgroundImage, RecImageURL_Deselected);
            }
            else
            {
                _recNumber.Enabled = true;
                _recNumber.Style.Add(HtmlTextWriterStyle.BackgroundImage, RecImageURL);
            }
            
            base.Render(writer);
        }

        void lastLink_Click(object sender, ImageClickEventArgs e)
        {
            CurrentPage = PagesCount;
        }

        void nextLink_Click(object sender, ImageClickEventArgs e)
        {
            if (PagesCount > CurrentPage)
                CurrentPage++;
        }

        void previousLink_Click(object sender, ImageClickEventArgs e)
        {
            if (CurrentPage > 1)
                CurrentPage--;
        }

        void firstLink_Click(object sender, ImageClickEventArgs e)
        {
            CurrentPage = 1;
        }
        #endregion

        #region Methods
        private void AddStyles()
        {
            this.Style.Add("display", "inline");
            this.Style.Add("vertical-align", "bottom");
            _firstLink.Style.Add("display", "inline");
            _firstLink.Style.Add(HtmlTextWriterStyle.MarginLeft, "5px");
            _firstLink.Style.Add(HtmlTextWriterStyle.MarginRight, "5px");
            _previousLink.Style.Add("display", "inline");
            _previousLink.Style.Add(HtmlTextWriterStyle.MarginLeft, "5px");
            _previousLink.Style.Add(HtmlTextWriterStyle.MarginRight, "5px");
            /*_currentDropDown.Style.Add("display", "inline");
            _currentDropDown.Style.Add("margin", "5px 5px 5px 5px");*/
            _nextLink.Style.Add("display", "inline");
            _nextLink.Style.Add(HtmlTextWriterStyle.MarginLeft, "5px");
            _nextLink.Style.Add(HtmlTextWriterStyle.MarginRight, "5px");
            _lastLink.Style.Add("display", "inline");
            _lastLink.Style.Add(HtmlTextWriterStyle.MarginLeft, "5px");
            _lastLink.Style.Add(HtmlTextWriterStyle.MarginRight, "5px");

            _recordCount.Style.Add("display", "inline");
            _recordCount.Style.Add(HtmlTextWriterStyle.MarginLeft, "5px");
            _recordCount.Style.Add(HtmlTextWriterStyle.MarginRight, "5px");
        }
        #endregion
    }

    public class PageChangedEventArgs : EventArgs
    {
        private int _previousPage;
        private int _currentPage;

        public int PreviousPage
        {
            get { return _previousPage; }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
        }

        public PageChangedEventArgs(int previousPage, int currentPage)
        {
            _previousPage = previousPage;
            _currentPage = currentPage;
        }
    }
}
