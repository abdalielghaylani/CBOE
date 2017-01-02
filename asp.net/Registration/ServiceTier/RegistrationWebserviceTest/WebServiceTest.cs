using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.Web.Services.Protocols;

namespace RegistrationWebserviceTest {
    public partial class WebServiceTest : Form {
        #region Variables
        XmlDocument _xmlDoc = new XmlDocument();
        string _resultingId = string.Empty;
        string _temporaryId = string.Empty;
        string _authTicket;
        #endregion

        #region Constructors
        public WebServiceTest() {
            InitializeComponent();
        }
        #endregion

        #region Events
        
        private void BrowseButton_Click(object sender, EventArgs e) {
            using(this.openFileDialog1 = new OpenFileDialog()) {
                this.openFileDialog1.DefaultExt = "xml";
                this.openFileDialog1.Filter = "XML Files|*.xml;*.txt|All Files|*.*";
                this.openFileDialog1.Multiselect = false;
                DialogResult result = this.openFileDialog1.ShowDialog(this);
                if(result == DialogResult.OK) {
                    ShowFileLabel(this.openFileDialog1.FileName);
                    ClearProceedLinkLabel();
                    ClearTempIdLinkLabel();
                    ClearReviewLinkLabel();
                    EnableProceedButton(false);
                    EnableReviewButton(false);
                    EnableDeleteButton(false);
                }
            }
        }

        private void ProceedButton_Click(object sender, EventArgs e) {
            try {
                CallWebServiceSSO(this.UserNameTextBox.Text, this.PasswordTextBox.Text);
                //CallWebServiceSave(this.UserNameTextBox.Text, this.PasswordTextBox.Text);
                CallWebServiceSave(_authTicket);
                ShowSubmitUrl();
                EnableReviewButton(true);
            } catch(Exception ex) {
                this.ProceedLinkLabel.Text = "The following errors occurred:\n " + ex.Message;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                CallWebServiceDeleteTemporary(_temporaryId, _authTicket);

                this.ReviewLinkLabel.Text = string.Format("The temporary registry {0} registry was deleted", _temporaryId);
                this.ReviewLinkLabel.Enabled = false;

                this.EnableReviewButton(false);
            }
            catch (Exception ex)
            {
                this.ReviewLinkLabel.Text = "The following errors occurred:\n " + ex.Message;
                this.ReviewLinkLabel.Enabled = false;
            }
        }

        private void ProceedLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if(e.Button == MouseButtons.Left)
                System.Diagnostics.Process.Start("IExplore", ProceedLinkLabel.Text + "&ticket=" + _authTicket);
        }

        private void ReviewLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if(e.Button == MouseButtons.Left)
                System.Diagnostics.Process.Start("IExplore", ReviewLinkLabel.Text + "&ticket=" + _authTicket);
        }

        private void TempIDLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if(e.Button == MouseButtons.Left)
                CallWebServiceGetTempID(this.UserNameTextBox.Text, this.PasswordTextBox.Text);
            ChangeTempIDLinkLabel();
            EnableProceedButton(true);
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e) {
            if(LinkContextMenuStrip.SourceControl.Name == "ProceedLinkLabel")
                Clipboard.SetData(DataFormats.StringFormat, this.ProceedLinkLabel.Text);
            else if(LinkContextMenuStrip.SourceControl.Name == "ReviewLinkLabel")
                Clipboard.SetData(DataFormats.StringFormat, this.ReviewLinkLabel.Text);
        }

        private void ReviewButton_Click(object sender, EventArgs e) {
            if(ShowReviewUrl())
                EnableDeleteButton(true);
        }

        private void FillPickListButton_Click(object sender, EventArgs e)
        {
            CallWebServiceSSO(UserNameTextBox.Text, PasswordTextBox.Text);
            PickListListBox.Items.Clear();
            string result = CallWebServiceGetPickList(PickListCriteria.Text, _authTicket);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            foreach(XmlNode node in doc.SelectNodes("//PicklistItem"))
            {
                PickListListBox.Items.Add("(ID - Value): " + node.Attributes["ID"].Value + " - "+ node.InnerText);
            }
        }
        #endregion

        #region UI Methods

        private void ClearProceedLinkLabel() {
            this.ProceedLinkLabel.Text = "Click Proceed to get the link...";
            this.ProceedLinkLabel.Enabled = false;
        }

        private void ClearTempIdLinkLabel() {
            this.TempIDLinkLabel.Text = "Click here to get a Temporary ID...";
            this.TempIDLinkLabel.Enabled = true;
        }

        private void ClearReviewLinkLabel() {
            this.ReviewLinkLabel.Text = "Click Review to get the link...";
            this.ReviewLinkLabel.Enabled = false;
        }

        private void ChangeTempIDLinkLabel() {
            this.TempIDLinkLabel.Text = _temporaryId;
            this.TempIDLinkLabel.Enabled = false;
        }

        private void ShowFileLabel(string fileName) {
            this.FileNameLabel.Text = fileName;
        }

        private void ShowSubmitUrl() {
            if(_resultingId.ToUpper() == "NOT AUTHORIZED") {
                this.ProceedLinkLabel.Text = "User Name or Password does not have permission to perform the operation";
                this.ProceedLinkLabel.Enabled = false;
            } else if(_resultingId.ToUpper() == "NOT SAVED") {
                this.ProceedLinkLabel.Text = "There was an error saving the compound, and the operation failed";
                this.ProceedLinkLabel.Enabled = false;
            } else {
                //string webServerName = Properties.Settings.Default.PropertyValues["WebServerName"].PropertyValue.ToString();

                COERegistrationServices.COERegistrationServices webserver = new RegistrationWebserviceTest.COERegistrationServices.COERegistrationServices();
                string webServerName = webserver.Url.Replace("http://", "");
                webServerName = webServerName.Substring(0, webServerName.IndexOf('/'));

                if(_xmlDoc.FirstChild.Name.Contains("SingleCompound")) {
                    this.ProceedLinkLabel.Text = "http://" + webServerName + "/COERegistration/Forms/ContentArea/AddRegistryRecord.aspx?SavedObjectId=" + _resultingId;
                    this.ProceedLinkLabel.Enabled = true;
                } else if(_xmlDoc.FirstChild.Name.Contains("MultiCompound")) {
                    this.ProceedLinkLabel.Text = "http://" + webServerName + "/COERegistration/Forms/SubmitRecord/ContentArea/SubmitMixture.aspx?SavedObjectId=" + _resultingId;
                    this.ProceedLinkLabel.Enabled = true;
                }
            }
        }

        private bool ShowReviewUrl() {
            if(_resultingId.ToUpper() == "NOT AUTHORIZED") {
                this.ReviewLinkLabel.Text = "User Name or Password does not have permission to perform the operation";
                this.ReviewLinkLabel.Enabled = false;
            } else if(_resultingId.ToUpper() == "NOT SAVED") {
                this.ReviewLinkLabel.Text = "There was an error saving the compound, and the operation failed";
                this.ReviewLinkLabel.Enabled = false;
            } else if(!string.IsNullOrEmpty(_temporaryId)) {
                COERegistrationServices.COERegistrationServices webserver = new RegistrationWebserviceTest.COERegistrationServices.COERegistrationServices();
                string webServerName = webserver.Url.Replace("http://", "");
                webServerName = webServerName.Substring(0, webServerName.IndexOf('/'));

                if(_xmlDoc.FirstChild.Name.Contains("SingleCompound")) {
                    this.ReviewLinkLabel.Text = "http://" + webServerName + "/COERegistration/Forms/ContentArea/ReviewRegister.aspx?SubmittedObjectId=" + _temporaryId;
                    this.ReviewLinkLabel.Enabled = true;
                } else if(_xmlDoc.FirstChild.Name.Contains("MultiCompound")) {
                    this.ReviewLinkLabel.Text = "http://" + webServerName + "/COERegistration/Forms/ReviewRegister/ContentArea/ReviewRegisterMixture.aspx?SubmittedObjectId=" + _temporaryId;
                    this.ReviewLinkLabel.Enabled = true;
                }
            }

            return ReviewLinkLabel.Enabled;
        }

        private void EnableProceedButton(bool enabled) {
            this.ProceedButton.Enabled = enabled;
        }

        private void EnableReviewButton(bool enabled) {
            this.ReviewButton.Enabled = enabled;
        }
        private void EnableDeleteButton(bool enabled)
        {
            this.DeleteButton.Enabled = enabled;
        }
        #endregion

        #region Registration WebService Calls

        private void CallWebServiceSave(string userName, string password) {
            COERegistrationServices.COECredentials credentials = new COERegistrationServices.COECredentials();
            credentials.UserName = userName;
            credentials.Password = password;
            COERegistrationServices.COERegistrationServices webService = new COERegistrationServices.COERegistrationServices();
            webService.COECredentialsValue = credentials;
            _xmlDoc.Load(FileNameLabel.Text);
            _xmlDoc.FirstChild.FirstChild.InnerText = _temporaryId;
            try {
                _resultingId = webService.SaveRegistryRecord(_xmlDoc.OuterXml);
            } catch(SoapException se) {
                throw new Exception(se.Detail.InnerText);
            }
        }

        private void CallWebServiceSave(string authTicket) {
            COERegistrationServices.COECredentials credentials = new COERegistrationServices.COECredentials();
            credentials.AuthenticationTicket = authTicket;
            COERegistrationServices.COERegistrationServices webService = new COERegistrationServices.COERegistrationServices();
            webService.COECredentialsValue = credentials;
            _xmlDoc.Load(FileNameLabel.Text);
            _xmlDoc.FirstChild.FirstChild.InnerText = _temporaryId;
            try {
                _resultingId = webService.SaveRegistryRecord(_xmlDoc.OuterXml);
            } catch(SoapException se) {
                throw new Exception(se.Detail.InnerText);
            }
        }
        private void CallWebServiceDeleteTemporary(string _resultingId, string authTicket)
        {
            if (!string.IsNullOrEmpty(_resultingId) && int.Parse(_resultingId) > 0)
            {
                COERegistrationServices.COECredentials credentials = new COERegistrationServices.COECredentials();
                credentials.AuthenticationTicket = authTicket;
                COERegistrationServices.COERegistrationServices webService = new COERegistrationServices.COERegistrationServices();
                webService.COECredentialsValue = credentials;
                try {
                    //webService.DeleteTemporaryRegistry(int.Parse(_resultingId));
                } catch(SoapException se) {
                    throw new Exception(se.Detail.InnerText);
                }
            }
        }

        private void CallWebServiceGetTempID(string userName, string password) {
            COERegistrationServices.COECredentials credentials = new COERegistrationServices.COECredentials();
            credentials.UserName = userName;
            credentials.Password = password;
            COERegistrationServices.COERegistrationServices webService = new COERegistrationServices.COERegistrationServices();
            webService.COECredentialsValue = credentials;
            try {
                _temporaryId = webService.GetTempID();
            } catch(SoapException se) {
                throw new Exception(se.Detail.InnerText);
            }
        }

        private string CallWebServiceGetPickList(string picklistcriteria, string authTicket)
        {
            COERegistrationServices.COECredentials credentials = new COERegistrationServices.COECredentials();
            credentials.AuthenticationTicket = authTicket;
            COERegistrationServices.COERegistrationServices webService = new COERegistrationServices.COERegistrationServices();
            webService.COECredentialsValue = credentials;
            try
            {
                return webService.RetrievePicklist(picklistcriteria);
            }
            catch(SoapException se)
            {
                throw new Exception(se.Detail.InnerText);
            }
        }
 
        #endregion
        
        #region SSO WebService calls
        
        private void CallWebServiceSSO(string userName, string password) {
            SingleSignOn.SingleSignOn ssoWebService = new RegistrationWebserviceTest.SingleSignOn.SingleSignOn();
            _authTicket = ssoWebService.GetAuthenticationTicket(userName, password);
        }
        
        #endregion

    }
}