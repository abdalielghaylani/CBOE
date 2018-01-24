using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.Framework.Controls.COEFormGenerator {
    /// <summary>
    /// <para>In this namespaces there are several classes intended for Form Generation. The following controls are already implemented:
    /// </para>
    /// <list type="bullet">
    ///     <item>COECheckBox: A checkbox for the FormGenerator.</item>
    ///     <item>COECheckBoxReadOnly: A read only checkbox for the FormGenerator.</item>
    ///     <item>COEChemDraw: A ChemDrawEmbed for the FormGenerator.</item>
    ///     <item>COEChemDrawEmbedReadOnly: A read only ChemDrawEmbed for the FormGenerator.</item>
    ///     <item>COEDatePicker: An infragistics' DatePicker for the FormGenerator.</item>
    ///     <item>COEDatePickerReadOnly: An infragistics' read only DatePicker for the FormGenerator.</item>
    ///     <item>COEDocManager: A docmanager for the FormGenerator.</item>
    ///     <item>COEDocManagerClassic: A docmanager classic for the FormGenerator.</item>
    ///     <item>COEDropDownList: A dropdownlist for the FormGenerator</item>
    ///     <item>COEFormGenerator: A container that is capable of dinamically rendering several <see cref="ICOEGenerableControl"/> based on
    ///     an xml definition.</item>
    ///     <item>COEFormGeneratorManager: A <see cref="COEFormGenerator"/> holder that allows to manipulate several form generators into a 
    ///     single xml definition.</item>
    ///     <item>COEFormGeneratorRepeater: A <see cref="COEFormGenerator"/> repeater.</item>
    ///     <item>COEGridView: A gridview for the FormGenerator.</item>
    ///     <item>COELabel: A label for the FormGenerator.</item>
    ///     <item>COELinkButton: A linkbutton for the FormGenerator.</item>
    ///     <item>COERadioButton: A radiobutton for the FormGenerator.</item>
    ///     <item>COETexArea: A multiline TextBox for the FormGenerator.</item>
    ///     <item>COETextAreaReadOnly: A read only multiline TextBox for the FormGenerator.</item>
    ///     <item>COETextBox: A TextBox for the FormGenerator.</item>
    ///     <item>COETextBoxReadOnly: A read only TextBox for the FormGenerator.</item>
    ///     <item>COETextEdit: An infragistic's textbox for the FormGenerator.</item>
    ///     <item>COEUrlControl: Unknown.</item>
    /// </list>
    /// <para>
    /// Some interfaces are also provided to allow thid party controls to be plugged into a FormGenerator:
    /// </para>
    /// <list type="bullet">
    ///     <item>ICOEContainer: Mark a control as holder of controls and that allows the use of Datasources and pagging.</item>
    ///     <item>ICOECultureable: Mark a control as cultureable, meaning that the control will behave different for each CultureInfo.</item>
    ///     <item>ICOEGenerableControl: This is the main interface and is mandatory to implement, it defines de basic capabilities needed
    ///     to be rendered by a FormGenerator.</item>
    ///     <item>ICOELabelable: Mark the control as labelable.</item>
    ///     <item>ICOERequireable: Defines that the control may be required, and may behave different if it is required.</item>
    /// </list>
    /// </summary>
    class NamespaceDoc { }
}
namespace CambridgeSoft.COE.Framework.Controls.ChemDraw {
    /// <summary>
    /// <para>In this namespaces there are several classes intended for Chemimcal drawing.</para>
    /// </summary>
    class NamespaceDoc { }
}

namespace CambridgeSoft.COE.Framework.Common {
    /// <summary>
    /// <para></para>
    /// </summary>
    class NamespaceDoc { }
}

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator {
    /// <summary>
    /// <para>
    /// Base namespace. It encapsulates all the tasks requied for generating executable SQL queries in the form of prepared statements 
    /// (that means, the query text on one side and the parameters in the other) whether from xml input or programmatically.
    /// </para>
    /// </summary>
    class NamespaceDoc { }
}

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData {
    /// <summary>
    /// <para>
    /// Metadata namespace contains classes DataView, ResultCriteria and SearchCriteria. These classes are responsible for creating a query 
    /// based upon the corresponding input xmls: DataView.xml, ResultCriterial.xml and SearchCriteria.xml (or the MessagingType that 
    /// represents them). For this purpose, these classes use the classes contained in the query. 
    /// The DataView, class besides parsing the input xml, stores the database schema required for this query. It translates ids to names, 
    /// solves relationships between tables, etc. 
    /// Both ResultCriteria and SearchCriteria use the following approach for parsing: They iterate through the items of the xml, and extract 
    /// the xml tag of that portion. This is passed as parameter to a factory that knows wich class to instanciate depending on the type of 
    /// item and returns it. The resulting item is then added to the query. 
    /// ResultsCriteria differs in that the user can provide new plug-in based resultsCriteria. For this, the user must supply an assembly 
    /// containing the item itself and a parser, required for creating the item from xml. Also, this plug-in has to be registered in the file 
    /// mappings.xml. 
    /// </para>
    /// </summary>
    class NamespaceDoc { }
}

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.MetaData.Graphs {
    /// <summary>
    /// <para>
    /// This namespace wraps the functionality of the quickgraph library, needed for resolving tables relationships from the dataview.
    /// </para>
    /// </summary>
    class NamespaceDoc { }
}

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries {
    /// <summary>
    /// <para>
    /// This namespace contains the required classes for producing a SQL Select Statement targeted to a given DataBase. Currently supports 
    /// Oracle, MS-Access and MS-SQL Server. 
    /// 
    /// This classes only manipulate strings, and don't have any additional "inteligence". 
    /// 
    /// The Design of this classes follow a Command Pattern: The Main class contains an array of Commands. Each Command implements an 
    /// "Execute" Method. When the main class is asked to perform its task, ir loops the array calling the execute method of every command 
    /// that was added to it. 
    /// 
    /// In this case, the main class is Query, and the "Execute method" is called GetDependantString. In this case, there are two arrays of 
    /// commands: one for the select clause and other for the where clause. 
    /// 
    /// When you want to retrieve the sql query string, you call GetDependantString on Query, which in turn calls it on every command it 
    /// contains. 
    /// </para>
    /// </summary>
    class NamespaceDoc { }
}

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.NonQueries {
    /// <summary>
    /// <para>
    /// This namespace contains the required classes for producing a SQL Statement that are not Select (Queries), targeted to a given DataBase. Currently supports 
    /// Oracle, MS-Access and MS-SQL Server. 
    /// 
    /// This classes only manipulate strings, and don't have any additional "inteligence". 
    /// 
    /// The Design of this classes follow a Command Pattern, with the difference that the method to be called is named GetDependantString instead
    /// of Execute.
    /// 
    /// Currently there are two NonQueries implemented:
    /// </para>
    /// <list type="bullet">
    ///   <item>Insert statement</item>
    ///   <item>Truncate statement</item>
    /// </list>
    /// 
    /// <para>
    /// When you want to retrieve the sql nonquery string, you call GetDependantString on NonQuery, which in turn calls it on every command it 
    /// contains. 
    /// </para>
    /// </summary>
    class NamespaceDoc { }
}
namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.SelectItems {
    /// <summary>
    /// <para>
    /// Select items of a query (Single Field, Literal, MAX(Field), ISNULL(...), etc. Anything that can go in the select part of the statement.
    /// 
    /// Analog to WhereClauseItems with the exception that the user can supply it's own commands here, as plug-ins located in a separate 
    /// assembly. 
    /// </para>
    /// </summary>
    class NamespaceDoc { }
}

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Queries.WhereItems {
    /// <summary>
    /// <para>
    /// Where items of a query (Comparisons, IN, ISNULL, etc.). Anything that can go in the where part of the query statement.
    /// Analog to SelectClauseItems
    /// </para>
    /// </summary>
    class NamespaceDoc { }
}

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils {
    /// <summary>
    /// <para>
    /// Numerous utilities needed for making our life easier :). Convertion from db-specific types to abstracts like numeric, text, date, etc.
    /// Data Manipulation functions like trims of numeric values, chemical normalizations, etc. Special Characters Handling. 
    /// </para>
    /// </summary>
    class NamespaceDoc { }
}