using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;
using System.Xml;
using System.Reflection;
using CambridgeSoft.COE.Framework.Controls.COEDataMapper;
using System.Data;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.ServerControls.FormGenerator;
using CambridgeSoft.COE.Framework.COELoggingService;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator
{
    /// <summary>
    /// <para>
    /// This class implements a GridView control that may be used inside a <see cref="COEFormGenerator"/>.
    /// </para>
    /// <para>
    /// The COEGridView class accepts every GridView property to be set, but as ICOEGenerable control it also provides
    /// GetData(), PutData(), DefaultValue and LoadFromXml() Methods.
    /// </para>
    /// <para>
    /// <b>Input XML</b>
    /// </para>
    /// <para>
    ///     <list type="bullet">
    ///         <item>defaultValue: What is the default caption of the grid?</item>
    ///         <item>bindingExpression: What is the binding Attribute, relative to the datasource, of the formgenerator.</item>
    ///         <item>configInfo: Additional attributes like CSSClass, Style, Columns, column's datafield and headertext...</item>
    ///     </list>
    /// </para>
    /// <para>
    /// <b>Example:</b>
    /// </para>
    /// <b>With XML:</b>
    /// <code lang="XML">
    ///   &lt;formElement&gt;
    ///     &lt;bindingExpression&gt;PercentageList&lt;/bindingExpression&gt;
    ///     &lt;dataSource&gt;PercentageList&lt;/dataSource&gt;
    ///     &lt;configInfo&gt;
    ///       &lt;fieldConfig&gt;
    ///         &lt;tables&gt;
    ///             &lt;table name="Table_210"&gt;
    ///                 &lt;CSSClass&gt;myTableClass&lt;/CSSClass&gt;
    ///                 &lt;headerStyle&gt;color: white; background-color: rgb(0, 153, 255); font-weight: bold;&lt;/headerStyle&gt;
    ///                 &lt;Columns&gt;
    ///                     &lt;Column&gt;
    ///                         &lt;width&gt;5%&lt;/width&gt;
    ///                         &lt;formElement name="ID"&gt;
    ///                             &lt;configInfo&gt;
    ///                                 &lt;fieldConfig&gt;
    ///                                     &lt;CSSClass&gt;COELabel&lt;/CSSClass&gt;
    ///                                 &lt;/fieldConfig&gt;
    ///                             &lt;/configInfo&gt;
    ///                         &lt;/formElement&gt;
    ///                     &lt;/Column&gt;
    ///                     &lt;Column&gt;
    ///                         &lt;height&gt;153px&lt;/height&gt;
    ///                         &lt;width&gt;20%&lt;/width&gt;
    ///                         &lt;formElement name="Structure"&gt;
    ///                             &lt;Id&gt;StructureColumn&lt;/Id&gt;
    ///                             &lt;displayInfo&gt;
    ///                                 &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEChemDrawEmbedReadOnly&lt;/type&gt;
    ///                             &lt;/displayInfo&gt;
    ///                             &lt;configInfo&gt;
    ///                                 &lt;fieldConfig&gt;
    ///                                     &lt;Height&gt;140px&lt;/Height&gt;
    ///                                     &lt;Width&gt;140px&lt;/Width&gt;
    ///                                 &lt;/fieldConfig&gt;
    ///                             &lt;/configInfo&gt;
    ///                         &lt;/formElement&gt;
    ///                     &lt;/Column&gt;
    ///                     &lt;Column&gt;
    ///                         &lt;width&gt;30%&lt;/width&gt;
    ///                         &lt;formElement name="Resources"&gt;
    ///                             &lt;configInfo&gt;
    ///                                 &lt;fieldConfig&gt;
    ///                                     &lt;CSSClass&gt;COELabel&lt;/CSSClass&gt;
    ///                                 &lt;/fieldConfig&gt;
    ///                             &lt;/configInfo&gt;
    ///                         &lt;/formElement&gt;
    ///                     &lt;/Column&gt;
    ///                     &lt;Column&gt;
    ///                         &lt;width&gt;20%&lt;/width&gt;
    ///                         &lt;formElement name="Mol Wt"&gt;
    ///                             &lt;configInfo&gt;
    ///                                 &lt;fieldConfig&gt;
    ///                                     &lt;CSSClass&gt;COELabel&lt;/CSSClass&gt;
    ///                                 &lt;/fieldConfig&gt;
    ///                             &lt;/configInfo&gt;
    ///                         &lt;/formElement&gt;
    ///                     &lt;/Column&gt;
    ///                     &lt;Column&gt;
    ///                         &lt;width&gt;20%&lt;/width&gt;
    ///                         &lt;formElement name="Mol Formula"&gt;
    ///                             &lt;configInfo&gt;
    ///                                 &lt;fieldConfig&gt;
    ///                                     &lt;CSSClass&gt;COELabel&lt;/CSSClass&gt;
    ///                                 &lt;/fieldConfig&gt;
    ///                             &lt;/configInfo&gt;
    ///                         &lt;/formElement&gt;
    ///                     &lt;/Column&gt;
    ///                     &lt;Column&gt;
    ///                         &lt;width&gt;5%&lt;/width&gt;
    ///                         &lt;formElement name="ID"&gt;
    ///                             &lt;defaultValue&gt;View Details&lt;/defaultValue&gt;
    ///                             &lt;Id&gt;DetailsLink&lt;/Id&gt;
    ///                             &lt;displayInfo&gt;
    ///                                 &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELinkButton&lt;/type&gt;
    ///                             &lt;/displayInfo&gt;
    ///                             &lt;configInfo&gt;
    ///                                 &lt;fieldConfig&gt;
    ///                                     &lt;CSSClass&gt;LinkButton&lt;/CSSClass&gt;
    ///                                 &lt;/fieldConfig&gt;
    ///                             &lt;/configInfo&gt;
    ///                         &lt;/formElement&gt;
    ///                     &lt;/Column&gt;
    ///                 &lt;/Columns&gt;
    ///             &lt;/table&gt;
    ///         &lt;/tables>
    ///       &lt;/fieldConfig&gt;
    ///     &lt;/configInfo&gt;
    ///     &lt;displayInfo&gt;
    ///       &lt;position&gt;relative&lt;/position&gt;
    ///       &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEGridView&lt;/type&gt;
    ///     &lt;/displayInfo&gt;
    ///   &lt;/formElement&gt;
    ///   &lt;/formElements&gt;
    /// </code>
    /// <para>
    /// <b>Notes:</b>
    /// </para>
    /// <para>
    /// In this implementation "Default Value" reffers to the "Caption", GetData and PutData Methods reffer to the whole gridview's values.
    /// </para>
    /// </summary>
    [ToolboxData("<{0}:COEGridView runat=server></{0}:COEGridView>")]
    public class COEGridView : GridView, ICOEGenerableControl, ICOEHitMarker, ICOEDesignable, ICOEFullDatasource, ICOEGrid
    {
        #region Variables
        private bool _readOnly = false;
        private XmlNodeList _xmlTablesDef;
        XmlNamespaceManager _manager = null;
        private object _fullDatasource;
        #endregion
        [NonSerialized]
        static COELog _coeLog = COELog.GetSingleton("COEFormGenerator");

        #region Properties
        public string ColumnIDValue {
            get {
                return string.Empty;
            }
            set { }
        }

        public string ColumnIDBindingExpression {
            get { return string.Empty; }
            set { }
        }
        #endregion

        #region ICOEHitMarker Members
        public event MarkingHitHandler MarkingHit;
        #endregion

        #region Contructors

        public COEGridView()
        {
            this.AutoGenerateDeleteButton = false;
            this.AutoGenerateEditButton = false;
            this.AutoGenerateSelectButton = false;
            this.AutoGenerateColumns = true;
            this.AllowPaging = false;
        }
        #endregion

        #region ICOEGenerableControl Members
        /// <summary>
        /// <para>Builds a DataTable with the data of the gridview.</para>
        /// </summary>
        /// <returns>A DataTable with the gridview's data.</returns>
        public object GetData()
        {
            DataTable result = new DataTable();
            if (_xmlTablesDef != null && _xmlTablesDef.Count > 0) //Coverity Fix CID 13139 ASV
            {
                XmlNodeList managerNodes = _xmlTablesDef[0].SelectNodes("./COE:Columns/COE:Column", _manager); //Coverity Fix CID 13139 ASV
                if (managerNodes != null)  //Coverity Fix CID 13139 ASV
                {
                    foreach (XmlNode column in managerNodes)
            {
                string columnName, columnBindingExpression;

                GetColumnInfo(column, out columnName, out columnBindingExpression);

                result.Columns.Add(columnBindingExpression);
            }

            foreach (GridViewRow currentRow in this.Rows)
            {
                DataRow currentDataRow = result.NewRow();
                result.Rows.Add(currentDataRow);

                foreach (DataControlFieldCell currentCell in currentRow.Cells)
                {
                    System.Collections.Specialized.OrderedDictionary dictionary = new System.Collections.Specialized.OrderedDictionary();


                    ((FormElementField)currentCell.ContainingField).ExtractValuesFromCell(dictionary, currentCell, DataControlRowState.Normal, false);

                    IEnumerator enumerator = dictionary.Keys.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        currentDataRow[enumerator.Current.ToString()] = dictionary[enumerator.Current.ToString()];
                    }
                }
            }
                }
            }
                return result;
        }
        /// <summary>
        /// </para>Populates the gridview from an IEnumerable object.</para>
        /// </summary>
        /// <param name="data">An IEnumerable object.</param>
        public void PutData(object data)
        {
            this.DataSource = data;
            //Coverity Fix CID 13139 ASV
            if (_xmlTablesDef != null && _xmlTablesDef.Count > 0 && _xmlTablesDef[0].Attributes["name"] != null && !string.IsNullOrEmpty(_xmlTablesDef[0].Attributes["name"].Value))
                this.DataMember = _xmlTablesDef[0].Attributes["name"].Value;
            this.DataBind();
        }

        /// <summary>Loads its specific configuration from an xml in the form:
        /// <code lang="Xml">
        ///   &lt;fieldConfig&gt;
        ///     &lt;tables&gt;
        ///         &lt;table name="Table_210"&gt;
        ///             &lt;CSSClass&gt;myTableClass&lt;/CSSClass&gt;
        ///             &lt;headerStyle&gt;color: white; background-color: rgb(0, 153, 255); font-weight: bold;&lt;/headerStyle&gt;
        ///             &lt;Columns&gt;
        ///                 &lt;Column&gt;
        ///                     &lt;width&gt;5%&lt;/width&gt;
        ///                     &lt;formElement name="ID"&gt;
        ///                         &lt;configInfo&gt;
        ///                             &lt;fieldConfig&gt;
        ///                                 &lt;CSSClass&gt;COELabel&lt;/CSSClass&gt;
        ///                             &lt;/fieldConfig&gt;
        ///                         &lt;/configInfo&gt;
        ///                     &lt;/formElement&gt;
        ///                 &lt;/Column&gt;
        ///                 &lt;Column&gt;
        ///                     &lt;height&gt;153px&lt;/height&gt;
        ///                     &lt;width&gt;20%&lt;/width&gt;
        ///                     &lt;formElement name="Structure"&gt;
        ///                         &lt;Id&gt;StructureColumn&lt;/Id&gt;
        ///                         &lt;displayInfo&gt;
        ///                             &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COEChemDrawEmbedReadOnly&lt;/type&gt;
        ///                         &lt;/displayInfo&gt;
        ///                         &lt;configInfo&gt;
        ///                             &lt;fieldConfig&gt;
        ///                                 &lt;Height&gt;140px&lt;/Height&gt;
        ///                                 &lt;Width&gt;140px&lt;/Width&gt;
        ///                             &lt;/fieldConfig&gt;
        ///                         &lt;/configInfo&gt;
        ///                     &lt;/formElement&gt;
        ///                 &lt;/Column&gt;
        ///                 &lt;Column&gt;
        ///                     &lt;width&gt;30%&lt;/width&gt;
        ///                     &lt;formElement name="Resources"&gt;
        ///                         &lt;configInfo&gt;
        ///                             &lt;fieldConfig&gt;
        ///                                 &lt;CSSClass&gt;COELabel&lt;/CSSClass&gt;
        ///                             &lt;/fieldConfig&gt;
        ///                         &lt;/configInfo&gt;
        ///                     &lt;/formElement&gt;
        ///                 &lt;/Column&gt;
        ///                 &lt;Column&gt;
        ///                     &lt;width&gt;20%&lt;/width&gt;
        ///                     &lt;formElement name="Mol Wt"&gt;
        ///                         &lt;configInfo&gt;
        ///                             &lt;fieldConfig&gt;
        ///                                 &lt;CSSClass&gt;COELabel&lt;/CSSClass&gt;
        ///                             &lt;/fieldConfig&gt;
        ///                         &lt;/configInfo&gt;
        ///                     &lt;/formElement&gt;
        ///                 &lt;/Column&gt;
        ///                 &lt;Column&gt;
        ///                     &lt;width&gt;20%&lt;/width&gt;
        ///                     &lt;formElement name="Mol Formula"&gt;
        ///                         &lt;configInfo&gt;
        ///                             &lt;fieldConfig&gt;
        ///                                 &lt;CSSClass&gt;COELabel&lt;/CSSClass&gt;
        ///                             &lt;/fieldConfig&gt;
        ///                         &lt;/configInfo&gt;
        ///                     &lt;/formElement&gt;
        ///                 &lt;/Column&gt;
        ///                 &lt;Column&gt;
        ///                     &lt;width&gt;5%&lt;/width&gt;
        ///                     &lt;formElement name="ID"&gt;
        ///                         &lt;defaultValue&gt;View Details&lt;/defaultValue&gt;
        ///                         &lt;Id&gt;DetailsLink&lt;/Id&gt;
        ///                         &lt;displayInfo&gt;
        ///                             &lt;type&gt;CambridgeSoft.COE.Framework.Controls.COEFormGenerator.COELinkButton&lt;/type&gt;
        ///                         &lt;/displayInfo&gt;
        ///                         &lt;configInfo&gt;
        ///                             &lt;fieldConfig&gt;
        ///                                 &lt;CSSClass&gt;LinkButton&lt;/CSSClass&gt;
        ///                             &lt;/fieldConfig&gt;
        ///                         &lt;/configInfo&gt;
        ///                     &lt;/formElement&gt;
        ///                 &lt;/Column&gt;
        ///             &lt;/Columns&gt;
        ///         &lt;/table&gt;
        ///     &lt;/tables>
        ///   &lt;/fieldConfig&gt;
        /// </code>
        /// </summary>
        /// <param name="xmlDataAsString">The configInfo xml snippet</param>
        public void LoadFromXml(string xmlDataAsString)
        {

            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(xmlDataAsString);

            _manager = new XmlNamespaceManager(xmlData.NameTable);
            _manager.AddNamespace("COE", xmlData.DocumentElement.NamespaceURI);
            _xmlTablesDef = xmlData.SelectNodes("//COE:table", _manager);

            if (_xmlTablesDef != null && _xmlTablesDef.Count > 0) //Coverity Fix CID 13139 ASV
            {
                //Try to avoid the use of Style, Width and Height; instead of it, define all in a CSSClass.
                XmlNode style = _xmlTablesDef[0].SelectSingleNode("./COE:Style", _manager);
                if (style != null && style.InnerText.Length > 0)
                {
                    string[] styles = style.InnerText.Split(new char[1] { ';' });
                    for (int i = 0; i < styles.Length; i++)
                    {
                        if (styles[i].Length > 0)
                        {
                            string[] styleDef = styles[i].Split(new char[1] { ':' });
                            string styleId = styleDef[0].Trim();
                            string styleValue = styleDef[1].Trim();
                            this.Style.Add(styleId, styleValue);
                        }
                    }
                }

                XmlNode headerStyle = _xmlTablesDef[0].SelectSingleNode("./COE:headerStyle", _manager);
                if (headerStyle != null && headerStyle.InnerText.Length > 0)
                {
                    string[] styles = headerStyle.InnerText.Split(new char[1] { ';' });
                    styles[0] = styles[0].Trim();
                    styles[1] = styles[1].Trim();
                    for (int i = 0; i < styles.Length; i++)
                    {
                        if (styles[i].Length > 0)
                        {
                            string[] styleDef = styles[i].Split(new char[1] { ':' });
                            switch (styleDef[0].Trim())
                            {
                                case "background-color":
                                    System.Drawing.Color color = new System.Drawing.Color();
                                    if (styles[1].ToLower().Contains("rgb("))
                                    {
                                        styles[1] = styles[1].Remove(0, styles[1].IndexOf("(") + 1);
                                        styles[1] = styles[1].Remove(styles[1].Length - 1);
                                        string[] rgb = styles[1].Split(',');
                                        this.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(int.Parse(rgb[0].Trim()), int.Parse(rgb[1].Trim()), int.Parse(rgb[2].Trim()));
                                    }
                                    else
                                    {
                                        color = System.Drawing.Color.FromName(styleDef[1]);
                                        this.HeaderStyle.BackColor = color;
                                    }
                                    break;
                                case "color":
                                    color = System.Drawing.Color.FromName(styleDef[1]);
                                    this.HeaderStyle.ForeColor = color;
                                    break;
                                case "font-weight":
                                    this.HeaderStyle.Font.Bold = styleDef[1].ToLower().Contains("bold");
                                    break;
                                case "font-family":
                                    this.HeaderStyle.Font.Name = styleDef[1];
                                    break;
                                case "font-size":
                                    this.HeaderStyle.Font.Size = new FontUnit(new Unit(styleDef[1]));
                                    break;
                                case "text-align":
                                    switch (styleDef[1].Trim().ToLower())
                                    {
                                        case "left":
                                            this.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                                            break;
                                        case "right":
                                            this.HeaderStyle.HorizontalAlign = HorizontalAlign.Right;
                                            break;
                                        case "center":
                                            this.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
                XmlNode width = _xmlTablesDef[0].SelectSingleNode("./COE:Width", _manager);
                if (width != null && width.InnerText.Length > 0)
                {
                    this.Width = new Unit(width.InnerText);
                }

                XmlNode height = _xmlTablesDef[0].SelectSingleNode("./COE:Height", _manager);
                if (height != null && height.InnerText.Length > 0)
                {
                    this.Height = new Unit(height.InnerText);
                }

                XmlNode cssClass = _xmlTablesDef[0].SelectSingleNode("./COE:CSSClass", _manager);
                if (cssClass != null && cssClass.InnerText.Length > 0)
                    this.CssClass = cssClass.InnerText;

                XmlNode readOnly = _xmlTablesDef[0].SelectSingleNode("./COE:ReadOnly", _manager);
                if (readOnly != null && readOnly.InnerText.Length > 0)
                    bool.TryParse(readOnly.InnerText, out _readOnly);
            }
        }

        /// <summary>
        /// <para>Sets or gets the default caption for the GridView.</para>
        /// </summary>
        public string DefaultValue
        {
            get
            {
                return this.Caption;
            }
            set
            {
                this.Caption = value;
            }
        }

        #endregion

        #region Life Cycle Events
        //protected override GridViewRow CreateRow(int rowIndex, int dataSourceIndex, DataControlRowType rowType, DataControlRowState rowState)
        //{
        //    GridViewRow retVal = null;
        //    if (rowType == DataControlRowType.EmptyDataRow || rowType == DataControlRowType.DataRow)
        //    {
        //        if (_readOnly)
        //            rowState = DataControlRowState.Normal;
        //        else
        //            rowState = DataControlRowState.Edit;

        //        retVal = base.CreateRow(rowIndex, dataSourceIndex, rowType, rowState);

        //        if (_xmlTablesDef[0].SelectSingleNode("./COE:itemsRowStyle", _manager) != null && _xmlTablesDef[0].SelectSingleNode("./COE:itemsRowStyle", _manager).InnerXml.Length > 0)
        //            this.ApplyRowStyles(_xmlTablesDef[0].SelectSingleNode("./COE:itemsRowStyle", _manager), retVal);
        //    }
        //    else if (rowType == DataControlRowType.Header)
        //    {
        //        retVal = base.CreateRow(rowIndex, dataSourceIndex, rowType, rowState);
        //        if (_xmlTablesDef[0].SelectSingleNode("./COE:headerRowStyle", _manager) != null && _xmlTablesDef[0].SelectSingleNode("./COE:headerRowStyle", _manager).InnerXml.Length > 0)
        //            this.ApplyRowStyles(_xmlTablesDef[0].SelectSingleNode("./COE:headerRowStyle", _manager), retVal);
        //    }

        //    return retVal;
        //}

        protected override GridViewRow CreateRow(int rowIndex, int dataSourceIndex, DataControlRowType rowType, DataControlRowState rowState)
        {
            if (rowType == DataControlRowType.EmptyDataRow || rowType == DataControlRowType.DataRow)
            {
                if (_readOnly)
                    rowState = DataControlRowState.Normal;
                else
                    rowState = DataControlRowState.Edit;

            }
            return base.CreateRow(rowIndex, dataSourceIndex, rowType, rowState);
        }

        protected override Style CreateControlStyle()
        {
            return base.CreateControlStyle();
        }

        protected override void Render(HtmlTextWriter writer)
        {//Coverity Fix CID 13139 ASV
            XmlNode managerNode = null; 
            if (_xmlTablesDef != null && _xmlTablesDef.Count > 0)
                managerNode = _xmlTablesDef[0].SelectSingleNode("./COE:headerRowStyle", _manager);

            if (managerNode != null && managerNode.InnerXml.Length > 0)
                this.ApplyRowStyles(managerNode, this.HeaderRow);

            managerNode = _xmlTablesDef[0].SelectSingleNode("./COE:itemsRowStyle", _manager);
            if (managerNode != null && managerNode.InnerXml.Length > 0)
            foreach (GridViewRow currentRow in this.Rows)
            {
                    if (currentRow.RowType == DataControlRowType.EmptyDataRow || currentRow.RowType == DataControlRowType.DataRow)
                        this.ApplyRowStyles(managerNode, currentRow);
            }

            base.Render(writer);
        }

        private void GetColumnInfo(XmlNode xmlNode, out string columnName, out string columnBindingExpression)
        {
            columnName = string.Empty;
            columnBindingExpression = string.Empty;
            //Coverity Fix CID 13139 ASV
            if (xmlNode != null)
            { 
                XmlNode managerNode = null;
                managerNode = xmlNode.SelectSingleNode("./COE:headerText", _manager);

                if (managerNode != null && managerNode.InnerText != string.Empty)
                    columnName = managerNode.InnerText;
                else if (xmlNode.Attributes["name"] != null && !string.IsNullOrEmpty(xmlNode.Attributes["name"].Value))
                    columnName = xmlNode.Attributes["name"].Value;

               managerNode = xmlNode.SelectSingleNode("./COE:formElement", _manager);
               if (managerNode != null && managerNode.OuterXml != string.Empty)
                {
                    FormGroup.FormElement formElement = FormGroup.FormElement.GetFormElement(managerNode.OuterXml);

                    if (!string.IsNullOrEmpty(formElement.Name)) //Name refers to a datatable column, so modify the binding expression.
                        columnBindingExpression = string.Format("this['{0}']", formElement.Name);
                    else if (!string.IsNullOrEmpty(formElement.BindingExpression))
                        columnBindingExpression = formElement.BindingExpression;
                }

                if (string.IsNullOrEmpty(columnName))
                    columnName = columnBindingExpression;
            }
        }
        protected override ICollection CreateColumns(PagedDataSource dataSource, bool useDataSource)
        {
            ArrayList columns = new ArrayList(); 
            FormElementField field;
            //Coverity Fix CID 13139 ASV
            XmlNodeList managerNodeList = null;
           
            if (_xmlTablesDef != null && _xmlTablesDef.Count >0)
                managerNodeList = _xmlTablesDef[0].SelectNodes("./COE:Columns/COE:Column", _manager);

            if (managerNodeList != null && managerNodeList.Count > 0)
            {
                foreach (XmlNode column in managerNodeList)
            {
                string columnName, columnBindingExpression;

                GetColumnInfo(column, out columnName, out columnBindingExpression);
                    XmlNode managerNode = column.SelectSingleNode("./COE:formElement", _manager);
                    if (managerNode != null)
                    {
                if(this.FullDatasource != null)
                            field = new FormElementField(managerNode, columnName, columnBindingExpression, this.FullDatasource);
                else
                            field = new FormElementField(managerNode, columnName, columnBindingExpression);
                
                XmlNode widthNode = column.SelectSingleNode("./COE:width", _manager);
                if (widthNode != null && !string.IsNullOrEmpty(widthNode.InnerText))
                {
                    field.ItemStyle.Width = new Unit(widthNode.InnerText);
                }

                XmlNode heightNode = column.SelectSingleNode("./COE:height", _manager);
                if (heightNode != null && !string.IsNullOrEmpty(heightNode.InnerText))
                {
                    field.ItemStyle.Height = new Unit(heightNode.InnerText);
                }

                if (column.Attributes["hidden"] != null)//Check visibility of the column (just if the attribute exists)
                {
                    this.SetColumnVisibility(ref field, column.Attributes["hidden"]);
                }

                columns.Add(field);

                field.MarkingHit += new MarkingHitHandler(field_MarkingHit);
            }
                }
            }
            return columns;
        }
        
        /// <summary>
        /// Sets the column visibility based on an xml attribute (hidden)
        /// </summary>
        /// <param name="col">Column to change visibility</param>
        /// <param name="value">Value to set</param>
        private void SetColumnVisibility(ref FormElementField field, XmlAttribute value)
        {
            bool hidden = false;
            if (value != null && !string.IsNullOrEmpty(value.Value) && field != null)
            {
                if (bool.TryParse(value.Value, out hidden))
                    field.Visible = (!hidden);
            }
        }

        #endregion

        #region ICOEGrid Members

        public void SetColumnVisibility(string key, bool visibility)
        {
            foreach (DataControlField field in this.Columns)
            {
                if (field is FormElementField)
                {
                    if (((FormElementField)field).ID == key)
                    {
                        field.Visible = visibility;
                    }
                }
            }
        }

        #endregion

        #region ICOEDesignable Members

        public XmlNode GetConfigInfo() {
            XmlDocument xmlData = new XmlDocument();
            string xmlns = "COE.FormGroup";
            string xmlprefix = "COE";

            xmlData.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "fieldConfig", xmlns));

            XmlNode tablesNode = xmlData.FirstChild.AppendChild(xmlData.CreateNode(XmlNodeType.Element, xmlprefix, "tables", xmlns));
            
            if(_xmlTablesDef.Count > 0) {
                tablesNode.AppendChild(xmlData.ImportNode(_xmlTablesDef[0], true));
            }

            return xmlData.FirstChild;
        }

        #endregion

        #region Misc Methods

        private void ApplyRowStyles(XmlNode rowStyle, GridViewRow row)
        {
            if (rowStyle != null && rowStyle.InnerText.Length > 0 && row != null)
            {
                string[] styles = rowStyle.InnerText.Split(new char[1] { ';' });
                styles[0] = styles[0].Trim();
                styles[1] = styles[1].Trim();
                for (int j = 0; j < styles.Length; j++)
                {
                    if (styles[j].Length > 0)
                    {
                        string[] styleDef = styles[j].Split(new char[1] { ':' });
                        switch (styleDef[0].Trim())
                        {
                            case "height":
                                row.Height = new Unit(styleDef[1]);
                                break;
                        }
                    }
                }
            }
        }

        private int GetFormGeneratorPageIndex()
        {
            Control currentParentControl = this.Parent;

            while (currentParentControl.GetType() != typeof(COEFormGenerator))
            {
                currentParentControl = currentParentControl.Parent;
            }

            return ((COEFormGenerator)currentParentControl).PageIndex;
        }

        void field_MarkingHit(object sender, MarkHitEventArgs eventArgs) {
            if(MarkingHit != null) {
                MarkingHit(sender, eventArgs);
            }
        }

        #endregion

        #region ICOEFullDatasource Members

        public object FullDatasource
        {
            set { _fullDatasource = value; }
            get { return _fullDatasource; }
        }

        #endregion

		#region Field classes

        private class FormElementField : BoundField
        {
            XmlNode _formElementDefinition = null;
            string _columnName = string.Empty;
            object _fullDataSource = null;
            string id = string.Empty;

            public string ID
            {
                get
                {
                    if (_formElementDefinition.SelectSingleNode("./Id") != null)
                    {
                        id = _formElementDefinition.SelectSingleNode("./Id").Value;
                    }
                    return id;
                }
            }

            public event MarkingHitHandler MarkingHit;

            public FormElementField(XmlNode formElementDefinition, string columnName, string columnBindingExpression)
            {
                _formElementDefinition = formElementDefinition;
                DataField = columnBindingExpression;
                _columnName = columnName;
            }

            public FormElementField(XmlNode formElementDefinition, string columnName, string columnBindingExpression, object datasource)
            {
                _formElementDefinition = formElementDefinition;
                DataField = columnBindingExpression;
                _columnName = columnName;
                _fullDataSource = datasource;
            }

            public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
            {
                string errorMsg = string.Empty;
                string methodSignature = MethodBase.GetCurrentMethod().DeclaringType.Name + "->" + MethodBase.GetCurrentMethod().Name.ToUpper() + " ";
                _coeLog.LogStart(methodSignature, 1, System.Diagnostics.SourceLevels.All);
                ICOEGenerableControl formElement = COEFormGenerator.GetCOEGenerableControl(_formElementDefinition.OuterXml, out errorMsg);

                if (cellType == DataControlCellType.DataCell)
                {
                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        cell.Controls.Add((Control)formElement);
                        if (formElement is ICOEDisplayModeChanger)
                        {
                            ((ICOEDisplayModeChanger)formElement).CurrentIndex = rowIndex;
                        }
                        if(formElement is ICOEHitMarker) {
                            ((ICOEHitMarker) formElement).MarkingHit += new MarkingHitHandler(FormElementField_MarkingHit);
                        }
                    }
                    else
                    {
                        Label errorLabel = new Label();
                        errorLabel.Text = errorMsg;
                        cell.Controls.Add(errorLabel);
                    }
                }
                else if (cellType == DataControlCellType.Header)
                {
                    Label headerLabel = new Label();
                    headerLabel.Text = _columnName;
                    cell.Controls.Add(headerLabel);
                }
                else
                {
                    //TODO: Do footer stuff.
                }

                if (formElement != null && this.Visible)
                    ((Control)formElement).DataBinding += new EventHandler(formElement_DataBinding);
                _coeLog.LogEnd(methodSignature);
            }

            void FormElementField_MarkingHit(object sender, MarkHitEventArgs eventArgs) {
                if(MarkingHit != null)
                    MarkingHit(sender, eventArgs);
            }

            void formElement_DataBinding(object sender, EventArgs e)
            {

                object container = null;
                if (sender is ICOEGenerableControl)
                {
                    ICOEGenerableControl formElement = sender as ICOEGenerableControl;
                    object dataItem = DataBinder.GetDataItem(((Control)formElement).NamingContainer);
                    object dataItemValue = "Error retrieving " + DataField + " value";
                    object columnIDValue = null;
                    try
                    {
                        COEDataBinder dataBinder = new COEDataBinder(dataItem);
                        //dataItemValue = DataBinder.GetPropertyValue(dataItem, this.DataField, null);
                        dataItemValue = dataBinder.RetrieveProperty(this.DataField);

                        if(formElement is ICOEHitMarker) {
                            ((ICOEHitMarker) formElement).ColumnIDValue = dataBinder.RetrieveProperty(((ICOEHitMarker) formElement).ColumnIDBindingExpression).ToString();
                        }
                        //Pass full datasource to inner controls
                        if (formElement is ICOEFullDatasource && _fullDataSource != null)
                            ((ICOEFullDatasource)formElement).FullDatasource = _fullDataSource;
                    }
                    catch (Exception) { }
                    formElement.PutData(dataItemValue);
                }
            }

            public override void ExtractValuesFromCell(System.Collections.Specialized.IOrderedDictionary dictionary, DataControlFieldCell cell, DataControlRowState rowState, bool includeReadOnly)
            {
                base.ExtractValuesFromCell(dictionary, cell, rowState, includeReadOnly);
                object value = null;

                if (cell.Controls.Count > 0)
                {
                    Control control = cell.Controls[0];

                    if (control == null)
                        throw new InvalidOperationException("The control cannot be extracted");

                    value = ((ICOEGenerableControl)control).GetData();
                }

                if (dictionary.Contains(this.DataField))
                    dictionary[this.DataField] = value;
                else
                    dictionary.Add(this.DataField, value);
            }
        }
        #endregion

        
    }
}
