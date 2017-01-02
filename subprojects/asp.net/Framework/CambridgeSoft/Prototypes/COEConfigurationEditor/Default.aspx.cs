

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
using System.IO;
using System.Drawing;
using System.Xml;
using System.Net;
using System.Net.Sockets;




public partial class _Default : System.Web.UI.Page
{

    private string FXMLFilename;
    private string FXSDFilename;
    private string FXMLDataSaveUrl;
    string[] FCurrentdirectory ;

    static private XmlNode     FUltraWebNavigatorSelectedNode=null;
    static private XmlNodeList FUltraWebNavigatorSelectedNodes = null;
    static private XmlNodeList FUltraWebNavigatorSelectedChildNodes = null;
    static private XmlNodeList FUltraWebNavigatorSelectedGrandChildNodes = null;

    private XmlDocument FXmlDocument = new XmlDocument();
    private XmlDocument FXsdDocument = new XmlDocument();

   

    
    protected void Page_Load(object sender, EventArgs e)
    {
        //Set page information
        FXMLFilename = ConfigurationManager.AppSettings["XMLTemporaryFile"];
        FXSDFilename = ConfigurationManager.AppSettings["XSDTemporaryFile"];
        FCurrentdirectory = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
        FXMLDataSaveUrl = "http://" + Request.ServerVariables["SERVER_NAME"] + "//" + FCurrentdirectory[FCurrentdirectory.Length - 1];

        //Verify if we have access to Temporary.xml
        try
        {
            WebRequest request = WebRequest.Create(FXMLDataSaveUrl+"//Temporary.xml");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }
        catch (Exception E)
        {
            System.Web.HttpContext.Current.Response.Write("WARNING: !!! Is necesary have access to " + FXMLDataSaveUrl + ". You should have writing permissions, too. - (" + E.Message + ")");
        }

         
    
        
        
        //Load a XML to edit
        FXmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "/" + FXMLFilename);
        FXsdDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "/" + FXSDFilename);

        SaveXMLChanges(AppDomain.CurrentDomain.BaseDirectory + "/" + FXMLFilename);

        //Build components of Navigator Pane
        BuildNavigatorPane();

        //Build components of ditor Pane
        BuildEditorPane();
    }


// ** BEGIN ** Procedures and Functions pertinent to the "Navigator Pane"

// ** BEGIN ** Procedures and Functions pertinent to the "Navigator Pane"

    // Create the Panes and the trees of each Pane of the "Navigator Pane"

    protected void BuildNavigatorPane()
    {
        // Build a Panel with each node the first level of the XML
        XmlNode LXmlNodeCOEConfig = FXmlDocument.SelectSingleNode("//COEConfig");

        XmlNodeList LXmlNodeList = LXmlNodeCOEConfig.ChildNodes;

        foreach (XmlNode LXmlNode in LXmlNodeList)
        {
            // Create and form a pane
            Infragistics.WebUI.Misc.WebPanel LWebPanel;
            LWebPanel = new Infragistics.WebUI.Misc.WebPanel();

            LWebPanel.Text = getNodeLabel(LXmlNode);
            LWebPanel.ToolTip = getNodeToolTip(LXmlNode);
            LWebPanel.ImageDirectory = "~/ig_common/20063CLR20/Styles/Office2007Blue/WebListBar/";
            LWebPanel.Header.BackgroundImage = "igpnl_office2k3_drk.png";
            LWebPanel.Header.BorderColor = Color.FromArgb(0, 45, 150);
            LWebPanel.Header.BorderStyle = System.Web.UI.WebControls.BorderStyle.Solid;
            LWebPanel.Header.BorderWidth = new Unit(1);
            LWebPanel.Header.ExpandedAppearance.Style.BackgroundImage = "igpnl_office2k3_drk.png";
            LWebPanel.Header.ExpandedAppearance.Style.BorderColor = Color.FromName("#002D96");
            LWebPanel.Header.ExpandedAppearance.Style.BorderStyle = System.Web.UI.WebControls.BorderStyle.Solid;
            LWebPanel.Header.ExpandedAppearance.Style.BorderWidth = new System.Web.UI.WebControls.Unit(1);
            LWebPanel.Header.ExpandedAppearance.Style.Font.Name = "Times New Roman";
            LWebPanel.Header.ExpandedAppearance.Style.Font.Size = 12;
            LWebPanel.Header.ExpandedAppearance.Style.ForeColor = Color.FromArgb(255, 255, 255);
            LWebPanel.Header.ExpansionIndicator.Height = new System.Web.UI.WebControls.Unit(25);
            LWebPanel.Header.ExpansionIndicator.Width = new System.Web.UI.WebControls.Unit(15);
            LWebPanel.Header.HoverAppearance.Style.Font.Size = 12;
            LWebPanel.Header.CollapsedAppearance.Style.BackgroundImage = "igpnl_office2k3_drk.png";
            LWebPanel.Header.CollapsedAppearance.Style.BorderColor = Color.FromName("#002D96");
            LWebPanel.Header.CollapsedAppearance.Style.BorderStyle = System.Web.UI.WebControls.BorderStyle.Solid;
            LWebPanel.Header.CollapsedAppearance.Style.BorderWidth = new System.Web.UI.WebControls.Unit(1);
            LWebPanel.Header.CollapsedAppearance.Style.Font.Name = "Times New Roman";
            LWebPanel.Header.CollapsedAppearance.Style.Font.Size = 12;
            LWebPanel.Header.CollapsedAppearance.Style.ForeColor = Color.FromArgb(255, 255, 255);
            LWebPanel.Header.ExpansionIndicator.Height = new System.Web.UI.WebControls.Unit(15);
            LWebPanel.Header.ExpansionIndicator.Width = new System.Web.UI.WebControls.Unit(15);
            LWebPanel.Header.Font.Name = "Times New Roman";
            LWebPanel.Header.Font.Size = 12;
            LWebPanel.Header.Height = 20;
            LWebPanel.Height = 20;
            LWebPanel.PanelStyle.BorderColor = Color.FromName("#002D96");
            LWebPanel.PanelStyle.Padding.Bottom = new System.Web.UI.WebControls.Unit(1);
            LWebPanel.PanelStyle.Padding.Left = new System.Web.UI.WebControls.Unit(1);
            LWebPanel.PanelStyle.Padding.Right = new System.Web.UI.WebControls.Unit(1);
            LWebPanel.PanelStyle.Padding.Top = new System.Web.UI.WebControls.Unit(1); ;
            LWebPanel.Width = new System.Web.UI.WebControls.Unit("90%");
            LWebPanel.Header.TextAlignment = Infragistics.WebUI.Misc.TextAlignment.Left;
            LWebPanel.Header.CollapsedAppearance.Style.BackColor = Color.FromName("RoyalBlue");
            LWebPanel.Header.ExpandedAppearance.Style.BackColor = Color.FromName("RoyalBlue");
            LWebPanel.Header.ExpandedAppearance.Style.BackColor = Color.FromName("RoyalBlue");

            Panel.Controls.Add(LWebPanel);

            // Create and form a tree
            Infragistics.WebUI.UltraWebNavigator.UltraWebTree LUltraWebTreeMain = new Infragistics.WebUI.UltraWebNavigator.UltraWebTree();
            LUltraWebTreeMain.WebTreeTarget = Infragistics.WebUI.UltraWebNavigator.WebTreeTarget.ClassicTree;
            LUltraWebTreeMain.NodeClicked += new Infragistics.WebUI.UltraWebNavigator.NodeClickedEventHandler(UltraWebTree_NodeClicked);

            BuildTree(LUltraWebTreeMain, LXmlNode);

            LWebPanel.Controls.Add(LUltraWebTreeMain);

        }
    }

    // Build the Tree into the Navigation Pane
    protected void BuildTree(Infragistics.WebUI.UltraWebNavigator.UltraWebTree AUltraWebTreeMain, XmlNode AXmlNode)
    {
        
        AddChildNodes(AUltraWebTreeMain, AXmlNode, null);

        //if exists a "Advanced" Node then move it to last node
        Infragistics.WebUI.UltraWebNavigator.Node LAdvancedNode=null;
        string LNodeAdvancesLabel = ConfigurationManager.AppSettings["SpecialTagName"];
        for (int i = 0; i < AUltraWebTreeMain.Nodes.Count; i++)
        {
            if (AUltraWebTreeMain.Nodes[i].Text == LNodeAdvancesLabel)
            {
                LAdvancedNode = AUltraWebTreeMain.Nodes[i];
                break;
            }

        }
        if (LAdvancedNode != null)
        {
            AUltraWebTreeMain.Nodes.Remove(LAdvancedNode);
            AUltraWebTreeMain.Nodes.Add(LAdvancedNode);
        }
        //End if exists a Advanced Node then move it to last node
    }

    // Create and Add the child nodes of a node into a Tree
    protected void AddChildNodes(Infragistics.WebUI.UltraWebNavigator.UltraWebTree AUltraWebTreeMain, XmlNode AXmlNode, Infragistics.WebUI.UltraWebNavigator.Node AParentNode)
    {
        
        XmlNodeList LXmlNodeList = AXmlNode.ChildNodes;
        Infragistics.WebUI.UltraWebNavigator.Node LNewNode;

        string LAttributeAdvancesLabel = ConfigurationManager.AppSettings["SpecialAttributeName"];

        foreach (XmlNode LXmlChildNode in LXmlNodeList)
        {
            LNewNode = new Infragistics.WebUI.UltraWebNavigator.Node();
            LNewNode.Text = getNodeLabel(LXmlChildNode);
            LNewNode.ToolTip = getNodeToolTip(LXmlChildNode);

            if (AParentNode == null)
            {
                LNewNode.Tag = "//COEConfig/" + AXmlNode.Name + "/" + LXmlChildNode.Name;
                AUltraWebTreeMain.Nodes.Add(LNewNode);   
            }
            else
            {
                LNewNode.Tag = AParentNode.Tag + "/" + LXmlChildNode.Name;
                AParentNode.Nodes.Add(LNewNode);
            }

            // if the Node is a Advanced Node then assign it to a "Advanced" Node.
            if (LXmlChildNode.Attributes != null)
            {
                XmlAttribute LXMLAdvancedAttribute = LXmlChildNode.Attributes[LAttributeAdvancesLabel];
                if (LXMLAdvancedAttribute != null)
                {
                    if (LXMLAdvancedAttribute.Value == "true")
                    {
                        Infragistics.WebUI.UltraWebNavigator.Node LAdvancedNode=null;
                        string LNodeAdvancesLabel = ConfigurationManager.AppSettings["SpecialTagName"];
                        for (int i = 0; i < AUltraWebTreeMain.Nodes.Count; i++)
                        {
                            if (AUltraWebTreeMain.Nodes[i].Text == LNodeAdvancesLabel)
                            {
                                LAdvancedNode = AUltraWebTreeMain.Nodes[i];
                                break;
                            }
                        }
                        if (LAdvancedNode == null)
                        {
                            LAdvancedNode = new Infragistics.WebUI.UltraWebNavigator.Node();
                            LAdvancedNode.Text = LNodeAdvancesLabel;
                            LAdvancedNode.Tag = "//COEConfig/" + AXmlNode.Name + "/" + LXmlChildNode.Name; 
                            AUltraWebTreeMain.Nodes.Add(LAdvancedNode);
                        }
                        if (LNewNode.Parent == null)
                        {
                            AUltraWebTreeMain.Nodes.Remove(LNewNode);
                        }
                        else
                        {
                            LNewNode.Parent.Nodes.Remove(LNewNode);
                        }
                        LAdvancedNode.Nodes.Add(LNewNode); 
                    }
                }
            }       

            string LshowChildrenInEditorAttributeValue = "false";
            if (LXmlChildNode.Attributes != null)
            {
                XmlAttribute LXMLshowChildrenInEditorAttribute = LXMLshowChildrenInEditorAttribute = LXmlChildNode.Attributes[ConfigurationManager.AppSettings["showChildrenInEditorAttributeName"]];
                if (LXMLshowChildrenInEditorAttribute != null)
                {
                    LshowChildrenInEditorAttributeValue = LXMLshowChildrenInEditorAttribute.Value;
                }
            }
            
            if (LshowChildrenInEditorAttributeValue != "true")
            {
                if (LNewNode.Level < 1)
                {
                    AddChildNodes(AUltraWebTreeMain, LXmlChildNode, LNewNode);
                }
            }
        }

    }

    // Return Label of the Node.
    protected string getNodeLabel(XmlNode AXmlNode)
    {
        //  If the "configEditorName" attribute exist then get it but if the "name" attribute exist then get it but get node name

        XmlAttribute LXmlAttribute;

        if (AXmlNode.Attributes != null)
        {
            LXmlAttribute = AXmlNode.Attributes["configEditorName"];
            if (LXmlAttribute != null)
            {
                if (LXmlAttribute.Value.Trim() != "")
                {
                    return (LXmlAttribute.Value);
                }
            }
            LXmlAttribute = AXmlNode.Attributes["name"];
            if (LXmlAttribute != null)
            {
                if (LXmlAttribute.Value.Trim() != "")
                {
                    return (LXmlAttribute.Value);
                }
            }

            return (AXmlNode.Name);
        }
        else return (AXmlNode.Name);
    }

    // Return the tooltip of a Node from the "toolTip" attribute
    protected string getNodeToolTip(XmlNode AXmlNode)
    {
        XmlAttribute LXmlAttribute;

        if (AXmlNode.Attributes != null)
        {
            LXmlAttribute = AXmlNode.Attributes["toolTip"];
            if (LXmlAttribute != null)
            {
                return (LXmlAttribute.Value);
            }
            else return ("");
        }
        else return ("");
    }

// ** END ** Procedures and Functions pertinent to the "Navigator Pane"

// ** BEGIN ** Procedures and Functions pertinent to the "Editor Pane"

    // Form each tab and each "Authentic Document View" of the Editor Pane
    protected void BuildEditorPane()
    {
        SetAuthenticDocumentView(FUltraWebNavigatorSelectedNode, advSNE);
        Activate_uwtNLE((Infragistics.WebUI.UltraWebTab.Tab)uwtNLE.Tabs[uwtNLE.SelectedTab]);
    }

    // Form a editor pane for a selected node
    protected void SetAuthenticDocumentView(XmlNode ANode, Altova.Authentic.WebControls.AuthenticDocumentView AAuthenticDocumentView)
    {
        if (ANode == null) return;

        // Set the data for the "Authentic DocumentView" component
        string[] Currentdirectory = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
        string LSPSDirectory = ConfigurationManager.AppSettings["SPSDirectory"] + "/";

        string LXMLDataURL = FXMLFilename;
        string LSchemaDataURL = FXSDFilename;

        string LSPSPath = AppDomain.CurrentDomain.BaseDirectory + LSPSDirectory;
        if (!Directory.Exists(LSPSPath))
        {
            Directory.CreateDirectory(LSPSPath);
        }
        string LXMLPathFilename = AppDomain.CurrentDomain.BaseDirectory + FXMLFilename;

        string LSPSFilename = DefineNodeSPS(ANode, LSPSPath, LXMLPathFilename);
        string LSPSDataURL = LSPSDirectory + LSPSFilename;

        int LCountAttributeShowed = 5;

        LCountAttributeShowed = GetCountShowedAttribute(LSPSPath + LSPSFilename);

        // END Set the data to the "Authentic DocumentView" component

        // Set property to the "Authentic DocumentView" component
        if (LCountAttributeShowed > 0)
        {
            AAuthenticDocumentView.Visible = true;
            AAuthenticDocumentView.Height = LCountAttributeShowed * 35 + 60;
        }
        else
            AAuthenticDocumentView.Visible = false;

        AAuthenticDocumentView.SPSDataURL = LSPSDataURL;
        AAuthenticDocumentView.SchemaDataURL = LSchemaDataURL;
        AAuthenticDocumentView.XMLDataURL = LXMLDataURL;
        AAuthenticDocumentView.XMLDataSaveUrl = FXMLDataSaveUrl+ "//" + AAuthenticDocumentView.ID + FXMLFilename;;
        
    }

    //Get the Count showed Attribute into a SPS Document
    protected int GetCountShowedAttribute(string ASPSDataURL)
    {
        char LQuotes = '"';

        //TextReader LtrSPSDocument = new StreamReader(ASPSDataURL);

        XmlDocument LSPSDocument = new XmlDocument();

        LSPSDocument.Load(ASPSDataURL);

        string[] LTextOfComponentes = new string[] { "<template match=" + LQuotes + "COEConfig" + LQuotes + ">" };

        string[] LSplit = LSPSDocument.InnerXml.Split(LTextOfComponentes, StringSplitOptions.None);


        return (LSplit.Length - 1);

    }

    // Get or Create and then get the SPS file for a node
    protected string DefineNodeSPS(XmlNode ANode, string ASPSPath, string AXMLPathFilename)
    {
        string LFilename = ANode.Name;
        for (XmlNode LNodeParent = ANode.ParentNode; LNodeParent != null; LNodeParent = LNodeParent.ParentNode)
        {
            LFilename = LNodeParent.Name + "." + LFilename;
        }

        string LSPSFilename = LFilename + ".sps";
        string LSPSPathFilename = ASPSPath + LSPSFilename;

        if (!File.Exists(LSPSPathFilename))
        {
            BuildSPS(ANode, LSPSPathFilename);
        }
        else
        {
            SetFilterAttributes(ANode, LSPSPathFilename);
        }

        return (LSPSFilename);
    }

    // Search a node using a Name into child of a node
    protected XmlNode SearchNode(XmlNode AParentNode,string  ANameNode)
    {
        foreach (XmlNode LCurrentNode in AParentNode.ChildNodes)
        {
            if (LCurrentNode.Name==ANameNode)
               return  LCurrentNode;
        }
        return null;
    }

    // Get the attribute value if exists it
    protected string getAttributeValue(XmlNode ANode, string ANameAttribute)
    {

        if (ANode != null)
        {
            XmlAttributeCollection LAttributeCollection = ANode.Attributes;
            if (LAttributeCollection != null)
            {
                XmlAttribute LAttribute = LAttributeCollection[ANameAttribute];
                if (LAttribute != null)
                    return (LAttribute.Value);
            }
        }
            
        return "";
    }

    // Identify and get the data type used by attribute value
    protected int getTypeField(XmlNode ANode, XmlAttribute AAttribute)
    {

        foreach (XmlNode LXMLCurrentNode1 in FXsdDocument.DocumentElement.ChildNodes)
        {
            if (getAttributeValue(LXMLCurrentNode1, "name") == ANode.Name)
            {
                XmlNode LcomplexTypeNode = SearchNode(LXMLCurrentNode1, "xs:complexType");
                if (LcomplexTypeNode != null)
                {
                    foreach (XmlNode LXMLCurrentNode2 in LcomplexTypeNode.ChildNodes)
                    {
                        if (getAttributeValue(LXMLCurrentNode2, "name") == AAttribute.Name)
                        {
                            XmlNode LsimpleTypeNode = SearchNode(LXMLCurrentNode2, "xs:simpleType");
                            if (LsimpleTypeNode != null)
                            {
                                XmlNode LrestrictionNode = SearchNode(LsimpleTypeNode, "xs:restriction");
                                if (LrestrictionNode != null)
                                {
                                    XmlNode LenumerationNode = SearchNode(LrestrictionNode, "xs:enumeration");
                                    if (LenumerationNode != null)
                                    {
                                        foreach (XmlNode LXMLCurrentNode4 in LrestrictionNode.ChildNodes)
                                        {
                                            if ((getAttributeValue(LXMLCurrentNode4, "value").ToUpper() == "YES") || (getAttributeValue(LXMLCurrentNode4, "value").ToUpper() == "NO"))
                                            {
                                                continue;
                                            }
                                          else return (3);
                                        }
                                        return (4);
                                    }
                                        
                                }
                            }
                        }
                    }
                }
            }
        }
         
        return (1);
        
    }

    // Get the template file name in accordance with the node type
    protected string getTemplateFileName(XmlNode ANode, XmlAttribute AAttribute)
    {
        int LType = getTypeField(ANode,AAttribute);
        string LFileName="";
        switch(LType) 
        {
            case 1: LFileName = "ImputField.sps"; break;
            case 2: LFileName = "MultipleImputField.sps"; break;
            case 3: LFileName = "ComboBox.sps"; break;
            case 4: LFileName = "CheckBox.sps"; break;
            case 5: LFileName = "RadioButton.sps"; break;
            case 7: LFileName = "DateButton.sps"; break;
            case 6: LFileName = "LinkEmail.sps"; break;
        }
        return (LFileName);
    }

    // Create a SPS for a node
    protected void BuildSPS(XmlNode ANode, string ASPSPathFilename)
    {

            //Select a sps template in accordance with the node type

            string SPSTemplateDirectory = ConfigurationManager.AppSettings["SPSTemplateDirectory"];
            string SPSPathTemplateDirectory = AppDomain.CurrentDomain.BaseDirectory + SPSTemplateDirectory;
            string BaseSourceFileName = "Base.sps";
            string SPSPathBaseTemplateFileName = SPSPathTemplateDirectory + '/' + BaseSourceFileName;
            XmlDocument LSPSDocument = new XmlDocument();
            LSPSDocument.Load(SPSPathBaseTemplateFileName);

            XmlNode LXMLTargetNode = ANode;
            if (LXMLTargetNode != null)
            {

                XmlAttributeCollection LXMLTargetAttributeCollection = LXMLTargetNode.Attributes;

                XmlNode LSPSParagraphChildreNewNode=null;

                //Add a tag for each one of the node attributes
                foreach (XmlAttribute LSPSCurrentAttributeTargetNode in LXMLTargetAttributeCollection)
                {

                    string LHideAttributes = ConfigurationManager.AppSettings["AttributesHide"].ToUpper();
                    if (LHideAttributes.Contains("'" + LSPSCurrentAttributeTargetNode.Name.ToUpper() + "'"))
                        continue;


                    string SourceFileName = getTemplateFileName(LXMLTargetNode, LSPSCurrentAttributeTargetNode);
                    string SPSPathTemplateFileName = SPSPathTemplateDirectory + '/' + SourceFileName;

                    XmlDocument LSPSTemplateDocument = new XmlDocument();
                    LSPSTemplateDocument.Load(SPSPathTemplateFileName);


                    XmlNode LSPSParagraphNode = LSPSDocument.SelectSingleNode("//structure/parts/children/globaltemplate/children/template/children/paragraph");

                    // Template Node for Data Imput
                    XmlNode LSPSParagraphChildrenNode = LSPSTemplateDocument.SelectSingleNode("//structure/parts/children/globaltemplate/children/template/children/paragraph/children");

                    if (LSPSParagraphChildreNewNode == null)
                    {
                        LSPSParagraphChildreNewNode = LSPSDocument.CreateNode(XmlNodeType.Element, "children", "");
                        LSPSParagraphNode.AppendChild(LSPSParagraphChildreNewNode);
                    }

                    XmlNode LSPSNewTemplateNode = LSPSDocument.CreateNode(XmlNodeType.Element, "template", "");
                    XmlAttribute LSPSAttributeNewTemplateNode = LSPSDocument.CreateAttribute("match");
                    LSPSAttributeNewTemplateNode.Value = "@" + LSPSCurrentAttributeTargetNode.Name;
                    LSPSNewTemplateNode.Attributes.Append(LSPSAttributeNewTemplateNode);
                    LSPSParagraphChildreNewNode.AppendChild(LSPSNewTemplateNode);

                    XmlNode LSPSCloneParagraphChildrenNode = LSPSParagraphChildrenNode.CloneNode(true);
                    XmlNode LSPSImputNode = LSPSDocument.CreateNode(LSPSCloneParagraphChildrenNode.NodeType, LSPSCloneParagraphChildrenNode.Name, null);
                    LSPSImputNode.InnerXml = LSPSCloneParagraphChildrenNode.InnerXml;
                   
                    //Automatization of labels
                    XmlNode LXmlTextNode = LSPSImputNode.SelectSingleNode("//text");
                    if (LXmlTextNode != null)
                    {
                        XmlAttribute LXmlTextAttribute = LXmlTextNode.Attributes["fixtext"];
                        if (LXmlTextAttribute != null)
                        {
                            if (LXmlTextAttribute.Value.IndexOf(":")!=-1)
                                LXmlTextAttribute.Value = LSPSCurrentAttributeTargetNode.Name + ": ";
                            else
                                LXmlTextAttribute.Value = LSPSCurrentAttributeTargetNode.Name;
                        }
                    }

                    XmlNode LXmlConditionalBranchNode = LSPSImputNode.SelectSingleNode("//conditionbranch");
                    if (LXmlConditionalBranchNode != null)
                    {
                        XmlAttribute LXmlConditionalBranchAttribute = LXmlConditionalBranchNode.Attributes["xpath"];
                        if (LXmlConditionalBranchAttribute != null)
                        {
                            XmlAttribute LXmlconfigEditorNameAttribute = LXMLTargetNode.Attributes["configEditorName"];
                            XmlAttribute LXmlnameAttribute = LXMLTargetNode.Attributes["name"];

                            if (LXmlconfigEditorNameAttribute != null)
                            {
                                LXmlConditionalBranchAttribute.Value = "../@configEditorName=\"" + LXmlconfigEditorNameAttribute.Value + "\"";
                            }
                            if (LXmlnameAttribute != null)
                            {
                                if (LXmlConditionalBranchAttribute.Value != "")
                                    LXmlConditionalBranchAttribute.Value = LXmlConditionalBranchAttribute.Value + " and ";
                                LXmlConditionalBranchAttribute.Value = LXmlConditionalBranchAttribute.Value + "../@name=\"" + LXmlnameAttribute.Value + "\"";

                            }

                        }
                    }

                    
                    LSPSNewTemplateNode.AppendChild(LSPSImputNode);

                    XmlNode LSPSOldChildrenNode = LSPSNewTemplateNode;
                    XmlNode LXMLCurrentTreeNode = LXMLTargetNode;

                    //Add tags for each parent node of the tree node
                    while (LXMLCurrentTreeNode.ParentNode != null)
                    {

                        XmlNode LSPSNewTreeNode = LSPSDocument.CreateNode(XmlNodeType.Element, "template", "");
                        XmlAttribute LSPSAttribute = LSPSDocument.CreateAttribute("match");
                        LSPSAttribute.Value = LXMLCurrentTreeNode.Name;
                        LSPSNewTreeNode.Attributes.Append(LSPSAttribute);

                        LSPSParagraphChildreNewNode.AppendChild(LSPSNewTreeNode);

                        XmlNode LXmlChildrenTemplateNode = LSPSDocument.CreateNode(XmlNodeType.Element, "children", "");
                        LSPSNewTreeNode.AppendChild(LXmlChildrenTemplateNode);

                        LXmlChildrenTemplateNode.AppendChild(LSPSOldChildrenNode);
                        LSPSOldChildrenNode = LSPSNewTreeNode;
                        LXMLCurrentTreeNode = LXMLCurrentTreeNode.ParentNode;
                    }

                }
            }

            //Save the new sps file
            LSPSDocument.Save(ASPSPathFilename);

        
    }

    protected string getFilterAttributes(XmlNode ANode, int ALevel)
    {
        string LFilterAttributes = "";

        if (ANode != null)
        {
            XmlAttributeCollection LXmlAttributeCollection = ANode.Attributes;
            if (LXmlAttributeCollection!=null)
            {
                ALevel = ALevel + 1;

                XmlAttribute LXmlconfigEditorNameAttribute = LXmlAttributeCollection["configEditorName"];
                XmlAttribute LXmlnameAttribute = ANode.Attributes["name"];

                string LPathParent = "";
                for (int Index = 0; Index <= ALevel; Index++)
                {
                    LPathParent = LPathParent + "../";
                }

                if (LXmlconfigEditorNameAttribute != null)
                {
                   
                    LFilterAttributes = LPathParent+"@configEditorName=\"" + LXmlconfigEditorNameAttribute.Value + "\"";
                }
                if (LXmlnameAttribute != null)
                {
                    if (LFilterAttributes != "")
                        LFilterAttributes = LFilterAttributes + " and ";
                    LFilterAttributes = LFilterAttributes + LPathParent+"@name=\"" + LXmlnameAttribute.Value + "\"";
                }
                XmlNode LParentNode = ANode.ParentNode;
                if (LParentNode != null)
                {
                    string LParentFilterAttributes = getFilterAttributes(LParentNode, ALevel);

                    if (LParentFilterAttributes != "")
                    {
                        if (LFilterAttributes != "")
                            LFilterAttributes = LFilterAttributes + " and ";
                        LFilterAttributes = LFilterAttributes + LParentFilterAttributes;
                    }
                }
            }
        }
        return (LFilterAttributes);
    }
    // Filter the Attributes of a node to show into each Tab 
    protected void SetFilterAttributes(XmlNode ANode, string ASPSPathFilename)
    {
        XmlDocument LSPSDocument = new XmlDocument();
        LSPSDocument.Load(ASPSPathFilename);

        XmlNodeList LXmlConditionalBranchNodes = LSPSDocument.SelectNodes("//conditionbranch");
        foreach (XmlNode LXmlConditionalBranchNode in LXmlConditionalBranchNodes)
        {
            XmlAttribute LXmlConditionalBranchAttribute = LXmlConditionalBranchNode.Attributes["xpath"];
            if (LXmlConditionalBranchAttribute != null)
            {
                XmlAttribute LXmlconfigEditorNameAttribute = ANode.Attributes["configEditorName"];
                XmlAttribute LXmlnameAttribute = ANode.Attributes["name"];

                if (LXmlconfigEditorNameAttribute != null)
                {
                    LXmlConditionalBranchAttribute.Value ="../@configEditorName=\"" + LXmlconfigEditorNameAttribute.Value + "\"";
                }
                if (LXmlnameAttribute != null)
                {
                    if (LXmlConditionalBranchAttribute.Value != "")
                        LXmlConditionalBranchAttribute.Value = LXmlConditionalBranchAttribute.Value + " and ";
                    LXmlConditionalBranchAttribute.Value = LXmlConditionalBranchAttribute.Value + "../@name=\"" + LXmlnameAttribute.Value + "\"";
                }
                XmlNode LParentNode = ANode.ParentNode;
                if (LParentNode != null)
                {
                    string LParentFilterAttributes = getFilterAttributes(LParentNode,0);

                    if (LParentFilterAttributes != "")
                    {
                        if (LXmlConditionalBranchAttribute.Value != "")
                            LXmlConditionalBranchAttribute.Value = LXmlConditionalBranchAttribute.Value + " and ";
                        LXmlConditionalBranchAttribute.Value = LXmlConditionalBranchAttribute.Value + LParentFilterAttributes;
                    }
                }
            }
        }
        LSPSDocument.Save(ASPSPathFilename);
    }

    // Get the XML complete path of a Node 
    protected string GetNodePath(Infragistics.WebUI.UltraWebNavigator.Node ANode)
    {

        Infragistics.WebUI.UltraWebNavigator.Node LNodeParent = ANode.Parent;

        string LNodePath = ANode.Tag.ToString();
        for (; LNodeParent != null; )
        {
            LNodePath = LNodeParent.Text + "/" + LNodePath;
            LNodeParent = LNodeParent.Parent;
        }

        return ( "//COEConfig/"+LNodePath);
    }

    // When a tree node  is selected the attributes of it are show into the "Editor Pane"
    protected void UltraWebTree_NodeClicked(object sender, Infragistics.WebUI.UltraWebNavigator.WebTreeNodeEventArgs e)
    {
        EditPane.Visible = true;
        uwtNLE.SelectedTab = 0;
        Infragistics.WebUI.UltraWebNavigator.UltraWebTree LUltraWebNavigator = (Infragistics.WebUI.UltraWebNavigator.UltraWebTree)sender;

        string XPath = "";
        if (LUltraWebNavigator.SelectedNode.Level == 1)
        {
            int LIndex = LUltraWebNavigator.SelectedNode.Index + 1;
            XPath = LUltraWebNavigator.SelectedNode.Tag.ToString() + "[" + LIndex.ToString() + "]";
        }
        else
        {
           XPath = LUltraWebNavigator.SelectedNode.Tag.ToString();
        }
        XmlNode LNode = FXmlDocument.SelectSingleNode(XPath);
        FUltraWebNavigatorSelectedNode = LNode;
        BuildEditPaneNLE(LNode);
    }

    // Create and show the components (AuthenticDocumentView and tabs) pertinent to a Node of the 1º Level
    protected void BuildEditPaneNLE(XmlNode ANode)
    {
        if (ANode == null) return;

        XmlNode LXmlTargetNode = ANode;

        //Single Node editor (SNE)
        SetAuthenticDocumentView(LXmlTargetNode, advSNE);

        //End Single Node editor (SNE)

        //Node List editor (NLE)
        if (LXmlTargetNode.ChildNodes.Count > 0)
        {

            uwtNLE.Visible = true;
            uwtNLE.Tabs.Clear();

            foreach (XmlNode LXMLCurrentNode in LXmlTargetNode.ChildNodes)
            {
                Infragistics.WebUI.UltraWebTab.Tab LTab = new Infragistics.WebUI.UltraWebTab.Tab();
                LTab.Text = getNodeLabel(LXMLCurrentNode);
                uwtNLE.Tabs.Add(LTab);
            }

            Infragistics.WebUI.UltraWebTab.Tab LTabIndex = (Infragistics.WebUI.UltraWebTab.Tab)uwtNLE.Tabs[uwtNLE.SelectedTab];
            LTabIndex.ContentPane.Controls.Add(advSNEofNLE);
            LTabIndex.ContentPane.Controls.Add(uwtCNLE);

            if (LXmlTargetNode.ChildNodes.Count > 0)
                BuildEditPaneCNLE(LXmlTargetNode.ChildNodes[0]);


            FUltraWebNavigatorSelectedNodes = LXmlTargetNode.ChildNodes;
        }
        else
        {
            uwtNLE.Visible = false;
        }
        //End Node List editor (NLE)

    }
     
    //Create and show the components (AuthenticDocumentView and tabs) pertinent to a Node of the 2º Level
    protected void BuildEditPaneCNLE(XmlNode ANode)
    {
        if (ANode == null) return;

        XmlNode LXmlTargetNode = ANode;

        //Single Node editor (SNE)
        SetAuthenticDocumentView(ANode, advSNEofNLE);

        //End Single Node editor (SNE)

        //Node List editor (NLE)
        if (LXmlTargetNode.ChildNodes.Count > 0)
        {
            uwtCNLE.Visible = true;
            uwtCNLE.Tabs.Clear();

            foreach (XmlNode LXMLCurrentNode in LXmlTargetNode.ChildNodes)
            {
                Infragistics.WebUI.UltraWebTab.Tab LTab = new Infragistics.WebUI.UltraWebTab.Tab();
                LTab.Text = getNodeLabel(LXMLCurrentNode);
                uwtCNLE.Tabs.Add(LTab);
            }
            //uwtCNLE.SelectedTab = 0;
            Infragistics.WebUI.UltraWebTab.Tab LTabSelected = (Infragistics.WebUI.UltraWebTab.Tab)uwtCNLE.Tabs[uwtCNLE.SelectedTab];
            LTabSelected.ContentPane.Controls.Add(advSNEofCNLE);
            LTabSelected.ContentPane.Controls.Add(uwtGNLE);

            if (LXmlTargetNode.ChildNodes.Count > 0)
            {
                SetAuthenticDocumentView(LXmlTargetNode.ChildNodes[0], advSNEofCNLE);
                BuildEditPaneGNLE(LXmlTargetNode.ChildNodes[0]);
            }

            FUltraWebNavigatorSelectedChildNodes = LXmlTargetNode.ChildNodes;
        }
        else
        {
            uwtCNLE.Visible = false;
        }
        //End Node List editor (NLE)

    }

    // Create and show the components (AuthenticDocumentView and tabs) pertinent to a Node of the 3º Level
    protected void BuildEditPaneGNLE(XmlNode ANode)
    {
        if (ANode == null) return;

        XmlNode LXmlTargetNode = ANode;


        //Single Node editor (SNE)
        SetAuthenticDocumentView(ANode, advSNEofCNLE);

        //End Single Node editor (SNE)

        //Node List editor (NLE)
        if (LXmlTargetNode.ChildNodes.Count > 0)
        {
            uwtGNLE.Visible = true;
            uwtGNLE.Tabs.Clear();

            foreach (XmlNode LXMLCurrentNode in LXmlTargetNode.ChildNodes)
            {
                Infragistics.WebUI.UltraWebTab.Tab LTab = new Infragistics.WebUI.UltraWebTab.Tab();
                LTab.Text = getNodeLabel(LXMLCurrentNode);
                uwtGNLE.Tabs.Add(LTab);
            }
            //uwtGNLE.SelectedTab = 0;
            Infragistics.WebUI.UltraWebTab.Tab LTabIndex = (Infragistics.WebUI.UltraWebTab.Tab)uwtGNLE.Tabs[uwtGNLE.SelectedTab];
            LTabIndex.ContentPane.Controls.Add(advSNEofGNLE);

            if (LXmlTargetNode.ChildNodes.Count > 0)
                SetAuthenticDocumentView(LXmlTargetNode.ChildNodes[0], advSNEofGNLE);

            FUltraWebNavigatorSelectedGrandChildNodes = ANode.ChildNodes;
        }
        else
        {
            uwtGNLE.Visible = false;
        }
        //End Node List editor (NLE)

    }

    // Show the information of the selected Node Tab of the 1º Level 
    protected void Activate_uwtNLE(Infragistics.WebUI.UltraWebTab.Tab ATabSelected)
    {
        ATabSelected.ContentPane.Controls.Add(advSNEofNLE);

        if (FUltraWebNavigatorSelectedNodes != null)
        {
            SetAuthenticDocumentView(FUltraWebNavigatorSelectedNodes[uwtNLE.Tabs.IndexOf(ATabSelected)], advSNEofNLE);

            XmlNode LNode = FUltraWebNavigatorSelectedNodes[uwtNLE.Tabs.IndexOf(ATabSelected)];

            ATabSelected.ContentPane.Controls.Add(uwtCNLE);
            BuildEditPaneCNLE(LNode);
        }
    }

    // Show the information of the selected Node Tab of the 2º Level
    protected void Activate_uwtCNLE(Infragistics.WebUI.UltraWebTab.Tab ATabSelected)
    {
        ATabSelected.ContentPane.Controls.Add(advSNEofCNLE);

        if (FUltraWebNavigatorSelectedChildNodes != null)
        {
            SetAuthenticDocumentView(FUltraWebNavigatorSelectedChildNodes[uwtGNLE.Tabs.IndexOf(ATabSelected)], advSNEofCNLE);
        
            XmlNode LNode = FUltraWebNavigatorSelectedChildNodes[uwtGNLE.Tabs.IndexOf(ATabSelected)];

            ATabSelected.ContentPane.Controls.Add(uwtGNLE);

            BuildEditPaneGNLE(LNode);
        }
    }

    // Show the information of the selected Node Tab of the 3º Level
    protected void Activate_uwtGNLE(Infragistics.WebUI.UltraWebTab.Tab ATabSelected)
    {
        ATabSelected.ContentPane.Controls.Add(advSNEofGNLE);
        if (FUltraWebNavigatorSelectedGrandChildNodes != null)
            SetAuthenticDocumentView(FUltraWebNavigatorSelectedGrandChildNodes[uwtGNLE.Tabs.IndexOf(ATabSelected)], advSNEofGNLE);
    }

    // Show the information of the selected Node Tab of the 2º Level
    protected void uwtCNLE_TabClick(object sender, Infragistics.WebUI.UltraWebTab.WebTabEvent e)
    {
        Activate_uwtCNLE((Infragistics.WebUI.UltraWebTab.Tab)e.Tab);
    }

    // Show the information of the selected Node Tab of the 3º Level
    protected void uwtGNLE_TabClick(object sender, Infragistics.WebUI.UltraWebTab.WebTabEvent e)
    {
        Activate_uwtGNLE((Infragistics.WebUI.UltraWebTab.Tab)e.Tab);
    }

// ** END ** Procedures and Functions pertinent to the "Editor Pane"

// ** BEGIN ** Procedures and Functions pertinent to save the XML 
    //Update XML with changes of the user
    protected void SaveXMLChanges(string AXMLFilenamePath)
    {
        //Update XML with each modified Level
        UpdateXML(advSNE, AXMLFilenamePath);
        UpdateXML(advSNEofNLE, AXMLFilenamePath);
        UpdateXML(advSNEofCNLE, AXMLFilenamePath);
        UpdateXML(advSNEofGNLE, AXMLFilenamePath);
        //Update XML
        FXmlDocument.Save(AXMLFilenamePath);
        //Update each XML temporary with all changed
        FXmlDocument.Save(AppDomain.CurrentDomain.BaseDirectory + "/" + advSNE.ID + FXMLFilename);
        FXmlDocument.Save(AppDomain.CurrentDomain.BaseDirectory + "/" + advSNEofNLE.ID + FXMLFilename);
        FXmlDocument.Save(AppDomain.CurrentDomain.BaseDirectory + "/" + advSNEofCNLE.ID + FXMLFilename);
        FXmlDocument.Save(AppDomain.CurrentDomain.BaseDirectory + "/" + advSNEofGNLE.ID + FXMLFilename);
    }

    //Update the node attributes with the attributes of others node
    protected void UpdateAttributes(XmlNode ASourceNode, XmlNode AActualNode, XmlNode ATargetNode)
    {
        XmlAttributeCollection LXMLSourceAttributeCollection;
        LXMLSourceAttributeCollection = ASourceNode.Attributes;

        XmlAttributeCollection LXMLActualAttributeCollection;
        LXMLActualAttributeCollection = AActualNode.Attributes;

        XmlAttributeCollection LXMLTargetAttributeCollection;
        LXMLTargetAttributeCollection = ATargetNode.Attributes;


        if ((LXMLActualAttributeCollection != null) && (LXMLSourceAttributeCollection != null))
        {
            XmlAttribute LXMLActualAttribute = null;
            XmlAttribute LXMLTargetAttribute = null;
            foreach (XmlAttribute LXMLSourceAttribute in LXMLSourceAttributeCollection)
            {
                LXMLActualAttribute = LXMLActualAttributeCollection[LXMLSourceAttribute.Name];
                LXMLTargetAttribute = LXMLTargetAttributeCollection[LXMLSourceAttribute.Name];
                if (LXMLActualAttribute!=null)
                {
                    if (LXMLActualAttribute.Value != LXMLSourceAttribute.Value)
                    {
                        if (LXMLTargetAttribute != null)
                        {
                            LXMLTargetAttribute.Value = LXMLSourceAttribute.Value;
                        }
                    }
               }
            }
        }

        for (int LIndex = 0; LIndex < ASourceNode.ChildNodes.Count; LIndex++)
        {
            XmlNode LSourceChildNode = ASourceNode.ChildNodes[LIndex];
            XmlNode LActualChildNode = AActualNode.ChildNodes[LIndex];
            XmlNode LTargetChildNode = ATargetNode.ChildNodes[LIndex];
            if ((LSourceChildNode != null) && (LActualChildNode != null) && (LTargetChildNode != null))
            {
                UpdateAttributes(LSourceChildNode, LActualChildNode, LTargetChildNode);
            }
        }
    }

    //Update a XML with the AuthenticDocumentView data 
    protected void UpdateXML(Altova.Authentic.WebControls.AuthenticDocumentView AadvSource, string AXMLFilenamePath)
    {
        if (AadvSource != null)
        {
            XmlDocument LXmlDocumentNew = new XmlDocument();
            string[] Currentdirectory = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                LXmlDocumentNew.Load(FXMLDataSaveUrl + "//" + AadvSource.ID + FXMLFilename);
            }
            catch (Exception e)
            {
                return;  // The file can be empty
            }

            if ((LXmlDocumentNew != null) && (FXmlDocument.DocumentElement != null))
            {
                XmlDocument LXmlActualDocument = new XmlDocument();
                LXmlActualDocument.Load(AXMLFilenamePath);
                if (LXmlDocumentNew.InnerXml != LXmlActualDocument.InnerXml)
                {
                    UpdateAttributes(LXmlDocumentNew.DocumentElement, LXmlActualDocument.DocumentElement, FXmlDocument.DocumentElement);
                }
            }

        }
    }
// ** END ** Procedures and Functions pertinent to save the XML 
}
 