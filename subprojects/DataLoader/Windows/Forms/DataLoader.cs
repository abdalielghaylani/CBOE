/*******************************************************************************
 * 
 * UI
 *  InputOutputMapper Preview...
 *  Summary screen
 *  OutputFileLabelAndType
 * 
 * InputObject:
 *  InputObjectXl (redo using PIA)
 * 
 * OutputObject:
 *  OutputSdf
 *  OutputXml
 *  OutputXl
 *  OutputObjectRegAdd
 *  OutputObjectRegBatch
 *  OutputObjectInvAdd (phase 2)
 *  OutputObjectInvContainer (phase 2)
 *  OutputObjectInvPlate (phase 2)
 * 
 * Technical
 *  try/catch throw
 *  nDoc
 * 
 * Bug: COETextReader::ReadLine does not quoted handle line breaks ?
 * Bug: COEXmlTextWriter::Pretty does not handle <!-- --> comments correctly
 * 
*******************************************************************************/
/*******************************************************************************
 * 
 * "Wizard" steps
 * 
 * Login
 * OutputTypeChooser
 * OutputConfiguration (optional)
 * InputFileChooser [Browse...]
 * InputConfiguration (optional)
 * InputTableChooser (optional)
 * InputFileLabelAndType [Save... Load...]
 * InputOutputMapper [Save... Load... Preview...]
 * OutputFileChooser [Browse...]
 * JobConfiguration
 * JobSummary
 *
*******************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.DataLoader.Data;
using CambridgeSoft.COE.DataLoader.Data.InputObjects;
using CambridgeSoft.COE.DataLoader.Data.OutputObjects;
using CambridgeSoft.COE.DataLoader.Windows.Common;
using CambridgeSoft.COE.DataLoader.Windows.Controls;
using System.Net.NetworkInformation;

namespace CambridgeSoft.COE.DataLoader.Windows.Forms
{
      /// <summary>
    /// This is the main Form of the application
    /// </summary>
    public partial class DataLoader : Form
    {
        private const string Task_load = "Load a previously saved job";
        private const string Task_view = "View previous job summary";
        #region data
        // Job
        Job _Job;
        // Potential Input / Output Objects
        private List<InputObject> _InputObjectList;
        private List<OutputObject> _OutputObjectList;
        private string _strInputObjectFilters;
        private Dictionary<string, string> _OutputDictionary;
        private List<string> _TaskList; // "internal" tasks
        private Dictionary<string, InputObject> _dictFilter = new Dictionary<string, InputObject>();
        // UI controls
        private readonly PictureBox _Logo;
        private readonly PictureBox _LogoMid;
        private readonly PictureBox _LogoRht;
        private readonly JobSpec _JobSpec;

        private Login _Login;
        private OutputTypeChooser _OutputTypeChooser;
        private OutputConfiguration _OutputConfiguration;
        private InputFileChooser _InputFileChooser;
        private InputConfiguration _InputConfiguration;
        private InputTableChooser _InputTableChooser;
        private InputFileLabelAndType _InputFileLabelAndType;
        private InputOutputMapper _InputOutputMapper;
        private OutputFileChooser _OutputFileChooser;
        private JobConfiguration _JobConfiguration;
        private JobSummary _JobSummary;
        private Control _CurrentControl; // Alias
        private Uri _HelpUri;

        // status bar
        private readonly System.Windows.Forms.StatusStrip _StatusStrip;
        private readonly System.Windows.Forms.ToolStripStatusLabel _StatusLabel;

        private readonly COEProgressHelper _Ph = new COEProgressHelper();
        #endregion

        #region temporary data
        private static string _strApplicationPath;
        /// <summary>
        /// Temporary ApplicationPath for loading icons etc.
        /// </summary>
        public static string ApplicationPath
        {
            get
            {
                return _strApplicationPath;
            }
        } // ApplicationPath
        #endregion

        // constructor
        /// <summary>
        /// <c>DataLoader</c> is the constructor for the main form.
        /// </summary>
        public DataLoader()
        {

            _HelpUri = GetHelpContentUri();

            #region temporary code

            _strApplicationPath = Path.GetDirectoryName(Application.ExecutablePath);
            if (_strApplicationPath.EndsWith("\\Debug") || _strApplicationPath.EndsWith("\\Release"))
            {
                _strApplicationPath = _strApplicationPath.Substring(0, _strApplicationPath.LastIndexOf('\\'));
            }
            if (_strApplicationPath.EndsWith("\\bin"))
            {
                _strApplicationPath = _strApplicationPath.Substring(0, _strApplicationPath.LastIndexOf('\\'));
            }
            #endregion
            _Job = new Job();
            _Job.Ph = _Ph;
            UIBase.Ph = _Ph;
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            BackColor = UIBase.White; //JHS 10/29/2008
            Size = new Size(800, 600);
            {
                SuspendLayout();
                {
                    MainMenu oMainMenu = new MainMenu();
                    MenuItem oMenuItem = new MenuItem("&Help");
                    {
                        MenuItem oMenuSubItem = new MenuItem("&Contents", new EventHandler(HelpContents_Click));
                        oMenuItem.MenuItems.Add(oMenuSubItem);
                    }
                    {
                        MenuItem oMenuSubItem = new MenuItem("-");
                        oMenuItem.MenuItems.Add(oMenuSubItem);
                    }
                    {
                        MenuItem oMenuSubItem = new MenuItem("&About Data Loader", new EventHandler(HelpAbout_Click));
                        oMenuItem.MenuItems.Add(oMenuSubItem);
                    }
                    oMainMenu.MenuItems.Add(oMenuItem);
                    Menu = oMainMenu;
                }
                _Logo = UIBase.GetPictureBox(UIBase.PictureBoxType.Logo);
                _Logo.BackColor = Color.LightGray;
                _Logo.Top = 0;
                _Logo.Left = 0;
                _Logo.Width = _Logo.Image.Width;
                _Logo.Height = _Logo.Image.Height;
                Controls.Add(_Logo);
#if CENTERED
                {
                    string strPath = Application.ExecutablePath;
                    if (File.Exists(Path.GetDirectoryName(strPath) + "\\" + "ClientLogo.gif"))
                    {
                        _LogoMid = UIBase.GetPictureBox();
                        _LogoMid.BackColor = Color.LightGray;
                        _LogoMid.Image = Image.FromFile(Path.GetDirectoryName(strPath) + "\\" + "ClientLogo.gif");
                        _LogoMid.Padding = new Padding(5);
                        {
                            int top = 0;
                            int bottom = 0;
                            top = (_Logo.Height > _LogoMid.Image.Height) ? (_Logo.Height - _LogoMid.Image.Height) / 2 : 0;
                            bottom = (top > 0) ? (_Logo.Height - _LogoMid.Image.Height - top) : 0;
                            _LogoMid.Padding = new Padding(0, top, 0, Bottom);
                        }
                        _LogoMid.Top = 0;
                        _LogoMid.Height = _Logo.Height - _LogoMid.Top;
                        _LogoMid.Left = _Logo.Left + _Logo.Width;
                        _LogoMid.Width = _LogoMid.Image.Width;
                        Controls.Add(_LogoMid);
                        _LogoRht = UIBase.GetPictureBox();
                        _LogoRht.BackColor = Color.LightGray;
                        _LogoRht.Top = 0;
                        _LogoRht.Width = 1;
                        _LogoRht.Height = _Logo.Image.Height;
                        Controls.Add(_LogoRht);
                    }
                    else
                    {
                        _LogoRht = UIBase.GetPictureBox(UIBase.PictureBoxType.LogoRht);
                        _LogoRht.BackColor = Color.LightGray;
                        _LogoRht.Top = 0;
                        _LogoRht.Left = ClientSize.Width - _LogoRht.Image.Width;
                        _LogoRht.Width = _LogoRht.Image.Width;
                        _LogoRht.Height = _LogoRht.Image.Height;
                        Controls.Add(_LogoRht);
                        _LogoMid = UIBase.GetPictureBox(UIBase.PictureBoxType.LogoMid);
                        _LogoMid.BackColor = Color.LightGray;
                        _LogoMid.SizeMode = PictureBoxSizeMode.StretchImage;
                        _LogoMid.Top = 0;
                        _LogoMid.Left = _Logo.Left + _Logo.Width;
                        _LogoMid.Width = (ClientSize.Width - 1) - _LogoMid.Left;
                        _LogoMid.Height = _LogoMid.Image.Height;
                        Controls.Add(_LogoMid);
                    }
                }
#endif
                {
                    string strPath = Application.ExecutablePath;
                    if (File.Exists(Path.GetDirectoryName(strPath) + "\\" + "ClientLogo.gif"))
                    {
                        _LogoRht = UIBase.GetPictureBox();
                        _LogoRht.BackColor = Color.LightGray;
                        _LogoRht.Image = Image.FromFile(Path.GetDirectoryName(strPath) + "\\" + "ClientLogo.gif");
                        _LogoRht.Padding = new Padding(5);
                        {
                            int top = 0;
                            int bottom = 0;
                            top = (_Logo.Height > _LogoRht.Image.Height) ? (_Logo.Height - _LogoRht.Image.Height) / 2 : 0;
                            bottom = (top > 0) ? (_Logo.Height - _LogoRht.Image.Height - top) : 0;
                            _LogoRht.Padding = new Padding(0, top, 0, Bottom);
                        }
                        _LogoRht.Top = 0;
                        _LogoRht.Height = _Logo.Height - _LogoRht.Top;
                        _LogoRht.Left = ClientSize.Width - _LogoRht.Image.Width;
                        _LogoRht.Width = _LogoRht.Image.Width;
                        Controls.Add(_LogoRht);
                    }
                    else
                    {
                        _LogoRht = UIBase.GetPictureBox(UIBase.PictureBoxType.LogoRht);
                        _LogoRht.BackColor = Color.LightGray;
                        _LogoRht.Top = 0;
                        _LogoRht.Left = ClientSize.Width - _LogoRht.Image.Width;
                        _LogoRht.Width = _LogoRht.Image.Width;
                        _LogoRht.Height = _LogoRht.Image.Height;
                        Controls.Add(_LogoRht);
                    }
                    _LogoMid = UIBase.GetPictureBox(UIBase.PictureBoxType.LogoMid);
                    _LogoMid.BackColor = Color.LightGray;
                    _LogoMid.SizeMode = PictureBoxSizeMode.StretchImage;
                    _LogoMid.Top = 0;
                    _LogoMid.Left = _Logo.Left + _Logo.Width;
                    _LogoMid.Width = (ClientSize.Width - 1) - _LogoMid.Left;
                    _LogoMid.Height = _LogoMid.Image.Height;
                    Controls.Add(_LogoMid);
                }
                _JobSpec = new JobSpec();
                _JobSpec.Job = _Job;
                _JobSpec.TabStop = false;
                _JobSpec.Visible = false;
                Controls.Add(_JobSpec);
                _StatusStrip = UIBase.GetStatusStrip();
                _StatusStrip.Name = "ssProgressHelper";
                _StatusLabel = UIBase.GetToolStripStatusLabel();
                _StatusLabel.Name = "tsslProgressHelper";
                _StatusLabel.Alignment = ToolStripItemAlignment.Left;
                _StatusLabel.AutoSize = false;
                _StatusLabel.TextAlign = ContentAlignment.MiddleLeft;
                _StatusStrip.Items.Add(_StatusLabel);
                {
                    COEWait oWaitForm = new COEWait();
                    _StatusStrip.Height = oWaitForm.Height;
                }
                Controls.Add(_StatusStrip);
                //
                _Login = new Login();
                _Login.Accept += new EventHandler(Login_Ok);
                _Login.Cancel += new EventHandler(Login_Cancel);
                //
                _OutputTypeChooser = new OutputTypeChooser();
                _OutputTypeChooser.Accept += new EventHandler(OutputTypeChooser_Ok);
                _OutputTypeChooser.Cancel += new EventHandler(OutputTypeChooser_Cancel);
                //
                _OutputConfiguration = new OutputConfiguration();
                _OutputConfiguration.Accept += new EventHandler(OutputConfiguration_Ok);
                _OutputConfiguration.Cancel += new EventHandler(OutputConfiguration_Cancel);
                //
                _InputFileChooser = new InputFileChooser();
                _InputFileChooser.Accept += new EventHandler(InputFileChooser_Ok);
                _InputFileChooser.Cancel += new EventHandler(InputFileChooser_Cancel);
                //
                _InputConfiguration = new InputConfiguration();
                _InputConfiguration.Accept += new EventHandler(InputConfiguration_Ok);
                _InputConfiguration.Cancel += new EventHandler(InputConfiguration_Cancel);
                //
                _InputTableChooser = new InputTableChooser();
                _InputTableChooser.Accept += new EventHandler(InputTableChooser_Ok);
                _InputTableChooser.Cancel += new EventHandler(InputTableChooser_Cancel);
                //
                _InputFileLabelAndType = new InputFileLabelAndType();
                _InputFileLabelAndType.Accept += new EventHandler(InputFileLabelAndType_Ok);
                _InputFileLabelAndType.Cancel += new EventHandler(InputFileLabelAndType_Cancel);
                _InputFileLabelAndType.SortChanged += new InputFileLabelAndType.SortChangedEvent(InputFileLabelAndType_SortChanged);
                //
                _InputOutputMapper = new InputOutputMapper();
                _InputOutputMapper.Accept += new EventHandler(InputOutputMapper_Ok);
                _InputOutputMapper.Cancel += new EventHandler(InputOutputMapper_Cancel);
                //
                _OutputFileChooser = new OutputFileChooser();
                _OutputFileChooser.Accept += new EventHandler(OutputFileChooser_Ok);
                _OutputFileChooser.Cancel += new EventHandler(OutputFileChooser_Cancel);
                //
                _JobConfiguration = new JobConfiguration();
                _JobConfiguration.Accept += new EventHandler(JobConfiguration_Ok);
                _JobConfiguration.Cancel += new EventHandler(JobConfiguration_Cancel);
                //
                _JobSummary = new JobSummary();
                _JobSummary.Accept += new EventHandler(JobSummary_Ok);
                _JobSummary.Cancel += new EventHandler(JobSummary_Cancel);
                //
                ResumeLayout(false);
                PerformLayout();
            }
            _Ph.Parent = this;
            _Ph.StatusStrip = _StatusStrip;
            return;
        } // DataLoader()
        // methods

        // Uses the information from CslaDataPortalUrl  in app settings 
        // to build the Uri for the help content 
        private Uri GetHelpContentUri()
        {
            Uri HelpUri = null;
            string CslaDataPortalUrl = ConfigurationManager.AppSettings["CslaDataPortalUrl"];

            if (CslaDataPortalUrl != null)
            {
                Uri CslaUri = new Uri(CslaDataPortalUrl);
                string helpPath = @"CBOEHelp/CBOEContextHelp/DataLoader Webhelp/Default.htm";
                HelpUri = new Uri(string.Format(@"{0}://{1}:{2}/{3}", CslaUri.Scheme, CslaUri.Host, CslaUri.Port, helpPath));           
            }
            return HelpUri;
        }



        // event handlers
        private void DataLoader_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            int nTop;
            int nWidth;
            int nHeight;
            {
                Rectangle rectClient = ClientRectangle;
                nWidth = rectClient.Right;
                nHeight = rectClient.Bottom;
            }
            nTop = 0;
            {
                if (_LogoRht != null)
                {
                    _LogoRht.Left = ClientSize.Width - _LogoRht.Width;
                    _LogoMid.Width = _LogoRht.Left - _LogoMid.Left;
                }
                nTop += _Logo.Height;
                nHeight -= _Logo.Height;
            }
            if (_JobSpec.Visible)
            {
                {
                    int nHeightJobSpec = nHeight / 2;   // More if ctlCurrent == JobSummary
                    _JobSpec.MaximumSize = new Size(nWidth, nHeightJobSpec - ((_StatusStrip != null) ? _StatusStrip.Height : 0));
                    if (_JobSpec.Width < nWidth) _JobSpec.Width = nWidth;
                    if (_JobSpec.Height < _JobSpec.PreferredHeight) _JobSpec.Height = nHeightJobSpec - ((_StatusStrip != null) ? _StatusStrip.Height : 0);
                }
                _JobSpec.Top = nTop;
                nTop += _JobSpec.Height;
                nHeight -= _JobSpec.Height;
            }
            if (_StatusStrip != null)
            {
                _StatusStrip.Width = nWidth;
                _StatusLabel.Width = _StatusStrip.Width - (_StatusStrip.Height - (1 + 2 + 2));
                nHeight -= _StatusStrip.Height;
            }
            if (_CurrentControl != null)
            {
                _CurrentControl.Size = _CurrentControl.MaximumSize = new Size(nWidth, nHeight);
                _CurrentControl.Left = (nWidth - _CurrentControl.Width) / 2;
                _CurrentControl.Top = (nHeight - _CurrentControl.Height) / 2 + ((_JobSpec.Visible) ? _JobSpec.Height : 0) + _Logo.Height;
            }
            return;
        } // DataLoader_Layout()
        private void DataLoader_Load(object sender, EventArgs e)
        {
            try
            {
                Icon = Properties.Resources.DL;
                //Icon = new Icon(UIBase.GetIcon("DL"));
                Text = Application.ProductName;
                {
                    string strVersion = string.Empty;
                    {
                        string[] strVersionParts = Application.ProductVersion.Split('.');
                        if (strVersionParts.Length > 0)
                        {
                            strVersion += " v" + strVersionParts[0];
                            if (strVersionParts.Length > 1)
                            {
                                strVersion += "." + strVersionParts[1];
                                if ((strVersionParts.Length > 2) && (strVersionParts[2] != "0"))
                                {
                                    strVersion += "." + strVersionParts[2];
                                }
                            }
                        }
                    }
                    Text += strVersion;
                }
                Layout += new LayoutEventHandler(DataLoader_Layout);
                Update();
                Activate();
                TestProgress();

                Accept(null, _Login, delegate()
                {
                    return false;
                });
            }
            catch (Exception ex)
            {
                string strCaption = "Unexpected error";
                string strText = ex.ToString();
                MessageBox.Show(strText, strCaption, MessageBoxButtons.OK);
            }
            return;
        } // DataLoader_Load()
       
        private void HelpAbout_Click(object sender, EventArgs e)
        {
            AboutBox frmAbout = new AboutBox();
            frmAbout.ShowDialog();
        } // HelpAbout_Click()
        private void HelpContents_Click(object sender, EventArgs e)
        {
            HelpBrowser hb = new HelpBrowser();
            WebBrowser wb = new WebBrowser();
            try
            {
                wb.Dock = DockStyle.Fill;
                string LocalIp = ConfigurationManager.AppSettings["DefaultHostName"];
                string Helpfilepath = ConfigurationManager.AppSettings["Help"];
                string Url = ConfigurationManager.AppSettings["Url"];
                string path = Url + LocalIp + Helpfilepath;
                Ping myPing = new Ping();
                PingReply reply = myPing.Send(LocalIp, 1000);
                if (reply != null && reply.Status == IPStatus.Success)
                {
                    wb.Url = new Uri(path);
                }
                else
                {
                    MessageBox.Show("Please provide the fully qualified host name in app.config");
                }                
                hb.Controls.Add(wb);
                hb.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }            
        }
       
        
        /************************************************************************
         * Login
        *************************************************************************/
        private void Login_Cancel(object sender, EventArgs e)
        {
            // Transition Login to Exit
            {
                SuspendLayout();
                Controls.Remove(_Login); _Login = null;
                ResumeLayout(false);
                PerformLayout();
            }
            _StatusLabel.Text = string.Empty;
            Close();
            return;
        } // Login_Cancel()
        private void Login_Ok(object sender, EventArgs e)
        {
            // need logged in user to get these
            _InputObjectList = InputObject.ObjectList;
            _OutputObjectList = OutputObject.ObjectList;
            // Derive
            _strInputObjectFilters = string.Empty;
            _dictFilter.Clear();
            foreach (InputObject oInputObject in _InputObjectList)
            {
                if (oInputObject.Filter.Length > 0)
                {
                    { // We're assuming no parsing errors :-)
                        string[] strFilters = oInputObject.Filter.Split('|');
                        int cFilters = strFilters.Length / 2;
                        for (int nFilter = 0; nFilter < cFilters; nFilter++)
                        {
                            _dictFilter[strFilters[0 + nFilter * 2] + "|" + strFilters[1 + nFilter * 2]] = oInputObject;
                        }
                    }
                    if (_strInputObjectFilters.Length != 0)
                    {
                        _strInputObjectFilters += "|";
                    }
                    _strInputObjectFilters += oInputObject.Filter;
                }
            }
            _OutputDictionary = new Dictionary<string, string>();
            foreach (OutputObject outputObject in _OutputObjectList)
            {
                _OutputDictionary.Add(outputObject.ToString(), outputObject.OutputType);
            }
            // End of derive

            _TaskList = new List<string>();
            _TaskList.Add(Task_load);
            _TaskList.Add(Task_view);

            Accept(_Login, _OutputTypeChooser, delegate()
            {
                _Job.User = _Login.User;
                _JobSpec.Visible = true;

                _OutputTypeChooser.Setup(_OutputDictionary, _TaskList);
                return false;
            });
            return;
        } // Login_Ok()

        /************************************************************************
         * OutputTypeChooser
        *************************************************************************/
        private void OutputTypeChooser_Cancel(object sender, EventArgs e)
        {
            Cancel(_OutputTypeChooser, _Login, delegate()
            {
                _Job.User = null;
                _JobSpec.Visible = false;
                return;
            });
            return;
        } // OutputTypeChooser_Cancel)
        private void OutputTypeChooser_Ok(object sender, EventArgs e)
        {
            int nOutputType = _OutputTypeChooser.OutputType;
            if (nOutputType >= 0)
            {
                Accept(_OutputTypeChooser, _OutputConfiguration, delegate()
                {
                    _Job.Output = _OutputObjectList[nOutputType];
                    _Job.OutputType = _OutputObjectList[nOutputType].OutputType;

                    _OutputConfiguration.Settings = _Job.OutputConfiguration;
                    _OutputConfiguration.Visible = _OutputConfiguration.HasSettings;
                    return false;
                });
            }
            else
            {
                int nTask = _OutputTypeChooser.Task;
                switch (_TaskList[nTask])
                {
                    case Task_load:
                        {
                            PersistSettings oSettingsForm = new PersistSettings("JobConfiguration");
                            oSettingsForm.Direction = PersistSettings.DirectionType.Load;
                            oSettingsForm.ShowDialog(this);
                            if (oSettingsForm.DialogButton == DialogResult.OK)
                            {
                                string xmlJob = oSettingsForm.Settings;
                                if (_Job.Load(xmlJob))
                                {
                                    MessageBox.Show("The job could not be loaded.\n" + String.Join("\n", _Job.MessageList.ToMessageArray()), Task_load, MessageBoxButtons.OK);
                                    break;
                                }
                                //
                                Control currentControl = _CurrentControl; _CurrentControl = null;
                                currentControl.Visible = false;
                                //
                                //
                                // OutputTypeChooser <--
                                _OutputTypeChooser.Setup(_OutputDictionary, _TaskList);
                                if (_OutputDictionary.ContainsKey(_Job.OutputToString) == false)
                                {
                                    MessageBox.Show("Could not locate the output object.\n", Task_load, MessageBoxButtons.OK);
                                    currentControl.Visible = true;
                                    _CurrentControl = currentControl;
                                    break;
                                }
                                _OutputTypeChooser.Setup(_OutputDictionary, _TaskList);
                                _OutputTypeChooser.Select(_OutputDictionary[_Job.OutputToString]);
                                // OutputConfiguration <--
                                _OutputConfiguration.Settings = _Job.OutputConfiguration;
                                _OutputConfiguration.Visible = _OutputConfiguration.HasSettings;
                                // InputFileChooser <--
                                _InputFileChooser.Filter = _strInputObjectFilters;
                                _InputFileChooser.InputFilter = _Job.InputFilter;   // Empty because not persisted
                                // InputConfiguration <--
                                _InputConfiguration.Settings = _Job.InputConfiguration;
                                _InputConfiguration.Visible = _InputConfiguration.HasSettings;
                                // InputTableChooser <--
                                _Job.OpenInputDb(); // Could be time consuming. Need a quick open flag.
                                _InputTableChooser.TableList = _Job.InputTableList;
                                _InputTableChooser.Visible = (_Job.InputTableList.Count > 1);
                                _Job.OpenInputTable();
                                // InputFileLabelAndType <--
                                _InputFileLabelAndType.DataSetPreview = _Job.DataSetForPreview;
                                _InputFileLabelAndType.InputFieldSpec = _Job.InputFieldSpec;
                                _InputFileLabelAndType.InputFieldSort = _Job.InputFieldSort;
                                // InputOutputMapper <--
                                _InputOutputMapper.InputFieldSpec = _Job.InputFieldSpec;
                                _InputOutputMapper.OutputFieldSpec = _Job.OutputFieldSpec;
                                _InputOutputMapper.DataSetPreview = _Job.DataSetForPreview;
                                // OutputFileChooser <--
                                _OutputFileChooser.Filter = _Job.OutputFilter;
                                _OutputFileChooser.Visible = (_OutputFileChooser.Filter.Length > 0);
                                //
                                //
                                currentControl.Visible = true;
                                _CurrentControl = currentControl;
                                //
                                //
                                // // // Confirm Input.IsValid and Output.IsValid
                                Accept(_OutputTypeChooser, _JobConfiguration, delegate()
                                {
                                    // JobConfiguration <--
                                    _JobConfiguration.Job = _Job;
                                    _JobConfiguration.JobChanged = false;
                                    return false;
                                });
                            }
                            break;
                        } // case Task_load
                    case Task_view:
                        {
                            string strLogFilename = JobSummary.OpenLogFileDialog();
                            if (strLogFilename != string.Empty)
                            {
                                _Job.OutputType = _TaskList[nTask];
                                Accept(_OutputTypeChooser, _JobSummary, delegate()
                                {
                                    _JobSummary.LogMessagePath = strLogFilename;
                                    _JobSummary.Job = _Job;
                                    return false;
                                });
                            }
                            break;
                        } // case Task_view
                    default: break;
                } // switch (_TaskList[nTask])
            }
            return;
       } // OutputTypeChooser_Ok()

        /************************************************************************
        * OutputConfiguration
       *************************************************************************/
        private void OutputConfiguration_Cancel(object sender, EventArgs e)
       {
           Cancel(_OutputConfiguration, _OutputTypeChooser, delegate()
           {
               _Job.Output = null;
               _Job.OutputType = string.Empty;
               return;
           });
           return;
       } // OutputConfiguration_Cancel()
        private void OutputConfiguration_Ok(object sender, EventArgs e)
       {
           Accept(_OutputConfiguration, _InputFileChooser, delegate()
           {
               _Job.OutputConfiguration = _OutputConfiguration.Settings; // WJC need to validate ???

               _InputFileChooser.Filter = _strInputObjectFilters;
               _InputFileChooser.InputFilter = _Job.InputFilter;
               return false;
           });
           return;
       } // OutputConfiguration_Ok()

        /************************************************************************
        * InputFileChooser
       *************************************************************************/
        private void InputFileChooser_Cancel(object sender, EventArgs e)
        {
            Cancel(_InputFileChooser, _OutputConfiguration, delegate()
            {
                _Job.OutputConfiguration = string.Empty;
                return;
            });
            return;
        } // InputFileChooser_Cancel()
        private void InputFileChooser_Ok(object sender, EventArgs e)
        {
            Accept(_InputFileChooser, _InputConfiguration, delegate()
            {
                string strInputDb = _InputFileChooser.InputFile;
                // Make sure it at least exists
                if (File.Exists(strInputDb) == false)
                {
                    MessageBox.Show("File does not exist: '" + strInputDb + "'", "Error");
                    return true;  // ERROR
                }
                // Any Input objects?
                List<string>    listMatchingFilters = _InputFileChooser.MatchingFilters;
                if (listMatchingFilters.Count == 0)
                {
                    string strExt = Path.GetExtension(strInputDb).ToLower();
                    MessageBox.Show("Unknown filename extension: " + strExt, "Error");
                    return true;  // ERROR
                }
                // Too many Input objects?
                if (listMatchingFilters.Count > 1)
                {
                    MessageBox.Show("Use the Browse button and choose the desired entry under file of type", "Error");
                    return true;  // ERROR
                }

                // Map to InputObject
                _Job.Input = _dictFilter[listMatchingFilters[0]];
                // And remember which filter
                _InputFileChooser.InputFilter = listMatchingFilters[0];
                _Job.InputFilter = listMatchingFilters[0];
                // Save DB name and open DB
                _Job.InputDb = strInputDb;
                if (_Job.OpenInputDb())
                {
                    _Job.InputDb = string.Empty;
                    MessageBox.Show("Error opening: '" + strInputDb + "'\n" + String.Join("\n", _Job.InputMessageList.ToMessageArray()), "Error");
                    return true;  // ERROR
                }

                _Job.InputFieldSpec = string.Empty;
                _InputConfiguration.Settings = _Job.InputConfiguration;
                _InputConfiguration.Visible = _InputConfiguration.HasSettings;

                return false;
            });
            return;
        } // InputFileChooser_Ok()

        /************************************************************************
         * InputConfiguration
        *************************************************************************/
        private void InputConfiguration_Cancel(object sender, EventArgs e)
        {
            Cancel(_InputConfiguration, _InputFileChooser, delegate()
            {
                _Job.CloseInputDb();
                _Job.InputDb = string.Empty;
                return;
            });
            return;
        } // InputConfiguration_Cancel()
        private void InputConfiguration_Ok(object sender, EventArgs e)
        {
            Accept(_InputConfiguration, _InputTableChooser, delegate()
            {
                _Job.InputConfiguration = _InputConfiguration.Settings; // WJC need to validate ???

                _InputTableChooser.TableList = _Job.InputTableList;
                _InputTableChooser.Visible = (_Job.InputTableList.Count > 1);
                return false;
            });
            return;
        } // InputConfiguration_Ok()

        /************************************************************************
         * InputTableChooser
        *************************************************************************/
        private void InputTableChooser_Cancel(object sender, EventArgs e)
        {
            Cancel(_InputTableChooser, _InputConfiguration, delegate()
            {
                _Job.InputConfiguration = string.Empty;
                return;
            });
            return;
        } // InputTableChooser_Cancel()
        private void InputTableChooser_Ok(object sender, EventArgs e)
        {
            Accept(_InputTableChooser, _InputFileLabelAndType, delegate()
            {
                string strInputTable = _InputTableChooser.InputTable;

                _Job.InputTable = strInputTable;
                _StatusLabel.Text = "Opening Input Table";

                //re-set the field mapper form for, allowing re-use by subsequent uploads
                _InputOutputMapper.DataSetPreview = null;

                if (_Job.OpenInputTable())
                {
                    MessageBox.Show("Error opening table: " + _Job.InputTable + "\n" + String.Join("\n", _Job.InputMessageList.ToMessageArray()), "Error");
                    if (_InputTableChooser.Visible == false)
                    {
                        InputTableChooser_Cancel(null, null);
                    }
                    return true;  // ERROR
                }

                //refresh the upload preview form, allowing re-use by subsequent uploads
                _InputFileLabelAndType.InputFieldSpec = _Job.InputFieldSpec;
                _InputFileLabelAndType.DataSetPreview = _Job.DataSetForPreview;
                _InputFileLabelAndType.InputFieldSort = _Job.InputFieldSort;

                //JED: 2nd time around, _Job.Input.DataSetForJob contains only the auto-matched columns
                _InputOutputMapper.InputFieldSpec = string.Empty;

                return false;

            });

            return;
        } // InputTableChooser_Ok()

        /************************************************************************
         * InputFileLabelAndType
        *************************************************************************/
        private void InputFileLabelAndType_Cancel(object sender, EventArgs e)
        {
            Cancel(_InputFileLabelAndType, _InputTableChooser, delegate()
            {
                // Completely reset
                _InputFileLabelAndType = new InputFileLabelAndType();
                _InputFileLabelAndType.Accept += new EventHandler(InputFileLabelAndType_Ok);
                _InputFileLabelAndType.Cancel += new EventHandler(InputFileLabelAndType_Cancel);
                _InputFileLabelAndType.SortChanged += new InputFileLabelAndType.SortChangedEvent(InputFileLabelAndType_SortChanged);

                _Job.InputFieldSort = string.Empty;
                _Job.CloseInputTable();
                _Job.InputTable = string.Empty;
                return;
            });
            return;
        } // InputFileLabelAndType_Cancel()
        private void InputFileLabelAndType_Ok(object sender, EventArgs e)
        {
            Accept(_InputFileLabelAndType, _InputOutputMapper, delegate()
            {
                _Job.InputFieldSpec = _InputFileLabelAndType.InputFieldSpec;
                _Job.InputFieldSort = _InputFileLabelAndType.InputFieldSort;

                _InputOutputMapper.InputFieldSpec = _Job.InputFieldSpec;
                _InputOutputMapper.OutputFieldSpec = _Job.OutputFieldSpec;
                _InputOutputMapper.DataSetPreview = _Job.DataSetForPreview;
                return false;
            });
            return;
        } // InputFileLabelAndType_Ok()
        private void InputFileLabelAndType_SortChanged(object sender, SortChangedEventArgs e)
        {
            _Job.InputFieldSort = e.SortSpec;
            _InputFileLabelAndType.DataSetPreview = _Job.DataSetForPreview;
            return;
        } // InputFileLabelAndType_SortChanged()

        /************************************************************************
         * InputOutputMapper
        *************************************************************************/
        private void InputOutputMapper_Cancel(object sender, EventArgs e)
        {
            Cancel(_InputOutputMapper, _InputFileLabelAndType, delegate()
            {
                _Job.InputFieldSpec = string.Empty;   // Reset labelling but not sorting!
                return;
            });
            return;
        } // InputOutputMapper_Cancel()
        private void InputOutputMapper_Ok(object sender, EventArgs e)
        {
            Accept(_InputOutputMapper, _OutputFileChooser, delegate()
            {
                _Job.Mappings = _InputOutputMapper.Mappings;

                _OutputFileChooser.Filter = _Job.OutputFilter;
                _OutputFileChooser.Visible = (_OutputFileChooser.Filter.Length > 0);
                return false;
            });

            return;
        } // InputOutputMapper_OK()

        /************************************************************************
         * OutputFileChooser
        *************************************************************************/
        private void OutputFileChooser_Cancel(object sender, EventArgs e)
        {
            Cancel(_OutputFileChooser, _InputOutputMapper, delegate()
            {
                _Job.Mappings = string.Empty;
                return;
            });
            return;
        } // OutputFileChooser_Cancel()
        private void OutputFileChooser_Ok(object sender, EventArgs e)
        {
            Accept(_OutputFileChooser, _JobConfiguration, delegate()
            {
                string strOutputDb = _OutputFileChooser.OutputFile;
                if (File.Exists(strOutputDb))
                {
                    if ((File.GetAttributes(strOutputDb) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        MessageBox.Show("File '" + strOutputDb + "' is readonly.", "Output file");
                        return true;
                    }
                    DialogResult dr = MessageBox.Show("File '" + strOutputDb + "' exists. " + "Are you sure you want to overwrite it?", "Output file", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.No)
                    {
                        return true; // User does not want to overwrite
                    }
                }
                _Job.OutputDb = strOutputDb;
                _Job.JobStart = 1;
                _Job.JobCount = Int32.MaxValue;
                _JobConfiguration.Job = _Job;
                _JobConfiguration.JobChanged = false;
                return false;
            });
            return;
        } // OutputFileChooser_OK()

        /************************************************************************
         * JobConfiguration
        *************************************************************************/
        private void JobConfiguration_Cancel(object sender, EventArgs e)
        {
            Cancel(_JobConfiguration, _OutputFileChooser, delegate()
            {
                _Job.OutputDb = string.Empty;
                _Job.JobStart = 0;
                _Job.JobCount = 0;
                return;
            });
            return;
        } // JobConfiguration_Cancel()
        private void JobConfiguration_Ok(object sender, EventArgs e)
        {
            _Job.Execute(); // Could fail iff invalid ???
            Accept(_JobConfiguration, _JobSummary, delegate()
            {
                _JobSummary.Job = _Job;
                _JobSummary.LogMessagePath = _Job.LogMessagePath;
                return false;
            });
            return;
        } // JobConfiguration_OK()

        /************************************************************************
         * JobSummary
        *************************************************************************/
        private void JobSummary_Cancel(object sender, EventArgs e)
        {
            if (_Job.OutputType == Task_view)
            {
                Cancel(_JobSummary, _OutputTypeChooser, delegate()
                {
                    _Job.OutputType = string.Empty;
                    return;
                });
            }
            else
            {
                Cancel(_JobSummary, _JobConfiguration, delegate()
                {
                    return;
                });
            }
            return;
        } // JobSummary_Cancel()
        private void JobSummary_Ok(object sender, EventArgs e)
        {
            //Fix for CSBR 160709-Application hangs while uploading data after writing the files to text or XML kind 
            string strOutputType = _Job.OutputType.ToString();
            if (strOutputType == "Output to a text file" || strOutputType == "Output to an XML file")
            {
                _OutputFileChooser.Filter = string.Empty;
                _OutputFileChooser.OutputFile = string.Empty;
                _Job.OutputDb = string.Empty;
            }

            if (_Job.OutputType == Task_view)
            {
                Cancel(_JobSummary, _OutputTypeChooser, delegate()
                {
                    _Job.OutputType = string.Empty;
                    return;
                });
            }
            else
            {
                Accept(_JobSummary, _OutputTypeChooser, delegate()
                {
                    //clean up 'previous' job
                    _Job.CloseInputTable();
                    _Job.CloseInputDb();

                    //JED: Note to self!: The following line causes an exception (improper use of the property)
                    _InputFileLabelAndType.InputFieldSpec = string.Empty;
                    _InputFileLabelAndType.DataSetPreview = null;
                    //_InputFileLabelAndType.RefreshPreviewGrid();

                    //initialize 'next' job
                    _Job = new Job();
                    _Job.User = _Login.User;
                    _Job.Ph = _Ph;
                    UIBase.Ph = _Ph;

                    _JobSpec.Job = _Job;
                    _JobSpec.InputFieldSpec = string.Empty;
                    //JED: Reset output spec first or the GUI-builder will break
                    _InputOutputMapper.OutputFieldSpec = string.Empty;
                    _InputOutputMapper.InputFieldSpec = string.Empty;

                    _OutputTypeChooser.Setup(_OutputDictionary, _TaskList);
                    return false;
                });
            }
            return;
        } // JobSummary_OK()

        /************************************************************************
         * 
        *************************************************************************/
        /// <summary>
        /// <para>Code to execute when advancing to a UI step</para>
        /// <para>For use with <see cref="Accept"/></para>
        /// </summary>
        private delegate bool Accept_PrepareToActivate();

        /// <summary>
        /// Used to advance to the next UI step
        /// </summary>
        /// <param name="ControlBeingAccepted">Is the control for the step being accepted</param>
        /// <param name="ControlToActivate">Is the control for the step being activated</param>
        /// <param name="PrepareToActivate">Ecapsulates the transition code. See <see cref="Accept_PrepareToActivate"/></param>
        private void Accept(UIBase ControlBeingAccepted, UIBase ControlToActivate, Accept_PrepareToActivate PrepareToActivate)
        {
            if (_CurrentControl != ControlBeingAccepted)
            {
                throw new Exception("Developer error in UI step order");
            }
            if (PrepareToActivate() == false)
            {
                {
                    SuspendLayout();
                    if (_CurrentControl != null) Controls.Remove(_CurrentControl);
                    Controls.Add(ControlToActivate);
                    _CurrentControl = ControlToActivate;
                    _CurrentControl.BackColor = BackColor;
                    ResumeLayout(false);
                    PerformLayout();
                }
                if (ControlToActivate.Visible)
                {
                    Visible = true;
                    ControlToActivate.Select();
                    ControlToActivate.Focus();
                    AcceptButton = ControlToActivate.AcceptButton;
                    CancelButton = ControlToActivate.CancelButton;
                    _StatusLabel.Text = ControlToActivate.StatusText;
                   
                }
                else
                {
                    ControlToActivate.OnAccept();
                }
                WindowState = FormWindowState.Normal;
            }
            return;
        } // Accept()

        /// <summary>
        /// <para>Code to execute when cancelling a UI step</para>
        /// <para>For use with <see cref="Cancel"/></para>
        /// </summary>
        private delegate void Cancel_ThingsToDo();

        /// <summary>
        /// Used to cancel a UI step
        /// </summary>
        /// <param name="ControlBeingCancelled">Is the control for the step being cancelled</param>
        /// <param name="ControlToActivate">Is the control for the step being activated</param>
        /// <param name="ThingsToUndo">Ecapsulates the undo code. See <see cref="Cancel_ThingsToDo"/></param>
        /// <example>
        /// Example of Cancel usage:
        /// <code>
        /// Cancel(delegate()
        /// Cancel(_OutputConfiguration, _OutputTypeChooser, delegate()
        /// {
        ///     _JobSpec.OutputTypeSet(string.Empty);
        ///     return;
        /// });
        /// </code>
        /// </example>
        private void Cancel(UIBase ControlBeingCancelled, UIBase ControlToActivate, Cancel_ThingsToDo ThingsToUndo)
        {
            if (_CurrentControl != ControlBeingCancelled)
            {
                throw new Exception("Developer error in UI step order");
            }
            {
                SuspendLayout();
                Controls.Remove(ControlBeingCancelled); ControlBeingCancelled = null;
                Controls.Add(ControlToActivate);
                _CurrentControl = ControlToActivate;
                ResumeLayout(false);
                PerformLayout();
            }
            ThingsToUndo();
            if (ControlToActivate.Visible)
            {
                ControlToActivate.Select();
                ControlToActivate.Focus();
                AcceptButton = ControlToActivate.AcceptButton;
                CancelButton = ControlToActivate.CancelButton;
                _StatusLabel.Text = ControlToActivate.StatusText;
            }
            else
            {
                ControlToActivate.OnCancel();
            }
            return;
        } // Cancel()

        private void TestProgress()
        {
#if DEBUGGING
            int nMax = 20 * 1000;
#if DEBUGGING
            //
            //_Ph.SupportsCancellation = false;    // default true
            //_Ph.Maximum = -1; // <0 0 >0 default 100
            _Ph.Maximum = 0; // <0 0 >0 default 100
            _Ph.Maximum = nMax; // <0 0 >0 default 100
            //_Ph.IntervalMs = 500;
            _Ph.ProgressSection(delegate() /* Test */
            {
                int nSlice = 250;
                for (int n = 0; n < nMax; n += nSlice)
                {
                    if (_Ph.CancellationPending) break;
                    _Ph.Value = n;
                    _Ph.StatusText = (n / 1000) + " seconds of " + (nMax / 1000) + " seconds";
                    System.Threading.Thread.Sleep(nSlice);
                }
            });
#endif
//#if DEBUGGING
            //
            _Ph.ProgressSection(delegate() /* Test default Maximum > 0 */
            {
                int nSlice = 500;
                for (int n = 0; n < nMax; n += nSlice)
                {
                    if (_Ph.CancellationPending) break;
                    _Ph.Value = (n * 100) / nMax;
                    _Ph.StatusText  = (n / 1000) + " seconds of " + (nMax / 1000) + " seconds";
                    System.Threading.Thread.Sleep(nSlice);
                }
            });
            //
            _Ph.Maximum = nMax;
            _Ph.ProgressSection(delegate() /* Test non-default Maximum > 0 */
            {
                int nSlice = 500;
                for (int n = 0; n < nMax; n += nSlice)
                {
                    if (_Ph.CancellationPending) break;
                    _Ph.Value = n;
                    _Ph.StatusText = (n / 1000) + " seconds of " + (nMax / 1000) + " seconds";
                    System.Threading.Thread.Sleep(nSlice);
                }
            });
//#endif
#endif
            return;
        } // TestProgress()

    } // class DataLoader
}
