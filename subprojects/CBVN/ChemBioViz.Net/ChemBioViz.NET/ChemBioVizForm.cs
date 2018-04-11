using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Windows.Forms.Layout;
using System.Threading;
using System.Linq;

using Greatis.FormDesigner;
using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COESearchService;
using CambridgeSoft.COE.Framework.Common.Messaging;
using CambridgeSoft.COE.Framework.COEExportService;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.COEHitListService;

using Infragistics.Win;
using Infragistics.Win.UltraWinExplorerBar;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinDock;
using Infragistics.Win.UltraWinTree;

using ChemControls;
using FormDBLib;
using CBVUtilities;
using CBVControls;
using Utilities;
using ChemBioViz.NET.Exceptions;
using FormDBLib.Exceptions;
using System.Drawing.Printing;
using Infragistics.Win.Printing;
using Infragistics.Win.UltraWinToolTip;
using SearchPreferences;
using FormWizard;
using SharedLib;

/*  VERSION HISTORY
 * 
 * 8.24.10      introduce build labels; enable close/export
 * 8.24.10-2    CSBR-130064 copyright update; CSBR-129166 action menus
 * 8.25.10      update to latest framework .net\release\11.0.x.  Breaks mothball.  No chgs => no commit.
 * 8.25.10-2    CSBR-129260: allow full pathname starting with root, as arg to Open Form
 * 8.27.10      CSBR-129488: prevent overflow when plotting against min/max
 * 8.27.10-2    (bug not yet filed): prevent crash in cancel out of login dialog
 * 8.30.10      CSBR-130311: Buttons whose visibility was set False are appearing in Print preview
 * 8.30.10-2    Point framework references to 11.0.2 instead of 11.0.x
 * 9.02.10      CSBR-126117: Deleting a form doesn't delete the form until the applications is closed
 * 9.08.10      ----------- Now at ver 11.0.3.  Implement lookup combos.
 * 9.09.10      Allow server paths as child form names; new forms on mothball
 * 9.13.10-1    Use Close instead of Exit (suggested by Sunil)
 * 9.13.10-2    Search over list
 * 9.14.10      Query with parent id, serialized; move chunk of code to form_treemethods.cs
 * 9.15.10      Handle child query leaf items when building UI tree
 * 9.15.10-2    Fix child query leaf items to appear in correct locations in tree config
 * 9.16.10      Fix to allow use of cached dataviews .. but not yet working except in 2T mode
 * 9.20.10      Fix to restore last group selected before edit
 * 9.20.10-2    Hide toolbars from child forms
 * 9.20.10-3    Implement sensible action when rerunning child query
 * 9.21.10      Crash prot in ParseAndFill; add SOCL to feat enabler
 * 9.22.10      Return button data from grandchild fields
 * 9.27.10      Binding navigator in child windows; move record on grid row change
 * 9.27.10-2    CSBR-130282: use Close instead of App.Exit everywhere
 * 9.28.10      CSBR-128496: add Revert Edits on File menu
 * 9.28.10-2    CSBR-130705: warn if name in use on Save As
 * 9.29.10      CSBR-120726: fix form #166; use last save dir when prompting for Open Local
 * 9.29.10-2    Show form ID in connection dialog
 * 10.11.10     Addins; units; child rec select; no combo in std form; action menu clear; exit not close
 * 10.18.10     Add richtext support; struc lookups
 * 10.19.10     Support date picker and month control
 * 10.19.10-2   CSBR-131778: Hidden fields reappearing in subform grids when query form edited
 * 10.19.10-3   Units table moved into resources
 * 10.19.10-4   CSBR-126115: shift all controls to positive Y on saving
 * 10.21.10     CSBR-132611: modify machine name if starts with digit
 * 10.26.10     SSS Combo; QueryExtractor; Exact search; no combo fill on non-lookup; transparent trackbar
 * 10.27.10     View Tooltips; improved card view
 * 11.02.10     Export delimited; browse button
 * 11.02.10-2   Refinements of browse button
 * 11.03.10     Prevent changing tabs when editing grid
 * 11.04.10     CSBR-132740: struct scaling in grid; 132554: prevent open if wrong dataview; fix FullBindingSource
 * 11.05.10     CSBR-132766: save visible tab order as new VisIndex flags in tabs xml
 * 11.05.10-1   CSBR-133387,133382,133193,132766,133372,133385,133386,133380,133377
 * 11.08.10     133388 (rename child headers); 133431 (Clear button); don't disable Close; 
 *              133438 (row sel in child band); tab delete; 132998 (create log file)
 * 11.10.10     CSBR-133490 (crash in tab delete); fix View Explorer menu; prevent alert if Export fails; 133517 (file ext); 
 *              try-catch around validateTicket; carry hitlist type around with id, update query.IsSaved
 * 11.10.10-2   Retain username after auth ticket login
 * 11.12.10     CSBR-133438; revamp ChemDataProvider
 * 11.15.10     CSBR-133756: remove tag after edit if no binding; 133393: retain col ordering in card view
 * 11.16.10     CSBR-133913: fix sel to list when plotting primary key
 * 11.16.10-2   CSBR-133375: structure layouts in card view
 * 11.16.10-3   CSBR-133884: expose plot point limit on prefs
 * 11.16.10-4   CSBR-132401: safe get of units resources
 * 11.17.10     CSBR-123614: add "Use SSL" to login dialog
 * 11.17.10-2   CSBR-133977: change style on select No Style
 * 11.17.10-3   CSBR-133968: variable height in card view
 * 11.17.10-4   CSBR-132931: return after Exit call
 * 11.17.10-5   CSBR-131778: skip hidden fields when building rc's
 * 11.18.10     Fixes for card view; white background for structs in grids
 * 11.19.10     CSBR-134011 (unremoved menu); Frame instead of group box; fix DoMove call at rowactivate; treenode fix
 * 11.19.10-2   Fix erase of text before frame update
 * 11.19.10-3   CSBR-134055: better structure sizing in card view
 * 11.19.10-4   CSBR-134056: prohibit add new card view tab except on form tab
 * 11.22.10     CSBR-134176: Fix for SSO authentication
 * 11.22.10-2   CSBR-133974: Return to specified group after form edit
 * 11.26.10     CSBR-134386: do not remove tags from query boxes (reintroduces CSBR-133756); trim down toolbox for query tab
 * 11.29.10     CSBR-134344: Frame box name disappears when we close & reopen the form
 * 11.29.10-2   CSBR-134439 (crash if can't create file); CSBR-133517 (change extension to sdf)
 * 12.08.10     CSBR-135003: handle auto-link box without subform
 * 12.09.10     CSBR-134486: Hidden Subform grid is reappearing once the form is closed & reopened
 * 12.10.10     Fix order dependence of subform/grandchild binding; better error handling on opening defective form
 * 12.13.10     CSBR-135144: handle checkbox (2- and 3-state) in query form
 * 12.13.10-2   CSBR-135168: retain Accept/Cancel buttons thru edit/save/reload
 * 12.14.10     New Boxname on QueryComp for restoring; CSBR-134533: no preview over selection in card view; clear q form before restore
 * 12.14.10-2   CSBR-134572: restore view menu dimming when editing form; view commands now toggles
 * 12.14.10-3   CSBR-134653: pass correct row when click grid hyperlink past first page
 * 12.14.10-4   CSBR-134655: keep drill-down form in synch on click child row in subform
 * 12.14.10-5   CSBR-134754: do not report export job completed if errors encountered
 * 12.14.10-6   CSBR-134839: avoid error if addins dir nonexistent
 * 12.14.10-7   CSBR-134875: Tabs returning to default position
 * 12.15.10     CSBR-134541: mark mod on Revert; CSBR-135265: form edit privs on menu command
 * 12.16.10     CSBR-135329: if query is quoted, do not interpret as list
 * 12.21.10     ----------- 11.0.4 starts here
 * 12.22.10     CSBR-132967: prevent crash if no CDAX installed
 * 01.03.11     CSBR-135660: fix parsing command line from button arg
 * 01.03.11-2   CSBR-135661: new /hitlist=id on cmd line; * handling in q tree; add cmd-line query to tree
 * 01.05.11     New Jump To button command; ChildDocForm class; new childforms.cs file
 * 01.05.11-2   CSBR-135536: restore status line after query run
 * 01.11.11     CSBR-135838: merge tables in rc rather than skip if found; aggreg code under development
 * 01.12.11     CSBR-135585 (in progress): separate query member SC from global default
 * 01.12.11-2   CSBR-135585 fixed: restore sss combo on restore query
 * 01.13.11     CSBR-135582: freeze display, then mark each tab as visible before saving
 * 01.13.11-2   CSBR-135966 -- rec nav invisible in query tab; CSBR-134702 -- rebind form tab after edit query
 * 01.13.11-3   CSBR-133936 -- do not default plots to stretchable
 * 01.24.11     CSBR-132594 -- save infragistics display layout settings in form
 * 01.25.11     Prototypes for: aggregate values in new CBVTextBox; multi-structure query input
 * 01.25.11-2   CSBR-132719: Card view - adjusted dimensions are not saved
 * 01.27.11     CSBR-115292, 132998: protect against faulty log path
 * 01.27.11-2   CSBR-132401: units drop-down
 * 02.01.11     aggregate function boxes; update copyright year
 * 02.02.11     add units data to query for restoring; move code from query to cbvform
 * 02.03.11     CSBR-136866: finally a proper fix for record navigator text entry
 * 02.04.11     CSBR-137043: set numeric form and null value for form and grid; uses Tag string
 * 02.08.11     Add Jump-To with field search
 * 02.09.11     Fix bindingsToTags for grandchild boxes -- use relation string, not syncroot
 * 02.10.11     CSBR-132730, 137253 - tooltip issues; larger chemdraw box
 * 02.10.11-2   Custom tooltips for buttons
 * 02.10.14     CSBR-137261: move frames to end of tab order list <== NOTE: wrong date
 * 02.15.11     Rework areas where grandchild binding source was not being interpreted well
 * 02.15.11-2   Revise previous fix to handle both child and grandchild and aggregates
 * 02.16.11     Better fix for handling grandchild tables, adapting to change in server dataset
 * 02.16.11-2   CSBR-133822: sort dataviews tree
 * 02.16.11-3   Sort on child table cols
 * 02.17.11     Instant subform sort using grid; CSBR-134655: fixed synch of subform with child form
 * 02.17.11-2   CSBR-134057: Height of structure cell not saved in card view
 * 02.17.11-3   CSBR-136292: replace old-style textbox objects with new on loading file
 * 02.22.11     Add tooltips for query and text boxes; query text box properties editor
 * 02.23.11     CSBR-134061-related: do not abort on failure to load infra settings; ask user
 * 02.28.11     Completed scheme for search by hitlist
 * 03.01.11     CSBR-138041: new pager mechanism with ProcessIncomingDataSet; finished JumpTo
 * 03.01.11-2   CSBR-131835-related: do not allow entry to form editor if blank form and dv tree not showing
 * 03.01.11-3   CSBR-131835-related: ensure table is selected on entry to form edit; change ViewRefresh
 * 03.02.11     Completion of jump-to; aggregate fixes
 * 03.03.11     Improve dv caching to incorporate dv name into filename
 * 03.07.11     Add aggregate prop to query text box; use in searching
 * 03.08.11     Fix binding aggreg box to grid; SD struct search; dclick dv tree => no subforms; crash prot in conn dlg; better dvid save
 * 03.09.11     Improved fld change detect on end edit; try/catch in domove
 * 03.09.11-2   View Tooltips better fix
 * 03.09.11-3   Report binding error to user
 * 03.14.11     Fixed problem with Ironwood db: table id > 32768 -- changed int16.parse to int32
 * 03.15.11     SD Export with field chooser; initial routine to flatten dataset
 * 03.16.11     SD Export prohibit grandchildren; show or hide children based on radio; hide for Delim Export
 * 03.17.11     Match on alias when deciding if subform can be created from dv tree
 * 03.21.11     Report errors if unable to load add-in
 * 03.22.11     CSBR-139239: allow adding aggreg box without subform present
 * 03.22.11-2   CSBR-138420: use page row when filling button args
 * 03.28.11     Launch Embedded Doc action and handler
 * 03.28.11-2   CSBR-139347: turn off tipstylescroll
 * 03.30.11     Launch on click; ChildAggregTable on cbvTextBox; rebuild on View Refresh
 * 03.31.11     Point to framework 11.0.4 instead of 11.0.x
 * 03.31.11-2   CSBR-139779: handle subform fields in button args; fix some tag<->dbinding problems
 * 04.04.11     ----------- Start 12.1 / Monet branch; point to fw 11.0.x
 * 04.04.11-2   Update to Greatis 2.8
 * 04.04.11-3   Include spotfire addin as reference
 * 04.05.11     Add smart-tag machinery to cbvtextbox
 * 04.07.11     Brute-force fix for bad scroll positions in save/edit: scroll to origin first
 * 04.08.11     CSBR-140172: cancel after prompt for new query name; some crash protection in form edit
 * 04.12.11     Smart tags for choosing table/field/aggreg in form editor; written then rewritten
 * 04.22.11     Upgrade to new spotfire addin code from Array; update greatis patch
 * 04.27.11     Restore child queries to tree with parentage (bug being filed)
 * 05.04.11     Default sort: add RtvAll query to tree
 * 05.05.11     CSBR-141475: unable to mark controls as invisible (Greatis regression)
 * 05.10.11     CSBR-141809: protect against missing RetrieveAllQuery
 * 05.11.11     CSBR-141787: reworked handing of child queries
 * 05.11.11-2   Fix menu dimmings in query context menu
 * 05.12.11     Fix bug in sorting by aggregate functions; try/catch in QueriesToTree
 * 05.23.11     Fix problem in 2T login -- override coeframeworkconfig url with that of requested server
 * 05.24.11     Back out above change
 * 05.26.11     CSBR-142585: do not set binding prop to "Text" if set to other type
 * 05.26.11-2   Disable smart tag mechanism for upcoming monet release
 * 05.31.11     CSBR-142661: disable tab context menu when editing; crash fix for folder in query tree
 * 06.01.11     CSBR-142590: new query member DefaultSort
 * 06.02.11     CSBR-142926: Query tree does not hilite current query after merge
 * 06.08.11     CSBR-143105; CSBR-143048: Restoring full list does not go back to default sort order
 * 06.15.11     CSBR-142224; remove form wizard; prev ambiguous cols if two boxes on same aggreg
 * 06.16.11     CSBR-143531: look for relations in main binding source, not bs
 * 06.18.11     CSBR-141475: if form is maximizing or restoring, set window state of form view to match
 * 06.18.11     CSBR-141475: better fix
 * 06.23.11     CSBR-143846: case-dependent match on subform table name; CSBR-143860: fail binding to child field if no subform
 * 06.26.11     CSBR-143951: Wrong HELP file is linked under HELP in the ChemBioViz UI
 * 06.27.11     CSBR-144005: clear rc before adding fields from export checkbox
 * 06.28.11     ----------- Start 12.3 / Degas branch; point to fw EN1230_development
 * 06.28.11     work on structure highlights begun; merge 12.1 fixes (143846,143951,144005,143860)
 * 06.30.11     CSBR-144127: accept/cancel buttons improved; crash prot added in ParseBindingSource
 * 07.05.11     CSBR-144127 et al: logic bug in ParseBindingSource; affects binding to child fields.  Reenable smart tags.
 * 07.07.11     ----------- Merged from 12.1.1 patch
 * 07.07.11     CSBR-144127: data binding fix; fix for accept/cancel buttons
 * 07.08.11     CSBR-144081: repair binding on change of aggreg field
 * 07.08.11-2   CSBR-144053: move query buttons to top of control list after editing when maximized
 * 07.11.11     Fixes merged from 12.1.1 patch
 * 07.12.11     Child hits filtering
 * 07.13.11     Replace sim trackbar with num ctl; remove * on rename query; restore struct options on restore query
 * 07.14.11     Fix for initial value of FilterChildHits; add smart tag to query text box
 * 07.18.11     Structure highlighting
 * 07.21.11     Crash protection in tool check
 * 07.28.11     Add global pref to turn on/off sss highlights
 * 08.08.11     Downgrade Greatis to 11.0.4 version
 * 09.09.11     Revamp projects to build x86 only, output to single bin; sign assemblies
 * 09.09.11-2   CSBR-147470: Hyperlinks in Grid or Table tab are not showing data.  Introduce RowStack.
 * 09.09.11-3   Reference signed ChemControls in determining presence of CDAX
 * 09.09.11-4   CSBR-147559: prevent crash if dataview faulty
 * 09.09.11-5   Access to form wizard via debug
 * 09.12.11     CSBR-147673: avoid sending HighlightedStructure in RC when exporting to SD
 * 09.13.11     Export to CD/Excel now working
 * 09.13.11-2   Finish hack to convert incoming ChemControls to new signed versions
 * 09.14.11     New mechanism for loading addins from strings in app.config
 * 09.15.11     Export to CD/Excel working for ChemOffice 12
 * 09.15.11-2   Save name and comments with saved hitlist
 * 09.15.11-3   Fix rec synch problem in SFI when changing to grid view
 * 09.15.11-4   DemoExcelAddin fails to open file if name contains spaces
 * 09.19.11     Better scheme for processing incoming ChemControls defs; remove proj ResearchAssayHistoryForm
 * 09.20.11     Allow spotfire dxp filename to be added to button arg
 * 09.21.11     Allow InitWithString addin method => dxp filename as SF addin button arg
 * 09.21.11-2   Eliminate dependency of FormDBLib on ChemControls; introduce new SharedLib
 * 09.22.11     Make SSSHilites flag static so consultable at low level; eliminate x86 sln platform
 * 09.26.11     Incorporate Array changes to addins; calls Spotfire without COM
 * 09.29.11     Logging of stat msgs; hook up RecordsetChanged event; preliminary alert on change recordset
 * 10.03.11     Spotfire menu; new SpotfireMgr class; SFAutoRefresh global; CheckMenuItem; addin Deinit method; GetSelRecnos change; rid of ResultSet
 * 10.06.11     Add Spotfire Open Analysis
 * 10.10.11     Revamp addins with menu interface
 * 10.10.11-2   Add comments to interface file; add settings to SFI project
 * 10.10.11     ----------- Start 12.4 branch
 * 10.11.11     Get rid of SF auto-refresh setting in app, move to addin
 * 10.13.11     Addin settings serialized in new prefs string
 * 10.13.11-2   Add "warn on refresh" setting
 * 10.13.11-3   Add "warn on rebuild" setting
 * 10.17.11     Do not create menu if Spotfire not available
 * 10.17.11-2   New AddinException class; show alert if addin cannot execute
 * 10.17.11-3   Spotfire addin props dialog; warn on refresh/rebuild
 * 10.18.11     Added events for FormOpened, FormEdited
 * 10.20.11     Warning dialogs with checkboxes
 * 10.20.11-2   Change term "recordset" to "Spotfire data table" per Weaver suggestion
 * 10.20.11-3   Add try/catch to prevent breaking connection on table loader error
 * 10.21.11     Export to CBV Excel (Santosh)
 * 10.21.11-1   Update to new Ian scheme; fix broken dxp load; no warn on form close if SF not running
 * 10.24.11     Force SF table rebuild on load from dxp file
 * 10.24.11-2   Add CBVFormClosed event; handle state flag in integration object
 * 10.24.11-3   Add new member to SFI: formAddinArgs, per Ian suggestion
 * 10.25.11     Fire FormClosed event on UnloadForm; fix debug output; add forceReload flag
 * 10.26.11     Add MaxRows prop to addin and extension; not yet available in UI
 * 10.27.11     Handle Spotfire update on change of resultsCriteria (as in sort); add row limit to SF prefs
 * 10.28.11     Show rebuild warning instead of refresh if hitlists not on same dataviews
 * 10.31.11     Add rest of list in UnregisterEvents; prevent warning when first connecting to SF
 * 11.01.11     Strip username from MRU entry, fixes loading dxp into SF.  Rename Login.ServerDisplay to avoid further confusion
 * 11.02.11     Protections during close of SF analysis: try/catch on closing channelfactory; elim redundant EndInteg calls
 * 11.02.11-2   Prevent SF table rebuild warning if no hitlist open in SF
 * 11.03.11     Prevent crash in StartIntegration if no form content, also dim Open Analysis menu item
 * 11.08.11     Addin menus with icons and updatable state; new scheme for add/remove of addin and action menus
 * 11.15.11     Do not allow sort by click in grid view; add list-size warning on recordset change
 * 
 * */

namespace ChemBioViz.NET
{
    public partial class ChemBioVizForm : Form
    {
        #region Variables
        public const String BuildDate = "11.15.11";

        protected FormDbMgr m_formdbMgr;                // layer between form and database
        protected BindingSource m_bindingSource;            // from boxes of form tabs
        protected BindingSource m_fullBindingSource;        // from full dataview

        protected TabManager m_tabManager;
        protected ToolboxControlEx m_toolbox;
        protected PropertyGrid m_propertyGrid;
        protected CBVBindingNavigator m_bindingNavigator;

        protected UltraTree m_ultraTreeView;
        protected QueryCollection m_queries;
        protected Query m_currQuery = null;
        protected Query m_qToMerge = null;

        protected String m_formName = string.Empty;  // saved under this name if in bank
        protected int m_formID = -1;              // saved under this ID in bank
        protected String m_appName, m_tableName;	    // stored in old-style formfile
        protected int m_appID, m_tableID;         // stored in formfile as of 12/09: supercedes the above
        protected String m_localFilePath;            // blank if not stored locally
        protected String m_comments;
        protected string m_cbvExcelExportFileName = string.Empty; //saved the filename which used in saveas dialog box                
        protected string initialSXML = string.Empty;
        bool canSavePublic = false;

        public enum formType { Unknown = -1, Unsaved, Public, Private, Local };
        formType m_formType;
        protected CommandLine m_cmdLine;                  // command-line argument processor

        protected int m_nextFormNo = 0;
        protected int m_rightClickedTabIndex = -1;
        protected int m_maxGroupHeadersBeforeEdit = -1;
        protected Point m_lastMouseDownPoint = Point.Empty;

        protected bool m_modified;             // true if form has been changed and not saved
        protected bool m_editingGrid;
        protected bool m_bHasNoCdax;
        protected bool m_isPublicPane;

        protected ExportOpts m_exportOpts;
        protected Infragistics.Win.UltraWinGrid.ColumnHeader m_clickedGridHeader;

        protected CBVGridPrintingHelper m_gridprint;
        protected CBVFormPrintingHelper m_formprint;
        protected FormsTreeConfig m_FTreeConf;
        protected QueriesTreeConfig m_QTreeConf;
        protected UltraToolTipManager m_tooltipManager = null;
        protected CBVChartController m_chartController;

        protected List<CBVChildForm> m_children;
        protected FeatEnabler m_featEnabler;
        protected UltraExplorerBarGroup m_groupBeforeEdit = null;

        public CBVAddinsManager m_addinMgr = null;

        public delegate void SearchCompletedEventHandler(Object sender, EventArgs e);
        public event SearchCompletedEventHandler SearchCompleted;

        public delegate void ActionButtonClickedEventHandler(Object sender, ActionButtonEventArgs e);
        public event ActionButtonClickedEventHandler ActionButtonClicked;

        public delegate void RecordsetChangedEventHandler(Object sender, RecordsetChangedEventArgs e);
        public event RecordsetChangedEventHandler RecordsetChanged;

        public delegate void RecordChangedEventHandler(Object sender, RecordChangedEventArgs e);
        public event RecordChangedEventHandler RecordChanged;

        public delegate void SubRecordChangedEventHandler(Object sender, SubRecordChangedEventArgs e);
        public event SubRecordChangedEventHandler SubRecordChanged;

        public delegate void FormOpenedEventHandler(Object sender, FormOpenedEventArgs e);
        public event FormOpenedEventHandler FormOpened;

        public delegate void FormEditedEventHandler(Object sender, FormEditedEventArgs e);
        public event FormEditedEventHandler FormEdited;

        public delegate void CBVFormClosedEventHandler(Object sender, CBVFormClosedEventArgs e);
        public event CBVFormClosedEventHandler CBVFormClosed;

        /* CSBR-136190 Feature Request: It would be helpful if the Export status is shown while exporting to SDFile   
           Creating the BackgroundWorker Object and Exportprocess Object */
        public BackgroundWorker bgw;
        public static ExportProcess _exportProcess = new ExportProcess();
        /* End of CSBR-136190  */

        #endregion

        #region Properties
        public ExportOpts ExportOpts
        {
            get { return m_exportOpts; }
            set { m_exportOpts = value; }
        }
        //---------------------------------------------------------------------
        public CommandLine CommandLine
        {
            get { return m_cmdLine; }
            set { m_cmdLine = value; }
        }
        //---------------------------------------------------------------------
        public bool HasChildForms
        {
            get { return m_children != null && m_children.Count > 0; }
        }
        //---------------------------------------------------------------------
        public virtual bool IsChildForm
        {
            get { return false; }
        }
        //---------------------------------------------------------------------
        public virtual bool IsChildDocForm
        {
            get { return false; }
        }
        //---------------------------------------------------------------------
        public virtual new ChemBioVizForm ParentForm
        {
            get { return null; }
        }
        //---------------------------------------------------------------------
        public bool IsLocal
        {
            get { return m_formType == formType.Local; }
        }
        //---------------------------------------------------------------------
        public bool IsPublic
        {
            get { return m_formType == formType.Public; }
        }
        //---------------------------------------------------------------------
        public bool IsPrivate
        {
            get { return m_formType == formType.Private; }
        }
        //---------------------------------------------------------------------
        public bool IsUnsaved
        {
            get { return m_formType == formType.Unsaved; }
        }
        //---------------------------------------------------------------------
        public formType FormType
        {
            get { return m_formType; }
            set { m_formType = value; }
        }
        //---------------------------------------------------------------------
        public TabManager TabManager
        {
            get { return this.m_tabManager; }
        }
        //---------------------------------------------------------------------
        public FormDbMgr FormDbMgr
        {
            get { return m_formdbMgr; }
            set { m_formdbMgr = value; }
        }
        //---------------------------------------------------------------------
        public int AppID
        {
            get { return m_appID; }
            set { m_appID = value; }
        }
        //---------------------------------------------------------------------
        public int TableID
        {
            get { return m_tableID; }
            set { m_tableID = value; }
        }
        //---------------------------------------------------------------------
        public int FormID
        {
            get { return m_formID; }
        }
        //---------------------------------------------------------------------
        public CBVBindingNavigator BindingNavigator
        {
            get { return m_bindingNavigator; }
            set { m_bindingNavigator = value; }
        }
        //---------------------------------------------------------------------
        public BindingSource BindingSource
        {
            get { return m_bindingSource; }
            set { m_bindingSource = value; }
        }
        //---------------------------------------------------------------------
        public BindingSource FullBindingSource
        {
            get
            {
                BindingSource bs = m_fullBindingSource;
                bool bFBSIsObsolete = m_fullBindingSource != null && FormDbMgr.SelectedDataView != null &&
                        !CBVUtil.Eqstrs(FormDbMgr.SelectedDataViewBOName, FormDbMgr.DvNameOfFullCriteria);
                if (bFBSIsObsolete || m_fullBindingSource == null)
                    m_fullBindingSource = CreateFullBindingSource();
                return m_fullBindingSource;
            }
            set { m_fullBindingSource = value; }
        }
        //---------------------------------------------------------------------
        public Control FillPanel
        {
            get { return Form1_Fill_Panel; }
        }
        //---------------------------------------------------------------------
        public UltraTabControl TabControl
        {
            get { return MainUltraTabControl; }
        }
        //---------------------------------------------------------------------
        public Query CurrQuery
        {
            get { return m_currQuery; }
            set { m_currQuery = value; }
        }
        //---------------------------------------------------------------------
        public Pager Pager
        {
            get { return (CurrQuery == null) ? null : CurrQuery.Pager; }
        }
        //---------------------------------------------------------------------
        public String FormName
        {
            get { return m_formName; }
            set { m_formName = value; }
        }
        //---------------------------------------------------------------------
        public String Comments
        {
            get { return m_comments; }
            set { m_comments = value; }
        }
        //---------------------------------------------------------------------
        public QueryCollection QueryCollection
        {
            get { return m_queries; }
            set { m_queries = value; }
        }
        //---------------------------------------------------------------------
        public CBVAddinsManager AddinsManager
        {
            get { return m_addinMgr; }
        }
        //---------------------------------------------------------------------
        public string CBVExcelExportFilename
        {
            get { return m_cbvExcelExportFileName; }
            set { m_cbvExcelExportFileName = value; }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CBVChartController ChartController
        {
            get { return m_chartController; }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(false)]
        public CBVChartControl SelectedPlot
        {
            get { return ChartController.SelectedPlot; }
            set { ChartController.SelectedPlot = value; }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ContextMenuStrip PlotContextMenuStrip
        {
            get { return this.plotContextMenuStrip; }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Indicates when the form has been modified
        /// </summary>
        public bool Modified
        {
            get { return m_modified; }
            set
            {
                m_modified = value;

                string paneName = this.IsPublic ? CBVConstants.PUBLIC_GROUPNAME :
                    this.IsPrivate ? CBVConstants.PRIVATE_GROUPNAME :
                    this.IsLocal ? CBVConstants.LOCAL_FORMS : string.Empty;
                // show * in title if modified
                SetTitles(paneName,
                        String.Concat(paneName, " - ", FormName, " (opened)"), FormName + (Modified ? "*" : string.Empty));
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Returns the name of selected group from the Nav Pane
        /// </summary>
        private String ActivePaneName
        {
            get { return ultraExplorerBar1.SelectedGroup.Key; }
        }
        //---------------------------------------------------------------------
        private bool IsFormsTree
        {
            get
            {
                return (
                    (ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                    ? true : false);
            }
        }
        //---------------------------------------------------------------------
        private bool IsPublicPaneActive
        {
            get { return CBVUtil.StartsWith(ActivePaneName, CBVConstants.PUBLIC); }
        }
        //---------------------------------------------------------------------
        public DbObjectBank ActiveDBBank
        {
            get { return IsPublicPaneActive ? m_formdbMgr.PublicFormBank : m_formdbMgr.PrivateFormBank; }
        }
        //---------------------------------------------------------------------
        [BrowsableAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FeatEnabler FeatEnabler
        {
            get { return m_featEnabler; }
        }
        //---------------------------------------------------------------------
        public UltraToolbarsManager MainToolbarManager
        {
            get { return this.mainFormUltraToolbarsManager; }
        }
        //---------------------------------------------------------------------

        /* CSBR-136190 Feature Request: It would be helpful if the Export status is shown while exporting to SDFile
         * Change done by Jogi */
        public bool CancelBackgroundWorker { set { if (value && bgw != null && bgw.IsBusy) bgw.CancelAsync(); } }
        /* End of CSBR-136190 */
        //---------------------------------------------------------------------
        #endregion

        #region Constructor
        /// <summary>
        /// This is the main dialog: contains form, grid, query views bound to data source
        /// change view via tabs on form
        /// </summary>
        public ChemBioVizForm(CommandLine cmdLine)
        {
            m_cmdLine = cmdLine;
            InitializeComponent();
            this.CenterToParent();
            m_gridprint = new CBVGridPrintingHelper();
            m_formprint = new CBVFormPrintingHelper();
            m_appID = m_tableID = 0;
            m_FTreeConf = new FormsTreeConfig();
            m_QTreeConf = new QueriesTreeConfig();
            m_tooltipManager = new UltraToolTipManager();
            m_tooltipManager.InitialDelay = 2000;   // needs more work .. this doesn't work as expected
            m_tooltipManager.AutoPopDelay = 4000;
            m_formType = formType.Unsaved;
            m_chartController = new CBVChartController(this);
            m_children = new List<CBVChildForm>();
            m_editingGrid = false;
            m_featEnabler = new FeatEnabler();
            ExportOpts = new ExportOpts();
            m_bHasNoCdax = false;
        }
        #endregion

        #region Methods
        //---------------------------------------------------------------------
        public int GetSavedHitlistID()
        {
            if (m_currQuery == null)
                return 0;
            if (!m_currQuery.IsSaved)
                m_currQuery.SaveHitlist();

            return m_currQuery.HitListID;
        }
        //---------------------------------------------------------------------
        public void AddChildForm(CBVChildForm childForm)
        {
            m_children.Add(childForm);
        }
        //---------------------------------------------------------------------
        public void RemoveChildForm(CBVChildForm childForm)
        {
            m_children.Remove(childForm);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Initialize the PreferencesHelper instance
        /// </summary>
        private void ConfigureSettingsOnLoading()
        {
            bool bInitialCall = false;
            if (m_formdbMgr == null || m_formdbMgr.Login == null)
            {   // if login fails or haven't logged in yet
                bInitialCall = true;
                PreferencesHelper.PreferencesHelperInstance.InitializePrefsHelper(string.Empty, string.Empty);
            }
            else
            {
                PreferencesHelper.PreferencesHelperInstance.InitializePrefsHelper(m_formdbMgr.Login.UserName, m_formdbMgr.Login.Password);
            }
            try
            {
                if (bInitialCall && !string.IsNullOrEmpty(PreferencesHelper.PreferencesHelperInstance.InfraSettings))
                {
                    // there used to be code here to save settings to a member var (m_infraSource)
                    // and then overwrite the prefs settings with it.  Comment said "in case of dev changes" -- ? 
                    // Ripped out to fix CSBR-122224.
                    this.mainFormUltraToolbarsManager.LoadFromXml(CBVUtil.StringToStream(PreferencesHelper.PreferencesHelperInstance.InfraSettings));
                    this.RemoveNonStandardMenus();  // rip out all but File/Edit/View/Help
                }
            }
            catch (Exception ex)
            {
                // CSBR-134061-related: on failure to load Infra settings, don't throw here, it causes app to abort.
                // But let's ask the user.  Do this only the first time.

                // throw new UICustomizationException("Cannot load user controls customizations", ex);
                String sMsg1 = String.Format("The following error was encountered in loading toolbar settings:\n\n{0}\n\n", ex.Message);
                String sMsg2 = String.Format("Click OK to reset toolbars and continue, or Cancel to exit the program.");

                if (MessageBox.Show(sMsg1 + sMsg2, "Reset Toolbars", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    throw new UICustomizationException("");     // blank message prevents second alert
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///   Configures the main form
        /// </summary>
        private void ConfigureMainForm()
        {
            //CSBR-114334: load window state and size
            if (Properties.Settings.Default.MainWindowMaximized == true)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else if (Properties.Settings.Default.MainFormHeight > 0 && Properties.Settings.Default.MainFormWidth > 0)
            {
                this.Visible = false;   // avoid flash
                this.WindowState = FormWindowState.Normal;
                this.Height = Properties.Settings.Default.MainFormHeight;
                this.Width = Properties.Settings.Default.MainFormWidth;
                this.Left = Properties.Settings.Default.MainFormLeft;
                this.Top = Properties.Settings.Default.MainFormTop;

                if (this.IsChildForm || this.IsChildDocForm)
                {
                    this.Left += 500;
                    this.Top += 50;
                    this.Width -= 300;
                }
            }
            else  // CSBR-115293: Bad initial window position
            {
                this.CenterToParent();
            }

            this.ConfigureSettingsOnLoading();
            // If the user had selected an certain style, apply it. 
            if (!string.IsNullOrEmpty(PreferencesHelper.PreferencesHelperInstance.StyleLibrary))
                PreferencesHelper.PreferencesHelperInstance.LoadStyle(PreferencesHelper.PreferencesHelperInstance.StyleLibrary);

            this.ShowSearchButtons(false);
            this.SetNavigationPane();
            this.KeyDown += new KeyEventHandler(ChemBioVizForm_KeyDown);

            this.TabControl.TabMoved += new TabMovedEventHandler(TabControl_TabMoved);

            if (!string.Equals(PreferencesHelper.PreferencesHelperInstance.OpenMode, CBVConstants.OPEN_BLANK_FORM, StringComparison.InvariantCultureIgnoreCase))
            {
                this.EnableMenu(true);
            }
            else
                this.ShowToolbar(CBVConstants.TOOLBAR_NAVIGATOR, false); //Hide the Record Navigator

            ChemDataGrid.bgAllowGridTooltips = ChemBioViz.NET.Properties.Settings.Default.ShowTooltips; // CSBR-132730
            m_formdbMgr.FilterChildHits = Properties.Settings.Default.FilterChildHits;
            FormDbMgr.ShowSSSHilites = Properties.Settings.Default.ShowSSSHilites;

            this.SizeChanged += new EventHandler(ChemBioVizForm_SizeChanged);
        }
        //---------------------------------------------------------------------
        void TabControl_TabMoved(object sender, TabMovedEventArgs e)
        {
            // no-op: we no longer attempt to keep our array in the same order as the visible tabs
            this.Modified = true;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Sets the application title with the selected form name
        /// </summary>
        private void SetApplicationTitle()
        {
            StringBuilder title = new StringBuilder(CBVConstants.APP_NAME);
            title.Append(" - ");
            if (m_formdbMgr.Login != null)
                title.Append(m_formdbMgr.Login.CurrentMRU.DisplayName); // CSBR-112905
            if (!string.IsNullOrEmpty(FormName))
            {
                title.Append(" - ");
                title.Append(FormName);
                string visibility = IsLocal ? CBVConstants.LOCAL_FORM :
                                   IsPublic ? CBVConstants.PUBLIC_FORM : CBVConstants.PRIVATE_FORM;
                title.Append(visibility);
            }
            this.Text = title.ToString();
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Append info to the title bar
        /// </summary>
        /// <param name="newtext"></param>
        private void AppendToAppTitle(string newtext, bool append)
        {
            StringBuilder title = new StringBuilder();
            if (!append)
            {
                title.Append(CBVConstants.APP_NAME);
                title.Append(" - ");
                if (m_formdbMgr.Login != null)
                    title.Append(m_formdbMgr.Login.CurrentMRU.DisplayName);

                title.Append(" - ");
                title.Append(newtext);
                this.Text = title.ToString();
            }
            else
            {
                Debug.WriteLine(String.Concat("NOT appending to title: ", newtext));
                return; // avoid appending query info to title

                title.Append(" ");
                title.Append(newtext);
                this.Text += title.ToString();
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Clears most of the app title. It only keeps the session info
        /// </summary>
        private void ClearAppTitle()
        {
            StringBuilder title = new StringBuilder(CBVConstants.APP_NAME);
            title.Append(" - ");
            if (m_formdbMgr.Login != null)
                title.Append(m_formdbMgr.Login.CurrentMRU.DisplayName);
            this.Text = title.ToString();
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Append logged-in username to User Forms title
        /// </summary>
        /// <param name="group"></param>
        private void SetGroupText(UltraExplorerBarGroup group)
        {
            if (group != null && string.Equals(group.Key, CBVConstants.PRIVATE_GROUPNAME))
                group.Text = String.Concat(group.Key, " [", m_formdbMgr.Login.UserName, "]");
        }
        //---------------------------------------------------------------------
        public void ShowSearchButtons(bool bShow)
        {
            // called when changing tabs; show buttons on query views
            cancelUltraButton.Visible = bShow;
            searchUltraButton.Visible = bShow;
            clearUltraButton.Visible = bShow;   // CSBR-133431
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Creates an empty form and cleans any title which contains previous opened form name
        /// </summary>
        private void CreateEmptyForm()
        {
            if (m_tabManager != null)
                m_tabManager.Clear();

            m_tabManager = new TabManager(this);
            m_tabManager.Create3TBlank();	// standard 3 tabs: form, table, query

            Application.DoEvents();

            if (m_queries != null)
                m_queries.Clear();
            Modified = false;

            this.ClearAppTitle();
            this.SetPaneTitle(string.Empty, CBVConstants.EXPLORER_PANE_NAME);
            this.FormName = string.Empty;
            CBVStatMessage.ShowReadyMsg();
        }
        //---------------------------------------------------------------------
        public bool IsFormInPublicBank()
        {
            String formName = this.FormName;
            List<String> names = m_formdbMgr.PublicFormBank.GetNameList();
            foreach (String name in names)
                if (CBVUtil.Eqstrs(name, formName))
                    return true;
            return false;
        }
        //---------------------------------------------------------------------
        private bool LoadForm(String sXml, String formName, int formID)
        {
            initialSXML = sXml; // storing a copy
            // main form-reading routine for open from dialog or form tree
            // convert string to xml document, read, create form
            try
            {
                if (String.IsNullOrEmpty(sXml))
                    throw new Exception("Empty form");

#if DEBUG
                bool bDumpForm = false;
                if (bDumpForm)
                {
                    String fname1 = "C:\\form_in.xml";
                    CBVUtil.StringToFile(sXml, fname1, Encoding.Unicode);    // UTF-8 creates file unreadable by IE
                }
#endif
                CBVStatMessage.StatMessageEvent += new EventHandler<CBVEventArgs>(statMsg_event);
                FormName = formName;
                m_formID = formID;
                CBVStatMessage.Show("Opening form " + formName);
                Application.DoEvents();

                XmlDocument xdoc = new XmlDocument();
                XmlTextReader xmlReader = new XmlTextReader(new StringReader(sXml));
                xmlReader.MoveToContent();
                XmlNode root = xdoc.ReadNode(xmlReader);

                //#if DEBUG // do this in release mode so Megean can use it
                if (CBVUtil.Eqstrs(root.Name, "configuration") && root.ChildNodes.Count > 0)
                {
                    // this is a config loader block; form is inside cdata
                    foreach (XmlNode formNode in root.ChildNodes)
                    {
                        if (formNode != null && CBVUtil.Eqstrs(formNode.Name, "xml"))
                        {
                            FormName = CBVUtil.GetStrAttrib(root, "name");
                            int storedID = CBVUtil.GetIntAttrib(root, "id");

                            String formXml = formNode.InnerText;
                            XmlDocument xdocF = new XmlDocument();
                            XmlTextReader xmlReaderF = new XmlTextReader(new StringReader(formXml));
                            xmlReaderF.MoveToContent();
                            root = xdocF.ReadNode(xmlReaderF);
                            break;
                        }
                    }
                }
                //#endif
                // tab manager reads the file and creates views
                if (m_tabManager != null)
                    m_tabManager.Clear();
                LoadFromXmlEx(root);

                // form must contain connection info; if old-style, convert
                if (m_appID == 0 && m_tableID == 0)
                {
                    if (CBVUtil.StrEmpty(m_appName) || CBVUtil.StrEmpty(m_tableName))
                        throw new FormDBLib.Exceptions.ObjectBankException("Form does not contain connection data");
                    FormDbMgr.Select(m_appName, m_tableName, true);
                    if (FormDbMgr.SelectedDataView == null)
                        throw new FormDBLib.Exceptions.ObjectBankException("Form contains invalid connection data");

                    // tell user the form is old .. assign id's here and suggest saving
                    MessageBox.Show("This form is in an older format.  To update, save the form.");
                    m_appID = FormDbMgr.SelectedDataView.DataViewID;
                    if (FormDbMgr.SelectedDataViewTable != null)
                        m_tableID = FormDbMgr.SelectedDataViewTable.Id;
                }
                // select dataview and table by id
                Debug.Assert(m_appID > 0 && m_tableID > 0);
                bool bOKApp = FormDbMgr.SelectDataViewByID(m_appID);
                if (!bOKApp || FormDbMgr.SelectedDataView == null)
                {
                    String msg = String.Format("Form refers to unknown dataview {0} -- may have been created for a different database.",
                        m_appID.ToString());
                    throw new FormDBLib.Exceptions.ObjectBankException(msg);
                }
                bool bOKTable = FormDbMgr.SelectDataViewTableByID(m_tableID);
                if (!bOKTable || FormDbMgr.SelectedDataViewTable == null)
                    throw new FormDBLib.Exceptions.ObjectBankException("Form refers to unknown table " + m_tableID.ToString());

                // now we have the base table count; tell the RetrieveAll query
                m_queries.ResetRAQCount();

                // create the rc from form, not dataview
                m_formdbMgr.ResultsCriteria = FormUtil.FormToResultsCriteria(this);
                bool bIsRCValid = IsRCValid(m_formdbMgr.ResultsCriteria);
                if (!bIsRCValid)
                {
                    String errMsg = String.Format("Form is defective: may refer to invalid table or table with no columns.  Open anyway?");
                    if (MessageBox.Show(errMsg, "RC Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                        throw new FormDBLib.Exceptions.ObjectBankException("");
                }
                m_formdbMgr.BaseTableRecordCount = -1;	// recount when needed
                Application.DoEvents();

#if DEBUG
                String fname = "C:\\rc_out.xml";
                String sXmlDump = m_formdbMgr.ResultsCriteria.ToString();
                CBVUtil.StringToFile(sXmlDump, fname, Encoding.Unicode);    // UTF-8 creates file unreadable by IE
#endif
                // open the database, run startup query, show the form
                Query query = (m_cmdLine == null) ? null :
                    m_cmdLine.GetStartupQuery(m_formdbMgr, m_queries);      // shows alert if not found

                if (query == null && bIsRCValid)                            // CSBR-117699: if invalid RC, leave query null, don't run
                    query = m_queries.RetrieveAllQuery;

                GenerateOrOpenForm(sXml, false, query, true);	// f => do not generate, just show; t => with subforms
                ActivateFormInPane();
                ActivatePlotInPane(null);

                if (HasActionMenuButtons())
                    CreateActionsMenus();

                Modified = false;
                Application.DoEvents();
                FireFormOpened();
            }
            catch (FormDBLib.Exceptions.ObjectBankException ex)
            {
                UnloadForm();
                if (!String.IsNullOrEmpty(ex.Message))
                    CBVUtil.ReportError(ex, "Form load error. ");
            }
            finally
            {
                CBVStatMessage.ShowReadyMsg();
            }
            return true;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Load a form from its given path
        /// </summary>
        /// <param name="fullPath"></param>
        public void LoadFormByPath(String fullPath)
        {
            // for now: fullPath must begin with Public Forms\ or My Forms\
            string formName = string.Empty;
            int formID = -1;
            bool bPublic = false;

            if (CBVUtil.StrEmpty(fullPath))
                throw new Exception("Empty form name");

            string[] path = fullPath.Split(new string[] { "\\", "/" }, StringSplitOptions.None);
            ITreeNode node = m_FTreeConf.MainTree.GetNodeByPath(path);
            // user provided form name only
            if (node == null && path.Length == 1)
                formName = fullPath;    // in which case it doesn't make sense to check path[0] below
            //else
            //    return;   ??? this prevents opening a valid pathname [CSBR-129260]

            ObjectBank bank = null;
            String sXml = string.Empty;
            if (path[0].Equals(CBVConstants.PUBLIC_GROUPNAME) || path[0].Equals(CBVConstants.PRIVATE_GROUPNAME))
            {
                bank = path[0].Equals(CBVConstants.PUBLIC_GROUPNAME) ? m_formdbMgr.PublicFormBank : m_formdbMgr.PrivateFormBank;
                bPublic = path[0].Equals(CBVConstants.PUBLIC_GROUPNAME);
                if (node != null && node is TreeLeaf)
                {
                    formName = node.Name;
                    formID = ((TreeLeaf)node).Id;
                }
            }
            else
            {
                if (m_formdbMgr.PublicFormBank.HasName(formName))
                {
                    bank = m_formdbMgr.PublicFormBank;
                    bPublic = true;
                }
                else if (m_formdbMgr.PrivateFormBank.HasName(formName))
                    bank = m_formdbMgr.PrivateFormBank;
                else
                    throw new FormDBLib.Exceptions.ObjectBankException(string.Format("Form {0} not found.", formName));
                if (bank != null)
                    formID = bank.RetrieveID(formName);
            }
            if (formID == -1 && String.IsNullOrEmpty(formName))
                return;

            //Coverity Bug Fix CID 12960 
            sXml = bank != null ? bank.Retrieve(formID) : string.Empty;

            m_formID = formID;
            m_formName = formName;
            m_formType = bPublic ? formType.Public : formType.Private;
            m_localFilePath = string.Empty;

            LoadForm(sXml, formName, formID);
        }
        //---------------------------------------------------------------------
        public void LoadFormByID(int formID, formType fType)
        {
            bool bPublic = false;
            String sXml = string.Empty;
            if (formID > 0)
            {
                bPublic = fType == formType.Public;
                ObjectBank bank = bPublic ? m_formdbMgr.PublicFormBank : m_formdbMgr.PrivateFormBank;
                if (bank == null)
                    throw new FormDBLib.Exceptions.ObjectBankException("Form not found.");

                sXml = bank.Retrieve(formID);
                if (!string.IsNullOrEmpty(sXml))
                {
                    ITreeNode node = m_FTreeConf.MainTree.GetLeafFromListById(formID);
                    if (node != null)
                    {
                        m_formName = node.Name;
                        m_formID = formID;
                        m_formType = bPublic ? formType.Public : formType.Private;
                        m_localFilePath = string.Empty;
                        LoadForm(sXml, node.Name, formID);
                    }
                }
                else
                {
                    m_formID = -1;
                    m_formName = string.Empty;
                    CBVStatMessage.Hide();
                    RefreshBindingNavigator();
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Form is closing: remove global info about it
        /// </summary>
        public void UnloadForm()
        {
            m_localFilePath = string.Empty;
            m_appName = string.Empty;
            m_tableName = string.Empty;
            m_appID = m_tableID = 0;
            m_formdbMgr.UnselectDataView();

            FormName = string.Empty;
            Modified = false;
            this.ClearAppTitle();
            this.SetPaneTitle(string.Empty, CBVConstants.EXPLORER_PANE_NAME);

            DeactivateTreeNode();

            CurrQuery = null;
            BindingSource = null;

            // unhook binding navigator
            if (BindingNavigator != null)
                BindingNavigator.BindingSource = null;

            // Remove remaining query stuff
            m_QTreeConf = new QueriesTreeConfig();
            // Clear previous tree 
            ClearTree(CBVConstants.QUERIES_GROUPNAME);
            if (m_queries != null)
            {
                m_queries.Clear();
                m_queries.QueryOnOpen = 0;
            }

            CBVStatMessage.StatMessageEvent -= statMsg_event;
            DeactivatePlotInPane();
            this.RemoveNonStandardMenus(); // CSBR-152348, removes all menu items except standard (File, Edit, View, Help)

            FireCBVFormClosed();
        }
        //---------------------------------------------------------------------
        private bool IsRCValid(ResultsCriteria rc)
        {
            // checks whether rc has tables and criterias
            // TO DO: go further and test whether all are valid
            if (rc.Tables.Count == 0) return false;
            for (int i = 0; i < rc.Tables.Count; ++i)
                if (rc.Tables[i].Criterias.Count == 0) return false;
            return true;
        }
        //---------------------------------------------------------------------

        #region Toolbars methods
        /// <summary>
        ///  Show/ Hide the toolbar <paramref name="toolbar"/>
        /// </summary>
        /// <param name="toolbar"></param>
        /// <param name="visible"></param>
        private void ShowToolbar(string toolbar, bool visible)
        {
            mainFormUltraToolbarsManager.Toolbars[toolbar].Visible = visible;
        }
        #endregion

        #region Tree methods
        /// <summary>
        ///  Create tree control in group <paramref name="groupName"/> 
        ///  Does not fill the tree
        /// </summary>
        /// <param name="groupName">The group where the tree is created</param>
        /// <returns></returns>
        private UltraTree CreatePanelTree(String groupName)
        {
            UltraExplorerBarGroup group = ultraExplorerBar1.Groups[groupName];
            UltraExplorerBarContainerControl uBarContainer = group.Container;
            string treeKey = string.Empty;
            switch (groupName)
            {
                case CBVConstants.PUBLIC_GROUPNAME: treeKey = CBVConstants.PUBLICFORMS_TREE; break;
                case CBVConstants.PRIVATE_GROUPNAME: treeKey = CBVConstants.USERFORMS_TREE; break;
                case CBVConstants.QUERIES_GROUPNAME: treeKey = CBVConstants.QUERIES_TREE; break;
                case CBVConstants.DATAVIEWS_GROUPNAME: treeKey = CBVConstants.DATAVIEWS_TREE; break;
            }
            UltraTree tree = this.ConfigureTree(treeKey);
            if (uBarContainer != null)
            {
                uBarContainer.Controls.Clear();
                tree.Size = uBarContainer.Size;
                tree.HideSelection = false;
                uBarContainer.Controls.Add(tree);
            }
            // Enable node edition
            tree.ViewStyle = Infragistics.Win.UltraWinTree.ViewStyle.Standard;
            if (treeKey != CBVConstants.DATAVIEWS_TREE)    // CSBR-155503:To avoid renaming the Dataviews and tables under it
                tree.Override.LabelEdit = DefaultableBoolean.True;
            return tree;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Create a tree and configure it.
        /// </summary>
        /// <param name="treeKey"></param>
        /// <returns></returns>
        private UltraTree ConfigureTree(string treeKey)
        {
            UltraTree treeView = new UltraTree();
            treeView.Appearance.Key = treeKey;
            treeView.DisplayStyle = UltraTreeDisplayStyle.WindowsVista;
            treeView.ExpansionIndicatorColor = Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            treeView.ImageTransparentColor = Color.Transparent;
            treeView.NodeConnectorColor = SystemColors.ControlDark;
            treeView.Dock = DockStyle.Fill;

            return treeView;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Create plot control panel in explorer bar
        /// </summary>
        private void CreatePlotControlPane()
        {
            //if (m_plotControls == null)
            //{
            UltraExplorerBarGroup group = ultraExplorerBar1.Groups[CBVConstants.PLOT_GROUPNAME];
            UltraExplorerBarContainerControl uBarContainer = group.Container;

            CBVChartPanel plotControls = ChartController.PlotControls;
            //m_plotControls = new CBVChartPanel(this);
            plotControls.Dock = DockStyle.Fill;
            uBarContainer.Controls.Clear();
            plotControls.Size = uBarContainer.Size;
            uBarContainer.Controls.Add(plotControls);
            plotControls.Visible = true;
            //}
        }
        private void RemovePlotControlPane()
        {
            UltraExplorerBarGroup group = ultraExplorerBar1.Groups[CBVConstants.PLOT_GROUPNAME];
            if (group != null)
                ultraExplorerBar1.Groups.Remove(group);
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Populate the Form Panel <paramref name="groupName"/>. 
        ///  It could be a public or a private panels. 
        ///  It depends on <paramref name="bPublic"/>
        /// </summary>
        /// <param name="groupName">Group which contains the tree</param>
        /// <param name="bPublic">Are the forms public?</param>
        /// <param name="title">Group title</param>
        private void FormsToPanel(String groupName, bool bPublic)
        {
            // get list of forms from appropriate bank, add to explorer bar group
            UltraExplorerBarGroup gFormPane = ultraExplorerBar1.Groups[groupName];
            if (gFormPane == null) return;

            gFormPane.Settings.AllowDrag = DefaultableBoolean.False;    // CSBR-111320
            gFormPane.Settings.AllowItemDrop = DefaultableBoolean.False;

            this.SetGroupText(gFormPane);
            this.FormsToTree(groupName, bPublic);
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Populate the tree created inside the panel <paramref name="groupName"/> 
        ///  with the forms list <paramref name="formNames"/>
        /// </summary>
        /// <param name="groupName">Group which contains the tree</param>
        /// <param name="bPublic">Are the forms public?</param>
        /// <param name="formNames">Form list to be added</param>
        private void FormsToTree(string groupName, bool bPublic)
        {
            DbObjectBank bank = bPublic ? m_formdbMgr.PublicFormBank : m_formdbMgr.PrivateFormBank;

            // Get the tree structure from the settings
            m_FTreeConf.DeserializeTreeFromXML(groupName);
            // If the structure is not mapped on the settings this loads a basic one. 
            // For back compatibility, if any stored db form has not xml mapping, then add them to the list.
            TreeNode tStructure = m_FTreeConf.VerifyDBAgainstListNodes(groupName, bank.GetIdValuePairList(), null);
            // If there is a structure for the current group tree
            if (tStructure != null)
            {
                // Verify that all nodes exist on the db
                m_FTreeConf.VerifyListAgainstDBNodes(groupName, bank.GetIdValuePairList());
                // Create the pane
                UltraTree tree = CreatePanelTree(groupName);
                // Build the ultraTree
                tree = m_FTreeConf.BuildUITree(groupName, tStructure, tree);
                // Add context menu to each node
                for (int i = 0; i < tree.Nodes.Count; i++)
                {
                    tree.Nodes[i].Control.ContextMenuStrip = FormContextMenuStrip;
                    tree.Nodes[i].Control.AllowDrop = true;
                }

                tree.DoubleClick += new EventHandler(formTree_DblClick);  // Check which behavior it'll have for folders
                tree.KeyDown += new KeyEventHandler(tree_KeyDown);

                tree.AllowDrop = true;
                tree.HideSelection = false; //nodes' selected state is depicted even when the control does not have focus.

                tree.MouseDown += new MouseEventHandler(tree_MouseDown);
                tree.MouseMove += new MouseEventHandler(tree_MouseMove);
                tree.DragOver += new DragEventHandler(tree_DragOver);
                tree.DragDrop += new DragEventHandler(FormTree_DragDrop);

                tree.MouseEnterElement += new UIElementEventHandler(tree_MouseEnterElement);

                tree.BeforeLabelEdit += new BeforeNodeChangedEventHandler(tree_BeforeLabelEdit);
                tree.AfterLabelEdit += new AfterNodeChangedEventHandler(tree_AfterLabelEdit);
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Updates node text
        /// </summary>
        private void UpdateNodeInPanel(UltraTreeNode sUINode, string newName)
        {
            sUINode.Text = newName;
        }
        //---------------------------------------------------------------------
        private static int CompareQueries(Query q1, Query q2)
        {
            String q1Name = q1.Name, q2Name = q2.Name;
            if (q1Name.StartsWith("Q") && q2Name.StartsWith("Q"))
            {
                // if comparing Qn to Qm, compare only the numeric parts
                int nSuffix1 = CBVUtil.ParseLetterNumStr(q1Name);
                int nSuffix2 = CBVUtil.ParseLetterNumStr(q2Name);
                if (nSuffix1 > 0 && nSuffix2 > 0)
                    return (nSuffix1 < nSuffix2) ? -1 : (nSuffix1 == nSuffix2) ? 0 : 1;
            }
            return q1Name.CompareTo(q2Name);
        }
        //---------------------------------------------------------------------
        private List<Query> SortQueries(QueryCollection queries)
        {
            List<Query> qlist = new List<Query>();
            foreach (Query q in queries)
                qlist.Add(q);
            qlist.Sort(CompareQueries);
            return qlist;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Creates and populates the tree which will contain the queries
        /// </summary>
        private void QueriesToTree(string sXml)
        {
            string groupName = CBVConstants.QUERIES_GROUPNAME;
            TreeNode tStructure = null;
            Dictionary<int, string> sortedDictionary = null;

            // try/catch -- GetIdValuePairList fails if incoming id's contain duplicates
            //Coverity Bug Fix Local Analysis
            if (m_QTreeConf != null)
            {
                try
                {
                    // Deserialize the Qtree structure from the <queries/> tag inside Form xml
                    m_QTreeConf.DeserializeTreeFromXML(groupName, sXml);
                    sortedDictionary = CBVUtil.SortDictionary(m_QTreeConf.GetIdValuePairList(m_queries));
                    tStructure = m_QTreeConf.VerifyDBAgainstListNodes(groupName, sortedDictionary, m_queries);
                }
                catch (Exception)
                {
                    tStructure = null;
                }

                // If there is a structure for the current group tree
                if (tStructure != null)
                {
                    // Verify that all nodes exist on the db
                    m_QTreeConf.VerifyListAgainstDBNodes(groupName, sortedDictionary);
                    // Complete info
                    if (tStructure.Nodes.Count > 0)
                        m_QTreeConf.AddComments(m_queries, m_QTreeConf.MainTree.GetNodeFromListByName(CBVConstants.QUERIES_GROUPNAME));
                }
                else
                    tStructure = m_QTreeConf.CreateRoot(CBVConstants.QUERIES_GROUPNAME);

                // Create the pane
                UltraTree tree = CreatePanelTree(groupName);
                //Coverity Bug Fix local Analysis
                if (tree != null)
                {
                    // Build the ultraTree; use Ex method to handle queries beneath queries
                    tree = m_QTreeConf.BuildUITreeEx(groupName, tStructure, tree);

                    // add child queries if any
                    AddChildQueriesToTree(tree, m_QTreeConf);

                    // Add context menu to each node
                    for (int i = 0; i < tree.Nodes.Count; i++)
                    {
                        tree.Nodes[i].Control.ContextMenuStrip = queryContextMenuStrip;
                        tree.Nodes[i].Control.AllowDrop = true;
                    }

                    tree.DoubleClick += new EventHandler(queryTree_DoubleClick);  // Check which behavior it'll have for folders
                    tree.KeyDown += new KeyEventHandler(tree_KeyDown);

                    tree.AllowDrop = true;
                    tree.HideSelection = false; //nodes' selected state is depicted even when the control does not have focus.

                    tree.MouseDown += new MouseEventHandler(tree_MouseDown);
                    tree.MouseMove += new MouseEventHandler(tree_MouseMove);
                    tree.DragOver += new DragEventHandler(tree_DragOver);

                    tree.DragDrop += new DragEventHandler(queryTree_DragDrop);
                    tree.MouseEnterElement += new UIElementEventHandler(tree_MouseEnterElement);

                    tree.BeforeLabelEdit += new BeforeNodeChangedEventHandler(tree_BeforeLabelEdit);
                    tree.AfterLabelEdit += new AfterNodeChangedEventHandler(tree_AfterLabelEdit);
                }
            }

        }
        //---------------------------------------------------------------------
        private void ClearTree(string groupName)
        {
            UltraTree tree = GetTreeFromGroup(groupName);
            if (tree != null && tree.Nodes != null)
                tree.Nodes.Clear();
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Highlights the form on its pane; activates the appropriate pane; for public or private db forms only
        /// </summary>
        private void ActivateFormInPane()
        {
            if (!String.IsNullOrEmpty(FormName))
            {
                // determine group to be activated
                String groupName = this.IsPublic ? CBVConstants.PUBLIC_GROUPNAME :
                                    this.IsPrivate ? CBVConstants.PRIVATE_GROUPNAME : string.Empty;
                if (!String.IsNullOrEmpty(groupName))
                {
                    if (m_formID > 0)
                    {
                        //Coverity Bug fix CID 13089 
                        if (m_FTreeConf != null && m_FTreeConf.MainTree != null)
                        {
                            ITreeNode leafNode = m_FTreeConf.MainTree.GetLeafFromListById(m_formID);
                            if(leafNode != null)
                            {
                                ActivateTreeNode(groupName, leafNode.Key);
                                SetTitles(groupName,
                                    String.Concat(groupName, " - ", FormName, " (opened)"), FormName + (Modified ? "*" : string.Empty));
                            }
                        }
                    }
                }
            }
            else
                this.ClearAppTitle();
        }
        //---------------------------------------------------------------------
        public void ActivatePlotInPane(CBVChartControl chartControl)
        {
            // activate given plot; if null, find first in form and activate that
            if (chartControl == null && TabManager.CurrentTab is FormViewTab)
            {
                FormViewControl fvc = TabManager.CurrentTab.Control as FormViewControl;
                //Coverity Bug Fix : CID :12953 
                if (fvc != null)
                    chartControl = fvc.FindPlot();
            }
            this.ChartController.SelectedPlot = chartControl;
        }
        //---------------------------------------------------------------------
        private void DeactivatePlotInPane()
        {
            this.ChartController.SelectedPlot = null;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Highlights a certain query on the Queries tree
        /// </summary>
        /// <param name="q"></param>
        protected void ActivateQueryOnTree(Query q)
        {
            ITreeNode inode = (q == null) ? null : m_QTreeConf.MainTree.GetLeafFromListById(q.ID);
            if (inode != null)
                this.ActivateTreeNode(CBVConstants.QUERIES_GROUPNAME, inode.Key);
            else
                this.ActivateTreeNode(CBVConstants.QUERIES_GROUPNAME, "");
        }
        //---------------------------------------------------------------------
        #endregion

        #region Menu methods
        /// <summary>
        ///   Enable/Disable all menu items. Check if the user is inside Form Editor 
        /// </summary>
        /// <param name="enable"></param>
        private void EnableMenu(bool enable)
        {
            //  File Menu
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, enable);
            EnableSubMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, CBVConstants.MENU_ITEM_EXPORT_SD, enable);
            EnableSubMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, CBVConstants.MENU_ITEM_EXPORT_DELIM, enable);
            EnableSubMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, CBVConstants.MENU_ITEM_EXPORT_EXCEL, enable);
            EnableSubMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, CBVConstants.MENU_ITEM_EXPORT_CBVEXCEL, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_CLOSE_FORM, enable);

            // Edit Menu
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_EDIT, CBVConstants.MENU_ITEM_SORT, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_EDIT, CBVConstants.MENU_ITEM_EDIT_FORM, enable);

            // View Menu
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_VIEW, CBVConstants.MENU_ITEM_TOOLBOX, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_VIEW, CBVConstants.MENU_ITEM_PROPERTIES, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_VIEW, CBVConstants.MENU_ITEM_EXPLORER, !enable);    // this one is special
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_VIEW, CBVConstants.MENU_ITEM_REFRESH, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_VIEW, CBVConstants.MENU_ITEM_VIEWTOOLTIPS, true);   // always on, even if no form
            CheckMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_VIEW, CBVConstants.MENU_ITEM_VIEWTOOLTIPS,
                            Properties.Settings.Default.ShowTooltips);

            // other Edit commands -- enabled only during form edit
            if (IsEditingForm())
                this.EnableMenusOnEdition(true);
            else
                this.EnableMenusOnEdition(false);
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///   Enable/Disable Edit Menu. Mostly used for validating whether the user is on Form Editor
        /// </summary>
        /// <param name="enable"></param>
        private void EnableMenusOnEdition(bool enable)
        {
            //  Edit menu items -- available during form editing, otherwise not
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_EDIT, CBVConstants.MENU_ITEM_SELECT_ALL, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_EDIT, CBVConstants.MENU_ITEM_CUT, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_EDIT, CBVConstants.MENU_ITEM_COPY, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_EDIT, CBVConstants.MENU_ITEM_PASTE, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_EDIT, CBVConstants.MENU_ITEM_UNDO, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_EDIT, CBVConstants.MENU_ITEM_REDO, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_EDIT, CBVConstants.MENU_ITEM_EDIT_FORM, !enable);

            // View menu items -- all except Explorer available during form editing, otherwise vice-versa
            // CSBR-134572: restore this paragraph; was disabled for a while
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_VIEW, CBVConstants.MENU_ITEM_TOOLBOX, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_VIEW, CBVConstants.MENU_ITEM_PROPERTIES, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_VIEW, CBVConstants.MENU_ITEM_EXPLORER, !enable);
        }
        /// <summary>
        ///  Enable/Disable Save Menu. Mostly used for validating whether the user selected a form or Not.
        /// </summary>
        /// <param name="enable"></param>
        private void EnableMenusOnSave(bool enable)
        {
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_SAVE, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_SAVE_AS, enable);
            EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_SAVE_LOCAL, enable);
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Enable/Disable menu item from main menu <paramref name="toolbar"/> - Menu <paramref name="menu"/> - Item <paramref name="item"/> 
        ///  depending on <paramref name="enable"/>
        /// </summary>
        /// <param name="toolbar"></param>
        /// <param name="menu"></param>
        /// <param name="item"></param>
        /// <param name="enable"></param>
        public void EnableMenuItem(string toolbar, string menu, string item, bool enable)
        {
            try
            {    // try/catch in case of old infragistics settings -- menu item may not be listed there
                ((PopupMenuTool)mainFormUltraToolbarsManager.Toolbars[toolbar].Tools[menu]).Tools[item].SharedProps.Enabled = enable;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EXCEPTION in EnableMenuItem: " + ex.Message);
                // TO DO: in this case, set a flag to cause infra settings to be wiped out before next session, say on app exit
            }
        }
        //---------------------------------------------------------------------
        public void CheckMenuItem(string toolbar, string menu, string item, bool bChecked)
        {
            try
            {
                StateButtonTool sButton = ((PopupMenuTool)mainFormUltraToolbarsManager.Toolbars[toolbar].Tools[menu]).Tools[item] as StateButtonTool;
                if (sButton != null)
                {
                    sButton.MenuDisplayStyle = StateButtonMenuDisplayStyle.DisplayCheckmark;
                    sButton.InitializeChecked(bChecked);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EXCEPTION in CheckMenuItem: " + ex.Message);
            }
        }
        //---------------------------------------------------------------------
        public void UpdateAddinMenu(string addinMenuTitle)
        {
            CBVAddin addin = m_addinMgr.FindByMenuTitle(addinMenuTitle);
            if (addin != null)
                addin.UpdateMenuImage();
        }
        //---------------------------------------------------------------------
        public void UpdateMenuTitle(string toolbar, string menu, string newText, Image newImage)
        {
            try
            {
                ToolsCollection tools = mainFormUltraToolbarsManager.Toolbars[toolbar].Tools;
                int index = tools.IndexOf(menu);
                PopupMenuTool tool = (PopupMenuTool)tools[menu];

                tools.Remove(tool);
                if (!String.IsNullOrEmpty(newText))
                {
                    tool.CustomizedCaption = newText;
                    tool.CustomizedDisplayStyle = ToolDisplayStyle.Default;
                }
                tool.CustomizedImage = newImage;
                tool.InstanceProps.AppearancesSmall.Appearance.Image = newImage;
                if (newImage != null)
                {
                    tool.CustomizedDisplayStyle = ToolDisplayStyle.ImageAndText;
                }
                tools.Insert(index, tool);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EXCEPTION in UpdateMenuTitle: " + ex.Message);
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Enable/Disable menu item from main menu <paramref name="toolbar"/> - Menu <paramref name="menu"/> - Item <paramref name="item"/> - SubItem <paramref name="subMenuItem"/>
        ///  depending on <paramref name="enable"/>
        /// </summary>
        /// <param name="toolbar"></param>
        /// <param name="menu"></param>
        /// <param name="item"></param>
        /// <param name="subMenuItem"></param>
        /// <param name="enable"></param>
        private void EnableSubMenuItem(string toolbar, string menu, string item, string subMenuItem, bool enable)
        {
            try
            {
                ToolsCollection tools = ((PopupMenuTool)((PopupMenuTool)mainFormUltraToolbarsManager.Toolbars[toolbar].Tools[menu]).Tools[item]).Tools;
                if (tools.IndexOf(subMenuItem) != -1)   // CSBR-152173: Contains didn't work as expected
                    tools[subMenuItem].SharedProps.Enabled = enable;
                else
                    Debug.WriteLine("Sub menu not found: " + subMenuItem);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EXCEPTION in EnableSubMenuItem: " + ex.Message);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        ///  Show/Hide a menu item from main menu <paramref name="toolbar"/> - Menu <paramref name="menu"/> - Item <paramref name="item"/>
        ///  depending on <paramref name="visible"/>
        /// </summary>
        /// <param name="toolbar"></param>
        /// <param name="menu"></param>
        /// <param name="item"></param>
        /// <param name="visible"></param>
        private void SetVisibleMenuItem(string toolbar, string menu, string item, bool visible)
        {
            try
            {
                ((PopupMenuTool)mainFormUltraToolbarsManager.Toolbars[toolbar].Tools[menu]).Tools[item].SharedProps.Visible = visible;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EXCEPTION in SetVisibleMenuItem: " + ex.Message);
            }
        }
        //------------------------------------------------------------------------------------------------
        private void SetVisibleSubMenuItem(string toolbar, string menu, string item, string subMenuItem, bool visible)
        {
            try
            {
                ToolsCollection tools = ((PopupMenuTool)((PopupMenuTool)mainFormUltraToolbarsManager.Toolbars[toolbar].Tools[menu]).Tools[item]).Tools;
                if (tools.IndexOf(subMenuItem) != -1)
                    tools[subMenuItem].SharedProps.Visible = visible;
                else
                    Debug.WriteLine("Sub menu not found: " + subMenuItem);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EXCEPTION in SetVisibleSubMenuItem: " + ex.Message);
            }
        }
        //---------------------------------------------------------------------
        private static ChemBioVizForm currForm;

        public static void ExportThreadProc()
        {
            DelimExporter exporter = new DelimExporter(currForm);
            if (exporter.Export())  // CSBR-134754: watch for false if error
                currForm.ExportThreadDone();
        }
        //---------------------------------------------------------------------
        private void ExportThreadDone()
        {
            String msg = String.Format("Export completed to file {0}\n\nOpen exported file?", currForm.ExportOpts.OutputPath);
            if (MessageBox.Show(msg, "File Ready", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (File.Exists(currForm.ExportOpts.OutputPath))
                    System.Diagnostics.Process.Start(currForm.ExportOpts.OutputPath);
            }
        }
        //---------------------------------------------------------------------
        private void ExportDelimMenuItemSelected()
        {
            currForm = this;
            BindingSource bsTmp = currForm.FullBindingSource;   // force retrieve
            int tableID = this.FormDbMgr.SelectedDataViewTable.Id;
            // Uncomment and comment below 2 lines to export only base table
            //ExportOpts.AllFieldNames = this.FormDbMgr.GetFieldList(false, false, false, tableID, true); 

            //bool bWithChild = true; bool bWithGrandchild = false;
            //ExportOpts.AllFieldNames = this.FormDbMgr.GetFieldNames(bWithChild, bWithGrandchild);
            // Retrieves the base table fields and child table fields if any 
            ExportOpts.AllFieldNames = this.FormDbMgr.GetDelimExportFieldNames();

            ExportDelimDialog dlg = new ExportDelimDialog(this);
            if (dlg.ShowDialog() == DialogResult.Cancel)
                return;

            ExportOpts = dlg.ExportOpts;
            Thread t = new Thread(new ThreadStart(ExportThreadProc));
            t.Start();
        }
        //---------------------------------------------------------------------
        /* CSBR-136190 Feature Request: It would be helpful if the Export status is shown while exporting to SDFile 
         * Passing the BackgroundWorker object as parameter in Export method
         * Changes Done by Jogi*/
        public static void ExportSDThreadProc(BackgroundWorker bgwOwner = null)
        {
            SDExporter exporter = new SDExporter(currForm);
            int? incompRecCnt = null;
            if (exporter.Export(ref incompRecCnt, bgwOwner))
                currForm.ExportSDThreadDone(ref incompRecCnt);
        }
        //---------------------------------------------------------------------
        private void DoImportToExcel(String sdFilename)
        {
            CBVExcelMgr excelMgr = new CBVExcelMgr();
            excelMgr.LoadSDFile(sdFilename);
        }
        //---------------------------------------------------------------------
        private void ExportSDThreadDone(ref int? incompRecCnt)  //Fixed CSBR-161147
        {
            if (currForm.ExportOpts.ForExcel)
            {
                DoImportToExcel(currForm.ExportOpts.OutputPath);
                currForm.ExportOpts.ForExcel = false;
            }
            else
            {
                String msg = string.Empty;
                //CSBR-161147
                if (incompRecCnt == null) //If variable is null then it exported all the data.
                {
                    msg = String.Format("Export completed to file {0}", currForm.ExportOpts.OutputPath);
                }
                else if (incompRecCnt.Value == 0) //If variable is 0 then it doesn't export any data.
                {
                    msg = String.Format("No records retrieved.");
                }
                else if (incompRecCnt.Value > 0) //If variable is 0 then it exported the retieved data till the cancellation of export progress bar.
                {
                    msg = String.Format("{0} records retrieved and exported to file {1}", incompRecCnt.Value, currForm.ExportOpts.OutputPath);
                }
                MessageBox.Show(msg, "File Ready");
            }

            ExportOpts.ForExcel = false; //CSBR-157067
        }
        //---------------------------------------------------------------------
        private void ExportExcelMenuItemSelected()
        {
            // export to a temporary SD
            String sTempFile = Path.GetTempFileName();  // created with .tmp extension; no need to keep
            ExportOpts.OutputPath = Path.ChangeExtension(sTempFile, "sdf");
            ExportOpts.ForExcel = true;

            ExportSDMenuItemSelected();

        }
        // CBV Excel Export
        //---------------------------------------------------------------------
        private void ExportCBVExcelMenuItemSelected()
        {
            Query q = this.CurrQuery;
            int recCount = (q == null) ? this.FormDbMgr.BaseTableRecordCount : q.NumHits;


            if (recCount > 0)
            {
                if (MessageBox.Show("Are you sure you want to create a CBV Excel sheet?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;

                SaveFileDialog dlg = new SaveFileDialog();

                dlg.InitialDirectory = Application.CommonAppDataPath;
                dlg.Filter = CBVConstants.CBVExcel_FILE_FILTERS;
                dlg.DefaultExt = ".xls";
                dlg.FileName = CBVConstants.DEFAULT_CBVExcel_FILE_NAME;

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    CBVExcelExportFilename = dlg.FileName;
                    currForm = this;
                    Thread t = new Thread(new ThreadStart(ExportCBVExcelThreadProc));
                    t.Start();
                }
            }
        }
        // CBV Excel Export
        //---------------------------------------------------------------------
        public static void ExportCBVExcelThreadProc()
        {
            CBVExcelExporter exporter = new CBVExcelExporter(currForm);

            string exportedResult = exporter.Export();
            if (exportedResult.Trim() != string.Empty)
                currForm.ExportCBVExcelThreadDone(exportedResult);

        }

        // CBV Excel Export
        //---------------------------------------------------------------------
        private void ExportCBVExcelThreadDone(string exportedResult)
        {
            try
            {
                /* CSBR-162413: Export to CBVExcel records can not be seen after exporting the same results initially in Chemdraw For Excel  */
                ProcessStartInfo startInfo = new ProcessStartInfo(); //Fixed CSBR-167551 and CSBR-164443
                startInfo.FileName = "Excel.EXE";
                Process.Start(startInfo);
                File.WriteAllText(currForm.CBVExcelExportFilename, exportedResult);

                String msg = String.Format("CBV Excel Export completed to file {0}\n\nOpen exported file?", currForm.CBVExcelExportFilename);

                if (MessageBox.Show(msg, "File Ready", MessageBoxButtons.YesNo) == DialogResult.Yes) //Fixed CSBR-153547
                {
                    if (File.Exists(currForm.CBVExcelExportFilename))
                    {
                        Process.Start(CBVExcelExportFilename);
                    }
                    else
                    {
                        throw new FileNotFoundException();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //---------------------------------------------------------------------
        private void ExportSDMenuItemSelected()
        {
            String sMsg = String.Empty;
            Query q = this.CurrQuery;
            int recCount = (q == null) ? this.FormDbMgr.BaseTableRecordCount : q.NumHits;

            // get confirmation if list is large
            int ARBITRARY_HIGH_NUMBER = 250;
            if (recCount > ARBITRARY_HIGH_NUMBER)
            {
                String sWarning = String.Format("You are about to export {0} records.  Are you sure?", recCount);
                if (MessageBox.Show(sWarning, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
            }
            if (recCount > 0)
            {
                // show dialog and get options
                // Use default sdf name if empty or if previously used output path was temporary
                if (m_exportOpts.OutputPath.Contains(Path.GetTempPath()) || String.IsNullOrEmpty(m_exportOpts.OutputPath))
                    m_exportOpts.OutputPath = Path.Combine(Application.CommonAppDataPath, CBVConstants.DEFAULT_SDF_FILE_NAME);

                BindingSource bsTmp = this.FullBindingSource;   // force retrieve
                int tableID = this.FormDbMgr.SelectedDataViewTable.Id;
                bool bWithChild = true, bWithGrandchild = false;
                ExportOpts.AllFieldNames = this.FormDbMgr.GetExportFieldNames(bWithChild, bWithGrandchild);

                ExportDialog dialog = new ExportDialog(this, m_exportOpts); // TO DO: modify dialog if exporting to Excel
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // do the export
                    ExportOpts = dialog.ExportOpts;
                    currForm = this;
                    // changes made for the feature request 
                    /* CSBR-136190 Feature Request: It would be helpful if the Export status is shown while exporting to SDFile
                     * Changes Done by Jogi
                     *  Using the Background worker thread to Asyncronously update the ProgressBar */

                    bgw = new BackgroundWorker();
                    bgw.WorkerSupportsCancellation = true;
                    bgw.WorkerReportsProgress = true;
                    bgw.DoWork += new DoWorkEventHandler(bw_DoWork);
                    bgw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                    bgw.RunWorkerAsync();
                    _exportProcess = new ExportProcess();
                    _exportProcess.Owner = this;
                    _exportProcess.Show();

                    /* End of CSBR-136190 */

                }
                else
                {
                    ExportOpts.ForExcel = false;
                }
            }
            if (!String.IsNullOrEmpty(sMsg))
                MessageBox.Show(sMsg);
        }
        //---------------------------------------------------------------------
        /* CSBR-136190 Feature Request: It would be helpful if the Export status is shown while exporting to SDFile
         * Events of BackgroundWorker Thread*/
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            ExportSDThreadProc(sender as BackgroundWorker);

        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            if (e.ProgressPercentage <= 100)
                _exportProcess.Progress = e.ProgressPercentage;
            if (e.UserState != null && e.UserState is string)
                _exportProcess.ProgressMessage = e.UserState.ToString();


        }

        /* End of CSBR-136190 */
        //---------------------------------------------------------------------
        private void OpenMenuItemSelected()
        {
            if (IsEditingForm()) EndFormEdit();
            if (!CheckForSaveOnClose())
                return;

            // open local saved form: prompt for filename, read and create form
            OpenFileDialog dlg = new OpenFileDialog();

            if (!String.IsNullOrEmpty(m_localFilePath))
                dlg.InitialDirectory = Path.GetDirectoryName(m_localFilePath);
            else if (!String.IsNullOrEmpty(m_lastSaveDirEntered))   // new 9/10
                dlg.InitialDirectory = m_lastSaveDirEntered;
            else
                dlg.InitialDirectory = Application.CommonAppDataPath;

            dlg.Filter = CBVConstants.XML_FILE_FILTERS;
            dlg.FileName = "*.xml";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadLocalForm(dlg.FileName);   // sets m_localFilePath
                    if (!String.IsNullOrEmpty(m_localFilePath))
                        m_lastSaveDirEntered = Path.GetDirectoryName(m_localFilePath);
                }
                catch (Exception ex)
                {
                    CBVUtil.ReportError(ex, "Error loading local file");
                    m_localFilePath = "";
                }
            }
        }
        //---------------------------------------------------------------------
        private void CloseMenuItemSelected()
        {
            if (IsEditingForm()) EndFormEdit();
            if (!CheckForSaveOnClose())
                return;

            // close: create new empty standard 3-tab form
            UnloadForm();
            FireCBVFormClosed();
            CreateEmptyForm();
            // disable the Close menu item
            //this.EnableMenu(false); .. don't close - might want to create new empty form this way

            mainFormUltraToolbarsManager.Toolbars[CBVConstants.TOOLBAR_NAVIGATOR].Visible = false;
        }
        //---------------------------------------------------------------------
        private bool CanRevertEdits()
        {
            FormViewTab ctrl = TabManager.CurrentTab as FormViewTab;
            return ctrl != null ? ctrl.CanRevertEdits() : false;
            //return TabManager.CurrentTab is FormViewTab &&
            //    (TabManager.CurrentTab as FormViewTab).CanRevertEdits();
        }
        //---------------------------------------------------------------------
        private void RevertEditsMenuItemSelected()
        {
            if (TabManager.CurrentTab is FormViewTab)
            {
                FormViewTab formTab = TabManager.CurrentTab as FormViewTab;
                //Coverity Bug Fix CID 12968 
                if (formTab != null && formTab.CanRevertEdits())
                    formTab.RevertEdits();
            }
        }
        //---------------------------------------------------------------------
        private String m_lastSaveDirEntered = String.Empty;

        private void SaveLocalToolMenuItemSelected()
        {
            if (IsEditingForm()) EndFormEdit();

            // save form to local file
            SaveFileDialog dlg = new SaveFileDialog();
            //The initial directory will be: …\Documents and Settings\All Users\Application Data\CambridgeSoft\ChemBioViz.NET\12.1.0.0
            dlg.InitialDirectory = Application.CommonAppDataPath;
            dlg.Filter = CBVConstants.XML_FILE_FILTERS;
            dlg.DefaultExt = ".xml";
            dlg.FileName = CBVConstants.DEFAULT_FORM_FILE_NAME;

            // CSBR-118518: Default save to location for local forms is convoluted
            // if form has been saved locally, use its dir; otherwise use last dir entered
            if (!String.IsNullOrEmpty(m_localFilePath))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(m_localFilePath);
                dlg.FileName = Path.GetFileName(m_localFilePath);
            }
            else if (!String.IsNullOrEmpty(m_lastSaveDirEntered))
            {
                dlg.InitialDirectory = m_lastSaveDirEntered;
            }

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SaveFormToFile(dlg.FileName);   // sets m_localFilePath
                m_lastSaveDirEntered = Path.GetDirectoryName(dlg.FileName);
                FormName = Path.GetFileNameWithoutExtension(m_localFilePath);
                FormType = formType.Local;

                // Deactivate any open form on the tree
                DeactivateTreeNode();

                SetApplicationTitle();
                SetNavigationPaneTitle();
            }
            ViewRefresh();
        }
        //---------------------------------------------------------------------
        private bool SaveMenuItemSelected()
        {
            // overwrite existing local file or entry in database object bank
            // if no existing entry, then this is a Save As
            // return false if doing SaveAs and user cancels
            bool ret = true;
            if (IsEditingForm()) EndFormEdit();
            ObjectBank bank = this.IsPublic ? m_formdbMgr.PublicFormBank : m_formdbMgr.PrivateFormBank;
            if (this.IsLocal)
            {
                SaveFormToFile(m_localFilePath);
            }
            else if (String.IsNullOrEmpty(FormName) || !bank.HasObject(m_formID))
            {
                ret = SaveAsMenuItemSelected();
            }
            else
            {
                String sXml = SaveFormToString();
                try
                {

                    bank.Store(m_formID, FormName, sXml);
                    this.m_localFilePath = string.Empty;
                    m_formType = (bank == m_formdbMgr.PublicFormBank) ? formType.Public : formType.Private;
                }
                catch (ObjectBankException ex)
                {
                    CBVUtil.ReportError(ex, "Error saving form");
                    ret = false;
                }
            }
            ViewRefresh();
            CBVStatMessage.ShowReadyMsg();
            return ret;
        }
        //---------------------------------------------------------------------
        private bool SaveAsMenuItemSelected()
        {
            if (IsEditingForm()) EndFormEdit();

            // save to object bank (database), prompting for name and comments
            if (CBVUtil.StrEmpty(FormName))
                FormName = String.Concat("Form", (++m_nextFormNo).ToString());

            bool bOK = SaveDialog.PromptForSaveInfo(this);
            if (!bOK)
                return false;   // user cancelled
            if (FormName.Trim().Length < 1)
            {
                MessageBox.Show("Please enter valid Form name");
                return false;
            }
            String sXml = SaveFormToString();

            bool bSaveAsPublicForm = this.IsPublic;
            ObjectBank bank = bSaveAsPublicForm ? m_formdbMgr.PublicFormBank : m_formdbMgr.PrivateFormBank;
            bool bIsNewForm = !bank.HasName(FormName);
            bank.Store(FormName, sXml);
            m_formID = bank.RetrieveID(FormName);

            Application.DoEvents();
            this.m_localFilePath = string.Empty;

            // add to tree and bring appropriate group in view
            String paneName = bSaveAsPublicForm ? CBVConstants.PUBLIC_GROUPNAME : CBVConstants.PRIVATE_GROUPNAME;
            UltraExplorerBarGroup exBarGroup = ultraExplorerBar1.Groups[paneName];
            exBarGroup.Selected = true;
            /* CSBR-141147 Form(s) saved is(are) not available under Public/Private forms when saved after installation
             * Before adding the leaf to tree see whether the tree is null or not */
            UltraTree tree = GetTreeFromGroup(paneName);
            if (bIsNewForm && tree != null)
            {
                AddLeafToTree(paneName, m_formID, bSaveAsPublicForm);
            }
            else
            {
                this.FormsToPanel(paneName, bSaveAsPublicForm);
            }
            this.ActivateFormInPane();
            CBVStatMessage.ShowReadyMsg();
            ViewRefresh();

            return true;
        }
        //---------------------------------------------------------------------
        public void ShowSearchPrefsDialog()
        {
            PreferencesMenuItemSelected(true);
        }
        //---------------------------------------------------------------------
        private void PreferencesMenuItemSelected(bool bOnSearchTab)
        {
            if (IsEditingForm()) EndFormEdit();

            String[] barGroupKeys = new String[ultraExplorerBar1.Groups.Count];
            for (int i = 0; i < ultraExplorerBar1.Groups.Count; i++)
            {
                barGroupKeys[i] = ultraExplorerBar1.Groups[i].Key;
            }
            // Pass the path collection of each tree
            PreferencesDialog userPreferences = new PreferencesDialog(barGroupKeys,
                GetPathsCollection(CBVConstants.PUBLIC_GROUPNAME), GetPathsCollection(CBVConstants.PRIVATE_GROUPNAME));
            if (bOnSearchTab)
                userPreferences.SelectSearchTab();

            userPreferences.ShowDialog();

            // if current tab is query, refresh it to update sss combo if any
            bool bIsQueryTab = this.TabManager.CurrentTab is QueryViewTab;
            if (bIsQueryTab)
                (this.TabManager.CurrentTab as QueryViewTab).RefreshSSSCombo();

            // if user changed child filtering checkbox, offer to rerun query
            bool bOldFilterSetting = m_formdbMgr.FilterChildHits;
            m_formdbMgr.FilterChildHits = Properties.Settings.Default.FilterChildHits;
            if (bOldFilterSetting != m_formdbMgr.FilterChildHits && CurrQuery != null && CurrQuery.HasChildField)
            {
                //if (ShowChildHitsAlert()) {
                RerunQuery(CurrQuery);
                return; // avoid rerunning again below
                //}
            }
            // if user changed show-sss-hilites, rerun query
            if (CurrQuery != null && CurrQuery.IsHilitable)
            {
                bool bOldHiliteSetting = FormDbMgr.ShowSSSHilites;
                FormDbMgr.ShowSSSHilites = Properties.Settings.Default.ShowSSSHilites;
                if (bOldHiliteSetting != FormDbMgr.ShowSSSHilites && CurrQuery != null && CurrQuery.HasStructComponent)
                    DoRestoreList(CurrQuery);
            }
        }
        //---------------------------------------------------------------------
        private bool ShowChildHitsAlert()
        {
            String msg = "You changed the child hits filtering setting.  Do you want to rebuild the current list with the new setting?";
            return MessageBox.Show(msg, "Change Filtering", MessageBoxButtons.YesNo) == DialogResult.Yes;
        }
        //---------------------------------------------------------------------
        private void ExitMenuItemSelected()
        {
            if (IsEditingForm())
                EndFormEdit();
            if (!CheckForSaveOnClose())
                return;
            this.Modified = false;  // prevent asking again in form close
            this.Close();           // suggestion from Sunil
        }
        //---------------------------------------------------------------------
        private void AboutChemFinderMenuItemSelected()
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.Show();
        }
        //---------------------------------------------------------------------
        private void ConnectionMenuItemSelected()
        {
            FormUtil.DoConnectionDialog(this);
        }
        //---------------------------------------------------------------------
        #endregion

        #region Panes methods
        /// <summary>
        ///  Select the Group in the Explorer Pane as indicated in the preferences
        /// </summary>
        private void SetNavigationPane()
        {
            // create a tree control for dataviews in explorer bar
            m_ultraTreeView = CreatePanelTree(CBVConstants.DATAVIEWS_GROUPNAME);
            m_ultraTreeView.DoubleClick += new EventHandler(treeView_DoubleClick);
            m_ultraTreeView.MouseDown += new MouseEventHandler(UltraTreeView_MouseDown);
            m_ultraTreeView.Override.Sort = SortType.None; // CSBR-133822            

            ultraExplorerBar1.NavigationCurrentGroupAreaHeaderVisible = true;
            ultraExplorerBar1.NavigationMaxGroupHeaders = -1;
            if (!String.IsNullOrEmpty(PreferencesHelper.PreferencesHelperInstance.NavigationPane))
            {
                if (PreferencesHelper.PreferencesHelperInstance.NavigationPane.Equals(CBVConstants.OLD_USERFORMS_GROUPNAME))
                {
                    PreferencesHelper.PreferencesHelperInstance.NavigationPane = CBVConstants.PRIVATE_GROUPNAME;
                }
                ultraExplorerBar1.Groups[PreferencesHelper.PreferencesHelperInstance.NavigationPane].Selected = true;
            }
            this.SetPaneTitle(string.Empty, CBVConstants.EXPLORER_PANE_NAME);
            this.SetPaneTab(CBVConstants.EXPLORER_PANE_NAME, CBVConstants.EXPLORER_PANE_NAME);
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Decide which title to set depending on the user preferences
        /// </summary>
        private void SetNavigationPaneTitle()
        {
            if (!String.IsNullOrEmpty(PreferencesHelper.PreferencesHelperInstance.NavigationPane))
            {
                string title = string.Empty;
                if (string.IsNullOrEmpty(FormName))
                {
                    if (string.Equals(PreferencesHelper.PreferencesHelperInstance.OpenMode, CBVConstants.OPEN_BLANK_FORM, StringComparison.InvariantCultureIgnoreCase))
                        title = PreferencesHelper.PreferencesHelperInstance.NavigationPane;
                    else if (string.Equals(PreferencesHelper.PreferencesHelperInstance.OpenMode, CBVConstants.OPEN_LAST_FORM)
                        || string.Equals(PreferencesHelper.PreferencesHelperInstance.OpenMode, CBVConstants.OPEN_DEFAULT_FORM))
                        title = DecideFormPaneTitle();
                    else
                        title = string.Empty;
                }
                else
                    title = DecideFormPaneTitle();

                this.SetPaneTitle(title, CBVConstants.EXPLORER_PANE_NAME);
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Writes the Pane title for public or private groups
        /// </summary>
        /// <returns></returns>
        private string DecideFormPaneTitle()
        {
            string title = CBVConstants.FORM_UNSAVED;
            if (IsPublic)
                title = CBVConstants.PUBLIC_GROUPNAME;
            else if (IsPrivate)
                title = CBVConstants.PRIVATE_GROUPNAME;
            else if (IsLocal)
                title = CBVConstants.LOCAL_FORMS;

            title += string.Concat(" - ", FormName, " (opened) ");
            return title;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Set title <paramref name="title"/> of the docakable Pane <paramref name="dPane"/>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="dPane"></param>
        private void SetPaneTitle(string title, string dPane)
        {
            DockablePaneBase pane = this.GetPane(dPane);
            if (pane != null)
                pane.Text = title;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Set the tab text <paramref name="textTab"/> of the dockable Pane <paramref name="dPane"/>
        /// </summary>
        /// <param name="textTab"></param>
        /// <param name="dPane"></param>
        private void SetPaneTab(string textTab, string dPane)
        {
            DockablePaneBase pane = this.GetPane(dPane);
            if (pane != null)
            {
                pane.TextTab = textTab;
                switch (textTab)
                {
                    case CBVConstants.EXPLORER_PANE_NAME:
                        pane.Settings.TabAppearance.Image = ChemBioViz.NET.Properties.Resources.Preview; break;
                    case CBVConstants.PROPERTIES_PANE_NAME:
                        pane.Settings.TabAppearance.Image = ChemBioViz.NET.Properties.Resources.Properties1; break;
                    case CBVConstants.TOOLBOX_PANE_NAME:
                        pane.Settings.TabAppearance.Image = ChemBioViz.NET.Properties.Resources.Tool_Set1; break;
                }
            }
        }
        //---------------------------------------------------------------------
        private void CreateToolboxPane(bool bQueryMode)
        {
            // create Toolbox Pane on first call
            UltraExplorerBarGroup exBarGroup = ultraExplorerBar1.Groups[CBVConstants.FORMEDIT_GROUPNAME];
            UltraExplorerBarContainerControl ubcc = exBarGroup.Container;

            // if we are creating a new one, get rid of the old
            if (m_toolbox != null)
                ubcc.Controls.Remove(m_toolbox);

            m_toolbox = new ToolboxControlEx();
            InitToolbox(m_toolbox, bQueryMode);

            m_toolbox.Dock = DockStyle.Fill;
            m_toolbox.BackColor = Color.FromKnownColor(KnownColor.Control);

            ubcc.Controls.Add(m_toolbox);
            exBarGroup.Selected = true;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Helper functions
        /// </summary>
        /// <param name="toolbox"></param>
        private void InitToolbox(ToolboxControlEx toolbox, bool bQueryMode)
        {
            // standard types
            toolbox.AddToolboxItem(new ToolboxItem(typeof(System.Windows.Forms.Label)));

            //toolbox.AddToolboxItem(new ToolboxItem(typeof(GroupBox)));  // no good, fouls up control loops [was CSBR-133377]
            ToolboxItem frame = new ToolboxItem(typeof(CBVFrame));
            frame.DisplayName = "Frame";
            toolbox.AddToolboxItem(frame);

            if (bQueryMode && this.FeatEnabler.CanCreateQueryTextBox())
            {
                ToolboxItem button = new ToolboxItem(typeof(CBVQueryTextBox));
                button.DisplayName = "QueryText";
                toolbox.AddToolboxItem(button);
            }
            else
            {
                //toolbox.AddToolboxItem(new ToolboxItem(typeof(TextBox)));
                ToolboxItem button = new ToolboxItem(typeof(CBVTextBox));
                button.DisplayName = "TextBox";
                toolbox.AddToolboxItem(button);
            }
            toolbox.AddToolboxItem(new ToolboxItem(typeof(CheckBox)));
            if (!bQueryMode)
                toolbox.AddToolboxItem(new ToolboxItem(typeof(RichTextBox)));

            if (!bQueryMode)    // CSBR-118514: don't show subform tool 
            {
                ToolboxItem button = new ToolboxItem(typeof(CBVButton));
                button.DisplayName = "Button";
                button.Bitmap = (new ToolboxItem(typeof(Button))).Bitmap;
                toolbox.AddToolboxItem(button);
            }

            if (!m_bHasNoCdax)
            {
                ToolboxItem cdbox = new ToolboxItem(typeof(CBVSafeChemDrawBox));
                cdbox.DisplayName = "ChemDrawBox";
                cdbox.Bitmap = ChemBioViz.NET.Properties.Resources.CDR.ToBitmap();
                toolbox.AddToolboxItem(cdbox);
            }

            if (!bQueryMode)
            {
                ToolboxItem fmlabox = new ToolboxItem(typeof(ChemFormulaBox));
                fmlabox.DisplayName = "FormulaBox";
                fmlabox.Bitmap = ChemBioViz.NET.Properties.Resources.Formula;
                toolbox.AddToolboxItem(fmlabox);
            }

            if (!bQueryMode)
                toolbox.AddToolboxItem(new ToolboxItem(typeof(PictureBox)));
            toolbox.AddToolboxItem(new ToolboxItem(typeof(MonthCalendar)));
            toolbox.AddToolboxItem(new ToolboxItem(typeof(DateTimePicker)));

            if (!bQueryMode)
            {
                ToolboxItem dgvbox = new ToolboxItem(typeof(CBVDataGridView));
                dgvbox.DisplayName = "SubformGrid";
                dgvbox.Bitmap = ChemBioViz.NET.Properties.Resources.Grid;
                toolbox.AddToolboxItem(dgvbox);
            }

            if (FeatEnabler.CanCreateComboBoxes())
            {
                // CSBR-127670: combo boxes are no longer available except for queries
                if (bQueryMode)
                {
                    ToolboxItem button = new ToolboxItem(typeof(CBVLookupCombo));
                    button.DisplayName = "Lookup Combo";
                    toolbox.AddToolboxItem(button);
                }
            }

            if (FeatEnabler.CanPlot() && !bQueryMode)
                toolbox.AddToolboxItem(new ToolboxItem(typeof(CBVChartControl)));

            if (bQueryMode)
            {
                ToolboxItem button = new ToolboxItem(typeof(CBVSSSOptionsCombo));
                button.DisplayName = "Structure Search Combo";
                toolbox.AddToolboxItem(button);

                ToolboxItem browseButton = new ToolboxItem(typeof(CBVBrowseButton));
                browseButton.DisplayName = "File Browse Button";
                toolbox.AddToolboxItem(browseButton);

                if (FeatEnabler.CanUseNumericUnits())
                {
                    ToolboxItem unitsCombo = new ToolboxItem(typeof(CBVUnitsCombo));    // CSBR-132401
                    unitsCombo.DisplayName = "Units Combo";
                    toolbox.AddToolboxItem(unitsCombo);
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Creates the Properties pane which appears during the form edition
        /// </summary>
        private void CreatePropertiesPane()
        {
            // create Properties Grid on first call
            DockAreaPane propertiesDockAreaPane = new Infragistics.Win.UltraWinDock.DockAreaPane(Infragistics.Win.UltraWinDock.DockedLocation.DockedRight, new Guid());
            propertiesDockAreaPane.Size = new Size(220, propertiesDockAreaPane.Size.Height);
            DockableControlPane propertiesDockableControlPane = new DockableControlPane();
            propertiesDockableControlPane.TextTab = CBVConstants.PROPERTIES_PANE_NAME;
            propertiesDockableControlPane.Key = CBVConstants.PROPERTIES_PANE_NAME;

            propertiesDockAreaPane.Key = CBVConstants.PROPERTIES_DOCKAREA_PANE;

            // Add the pane to the dock area pane
            propertiesDockAreaPane.Panes.AddRange(new Infragistics.Win.UltraWinDock.DockablePaneBase[] {
                    propertiesDockableControlPane});
            propertiesDockableControlPane.Text = CBVConstants.PROPERTIES_PANE_NAME;
            this.ultraDockManager1.DockAreas.Add(propertiesDockAreaPane);

            m_propertyGrid = new PropertyGrid();
            m_propertyGrid.Dock = DockStyle.Fill;
            m_propertyGrid.BackColor = Color.FromKnownColor(KnownColor.Control);
            propertiesDockableControlPane.Control = m_propertyGrid;
        }
        //---------------------------------------------------------------------
        protected void OnViewTooltips()
        {
            bool bEnable = !ChemBioViz.NET.Properties.Settings.Default.ShowTooltips;    // toggle
            ChemBioViz.NET.Properties.Settings.Default.ShowTooltips = bEnable;
            ChemDataGrid.bgAllowGridTooltips = bEnable;

            if (TabManager.CurrentTab is GridViewTab)

            // PROBLEM: what about subform grids?  View Tooltips doesn't work on them unless you go to grid tab and return
            {
                ChemDataGrid cdg = ((GridViewTab)TabManager.CurrentTab).CDGrid;
                if (cdg != null)
                {
                    ChemDataGridToolTipItemCreationFilter filter = cdg.CreationFilter as ChemDataGridToolTipItemCreationFilter;
                    //coverity Bug Fix : CID 12962 
                    if (filter != null)
                        filter.Enabled = bEnable;
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Reload current record and refresh screen
        /// </summary>
        protected void ViewRefresh()
        {
            ViewRefresh(false);
        }
        //---------------------------------------------------------------------
        protected void ViewRefresh(bool bRecreateBinding)
        {
            FormTab currTab = this.TabManager.CurrentTab;
            if (currTab == null || CurrQuery == null || BindingSource == null)
                return;

            // new 2/11: rebuild the bindings
            if (bRecreateBinding)
            {
                RecreateBindingSource(false, this.Pager.CurrRow);
#if DEBUG
                CBVUtil.StringToFile(m_formdbMgr.ResultsCriteria.ToString(), "C:\\rc_out_vr.xml", Encoding.Unicode);
#endif
            }
            currTab.Bind(CurrQuery, BindingSource);

            // update plot bar
            //Coverity Bug Fix CID :12973 
            if (currTab.IsFormView() && currTab.Control is FormViewControl)
            {
                if (((FormViewControl)currTab.Control).HasPlots())
                {
                    CBVChartPanel plotControls = ChartController.PlotControls;
                    plotControls.Bind(this, ChartController.SelectedPlot);
                }
            }
            this.DoMove(Pager.MoveType.kmCurr);
        }
        //---------------------------------------------------------------------
        private bool IsPaneShowing(string dPane)
        {
            //Coverity Bug Fix :CID 13088 
            DockablePaneBase pane = this.GetPane(dPane);
            if (pane != null)
                return pane.IsVisible;
            return false;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Show or hide a pane <paramref name="dPane"/>
        /// </summary>
        /// <param name="dPane"></param>
        private void ShowHidePane(bool show, string dPane)
        {
            DockablePaneBase pane = this.GetPane(dPane);

            // show or hide; resize main window accordingly
            if (pane != null)
            {
                int paneWidth = pane.DockAreaPane.Size.Width;   // not pane.Size! that seems to be 100x100
                if (show)
                {
                    if (!this.IsEditingForm() && string.Equals(dPane, CBVConstants.PROPERTIES_PANE_NAME))
                        this.TopLevelControl.Width += paneWidth;
                    pane.Show();
                    Application.DoEvents(); // repaint vacant expansion space
                }
                else
                {
                    pane.Close();
                    this.TopLevelControl.Width -= paneWidth;
                    Application.DoEvents(); // repaint vacant expansion space
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Get the dockable pane from a given name <paramref name="dPane"/> 
        ///  It could be docked in any place or even floating - which is considered another dock area as well
        /// </summary>
        /// <param name="dPane"></param>
        /// <returns></returns>
        private DockablePaneBase GetPane(string dPane)
        {
            //Evaluate where is the pane now
            DockablePaneBase pane = null;
            if (this.ultraDockManager1.DockAreas.Exists(dPane))
            {
                pane = this.ultraDockManager1.DockAreas[dPane];
            }
            else
            {
                foreach (DockAreaPane dockArea in this.ultraDockManager1.DockAreas)
                {
                    string mainPane = (string.Equals(dPane, CBVConstants.TOOLBOX_PANE_NAME)
                        || string.Equals(dPane, CBVConstants.PUBLIC_GROUPNAME) || string.Equals(dPane, CBVConstants.PRIVATE_GROUPNAME)
                        || string.Equals(dPane, CBVConstants.QUERIES_GROUPNAME)) ? CBVConstants.EXPLORER_PANE_NAME : dPane;
                    if (dockArea.Panes.Exists(mainPane))
                    {
                        pane = dockArea.Panes[mainPane];
                        break;
                    }
                }
            }
            return pane;
        }
        //---------------------------------------------------------------------
        private void SetEditButtonText()
        {
            String label = CBVConstants.FORMTABEDIT;
            if (m_tabManager.CurrentTab.IsQueryView()) label = CBVConstants.QUERYTABEDIT;
            else if (m_tabManager.CurrentTab.IsGridView()) label = CBVConstants.GRIDTABEDIT;

            ultraExplorerBar1.Groups[CBVConstants.FORMEDIT_GROUPNAME].Text = label;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Sets the default button of the form as searchUltraButton if the app is in query view
        /// </summary>
        private void SetAcceptButton()
        {
            int index = MainUltraTabControl.SelectedTab.Index;
            FormTab ftab = m_tabManager.GetTab(index);
            if (ftab.IsQueryView())
                this.AcceptButton = this.searchUltraButton;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Restore the ultra explorer bar in the navigation pane to normal state 
        ///  selecting the group indicated by <paramref name="groupName"/>
        /// </summary>
        /// <param name="groupName">Indicates the group to select</param>
        public void RestoreExplorerBar()
        {
            // put up the default group, after editing
            ultraExplorerBar1.Groups[CBVConstants.PUBLIC_GROUPNAME].Text = CBVConstants.PUBLIC_GROUPNAME;
            ultraExplorerBar1.Groups[CBVConstants.PRIVATE_GROUPNAME].Text = CBVConstants.PRIVATE_GROUPNAME;
            ultraExplorerBar1.Groups[CBVConstants.QUERIES_GROUPNAME].Text = CBVConstants.QUERIES_GROUPNAME;
            ultraExplorerBar1.Groups[CBVConstants.DATAVIEWS_GROUPNAME].Text = CBVConstants.DATAVIEWS_GROUPNAME;

            UltraExplorerBarGroup exBarGroup = m_groupBeforeEdit;
            if (exBarGroup != null)
                exBarGroup.Selected = true;
            ultraExplorerBar1.NavigationCurrentGroupAreaHeaderVisible = true;
            ultraExplorerBar1.NavigationMaxGroupHeaders = m_maxGroupHeadersBeforeEdit;

            this.SetGroupText(exBarGroup);
            this.SetNavigationPaneTitle();
            this.SetPaneTab(CBVConstants.EXPLORER_PANE_NAME, CBVConstants.EXPLORER_PANE_NAME);
        }
        //---------------------------------------------------------------------
        #endregion

        #region Binding methods
        public void RecreateBindingSource(bool bOnlyIfFieldsAdded, int moveToRec)
        {
            // after form changes: make new RC and new BS from curr query
            if (FormDbMgr.SelectedDataView == null || FormDbMgr.SelectedDataViewTable == null)
                return;

            // do this only if fields were added which we don't already have
            // NO! let's do it whenever rc's do not match exactly
            ResultsCriteria rcOld = this.FormDbMgr.ResultsCriteria;
            ResultsCriteria rcNew = FormUtil.FormToResultsCriteria(this);
            //bool bFieldsWereAdded = FormUtil.HasAddedFields(rcNew, rcOld);
            //if (!bFieldsWereAdded && bOnlyIfFieldsAdded && BindingSource != null)
            //    return;
            String sXmlOld = rcOld.ToString();
            String sXmlNew = rcNew.ToString();
            //if (sXmlOld.Equals(sXmlNew) && BindingSource != null)  Fixed CSBR-163080 Before refreshing the data have to create binding source irrespective of same result criteria and binding source
            //    return;

            CBVTimer.StartTimerWithMessage(true, "Refreshing data bindings", true);
            m_formdbMgr.ResultsCriteria = rcNew;
            m_formdbMgr.BaseTableRecordCount = -1;	// recount when needed

            bool bDumpRC = false;
            if (bDumpRC)
            {
                String fname = "C:\\rc_after_edit.xml";
                String sXmlDump = m_formdbMgr.ResultsCriteria.ToString();
                CBVUtil.StringToFile(sXmlDump, fname, Encoding.Unicode);
            }

            Query query = this.CurrQuery;

            // if there is no curr query, we are hooking up a form to a new datasource
            if (query == null)
            {
                query = this.QueryCollection.RetrieveAllQuery;
                this.CurrQuery = query;
                if (this.BindingNavigator == null)
                    this.AddNavigator();
            }

            query.Pager.Clear();   // force new fetch
            query.Run();
            if (query.DataSet != null && query.DataSet.Tables.Count > 0)
            {
                m_tabManager.Bind(query, null);
                m_tabManager.ReportBindingError();
                ActivateQueryOnTree(query);
                if (moveToRec > 0)
                    DoMove(Pager.MoveType.kmGoto, moveToRec);
            }
            CBVTimer.EndTimer();
        }
        //---------------------------------------------------------------------
        #endregion

        #region Load/Save methods
        static public void RestrictColumns(DataTable dataTable)
        {
            if (Properties.Settings.Default.UseDefaultFieldsOnly)
            {
                // CSBR-152206: Use new extended property to determine
                //  if this field should be included (i.e. for auto-
                //  generated forms and subforms)
                for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                {
                    DataColumn c = dataTable.Columns[i];
                    PropertyCollection exprops = c.ExtendedProperties;
                    bool bIsDefault = ((exprops["Default"] != null) && ((bool)exprops["Default"] == true));
                    if ((!bIsDefault) && (dataTable.Columns.CanRemove(c)))
                        dataTable.Columns.Remove(c);
                }
            }
        }
        //-------------------------------------------------------------------------------------
        /// <summary>
        /// Open/save forms
        /// </summary>
        /// <param name="bGenerate"></param>
        private void GenerateOrOpenForm(string sXml, bool bGenerate, Query query, bool withSubforms)
        {
            // code shared by form open and form create (double-click on dataview tree)
            CBVTimer.StartTimerWithMessage(false, "Opening form", true);
            CBVUtil.BeginUpdate(this);

            try
            {
                // run a query to get first page of data
                if (query != null)
                {
                    try
                    {
                        query.RestoreHitlist(); // uses saved hitlist if possible, otherwise runs query
                    }
                    catch (NoHitsException e)
                    {
                        CBVUtil.ReportError(e, "");
                    }
                }

                bool bInvalidResult = query == null || query.DataSet == null || query.DataSet.Tables.Count < 1;
                if (bGenerate && bInvalidResult)
                {
                    CBVUtil.EndUpdate(this);
                    throw new Exception("Empty or invalid dataset; unable to generate form");
                }
                CurrQuery = query;  // add to m_queries?

                // add or create tabs
                if (!bGenerate)
                    m_tabManager.AddToForm();
                else
                {
                    DataTableCollection dataTables = query.DataSet.Tables;
                    m_bindingSource = null; // CSBR-110026
                    m_formID = -1;
                    FormName = FormDbMgr.SelectedDataViewBOName;
                    Modified = true;
                    //CBOE- 911 Restrict the columns when UseDefaultFields option is selected  
                    if (Properties.Settings.Default.UseDefaultFieldsOnly)
                    {
                        foreach (DataTable table in dataTables)
                            RestrictColumns(table);
                    }
                    m_tabManager.Create3TFromDataSource(query.DataSet, dataTables[0].TableName, withSubforms);
                }

                // create record navigator on toolbar or child window
                m_bindingSource = null;
                if (this is CBVChildForm)
                    AddNavigatorToChildWnd();
                else
                    AddNavigator();

                // TO DO: if grandchild subforms present, make sure their parents are too
                // and/or: return an error message from Bind if any problems were encountered

                // bind form to data
                m_tabManager.Bind(query, m_bindingSource);
                // CBOE-911 Update ResultCriteria when UseDefaultFields is selected
                if (bGenerate && Properties.Settings.Default.UseDefaultFieldsOnly)
                {
                    // CSBR-153313: Update ResultsCriteria according to possible restriction
                    //  by RestrictColumns (above)
                    m_formdbMgr.ResultsCriteria = FormUtil.FormToResultsCriteria(this);
                }

                m_tabManager.BuildTabControl();
                Application.DoEvents();

                if (m_tabManager.Count > 0)
                    m_tabManager.SelectTab(m_tabManager.SelectedIndex); // index is stored in file, 0 if not found

                // Build the Queries tree
                QueriesToTree(sXml);
                ActivateQueryOnTree(query);

                if (!bGenerate)
                    Modified = false;   // might have been set true during binding

                if (!this.TabManager.CurrentTab.IsQueryView())  // CSBR-135966 -- rec nav invisible in query tab
                    this.ShowToolbar(CBVConstants.TOOLBAR_NAVIGATOR, true); // show the Record Navigator

                if (query != null && !(query is RetrieveAllQuery) && SearchCompleted != null)
                    this.SearchCompleted.Invoke(this, new EventArgs());

                m_tabManager.ReportBindingError();   // only if non-blank
            }
            catch (Exception e)
            {
                this.UnloadForm();
                CBVUtil.ReportError(e, "Form generation error");
            }
            CBVUtil.EndUpdate(this);
            CBVTimer.EndTimer();
            Application.DoEvents();
            Refresh();
        }
        //---------------------------------------------------------------------	
        private void WriteConfigXml(String sData, String filename, String formName, String userName)
        {
            int formID = 13;
            String sPrefix = String.Format("<configuration subdirectory=\"GenericObjects\" " +
                "databasename=\"ChemBioViz\" description=\"CBVN Form\" formgroup=\"0\" id=\"{0}\" " +
                "ispublic=\"True\" name=\"{1}\" username=\"{2}\"><xml>" +
                "<![CDATA[", formID, formName, userName);

            String sSuffix = "]]></xml></configuration>";
            String sAll = String.Concat(sPrefix, sData, sSuffix);
            CBVUtil.StringToFile(sAll, filename);
        }
        //---------------------------------------------------------------------	
        private void SaveFormToFile(String filename)
        {
            CBVStatMessage.Show("Saving form " + filename);

            // for developer's use: if filename ends with "_c", write to config xml file instead of form file
            bool bIsConfigFile = false;

            //#if DEBUG // do this in release mode so Megean can use it
            bIsConfigFile = CBVUtil.EndsWith(filename, "_c.xml");
            //#endif
            try
            {
                //XmlTextWriter xmlWriter = new XmlTextWriter(filename, Encoding.Unicode);
                XmlTextWriter xmlWriter = null;
                StringWriter stringWriter = null;
                if (bIsConfigFile)
                {
                    stringWriter = new StringWriter();
                    xmlWriter = new XmlTextWriter(stringWriter);
                }
                else
                {
                    xmlWriter = new XmlTextWriter(filename, Encoding.Unicode);
                }
                xmlWriter.Formatting = Formatting.Indented;

                XmlDocument xdoc = new XmlDocument();
                XmlElement root = xdoc.CreateElement("cbvnform");
                xdoc.AppendChild(root);

                SaveToXmlEx(root, xdoc);
                xdoc.Save(xmlWriter);
                xmlWriter.Close();

                Modified = false;
                this.m_localFilePath = filename;

                if (bIsConfigFile)
                    WriteConfigXml(stringWriter.ToString(), filename, this.FormName, "CBVN");
            }
            catch (Exception e)
            {
                CBVUtil.ReportError(e, String.Concat("Error saving file ", filename));
            }
            finally
            {
                CBVStatMessage.ShowReadyMsg();
            }
        }
        //---------------------------------------------------------------------
        private String SaveFormToString()
        {
            CBVStatMessage.Show("Saving form " + FormName);

            String sXml = string.Empty;
            XmlDocument xdoc = new XmlDocument();
            XmlElement root = xdoc.CreateElement("cbvnform");
            xdoc.AppendChild(root);
            SaveToXmlEx(root, xdoc);
            //Coverity Bug Fix local Analysis
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter))
                {
                    xdoc.Save(xmlWriter);
                }
                sXml = stringWriter.ToString();
            }

            //xdoc = null;
            Modified = false;   // assume string will be persisted
            //stringWriter.Dispose();
            //stringWriter = null;
            CBVStatMessage.ShowReadyMsg();
            return sXml;
        }
        //---------------------------------------------------------------------
        #endregion

        #region Query methods
        public void AddQueryToTree(Query q, bool bAndSelect)
        {
            if (String.IsNullOrEmpty(q.Name))
                q.Name = QueryCollection.GenerateUniqueQueryName();
            m_queries.Add(q);

            // bring up query tree if not already showing
            if (bAndSelect)
            {
                UltraExplorerBarGroup exBarGroup = ultraExplorerBar1.Groups[CBVConstants.QUERIES_GROUPNAME];
                exBarGroup.Selected = true;
            }
            if (q.HasParentQuery)
                AddChildLeafToTree(CBVConstants.QUERIES_GROUPNAME, q.ID, q.ParentQueryID);
            else
                AddLeafToTree(CBVConstants.QUERIES_GROUPNAME, q.ID, false);

            // user should save the form to save new node on structure
            Modified = true;
            this.ActivateQueryOnTree(q);  // Add * to show that the form changed
        }
        //---------------------------------------------------------------------
        #endregion

        #region Dataset methods
        private String GetCachedFullDataSetFileName()
        {
            // like "CHEMFINDER_CSDEMO_FDS.xml"
            // in C:\Documents and Settings\All Users\Application Data\CambridgeSoft\ChemBioViz.NET\12.1.0.0
            String fname = String.Concat(FormDbMgr.SelectedDataViewBOName,
                                            "_", FormDbMgr.TableName, "_FDS.xml");
            String s = Path.Combine(Application.CommonAppDataPath, fname);
            return s;
        }
        //---------------------------------------------------------------------
        private DataSet GetCachedFullDataSet()
        {
#if USING_CACHED_DSETS
            String fname = GetCachedFullDataSetFileName();
            if (!File.Exists(fname))
                return null;
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(fname);
            FormDbMgr.PrepDataSet(dataSet);
            return dataSet;
#else
            return null;
#endif
        }
        //---------------------------------------------------------------------
        private void SaveCachedFullDataSet(DataSet dataSet)
        {
#if USING_CACHED_DSETS
            String fname = GetCachedFullDataSetFileName();
            dataSet.WriteXml(fname, XmlWriteMode.WriteSchema);
#endif
        }
        //---------------------------------------------------------------------
        public BindingSource CreateFullBindingSource()
        {
            // create bindingsource representing entire dataview
            // do this by using full RC, retrieving one record, creating BS from dataset
            // TEMPORARY: save in local file and reuse; otherwise takes too long

            // caller must ensure we have a selected dataview
            if (FormDbMgr.SelectedDataView == null)
                return null;

            // create new FRC if we have none or current one is invalid
            ResultsCriteria rcFull = FormDbMgr.FullResultsCriteria;

            bool bInvalidRc = rcFull == null ||
                                !CBVUtil.Eqstrs(FormDbMgr.DvNameOfFullCriteria, FormDbMgr.SelectedDataViewBOName);
            if (bInvalidRc)
            {
                rcFull = FormDbMgr.CreateResultsCriteria(true); // with subforms
                FormDbMgr.FullResultsCriteria = rcFull;
                FormDbMgr.DvNameOfFullCriteria = FormDbMgr.SelectedDataViewBOName;
            }

            // retrieve one record using this RC
            DataSet dataSet = GetCachedFullDataSet();
            if (dataSet == null)
            {
                CBVTimer.StartTimerWithMessage(true, "Retrieving database definition", true);
                dataSet = Pager.GetOneRecord(rcFull, FormDbMgr.SelectedDataView);
                if (dataSet != null)
                {
                    FormDbMgr.PrepDataSet(dataSet); // new 11/09
                    SaveCachedFullDataSet(dataSet);
                }
                CBVTimer.EndTimer();
            }
            if (dataSet == null || dataSet.Tables.Count == 0)
                return null;

            // then create bindingsource from result
            m_fullBindingSource = new BindingSource(dataSet, dataSet.Tables[0].TableName);
            return m_fullBindingSource;
        }
        //---------------------------------------------------------------------
        #endregion

        #region Sort methods
        private void SortMenuItemSelected()
        {
            // put up sort dialog, get resulting field(s), then sort
            String sortStr = (this.CurrQuery == null) ? String.Empty : this.CurrQuery.SortString;
            SortDialog dialog = new SortDialog(this, sortStr);
            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                sortStr = dialog.SortString;
                Application.DoEvents();
                this.SortOnSortString(sortStr);
            }
        }
        //---------------------------------------------------------------------
        private void SortOnSortString(String sortStr)
        {
            // this routine does the sort; called by menu handlers
            Query q = this.CurrQuery;
            q.SortString = sortStr;
            FormDbMgr.ApplySortStringToRC(sortStr);

            // do the sort by repeating the last search
            CBVStatMessage.Show("Sorting");
            CBVUtil.BeginUpdate(this);
            try
            {
                q.RestoreHitlist();                     // restore if possible, otherwise run
                this.DoMove(Pager.MoveType.kmFirst);

                m_tabManager.Bind(q, BindingSource);    // to rebuild grid; TO DO: only bind if in grid view?
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex);
            }
            CBVUtil.EndUpdate(this);
            CBVStatMessage.ShowReadyMsg();
        }
        //---------------------------------------------------------------------
        #endregion

        #region Form edit methods
        /// <summary>
        /// Edit form or grid
        /// </summary>
        private void EditFormMenuItemSelected()
        {
            // Edit > Edit Form is a toggle, for now
            if (IsEditingForm())
                EndFormEdit();
            else
            {
                if (!FormDbMgr.PrivilegeChecker.CanEditForms)
                {     // CSBR-135265
                    FormDbMgr.PrivilegeChecker.ShowRestrictionMessage(CBVConstants.CAN_EDIT_FORMS, false);
                    return;
                }
                BeginFormEdit();
            }
        }
        //---------------------------------------------------------------------
        private bool IsEditingForm()
        {
            return m_tabManager != null && m_tabManager.IsEditingForm();
        }
        //---------------------------------------------------------------------
        private bool IsEditingGrid()
        {
            return this.m_editingGrid;
        }
        //---------------------------------------------------------------------
        private bool CheckForDataView()
        {
            // if there is no selected dv, we are trying to edit a blank form
            // use the selection from the dv pane if any; if none, put up an alert and ret false
            if (FormDbMgr.SelectedDataView != null
                    && FormDbMgr.SelectedDataViewTable != null) // CSBR-131835 
                return true;

            UltraExplorerBarGroup dataViewGroup = ultraExplorerBar1.Groups[CBVConstants.DATAVIEWS_GROUPNAME];
            UltraExplorerBarContainerControl ubcc = dataViewGroup.Container;
            if (this.m_groupBeforeEdit == dataViewGroup)   // CSBR-131835: confusion if dv tree is hidden but has a selection
            {
                UltraTree treeView = ubcc.Controls[0] as UltraTree;
                //Coverity Bug Fix : CID 12949 
                if (treeView != null)
                {
                    UltraTreeNode selNode = treeView.ActiveNode;
                    if (selNode != null && selNode.Parent != null)
                    {
                        String tableName = selNode.Text;
                        String appName = selNode.Parent.Text;
                        if (selNode.Parent.Parent != null)
                            appName = selNode.Parent.Parent.Text;
                        if (!String.IsNullOrEmpty(tableName) && !String.IsNullOrEmpty(appName))
                        {
                            FormDbMgr.Select(appName, tableName, true);   // TO DO: use id's?
                            return true;
                        }
                    }
                }
            }
            String msg = "Before editing, select a base table from the Dataviews pane";
            dataViewGroup.Selected = true;
            MessageBox.Show(msg);
            return false;
        }
        //---------------------------------------------------------------------
        private bool BeginFormEdit()
        {
            // prep for form editing: create toolbox and prop grid on first call; change nav bar
            // first check whether we have a selected dataview; if not, prompt user to choose one
            // return false if user needs to choose a dataview first
            if (!CheckForDataView())
                return false;

            if (m_editingGrid)
            {
                EndFormEdit();
                return false;
            }
            m_editingGrid = m_tabManager.CurrentTab.IsGridView();

            CBVUtil.BeginUpdate(this);
            bool bIsFormView = m_tabManager.CurrentTab.IsFormView();

            if (m_propertyGrid == null)
            {
                this.CreatePropertiesPane();
            }
            this.ShowHidePane(true, CBVConstants.PROPERTIES_PANE_NAME);  // show and resize main window
            this.SetPaneTitle(CBVConstants.PROPERTIES_PANE_NAME, CBVConstants.PROPERTIES_PANE_NAME);
            if (m_editingGrid)
                ultraExplorerBar1.Groups[CBVConstants.FORMEDIT_GROUPNAME].Text = CBVConstants.GRIDEDITDONE;

            if (bIsFormView)
            {
                m_maxGroupHeadersBeforeEdit = ultraExplorerBar1.NavigationMaxGroupHeaders;

                // change public forms button to say End Editing; private forms button to say Cancel
                // assume these are the top two buttons
                ultraExplorerBar1.Groups[CBVConstants.PUBLIC_GROUPNAME].Text = CBVConstants.EDITING_GROUPNAME;
                ultraExplorerBar1.Groups[CBVConstants.PRIVATE_GROUPNAME].Text = CBVConstants.CANCELEDIT_GROUPNAME;

                // CSBR-118514: Subforms should not be listed in the control list when editing a query form
                this.CreateToolboxPane(m_tabManager.CurrentTab.IsQueryView());
                this.SetPaneTitle(CBVConstants.TOOLBOX_PANE_NAME, CBVConstants.TOOLBOX_PANE_NAME);

                // hide nav bar buttons to give more room for toolbox -- leave two visible
                ultraExplorerBar1.NavigationCurrentGroupAreaHeaderVisible = false;
                ultraExplorerBar1.NavigationMaxGroupHeaders = 2;

                // CSBR-110600: disable binding nav bar during edit
                if (BindingNavigator != null)   // CSBR-110916
                    BindingNavigator.BindingSource = null;

                this.ShowToolbar(CBVConstants.TOOLBAR_NAVIGATOR, false); //Hide the Record Navigator
                DeactivatePlotInPane();     // unselect plot, in case deleted while editing (CSBR-128494)
            }
            m_tabManager.BeginEdit(m_toolbox, m_propertyGrid);
            this.SetPaneTab(CBVConstants.TOOLBOX_PANE_NAME, CBVConstants.EXPLORER_PANE_NAME);
            this.SetPaneTab(CBVConstants.PROPERTIES_PANE_NAME, CBVConstants.PROPERTIES_PANE_NAME);
            CBVUtil.EndUpdate(this);
            return true;
        }
        //---------------------------------------------------------------------
        public void EndFormEdit()
        {
            EndOrCancelFormEdit(false);
        }
        //---------------------------------------------------------------------
        private void EndOrCancelFormEdit(bool bCancel)
        {
            // restore nav bar after edit
            CBVUtil.BeginUpdate(this);
            bool bIsFormView = m_tabManager != null && m_tabManager.CurrentTab.IsFormView();
            bool bIsQueryTab = m_tabManager != null && m_tabManager.CurrentTab.IsQueryView();

            //Coverity Bug Fix : CID 12958 
            if (m_tabManager != null && !m_tabManager.EndEdit(bCancel))  // sets Modified if changed; returns false if user cancels alert => keep editing
            {
                CBVUtil.EndUpdate(this);
                return;
            }
            if (m_editingGrid)
                ultraExplorerBar1.Groups[CBVConstants.FORMEDIT_GROUPNAME].Text = CBVConstants.GRIDTABEDIT;
            m_editingGrid = false;

            this.RestoreExplorerBar();
            this.ShowHidePane(false, CBVConstants.PROPERTIES_PANE_NAME);
            if (bIsFormView)
            {
                // re-display four buttons in the nav bar
                this.RestoreExplorerBar();

                // reactivate binding navigator
                if (BindingNavigator != null)   // TO DO: create if null?
                    BindingNavigator.BindingSource = this.BindingSource;
            }
            CBVUtil.EndUpdate(this);
            if (!bIsQueryTab)   // CSBR-135966: don't show rec nav in query tab
            {
                this.ShowToolbar(CBVConstants.TOOLBAR_NAVIGATOR, true); //Show the Record Navigator
                RefreshBindingNavigator();
            }
            if (m_tabManager != null && m_tabManager.CurrentTab.Control != null)
                m_tabManager.CurrentTab.Control.Focus();
            FireFormEdited();
        }
        //---------------------------------------------------------------------
        private void CancelFormEdit()
        {
            EndOrCancelFormEdit(true);
        }
        #endregion

        #region XML methods
        //---------------------------------------------------------------------
        public void BeginQueryMode()
        {
            this.ShowToolbar(CBVConstants.TOOLBAR_NAVIGATOR, false);
            m_tabManager.BeginQueryMode();
        }
        //---------------------------------------------------------------------
        public void EndQueryMode()
        {
            m_tabManager.EndQueryMode();
            this.ShowToolbar(CBVConstants.TOOLBAR_NAVIGATOR, true);
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Save/load complete form
        /// </summary>
        /// <param name="root"></param>
        /// <param name="xdoc"></param>
        private void SaveToXmlEx(XmlElement root, XmlDocument xdoc)
        {
            // main form contains sections <connection> <tabs> <queries> [<COEform>]
            CBVUtil.BeginUpdate(this);
            int currRow = (this.Pager == null) ? 0 : this.Pager.CurrRow; // prevent crash if form not connected to db

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name.Equals("queries"))
                {
                    root.RemoveChild(node);
                    break;
                }
            }

            XmlElement headerElement = CreateHeaderXmlElement(xdoc, "connection");
            root.AppendChild(headerElement);

            XmlElement tabsElement = CreateTabsXmlElement(xdoc, "tabs");
            root.AppendChild(tabsElement);

            // Save Queries and tree structure
            XmlElement queriesElement = this.m_queries.CreateXmlElement(xdoc, "queries");
            XmlNode tempElement = xdoc.CreateNode(XmlNodeType.Element, CBVConstants.TREE_QUERIES_ROOT.ToLower(), string.Empty);
            tempElement.InnerXml = m_QTreeConf.SerializeTreeConfig(CBVConstants.QUERIES_GROUPNAME, string.Empty);
            XmlNode n = tempElement.SelectSingleNode("//TreeStructure");
            queriesElement.AppendChild(n);
            root.AppendChild(queriesElement);

            // printer settings
            XmlElement formPrintElement = m_formprint.CreateXmlElement(xdoc, "formprint");
            XmlElement gridPrintElement = m_gridprint.CreateXmlElement(xdoc, "gridprint");
            root.AppendChild(formPrintElement);
            root.AppendChild(gridPrintElement);

            // export opts
            XmlElement exportOptsElement = ExportOpts.CreateXmlElement(xdoc, "export");
            root.AppendChild(exportOptsElement);

            this.DoMove(Pager.MoveType.kmGoto, currRow);    // hack for CSBR-128175
            CBVUtil.EndUpdate(this);
        }
        //---------------------------------------------------------------------
        private void LoadFromXmlEx(XmlNode root)
        {
            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name.Equals("connection"))
                {
                    LoadHeaderXmlElement(node);
                }
                else if (node.Name.Equals("tabs"))
                {
                    LoadTabsXmlElement(node);	// creates new tabmanager
                }
                else if (node.Name.Equals("queries"))
                {
                    m_queries = new QueryCollection(m_formdbMgr);
                    m_queries.LoadXmlElement(node);
                    m_queries.LookupMergeQueries();
                }
                else if (node.Name.Equals("formprint"))
                {
                    m_formprint.LoadXmlElement(node);
                }
                else if (node.Name.Equals("gridprint"))
                {
                    m_gridprint.LoadXmlElement(node);
                }
                else if (node.Name.Equals("export"))
                {
                    ExportOpts.LoadXmlElement(node);
                }
            }
        }
        //---------------------------------------------------------------------
        private void LoadTabsXmlElement(XmlNode node)
        {
            m_tabManager = new TabManager(this);
            m_tabManager.LoadXmlElement(node);
        }
        //---------------------------------------------------------------------
        private XmlElement CreateTabsXmlElement(XmlDocument xdoc, String eltname)
        {
            m_tabManager.SaveTabOrdering(MainUltraTabControl);
            XmlElement element = m_tabManager.CreateXmlElement(xdoc, eltname);
            return element;
        }
        //---------------------------------------------------------------------
        private XmlElement CreateHeaderXmlElement(XmlDocument xdoc, String eltname)
        {
            XmlElement element = xdoc.CreateElement(eltname);

            // we no longer use these names, but let's write them out anyway for back-compatibility
            element.SetAttribute("dbappname", m_formdbMgr.AppName);
            element.SetAttribute("tablename", m_formdbMgr.TableName);

            if (!CBVUtil.StrEmpty(m_comments))
                element.SetAttribute("comments", m_comments);

            // new 12/-9: write out the id's
            element.SetAttribute("dbappid", m_formdbMgr.AppID.ToString());
            element.SetAttribute("tableid", m_formdbMgr.TableID.ToString());
            return element;
        }
        //---------------------------------------------------------------------
        private void LoadHeaderXmlElement(XmlNode node)
        {
            m_appName = CBVUtil.GetStrAttrib(node, "dbappname");
            m_tableName = CBVUtil.GetStrAttrib(node, "tablename");
            m_comments = CBVUtil.GetStrAttrib(node, "comments");
            m_appID = CBVUtil.GetIntAttrib(node, "dbappid");
            m_tableID = CBVUtil.GetIntAttrib(node, "tableid");
        }
        //---------------------------------------------------------------------
        #endregion

        #region More query methods
        /// <summary>
        /// Choose query from tree: rerun or restore list
        /// </summary>
        /// <param name="bCreateIfRoot"></param>
        /// <returns></returns>
        private Query GetSelectedQueryFromTree(bool bCreateIfRoot)
        {
            Query q = null;
            UltraExplorerBarGroup gDViews = ultraExplorerBar1.Groups[CBVConstants.QUERIES_GROUPNAME];
            UltraTreeNode selNode = QueryUtil.GetSelectedNodeFromTree(gDViews, m_lastMouseDownPoint);

            // new 9/10: query leaf node now has query ID attached
            // CSBR-133193: avoid crash if no selection
            int nodeTag = (selNode == null || selNode.Tag == null) ? 0 : (int)selNode.Tag;
            if (nodeTag > 0)
            {
                q = m_queries.FindByID(nodeTag);
                if (q != null)
                    return q;
            }

            // the old way
            if (selNode != null)
            {
                if (!String.IsNullOrEmpty(selNode.Text) && selNode.Text.Contains(":"))
                {
                    String queryName = selNode.Text.Substring(0, selNode.Text.IndexOf(':')).Trim();
                    if (queryName.EndsWith("*"))
                        queryName = queryName.Substring(0, queryName.Length - 1);
                    q = m_queries.Find(queryName);
                }
                else if (bCreateIfRoot && selNode.Level == 0)
                {
                    q = m_queries.RetrieveAllQuery;
                }
            }
            return q;
        }
        //---------------------------------------------------------------------
        private Query MergeWithFullList(Query q, MergeQuery.LogicChoice choice)
        {
            switch (choice)
            {
                case MergeQuery.LogicChoice.kmIntersect:
                    return q;
                case MergeQuery.LogicChoice.kmSubtract:     // q - full ... always gets no hits
                    return null;
                case MergeQuery.LogicChoice.kmSubtractFrom: // full - q ... don't know how to do this
                    return null;
                case MergeQuery.LogicChoice.kmUnion:
                    return m_queries.RetrieveAllQuery;
            }
            return null;
        }
        //---------------------------------------------------------------------
        private void MergeQueries(Query q1, Query q2, MergeQuery.LogicChoice choice)
        {
            // combine given queries, create a new one with the result and add to tree
            // if one is full list (RAQ), take special action
            if (q1 is RetrieveAllQuery || q2 is RetrieveAllQuery)
            {
                Debug.Assert(q1 is RetrieveAllQuery != q2 is RetrieveAllQuery); // can't BOTH be full list
                Query qResult = MergeWithFullList(q1 is RetrieveAllQuery ? q2 : q1, choice);
                if (qResult == null)
                {
                    String msg = "Subtraction involving the full list is not supported in this version.";
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                ActivateQueryOnTree(qResult);
                qResult.RestoreHitlist();
                m_currQuery = qResult;
                m_tabManager.Bind(qResult, BindingSource);
                return;
            }

            CBVStatMessage.Show("Merging lists");
            MergeQuery mergeQuery = new MergeQuery(FormDbMgr, q1, q2, choice, m_queries);
            try //CSBR-111886 : Add try/catch block to get error report
            {
                mergeQuery.Run();

                m_currQuery = mergeQuery;
                this.DoMove(Pager.MoveType.kmFirst);

                mergeQuery.Flag(Query.QueryFlag.kfDiscard, true);  // mark with * until saved
                AddQueryToTree(mergeQuery, true);
                m_tabManager.Bind(mergeQuery, BindingSource);
                if (SearchCompleted != null)
                    SearchCompleted.Invoke(this, new EventArgs());
            }
            catch (FormDBLib.Exceptions.SearchException ex)
            {
                CBVUtil.ReportError(ex);
            }
            catch (FormDBLib.Exceptions.NoHitsException ex)
            {
                CBVUtil.ReportWarning(ex, string.Empty);
            }
            CBVStatMessage.Hide();
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Reruns the selected query.
        /// </summary>
        private void RerunQuery()
        {
            Query selQuery = GetSelectedQueryFromTree(true);
            RerunQuery(selQuery);
        }
        //---------------------------------------------------------------------
        private void RerunQuery(Query selQuery)
        {
            try
            {
                Query newQuery = null;
                if (selQuery != null /*&& BindingSource != null*/)  // if bs is null, gets created in TabMgr.Bind below
                {
                    if (selQuery.IsMergeQuery)
                        throw new Exception("Cannot rerun a merge query");

                    // make a new query if this one has a saved hitlist
                    if (selQuery.IsSaved)
                    {
                        newQuery = new Query(selQuery, true); // no good if merged query! t => detach from hitlist
                        newQuery.Flag(Query.QueryFlag.kfDiscard, true);
                        newQuery.Name = String.Empty;
                        selQuery = newQuery;
                    }

                    this.ActivateQueryOnTree(selQuery);

                    // we may have a hitlist, a restorable query, both, or neither
                    // let's just always run, to be safe
                    selQuery.Run();

                    m_currQuery = selQuery;
                    m_tabManager.Bind(selQuery, BindingSource);
                    if (newQuery != null)
                        AddQueryToTree(newQuery, true);

                    if (SearchCompleted != null)
                        SearchCompleted.Invoke(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex);
            }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Other methods
        /// <summary>
        ///   Adds images to the left side of the tree nodes - for dataviews and base tables
        /// </summary>
        private void SetDVIcons()
        {
            if (m_ultraTreeView.Nodes.Count > 0)
            {
                // Dataview
                for (int i = 0; i < m_ultraTreeView.Nodes.Count; i++)
                {
                    m_ultraTreeView.Nodes[i].LeftImages.Add(ChemBioViz.NET.Properties.Resources.Database);
                    if (m_ultraTreeView.Nodes[i].Nodes.Count > 0)
                    {
                        for (int j = 0; j < m_ultraTreeView.Nodes[i].Nodes.Count; j++)
                        {
                            // lookup folder
                            if (String.Equals(m_ultraTreeView.Nodes[i].Nodes[j].Text, CBVConstants.LOOKUP_NAME))
                            {
                                m_ultraTreeView.Nodes[i].Nodes[j].LeftImages.Add(ChemBioViz.NET.Properties.Resources.Folder_Yellow);
                                if (m_ultraTreeView.Nodes[i].Nodes[j].Nodes.Count > 0)
                                {
                                    //  Child tables
                                    this.SetChildTableDVIcons(m_ultraTreeView.Nodes[i].Nodes[j].Nodes);
                                }
                            }
                            else
                            {
                                // base table
                                m_ultraTreeView.Nodes[i].Nodes[j].LeftImages.Add(ChemBioViz.NET.Properties.Resources.BaseGrid);
                                if (m_ultraTreeView.Nodes[i].Nodes[j].Nodes.Count > 0)
                                {
                                    //  Child tables
                                    this.SetChildTableDVIcons(m_ultraTreeView.Nodes[i].Nodes[j].Nodes);
                                }
                            }
                        }
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///   Adds images to the left side of the tree nodes - for child tables
        /// </summary>
        /// <param name="subtree"></param>
        private void SetChildTableDVIcons(TreeNodesCollection subtree)
        {
            for (int k = 0; k < subtree.Count; k++)
            {
                subtree[k].LeftImages.Add(ChemBioViz.NET.Properties.Resources.Grid);
                if (subtree[k].Nodes.Count > 0)
                {
                    //  Child tables
                    this.SetChildTableDVIcons(subtree[k].Nodes);
                }
            }
        }
        //---------------------------------------------------------------------
        private bool ConfigureSettingsOnClosing(bool bClosingWhileEditing)
        {
            if (this.IsChildForm || this.IsChildDocForm)
                return true;

            if (!bClosingWhileEditing)
            {
                Properties.Settings.Default.MainWindowMaximized = (this.WindowState == FormWindowState.Maximized) ? true : false;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.MainFormHeight = this.Height;
                    Properties.Settings.Default.MainFormWidth = this.Width;
                    Properties.Settings.Default.MainFormLeft = this.Left;
                    Properties.Settings.Default.MainFormTop = this.Top;
                }
            }

            String lastOpened = this.IsLocal ? m_localFilePath : FormName;
            PreferencesHelper.PreferencesHelperInstance.StoreLastOpenedForm(m_formID, lastOpened, this.FormType);

            this.SaveInfraSettings();
            //Always save tree structures
            PreferencesHelper.PreferencesHelperInstance.TreeConfig = m_FTreeConf.SerializeTreeConfig(CBVConstants.PUBLIC_GROUPNAME, PreferencesHelper.PreferencesHelperInstance.TreeConfig);
            PreferencesHelper.PreferencesHelperInstance.TreeConfig = m_FTreeConf.SerializeTreeConfig(CBVConstants.PRIVATE_GROUPNAME, PreferencesHelper.PreferencesHelperInstance.TreeConfig);

            // copy addin settings to prefs string for saving
            String sAddinSettings = m_addinMgr != null ? m_addinMgr.GetAddinsXml() : string.Empty;
            Properties.Settings.Default.AddinSettings = sAddinSettings;
            PreferencesHelper.PreferencesHelperInstance.SaveAllPreferences();

            bool ret = true;
            if (PreferencesHelper.PreferencesHelperInstance.GetPrefsFromServer)
                ret = PreferencesHelper.PreferencesHelperInstance.StoreSettingsOnServer();  // false if cancelled
            return ret;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///   Save the user customizations made over Infragistics controls
        /// </summary>
        private void SaveInfraSettings()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                this.mainFormUltraToolbarsManager.SaveAsXml((Stream)stream, true);
                PreferencesHelper.PreferencesHelperInstance.SaveUserCustomization(stream);
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Open ChemBioVizHelp.chm help file
        /// </summary>
        private void OpenHelp()
        {
            FormDbMgr formdbMgr = this.FormDbMgr;
            String sServer = formdbMgr.Login.Server;
            bool bIs2Tier = CBVUtil.StartsWith(sServer, "2-tier");

            if (bIs2Tier)
            {
                string fileName = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Help"), CBVConstants.HELP_FILE);
                if (File.Exists(fileName))
                    Help.ShowHelp(this, fileName);
                else
                    MessageBox.Show("There is no help file. Contact ChemBioViz administrator", "No help file found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                String sProtocol = "http";
                if (formdbMgr.Login.IsSSL)
                    sProtocol += "s";
                string fileName = sProtocol + "://" + sServer + CBVConstants.HELP_URL;
                Help.ShowHelp(this, fileName);
            }
        }
        //---------------------------------------------------------------------
        public void LoadLocalForm(String sPath)
        {
            // read file into xml string, then create form from it
            StreamReader reader = new StreamReader(sPath);
            String sXml = reader.ReadToEnd();
            reader.Close();
            if (sXml.Length == 0)
                throw new Exception("Local file not found: " + sPath);

            UnloadForm();
            m_localFilePath = sPath;
            // there is no ID for local forms
            if (LoadForm(sXml, sPath, -1))
            {
                FormName = Path.GetFileName(sPath);
                FormType = formType.Local;
                DeactivateTreeNode();
                SetApplicationTitle();
                SetNavigationPaneTitle();
            }
            else
                throw new Exception("Error loading local form: " + sPath);
        }
        //---------------------------------------------------------------------
        public void CreateAddinMenu(IAddinMenu imenu)
        {
            RootToolsCollection rootTools = mainFormUltraToolbarsManager.Tools;
            ToolsCollection menuTools = mainFormUltraToolbarsManager.Toolbars[CBVConstants.TOOLBAR_MAIN].Tools;

            // if there is already a menu by this title, get rid of it and its contents
            String menuName = imenu.Title;
            RemoveMenuByName(menuName);
            foreach (IAddinMenuItem item in imenu.Items)
                if (!item.Separator)
                    RemoveMenuByName(item.Command);

            // create new popup menu, add to main toolbar
            PopupMenuTool addinPopup = new PopupMenuTool(menuName);
            rootTools.Add(addinPopup);
            addinPopup.SharedProps.Caption = menuName;
            addinPopup.CustomizedCaption = menuName;
            addinPopup.AllowTearaway = true;
            addinPopup.Settings.PopupStyle = Infragistics.Win.UltraWinToolbars.PopupStyle.Menu;
            menuTools.AddTool(menuName);

            // add menu items and separators
            bool bInsertSeparator = false;
            foreach (IAddinMenuItem item in imenu.Items)
            {
                if (item.Separator)
                {
                    bInsertSeparator = true;
                }
                else
                {
                    ButtonTool btool = null;
                    if (item.Checkable)
                    {
                        btool = new StateButtonTool(item.Command);
                        (btool as StateButtonTool).MenuDisplayStyle = StateButtonMenuDisplayStyle.DisplayCheckmark;
                    }
                    else
                    {
                        btool = new ButtonTool(item.Command);
                    }
                    btool.SharedProps.Caption = item.DisplayString;

                    rootTools.Add(btool);
                    addinPopup.Tools.AddTool(btool.Key);

                    if (bInsertSeparator)
                        addinPopup.Tools[item.Command].InstanceProps.IsFirstInGroup = true;
                    bInsertSeparator = false;
                }
            }
        }
        //---------------------------------------------------------------------
        private void CreateActionsMenus()
        {
            RootToolsCollection rootTools = mainFormUltraToolbarsManager.Tools;
            ToolsCollection menuTools = mainFormUltraToolbarsManager.Toolbars[CBVConstants.TOOLBAR_MAIN].Tools;

            List<String> menuNames = TabManager.GetMenuNames();
            foreach (String menuName in menuNames)
            {
                PopupMenuTool actionsPopup = null;
                if (rootTools.IndexOf(menuName) == -1)
                {
                    actionsPopup = new PopupMenuTool(menuName);
                    rootTools.Add(actionsPopup);
                }
                else
                {
                    actionsPopup = rootTools[menuName] as PopupMenuTool;
                }
                //Coverity Bug Fix CID 12955  
                if (actionsPopup != null)
                {
                    actionsPopup.SharedProps.Caption = menuName;
                    menuTools.AddTool(menuName);
                    actionsPopup.BeforeToolDropdown += new BeforeToolDropdownEventHandler(actionsPopup_BeforeToolDropdown);
                }
            }
        }
        //---------------------------------------------------------------------
        private void RemoveMenuByName(String menuName)
        {
            RootToolsCollection rootTools = mainFormUltraToolbarsManager.Tools;
            ToolsCollection menuTools = mainFormUltraToolbarsManager.Toolbars[CBVConstants.TOOLBAR_MAIN].Tools;
            int jFound = menuTools.IndexOf(menuName);
            if (jFound != -1)
            {
                PopupMenuTool popup = menuTools[jFound] as PopupMenuTool;
                if (popup != null)
                {
                    foreach (ToolBase tool in popup.Tools)
                        RemoveMenuByName(tool.Key);
                }
                menuTools.RemoveAt(jFound);
            }
            int iFound = rootTools.IndexOf(menuName);
            if (iFound != -1)
                rootTools.RemoveAt(iFound);
        }
        //---------------------------------------------------------------------
        private bool IsStandardMenu(String s)
        {
            return s.Equals(CBVConstants.MENU_FILE) ||
                    s.Equals(CBVConstants.MENU_EDIT) ||
                    s.Equals(CBVConstants.MENU_HELP) ||
                    s.Equals(CBVConstants.MENU_VIEW) ||
                    s.Equals("Spotfire"); // CSBR-166238 Fixed
        }
        //---------------------------------------------------------------------
        private bool IsActionMenu(String menuName)
        {
            // true if menu name refers to an action menu connected to a button on form
            List<CBVButton> buttons = TabManager.GetButtonList(true);
            foreach (CBVButton c in buttons)
            {
                if (CBVUtil.Eqstrs(c.MenuName, menuName))
                    return true;
            }
            return false;
        }
        //---------------------------------------------------------------------
        private void RemoveNonStandardMenus()
        {
            // remove all but four from main toolbar; leave rootTools alone
            ToolsCollection menuTools = mainFormUltraToolbarsManager.Toolbars[CBVConstants.TOOLBAR_MAIN].Tools;
            for (int i = 0; i < menuTools.Count; ++i)
            {
                PopupMenuTool tool = menuTools[i] as PopupMenuTool;
                if (tool != null && !IsStandardMenu(tool.Key))
                {
                    menuTools.Remove(tool);
                    --i;
                }
            }
        }
        //---------------------------------------------------------------------
        public void RemoveActionMenus()
        {
            // remove all main menu items except the four standard (File, Edit, Help, View)
            ToolsCollection menuTools = mainFormUltraToolbarsManager.Toolbars[CBVConstants.TOOLBAR_MAIN].Tools;
            for (int i = 0; i < menuTools.Count; ++i)
            {
                PopupMenuTool tool = menuTools[i] as PopupMenuTool;
                if (tool != null && IsActionMenu(tool.Key))
                {
                    menuTools.Remove(tool);
                    --i;
                }
            }
        }
        //---------------------------------------------------------------------
        public bool HasActionMenuButtons()
        {
            List<CBVButton> buttons = TabManager.GetButtonList(true);
            return buttons.Count > 0;
        }
        //---------------------------------------------------------------------
        private void RebuildActionsMenu(PopupMenuTool actionsPopup)
        {
            // create menu items for each button on form
            RootToolsCollection rootTools = mainFormUltraToolbarsManager.Tools;
            ToolsCollection menuTools = mainFormUltraToolbarsManager.Toolbars[CBVConstants.TOOLBAR_MAIN].Tools;

            List<ButtonTool> btools = new List<ButtonTool>();
            List<CBVButton> buttons = TabManager.GetButtonList(true);
            foreach (CBVButton c in buttons)
            {
                if (CBVUtil.Eqstrs(c.MenuName, actionsPopup.Key))
                {
                    ButtonTool tool = new ButtonTool(c.Name);
                    tool.SharedProps.Caption = c.Text;
                    btools.Add(tool);
                }
            }
            // add items to menu
            actionsPopup.Tools.Clear();
            foreach (ButtonTool btool in btools)
            {
                btool.ToolClick += new ToolClickEventHandler(btool_ToolClick);
                int iFound = rootTools.IndexOf(btool.Key);
                if (iFound != -1)
                    rootTools.RemoveAt(iFound);
                rootTools.Add(btool);
                actionsPopup.Tools.AddTool(btool.Key);
            }
        }
        //---------------------------------------------------------------------
        void actionsPopup_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            RebuildActionsMenu(sender as PopupMenuTool);
        }
        //---------------------------------------------------------------------
        void btool_ToolClick(object sender, ToolClickEventArgs e)
        {
            ButtonTool btool = sender as ButtonTool;
            CBVButton cbvButton = TabManager.FindButtonByName(btool.Key);
            if (cbvButton == null) return;

            // also in CBVButton_Click (CBVControls.cs)
            RowStack rstack = FormUtil.GetRowStack(cbvButton);
            cbvButton.DoClickAction(e, rstack);
        }
        //---------------------------------------------------------------------
        public void RefreshChildForm(ChemDataGrid cdgrid, String childFormName, int selectedSubRow)
        {
            // create new child form if we don't have one
            CBVChildForm childForm = null;
            if (!this.HasChildForms)
            {
                BindingSource subBS = cdgrid.DataSource as BindingSource;
                if (subBS != null && !String.IsNullOrEmpty(subBS.DataMember))
                {
                    childForm = new CBVChildForm(this, childFormName, subBS.DataMember, cdgrid);
                    this.m_children.Add(childForm);
                    childForm.Show();
                    Application.DoEvents();
                }
            }
            else
            {
                childForm = this.m_children[0];
                childForm.Fill();
            }
            //Coverity Bug Fix CID 12964 
            if (childForm != null)
            {
                childForm.DoMove(Pager.MoveType.kmGotoPageRow, selectedSubRow);
                childForm.BringToFront();
                childForm.ViewRefresh();
                Application.DoEvents();
            }
        }
        //---------------------------------------------------------------------
        public void DoJumpTo(String sArgs)
        {
            // bring up child doc form and search for matches on current hitlist
            // args: target formname, source and target field names
            // example:   "c:\myform.xml /source=CAS /target=CASNO"
            // or:        "c:\myform.xml /link=CAS"    [means source and target fields have same name]
            String s = sArgs;
            String sSource = CBVUtil.ParseQualifier(s, "/S", ref s);
            String sTarget = CBVUtil.ParseQualifier(s, "/T", ref s);
            String sLink = CBVUtil.ParseQualifier(s, "/L", ref s);
            bool bHasSource = !String.IsNullOrEmpty(sSource);
            bool bHasTarget = !String.IsNullOrEmpty(sTarget);
            bool bHasLink = !String.IsNullOrEmpty(sLink);

            // process input into source and target names, if any
            String sFilename = s;
            if (String.IsNullOrEmpty(sFilename))
                throw new Exception("Jump To requires a target form name or path");

            if (bHasLink)                           // link qualifier overrides others => sets source and target to same fldname
                sSource = sTarget = sLink;
            else if (!bHasSource && !bHasTarget)    // no target or source => use shared hitlist (source and target must have same PK)
                ;
            else if (bHasSource && !bHasTarget)     // source but no target: use PK of target (handle in childform)
                ;
            else if (!bHasSource && bHasTarget)     // target but no source: use PK of source (handle here)
                sSource = FormDbMgr.PKFieldName();

            // make sure we have a hitlist
            int hitlistID = this.CurrQuery.HitListID;
            if (hitlistID == 0)
                throw new Exception("Jump To requires a hitlist");
            HitListType hlType = this.CurrQuery.IsSaved ? HitListType.SAVED : HitListType.TEMP;

            // if source field given, convert to ID
            int srcFldID = 0;
            if (!String.IsNullOrEmpty(sSource))
            {
                COEDataView.Field srcFld = FormDbMgr.FindDVFieldByName(FormDbMgr.SelectedDataViewTable, sSource);
                if (srcFld == null)
                    throw new Exception(String.Concat("Invalid source field for jump: ", sSource));
                srcFldID = srcFld.Id;
            }

            // prepare command line
            // use undocumented command-line qualifier: "/HL=hlID,hltype,srcFldID,tgtFldName"
            String hlTypeStr = this.CurrQuery.IsSaved ? "S" : "T";
            String cmdLineArg = String.Format("/HL={0},{1},{2},{3}", hitlistID, hlTypeStr, srcFldID, sTarget);

            // if child window already up, search to match current hitlist
            CBVChildDocForm childDoc = CBVChildDocForm.FindChildDoc(sFilename);
            if (childDoc != null)
            {
                childDoc.BringToFront();
                childDoc.RefreshSearch(hitlistID, cmdLineArg);
            }
            else // otherwise create child window, pass command line to do search
            {
                childDoc = new CBVChildDocForm(this, sFilename, hitlistID, cmdLineArg);
                childDoc.Show();
                Application.DoEvents();
            }
            childDoc.DoMove(Pager.MoveType.kmFirst, 0);
        }
        //---------------------------------------------------------------------
        private void RCToDS()
        {
            // read RC from file, then get one page dataset from it
            // for experimenting with RC
            String filename = "C:\\rc_in.xml";
            String sXML = CBVUtil.FileToString(filename);

            ResultsCriteria rc = new ResultsCriteria();
            rc.GetFromXML(sXML);

            PagingInfo pi = new PagingInfo();
            pi.Start = 1; pi.End = 21; pi.RecordCount = 20;

            COESearch coeSearch = new COESearch();
            DataSet dataSet = coeSearch.GetData(rc, pi, this.FormDbMgr.SelectedDataView);

            String fname = "C:\\dset_test.xml";
            dataSet.WriteXml(fname, XmlWriteMode.WriteSchema);
        }
        //---------------------------------------------------------------------
        private void DebugMenuItemSelected()
        {
            if (IsEditingForm()) EndFormEdit();
            // temporary debug code goes here


            SelectDataForm fwdlg = new SelectDataForm();
            fwdlg.availableDataViews = this.FormDbMgr.DataViews;

            String dvname = this.FormDbMgr.SelectedDataViewBOName;
            COEDataViewBO dataViewBO = this.FormDbMgr.FindDVBOByName(dvname);
            fwdlg.dataViewBO = dataViewBO;

            DialogResult dlgResult = fwdlg.ShowDialog();
            if (dlgResult == DialogResult.Cancel)
                return;

            ResultsCriteria resultsCriteria = fwdlg.resultsCriteria;
            COEDataViewBO dataViewBO_out = fwdlg.dataViewBO;
            CreateFormFromRCAndDV(resultsCriteria, dataViewBO_out);
        }
        //---------------------------------------------------------------------
        private String m_addinArguments = String.Empty;
        public String AddinArguments
        {
            get { return m_addinArguments; }
            set { m_addinArguments = value; }
        }
        //---------------------------------------------------------------------
        public void ExecuteAddin(String sArgs)
        {
            // sArgs start with type (assembly) name, optionally followed by string of args
            // if arg present, call "InitWithString", otherwise "Init"
            // if addin name is followed by ":<method>" then call that method instead

            CBVAddinsManager mgr = m_addinMgr;

            String typeName = CBVUtil.BeforeFirstDelimiter(sArgs, ' ');
            String args = CBVUtil.AfterFirstDelimiter(sArgs, ' ');
            int nArgs = String.IsNullOrEmpty(args) ? 0 : 1;
            String givenMethodName = String.Empty;
            if (typeName.Contains(":"))
            {
                givenMethodName = CBVUtil.AfterDelimiter(typeName, ':');
                typeName = CBVUtil.BeforeFirstDelimiter(typeName, ':');
            }
            String methodName = (nArgs == 0) ? "Execute" : "ExecuteWithString";
            if (!String.IsNullOrEmpty(givenMethodName))
                methodName = givenMethodName;

            CBVAddin addin = mgr.FindByTypeName(typeName);
            if (addin == null)
                throw new Exception(String.Format("Unable to locate addin type {0}", typeName));

            try
            {
                if (nArgs == 0)
                    addin.Invoke(methodName);   // might throw exception
                else if (nArgs == 1)
                    addin.Invoke(methodName, args);
                else
                    Debug.Assert(false);
            }
            catch (AddinException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Edit command handlers
        /// </summary>
        /// <param name="key"></param>
        private void DoEditCommand(String key)
        {
            // command made on Edit menu
            if (IsEditingForm())
            {
                //Coverity Bug Fix CID :12957 
                Designer dsr = null;
                FormViewTab formViewTab = TabManager.CurrentTab as FormViewTab;
                if (formViewTab != null)
                {
                    //coverity fix
                    Control _control = formViewTab.Control;
                    if (_control is FormViewControl)
                    {
                        FormViewControl frmViewControl = (FormViewControl)_control;
                        dsr = frmViewControl.Designer;
                    }
                }
                if (dsr != null)
                {
                    switch (key)
                    {
                        case CBVConstants.MENU_ITEM_UNDO: dsr.Undo(); break;
                        case CBVConstants.MENU_ITEM_REDO: dsr.Redo(); break;
                        case CBVConstants.MENU_ITEM_CUT: dsr.CopyControlsToClipboard();
                            dsr.DeleteSelected(); break;
                        case CBVConstants.MENU_ITEM_COPY: dsr.CopyControlsToClipboard(); break;
                        case CBVConstants.MENU_ITEM_PASTE: dsr.PasteControlsFromClipboard(); break;
                        case CBVConstants.MENU_ITEM_SELECT_ALL: dsr.SelectAll(); break;
                    }
                }
            }
            else
            {
                switch (key)
                {
                    case CBVConstants.MENU_ITEM_UNDO:
                    case CBVConstants.MENU_ITEM_REDO:
                    case CBVConstants.MENU_ITEM_CUT:
                    case CBVConstants.MENU_ITEM_COPY:
                    case CBVConstants.MENU_ITEM_PASTE:
                    case CBVConstants.MENU_ITEM_SELECT_ALL:
                        // these should be passed on to the control with the focus, I guess

                        Debug.WriteLine(String.Concat("EDIT CMD: ", key));
                        break;
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Define printer settings
        /// </summary>
        private void PageSetup()
        {
            FormTab ftab = m_tabManager.GetTab(MainUltraTabControl.SelectedTab.Index);
            if (ftab.IsGridView())
                m_gridprint.PageSetup(this);
            else if (ftab.IsFormView())
            {
                m_formprint.InitializeFormPrintHelper(((FormViewTab)ftab).Control as FormViewControl, this, ftab.IsQueryView());
                m_formprint.PageSetup(this);
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Show print preview or send the record to the printer
        /// </summary>
        private void Print(bool preview, bool bSilent)
        {
            FormTab ftab = m_tabManager.GetTab(MainUltraTabControl.SelectedTab.Index);
            if (ftab.IsGridView())
            {
                m_gridprint.InitializeGridPrintHelper(((GridViewTab)ftab).CDGrid, this);
                m_gridprint.AskPrintingMode();
                if (m_gridprint.PrintCancelled)
                    return;

                if (preview)
                    m_gridprint.PrintPreview();
                else
                    m_gridprint.Print(bSilent);
            }
            else if (ftab.IsFormView())
            {
                m_formprint.InitializeFormPrintHelper(((FormViewTab)ftab).Control as FormViewControl, this, ftab.IsQueryView());
                if (preview)
                    m_formprint.PrintPreview();
                else
                    m_formprint.Print(bSilent);
            }
        }

        #region Dataview Context Menu
        public enum DVNodeType
        {
            DataView = 0,
            Table = 1,
            Child = 2,
            Grandchild = 3,
            Lookup = 4
        }
        //---------------------------------------------------------------------
        private DVNodeType GetNodeType(UltraTreeNode node)
        {
            if (node == null || node.Parent == null) return DVNodeType.DataView;
            if (node.Parent.Parent == null && !String.Equals(node.Text, CBVConstants.LOOKUP_NAME)) return DVNodeType.Table;
            if (String.Equals(node.Text, CBVConstants.LOOKUP_NAME)) return DVNodeType.Lookup;
            if (node.Parent.Parent.Parent == null) return DVNodeType.Child;
            return DVNodeType.Grandchild;
        }
        //---------------------------------------------------------------------
        private String GetParentNameOfType(UltraTreeNode node, DVNodeType type)
        {
            UltraTreeNode curNode = node;
            while (curNode != null && GetNodeType(curNode) != type)
                curNode = curNode.Parent;
            return (curNode == null) ? String.Empty : curNode.Text;
        }
        //---------------------------------------------------------------------
        void UltraTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            m_lastMouseDownPoint = new Point(e.X, e.Y);
        }
        //---------------------------------------------------------------------
        private bool GiveReason(String sName, String sReason, ref String sReturn)
        {
            sReturn = String.Concat(sName, " ", sReason);
            return false;
        }
        //---------------------------------------------------------------------
        private bool CanGenerateForm(UltraTreeNode selNode, ref String sReason)
        {
            if (GetNodeType(selNode) == DVNodeType.DataView)
                return GiveReason(selNode.Text, "is not a table", ref sReason);

            String selDataViewName = GetParentNameOfType(selNode, DVNodeType.DataView);
            bool bHasPK = FormDbMgr.HasPK(selDataViewName, selNode.Text);
            if (!bHasPK)
                return GiveReason(selNode.Text, "does not have a primary key", ref sReason);

            return true;
        }
        //---------------------------------------------------------------------
        private bool CanAddChildNodeAsSubform(UltraTreeNode selNode, ref String sReason)
        {
            // true if node is child which can be added as subform
            if (GetNodeType(selNode) != DVNodeType.Child && GetNodeType(selNode) != DVNodeType.Grandchild)
                return GiveReason(selNode.Text, "is not a child or grandchild table", ref sReason);
            //Coverity Bug Fix : CID 12947 
            bool bMainFormIsOpen = (this.TabManager.CurrentTab is FormViewTab && this.TabManager.CurrentTab.Control is FormViewControl &&
                ((FormViewControl)this.TabManager.CurrentTab.Control).HasContent());


            if (!bMainFormIsOpen)
                return GiveReason("", "form is empty -- cannot add subform", ref sReason);
            String selectedTableName = GetParentNameOfType(selNode, DVNodeType.Table);

            // TableName might not match when Alias does
            bool bIsUnderMain = CBVUtil.Eqstrs(selectedTableName, FormDbMgr.TableName) ||
                                CBVUtil.Eqstrs(selectedTableName, FormDbMgr.TableAlias);

            if (!bIsUnderMain)
                return GiveReason(selNode.Text, "is not a child of current table " + FormDbMgr.TableName, ref sReason);
            if (TabManager.CurrentTab.IsQueryView())
                return GiveReason("", "cannot add subform to query tab", ref sReason);
            if (GetNodeType(selNode) == DVNodeType.Grandchild && !this.FeatEnabler.CanCreateGrandchildForm())
                return GiveReason(selNode.Text, ": grandchild form not allowed in this version", ref sReason);

            return true;
        }
        //---------------------------------------------------------------------
        private void DataViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // dim inappropriate items based on selected node type (dview, table, child, grandchild table)
            // CSBR-133193: avoid crash if no selection
            UltraTree treeView = this.GetTreeFromGroup(ActivePaneName);
            UltraTreeNode selNode = treeView.GetNodeFromPoint(m_lastMouseDownPoint);
            bool canRefreshAll = true, canGenForm = false, canGenFormNS = false, canAddAsSubform = false;
            if (selNode != null)
            {
                DVNodeType nodeType = GetNodeType(selNode);
                String selDataViewName = GetParentNameOfType(selNode, DVNodeType.DataView);
                String selTableName = selNode.Text, sDum = String.Empty;

                canGenForm = CanGenerateForm(selNode, ref sDum);
                canGenFormNS = canGenForm && selNode.Nodes.Count > 0;
                canAddAsSubform = CanAddChildNodeAsSubform(selNode, ref sDum);
            }
            RefreshAllMenuItem.Enabled = canRefreshAll;
            GenerateFormMenuItem.Enabled = canGenForm;
            GenNoSubsMenuItem.Enabled = canGenFormNS;
            AddAsSubformMenuItem.Enabled = canAddAsSubform;
            useDefaultFieldsOnlyToolStripMenuItem.Checked = Properties.Settings.Default.UseDefaultFieldsOnly;
        }
        //---------------------------------------------------------------------
        private void RefreshAllMenuItem_Click(object sender, EventArgs e)
        {
            m_formdbMgr.GetDataViews();
            m_ultraTreeView.Nodes.Clear();
            m_formdbMgr.DataViewsToTree(m_ultraTreeView);
            this.SetDVIcons();

            // attach context menus
            for (int i = 0; i < m_ultraTreeView.Nodes.Count; i++)
                m_ultraTreeView.Nodes[i].Control.ContextMenuStrip = DataViewContextMenuStrip;
        }
        //---------------------------------------------------------------------
        private void GenerateFormMenuItem_Click(object sender, EventArgs e)
        {
            UltraTree treeView = this.GetTreeFromGroup(ActivePaneName);
            UltraTreeNode selNode = treeView.GetNodeFromPoint(m_lastMouseDownPoint);
            if (DbObjectBank.m_dv_ID_name_pair.ContainsValue(selNode.RootNode.ToString()))
            {
                KeyValuePair<int, string> pair = DbObjectBank.m_dv_ID_name_pair.FirstOrDefault(ID_name_pair => ID_name_pair.Value == selNode.RootNode.ToString());
                DbObjectBank.m_AssociatedDataviewID = pair.Key.ToString();
            }
            CreateFormFromDVNode(selNode, true);
        }
        //---------------------------------------------------------------------
        private void GenNoSubsMenuItem_Click(object sender, EventArgs e)
        {
            UltraTree treeView = this.GetTreeFromGroup(ActivePaneName);
            UltraTreeNode selNode = treeView.GetNodeFromPoint(m_lastMouseDownPoint);
            CreateFormFromDVNode(selNode, false);
        }
        //---------------------------------------------------------------------
        private void AddAsSubformMenuItem_Click(object sender, EventArgs e)
        {
            UltraTree treeView = this.GetTreeFromGroup(ActivePaneName);
            UltraTreeNode selNode = treeView.GetNodeFromPoint(m_lastMouseDownPoint);
            FormViewControl fvc = this.TabManager.CurrentTab.Control as FormViewControl;
            if (fvc != null)
                CreateSubformFromDVNode(selNode, fvc);
        }
        //---------------------------------------------------------------------
        private void UseDefaultFieldsOnly_Click(object sender, EventArgs e)
        {
            //Coverity Bug Fix CID 12972 
            Properties.Settings.Default.UseDefaultFieldsOnly = (sender is ToolStripMenuItem) ? (sender as ToolStripMenuItem).Checked : false;
        }
        //---------------------------------------------------------------------
        private void CreateSubformFromDVNode(UltraTreeNode selNode, FormViewControl fvc)
        {
            String subTableName = selNode.Text;
            COEDataView.DataViewTable tSub = this.FormDbMgr.FindDVTableByName(subTableName);
            ResultsCriteria rc = this.FormDbMgr.ResultsCriteria;
            if (rc == null) return;

            bool bAlreadyHasTable = false;
            foreach (ResultsCriteria.ResultsCriteriaTable subTable in rc.Tables)
            {
                if (subTable.Id == tSub.Id)
                {
                    bAlreadyHasTable = true;
                }
            }

            if (!bAlreadyHasTable)
            {
                // add subtable to rc, rerun query
                DataTable dataTable = null;
                Query query = this.CurrQuery;
                if (tSub != null)
                {
                    if (!bAlreadyHasTable)
                        this.FormDbMgr.AddTableToRC(tSub, rc);
                    query.RestoreHitlist();

                    DataSet dataSet = this.Pager.CurrDataSet;
                    String dsTableName = String.Concat("Table_", CBVUtil.IntToStr(tSub.Id));
                    dataTable = dataSet.Tables[dsTableName];
                }
                if (dataTable != null)
                {
                    int gap = 10;
                    Rectangle rForm = fvc.GetFormSpace();
                    Point controlPosition = new Point(rForm.Left, rForm.Bottom + gap);
                    RestrictColumns(dataTable);
                    ChemDataGrid cdGrid = fvc.CreateSubformBoxEx(dataTable, ref controlPosition);
                    // CSBR-154316: Add boxes to query forms accordingly
                    foreach (FormTab tab in m_tabManager.Tabs)
                    {
                        if (tab is QueryViewTab)
                        {
                            FormViewControl qvc = tab.Control as FormViewControl;
                            Rectangle rQForm = qvc.GetFormSpace();
                            int yPosition = rQForm.Bottom + gap;
                            qvc.CreateQueryBoxesForSubtable(cdGrid, ref yPosition);
                        }
                    }
                    m_tabManager.Bind(query, null);
                    // CSBR-153313: Update ResultsCriteria according to possible restriction
                    //  by RestrictColumns (above)
                    m_formdbMgr.ResultsCriteria = FormUtil.FormToResultsCriteria(this);
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Choose from tree: create new form on selected table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_DoubleClick(object sender, EventArgs e)
        {
            String sReason = String.Empty, sFormType = "form";
            UltraTree ultraTree = sender as UltraTree;
            if (ultraTree == null)
                return;
            UltraTreeNode selNode = ultraTree.ActiveNode;
            if (selNode == null) return;

            switch (GetNodeType(selNode))
            {
                case DVNodeType.Table:
                    if (CanGenerateForm(selNode, ref sReason))
                    {
                        SelectNodeInActiveTree(selNode);
                        bool bWithSubforms = false; //  true; ... DG suggestion - default without subforms
                        CreateFormFromDVNode(selNode, bWithSubforms);
                    }
                    break;
                case DVNodeType.Child:
                case DVNodeType.Grandchild:
                    sFormType = "subform";
                    if (CanAddChildNodeAsSubform(selNode, ref sReason))
                    {
                        SelectNodeInActiveTree(selNode);
                        //Coverity Bug Fix CID 12983 
                        FormViewControl formViewControl = this.TabManager.CurrentTab.Control as FormViewControl;
                        if (formViewControl != null)
                        {
                            CreateSubformFromDVNode(selNode, formViewControl);
                        }
                    }
                    break;
                default:
                    return;
            }
            if (!String.IsNullOrEmpty(sReason))
            {
                String sMsg = String.Format("Cannot generate {0}: {1}", sFormType, sReason);
                MessageBox.Show(sMsg, "Form Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //---------------------------------------------------------------------
        private void UnselectNodes(UltraTreeNode selNode)
        {
            selNode.Override.NodeAppearance.FontData.Bold = DefaultableBoolean.False;
            foreach (UltraTreeNode node in selNode.Nodes)
                UnselectNodes(node);
        }
        //---------------------------------------------------------------------
        private void SelectNodeInActiveTree(UltraTreeNode selNode)
        {
            if (selNode.Parent != null)
                UnselectNodes(selNode.Parent);
            selNode.Override.NodeAppearance.FontData.Bold = DefaultableBoolean.True;
            Application.DoEvents();
        }
        //---------------------------------------------------------------------
        private void CreateFormFromRCAndDV(ResultsCriteria resultsCriteria, COEDataViewBO dataViewBO)
        {
            // used with rc generated by Array RC Editor
            this.UnloadForm();

            int dataViewID = dataViewBO.ID;
            m_formdbMgr.SelectDataViewByID(dataViewID);

            Debug.Assert(resultsCriteria.Tables.Count > 0);
            int baseTableID = resultsCriteria.Tables[0].Id;
            m_formdbMgr.SelectDataViewTableByID(baseTableID);
            m_formdbMgr.BaseTableRecordCount = -1;
            int recCount = this.FormDbMgr.BaseTableRecordCount;

            m_formdbMgr.ResultsCriteria = resultsCriteria;
            m_formdbMgr.BaseTableRecordCount = recCount;

            Query query = m_queries.RetrieveAllQuery;
            this.GenerateOrOpenForm(String.Empty, true, query, true);	// t => do generate
        }
        //---------------------------------------------------------------------
        private void CreateFormFromDVNode(UltraTreeNode selNode, bool withSubforms)
        {
            string tableNameSelected = selNode.Text;
            DVNodeType nodeType = GetNodeType(selNode);
            if (!CheckForSaveOnClose())
                return;
            if (nodeType == DVNodeType.DataView)
                return;

            this.UnloadForm();

            this.SetPaneTitle(string.Empty, CBVConstants.EXPLORER_PANE_NAME);
            this.ClearAppTitle();

            // march up to top-level parent
            UltraTreeNode selNodeParent = selNode.Parent;
            while (selNodeParent.Parent != null)
                selNodeParent = selNodeParent.Parent;

            selNodeParent.Expanded = true;
            SelectNodeInActiveTree(selNode);

            string appNameSelected = selNodeParent.Text;
            m_formdbMgr.Select(appNameSelected, tableNameSelected, withSubforms);
            Query query = m_queries.RetrieveAllQuery;
            this.GenerateOrOpenForm(string.Empty, true, query, withSubforms);	// t => do generate
        }
        //---------------------------------------------------------------------
        #endregion
        #endregion
        #endregion

        #region Event Handlers

        #region Form Handlers
        /// <summary>
        /// Load (startup): generate form on a specific data table, or load from cache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChemBioVizForm_Load(object sender, EventArgs e)
        {
            JTest();
            OnLoadForm();
        }
        //---------------------------------------------------------------------
        private void JTest()
        {
            // for small tests prior to login
        }
        //---------------------------------------------------------------------
        public virtual void OnLoadForm()
        {
            // called at start only; always login
            // delay lengthy operations until ChemBioVizForm_Shown below

            m_formdbMgr = new FormDbMgr(this);
            m_formdbMgr.GrandChildFormsOK = this.FeatEnabler.CanCreateGrandchildForm();

            try
            {
                this.ConfigureSettingsOnLoading();  // read local prefs to get login mru list

                // turn off explorer pane if command line requests
                // do this right away, before displaying form
                if (IsChildForm)
                {
                    m_formdbMgr.Login = new Login((this as CBVChildForm).ParentForm.FormDbMgr.Login.MRUList);
                    m_formdbMgr.Login = (this as CBVChildForm).ParentForm.FormDbMgr.Login;
                    this.ultraDockManager1.DockAreas[0].DockAreaPane.Panes[0].Closed = true;
                }
                else if (IsChildDocForm)
                {
                    m_formdbMgr.Login = new Login((this as CBVChildDocForm).ParentForm.FormDbMgr.Login.MRUList);
                    m_formdbMgr.Login = (this as CBVChildDocForm).ParentForm.FormDbMgr.Login;
                    this.ultraDockManager1.DockAreas[0].DockAreaPane.Panes[0].Closed = true;
                }
                else
                {
                    if (m_cmdLine != null && m_cmdLine.NoPanel)
                    {
                        this.ultraDockManager1.DockAreas[0].DockAreaPane.Panes[0].Closed = true;
                    }
                    m_formdbMgr.MRUList = Properties.Settings.Default.LoginMRUList;

                    // login via command line if provided
                    bool bDoLogin = true;		// change temporarily if unable to connect to server
                    if (bDoLogin && m_cmdLine != null && m_cmdLine.HasServer())
                    {
                        if (m_cmdLine.HasLogin())
                        {
                            if (m_formdbMgr.DoLogin(m_cmdLine.LoginName, m_cmdLine.LoginPassword, m_cmdLine.ServerName))
                                bDoLogin = false;
                        }
                        else if (m_cmdLine.HasTicket())
                        {
                            if (m_formdbMgr.DoLoginWithTicket(m_cmdLine.AuthTicket, m_cmdLine.ServerName))
                                bDoLogin = false;
                        }
                    }
                    // otherwise via dialog; returns false if cancelled
                    if (bDoLogin && !m_formdbMgr.LoginIfNeeded())
                    {
                        Application.Exit();		// user cancelled. CSBR-128496: call Exit, not Close so that ChemBioVizForm_FormClosing does no config save
                        return;                 // doesn't seem like this should be needed, but it is
                    }

                    // check for CBV roles
                    // if none assigned, warn but allow proceed with search privilege only [DG thinks we should abort instead]
                    if (!m_formdbMgr.PrivilegeChecker.HasAnyCBVPrivileges())
                    {
                        if (!m_formdbMgr.PrivilegeChecker.ShowNoCBVPrivsMessage())
                        {
                            Application.Exit();
                            return;     // CSBR-132931 -- crashes here
                        }
                        m_formdbMgr.PrivilegeChecker.AllowMinimalCBVPrivileges();
                    }
                }

                Application.DoEvents();
                this.ConfigureMainForm();

                // stash the login mru list in settings in case modified during dialog
                if (m_formdbMgr.Login != null && m_formdbMgr.Login.MRUList != null)
                {
                    m_formdbMgr.Login.MRUList.Trim();   // remove temporary entries with blank username
                    Properties.Settings.Default.LoginMRUList = m_formdbMgr.Login.MRUList;
                }
                RemoveMenuByName("Action"); // CSBR-129166: get rid of Action menu stored in settings if any                
            }
            catch (CustomSettingsProviderException ex)
            {
                CBVUtil.ReportError(ex);
            }
            catch (UICustomizationException ex)
            {
                CBVUtil.ReportError(ex);
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex);
            }
        }
        //---------------------------------------------------------------------
        public void RefreshDataViews()
        {
            // load or reload master list of dataviews
            try
            {
                if (this.IsChildForm)
                {
                    m_formdbMgr.DataViews = (this as CBVChildForm).ParentForm.FormDbMgr.DataViews;
                }
                else if (this.IsChildDocForm)
                {
                    m_formdbMgr.DataViews = (this as CBVChildDocForm).ParentForm.FormDbMgr.DataViews;
                }
                else
                {
                    m_formdbMgr.GetDataViews();
                    m_formdbMgr.DataViewsToTree(m_ultraTreeView);
                    this.SetDVIcons();
                }
            }
            catch (FormDBLib.Exceptions.ObjectBankException ex)
            {
                CBVUtil.ReportError(ex);
            }
        }
        //---------------------------------------------------------------------
        public static String GetFormTypeName(formType ftype)
        {
            switch (ftype)
            {
                case formType.Local: return CBVConstants.LOCAL;
                case formType.Private: return CBVConstants.PRIVATE;
                case formType.Public: return CBVConstants.PUBLIC;
            }
            return "";
        }
        //---------------------------------------------------------------------
        public static formType GetFormTypeFromName(String s)
        {
            switch (s)
            {
                case CBVConstants.LOCAL: return formType.Local;
                case CBVConstants.PRIVATE: return formType.Private;
                case CBVConstants.PUBLIC: return formType.Public;
            }
            return formType.Unknown;
        }
        //---------------------------------------------------------------------
        public static String GetResourceXmlString()
        {
            Assembly _assembly = Assembly.GetAssembly(typeof(ChemBioVizForm));
            String resourceName = "ChemBioViz.NET.Resources.unit_dict.xml";
            Stream xmlStream = (_assembly == null) ? null : _assembly.GetManifestResourceStream(resourceName);
            String s = (xmlStream == null) ? String.Empty : CBVUtil.StreamToString(xmlStream);
            return s;
        }
        //---------------------------------------------------------------------
        private void ChemBioVizForm_Shown(object sender, EventArgs e)
        {
            InitForm();
        }
        //---------------------------------------------------------------------
        public void InitForm()
        {
            try
            {
                // if offline, continue with blank screen
                // 8/09: this no longer works .. do not continue
                if (m_formdbMgr.Login == null)
                {
                    Application.Exit();
                    return;
                }
                canSavePublic = !FormDbMgr.PrivilegeChecker.CanSavePublic;
                // get dataviews from server
                Application.DoEvents();
                CBVTimer.StartTimerWithMessage(true, "Displaying form", true);

                RefreshDataViews();

                // attach context menus .. DONT KNOW IF WE NEED THIS
                for (int i = 0; i < m_ultraTreeView.Nodes.Count; i++)
                    m_ultraTreeView.Nodes[i].Control.ContextMenuStrip = DataViewContextMenuStrip;

                //m_formdbMgr.SaveDataViews();  // uncomment to save a copy of current dataviews
                Application.DoEvents();

                // get formnames from db banks
                m_formdbMgr.CreateObjectBanks();
                this.FormsToPanel(CBVConstants.PUBLIC_GROUPNAME, true);
                Application.DoEvents();

                this.FormsToPanel(CBVConstants.PRIVATE_GROUPNAME, false);
                Application.DoEvents();

                // create plot control panel
                if (this.FeatEnabler.CanPlot())
                    this.CreatePlotControlPane();
                else
                    RemovePlotControlPane();

                // determine which cdax control to use
                DetermineAvailableCDAX();

                // load and configure addins
                m_addinMgr = new CBVAddinsManager(this);
                m_addinMgr.Load();
                m_addinMgr.CreateMenus();

                String sAddinSettings = Properties.Settings.Default.AddinSettings;
                if (!String.IsNullOrEmpty(sAddinSettings))
                    m_addinMgr.SetAddinsXml(sAddinSettings);

                // hide debug menu
#if !DEBUG
            ((PopupMenuTool)mainFormUltraToolbarsManager.Toolbars[CBVConstants.TOOLBAR_MAIN].
                Tools[CBVConstants.MENU_HELP]).Tools[CBVConstants.MENU_ITEM_DEBUG].InstanceProps.Visible = DefaultableBoolean.False;
#endif

                // create query collection before loading form
                m_queries = new QueryCollection(m_formdbMgr);

                // create tabmgr
                if (m_tabManager == null)
                    m_tabManager = new TabManager(this);
                m_tabManager.Clear();

                // check for args on command line
                if (m_cmdLine != null && m_cmdLine.HasPathname())
                {
                    if (m_cmdLine.FormType == formType.Local)
                    {
                        LoadLocalForm(m_cmdLine.Pathname);
                    }
                    else
                    {
                        // load the full node path to the form like: User Forms\TestFolder\CS_demo 
                        LoadFormByPath(m_cmdLine.Pathname);
                    }
                    if (m_cmdLine.DoPrint)
                    {
                        this.Print(false, true);
                    }
                }
                else
                {
                    String openMode = Properties.Settings.Default.OpenMode;
                    bool bLoaded = false;
                    switch (openMode)
                    {
                        case CBVConstants.OPEN_LAST_FORM:
                            {
                                int lastOpenFormID = Properties.Settings.Default.LastOpenFormID;
                                String lastOpenForm = Properties.Settings.Default.LastOpenFormName;
                                bool bIsLastFormValid = !String.IsNullOrEmpty(lastOpenForm) && !lastOpenForm.Equals("Untitled");
                                if (bIsLastFormValid)
                                {
                                    if (CBVUtil.EndsWith(lastOpenForm, ".xml"))
                                        LoadLocalForm(lastOpenForm);
                                    else
                                    {
                                        if (lastOpenFormID > 0)
                                        {
                                            formType ftype = GetFormTypeFromName(Properties.Settings.Default.LastOpenFormType);
                                            LoadFormByID(lastOpenFormID, ftype);
                                        }
                                    }
                                    bLoaded = true;
                                }
                            }
                            break;
                        case CBVConstants.OPEN_DEFAULT_FORM:
                            {
                                int defaultFormID = Properties.Settings.Default.DefaultFormID;
                                if (defaultFormID > 0)
                                {
                                    formType ftype = GetFormTypeFromName(Properties.Settings.Default.DefaultFormType);
                                    LoadFormByID(defaultFormID, ftype);
                                    bLoaded = true;
                                }
                            }
                            break;
                    }
                    if (!bLoaded)
                    {
                        CreateEmptyForm();
                        ClearAppTitle();
                    }
                }
                Application.DoEvents();
            }
            catch (ObjectBankException ex)
            {
                CBVUtil.ReportError(ex);
            }
            catch (FormDBLib.Exceptions.SearchException ex)
            {
                CBVUtil.ReportError(ex);
            }
            catch (Exception ex)
            {
                this.UnloadForm();
                CBVUtil.ReportError(ex, "Form load error");
            }

            CBVTimer.EndTimer();
            Application.DoEvents();
        }
        //---------------------------------------------------------------------
        private void DetermineAvailableCDAX()
        {
            // first try creating a version 12 control
            m_bHasNoCdax = false;
            String sTypeName = "ChemControls.ChemDraw";
            String sAssemblyName = "ChemControls";
            //String sAssemblyName = "ChemControls, Version=12.1.0.0, Culture=neutral, PublicKeyToken=null";
            //String sAssemblyName = "ChemControls, Version=12.1.0.0, Culture=neutral, PublicKeyToken=cb0b5bb9e67d878a";
            Assembly a = Assembly.Load(sAssemblyName);
            Type controlType = a.GetType(sTypeName);
            try
            {
                DualCdaxControl.m_bUsingConst11 = false;
                object o = Activator.CreateInstance(controlType);
            }
            catch (Exception)
            {
                // if that fails. try a const11 one
                try
                {
                    DualCdaxControl.m_bUsingConst11 = true;
                    object o = Activator.CreateInstance(controlType);
                }
                catch (Exception)
                {
                    String sMsg = "This machine does not have a registered ChemDraw ActiveX Control.";
                    m_bHasNoCdax = true;
                    MessageBox.Show(sMsg);
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Asks for saving the form in case it would had been modified
        /// </summary>
        /// <returns></returns>
        public bool CheckForSaveOnClose()
        {
            // if doc has been modified, ask about saving
            // if yes, do the save and return true
            // return false if user cancels, meaning don't proceed to close
            bool ret = true;
            if (Modified)
            {
                if (String.IsNullOrEmpty(FormName))
                    FormName = "Untitled";
                String msg = String.Format("The form '{0}' has changed.\n\nDo you want to save the changes?", FormName);
                DialogResult ans = MessageBox.Show(msg, "Form Modified", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (ans == DialogResult.Cancel)
                    ret = false;
                else if (ans == DialogResult.Yes)
                    ret = SaveMenuItemSelected();
            }
            return ret;
        }
        //---------------------------------------------------------------------
        private void ChemBioVizForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
                return;     // CSBR-128496: being closed due to cancel of login dialog .. no need to save config
            try
            {
                // called on click of close box or during app exit
                bool bWasEditing = IsEditingForm();
                if (bWasEditing)
                    EndFormEdit();

                if (!CheckForSaveOnClose())
                    e.Cancel = true;
                if (!ConfigureSettingsOnClosing(bWasEditing))
                    e.Cancel = true;

                if (e.Cancel != true)
                {
                    CBVStatMessage.StatMessageEvent -= statMsg_event;
                    if (IsChildForm)
                    {
                        ChemBioVizForm parentForm = (this as CBVChildForm).ParentForm;
                        parentForm.RemoveChildForm(this as CBVChildForm);
                    }
                    else
                    {
                        FireCBVFormClosed();
                        UnloadForm();   // found while working on CSBR-129166
                    }
                }
            }
            catch (CustomSettingsProviderException ex)
            {
                CBVUtil.ReportError(ex);
            }
            catch (ObjectBankException ex)
            {
                CBVUtil.ReportError(ex);
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex);
            }
        }
        //---------------------------------------------------------------------
        static private bool m_SizeChanging = false;

        void ChemBioVizForm_SizeChanged(object sender, EventArgs e)
        {
            if (m_SizeChanging)
                return;

            // CSBR-141475: if form is maximizing, set window state of form view to match
            // applies only within form editor
            // CSBR-144053: applies when editing query tab also
            if (TabManager != null && (TabManager.CurrentTab is FormViewTab || TabManager.CurrentTab is QueryViewTab) && this.IsEditingForm())
            {
                FormViewControl fvc = TabManager.CurrentTab.Control as FormViewControl;
                //Coverity Bug Fix CID 12954 
                if (fvc != null )
                {
                    FormWindowState wc = this.WindowState, wcfv = fvc.WindowState;
                    if (wc == FormWindowState.Maximized && wcfv == FormWindowState.Normal)
                    {
                        m_SizeChanging = true;  // prevent recursion
                        fvc.WindowState = FormWindowState.Normal;
                        fvc.WindowState = FormWindowState.Maximized;
                        m_SizeChanging = false;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private void ChemBioVizForm_KeyDown(object sender, KeyEventArgs e)
        {
            // doesn't get called, don't know why
            if (e.KeyCode == Keys.F1)
            {
                OpenHelp();
            }
        }
        //---------------------------------------------------------------------
        #endregion

        /// <summary>
        /// Show a messsage sent from a class outside this one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);
        //---------------------------------------------------------------------
        private void statMsg_event(object sender, CBVEventArgs e)
        {
            // if window is locked due to BeginUpdate call, we must unlock
            LockWindowUpdate(IntPtr.Zero);
            //Coverity Bug Fix CID 13092 
            if (ultraStatusBar.Panels.Count > 0)
            {
                ultraStatusBar.Panels[0].Text = e.Parameter;
                ultraStatusBar.Refresh();
            }
            // TO DO: restore to previous locked state, if we knew how to find it
        }

        #region Grid event handlers
        //---------------------------------------------------------------------
        public void InstallGridEvents(ChemDataGrid cdGrid)
        {
            // add event handlers for actions on infra grid views (subform or grid tab)
            cdGrid.AfterColPosChanged += new AfterColPosChangedEventHandler(cdGrid_AfterColPosChanged);
            cdGrid.MouseDown += new MouseEventHandler(cdGrid_MouseDown);
            cdGrid.DoubleClickCell += new DoubleClickCellEventHandler(GridViewTab.cdGrid_DoubleClickCell);
            cdGrid.MouseDoubleClick += new MouseEventHandler(cdGrid_MouseDoubleClick);
            cdGrid.AfterRowActivate += new EventHandler(cdGrid_AfterRowActivate);
        }
        //---------------------------------------------------------------------
        void cdGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // if we double-clicked a row in a subform, update child form if any
            if (!(sender is ChemDataGrid) || e.Button == MouseButtons.Right)
                return;
            ChemDataGrid cdgrid = sender as ChemDataGrid;
            if (cdgrid == null)
                return;
            bool bIsSubformGrid = cdgrid.IsSubformGrid;
            UIElement ui = cdgrid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
            if (!bIsSubformGrid || !(ui is RowSelectorUIElement))
                return;

            // if subform has child form attached, create or refresh child window
            if (!this.FeatEnabler.CanCreateDrillDownForm())
                return;

            String childFormName = cdgrid.ChildFormName;  // this is copied directly to a command line
            if (!String.IsNullOrEmpty(childFormName))
            {
                //Coverity Bug Fix : CID 12978 
                ChemBioVizForm mainForm = null;
                if(cdgrid.Parent is FormViewControl)
                {
                    mainForm = ((FormViewControl)cdgrid.Parent).Form; 
                }
                if (mainForm != null)
                {
                    int rowNo = cdgrid.Rows.IndexOf(cdgrid.ActiveRow);
                    mainForm.RefreshChildForm(cdgrid, childFormName, rowNo);
                    mainForm.SubRecordChanged.Invoke(mainForm, new SubRecordChangedEventArgs(cdgrid, rowNo));
                }
                
            }
        }
        //---------------------------------------------------------------------
        void cdGrid_AfterRowActivate(object sender, EventArgs e)
        {
            // user has clicked a row in grid or subform or child band of grid
            ChemDataGrid cdgrid = sender as ChemDataGrid;
            if (cdgrid == null)
                return;
            if (cdgrid.Parent is FormViewControl)
            {
                // grid in subform: tell main form that subrecord has changed
                //Coverity Bug Fix CID 12977 
                ChemBioVizForm mainForm = null;
                if(cdgrid.Parent is FormViewControl)
                {
                    mainForm = ((FormViewControl)cdgrid.Parent).Form;
                } 
                if (mainForm != null && cdgrid.IsSubformGrid && mainForm.SubRecordChanged != null)
                {
                    UltraGridRow activeRow = cdgrid.ActiveRow;
                    int rowno = cdgrid.Rows.IndexOf(activeRow);

                    if (rowno == -1)
                    {   // CSBR-134655: user clicked a child row in the subform grid
                        UltraGridRow parentRow = activeRow.ParentRow;
                        if (parentRow != null)
                            rowno = cdgrid.Rows.IndexOf(parentRow);
                    }
                    if (rowno < 0)
                        return;

                    mainForm.SubRecordChanged.Invoke(mainForm, new SubRecordChangedEventArgs(cdgrid, rowno));
                }
            }
            else
            {
                // main grid in tab: change main record to match row just selected
                // CSBR-133438: march upwards from child grid to find parent
                int nSelectedRow = -1;
                UltraGridRow parentRow = cdgrid.ActiveRow;
                while (parentRow != null)
                {
                    nSelectedRow = parentRow.Index;
                    parentRow = parentRow.ParentRow;
                }
                int currMainRow = this.Pager.CurrRow;
                if (nSelectedRow >= 0 && nSelectedRow != currMainRow)
                    this.DoMove(Pager.MoveType.kmGoto, nSelectedRow);
            }
            RefreshBindingNavigator();
        }
        //---------------------------------------------------------------------
        private void cdGrid_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            this.Modified = true;
        }
        //---------------------------------------------------------------------
        private void cdGrid_MouseDown(object sender, MouseEventArgs e)
        {
            // user clicked on a grid; see if it was a right-click on a column header
            // this handler subscribes to the grid mouseDown event

            m_clickedGridHeader = null;
            if (sender is ChemDataGrid)
            {
                ChemDataGrid cdg = sender as ChemDataGrid;
                m_clickedGridHeader = GetClickedHeader(cdg, e);

                if (e.Button == MouseButtons.Right && m_clickedGridHeader != null)
                {
                    // rclick on header: show the context menu
                    Point pScreen = cdg.PointToScreen(new Point(e.X, e.Y));
                    UltraGridColumn ugCol = m_clickedGridHeader.Column;

                    // CSBR-128730: prevent any menu on header of child band within grid view
                    bool bIsChildBand = ugCol.Band.Index > 0;
                    if (true) // (!bIsChildBand)    CSBR-133388: permit rename header in child grids
                    {
                        // hide sort menu on structure col
                        bool bIsStructCol = ugCol.Tag is ChemDrawTag;
                        ShowGridHeaderContextMenu(PointToClient(pScreen), bIsStructCol || bIsChildBand);
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private Infragistics.Win.UltraWinGrid.ColumnHeader GetClickedHeader(ChemDataGrid cdg, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            UIElement ui = cdg.DisplayLayout.UIElement.ElementFromPoint(p);
            Infragistics.Win.UltraWinGrid.ColumnHeader result = (ui == null) ? null :
                ui.SelectableItem as Infragistics.Win.UltraWinGrid.ColumnHeader;
            return result;
        }
        //---------------------------------------------------------------------
        private void ShowGridHeaderContextMenu(Point p, bool bHideSort)
        {
            // create and display context menu for grid header
            ContextMenuStrip cMenu = new ContextMenuStrip();
            ToolStripMenuItem sortAsc = new ToolStripMenuItem(ChemControlsConstants.GRID_CMENU_SORT_ASCENDING);
            ToolStripMenuItem sortDesc = new ToolStripMenuItem(ChemControlsConstants.GRID_CMENU_SORT_DESCENDING);
            ToolStripMenuItem renameHeader = new ToolStripMenuItem(ChemControlsConstants.GRID_CMENU_RENAME_HEADER);
            sortAsc.Click += new EventHandler(sortAsc_Click);
            sortDesc.Click += new EventHandler(sortDesc_Click);
            renameHeader.Click += new EventHandler(renameHeader_Click);

            cMenu.Items.AddRange(new ToolStripItem[] { sortAsc, sortDesc, renameHeader });
            if (bHideSort)
            {
                sortAsc.Visible = false;
                sortDesc.Visible = false;
            }
            cMenu.Show(this, p);
        }
        //---------------------------------------------------------------------
        private void renameHeader_Click(object sender, EventArgs e)
        {
            Debug.Assert(m_clickedGridHeader != null);
            String newName = CBVUtil.PromptForString("Rename column", m_clickedGridHeader.Caption);
            if (!String.IsNullOrEmpty(newName))
            {
                m_clickedGridHeader.Caption = newName;
                Modified = true;    // CSBR-128729
            }
        }
        //-------------------------------------------------------------------------------------
        private void DoSortOnGridClick(object sender, bool bAscending)
        {
            Debug.Assert(m_clickedGridHeader != null);
            UltraGridColumn ugCol = m_clickedGridHeader.Column;
            ChemDataGrid cdg = ugCol.Band.Layout.Grid as ChemDataGrid;
            Debug.Assert(cdg != null);
            //Coverity Bug Fix CID 19015 
            if (cdg != null && cdg.IsSubformGrid)
            {
                ugCol.SortIndicator = bAscending ? SortIndicator.Ascending : SortIndicator.Descending;
            }
            else
            {
                ugCol.SortIndicator = SortIndicator.None;
                String s = ugCol.Key;
                if (!bAscending) s += " D";
                SortOnSortString(s);
            }
        }
        //-------------------------------------------------------------------------------------
        private void sortDesc_Click(object sender, EventArgs e)
        {
            DoSortOnGridClick(sender, false);
        }
        //-------------------------------------------------------------------------------------
        private void sortAsc_Click(object sender, EventArgs e)
        {
            DoSortOnGridClick(sender, true);
        }
        //-------------------------------------------------------------------------------------
        #endregion

        #region Pane event handlers
        /// <summary>
        ///  Handles the click event on any group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraExplorerBar1_GroupClick(object sender, GroupEventArgs e)
        {
            string explorerTitle = e.Group.Key;
            // start or end form editing on click
            bool bClickedFormEdit = e.Group.Text.StartsWith(CBVConstants.EDIT_PREFIX);      // Edit...
            bool bClickedDoneEdit = e.Group.Text.Equals(CBVConstants.EDITING_GROUPNAME);    // Save and Return
            bool bClickedCancel = e.Group.Text.Equals(CBVConstants.CANCELEDIT_GROUPNAME);   // Cancel
            bool bClickedPlotPanel = e.Group.Text.Equals(CBVConstants.PLOT_GROUPNAME);      // Plot Control
            bool bClickedQueryTab = e.Group.Text.Equals(CBVConstants.QUERIES_GROUPNAME);    // query tab
            bool bDoSelectFormInPane = true;

            try
            {
                if (bClickedDoneEdit)
                {
                    this.Modified = true;
                    EndFormEdit();
                }
                else if (bClickedCancel)
                {
                    CancelFormEdit();
                    // for some reason this leaves the User Forms / Cancel button selected
                    // even though it calls RestoreExplorerBar and selects Public Forms
                }
                else if (bClickedFormEdit && !IsEditingForm())
                {
                    if (!FormDbMgr.PrivilegeChecker.CanEditForms)
                    {
                        FormDbMgr.PrivilegeChecker.ShowRestrictionMessage(CBVConstants.CAN_EDIT_FORMS, false);
                        this.RestoreExplorerBar();
                    }
                    else if (BeginFormEdit())
                    {
                        bDoSelectFormInPane = false;
                    }
                }
                else if (!bClickedFormEdit && IsEditingForm())
                {
                    m_groupBeforeEdit = null;   // CSBR-133974; do not return to orig tab unless clicked Done
                    EndFormEdit();
                }
                else if (bClickedPlotPanel)
                {
                    // CSBR-154024: This used to be done in the handler for the
                    //  panel's shown event (as a fix for CSBR-128172), but that
                    //  only happens the first time the panel is shown.
                    // N.B. This results in a call to ChartToDlg
                    ActivatePlotInPane(SelectedPlot);
                }

                if (string.IsNullOrEmpty(FormName))
                {
                    if (string.Equals(e.Group.Key, CBVConstants.FORMEDIT_GROUPNAME, StringComparison.InvariantCultureIgnoreCase)
                        && bDoSelectFormInPane)
                        this.SetPaneTitle(CBVConstants.DATAVIEWS_GROUPNAME, CBVConstants.EXPLORER_PANE_NAME);
                    else
                        this.SetPaneTitle(e.Group.Key, CBVConstants.EXPLORER_PANE_NAME);
                }
                if (bClickedQueryTab)
                    this.ActivateQueryOnTree(this.CurrQuery);
            }
            catch (ArgumentException ex)
            {
                CBVUtil.ReportError(ex, "Invalid item selection");
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex);
            }
        }
        //---------------------------------------------------------------------
        private void ultraExplorerBar1_SelectedGroupChanging(object sender, CancelableGroupEventArgs e)
        {
            // keep track of the current group so it can be restored after editing
            bool bChoseEdit = e.Group.Key.Equals(CBVConstants.FORMEDIT_GROUPNAME);
            if (bChoseEdit)
                m_groupBeforeEdit = (sender as UltraExplorerBar).SelectedGroup;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Reduce the window width after closing the Properties pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ultraDockManager1_AfterPaneButtonClick(object sender, PaneButtonEventArgs e)
        {
            if (string.Equals(e.Pane.Text, CBVConstants.PROPERTIES_PANE_NAME) && string.Equals(e.Button, "Close"))
            {
                this.TopLevelControl.Width -= e.Pane.Size.Width;
            }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Main Menu event handlers
        private void chemBioVizNETHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenHelp();
        }
        //---------------------------------------------------------------------
        private void mainFormUltraToolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            int oldHitListID = 0;
            int newHitListID = 0;
            // if this is an addin menu, let addin handle the command
            if (e.Tool != null && e.Tool.Owner != null && e.Tool.Owner is PopupMenuTool)
            {
                String menuName = (e.Tool.Owner as PopupMenuTool).CaptionResolved;
                CBVAddin addin = this.m_addinMgr.FindByMenuTitle(menuName);
                if (addin != null && addin.Menu != null)
                {
                    if (addin.HandleMenuCommand(e.Tool.Key))
                        return;
                }
            }
            // not an addin: process command
            switch (e.Tool.Key)
            {
                case CBVConstants.MENU_ITEM_OPEN:
                    this.OpenMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_EXPORT_SD:
                    this.ExportOpts.ForExcel = false;   //MN:19-SEP-2013:: CBOE-1877 
                    this.ExportSDMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_EXPORT_DELIM:
                    this.ExportDelimMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_EXPORT_EXCEL:
                     this.ExportOpts.ForExcel = false;   //MN:19-SEP-2013:: CBOE-1877 
                    this.ExportExcelMenuItemSelected();
                    break;
                case CBVConstants.MENU_ITEM_EXPORT_CBVEXCEL:
                    this.ExportCBVExcelMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_CLOSE_FORM:
                    this.CloseMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_SAVE:
                    if (this.CurrQuery != null) //CSBR - 150169
                    {
                        oldHitListID = this.CurrQuery.HitListID;
                        this.SaveMenuItemSelected();
                        newHitListID = this.CurrQuery.HitListID;
                        this.OnCallRefresh(oldHitListID, newHitListID);
                    }
                    else
                        this.SaveMenuItemSelected();

                    break;

                case CBVConstants.MENU_ITEM_SAVE_AS:
                    if (this.CurrQuery != null) //CSBR - 150169
                    {
                        oldHitListID = this.CurrQuery.HitListID;
                        this.SaveAsMenuItemSelected();
                        newHitListID = this.CurrQuery.HitListID;
                        this.OnCallRefresh(oldHitListID, newHitListID);
                    }
                    else
                        this.SaveAsMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_SAVE_LOCAL:
                    if (this.CurrQuery != null) //CSBR - 150169
                    {
                        oldHitListID = this.CurrQuery.HitListID;
                        this.SaveLocalToolMenuItemSelected();
                        newHitListID = this.CurrQuery.HitListID;
                        this.OnCallRefresh(oldHitListID, newHitListID);
                    }
                    else
                        this.SaveLocalToolMenuItemSelected();

                    break;

                case CBVConstants.MENU_ITEM_REVERT_EDITS:
                    this.RevertEditsMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_PREFERENCES:
                    this.PreferencesMenuItemSelected(false);
                    break;

                case CBVConstants.MENU_ITEM_PAGE_SETUP:
                    this.PageSetup();
                    break;

                case CBVConstants.MENU_ITEM_PRINT_PREVIEW:
                    this.Print(true, false);
                    break;

                case CBVConstants.MENU_ITEM_PRINT:
                    this.Print(false, false);
                    break;

                case CBVConstants.MENU_ITEM_EXIT:
                    this.ExitMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_USER_GUIDE:
                    this.OpenHelp();
                    break;

                case CBVConstants.MENU_ITEM_ABOUT:
                    this.AboutChemFinderMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_CONNECTION:
                    this.ConnectionMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_DEBUG:
                    this.DebugMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_EDIT_FORM:
                    this.EditFormMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_SORT:
                    this.SortMenuItemSelected();
                    break;

                case CBVConstants.MENU_ITEM_EXPLORER:
                    this.ShowHidePane(!IsPaneShowing(CBVConstants.EXPLORER_PANE_NAME), CBVConstants.EXPLORER_PANE_NAME);
                    break;

                case CBVConstants.MENU_ITEM_TOOLBOX:
                    this.ShowHidePane(!IsPaneShowing(CBVConstants.TOOLBOX_PANE_NAME), CBVConstants.TOOLBOX_PANE_NAME);
                    break;

                case CBVConstants.MENU_ITEM_PROPERTIES:
                    this.ShowHidePane(!IsPaneShowing(CBVConstants.PROPERTIES_PANE_NAME), CBVConstants.PROPERTIES_PANE_NAME);
                    break;

                case CBVConstants.MENU_ITEM_REFRESH:
                    this.ViewRefresh(true); // rebind AND retrieve page
                    break;

                case CBVConstants.MENU_ITEM_VIEWTOOLTIPS:
                    this.OnViewTooltips();
                    break;

                case CBVConstants.MENU_ITEM_UNDO:
                case CBVConstants.MENU_ITEM_REDO:
                case CBVConstants.MENU_ITEM_CUT:
                case CBVConstants.MENU_ITEM_COPY:
                case CBVConstants.MENU_ITEM_PASTE:
                case CBVConstants.MENU_ITEM_SELECT_ALL:
                    this.DoEditCommand(e.Tool.Key);
                    break;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///   This event is raised anytime the user unfolds any menu. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainFormUltraToolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            try
            {
                if (m_tabManager == null) return;
                FormTab ftab = m_tabManager.GetTab(MainUltraTabControl.SelectedTab.Index);

                SetVisibleMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_PRINT_PREVIEW, true);
                SetVisibleMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_PRINT, true);

                //  Prepare for Enable/Disable Print and PrintPreview menu items too later on for Form View
                if (ftab != null)
                {
                    EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_PRINT_PREVIEW, true);
                    EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_PRINT, true);
                }
                if (this.FormName == "" || this.FormName == null)
                {
                    this.EnableMenusOnSave(false);
                }
                else
                {
                    this.EnableMenusOnSave(true);
                }

                // dim Revert menu unless we are on form tab and revertible
                bool bCanRevert = CanRevertEdits();
                EnableMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_REVERT_EDITS, bCanRevert);

                if (!this.FeatEnabler.CanExport())
                {
                    SetVisibleMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, false);
                    SetVisibleSubMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, CBVConstants.MENU_ITEM_EXPORT_SD, false);
                    SetVisibleSubMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, CBVConstants.MENU_ITEM_EXPORT_DELIM, false);
                    SetVisibleSubMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, CBVConstants.MENU_ITEM_EXPORT_EXCEL, false);
                    SetVisibleSubMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, CBVConstants.MENU_ITEM_EXPORT_CBVEXCEL, false);
                }
                else //CSBR-152804: Impossible to export from CBV.Net.To set visible "Export" menu after it is disabled in Settings file for any reason.
                {
                    SetVisibleMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, true);
                    SetVisibleSubMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, CBVConstants.MENU_ITEM_EXPORT_SD, true);
                    SetVisibleSubMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, CBVConstants.MENU_ITEM_EXPORT_DELIM, true);
                    SetVisibleSubMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, CBVConstants.MENU_ITEM_EXPORT_EXCEL, true);
                    SetVisibleSubMenuItem(CBVConstants.TOOLBAR_MAIN, CBVConstants.MENU_FILE, CBVConstants.MENU_ITEM_EXPORT, CBVConstants.MENU_ITEM_EXPORT_CBVEXCEL, true);
                }
                // enable/disable addin menus
                this.AddinsManager.EnableMenuCommands();

                // CSBR-129891: do not go by form name, use presence of boxes instead
                //Coverity Bug Fix CID 12979 
                bool bIsEmpty = false;
                if(this.TabManager.CurrentTab is FormViewTab)
                {
                    FormViewTab frmViewTab = (FormViewTab)this.TabManager.CurrentTab;
                    FormViewControl frmViewControl = frmViewTab.Control as FormViewControl;
                    if(frmViewControl != null)
                    {
                        bIsEmpty = !frmViewControl.HasContent();
                    }
                }
                if (!bIsEmpty)
                    this.EnableMenu(true);
                else
                    this.EnableMenu(false);
            }
            catch (Exception)
            {
            }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Context Menus event handlers

        private void queryContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            Query selQuery = GetSelectedQueryFromTree(false);
            bool bCanRerun = false, bCanRestoreQuery = false, bCanRestoreList = false;
            bool bIsFullList = selQuery is RetrieveAllQuery;

            if (m_QTreeConf.HasNonKeptQueries(m_QTreeConf.MainTree))
                queryTree_keepAllToolStripMenuItem.Enabled = true;
            else
                queryTree_keepAllToolStripMenuItem.Enabled = false;

            queryTree_propertiesToolStripMenuItem.Enabled = selQuery != null;
            if (selQuery != null)
            {
                runOnOpenToolStripMenuItem.Enabled = !bIsFullList;
                runOnOpenToolStripMenuItem.Checked = selQuery.ID == this.QueryCollection.QueryOnOpen;
                queryTree_keepToolStripMenuItem.Enabled = !bIsFullList;
                queryTree_keepToolStripMenuItem.Checked = !selQuery.IsFlagged(Query.QueryFlag.kfDiscard);
                if (bIsFullList)
                {
                    runOnOpenToolStripMenuItem.Checked = this.QueryCollection.QueryOnOpen == 0;
                    queryTree_keepToolStripMenuItem.Checked = false;
                }

                FormViewControl formView = GetQueryFormView(selQuery);
                bCanRestoreQuery = selQuery.CanRestoreToForm(formView);
                bCanRestoreList = selQuery.CanRestoreHitlist;
                bCanRerun = selQuery.CanRerun(formView);
            }
            int qIDSearchOver = this.QueryCollection.QuerySearchOver;
            searchOverThisToolStripMenuItem.Checked = (qIDSearchOver != 0) &&
                    (selQuery != null) && (selQuery.ID == qIDSearchOver) && !bIsFullList;

            searchOverThisToolStripMenuItem.Enabled = !bIsFullList;
            DeleteQueryMenuItem.Enabled = !bIsFullList && selQuery != null;

            if (m_qToMerge != null)
            {
                queryTree_selectForMergeToolStripMenuItem.Visible = false;
                if (m_qToMerge != GetSelectedQueryFromTree(false))
                {
                    queryTree_mergeWithToolStripMenuItem.Visible = true;
                    queryTree_mergeWithToolStripMenuItem.Enabled = true;
                    queryTree_mergeWithToolStripMenuItem.Text = string.Concat("Merge with ", m_qToMerge.GetQueryText());
                }
                else
                {
                    queryTree_mergeWithToolStripMenuItem.Visible = false;
                }
            }
            else
            {
                if (selQuery != null)
                {
                    queryTree_selectForMergeToolStripMenuItem.Visible = true;
                    queryTree_mergeWithToolStripMenuItem.Visible = false;
                }
            }

            RerunQueryMenuItem.Enabled = bCanRerun;
            RestoreQueryMenuItem.Enabled = bCanRestoreQuery;
            restoreHitlistToolStripMenuItem.Enabled = bCanRestoreList;
        }
        //---------------------------------------------------------------------
        private void runOnOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Query selQuery = GetSelectedQueryFromTree(false);
            if (selQuery != null)
            {
                // toggle
                if (selQuery.ID == this.QueryCollection.QueryOnOpen)
                {
                    this.QueryCollection.QueryOnOpen = 0;
                    runOnOpenToolStripMenuItem.Checked = false;
                    if (!(selQuery.SaveFlag))
                        selQuery.Flag(Query.QueryFlag.kfDiscard, true);
                }
                else
                {

                    this.QueryCollection.QueryOnOpen = selQuery.ID;
                    if (!(selQuery.SaveFlag))
                    {
                        selQuery.Flag(Query.QueryFlag.kfDiscard, false);
                    }
                    else
                    {
                        runOnOpenToolStripMenuItem.Checked = selQuery.ID == this.QueryCollection.QueryOnOpen;
                    }

                }
                Modified = true;
                UpdateQueryText(selQuery); // CSBR-165241: Query shown as Temporary even though it is Kept after opting Run Query on Form opening
            }
        }
        //---------------------------------------------------------------------
        private void DoRestoreList(Query selQuery)
        {
            // create a new RC in case we need to do structure highlighting
            m_formdbMgr.ResultsCriteria = FormUtil.FormToResultsCriteria(this);

            selQuery.RestoreHitlist();
            m_currQuery = selQuery;
            m_tabManager.Bind(selQuery, BindingSource);
            ActivateQueryOnTree(selQuery);
        }
        //---------------------------------------------------------------------
        private void restoreHitlistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // retrieve query hitlist
            // throws no hits error on failure to retrieve
            CBVTimer.StartTimerWithMessage(true, "Restoring hitlist", true);

            Query selQuery = null;
            try
            {
                //Coverity Bug Fix :CID 12982 
                if (m_tabManager != null)
                {
                    if (m_tabManager.Count > 0 && m_tabManager.CurrentTab is QueryViewTab)
                        m_tabManager.SelectDefaultTab();  // CSBR-117935

                    selQuery = GetSelectedQueryFromTree(false);
                    if (selQuery != null)
                    {
                        try
                        {
                            DoRestoreList(selQuery);

                            if (SearchCompleted != null)
                                SearchCompleted.Invoke(this, new EventArgs());
                        }
                        catch (FormDBLib.Exceptions.NoHitsException ex)
                        {
                            CBVUtil.ReportWarning(ex, string.Empty);
                        }
                    }
                    else // user chose root: do retrieve all
                    {
                        selQuery = m_queries.RetrieveAllQuery;
                        selQuery.Run();
                        m_currQuery = selQuery;
                        m_tabManager.Bind(selQuery, BindingSource);
                        ActivateQueryOnTree((Query)null);
                    }
                }
                CBVTimer.EndTimer();
            }
            catch (FormDBLib.Exceptions.NoHitsException ex)
            {
                CBVUtil.ReportWarning(ex, String.Concat(selQuery.Name, " ", selQuery.Description));
            }
            catch (FormDBLib.Exceptions.SearchException ex)
            {
                CBVUtil.ReportError(ex);
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex);
            }
            CBVTimer.EndTimer();
        }
        //---------------------------------------------------------------------
        private void retrieveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CBVTimer.StartTimerWithMessage(true, "Retrieve all", true);
            try
            {
                Query selQuery = m_queries.RetrieveAllQuery;
                selQuery.Run();
                m_currQuery = selQuery;
                m_tabManager.Bind(selQuery, BindingSource);
                ActivateQueryOnTree((Query)null);
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex);
            }
            CBVTimer.EndTimer();
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Handles the click event on Rerun item in the Query tree context menu. It reruns the selected query.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RerunQueryMenuItem_Click(object sender, EventArgs e)
        {
            if (m_tabManager != null && m_tabManager.Count > 0 && m_tabManager.CurrentTab is QueryViewTab)
                m_tabManager.SelectDefaultTab();  // CSBR-117935
            this.RerunQuery();
        }
        //---------------------------------------------------------------------
        private void RestoreQueryMenuItem_Click(object sender, EventArgs e)
        {
            // get query selected in tree; bring up query tab; restore
            Query q = GetSelectedQueryFromTree(false);
            if (q is MergeQuery || q == null || String.IsNullOrEmpty(q.TabName))
            {
                MessageBox.Show(CBVConstants.QUERY_NORESTORE);  // CSBR-112119
            }
            else
            {
                FormViewControl fview = GetQueryFormView(q);
                if (fview != null)
                {
                    int index = TabManager.FindTab(q.TabName);
                    TabManager.SelectTab(index);
                    //Coverity Bug Fix : CID  12967 
                    QueryViewTab queryViewTab = TabManager.CurrentTab as QueryViewTab;
                    if (queryViewTab != null)
                    {
                        queryViewTab.ClearQueryForm();
                        queryViewTab.RefreshFromQuery(q);    // CSBR-135585: set sss combo to match query
                    }
                    //q.RestoreToForm(fview);
                    RestoreQueryToForm(q, fview);
                }
            }
        }
        //---------------------------------------------------------------------
        private void SetUnitsCombo(FormViewControl fview, QueryComponent qcomp, Control c)
        {
            // if box c has a units combo attached, and qcomp has units specified, select the item in the combo
            if (!(c is CBVQueryTextBox) || String.IsNullOrEmpty((c as CBVQueryTextBox).Units)) return;
            if (String.IsNullOrEmpty(qcomp.SelectedUnits)) return;

            foreach (Control cc in fview.Controls)
            {
                if (cc is CBVUnitsCombo && CBVUtil.Eqstrs(c.Name, (cc as CBVUnitsCombo).TargetBox))
                {
                    (cc as CBVUnitsCombo).SelectedItem = qcomp.SelectedUnits;
                    break;
                }
            }
        }
        //---------------------------------------------------------------------
        private void RestoreQueryToForm(Query q, FormViewControl fview)
        {
            // code moved here from query.cs so we can deal with units combos
            // goes through the component list, putting each in the appropriate box
            bool bHasStructureComp = false;

            foreach (QueryComponent qcomp in q.Components)
            {
                if (qcomp.Tag == null)
                    continue;

                Control c = String.IsNullOrEmpty(qcomp.BoxName) ? Query.FindControlByTag(fview, qcomp.Tag.ToString()) :
                                Query.FindControlByBoxname(fview, qcomp.BoxName);
                if (c != null)
                {
                    if (c is ChemDraw)
                    {
                        ((ChemDraw)c).Base64 = qcomp.Data;
                        bHasStructureComp = true;
                    }
                    else if (c is CheckBox)
                    {
                        CheckBox cbox = c as CheckBox;
                        cbox.CheckState = CheckState.Checked;
                        String queryStr = qcomp.Data;
                        if (cbox.ThreeState && (queryStr.Equals("= 0") || queryStr.Equals("null")))
                            cbox.CheckState = CheckState.Unchecked;
                    }
                    else
                    {
                        c.Text = qcomp.Data;

                        if (!String.IsNullOrEmpty(qcomp.RawInput))
                            c.Text = qcomp.RawInput;
                        if (!String.IsNullOrEmpty(qcomp.SelectedUnits))
                            SetUnitsCombo(fview, qcomp, c);
                    }
                }
            }
            if (bHasStructureComp)
            {
                // restore global structure search settings to match restored criteria [CSBR-144873]
                SearchCriteria.StructureCriteria sCriteria = Query.FindStructCriteria(q.SearchCriteria);
                if (sCriteria != null)
                    QueryUtil.CopySearchOptions(sCriteria, SearchOptionsSettings.Default, true);
            }
        }
        //---------------------------------------------------------------------
        private void RenameQuery(Query q, TreeLeaf sNode, string newName)
        {
            if (q != null)
            {
                if (!String.IsNullOrEmpty(newName))
                {
                    q.Name = newName;
                    q.Flag(Query.QueryFlag.kfDiscard, false);
                    Modified = true;
                    if (q.Flags == 0)
                        q.SaveFlag = true;
                }
            }
        }
        //---------------------------------------------------------------------
        private void queryTree_keepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // revamped 1/11: simplified
            // toggle the Keep flag and then update the tree item to add or remove *
            Query query = GetSelectedQueryFromTree(false);
            if (query != null)
            {
                query.Flag(Query.QueryFlag.kfDiscard, !query.IsFlagged(Query.QueryFlag.kfDiscard));
                UpdateQueryText(query);
                Modified = true;
                if (query.Flags == 0)
                    query.SaveFlag = true;
                else
                {
                    query.SaveFlag = false;
                    runOnOpenToolStripMenuItem.Enabled = false;
                    this.QueryCollection.QueryOnOpen = 0;
                }
            }
        }
        //---------------------------------------------------------------------
        private void UpdateQueryText(Query query)
        {
            if (query == null) return;

            //string key = m_QTreeConf.MainTree.GetLeafFromListById(query.ID).Key;  // crash if GLFLBI returns null
            ITreeNode tnode = m_QTreeConf.MainTree.GetLeafFromListById(query.ID);
            if (tnode == null) return;
            string key = tnode.Key;
            UltraTreeNode node = GetTreeFromGroup(CBVConstants.QUERIES_GROUPNAME).GetNodeByKey(key);
            if (node != null)
                node.Text = query.GetQueryText();
        }
        //---------------------------------------------------------------------
        private void queryTree_keepAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Query query in m_queries)
            {
                query.Flag(Query.QueryFlag.kfDiscard, false);
                UpdateQueryText(query);
                if (query.Flags == 0)
                    query.SaveFlag = true;
            }
        }
        //---------------------------------------------------------------------
        private void queryTree_propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Query selQuery = GetSelectedQueryFromTree(false);
            if (selQuery != null)
            {
                String oldSortStr = selQuery.DefaultSortString;     // CSBR-142590: don't use temporary SortString
                QueryDialog dialog = new QueryDialog(selQuery);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //string key = m_QTreeConf.MainTree.GetLeafFromListById(selQuery.ID).Key;  // crash if GLFLBI returns null
                    ITreeNode tnode = m_QTreeConf.MainTree.GetLeafFromListById(selQuery.ID);
                    if (tnode == null) return;
                    string key = tnode.Key;
                    if (!string.IsNullOrEmpty(key))
                    {
                        UltraTreeNode node = GetTreeFromGroup(CBVConstants.QUERIES_GROUPNAME).GetNodeByKey(key);
                        if (node != null)
                        {
                            node.Text = selQuery.GetQueryText();
                            Modified = true;
                        }
                    }
                    bool bNeedRefresh = !CBVUtil.Eqstrs(oldSortStr, selQuery.DefaultSortString) && selQuery == this.CurrQuery;
                    if (bNeedRefresh)
                    {
                        selQuery.SortString = selQuery.DefaultSortString;   // or we could just set SortString blank
                        this.ViewRefresh(true);
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private void queryTree_selectForMergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_qToMerge = GetSelectedQueryFromTree(false);
        }
        //---------------------------------------------------------------------
        private void queryTree_mergeWithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Query q1 = m_qToMerge;
            Query q2 = GetSelectedQueryFromTree(false);
            //Call to Merging Dialog .. if OK, form routine does the merge
            if (q1 != null && q2 != null)
            {
                MergeListDialog mergeDialog = new MergeListDialog(q1, q2);
                if (mergeDialog.ShowDialog() == DialogResult.OK)
                    MergeQueries(q1, q2, mergeDialog.LogicChoice);
            }
            m_qToMerge = null;
        }
        //---------------------------------------------------------------------
        // CREATE QUERY FROM QUERY FORM
        //---------------------------------------------------------------------
        public static Query CreateFromFormEx(ChemBioVizForm form)
        {
            FormQueryExtractor fqe = new FormQueryExtractor(form);
            Query query = fqe.CreateEx();
            return query;
        }
        //---------------------------------------------------------------------
        private void TabContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // do not show context menu unless right-click was on a tab
            if (m_rightClickedTabIndex == -1 || IsEditingForm())    // CSBR-142661
            {
                e.Cancel = true;
                return;
            }
            // prevent deleting last tab
            bool bCanDelete = this.TabManager.Count > 1;
            TabContextMenuStrip.Items[CBVConstants.DELETE_TAB].Enabled = bCanDelete;

            // adjust items in Add Tab submenu based on tab type
            FormTab clickedTab = m_tabManager.GetTab(m_rightClickedTabIndex);
            if (clickedTab != null)
            {
                ToolStripItemCollection items = ((ToolStrip)(((ToolStripDropDownItem)(TabContextMenuStrip.Items[0])).DropDown)).Items;
                if (clickedTab.IsQueryView())
                {
                    items[CBVConstants.DUPLICATE_FORM].Visible = true;
                    items[CBVConstants.GRID_FROM_FORM].Visible = false;
                    items[CBVConstants.QUERY_FORM].Visible = true;
                    items[CBVConstants.NEW_CARDVIEW].Visible = false;
                }
                else if (clickedTab.IsGridView())
                {
                    items[CBVConstants.DUPLICATE_FORM].Visible = false;
                    items[CBVConstants.GRID_FROM_FORM].Visible = false;
                    items[CBVConstants.QUERY_FORM].Visible = false;
                    items[CBVConstants.NEW_CARDVIEW].Visible = false;
                }
                else if (clickedTab.IsFormView())
                {
                    items[CBVConstants.DUPLICATE_FORM].Visible = true;
                    items[CBVConstants.GRID_FROM_FORM].Visible = true;
                    items[CBVConstants.QUERY_FORM].Visible = true;
                    items[CBVConstants.NEW_CARDVIEW].Visible = true;
                }

                // remove Card View item except when clicked on grid tab
                ToolStripMenuItem cardViewItem = TabContextMenuStrip.Items[CBVConstants.TAB_CARDVIEW] as ToolStripMenuItem;
                ToolStripItem cardViewSep = TabContextMenuStrip.Items[CBVConstants.TAB_CARDVIEW_SEP];
                ToolStripMenuItem cardViewStyleItem = TabContextMenuStrip.Items[CBVConstants.TAB_CARDVIEW_STYLE] as ToolStripMenuItem;
                if (cardViewItem != null && cardViewSep != null && cardViewStyleItem != null)
                {
                    cardViewItem.Visible = cardViewSep.Visible = cardViewStyleItem.Visible = false;
                    if (this.FeatEnabler.CanCreateCardView() && clickedTab.IsGridView())
                    {
                        cardViewItem.Visible = cardViewSep.Visible = cardViewStyleItem.Visible = true;
                        ChemDataGrid cdg = clickedTab.Control as ChemDataGrid;
                        // Coverity Bug Fix CID 12971 
                        if (cdg != null)
                        {
                            cardViewItem.Checked = cdg.CardViewMode;

                            // set checkmark on current style
                            CardStyle curStyle = cdg.DisplayLayout.Bands[0].CardSettings.Style;
                            standardToolStripMenuItem.CheckState = (curStyle == CardStyle.StandardLabels) ? CheckState.Checked : CheckState.Unchecked;
                            mergedLabelsToolStripMenuItem.CheckState = (curStyle == CardStyle.MergedLabels) ? CheckState.Checked : CheckState.Unchecked;
                            variableHeightToolStripMenuItem.CheckState = (curStyle == CardStyle.VariableHeight) ? CheckState.Checked : CheckState.Unchecked;
                            compressedToolStripMenuItem.CheckState = (curStyle == CardStyle.Compressed) ? CheckState.Checked : CheckState.Unchecked;

                            // and current structure layout
                            ChemDataGrid.CardViewLayoutType layout = cdg.CardViewLayout;

                            structTopToolStripMenuItem.CheckState = (layout == ChemDataGrid.CardViewLayoutType.strTop) ? CheckState.Checked : CheckState.Unchecked;
                            structBottomToolStripMenuItem.CheckState = (layout == ChemDataGrid.CardViewLayoutType.strBottom) ? CheckState.Checked : CheckState.Unchecked;
                            structLeftToolStripMenuItem.CheckState = (layout == ChemDataGrid.CardViewLayoutType.strLeft) ? CheckState.Checked : CheckState.Unchecked;
                            structRightToolStripMenuItem.CheckState = (layout == ChemDataGrid.CardViewLayoutType.strRight) ? CheckState.Checked : CheckState.Unchecked;
                        }
                    }
                }
                // same for SelectionToList
                ToolStripMenuItem selToListItem = TabContextMenuStrip.Items[CBVConstants.TAB_SELTOLIST] as ToolStripMenuItem;
                if (selToListItem != null)
                {
                    selToListItem.Visible = clickedTab.IsGridView();
                    selToListItem.Enabled = HasSelectedRecords();
                }
            }
        }
        //---------------------------------------------------------------------
        private void cardViewTabContextMenuItem_Click(object sender, EventArgs e)
        {
            FormTab clickedTab = (m_rightClickedTabIndex == -1) ? null : m_tabManager.GetTab(m_rightClickedTabIndex);
            if (clickedTab != null && clickedTab.IsGridView())
            {
                ChemDataGrid cdg = clickedTab.Control as ChemDataGrid;
                //Coverity Bug Fix CID 12976 
                if (cdg != null)
                {
                    bool bIsCardView = cdg.CardViewMode;
                    cdg.CardViewMode = !bIsCardView;
                    Modified = true;
                }
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///   Handles the Click event for the Blank Form menu item from the tab context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BlankTabContextMenuItem_Click(object sender, EventArgs e)
        {
            // create blank tab
            String name = String.Concat("Form", CBVUtil.IntToStr(m_tabManager.Count + 1));
            m_tabManager.CreateBlank(TabManager.TabType.ktForm, name);
            Modified = true;
        }
        //---------------------------------------------------------------------
        private void DuplicateTabContextMenuItem_Click(object sender, EventArgs e)
        {
            // create duplicate of right-clicked form tab (clicked must be form, not grid)
            FormTab clickedTab = (m_rightClickedTabIndex == -1) ? null : m_tabManager.GetTab(m_rightClickedTabIndex);
            if (clickedTab != null && clickedTab.IsFormView())
            {
                m_tabManager.CreateDuplicate(clickedTab);
                m_rightClickedTabIndex = -1;
                Modified = true;
            }
        }
        //---------------------------------------------------------------------
        private void fullGridTabContextMenuItem_Click(object sender, EventArgs e)
        {
            // create new grid tab with all fields (works from any tab)
            FormTab clickedTab = (m_rightClickedTabIndex == -1) ? null : m_tabManager.GetTab(m_rightClickedTabIndex);
            if (clickedTab != null)
            {
                m_tabManager.CreateDuplicateGrid(clickedTab);   // badly named; arg is basically ignored
                m_rightClickedTabIndex = -1;
                Modified = true;
            }
        }
        //---------------------------------------------------------------------
        private void gridTabContextMenuItem_Click(object sender, EventArgs e)
        {
            // create new grid tab from form (clicked must be form, not grid)
            FormTab clickedTab = (m_rightClickedTabIndex == -1) ? null : m_tabManager.GetTab(m_rightClickedTabIndex);
            if (clickedTab != null && clickedTab.IsFormView())
            {
                m_tabManager.CreateGridFromForm(clickedTab);
                m_rightClickedTabIndex = -1;
                Modified = true;
            }
        }
        //---------------------------------------------------------------------
        private void newCardViewTabContextMenuItem_Click(object sender, EventArgs e)
        {
            gridTabContextMenuItem_Click(sender, e);
            GridViewTab gridTab = m_tabManager.CurrentTab as GridViewTab;
            //Cverity Bug Fix CID 12981 
            if (gridTab != null)
            {
                ChemDataGrid chemDataGrid = gridTab.Control as ChemDataGrid;
                if(chemDataGrid != null)
                {
                    chemDataGrid.CardViewMode = true;
                }
            }
        }
        //---------------------------------------------------------------------
        private void QueryFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create new query tab as clone of clicked form or query tab
            FormTab clickedTab = (m_rightClickedTabIndex == -1) ? null : m_tabManager.GetTab(m_rightClickedTabIndex);
            if (clickedTab != null && clickedTab.IsFormView())
            {
                m_tabManager.CreateQueryTabFromForm(clickedTab);
                m_rightClickedTabIndex = -1;
                Modified = true;
            }
        }
        //---------------------------------------------------------------------
        private void RenameTabContextMenuItem_Click(object sender, EventArgs e)
        {
            // rename right-clicked tab; prompt for input
            FormTab clickedTab = (m_rightClickedTabIndex == -1) ? null : m_tabManager.GetTab(m_rightClickedTabIndex);
            if (clickedTab != null)
            {
            reprompt:
                String sCandidate = TabManager.GetUniqueTabName(clickedTab.Name);
                String sNewName = CBVUtil.PromptForString("Tab name:", sCandidate);
                if (!String.IsNullOrEmpty(sNewName))
                {
                    // CSBR-127060: warn if name is in use
                    int iFound = TabManager.FindTab(sNewName);
                    if (iFound != -1)
                    {
                        String sMessage = String.Format("The name '{0}' is already in use.  Do you want to continue?", sNewName);
                        if (MessageBox.Show(sMessage, "Tab Rename", MessageBoxButtons.YesNo) == DialogResult.No)
                            goto reprompt;
                    }
                    clickedTab.Name = sNewName;
                    m_tabManager.BuildTabControl();
                }
                m_rightClickedTabIndex = -1;
                Modified = true;
            }
        }
        //---------------------------------------------------------------------
        private void DeleteTabContextMenuItem_Click(object sender, EventArgs e)
        {
            // delete right-clicked tab
            if (m_rightClickedTabIndex >= 0)
            {
                FormTab clickedTab = m_tabManager.GetTab(m_rightClickedTabIndex);
                String tabName = clickedTab.Name;
                if (MessageBox.Show(String.Concat("Are you sure you want to delete tab '",
                        tabName, "'?"), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;

                m_tabManager.DeleteTab(m_rightClickedTabIndex);
                m_tabManager.BuildTabControl();
                m_rightClickedTabIndex = -1;
                Modified = true;
            }
        }
        //---------------------------------------------------------------------
        private bool AlertIfNameInUse(String name)
        {
            if (ActiveDBBank.HasName(name) &&
                MessageBox.Show(String.Concat("Form name '", name, "' is in use.  Overwrite?"),
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return false;
            return true;
        }
        //---------------------------------------------------------------------
        private void PropertiesContextMenuItem_Click(object sender, EventArgs e)
        {
            // form properties .. modified SaveAs dialog
            ITreeNode sNode = m_FTreeConf.MainTree.GetNodeFromListByKey(GetTreeNodeKey());
            if (sNode == null)
                return;
            string nodeName = sNode.Name;
            string nodeKey = sNode.Key;
            string dTitle = (sNode is TreeNode) ? "Folder properties" : "Form properties";

            UltraTree tree = GetTreeFromGroup(ActivePaneName);
            UltraTreeNode sUINode = tree.GetNodeByKey(GetTreeNodeKey());

            if (sNode is TreeLeaf)
            {
                if (IsFormsTree)
                {
                    bool bChosenFormIsCurrent = nodeName.Equals(this.FormName);
                    String oldComments = bChosenFormIsCurrent ? this.Comments :
                                    ActiveDBBank.GetObjectDataItem(nodeName, "//cbvnform/connection/@comments");
                    SaveDialog propsDialog = new SaveDialog(nodeName, oldComments, IsPublicPaneActive, true, dTitle);
                    if (propsDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        String sNewName = propsDialog.FileName;
                        bool bChangedName = !nodeName.Equals(sNewName);
                        bool bChangedComments = !string.IsNullOrEmpty(oldComments) ? !oldComments.Equals(propsDialog.Comments) : true;
                        if (!bChangedName && !bChangedComments)
                            return;

                        // check privilege if form is public and we are going to modify server
                        bool bChangedCurrCommentsOnly = bChangedComments && bChosenFormIsCurrent && !bChangedName;
                        if (IsPublicPaneActive && !FormDbMgr.PrivilegeChecker.CanSavePublic && !bChangedCurrCommentsOnly)
                        {
                            FormDbMgr.PrivilegeChecker.ShowRestrictionMessage(CBVConstants.CAN_SAVE_PUBLIC_FORM, false);
                            return;
                        }
                        // warn if new name is in use; return if cancelled
                        if (bChangedName && !AlertIfNameInUse(sNewName))
                            return;
                        // server update is required unless changed comments of curr form only
                        Application.DoEvents();
                        CBVTimer.StartTimerWithMessage(true, "Updating form data on server", true);

                        String sChangedName = bChangedName ? sNewName : "";
                        String sChangedComm = (bChangedComments && !bChosenFormIsCurrent) ? propsDialog.Comments : "";

                        if (!String.IsNullOrEmpty(sChangedName) || !String.IsNullOrEmpty(sChangedComm))
                            this.ActiveDBBank.UpdateObjectDataItem(nodeName, "//cbvnform/connection", "comments",
                                                                    sChangedComm, sChangedName);
                        if (bChosenFormIsCurrent)
                        {
                            if (bChangedComments)
                            {
                                this.Comments = propsDialog.Comments;
                                this.Modified = true;
                            }
                            this.FormName = sNewName;
                        }
                        UpdateNodeInPanel(sUINode, sNewName);
                        m_FTreeConf.UpdateNode(nodeKey, sNewName, propsDialog.Comments);
                        CBVTimer.EndTimer();
                    }
                }
            }
            else
            {
                String oldComments = sNode.Comments;
                SaveDialog propsDialog = new SaveDialog(nodeName, oldComments, IsPublicPaneActive, true, dTitle);
                if (propsDialog.ShowDialog(this) == DialogResult.OK)
                {
                    String sNewName = propsDialog.FileName;
                    bool bChangedName = !nodeName.Equals(sNewName);
                    bool bChangedComments = !string.IsNullOrEmpty(oldComments) ? !sNode.Comments.Equals(propsDialog.Comments) : true;
                    if (bChangedName)
                    {
                        sNewName = VerifyNewName(sUINode.Parent, sNewName, sNode.Name);
                        if (!string.IsNullOrEmpty(sNewName))
                            UpdateNodeInPanel(sUINode, sNewName);
                    }
                    if (bChangedName || bChangedComments)
                        m_FTreeConf.UpdateNode(nodeKey, sNewName, propsDialog.Comments);
                }
            }
        }
        //---------------------------------------------------------------------
        private void RenameTreeNodeContextMenuItem_Click(object sender, EventArgs e)
        {
            TreeConfig treeConf = null;
            if (ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                treeConf = m_FTreeConf;
            else if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))

                treeConf = m_QTreeConf;

            // get the node to rename
            //Coverity Bug Fix : CID 12965 
            ITreeNode sNode = null;
            if (treeConf != null)
            {
                TreeNode root = (TreeNode)treeConf.MainTree.GetNodeFromListByName(ActivePaneName);
                if (root != null)
                    sNode = root.GetNodeFromListByKey(GetTreeNodeKey());
                else
                    sNode = treeConf.MainTree.GetNodeFromListByKey(GetTreeNodeKey());
            }
            if (sNode == null)
                return;


            string nodeName = sNode.Name;
            string newName = string.Empty;
            string nodeKey = sNode is TreeNode ? treeConf.MainTree.GetFolderKey(sNode.Key) : sNode.Key;

            UltraTree tree = GetTreeFromGroup(ActivePaneName);
            UltraTreeNode sUINode = tree.GetNodeByKey(nodeKey);

            // Check if user has privileges or if it's a predefined node
            // CSBR-127968: added the ! at the beginning
            // CSBR-128309: still not the right logic
            //if (!(FormDbMgr.PrivilegeChecker.CanSavePublic && IsPublicPaneActive) || treeConf.IsRWord(sNode.Name))
            bool bCanRename = IsPublicPaneActive ? FormDbMgr.PrivilegeChecker.CanSavePublic : true;
            if (!bCanRename)
            {
                FormDbMgr.PrivilegeChecker.ShowRestrictionMessage(CBVConstants.CAN_SAVE_PUBLIC_FORM, false);
                UpdateNodeInPanel(sUINode, nodeName);   // Restore previous value
                return;
            }

            // Verify the source node is not a predefined node (like root nodes)
            if (!treeConf.IsRWord(nodeName))
            {
                // Prompt for name
                String promptName = nodeName;
                if (promptName.EndsWith("*"))
                    promptName = promptName.Substring(0, promptName.Length - 1);
                newName = PromptForNewName(sUINode.Parent, nodeKey, promptName);
                if (!String.IsNullOrEmpty(newName))     // CSBR-140172
                    RenameTreeNode(treeConf, sNode, nodeName, newName, sUINode);

            }// Verify the source node is not a predefined node (like root nodes)
            else
            {
                String sMsg = String.Format("{0} is a reserved word.  Choose a different name.", newName);
                MessageBox.Show(sMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateNodeInPanel(sUINode, nodeName);
            }
        }
        //---------------------------------------------------------------------
        private void RenameFormNode(string oldName, string newName, int id)
        {
            bool bChosenFormIsCurrent = oldName.Equals(this.FormName);
            ActiveDBBank.Rename(newName, id);
            if (bChosenFormIsCurrent)
                this.FormName = newName;
            // If it's the current form, rewrite the titles
            if (id == m_formID)
                SetTitles(ActivePaneName, String.Concat(ActivePaneName, " - ", FormName, " (opened)"), FormName + (Modified ? "*" : string.Empty));
            Application.DoEvents();
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Restore old name after edition
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        private void RestoreNodeName(string oldName, string newName)
        {
            UltraTreeNode node = null;
            if (ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME))
            {
                node = GetTreeNode(m_lastMouseDownPoint);
            }
            else if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
            {
                node = GetTreeNode(m_lastMouseDownPoint);
            }
            //Coverity Bug Fix CID :12966 
            if (node != null)
                node.Text = newName;
        }
        //---------------------------------------------------------------------
        private void DeleteNodeMenuItem_Click(object sender, EventArgs e)
        {
            string nodeKey = GetTreeNodeKey();
            ITreeNode sNode = null;
            TreeConfig treeConf = null;
            string nodeName = string.Empty;
            string extraMsg = string.Empty;

            if (!string.IsNullOrEmpty(nodeKey))
            {
                if (ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                {
                    treeConf = m_FTreeConf;
                    sNode = m_FTreeConf.MainTree.GetNodeFromListByKey(nodeKey);
                }
                else if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
                {
                    treeConf = m_QTreeConf;
                    sNode = m_QTreeConf.MainTree.GetNodeFromListByKey(nodeKey);
                }
                //Coverity Bug Fix CID 12956 
                nodeName = (sNode != null) ? sNode.Name : string.Empty;

                // User's not allowed to delete predefined nodes (like Public and User nodes)

                if (treeConf != null && !treeConf.IsRWord(nodeName))
                {
                    bool bChosenFormIsCurrent = false;
                    Query q = null;
                    if (sNode != null && sNode is TreeLeaf && ((TreeLeaf)sNode).Id > -1 &&
                        (ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME)
                        && ((TreeLeaf)sNode).Id == m_formID))
                    {
                        bChosenFormIsCurrent = true;
                    }
                    else if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
                    {
                        q = GetSelectedQueryFromTree(false);
                        if (q != null)
                        {
                            if (q.IsSaved)
                                extraMsg = String.Concat("\r\nNote: this query is attached to saved hitlist ",
                                    CBVUtil.IntToStr(q.HitListID), ".  The hitlist will be deleted.");
                        }
                    }
                    // check privilege if form is public
                    if (sNode != null && (IsPublicPaneActive && !FormDbMgr.PrivilegeChecker.CanSavePublic
                        && ((sNode is TreeNode && ((TreeNode)sNode).Nodes.Count > 0) || sNode is TreeLeaf)))
                    {
                        FormDbMgr.PrivilegeChecker.ShowRestrictionMessage(CBVConstants.CAN_SAVE_PUBLIC_FORM, true);
                        return;
                    }
                    // ask for confirmation
                    StringBuilder sb = new StringBuilder("Are you sure you want to delete ");
                    sb.AppendFormat("{0} ", treeConf.IsFolder(nodeName) ? "Folder" : (
                        ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME) ? "Query" : "Form"));
                    sb.AppendFormat(" \"{0}\" ?", nodeName);
                    if (!string.IsNullOrEmpty(extraMsg))
                        sb.Append(extraMsg);
                    if (MessageBox.Show(sb.ToString(), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        return;
                    Application.DoEvents();

                    // proceed with the delete
                    try
                    {
                        // Remove node from display pane, structure and db
                        this.RemoveTreeNode(nodeKey, IsPublicPaneActive, treeConf);
                        if (bChosenFormIsCurrent) // if current form, close
                        {
                            this.CreateEmptyForm();
                        }
                    }
                    catch (ObjectBankException ex)
                    {
                        CBVUtil.ReportError(ex, "Error deleting form");
                        return;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private void NewFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String sourceNodeKey = GetTreeNodeKey();
            TreeConfig treeConf = null;
            if (ActivePaneName.Equals(CBVConstants.PUBLIC_GROUPNAME) || ActivePaneName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                treeConf = m_FTreeConf;
            else if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
                treeConf = m_QTreeConf;

            if (!string.IsNullOrEmpty(sourceNodeKey))
            {
                // Add new folder just to folders not to forms or queries
                //Coverity Bug Fix CID :12961 
                if (treeConf != null && treeConf.IsFolder(sourceNodeKey))
                {
                    UltraTree tree = GetTreeFromGroup(ActivePaneName);
                    UltraTreeNode sourceNode = tree.GetNodeByKey(sourceNodeKey);
                    // Prompt for name
                    string newName = PromptForNewName(sourceNode, sourceNodeKey, CBVConstants.DEFAULT_FOLDER_NAME);
                    if (!string.IsNullOrEmpty(newName))
                    {
                        // Add node
                        TreeNode folder = treeConf.CreateFolder(newName);
                        UltraTreeNode newNode = treeConf.CreateUINode(folder, ActivePaneName);
                        treeConf.AddNode(sourceNode, newNode, folder, ActivePaneName);
                    }
                    if (ActivePaneName.Equals(CBVConstants.QUERIES_GROUPNAME))
                        Modified = true;
                }
            }
        }
        //---------------------------------------------------------------------
        private String PromptForNewName(UltraTreeNode sourceNode, string sourceNodeKey, string text)
        {
            String newName = string.Empty;
            newName = CBVUtil.PromptForString("New name:", text);
            newName = VerifyNewName(sourceNode, newName, text);
            return newName;
        }
        //---------------------------------------------------------------------
        private String VerifyNewName(UltraTreeNode sourceNode, String newName, string text)
        {
            if (!string.IsNullOrEmpty(newName))
            {
                // Verify that is not a reserved word
                if (!m_FTreeConf.IsRWord(newName))
                {
                    // Verify that there aren't siblings with the same name
                    if (!m_FTreeConf.ExistSiblingWithSameName(newName, sourceNode.Key, !m_FTreeConf.IsFolder(sourceNode.Key), sourceNode))
                    {
                        return newName;
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder("The name");
                        sb.AppendFormat(" \"{0}\" already exists. Specify a new one.", newName);
                        MessageBox.Show(sb.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        newName = CBVUtil.PromptForString("New name:", text);
                    }
                }
                else
                {
                    StringBuilder sb = new StringBuilder("You cannot use");
                    sb.AppendFormat(" \"{0}\". It's a reserved word. Specify another name.", newName);
                    MessageBox.Show(sb.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    newName = CBVUtil.PromptForString("New name:", text);
                }
            }
            return newName;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Changing global zoom mode from plot context menu
        /// </summary>
        private void xAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedPlot.SetZoomOnDragModes(true, false);
            UpdatePlots(true);
        }
        //---------------------------------------------------------------------
        private void yAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedPlot.SetZoomOnDragModes(false, true);
            UpdatePlots(true);
        }
        //---------------------------------------------------------------------
        private void bothToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedPlot.SetZoomOnDragModes(true, true);
            UpdatePlots(true);
        }
        //---------------------------------------------------------------------
        private void zoomToSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double xMin = 0.0, xMax = 0.0, yMin = 0.0, yMax = 0.0;
            if (SelectedPlot.GetSelectionLimits(ref xMin, ref xMax, ref yMin, ref yMax))
            {
                if (xMin < xMax && yMin < yMax)
                {
                    double xMargin = (xMax - xMin) / 15.0, yMargin = (yMax - yMin) / 10.0;
                    SelectedPlot.ZoomToRect(xMin - xMargin, xMax + xMargin, yMin - yMargin, yMax + yMargin);
                }
            }
        }
        //---------------------------------------------------------------------
        private void unzoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedPlot.Unzoom();
        }
        //---------------------------------------------------------------------
        private void plotContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            bool bHasSel = SelectedPlot != null &&
                SelectedPlot.SelectedRecords.Count > 0 && !SelectedPlot.IsSubformPlot;

            this.selectionToListToolStripMenuItem.Enabled = bHasSel;
            this.zoomToSelectionToolStripMenuItem.Enabled = bHasSel;
        }
        //---------------------------------------------------------------------
        private void plotSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ChartController.ModalPlotDialog(true);
        }
        //---------------------------------------------------------------------
        private void selectionToListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // plot point selection to new list/query
            if (SelectedPlot == null) return;
            List<int> recs = SelectedPlot.GetSelectedRecordPKs();
            if (recs.Count == 0)
                MessageBox.Show("Cannot convert selection to list -- no primary keys available");
            else
                PKListToSearch(recs, "Point selection");
        }
        //---------------------------------------------------------------------
        private void selectionToListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // grid row selection to new list/query
            List<int> pks = GetSelectedRecordPKs();
            if (pks.Count == 0)
                MessageBox.Show("Cannot convert selection to list -- no primary keys available");
            else
                PKListToSearch(pks, "Row selection");
        }
        //---------------------------------------------------------------------
        private bool HasSelectedRecords()
        {
            if (TabManager.CurrentTab.IsGridView())
            {
                ChemDataGrid cdg = TabManager.CurrentTab.Control as ChemDataGrid;
                //Coverity Bug Fix CID :12950 
                if (cdg != null)
                    return cdg.Selected.Rows.Count > 0;
            }
            return false;
        }
        //---------------------------------------------------------------------
        public List<int> GetSelectedRecordNos()
        {
            // new Oct 11: return absolute recnos, not relative
            // also: return single current row if not grid view (used by addin)
            List<int> recs = new List<int>();
            if (TabManager.CurrentTab.IsGridView())
            {
                ChemDataGrid cdg = TabManager.CurrentTab.Control as ChemDataGrid;
                //Coverity Bug Fix CID :12951 
                if (cdg != null)
                {
                    foreach (UltraGridRow row in cdg.Selected.Rows)
                    {
                        int index = cdg.Rows.IndexOf(row);
                        if (index >= 0)
                            recs.Add(index);
                    }
                }
            }
            else
            {
                recs.Add(this.Pager.CurrRow);
            }
            return recs;
        }
        //---------------------------------------------------------------------
        private List<int> GetSelectedRecordPKs()
        {
            // used only by grid view
            List<int> pks = new List<int>();
            if (TabManager.CurrentTab.IsGridView())
            {
                ChemDataGrid cdg = TabManager.CurrentTab.Control as ChemDataGrid;
                String sKey = FormDbMgr.PKFieldAlias(); // CSBR-151658: Use alias rather than name
                //Coverity Bug Fix : CID 12952 
                if (cdg != null && !String.IsNullOrEmpty(sKey))
                {
                    UltraGridColumn keyCol = cdg.DisplayLayout.Bands[0].Columns[sKey];
                    if (keyCol != null)
                    {
                        foreach (UltraGridRow row in cdg.Selected.Rows)
                        {
                            String sRowData = row.GetCellText(keyCol);
                            int pk = CBVUtil.StrToInt(sRowData);
                            pks.Add(pk);
                        }
                    }
                }
            }
            return pks;
        }
        //---------------------------------------------------------------------
        private void PKListToSearch(List<int> recs, String title)
        {
            // create a new query, with a search criterion "PK IN (list)"
            String delimList = CBVUtil.CreateDelimitedList(recs);
            String pkColName = this.FormDbMgr.PKFieldName();
            Query query = Query.CreateFromStrings(pkColName, delimList, this.FormDbMgr, this.QueryCollection, true, false);
            if (query != null)
            {
                query.Description = title;

                // ripped from DoSearch .. should be refactored
                query.Run();
                m_currQuery = query;
                this.DoMove(Pager.MoveType.kmFirst);
                query.Flag(Query.QueryFlag.kfDiscard, true);
                AddQueryToTree(query, true);
                m_tabManager.Bind(query, BindingSource);
                if (SearchCompleted != null)
                    SearchCompleted.Invoke(this, new EventArgs());
            }
        }
        //---------------------------------------------------------------------
        #endregion

        #region Tab event handlers
        /// <summary>
        /// Tab control management handlers. Determine which tab was clicked. It's called on right-click before menu comes up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainUltraTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is UltraTabControl && e.Button == MouseButtons.Right)
            {
                UltraTab ut = (sender as UltraTabControl).TabFromPoint(e.Location);
                m_rightClickedTabIndex = (ut == null) ? -1 : ut.Index;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Tab control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainUltraTabControl_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (m_tabManager != null)
            {
                int index = MainUltraTabControl.SelectedTab.Index;
                m_tabManager.SelectTab(index);
                SetEditButtonText();
                SetAcceptButton();
            }
        }
        //---------------------------------------------------------------------
        private void MainUltraTabControl_SelectedTabChanging(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangingEventArgs e)
        {
            if (IsEditingForm())
                e.Cancel = true;
            if (IsEditingGrid())
                e.Cancel = true;
        }
        //---------------------------------------------------------------------
        #endregion

        private void cancelUltraButton_Click(object sender, EventArgs e)
        {
            // for lack of a better thing to do, go back to tab zero
            if (m_tabManager != null && m_tabManager.Count > 0)
                m_tabManager.SelectDefaultTab();
        }
        //---------------------------------------------------------------------
        private void clearUltraButton_Click(object sender, EventArgs e)
        {
            if (m_tabManager != null)
                m_tabManager.ClearQueryTab();
        }
        //---------------------------------------------------------------------
        private void searchOverThisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Query qSel = GetSelectedQueryFromTree(false);
            if (qSel != null)
            {
                if (QueryCollection.QuerySearchOver == qSel.ID)
                    QueryCollection.QuerySearchOver = 0;    // toggle off
                else
                    QueryCollection.QuerySearchOver = qSel.ID;
            }
            UpdateQueryText(qSel);
        }
        //---------------------------------------------------------------------
        private void searchUltraButton_Click(object sender, EventArgs e)
        {
            // click Search: convert query info into search criteria, then search
            Query query = null;
            Query qDomain = null;
            try
            {
                Debug.Assert(m_tabManager.CurrentTab != null && m_tabManager.CurrentTab is QueryViewTab);
                query = CreateFromFormEx(this);
                if (query == null)
                {
                    // means form is blank or has no query info
                    // CSBR-127237 -- tell the user, and just stay in query mode
                    MessageBox.Show("Query is empty or has no components bound to database fields");
                    return;
                }
                query.TabName = m_tabManager.CurrentTab.Name;

                CBVStatMessage.Show("Searching " + query.Description);

                if (QueryCollection.QuerySearchOver != 0)
                {
                    qDomain = this.QueryCollection.FindByID(QueryCollection.QuerySearchOver);
                    if (qDomain != null)
                        query.RunOverCurrentList(qDomain);
                }
                else
                {
                    query.Run();
                }
                this.TabManager.ClearQueryOnNextOpen = true;
                m_currQuery = query;
                this.DoMove(Pager.MoveType.kmFirst);
                query.Flag(Query.QueryFlag.kfDiscard, true);  // mark with * until saved
                AddQueryToTree(query, true);
                m_tabManager.Bind(query, BindingSource);

                // switch back into form view
                m_tabManager.SelectDefaultTab();
                if (SearchCompleted != null)
                    SearchCompleted.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                CBVUtil.ReportError(ex);
            }
            finally
            {
                QueryCollection.QuerySearchOver = 0;    // turn off Search Over List after every search
                if (qDomain != null)
                    UpdateQueryText(qDomain);

                CBVStatMessage.ShowReadyMsg();
            }
        }
        //---------------------------------------------------------------------
        public void FireButtonEvent(String label, String arg, String menuName)
        {
            if (ActionButtonClicked == null)
                ActionButtonClicked += new ActionButtonClickedEventHandler(ChemBioVizForm_ActionButtonClicked);

            ActionButtonClicked.Invoke(this, new ActionButtonEventArgs(label, arg, menuName, this));
        }
        //---------------------------------------------------------------------
        void ChemBioVizForm_ActionButtonClicked(object sender, ActionButtonEventArgs e)
        {
            // no need to do anything -- this event is for calling programs
            Debug.WriteLine(String.Concat("BUTTON EVENT: ",
                e.buttonLabel, " arg=", e.buttonArg, " menu=", e.menuName));
        }
        //---------------------------------------------------------------------
        private void SetCardViewStyle(CardStyle style)
        {
            if (TabManager.CurrentTab.IsGridView())
            {
                ChemDataGrid cdg = this.TabManager.CurrentTab.Control as ChemDataGrid;
                //Coverity Bug Fix : CID 12970 
                if (cdg != null)
                {
                    cdg.CardViewStyle = style;
                    cdg.DisplayLayout.Bands[0].CardSettings.Style = style;
                }
            }
        }
        //---------------------------------------------------------------------
        private void SetCardViewLayout(ChemDataGrid.CardViewLayoutType layout)
        {
            if (TabManager.CurrentTab.IsGridView())
            {
                ChemDataGrid cdg = this.TabManager.CurrentTab.Control as ChemDataGrid;
                //Coverity Bug Fix : CID 12969 
                if (cdg != null)
                {
                    cdg.CardViewLayout = layout;
                    if (cdg.CardViewMode)
                        cdg.InitCardView();
                }
            }
        }
        //---------------------------------------------------------------------

        // CSBR #150169        
        private void OnCallRefresh(int oldhitlistid, int newhitlistid)
        {
            if (oldhitlistid != newhitlistid)
            {
                CBVAddin addin = this.m_addinMgr.FindByMenuTitle("Spotfire");
                if (addin != null && addin.Menu != null)
                {
                    if (addin.HandleMenuCommand("OnCallRefresh"))
                        return;
                }
            }
        }
        //-------------------------------------------------------------
        private void standardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCardViewStyle(CardStyle.StandardLabels);
        }
        private void mergedLabelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCardViewStyle(CardStyle.MergedLabels);
        }
        private void variableHeightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCardViewStyle(CardStyle.VariableHeight);
        }
        private void compressedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCardViewStyle(CardStyle.Compressed);
        }
        private void structTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCardViewLayout(ChemDataGrid.CardViewLayoutType.strTop);
        }
        private void structBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCardViewLayout(ChemDataGrid.CardViewLayoutType.strBottom);
        }
        private void structLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCardViewLayout(ChemDataGrid.CardViewLayoutType.strLeft);
        }
        private void structRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCardViewLayout(ChemDataGrid.CardViewLayoutType.strRight);
        }
        //---------------------------------------------------------------------
        #endregion
    }
    public class ActionButtonEventArgs : EventArgs
    {
        public String buttonLabel;
        public String buttonArg;
        public String menuName;
        public ActionButtonEventArgs(String label, String arg, String menu, ChemBioVizForm form)
        {
            buttonLabel = label;
            buttonArg = arg;
            menuName = menu;
        }
    }
    //---------------------------------------------------------------------
    public class RecordsetChangedEventArgs : EventArgs
    {
        public enum RecordsetChangeAction { Ask, Push, NoPush }

        private RecordsetChangeAction m_action;
        private String m_currRC;
        public RecordsetChangedEventArgs(String currRC)
        {
            m_action = RecordsetChangeAction.NoPush;
            m_currRC = currRC;
        }
        public RecordsetChangeAction ChangeAction
        {
            get { return m_action; }
            set { m_action = value; }
        }
        public String CurrentRC
        {
            get { return m_currRC; }
            set { m_currRC = value; }
        }
    }
    //---------------------------------------------------------------------
    public class RecordChangedEventArgs : EventArgs
    {
        public int cbvrecno;
        public RecordChangedEventArgs(int recno)
        {
            cbvrecno = recno;
        }
    }
    //---------------------------------------------------------------------
    public class SubRecordChangedEventArgs : RecordChangedEventArgs
    {
        public ChemDataGrid grid;
        public SubRecordChangedEventArgs(ChemDataGrid cdgrid, int recno)
            : base(recno)
        {
            grid = cdgrid;
        }
    }
    //---------------------------------------------------------------------
    public class FormOpenedEventArgs : EventArgs
    {
        public FormOpenedEventArgs()
        {
        }
    }
    //---------------------------------------------------------------------
    public class FormEditedEventArgs : EventArgs
    {
        public FormEditedEventArgs()
        {
        }
    }
    //---------------------------------------------------------------------
    public class CBVFormClosedEventArgs : EventArgs
    {
        public CBVFormClosedEventArgs()
        {
        }
    }
    //---------------------------------------------------------------------
    public class UnitsStringConverter : StringConverter
    {
        // moved here from cbvunits.cs to allow call to GetResourceXmlString
        public UnitsStringConverter()
        {
        }
        //---------------------------------------------------------------------
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        //---------------------------------------------------------------------
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true; // means no values are allowed except those we offer
        }
        //---------------------------------------------------------------------
        private bool m_bWarned = false;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            // updated to fix CSBR-132401
            String sXml = ChemBioVizForm.GetResourceXmlString();
            if (String.IsNullOrEmpty(sXml))
            {
                if (!m_bWarned)
                    MessageBox.Show("Unable to load units resources -- no units will be processed");
                m_bWarned = true;
                return null;
            }
            else
            {
                CBVUnitsManager unitsMgr = new CBVUnitsManager(sXml);
                List<String> valuesList = unitsMgr.GetComboList();
                StandardValuesCollection vals = new StandardValuesCollection(valuesList);
                return vals;
            }
        }
    }
    //---------------------------------------------------------------------
}
