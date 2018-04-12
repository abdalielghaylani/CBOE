using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CambridgeSoft.DataLoaderGUI.Common;


namespace CambridgeSoft.DataLoaderGUI.Controls
{
    /// <summary>
    /// Base class for UI controls
    /// </summary>
    public partial class UIBase : UserControl
    {
        #region data
        /// <summary>
        /// !!
        /// </summary>
        private System.Windows.Forms.Button _btnAcceptButton = null;
        /// <summary>
        /// !!
        /// </summary>
        private System.Windows.Forms.Button _btnCancelButton = null;
        /// <summary>
        /// !!
        /// </summary>
        protected string _strStatusText = string.Empty;
        #endregion

        #region properties
        /// <summary>
        /// Get property so form can set AcceptButton
        /// </summary>
        public Button AcceptButton
        {
            get
            {
                return _btnAcceptButton;
            }
            private set
            {
                _btnAcceptButton = value;
                return;
            }
        } // AcceptButton

        /// <summary>
        /// Get property so that form can set CancelButton
        /// </summary>
        public Button CancelButton
        {
            get
            {
                return _btnCancelButton;
            }
            private set
            {
                _btnCancelButton = value;
                return;
            }
        } // CancelButton

        /// <summary>
        /// Extra vertical padding
        /// </summary>
        public static Padding ExtraPadding
        {
            get
            {
                return new Padding(3, 3, 3, 3);
            }
        } // ExtraPadding

        /// <summary>
        /// Get property so form can set Status.Text
        /// </summary>
        public string StatusText
        {
            get
            {
                return _strStatusText;
            }
            protected set
            {
                _strStatusText = value;
                return;
            }
        } // StatusText

        #endregion

        #region constructors
        /// <summary>
        /// !! Constructor
        /// </summary>
        public UIBase()
        {
            AcceptButton = UIBase.GetButton(ButtonType.Next);
            CancelButton = UIBase.GetButton(ButtonType.Back);
            return;
        } // UIBase()
        #endregion

        #region events
        /// <summary>
        /// EventHandler for the Accept button
        /// </summary>
        public event EventHandler Accept;
        /// <summary>
        /// !!
        /// </summary>
        /// <param name="e"></param>
        private void OnAccept(EventArgs e)
        {
            if (Accept != null)
            {
                Accept(this, e);
            }
            return;
        } // OnAccept()
        /// <summary>
        /// !!
        /// </summary>
        public void OnAccept()
        {
            OnAccept(new EventArgs());
            return;
        } // OnAccept()

        /// <summary>
        /// EventHandler for the Cancel button
        /// </summary>
        public event EventHandler Cancel;
        /// <summary>
        /// !!
        /// </summary>
        /// <param name="e"></param>
        private void OnCancel(EventArgs e)
        {
            if (Cancel != null)
            {
                Cancel(this, e);
            }
            return;
        } // OnCancel()
        /// <summary>
        /// !!
        /// </summary>
        public void OnCancel()
        {
            OnCancel(new EventArgs());
            return;
        } // OnCancel()
        #endregion

        #region colors
        /// <summary>
        /// Color to use in place of standard Black
        /// </summary>
        public static Color Black
        {
            get
            {
                //return Color.FromArgb(0xFF, 0xFF, 0x33); // 0,0,0 = 0x00,0x00,0x00
                //return Color.FromArgb(0x00, 0x00, 0xFF); // 0,0,0 = 0x00,0x00,0x00
                //return Color.FromArgb(0x00, 0x00, 0x99); // 0,0,0 = 0x00,0x00,0x00
                return Color.Black;
            }
        } // Black

        /// <summary>
        /// Color to use in place of standard DarkGray
        /// </summary>
        public static Color DarkGray
        {
            get
            {
                //return Color.FromArgb(0x33, 0x33, 0xFF);
                //return Color.FromArgb(0x00, 0x00, 0x99);
                return Color.DarkGray;
            }
        } // DarkGray

        /// <summary>
        /// Color to use in place of standard LightGray
        /// </summary>
        public static Color LightGray
        {
            get
            {

                //?return Color.FromArgb(0x99, 0x99, 0xFF); // 212,208,200 ~= 0xCC,0xCC,0xCC
                //return Color.FromArgb(0xDD, 0xDD, 0xFF); // 212,208,200 ~= 0xCC,0xCC,0xCC
                //return Color.FromArgb(0xCC, 0xCC, 0xCC);
                //return Color.FromArgb(0xFF, 0xFF, 0xFF);
                return Color.LightGray;

            }
        } // LightGray

        /// <summary>
        /// Color to use in place of standard White
        /// </summary>
        public static Color White
        {
            get
            {
                return Color.White;
                // return Color.FromArgb(0xCC, 0xCC, 0xCC); // 255,255,255 = 0xFF,0xFF,0xFF
            }
        } // White
        #endregion

        #region Windows controls factory
        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static Button GetButton()
        {
            Button oControl = new Button();
            oControl.BackColor = LightGray;
            oControl.ForeColor = Black;
            //oControl.BackgroundImage;
            //oControl.Image;
            //oControl.MouseEnter;
            //oControl.MouseLeave;
            oControl.Height = oControl.Margin.Top + 2 + 16 + 2 + oControl.Margin.Bottom;    // WJC 2 for border 16 for icon
            return oControl;
        } // GetButton()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static CheckBox GetCheckBox()
        {
            CheckBox oControl = new CheckBox();
            oControl.BackColor = White;
            oControl.ForeColor = Black;
            return oControl;
        } // GetCheckBox()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static ComboBox GetComboBox()
        {
            ComboBox oControl = new ComboBox();
            oControl.BackColor = White;
            oControl.ForeColor = Black;
            return oControl;
        } // GetComboBox()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static ContainerControl GetContainerControlClass()
        {
            ContainerControl oControl = new ContainerControl();
            oControl.BackColor = White;
            oControl.ForeColor = Black;
            return oControl;
        } // GetContainerControl()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static DateTimePicker GetDateTimePicker()
        {
            DateTimePicker oControl = new COEDateTimePicker();
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            oControl.BackColor = White; // No effect
            oControl.ForeColor = Black; // No effect
            return oControl;
        } // GetDateTimePicker()


        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static GroupBox GetGroupBox()
        {
            GroupBox oControl = new GroupBox();
            oControl.BackColor = White;
            oControl.ForeColor = Black;
            return oControl;
        } // GetGroupBox()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static Label GetLabel()
        {
            Label oControl = new Label();
            oControl.BackColor = White;
            oControl.ForeColor = Black;
            return oControl;
        } // GetLabel()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static ListView GetListView()
        {
            ListView oControl = new ListView();
            oControl.BackColor = White;
            oControl.ForeColor = Black;
            return oControl;
        } // GetListView()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static RadioButton GetRadioButton()
        {
            RadioButton oControl = new RadioButton();
            oControl.BackColor = White;
            oControl.ForeColor = Black;
            return oControl;
        } // GetRadioButton()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static RichTextBox GetRichTextBox()
        {
            RichTextBox oControl = new RichTextBox();
            oControl.BackColor = White;
            oControl.ForeColor = Black;
            // Default is to set up for a single line
            oControl.Multiline = false;
            oControl.Height = 1;
            {
                oControl.AutoSize = true;
                int nHeight = oControl.Height;
                oControl.AutoSize = false;
                oControl.Height = nHeight;
            }
            return oControl;
        } // GetRichTextBox()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static StatusStrip GetStatusStrip()
        {
            StatusStrip oControl = new StatusStrip();
            oControl.BackColor = LightGray;
            oControl.ForeColor = Black;
            return oControl;
        } // GetStatusStrip()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static TableLayoutPanel GetTableLayoutPanel()
        {
            TableLayoutPanel oControl = new TableLayoutPanel();
            // Gray looks nice as a contrast
            oControl.BackColor = LightGray;
            oControl.ForeColor = Black;
            return oControl;
        } // GetTableLayoutPanel()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static TextBox GetTextBox()
        {
            TextBox oControl = new TextBox();
            oControl.BackColor = White;
            oControl.ForeColor = Black;
            return oControl;
        } // GetTextBox()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// Tools Strip on the bottom of the window
        /// </summary>
        /// <returns></returns>
        public static ToolStripStatusLabel GetToolStripStatusLabel()
        {
            ToolStripStatusLabel oControl = new ToolStripStatusLabel();
            oControl.BackColor = LightGray;
            oControl.ForeColor = Black;
            return oControl;
        } // GetToolStripStatusLabel()

        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static TreeView GetTreeView()
        {
            TreeView oControl = new TreeView();
            oControl.BackColor = LightGray;
            oControl.ForeColor = Black;
            oControl.LineColor = Color.FromArgb(0, oControl.ForeColor);
            return oControl;
        } // GetTreeView()
        #endregion

        #region Data Loader controls factory
        /// <summary>
        /// Return the standard control with revised BackColor and ForeColor properties.
        /// </summary>
        /// <returns></returns>
        public static Label GetHeaderLabel()
        {
            Label oControl = GetLabel();
            oControl.Font = new Font(oControl.Font, FontStyle.Bold);
            return oControl;
        } // GetHeaderLabel()

        /// <summary>
        /// Types of builtin buttons with icons
        /// </summary>
        public enum ButtonType
        {
            /// <summary>
            /// A Next button
            /// </summary>
            Next,
            /// <summary>
            /// A Back button
            /// </summary>
            Back,
            /// <summary>
            /// Green check
            /// </summary>
            Accept,
            /// <summary>
            /// Red x
            /// </summary>
            Cancel,
            /// <summary>
            /// A Browse button
            /// </summary>
            Browse,
            /// <summary>
            /// A SaveAs button
            /// </summary>
            Save,
            /// <summary>
            /// An Open button
            /// </summary>
            Load,
            /// <summary>
            /// A Preview button
            /// </summary>
            Preview,
            /// <summary>
            /// A Sort button
            /// </summary>
            Sort,
            /// <summary>
            /// A Sort Ascending button
            /// </summary>
            SortAscending,
            /// <summary>
            /// A Sort Descending button
            /// </summary>
            SortDescending,
            /// <summary>
            /// An Add Item button
            /// </summary>
            AddItem,
            /// <summary>
            /// A Remove Item button
            /// </summary>
            RemoveItem,
            /// <summary>
            /// An Move Up button
            /// </summary>
            MoveUp,
            /// <summary>
            /// A Move Down button
            /// </summary>
            MoveDown,
            /// <summary>
            /// A Remove button
            /// </summary>
            Remove,
            /// <summary>
            /// A Rename button
            /// </summary>
            Rename,
            /// <summary>
            /// Standard Windows Error button
            /// </summary>
            Error,
            /// <summary>
            /// Standard Windows Info button
            /// </summary>
            Info,
            /// <summary>
            /// Standard Windows Warning button
            /// </summary>
            Warning,
            /// <summary>
            /// A Zoom In button
            /// </summary>
            ZoomIn,
            /// <summary>
            /// Standard Windows Copy to clipboard button
            /// </summary>
            Copy,
            /// <summary>
            /// Standard Windows Open button
            /// </summary>
            Open,
            /// <summary>
            /// Standard Windows Export button
            /// </summary>
            Export,
            /// <summary>
            /// Standard Windows Import button
            /// </summary>
            Import,
            /// <summary>
            /// Standard Windows button with blue "f"
            /// </summary>
            BlueF,
            /// <summary>
            /// Standard Windows button with blue book
            /// </summary>
            BlueBook,
        }; // enum ButtonType
        /// <summary>
        /// Type of PictureBoxes; which can also act as buttons
        /// </summary>
        public enum PictureBoxType
        {
            /// <summary>
            /// CambridgeSoft COE logo
            /// </summary>
            Logo,
            /// <summary>
            /// CambridgeSoft COE logo (extension)
            /// </summary>
            LogoMid,
            /// <summary>
            /// CambridgeSoft COE logo (right hand side)
            /// </summary>
            LogoRht,
            /// <summary>
            /// First
            /// </summary>
            First,
            /// <summary>
            /// Previous
            /// </summary>
            Prev,
            /// <summary>
            /// Next
            /// </summary>
            Next,
            /// <summary>
            /// Last
            /// </summary>
            Last,
            /// <summary>
            /// TEllipsis; red and green
            /// </summary>
            Ellipsis,
        };

        /// <summary>
        /// Fetch a Gif from the embedded resources
        /// </summary>
        /// <param name="vstrGif"></param>
        /// <returns></returns>
        public static Bitmap GetGif(string vstrGif)
        {
            System.Reflection.Assembly oAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            // string[] strResourceNames = oAssembly.GetManifestResourceNames();
            string strName = oAssembly.GetName().Name;
            System.IO.Stream oStream = oAssembly.GetManifestResourceStream("CambridgeSoft.COE." + strName + "." + "Resources" + "." + vstrGif + ".gif");
            return new Bitmap(oStream);
        } // GetGif()

        /// <summary>
        /// Fetch an icon from the embedded resources
        /// </summary>
        /// <param name="vstrIcon"></param>
        /// <returns></returns>
        public static Icon GetIcon(string vstrIcon)
        {
            System.Reflection.Assembly oAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            // string[] strResourceNames = oAssembly.GetManifestResourceNames();
            string strName = oAssembly.GetName().Name;
            System.IO.Stream oStream = oAssembly.GetManifestResourceStream("CambridgeSoft." + strName + "." + "Resources" + "." + vstrIcon + ".ico");
            //System.IO.Stream oStream = oAssembly.GetManifestResourceStream("CambridgeSoft.DataLoaderGUI.Resources.Arrow_Right_B.ico");
            return new Icon(oStream);
        } // GetIcon()

        private static Button GetButton(string vstrIcon)
        {
            Button button = GetButton();
            button.Image = GetIcon(vstrIcon).ToBitmap();
            return button;
        } // GetButton(string vstrIcon)

        /// <summary>
        /// Get a button of style <see cref="ButtonType"/>
        /// </summary>
        /// <param name="veIcon"></param>
        /// <returns></returns>
        public static Button GetButton(ButtonType veIcon)
        {
            Button button = null;
            switch (veIcon)
            {
                case ButtonType.Next:
                    {
                        button = GetButton("Arrow_Right_B");
                        button.AutoSize = true;
                        button.ImageAlign = ContentAlignment.MiddleRight;
                        button.TextAlign = ContentAlignment.MiddleLeft;
                        button.Text = "Next";
                        button.Location = new System.Drawing.Point(590, 380);
                        break;
                    }
                case ButtonType.Back:
                    {
                        button = GetButton("Arrow_Left_B");
                        button.AutoSize = true;
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Back";
                        button.Location = new System.Drawing.Point(500, 380);
                        break;
                    }
                case ButtonType.Browse:
                    {
                        button = GetButton("Search_Document");
                        button.AutoSize = true;
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Browse...";
                        break;
                    }
                case ButtonType.Save:
                    {
                        button = GetButton("Import_Data");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Save...";
                        break;
                    }
                case ButtonType.Load:
                    {
                        button = GetButton("Export_Data");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Load...";
                        break;
                    }
                case ButtonType.Accept:
                    {
                        button = GetButton("Tick");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "OK";
                        break;
                    }
                case ButtonType.Cancel:
                    {
                        button = GetButton("Cross_R");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Cancel";

                        break;
                    }
                case ButtonType.Preview:
                    {
                        button = GetButton("Preview");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Preview...";
                        break;
                    }
                case ButtonType.Sort:
                    {
                        button = GetButton("Sort_Ascending");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Sort...";
                        break;
                    }
                case ButtonType.SortAscending:
                    {
                        button = GetButton("Sort_Ascending");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Sort Ascending";
                        break;
                    }
                case ButtonType.SortDescending:
                    {
                        button = GetButton("Sort_Descending");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Sort Descending";
                        break;
                    }
                case ButtonType.AddItem:
                    {
                        button = GetButton("Arrow_Right");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Add";
                        break;
                    }
                case ButtonType.RemoveItem:
                    {
                        button = GetButton("Arrow_Left_R");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Remove";
                        break;
                    }
                case ButtonType.MoveUp:
                    {
                        button = GetButton("Arrow_Up_B");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Move up";
                        break;
                    }
                case ButtonType.MoveDown:
                    {
                        button = GetButton("Arrow_Down_B");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Move down";
                        break;
                    }
                case ButtonType.Remove:
                    {
                        button = GetButton("Remove");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Remove";
                        break;
                    }
                case ButtonType.Rename:
                    {
                        button = GetButton("Rename");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Rename";
                        break;
                    }
                case ButtonType.Error:
                    {
                        button = GetButton("Error");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Error";
                        break;
                    }
                case ButtonType.Info:
                    {
                        button = GetButton("Info");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Info";
                        break;
                    }
                case ButtonType.Warning:
                    {
                        button = GetButton("Warning");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Warning";
                        break;
                    }
                case ButtonType.ZoomIn:
                    {
                        button = GetButton("ZoomIn");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Zoom in";
                        break;
                    }
                case ButtonType.Copy:
                    {
                        button = GetButton("Copy");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Copy";
                        break;
                    }
                case ButtonType.Open:
                    {
                        button = GetButton("Open");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Open";
                        break;
                    }
                case ButtonType.Export:
                    {
                        button = GetButton("Export");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Export...";
                        break;
                    }
                case ButtonType.Import:
                    {
                        button = GetButton("Import");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Import...";
                        break;
                    }
                case ButtonType.BlueBook:
                    {
                        button = GetButton("Thesaurus_4");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Fields...";
                        break;
                    }
                case ButtonType.BlueF:
                    {
                        button = GetButton("75_B");
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Text = "Functions...";
                        button.Width = button.Height + TextRenderer.MeasureText(button.Text, button.Font).Width;
                        break;
                    }
                default:
                    {
                        throw new Exception("arg");
                    }
            } // switch (veIcon)
            return button;
        } // GetButton(ButtonType veIcon)

        /// <summary>
        /// Return the standard control
        /// </summary>
        /// <returns></returns>
        public static PictureBox GetPictureBox()
        {
            PictureBox oControl = new PictureBox();
            return oControl;
        } // GetPictureBox()

        private static PictureBox GetPictureBoxGif(string vstrIcon)
        {
            PictureBox oControl = new PictureBox();
            oControl.Image = GetGif(vstrIcon);
            return oControl;
        } // GetPictureBoxGif()

        private static PictureBox GetPictureBox(string vstrIcon, string vstrIcon_d)
        {
            PictureBox oControl = new PictureBox();
            ImageList il = new ImageList();
            il.Images.Add(GetIcon(vstrIcon).ToBitmap());
            il.Images.Add(GetIcon(vstrIcon_d).ToBitmap());
            oControl.Tag = il;
            oControl.Image = ((ImageList)oControl.Tag).Images[0];
            oControl.EnabledChanged += new EventHandler(PictureBox_EnabledChanged);
            return oControl;
        } // GetPictureBox()

        private static void PictureBox_EnabledChanged(object sender, EventArgs e)
        {
            PictureBox oControl = sender as PictureBox;
            oControl.Image = ((ImageList)oControl.Tag).Images[(oControl.Enabled) ? 0 : 1];
            return;
        } // PictureBox_EnabledChanged()

        /// <summary>
        /// Get a PictureBox of type <see cref="PictureBoxType"/>
        /// </summary>
        /// <param name="veIcon"></param>
        /// <returns></returns>
        public static PictureBox GetPictureBox(PictureBoxType veIcon)
        {
            PictureBox oControl = null;
            switch (veIcon)
            {
                case PictureBoxType.Logo:
                    {
                        oControl = GetPictureBoxGif("Data_Loader");
                        break;
                    }
                case PictureBoxType.LogoMid:
                    {
                        oControl = GetPictureBoxGif("repeater_image");
                        break;
                    }
                case PictureBoxType.LogoRht:
                    {
                        oControl = GetPictureBoxGif("repeater_image");
                        break;
                    }
                case PictureBoxType.First:
                    {
                        oControl = GetPictureBox("First_2_B", "First_2_B_d");
                        break;
                    }
                case PictureBoxType.Prev:
                    {
                        oControl = GetPictureBox("Previous_2_B", "Previous_2_B_d");
                        break;
                    }
                case PictureBoxType.Next:
                    {
                        oControl = GetPictureBox("Next_2_B", "Next_2_B_d");
                        break;
                    }
                case PictureBoxType.Last:
                    {
                        oControl = GetPictureBox("Last_2_B", "Last_2_B_d");
                        break;
                    }
                case PictureBoxType.Ellipsis:
                    {
                        oControl = GetPictureBox("106_R", "106_G");
                        break;
                    }
                default:
                    {
                        throw new Exception("arg");
                    }
            } // switch (veIcon)
            oControl.Height = 16;
            oControl.Width = 16;
            return oControl;
        } // GetPictureBox(PictureBoxType veIcon)

        #endregion

        #region static helper methods

        /// <summary>
        /// Mark the contents of a RichTextBox as being in error
        /// </summary>
        /// <param name="vrtb"></param>
        /// <param name="vnColumn"></param>
        /// <param name="vcColumns"></param>
        public static void RichTestBox_MarkError(RichTextBox vrtb, int vnColumn, int vcColumns)
        {
            vrtb.Select(vnColumn, vcColumns);
            vrtb.SelectionColor = Color.Red;
            return;
        } // RichTestBox_MarkError()

        /// <summary>
        /// Mark the contents of a RichTextBox as being in error
        /// </summary>
        /// <param name="vrtb"></param>
        public static void RichTestBox_MarkError(RichTextBox vrtb)
        {
            RichTestBox_MarkError(vrtb, 0, vrtb.Text.Length);
            return;
        } // RichTestBox_MarkError()

        /// <summary>
        /// Mark the contents of a RichTextBox as being in error
        /// </summary>
        /// <param name="vrtb"></param>
        public static void RichTestBox_MarkUnknownError(RichTextBox vrtb)
        {
            vrtb.Text = vrtb.Text + "?";
            vrtb.Select(vrtb.Text.Length - 1, 1);
            vrtb.SelectionColor = Color.FromArgb(254, 0, 0);
            return;
        } // RichTestBox_MarkUnknownError()

        /// <summary>
        /// Remove error marking, if any, from RichTextBox
        /// </summary>
        /// <param name="vrtb"></param>
        public static void RichTestBox_Unmark(RichTextBox vrtb)
        {
            int nColumn = vrtb.SelectionStart;
            int cColumns = vrtb.SelectionLength;
            bool bExtra = (vrtb.SelectionColor == Color.FromArgb(254, 0, 0));
            vrtb.SelectAll();
            vrtb.SelectionColor = UIBase.Black;
            vrtb.Select(nColumn, cColumns);
            if (bExtra)
            {
                vrtb.Text = vrtb.Text.Substring(0, vrtb.Text.Length - 1);
                vrtb.Select(vrtb.Text.Length, 0);
            }
            return;
        } // RichTestBox_Unmark()

        /// <summary>
        /// Remove invalid characters from a path in a TextBox
        /// </summary>
        /// <param name="vtb"></param>
        public static void TextBox_CleanPath(TextBox vtb)
        {
            int selectionStart = vtb.SelectionStart;
            int selectionEnd = selectionStart + vtb.SelectionLength;
            string strFullName = vtb.Text;
            int at;
            while ((at = strFullName.IndexOf(' ')) == 0)
            {
                if (at < selectionStart) selectionStart--;
                if (at < selectionEnd) selectionEnd--;
                strFullName = strFullName.Remove(at, 1);    // Remove leading space
            } // while (...)
            string strDirectoryName;
            string strFileName;
            // Remove misplaced volume separators
            while ((at = strFullName.LastIndexOf(Path.VolumeSeparatorChar)) >= 0)
            {
                if (at == 1) break; // Correct position;
                if (at < selectionStart) selectionStart--;
                if (at < selectionEnd) selectionEnd--;
                strFullName = strFullName.Remove(at, 1);    // Remove extraneous volume separator
            } // while (...)
            while ((at = strFullName.IndexOf(Path.VolumeSeparatorChar)) >= 0)
            {
                if (at == 1) break; // Correct position;
                if (at < selectionStart) selectionStart--;
                if (at < selectionEnd) selectionEnd--;
                strFullName = strFullName.Remove(at, 1);    // Remove extraneous volume separator
            } // while (...)
            // Separate path into directory and filename
            if ((at = strFullName.LastIndexOf(Path.DirectorySeparatorChar)) >= 0)
            {
                strDirectoryName = strFullName.Substring(0, at + 1);
                strFileName = strFullName.Substring(at + 1);
            }
            else if ((at = strFullName.LastIndexOf(Path.VolumeSeparatorChar)) >= 0)
            {
                strDirectoryName = strFullName.Substring(0, at + 1);
                strFileName = strFullName.Substring(at + 1);
            }
            else
            {
                strDirectoryName = string.Empty;
                strFileName = strFullName;
            }
            // Remove invalid characters from the directory
            while ((at = strDirectoryName.IndexOfAny(Path.GetInvalidPathChars())) >= 0)
            {
                if (at < selectionStart) selectionStart--;
                if (at < selectionEnd) selectionEnd--;
                strDirectoryName = strDirectoryName.Remove(at, 1);
            }
            // Remove invalid characters from the filename
            while ((at = strFileName.IndexOfAny(Path.GetInvalidFileNameChars())) >= 0)
            {
                if (at < selectionStart) selectionStart--;
                if (at < selectionEnd) selectionEnd--;
                strFileName = strFileName.Remove(at, 1);
            }
            // Reassemble the path
            strFullName = strDirectoryName + strFileName;
            // Only assign if changed
            if (vtb.Text != strFullName)
            {
                vtb.Text = strFullName;
                vtb.Select(selectionStart, selectionEnd - selectionStart);
            }
            return;
        } // TextBox_CleanPath()

        #endregion

    } // class UIBase
}
