using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using CambridgeSoft.COE.Framework.Controls.COEFormGenerator;

namespace CambridgeSoft.COE.Framework.Controls.MarkHitControls
{
    #region MarkHitButton Class
    public class MarkHitButton : WebControl, INamingContainer, ICallbackEventHandler
    {
        #region Variables
        private RenderMode _renderMode = RenderMode.ImageButton;
        private Panel _holder;
        private Panel _center;
        private Panel _right;
        private Panel _left;
        private CheckBox _checkBox;
        private Label _label;
        private HiddenField _markedField;
        protected event EventHandler Marking;
        #endregion

        #region Properties
        public RenderMode RenderingMode
        {
            get { return _renderMode; }
            set { _renderMode = value; }
        }

        public bool Marked
        {
            get
            {
                if (ViewState["Marked"] == null)
                {
                    ViewState["Marked"] = false;
                }

                return (bool)ViewState["Marked"];
            }
            set
            {
                ViewState["Marked"] = value;
            }
        }

        public string ColumnIDValue
        {
            get
            {
                if (ViewState["ColumnIDValue"] == null)
                {
                    ViewState["ColumnIDValue"] = string.Empty;
                }
                return (string)ViewState["ColumnIDValue"];
            }
            set
            {
                ViewState["ColumnIDValue"] = value;
            }
        }

        public string Text
        {
            get
            {
                if(ViewState["Text"] == null)
                {
                    ViewState["Text"] = string.Empty;
                }
                return (string) ViewState["Text"];
            }
            set
            {
                ViewState["Text"] = value;
            }
        }        
        #endregion

        #region Overriden Methods (LifeCycle events)
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Controls.Clear();

            this.ID = "MarkHitControl";
            _holder = new Panel();
            this.Controls.Add(_holder);

            _center = new Panel();
            _center.Style.Add(HtmlTextWriterStyle.Display, "inline");
            _markedField = new HiddenField();
            _markedField.ID = "MarkedHidden";

            switch(this.RenderingMode)
            {
                case RenderMode.CheckBox:
                    _checkBox = new CheckBox();                     
                    _center.Controls.Add(_checkBox);
                    _holder.Controls.Add(_center);
                    break;
                case RenderMode.ImageButton:

                    _left = new Panel();
                    _right = new Panel();
                    _label = new Label();

                    _center.Controls.Add(_label);

                    _holder.Controls.Add(_left);
                    _holder.Controls.Add(_center);
                    _holder.Controls.Add(_right);
                    break;
            }
            _center.Controls.Add(_markedField);
            _holder.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            string callbackScript = @"
            var disabledBackground =  ""url(" + Page.ResolveClientUrl(Page.ClientScript.GetWebResourceUrl(typeof(MarkHitButton), "CambridgeSoft.COE.Framework.ServerControls.MarkHitControls.centerMarked.gif")) + @")"";
            var enabledBackground =  ""url(" + Page.ResolveClientUrl(Page.ClientScript.GetWebResourceUrl(typeof(MarkHitButton), "CambridgeSoft.COE.Framework.ServerControls.MarkHitControls.centerUnMarked.gif")) + @")"";

            function ApplyMarkCallBack(result, context)
            {                
                var buttontype = context.split(',')[0];
                var controlId = context.split(',')[1];
                var newContext = 'ApplyMark,' + controlId;
                if(buttontype == 'ImageButton')
                {                    
                    if(result == 'true')
                    {
                        document.getElementById(controlId).style.backgroundImage = disabledBackground;
                        document.getElementById(controlId).title = document.getElementById(context.split(',')[1]).firstChild.innerText = 'Unmark hit';
                        document.getElementById(controlId).firstChild.nextSibling.value = 'false';
                    }
                    else
                    {
                        document.getElementById(controlId).style.backgroundImage = enabledBackground;
                        document.getElementById(controlId).title = document.getElementById(context.split(',')[1]).firstChild.innerText = 'Mark hit';
                        document.getElementById(controlId).firstChild.nextSibling.value = 'true';
                    }
                }
                else
                {                    
                    if(result == 'true')
                    {                        
                        document.getElementById(controlId).checked = true;
                        document.getElementById(controlId).parentNode.title = 'Unmark hit';                        
                        document.getElementById('" + this._markedField.ClientID + @"').value = 'false';                        
                    }
                    else
                    {                        
                        document.getElementById(controlId).checked = false;
                        document.getElementById(controlId).parentNode.title = 'Mark hit';                        
                        document.getElementById('" + this._markedField.ClientID + @"').value = 'true';                       
                    }
                }

                
                if(typeof(OnCallbackComplete) == 'function')
                    OnCallbackComplete(result, newContext);
            }
            function ApplyMarkCallBackError(result, context)
            {
                alert(result); 
            }
            ";
            if(!this.Page.ClientScript.IsClientScriptBlockRegistered(typeof(MarkHitButton), "callbackScript"))
                this.Page.ClientScript.RegisterClientScriptBlock(typeof(MarkHitButton), "callbackScript", callbackScript, true);

            if(ScriptManager.GetCurrent(this.Page) != null)
                ScriptManager.RegisterClientScriptBlock(this, typeof(MarkHitButton), "callbackScript", callbackScript, true);

            string context = this.RenderingMode.ToString();

            string onclickJS = string.Format("javascript:{0};{1};{2}; return true;",
                "__theFormPostData = ''",
                "WebForm_InitCallback()",
                Page.ClientScript.GetCallbackEventReference(this,
                        "document.getElementById('" + _markedField.ClientID + "').value == 'true'",
                        "ApplyMarkCallBack",
                        "'" + context + "," + (RenderingMode == RenderMode.ImageButton ? _center.ClientID : _checkBox.ClientID) + "'",
                        "ApplyMarkCallBackError",
                        true));


            this.Attributes.Add("OnClick", onclickJS);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            Page.ClientScript.RegisterForEventValidation(this.UniqueID);
            _markedField.Value = (!this.Marked).ToString().ToLower();
            switch(this.RenderingMode)
            {
                case RenderMode.CheckBox:
                    _checkBox.Checked = this.Marked;
                    _checkBox.Style.Add(HtmlTextWriterStyle.PaddingTop, "4px");
                    _checkBox.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
                    _checkBox.ToolTip = this.Text;
                    _checkBox.CssClass = this.CssClass;
                    _checkBox.AutoPostBack = false;
                    break;
                case RenderMode.ImageButton:
                    _label.Style.Add(HtmlTextWriterStyle.PaddingTop, "4px");
                    _label.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
                    _label.CssClass = this.CssClass;
                    
                    _left.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url(" + Page.ClientScript.GetWebResourceUrl(typeof(MarkHitButton), "CambridgeSoft.COE.Framework.ServerControls.MarkHitControls.left.gif") + ")");
                    _left.Style.Add(HtmlTextWriterStyle.Display, "inline");
                    _right.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url(" + Page .ClientScript.GetWebResourceUrl(typeof(MarkHitButton), "CambridgeSoft.COE.Framework.ServerControls.MarkHitControls.right.gif") + ")");
                    _right.Style.Add(HtmlTextWriterStyle.Display, "inline");
                    _right.Width = _left.Width = new Unit("3px");

                    _label.Height = new Unit("24px");
                    _label.Width = new Unit("94px");
                    _holder.Width = new Unit("100px");
                    break;
            }
            UpdateTemplateValues();
            base.Render(writer);
        }
        #endregion

        #region EventHandlers
        void ApplyMarkAction(object sender, EventArgs e)
        {
            UpdateTemplateValues();
            if(Marking != null)
                Marking(sender, e);
        }
        #endregion

        #region Private Methods
        private void UpdateTemplateValues()
        {
            if (Marked)
                _center.ToolTip = Text = "Unmark hit";
            else
                _center.ToolTip = Text = "Mark hit";
            switch(this.RenderingMode)
            {
                case RenderMode.CheckBox:
                    _checkBox.Checked = this.Marked;                    
                    break;
                case RenderMode.ImageButton:
                    if(Marked)
                    {
                        _center.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url(" + Page.ClientScript.GetWebResourceUrl(typeof(MarkHitButton), "CambridgeSoft.COE.Framework.ServerControls.MarkHitControls.centerMarked.gif") + ")");
                    }
                    else
                    {
                        _center.Style.Add(HtmlTextWriterStyle.BackgroundImage, "url(" + Page.ClientScript.GetWebResourceUrl(typeof(MarkHitButton), "CambridgeSoft.COE.Framework.ServerControls.MarkHitControls.centerUnMarked.gif") + ")");
                    }
                    _label.Text = this.Text;
                    break;
            }
        }
        #endregion
        
        #region ICallbackEventHandler Members
        // CBOE-1289 - Fix
        string ICallbackEventHandler.GetCallbackResult()
        {
            string result = string.Empty;
            switch (this.RenderingMode)
            {
                case RenderMode.CheckBox:
                    result = _checkBox.Checked.ToString().ToLower();
                    break;
                case RenderMode.ImageButton:
                    result = (this.Marked) ? "true" : "false";
                    break;
            }
            return result;
        }
        // CBOE-1289 - Fix
        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            switch (this.RenderingMode)
            {
                case RenderMode.CheckBox:
                    this.Marked = (_checkBox.Checked);
                    break;
                case RenderMode.ImageButton:
                    this.Marked = Convert.ToBoolean(eventArgument.ToString());
                    break;
            }
            ApplyMarkAction(this, EventArgs.Empty);
        }
        #endregion

        #region Enum
        public enum RenderMode
        {
            ImageButton,
            CheckBox
        }
        #endregion
    }
    #endregion
}
