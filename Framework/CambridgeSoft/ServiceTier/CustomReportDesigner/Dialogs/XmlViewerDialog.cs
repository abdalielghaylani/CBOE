using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace CambridgeSoft.COE.Framework.CustomReportDesigner.Dialogs
{
    public partial class XmlViewerDialog : Form
    {
        #region Methods
        #region Constructors
        public XmlViewerDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region public methods
        public DialogResult ShowDialog(string xml)
        {
            string path = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf("\\") + 1) + "TemporaryXmlFile.xml";
            XmlDocument dataViewDocument = new XmlDocument();
            dataViewDocument.LoadXml(xml);
            dataViewDocument.Save(path);

            XmlViewerWebBrowser.Navigate(path);
            XmlViewerWebBrowser.Show();

            return this.ShowDialog();
        }
        #endregion
        #endregion
    }
}
