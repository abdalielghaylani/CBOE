using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEHitListService;

using FormDBLib;
using CBVUtilities;

namespace ChemBioViz.NET
{
    /// <summary>
    ///   Command Line class: parses command-line params and exposes properties
    ///   For format, see ShowHelp below
    /// </summary>
    public class CommandLine
    {
        #region Variables
        private String[] m_args;
        private ChemBioVizForm.formType m_formtype;
        private String m_pathname;
        private String m_queryArg;
        private String m_searchArg;
        private String m_loginArg;
        private String m_serverName;
        private String m_authTicket;
        private bool m_wantsHelp;
        private bool m_noPanel;
        private bool m_doPrint;
        private String m_hitlistIDArg;
        private String m_sbhlArg;   // search by hitlist; undocumented, special format
        #endregion

        #region Properties
        public bool WantsHelp { get { return m_wantsHelp; } }
        public String Pathname { get { return m_pathname; } }
        public String QueryName { get { return m_queryArg; } }
        public String SearchQueryField { get { return CBVUtil.BeforeDelimiter(m_searchArg, ':'); } }
        public String SearchQuery { get { return CBVUtil.AfterDelimiter(m_searchArg, ':'); } }
        public String HitlistIDArg { get { return m_hitlistIDArg; } }
        public String SBHLArg { get { return m_sbhlArg; } }

        public String LoginName { get { return CBVUtil.BeforeDelimiter(m_loginArg, ',').ToUpper(); } }
        public String ServerName { get { return m_serverName; } }
        public String AuthTicket { get { return m_authTicket; } }
        public String LoginPassword { get { return CBVUtil.AfterDelimiter(m_loginArg, ','); } }
        public ChemBioVizForm.formType FormType { get { return m_formtype; } }
        public bool NoPanel { get { return m_noPanel; } }
        public bool DoPrint { get { return m_doPrint; } }
        #endregion

        #region Constructors
        public CommandLine(String[] args)
        {
            m_args = args;
        }
        #endregion

        #region Methods
        public bool HasPathname() { return !String.IsNullOrEmpty(m_pathname); }
        public bool HasQueryName() { return !String.IsNullOrEmpty(m_queryArg); }
        public bool HasSearchQuery() { return !String.IsNullOrEmpty(m_searchArg); }
        public bool HasLogin() { return !String.IsNullOrEmpty(m_loginArg); }
        public bool HasServer() { return !String.IsNullOrEmpty(m_serverName); }
        public bool HasTicket() { return !String.IsNullOrEmpty(m_authTicket); }
        public bool HasHitlistIDArg() { return !String.IsNullOrEmpty(m_hitlistIDArg); }
        public bool HasSBHLArg() { return !String.IsNullOrEmpty(m_sbhlArg); }
        //---------------------------------------------------------------------
        public static String[] SplitCommandLine(String sArgs)
        {
            // split string into two tokens, splitting at first space
            // taking single or double quotes into effect around first token
            // examples:
            // path\prog.exe arg1 arg2 => (1) path\prog.exe (2) arg1 arg2
            // "path\prog name.exe" arg1 arg2 => (1) path\prog name.exe (2) arg1 arg2
            List<String> list = new List<String>();

            int startAt = 0;
            if (CBVUtil.StartsWith(sArgs, "\"")) {
                startAt = sArgs.IndexOf('"', 1);
            } else if (CBVUtil.StartsWith(sArgs, "\'")) {
                 startAt = sArgs.IndexOf('\'', 1);
            }
            int firstBlank = sArgs.IndexOf(' ', startAt);
            if (firstBlank > 0) {
                list.Add(sArgs.Substring(0, firstBlank));
                list.Add(sArgs.Substring(firstBlank).Trim());
            } else {
                list.Add(sArgs);
            }
            return (String[])list.ToArray();
        }
        //---------------------------------------------------------------------
        private bool ParseSBHLArg(String s, ref int hitlistID, ref HitListType hlType, ref int srcFldID, ref String tgtField)
        {
            String[] args = s.Split(',');
            if (args.Length != 4)
                return false;

            hitlistID = CBVUtil.StrToInt(args[0]);
            hlType = CBVUtil.Eqstrs(args[1], "S") ? HitListType.SAVED : HitListType.TEMP;
            srcFldID = CBVUtil.StrToInt(args[2]);
            tgtField = args[3];
            return true;
        }
        //---------------------------------------------------------------------
        public Query GetStartupQuery(FormDbMgr formDbMgr, QueryCollection queries)
        {
            Query q = null;
            String sErr = String.Empty;
            if (HasQueryName())
            {
                // named query: run if found -- query is already in tree
                q = queries.Find(QueryName);
                sErr = String.Format("Startup query {0} not found", QueryName);
            }
            else if (HasSBHLArg())
            {
                // format (from DoJumpTo, cbvf.cs): "/HL={0},{1},{2},{3}", hitlistID, hlTypeStr, srcFldID, sTarget
                int hitlistID = 0, srcFldID = 0;
                HitListType hlType = HitListType.SAVED;
                String tgtField = String.Empty;
                sErr = String.Format("Invalid hitlist arg {0}", SBHLArg);
                if (ParseSBHLArg(SBHLArg, ref hitlistID, ref hlType, ref srcFldID, ref tgtField))
                {
                    if (srcFldID == 0) {
                        // source not specified: means hitlist will be reused in target; requires matching PK fields in source and target
                        q = Query.CreateFromHitlistID(hitlistID, hlType, formDbMgr, queries);
                    }
                    else {
                        // source field given: use Search By Hitlist to obtain hitlist in target
                        // if target not specified, use PK
                        if (String.IsNullOrEmpty(tgtField))
                            tgtField = formDbMgr.PKFieldName();
                        q = Query.CreateFromHitlistInfo(hitlistID, hlType, formDbMgr, queries, srcFldID, tgtField);
                    }
                    sErr = String.Format("Invalid startup hitlist {0}", hitlistID);
                    if (q != null)
                        queries.Add(q);
                }
            }
            else if (HasHitlistIDArg())
            {
                int hitlistID = CBVUtil.StrToInt(HitlistIDArg);
                q = Query.CreateFromHitlistID(hitlistID, HitListType.SAVED, formDbMgr, queries);//, 0);
                sErr = String.Format("Invalid startup hitlist {0}", hitlistID);
                if (q != null)
                    queries.Add(q);
            }
            else if (HasSearchQuery())
            {
                // string-based: create new query in tree
                q = Query.CreateFromStrings(SearchQueryField, SearchQuery, formDbMgr, queries, false, false);
                sErr = String.Format("Invalid startup search {0}: {1} ", SearchQueryField, SearchQuery);
                if (q != null)
                    queries.Add(q);
            }
            else if (queries.QueryOnOpen > 0)
            {
                // selected query: run -- query is already in tree
                q = queries.FindByID(queries.QueryOnOpen);
                sErr = String.Format("Startup query ID {0} not found ", CBVUtil.IntToStr(queries.QueryOnOpen));
            }
            else
            {
                return null;
            }
            if (q == null && !String.IsNullOrEmpty(sErr))
                MessageBox.Show(sErr, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return q;
        }
        //---------------------------------------------------------------------
        private bool ProcessArg(String arg)
        {
            String val = CBVUtil.AfterFirstDelimiter(arg, '=');
            val.Trim();
            if (CBVUtil.Eqstrs(arg, "/H") || CBVUtil.Eqstrs(arg, "/?"))
            {
                m_wantsHelp = true;
                return false;
            }
            else if (CBVUtil.StartsWith(arg, "/Q"))
            {
                if (String.IsNullOrEmpty(val)) return false;
                m_queryArg = val;
            }
            else if (CBVUtil.StartsWith(arg, "/SERV"))
            {
                if (String.IsNullOrEmpty(val)) return false;
                m_serverName = val;
            }
            else if (CBVUtil.StartsWith(arg, "/AUTH"))
            {
                if (String.IsNullOrEmpty(val)) return false;
                m_authTicket = val;
            }
            else if (CBVUtil.StartsWith(arg, "/HIT"))
            {
                if (String.IsNullOrEmpty(val)) return false;
                m_hitlistIDArg = val;
            }
            else if (CBVUtil.StartsWith(arg, "/HL"))
            {
                if (String.IsNullOrEmpty(val)) return false;
                m_sbhlArg = val;
            }
            else if (CBVUtil.StartsWith(arg, "/S"))
            {
                if (String.IsNullOrEmpty(val) || !val.Contains(":")) return false;
                m_searchArg = val;
            }
            else if (CBVUtil.StartsWith(arg, "/L")) //  Login data
            {
                if (String.IsNullOrEmpty(val) || !val.Contains(",")) return false;
                m_loginArg = val;
            }
            else if (CBVUtil.StartsWith(arg, "/PUB"))
            {
                m_formtype = ChemBioVizForm.formType.Public;
            }
            else if (CBVUtil.StartsWith(arg, "/PRIV") || CBVUtil.StartsWith(arg, "/USER"))
            {
                m_formtype = ChemBioVizForm.formType.Private;
            }
            else if (CBVUtil.StartsWith(arg, "/NOPANEL"))
            {
                m_noPanel = true;
            }
            else if (CBVUtil.StartsWith(arg, "/PRINT"))
            {
                m_doPrint = true;
            }
            else if (CBVUtil.StartsWith(arg, "/"))
            {
                return false;   // invalid qualifier
            }
            return true;
        }
        //---------------------------------------------------------------------
        public bool Parse()
        {
            // convert cmdline args into local variables
            // return false if caller is to display help
            m_formtype = ChemBioVizForm.formType.Unknown;
            m_pathname = m_queryArg = m_searchArg = m_loginArg = m_serverName = m_hitlistIDArg = m_sbhlArg = null;
            m_wantsHelp = false;
            m_noPanel = false;
            m_doPrint = false;

            if (m_args.Length > 0)
            {
                ProcessArg(m_args[0]);  // check for /HELP
                if (m_wantsHelp)
                    return false;

                // first arg is form name if not preceded by /
                // this arg is now optional
                int firstArg = 0;
                if (!CBVUtil.StartsWith(m_args[0], "/"))
                {
                    m_pathname = m_args[0];
                    m_formtype = ChemBioVizForm.formType.Public;
                    if (CBVUtil.EndsWith(m_pathname, ".xml"))
                        m_formtype = ChemBioVizForm.formType.Local;
                    firstArg = 1;
                }

                // handle slash-args after form name
                for (int i = firstArg; i < m_args.Length; ++i)
                {
                    if (!ProcessArg(m_args[i]))
                    {
                        Console.WriteLine("\n\nInvalid argument: " + m_args[i]);
                        m_wantsHelp = true;
                        break;
                    }
                }
            }
            return !m_wantsHelp;
        }
        //---------------------------------------------------------------------
        public void ShowHelp()
        {
            String sHelpMsg = @"

Command line format:

  chembioviz.net.exe [<form>] [/option=value ...]

where <form> = pathname to xml file or name of public or user form
               blank if no form is to be opened on startup
               if name contains spaces, enclose in double quotes
Options:

  /login=name,password        log in without dialog; /server required
  /auth=auth_ticket           log in via authentication ticket; /server reqd
  /server=name                server address; ignored if no /login or /auth
                              for 2-tier use server=2-Tier[/oraservice]
  /search=fieldname:query     perform text or numeric search on open
  /hitlist=id                 restore given saved hitlist number
  /query=queryname            run named query on open
  /public                     named form is in Public collection
  /user or /private           named form is in User collection
  /nopanel                    hide explorer panel on startup
  /print                      print hitlist
  /help or /?                 display this text

Examples:

  chembioviz.net.exe C:\myform.xml /login=me,mypass /server=abc.camsoft.com /query=Q2
  chembioviz.net.exe CS_Demo /public /search=molweight:>200
  chembioviz.net.exe ""CS Demo Props"" /login=me,mypass /server=abc.camsoft.com
  chembioviz.net.exe /server=2-tier/myoracleservice /login=me,mypass 

press Enter to continue...
";
            Console.WriteLine(sHelpMsg);
        }
        //---------------------------------------------------------------------
        #endregion

    }
}
