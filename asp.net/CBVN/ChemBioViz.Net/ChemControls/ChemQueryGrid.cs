using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

using System.Data;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinEditors;

namespace ChemControls
{
    public delegate void FinishedEventHandler(object sender, EventArgs e);
    public delegate void FinishedExportEventHandler(object sender, EventArgs e);
    public delegate void OnSchemaSetEventHandler(object sender, EventArgs e);


    public partial class ChemQueryGrid : ChemDataGrid
    {
        DataSet schema;     // external schema provided by consumer
        DataSet querySet;

        // display parameters
        bool showAllFields;         // true to show all fields, regardless of dataview "Hidden" property
        bool showDBColumnNames;     // true to show DB column names rather than aliases
        bool showTableNames;        // true to show table names as well as column/alias names
        bool showTableAliases;      // true to show table aliases instead of table names, in a friendlier format
        bool showVisibleCheckboxes; // true to let the user select whether a column will be shown or hidden in the results
        bool autoSizeColumns;       // true to auto-size columns

        public event FinishedEventHandler Finished;
        public event FinishedExportEventHandler FinishedExport;
        public event OnSchemaSetEventHandler OnSchemaSet;

        public ChemQueryGrid()
        {
            InitializeComponent();

            schema = null;
            querySet = null;

            // set display options to mimic original behavior
            showAllFields = true;
            showDBColumnNames = true;
            showTableNames = true;
            showTableAliases = false;
            showVisibleCheckboxes = true;
            autoSizeColumns = false;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(ChemQueryGrid_KeyDown);
        }

        public bool ShowAllFields
        {
            get { return showAllFields; }
            set { showAllFields = value; }
        }

        public bool ShowDBColumnNames
        {
            get { return showDBColumnNames; }
            set { showDBColumnNames = value; }
        }

        public bool ShowTableNames
        {
            get { return showTableNames; }
            set { showTableNames = value; }
        }

        public bool ShowTableAliases
        {
            get { return showTableAliases; }
            set { showTableAliases = value; }
        }

        public bool ShowVisibleCheckboxes
        {
            get { return showVisibleCheckboxes; }
            set { showVisibleCheckboxes = value; }
        }

        public bool AutoSizeColumns
        {
            get { return autoSizeColumns; }
            set { autoSizeColumns = value; }
        }

        void ChemQueryGrid_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.F5:
                    RunQuery(false);
                    break;
                case System.Windows.Forms.Keys.F6:
                    RunQuery(true);
                    break;
            }
        }
        
        /// <summary>
        /// public interface to start the query
        /// </summary>
        /// <param name="forExport"></param>
        public void RunQuery(bool forExport)
        {
            try
            {
                if (forExport)
                {
                    if (FinishedExport != null)
                    {
                        ActiveCell = null;
                        FinishedExport(this, new EventArgs());
                    }
                }
                else
                {
                    if (Finished != null)
                    {
                        ActiveCell = null;
                        Finished(this, new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while processing your query: " + ex.Message);
         
            }
        }

        CheckEditor CreateCheckEditor()
        {
            DefaultEditorOwnerSettings editorSettings = new DefaultEditorOwnerSettings();
            editorSettings.DataType = typeof(bool);
            CheckEditor editor = new CheckEditor(new DefaultEditorOwner(editorSettings));
            return editor;
        }

        DefaultEditorOwner CreateEditorSettings(string[] options)
        {
            ValueList valueList = new ValueList();
            foreach(string option in options)
                valueList.ValueListItems.Add(option, option);
            
            DefaultEditorOwnerSettings editorSettings = new DefaultEditorOwnerSettings();
            editorSettings.DataType = typeof(string);
            editorSettings.ValueList = valueList;

            return new DefaultEditorOwner(editorSettings);
        }

        OptionSetEditor OptionsSetEditor(string[] options)
        {
            return new OptionSetEditor(CreateEditorSettings(options));
        }

        EditorWithCombo ComboEditor(string[] options)
        {
            return new EditorWithCombo(CreateEditorSettings(options));
        }

        void AddCartridgeParameters(DataTable parameters, int id)
        {
            //Coverity Bug Fix CID 11799 -local CheckeEditor variable was unused,hence removed
            parameters.Rows.Add("Full Structure Search", "no", id, OptionsSetEditor(new string[] { "yes", "no" }));
            parameters.Rows.Add("Identity Search", "no", id, OptionsSetEditor(new string[] { "yes", "no" }));
            parameters.Rows.Add("Similarity Search", "no", id, OptionsSetEditor(new string[] { "yes", "no" }));
            parameters.Rows.Add("Similarity Threshold %", "80", id, null);
            parameters.Rows.Add("Doublebond stereochemistry", "yes", id, OptionsSetEditor(new string[] { "yes", "no" }));
            parameters.Rows.Add("Tetrahedral stereochemistry", "yes", id, ComboEditor(new string[] { "yes", "no", "same", "either", "any", "identity" }));
        }

        public void SetSchema(DataSet schemaInput)
        {

            schema = schemaInput;

            DataTable fields = new DataTable();

            fields.Columns.Add("Name", typeof(string));
            fields.Columns.Add("Show", typeof(bool));
            fields.Columns.Add("=", typeof(string));
            fields.Columns.Add("Value", typeof(object));
            fields.Columns.Add("id", typeof(int));                  // internally used to link to the additional propertied table
            fields.Columns.Add("mimetype", typeof(string));         // hidden used internally
            fields.Columns.Add("columnreference", typeof(object));  // hidden used internally

            DataTable parameters = new DataTable();

            parameters.Columns.Add("Name", typeof(string));
            parameters.Columns.Add("Value", typeof(object));
            parameters.Columns.Add("id", typeof(int));  // connect to fields table
            parameters.Columns.Add("editor", typeof(EmbeddableEditorBase));

            int id = 0;
            int tablecount = 0;
            string tablename = "";

            foreach (DataTable t in schema.Tables)
            {
                foreach (DataColumn c in t.Columns)
                {
                    if (showAllFields || (bool)c.ExtendedProperties["Visible"])
                    {
                        string name = "";
                        // set column names based on display options
                        if (showDBColumnNames)
                        {
                            name = c.ColumnName;
                        }
                        else
                        {
                            name = (string)c.ExtendedProperties["Alias"];
                            if (name == "")
                                name = c.ColumnName;
                        }
                        if (schema.Tables.Count > 1 && showTableNames)
                            name += "(" + t.TableName + ")";
                        if (tablecount > 0 && showTableAliases)
                        {
                            tablename = t.ExtendedProperties["Alias"].ToString();
                            if (tablename == "")
                                tablename = t.TableName;
                            name = tablename + " - " + name;
                        }

                    string mimetype = "";

                    object mimetypeObject = c.ExtendedProperties["mimetype"];
                    if (mimetypeObject != null)
                        mimetype = System.Convert.ToString(mimetypeObject);

                    fields.Rows.Add(name, true, "=", null, id, mimetype, c);

                    if (mimetype == "cdx" || mimetype == "cdxml" || mimetype == "cdxb64")
                        AddCartridgeParameters(parameters, id);

                    id++;
                    }
                }
                tablecount++;
            }

            querySet = new DataSet();

            DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;

//            DisplayLayout.Override.AllowAddNew = AllowAddNew.TemplateOnBottom;
            DisplayLayout.Override.RowSizing = RowSizing.Free;


            querySet.Tables.Add(fields);
            querySet.Tables.Add(parameters);
            querySet.Relations.Add(fields.Columns["id"], parameters.Columns["id"]);

            DataSource = querySet;        // that changes everything

            DisplayLayout.Bands[0].Columns["Name"].Width = 200;
            // wrap text in name column
            DisplayLayout.Bands[0].Columns["Name"].CellMultiLine = DefaultableBoolean.True;
            DisplayLayout.Bands[0].Columns["Value"].Width = 200;
            DisplayLayout.Bands[0].Columns["="].Editor = ComboEditor(new string[] { "=", "<", "<=", ">", ">=" });

            DisplayLayout.Bands[0].Columns["mimetype"].Hidden = true;
            DisplayLayout.Bands[0].Columns["id"].Hidden = true;
            DisplayLayout.Bands[0].Columns["columnreference"].Hidden = true;

            DisplayLayout.Bands[1].Columns["id"].Hidden = true;
            DisplayLayout.Bands[1].Columns["editor"].Hidden = true;
            if (!showVisibleCheckboxes)
                DisplayLayout.Bands[0].Columns["Show"].Hidden = true;

            int n = Rows.Count;

            foreach (UltraGridRow row in Rows)
            {
                if (row.Band == DisplayLayout.Bands[0])
                {
                    if (System.Convert.ToString(row.Cells["mimetype"].Value) != "")
                    {     // marked as ChemDraw cell
                        SetCellToChemDraw(row.Cells["Value"]);
                        row.Height *= 4;
                        UltraGridChildBand band = row.ChildBands[0];
                        foreach (UltraGridRow childRow in band.Rows)
                        {
                            EmbeddableEditorBase editor = childRow.Cells["editor"].Value as EmbeddableEditorBase;
                            if (editor != null)
                                childRow.Cells["Value"].Editor = editor;
                        }
                    }
                    else
                    {
                        row.ExpansionIndicator = ShowExpansionIndicator.Never;
                        // hack to expand row height when there is a table - column label (indicating it's probably a long label)
                        if (row.Cells[0].Text.Contains(" - "))
                            row.Height *= 2;
                    }
                }
            }
            // resize columns
            if (autoSizeColumns)
                DisplayLayout.AutoFitStyle = AutoFitStyle.ResizeAllColumns;
            // send event
            if (OnSchemaSet != null)
                OnSchemaSet(this, new EventArgs());
        }

        PropertyCollection GetSearchOptions(UltraGridRow row)
        {
            PropertyCollection coll = new PropertyCollection();

            foreach (UltraGridRow r in row.ChildBands[0].Rows)
                coll.Add(r.Cells[0].Value, r.Cells["Value"].Value);

            return coll;
        }

        public void SetQuery(Hashtable hs)
        {
            // set query from a hashtable of name-value pairs
            // value includes the condition
            foreach (UltraGridRow row in Rows)
            {
                if (row.Band == DisplayLayout.Bands[0])
                {
                    DataColumn c = row.Cells["columnreference"].Value as DataColumn;
                    if (c == null)
                        continue;
                    string cname = c.ColumnName;
                    string comp = "";
                    string test = "";
                    string val = "";
                    if (hs.ContainsKey(cname))
                    {
                        string ccompval = hs[cname].ToString();
                        // pull out comparator
                        if (c.ExtendedProperties["mimetype"] != null)   // meaning it's chemdraw
                        {
                            row.Cells["Value"].Value = "";
                            Application.DoEvents();
                            row.Cells["Value"].Value = ccompval;
                            Application.DoEvents();
                            //v = this.chemDrawForm.Convert("chemical/smiles", v);
                            //c.ExtendedProperties["options"] = GetSearchOptions(row);

                        }
                        else
                        {

                            if (ccompval != "")
                            {
                                // well, I'm not proud of this
                                test = ">=";
                                if (ccompval.Contains(test))
                                {
                                    comp = test;
                                    val = ccompval.Replace(">=", "").Trim();
                                }
                                test = "<=";
                                if (ccompval.Contains(test))
                                {
                                    comp = test;
                                    val = ccompval.Replace(">=", "").Trim();
                                }
                                test = ">";
                                if (ccompval.Contains(test))
                                {
                                    comp = test;
                                    val = ccompval.Replace(">=", "").Trim();
                                }
                                test = "<";
                                if (ccompval.Contains(test))
                                {
                                    comp = test;
                                    val = ccompval.Replace(">=", "").Trim();
                                }
                                test = "=";
                                if (ccompval.Contains(test))
                                {
                                    comp = test;
                                    val = ccompval.Replace(">=", "").Trim();
                                }
                                if (comp == "")
                                {
                                    comp = "=";
                                    val = ccompval;
                                }

                                row.Cells["="].Value = comp;
                                row.Cells["Value"].Value = ccompval;
                            }
                        }
                    }
                }
            }
        }

        public DataSet GetResults()
        {
            foreach (UltraGridRow row in Rows)
            {
                DataColumn c = row.Cells["columnreference"].Value as DataColumn;
                if (c == null)
                    continue;

                object v = row.Cells["Value"].Value;

                // clear out query items if the value is null
                if (v == null || v is DBNull)
                {
                    c.ExtendedProperties["Condition"] = null;
                    c.ExtendedProperties["Value"] = null;
                    continue;
                }

                if (c.ExtendedProperties["mimetype"] != null)   // meaning it's chemdraw
                {
                    v = this.ChemDrawForm.Convert("chemical/smiles", v);
                    c.ExtendedProperties["options"] = GetSearchOptions(row);
                }

                c.ExtendedProperties["Condition"] = row.Cells["="].Value;
                c.ExtendedProperties["Value"] = v;
            }

            return schema;
        }
           
    }
}
