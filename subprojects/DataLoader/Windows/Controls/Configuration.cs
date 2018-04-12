using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CambridgeSoft.COE.DataLoader.Common;

namespace CambridgeSoft.COE.DataLoader.Windows.Controls
{
    /// <summary>
    /// Configuration control for optional use by UI elements
    /// </summary>
    public partial class Configuration : UserControl
    {
        #region data
        private System.Windows.Forms.GroupBox _grpConfiguration;
        private string _name;
        #endregion

        #region properties
        /// <summary>
        /// Get property that indicates whether configuration is available
        /// </summary>
        public bool HasSettings
        {
            get
            {
                return (_grpConfiguration.Controls.Count > 0);
            }
        } // HasSettings

        /// <summary>
        /// Get the configuration requirements and settings
        /// Set the configuration settings
        /// </summary>
        public string Settings
        {
            get
            {
                string xmlConfiguration = string.Empty;
                if (_grpConfiguration.Controls.Count > 0)
                {
                    COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                    ControlGetConfiguration(oCOEXmlTextWriter, _grpConfiguration, _name);
                    xmlConfiguration = COEXmlTextWriter.Pretty(oCOEXmlTextWriter.XmlString);
                    oCOEXmlTextWriter.Close();
                }
                return xmlConfiguration;
            } // get
            set
            {
                Controls.Remove(_grpConfiguration);
                _grpConfiguration.Controls.Clear();
                if ((value != null) && (value.Length > 0))
                {
                    XmlDocument oXmlDocument = new XmlDocument();
                    oXmlDocument.LoadXml(value);
                    XmlNode oXmlNodeConfiguration = oXmlDocument.DocumentElement;
                    _name = oXmlNodeConfiguration.Name;

                    Controls.Remove(_grpConfiguration);
                    _grpConfiguration = ConfigurationGetControl(oXmlNodeConfiguration);
                    Controls.Add(_grpConfiguration);
                    Controls.SetChildIndex(_grpConfiguration, 0);
                }
                return;
            } // set
        } // Settings

        #endregion

        #region methods
        private System.Windows.Forms.GroupBox ConfigurationGetControl(XmlNode voXmlNodeConfiguration)
        {
            string strText;
            {
                XmlAttribute oXmlAttribute = voXmlNodeConfiguration.Attributes["text"];
                strText = (oXmlAttribute != null) ? oXmlAttribute.Value : "aha!";
            }
            string strId;
            {
                XmlAttribute oXmlAttribute = voXmlNodeConfiguration.Attributes["member"];
                strId = (oXmlAttribute != null) ? oXmlAttribute.Value : string.Empty;
            }
            int nButton = 0;
            int nButtonChecked = 0;
            {
                XmlAttribute oXmlAttribute = voXmlNodeConfiguration.Attributes["value"];
                nButtonChecked = (oXmlAttribute != null) ? Convert.ToInt32(oXmlAttribute.Value) : 0;
            }
            // Right now assuming a GroupBox
            List<GroupBox> listGroupBoxNested = new List<GroupBox>();
            System.Windows.Forms.GroupBox groupBox = UIBase.GetGroupBox();
            groupBox.Tag = strId;
            groupBox.Text = strText;
            int xMax = 0;
            {
                xMax = groupBox.Padding.Left + 4 + TextRenderer.MeasureText(groupBox.Text, groupBox.Font).Width + 4 + groupBox.Padding.Right;
            }
            int y = 0;
            y += groupBox.Padding.Top;
            foreach (XmlNode oXmlNode in voXmlNodeConfiguration.ChildNodes)
            {
                switch (oXmlNode.Name)
                {
                    case "GroupBox":
                        {
                            System.Windows.Forms.GroupBox groupBoxNested = ConfigurationGetControl(oXmlNode);
                            groupBoxNested.Enabled = true;
                            if (groupBox.Controls.Count > 0)
                            {
                                Control ctl = groupBox.Controls[groupBox.Controls.Count - 1];
                                if (ctl.GetType().Name == "RadioButton")
                                {
                                    RadioButton rad = (RadioButton)ctl;
                                    groupBoxNested.Enabled = rad.Checked;
                                }
                            }
                            groupBoxNested.Left = (groupBoxNested.Margin.Left + groupBoxNested.Padding.Left);
                            if (nButton == 0)
                            {
                                int nHeight = TextRenderer.MeasureText("X", groupBoxNested.Font).Height;
                                y += nHeight;
                            }
                            groupBoxNested.Top = y;
                            y += groupBoxNested.Height;
                            groupBox.Controls.Add(groupBoxNested);
                            if (xMax < groupBoxNested.Right) xMax = groupBoxNested.Right;
                            listGroupBoxNested.Add(groupBoxNested);
                            break;
                        }
                    case "RadioButton":
                        {
                            System.Windows.Forms.RadioButton radioButton = UIBase.GetRadioButton();
                            radioButton.AutoSize = true;
                            radioButton.Checked = (nButton == nButtonChecked);
                            radioButton.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
                            radioButton.Text = oXmlNode.Attributes["text"].Value;
                            if (nButton == 0) y += radioButton.PreferredSize.Height / 2;
                            radioButton.Top = y;
                            radioButton.Left = groupBox.Padding.Left;
                            y += radioButton.PreferredSize.Height;
                            groupBox.Controls.Add(radioButton);
                            if (xMax < radioButton.Right) xMax = radioButton.Right;
                            nButton++;
                            break;
                        }
                    default:
                        {
                            break;  // OOPS
                        }
                } // switch (oXmlNode.Name)
            } // foreach (XmlNode oXmlNode in oXmlNodeList)
            groupBox.Left = groupBox.Padding.Left;
            groupBox.Width = groupBox.Padding.Left + xMax + groupBox.Padding.Right;
            groupBox.Height = groupBox.Padding.Top + y + groupBox.Padding.Bottom;
            foreach (GroupBox groupBoxNested in listGroupBoxNested)
            {
                groupBoxNested.Width = xMax - (groupBox.Padding.Left + groupBox.Padding.Right);
            }
            return groupBox;
        } // ConfigurationGetGroupBox()

        private void ControlGetConfiguration(XmlTextWriter voCOEXmlTextWriter, Control vctlParent, string vstrTag)
        {
            voCOEXmlTextWriter.WriteStartElement(vstrTag);
            voCOEXmlTextWriter.WriteAttributeString("text", vctlParent.Text);
            {
                int nButtonChecked = 0;
                foreach (Control ctlChild in vctlParent.Controls)
                {
                    if (ctlChild.GetType().Name == "RadioButton")
                    {
                        if (((RadioButton)ctlChild).Checked)
                        {
                            voCOEXmlTextWriter.WriteAttributeString("member", vctlParent.Tag.ToString());
                            voCOEXmlTextWriter.WriteAttributeString("value", nButtonChecked.ToString());
                            break;
                        }
                        nButtonChecked++;
                    }
                } // foreach (Control ctlChild in Controls)
            }
            foreach (Control ctlChild in vctlParent.Controls)
            {
                switch (ctlChild.GetType().Name)
                {
                    case "GroupBox":
                        {
                            ControlGetConfiguration(voCOEXmlTextWriter, ctlChild, "GroupBox");
                            break;
                        }
                    case "RadioButton":
                        {
                            voCOEXmlTextWriter.WriteStartElement("RadioButton");
                            voCOEXmlTextWriter.WriteAttributeString("text", ctlChild.Text);
                            voCOEXmlTextWriter.WriteEndElement();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                } // switch (ctlChild.GetType().Name)
            } // foreach (Control ctlChild in Controls)
            voCOEXmlTextWriter.WriteEndElement();
            return;
        } // ControlGetConfiguration()

        private void GroupBoxWidthSet(Control vgrpParent, int vnWidth)
        {
            vgrpParent.Width = vnWidth;
            vnWidth -= (vgrpParent.Padding.Left + vgrpParent.Padding.Right);
            foreach (Control ctlChild in vgrpParent.Controls)
            {
                if (ctlChild is GroupBox)
                {
                    GroupBoxWidthSet(ctlChild, vnWidth - (ctlChild.Margin.Left + ctlChild.Margin.Right));
                }
            }
            return;
        } // GroupBoxWidthSet()
        #endregion

        #region constructors
        /// <summary>
        /// ! Constructor
        /// </summary>
        public Configuration()
        {
            InitializeComponent();
            // Programmatically add control(s)
            // 
            SuspendLayout();
            // _grpConfiguration added later by set Configuration
            _grpConfiguration = UIBase.GetGroupBox();
            Controls.Add(_grpConfiguration);
            Layout += new LayoutEventHandler(Configuration_Layout);
            //
            ResumeLayout(false);
            PerformLayout();
            Enter += new EventHandler(Configuration_Enter);
            return;
        } // Configuration()
        #endregion

        #region event handlers
        void Configuration_Enter(object sender, EventArgs e)
        {
            if (ActiveControl == null) SelectNextControl(_grpConfiguration, true, true, true, true);
            ActiveControl.Select();
            ActiveControl.Focus();
            return;
        } // Configuration_Enter()

        private void Configuration_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedComponent == e.AffectedControl) && (e.AffectedProperty == "Parent"))
            {
                // Vertical
                int y = 0;
                _grpConfiguration.Top = y;
                y += _grpConfiguration.Height + UIBase.ExtraPadding.Bottom;
                Height = y;
                // Horizontal
                int x = 0;
                _grpConfiguration.Left = x;
                x += _grpConfiguration.Width;
                Width = x;
                //
                if (_grpConfiguration.Width < Width) GroupBoxWidthSet(_grpConfiguration, Width);
            }
            return;
        } // Configuration_Layout()

        private void RadioButton_CheckedChanged(Object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            GroupBox groupBox = (GroupBox)(radioButton).Parent;
            bool bFound = false;
            foreach (Control control in groupBox.Controls)
            {
                if (bFound)
                {
                    if (control.GetType() == typeof(GroupBox))
                    {
                        control.Enabled = radioButton.Checked;
                    }
                    break;
                }
                else if (control == sender)
                {
                    bFound = true;
                }
            }
            return;
        } // RadioButton_CheckedChanged
        #endregion

    } // class Configuration
}
