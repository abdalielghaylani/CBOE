using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace CambridgeSoft.COE.Framework.Controls
{
    /// <summary>
    /// WorkFlow server control that helps to jump from one page/section of the application to another.
    /// </summary>
    /// <example>Step1 -> Step 2 -> Step 3 </example>
    [DefaultProperty("Mask")]
    [ToolboxData("<{0}:WorkFlow runat=server></{0}:WorkFlow>")]
    [Description("Displays a list of links to whether go back to previous steps/pages or go forward")]
    public class WorkFlow : WebControl
    {
        #region Variables

        private CambridgeSoft.COE.Framework.Common.GUIShell.DataServices.COEWorkFlow _ds;
        private string _mask = string.Empty;
        private Panel _container = null;
        private string _containerCSS = string.Empty;
        private string _linkCSS = string.Empty;
        private string _linkCSSSeletected = string.Empty;
        private string _splitterCSS = string.Empty;
        private string _splitter = string.Empty;
        private string _linkCSSSeletectedContainer = string.Empty;
        private int _workFlowID = -1;

        #endregion

        #region Properties

        /// <summary>
        /// Sets the data source to be used to create each one of the links.
        /// </summary>
        /// <value>The data source.</value>
        [Bindable(true)]
        public CambridgeSoft.COE.Framework.Common.GUIShell.DataServices.COEWorkFlow DataSource
        {
            set
            {
                if(value != null)
                    _ds = value;
            }
        }

        /// <summary>
        /// Sets the mask value that is needed to find till what level to display the datasource.
        /// </summary>
        /// <value>The mask.</value>
        /// <example>If the Mask is C, it will display all the rows that have a C as part of the Mask. Check that a Mask value could be ABC</example>
        [Bindable(true)]
        public string Mask
        {
            set
            {
                if(!string.IsNullOrEmpty(value))
                    _mask = value;
            }
        }

        /// <summary>
        /// Sets the container CSS (the panel that contains all the links and splitters).
        /// </summary>
        /// <value>The container CSS.</value>
        [Bindable(true)]
        public string ContainerCSS
        {
            set
            {
                if(!string.IsNullOrEmpty(value))
                    _containerCSS = value;
            }
        }

        /// <summary>
        /// Sets the link CSS.
        /// </summary>
        /// <value>The link CSS class to be applied to the links</value>
        [Bindable(true)]
        public string LinkCSS
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _linkCSS = value;
            }
        }

        /// <summary>
        /// Sets the selected link CSS.
        /// </summary>
        /// <value>The link CSS class to be applied to the links</value>
        [Bindable(true)]
        public string LinkCSSSelected
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _linkCSSSeletected = value;
            }
        }

        /// <summary>
        /// Sets the link CSS to the div containing the selected (current) step.
        /// </summary>
        /// <value>The link CSS selected.</value>
        [Bindable(true)]
        public string LinkCSSSelectedContainer
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _linkCSSSeletectedContainer = value;
            }
        }

        /// <summary>
        /// Sets the splitter CSS class to be applied to the splitter strings.
        /// </summary>
        /// <value>The splitter CSS.</value>
        [Bindable(true)]
        public string SplitterCSS
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _splitterCSS = value;
            }
        }

        /// <summary>
        /// Sets the splitter string that divides one link to another.
        /// </summary>
        /// <value>The splitter string.</value>
        /// <example>-></example>
        [Bindable(true)]
        public string Splitter
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                    _splitter = value;
            }
        }


        /// <summary>
        /// Sets the work flow ID.
        /// This is used when you have different workflows as modes, pages related, etc
        /// </summary>
        /// <value>The work flow ID.</value>
        [Bindable(true)]
        public int WorkFlowID
        {
            set
            {
                _workFlowID = value;
            }
        }


        #endregion

        #region Events

        //protected override void RecreateChildControls()
        //{
        //    EnsureChildControls();
        //    base.RecreateChildControls();
        //}

        protected override void CreateChildControls()
        {
            this.EnableViewState = true;
            this.Controls.Clear();
            if (_ds != null && !string.IsNullOrEmpty(_mask) && _workFlowID > 0)
            {
                int counter = 0;
                //Filter by the given criterias set in previous from the caller page.
                string select = "WorkFlowID =" + _workFlowID;
                string currentSectionSelect = "Mask like '%" + _mask.ToUpper() + @"%' AND WorkFlowID =" + _workFlowID; ;
                DataRow[] results = _ds.Tables[0].Select(select);
                DataRow[] seletedRow = _ds.Tables[0].Select(currentSectionSelect);
                //Find 
                if (_container == null) 
                {
                    _container = new Panel();
                    _container.CssClass = _containerCSS;
                }
                foreach (DataRow row in results)
                {
                    if (row is CambridgeSoft.COE.Framework.Common.GUIShell.DataServices.COEWorkFlow.WorkFlowRow)
                    {
                        HyperLink link = new HyperLink();
                        link.Text = ((CambridgeSoft.COE.Framework.Common.GUIShell.DataServices.COEWorkFlow.WorkFlowRow)row).Text;
                        link.ToolTip = ((CambridgeSoft.COE.Framework.Common.GUIShell.DataServices.COEWorkFlow.WorkFlowRow)row).Tooltip;
                        link.NavigateUrl = ((CambridgeSoft.COE.Framework.Common.GUIShell.DataServices.COEWorkFlow.WorkFlowRow)row).URL;
                        if (seletedRow.Length > 0)
                        {
                            if (row.Equals(seletedRow[0]))
                            {
                                link.CssClass = _linkCSSSeletected;
                                //Add a div containing the link for better styling
                                Panel linkCont = new Panel();
                                linkCont.CssClass = _linkCSSSeletectedContainer;
                                linkCont.Controls.Add(link);
                                this._container.Controls.Add(linkCont);
                            }
                            else
                            {
                                link.CssClass = _linkCSS;
                                this._container.Controls.Add(link);
                            }
                        }
                        else
                        {
                            link.CssClass = _linkCSS;
                            this._container.Controls.Add(link);
                        }
                        if (results.Length > ++counter)//Add Splitter (if it's not the last item)
                        {
                            Label lbl = new Label();
                            lbl.Text = _splitter;
                            lbl.CssClass = _splitterCSS;
                            this._container.Controls.Add(lbl);
                        }
                    }
                }
                this.Controls.Add(_container);
            }
            base.CreateChildControls();
        }

        #endregion
    }
}
