/* "Wizard" steps
 * 
 * Login
 * OutputTypeChooser
 * OutputConfiguration (optional)
 * FolderChooser [Browse...]
 * InputConfiguration (optional)
 * InputTableChooser (optional)
 * InputFileLabelAndType [Save... Load...]
 * InputOutputMapper [Save... Load... Preview...]
 * OutputFileChooser [Browse...]
 * JobConfiguration
 * JobSummary
 *
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.COE.ConfigLoader.Data;
using CambridgeSoft.COE.ConfigLoader.Data.InputObjects;
using CambridgeSoft.COE.ConfigLoader.Data.OutputObjects;
using CambridgeSoft.COE.ConfigLoader.Windows.Common;
using CambridgeSoft.COE.ConfigLoader.Windows.Controls;

namespace CambridgeSoft.COE.ConfigLoader.Windows.Forms
{
    /// <summary>
    /// This is the main Form of the application
    /// </summary>
    public partial class ConfigLoader : Form
    {
        private const string Task_load = "Load a previously saved job";
        private const string Task_view = "View previous job summary";
        private const string HELP_FILE_PATH = @"\Help\ConfigLoader.chm";

        /// <summary>
        /// <para>Code to execute when advancing to a UI step</para>
        /// <para>For use with <see cref="Accept"/></para>
        /// </summary>
        private delegate bool Accept_PrepareToActivate();

        /// <summary>
        /// <para>Code to execute when cancelling a UI step</para>
        /// <para>For use with <see cref="Cancel"/></para>
        /// </summary>
        private delegate void Cancel_ThingsToDo();

        #region >Private data<

        private static string _strApplicationPath;
        /// <summary>
        /// Temporary ApplicationPath for loading icons etc.
        /// </summary>
        public static string ApplicationPath
        {
            get { return _strApplicationPath; }
        }

        // Job
        Job _Job;
        // Potential Input / Output Objects
        private List<InputObject> _InputObjectList;
        private List<OutputObject> _OutputObjectList;
        //xprivate string _strInputObjectFilters;
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
        //xprivate OutputConfiguration _OutputConfiguration;
        private FolderChooser _FolderChooser;
        //xprivate InputConfiguration _InputConfiguration;
        //xprivate InputTableChooser _InputTableChooser;
        //xprivate InputFileLabelAndType _InputFileLabelAndType;
        //xprivate InputOutputMapper _InputOutputMapper;
        //xprivate OutputFileChooser _OutputFileChooser;
        //xprivate JobConfiguration _JobConfiguration;
        private JobSummary _JobSummary;
        private Control _CurrentControl; // Alias

        // status bar
        private readonly System.Windows.Forms.StatusStrip _StatusStrip;
        private readonly System.Windows.Forms.ToolStripStatusLabel _StatusLabel;

        private readonly COEProgressHelper _Ph = new COEProgressHelper();

        #endregion

        /// <summary>
        /// <c>ConfigLoader</c> is the constructor for the main form.
        /// </summary>
        public ConfigLoader()
        {
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
            //BackColor = UIBase.LightGray;
            BackColor = UIBase.White; //JHS 10/29/2008
            Size = new Size(800, 600);
            {
                SuspendLayout();

                {
                    MainMenu oMainMenu = new MainMenu();
                    MenuItem oMenuItem = new MenuItem("&Help");
                    {
                        MenuItem oMenuSubItem = new MenuItem("&Search", new EventHandler(HelpSearch_Click));
                        oMenuItem.MenuItems.Add(oMenuSubItem);
                    }
                    {
                        MenuItem oMenuSubItem = new MenuItem("&Contents", new EventHandler(HelpContents_Click));
                        oMenuItem.MenuItems.Add(oMenuSubItem);
                    }
                    {
                        MenuItem oMenuSubItem = new MenuItem("&Index", new EventHandler(HelpIndex_Click), Shortcut.F1);
                        oMenuItem.MenuItems.Add(oMenuSubItem);
                    }
                    {
                        MenuItem oMenuSubItem = new MenuItem("-");
                        oMenuItem.MenuItems.Add(oMenuSubItem);
                    }
                    {
                        MenuItem oMenuSubItem = new MenuItem("&About Config Loader", new EventHandler(HelpAbout_Click));
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
                _Login = new Login(true);
                _Login.Accept += new EventHandler(Login_Ok);
                _Login.Cancel += new EventHandler(Login_Cancel);
                //
                _OutputTypeChooser = new OutputTypeChooser();
                _OutputTypeChooser.Accept += new EventHandler(OutputTypeChooser_Ok);
                _OutputTypeChooser.Cancel += new EventHandler(OutputTypeChooser_Cancel);
#if UNUSED
                //
                _OutputConfiguration = new OutputConfiguration();
                _OutputConfiguration.Accept += new EventHandler(OutputConfiguration_Ok);
                _OutputConfiguration.Cancel += new EventHandler(OutputConfiguration_Cancel);
#endif
                //
                _FolderChooser = new FolderChooser();
                _FolderChooser.Accept += new EventHandler(FolderChooser_Ok);
                _FolderChooser.Cancel += new EventHandler(FolderChooser_Cancel);
#if UNUSED
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
#endif
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
        }

        private void ConfigLoader_Layout(object sender, LayoutEventArgs e)
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
        }
        private void ConfigLoader_Load(object sender, EventArgs e)
        {
            try
            {
                Icon = CambridgeSoft.COE.ConfigLoader.Properties.Resources.CL;
                Text = Application.ProductName;
                {
                    string strVersion = "";
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
                Layout += new LayoutEventHandler(ConfigLoader_Layout);
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
        }

        private void Login_Cancel(object sender, EventArgs e)
        {
            // Transition Login to Exit
            {
                SuspendLayout();
                Controls.Remove(_Login); _Login = null;
                ResumeLayout(false);
                PerformLayout();
            }
            _StatusLabel.Text = "";
            Close();
            return;
        }
        private void Login_Ok(object sender, EventArgs e)
        {
            // need logged in user to get these
            _InputObjectList = InputObject.ObjectList;
            _OutputObjectList = OutputObject.ObjectList;
#if UNUSED
            // Derive
            _strInputObjectFilters = "";
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
#endif
            _OutputDictionary = new Dictionary<string, string>();
            foreach (OutputObject outputObject in _OutputObjectList)
            {
                _OutputDictionary.Add(outputObject.ToString(), outputObject.OutputType);
            }
            _TaskList = new List<string>();
            Accept(_Login, _OutputTypeChooser, delegate()
            {
                _Job.User = _Login.User;
                _JobSpec.Visible = true;

                _OutputTypeChooser.Setup(_OutputDictionary, _TaskList);
                return false;
            });
            return;
        }

        private void OutputTypeChooser_Cancel(object sender, EventArgs e)
        {
            Cancel(_OutputTypeChooser, _Login, delegate()
            {
                _Job.User = null;
                _JobSpec.Visible = false;
                return;
            });
            return;
        }
        private void OutputTypeChooser_Ok(object sender, EventArgs e)
        {
            int nOutputType = _OutputTypeChooser.OutputType;
            Accept(_OutputTypeChooser, _FolderChooser, delegate()
            {
                _Job.Output = _OutputObjectList[nOutputType];
                _Job.OutputType = _OutputObjectList[nOutputType].OutputType;
                _Job.Input = _InputObjectList[nOutputType]; // ASSUMPTION ! ! !
                _FolderChooser.Direction = (_Job.OutputType.EndsWith("Import")) ? FolderChooser.DirectionType.Import : FolderChooser.DirectionType.Export;
                return false;
            });
            return;
        }

        private void FolderChooser_Cancel(object sender, EventArgs e)
        {
            Cancel(_FolderChooser, _OutputTypeChooser, delegate()
            {
                _Job.OutputConfiguration = "";
                _Job.Output = null;
                _Job.OutputType = "";
                return;
            });
            return;
        }
        private void FolderChooser_Ok(object sender, EventArgs e)
        {
            Accept(_FolderChooser, _JobSummary, delegate()
            {
                string strFolder = _FolderChooser.Folder;
                // Make sure it at least exists
                if (Directory.Exists(strFolder) == false)
                {
                    MessageBox.Show("Folder does not exist: '" + strFolder + "'", "Error");
                    return true;  // ERROR
                }
                // Save folder name
                if (_FolderChooser.Direction == FolderChooser.DirectionType.Import)
                {
                    _Job.InputDb = strFolder;
                }
                else
                {
                    _Job.OutputDb = strFolder;
                }
                _Job.OpenInputDb();
                _Job.OpenInputTable();
                _Job.InputFieldSpec = _Job.InputFieldSpec;
                _Job.JobStart = 1;
                _Job.JobCount = Int32.MaxValue;
                {
                    COEXmlTextWriter oCOEXmlTextWriter = new COEXmlTextWriter();
                    oCOEXmlTextWriter.WriteStartElement("fieldlist");
                    if (_FolderChooser.Direction == FolderChooser.DirectionType.Import)
                    {
                        oCOEXmlTextWriter.WriteStartElement("field");
                        oCOEXmlTextWriter.WriteAttributeString("name", "Metadata");
                        oCOEXmlTextWriter.WriteAttributeString("source", "map");
                        oCOEXmlTextWriter.WriteAttributeString("value", "Metadata");
                        oCOEXmlTextWriter.WriteAttributeString("fields", "Metadata");
                        oCOEXmlTextWriter.WriteEndElement();
                    }
                    {
                        oCOEXmlTextWriter.WriteStartElement("field");
                        oCOEXmlTextWriter.WriteAttributeString("name", "XML");
                        oCOEXmlTextWriter.WriteAttributeString("source", "map");
                        oCOEXmlTextWriter.WriteAttributeString("value", "XML");
                        oCOEXmlTextWriter.WriteAttributeString("fields", "XML");
                        oCOEXmlTextWriter.WriteEndElement();
                    }
                    oCOEXmlTextWriter.WriteEndElement();
                    _Job.Mappings = UIBase.FormatXmlString(oCOEXmlTextWriter.XmlString);
                    oCOEXmlTextWriter.Close();
                }
                // Execute
                _Job.Execute(); // Could fail iff invalid ???
                _JobSummary.Job = _Job;
                _JobSummary.LogMessagePath = _Job.LogMessagePath;
                return false;
            });
            return;
        }

        private void JobSummary_Cancel(object sender, EventArgs e)
        {
            {
                Cancel(_JobSummary, _FolderChooser, delegate()
                {
                    return;
                });
            }
            return;
        }
        private void JobSummary_Ok(object sender, EventArgs e)
        {
            {
                Accept(_JobSummary, _OutputTypeChooser, delegate()
                {
                    {
                        // _JobSummary
                        // _JobConfiguration
                        _Job.OutputDb = "";
                        _Job.JobStart = 0;
                        _Job.JobCount = 0;
                        // _OutputFileChooser
                        _Job.Mappings = "";
                        // _InputOutputMapper
                        _Job.InputFieldSpec = "";
                        // _InputFileLabelAndType
                        _Job.InputFieldSort = "";
                        _Job.CloseInputTable();
                        _Job.InputTable = "";
                        // _InputTableChooser
                        _Job.InputConfiguration = "";
                        // _InputConfiguration
                        _Job.CloseInputDb();
                        _Job.InputDb = "";
                        // _FolderChooser
                        _Job.OutputConfiguration = "";
                        // _OutputConfiguration
                        _Job.Output = null;
                        _Job.OutputType = "";
                    }

                    return false;
                });
            }
            return;
        }

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
        }

        #region >Event-handlers<

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
            }
            return;
        }

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
        ///     _JobSpec.OutputTypeSet("");
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
        }

        /// <summary>
        /// Displays the standard About box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpAbout_Click(object sender, EventArgs e)
        {
            Form frmAbout = new Form();
            frmAbout.BackColor = Color.White;
            frmAbout.FormBorderStyle = FormBorderStyle.FixedDialog;
            frmAbout.Icon = Properties.Resources.CL;
            frmAbout.Size = new Size(383, 357);
            frmAbout.StartPosition = FormStartPosition.CenterParent;
            frmAbout.MinimizeBox = false;
            frmAbout.MaximizeBox = false;

            frmAbout.SuspendLayout();
            AboutControl about = new AboutControl();
            frmAbout.Controls.Add(about);
            about.ExtractAndBindAssemblyData();
            about.DictatesToParentForm = true;
            about.Dock = DockStyle.Fill;
            about.LogoBox.Image = Properties.Resources.ConfigLoader_02;
            frmAbout.ResumeLayout();

            frmAbout.ShowDialog();
        }

        /// <summary>
        /// Displays the Table of Contents for the applciation's Help file (CHM)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpContents_Click(object sender, EventArgs e)
        {
            HelpNavigator oHelpNavigator = HelpNavigator.TableOfContents;
            ShowHelp(ApplicationPath + HELP_FILE_PATH, oHelpNavigator, null);
        }

        /// <summary>
        /// Displays the Index for the applciation's Help file (CHM)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpIndex_Click(object sender, EventArgs e)
        {
            HelpNavigator oHelpNavigator = HelpNavigator.Index;
            ShowHelp(ApplicationPath + HELP_FILE_PATH, oHelpNavigator, null);
        }

        /// <summary>
        /// Displays the Search page of the applciation's Help file (CHM)
        /// </summary>
        /// <remarks>
        /// The 'parameter' used for Help.ShowHelp must NOT be the null value, so instead
        /// this method uses the string.Empty value.
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpSearch_Click(object sender, EventArgs e)
        {
            HelpNavigator oHelpNavigator = HelpNavigator.Find;
            ShowHelp(ApplicationPath + HELP_FILE_PATH, oHelpNavigator, string.Empty);
        }

        private void ShowHelp(string helpURL, HelpNavigator helpNavigator, object parameter)
        {
            if (File.Exists(helpURL) == false)
                MessageBox.Show("Missing help file: " + helpURL, "Help", MessageBoxButtons.OK);
            else
                Help.ShowHelp(this, helpURL.Replace('\\', '/'), helpNavigator, parameter);
        }

        #endregion

    }
}
