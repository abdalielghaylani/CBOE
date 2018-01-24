using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Configuration;

namespace CambridgeSoft.COE.Framework.Common
{
    /// <summary>
    /// This class defines the chembioviz behavior for parent applications. 
    /// <code lang="Xml">
    ///     &lt;parentApplication&gt;
	///     	&lt;add name="REGISTRATION"&gt;
	///     		&lt;applicationBehaviour&gt;
	///     			&lt;generalOptions&gt;
	///     			&lt;/generalOptions&gt;
	///     			&lt;chemDrawOptions&gt;
	///     				&lt;add name="ChemDrawPluginPolicy" value="Available"/&gt;
	///     				&lt;!-- Allowable values are: Available | Unavailable | Detect --&gt;
	///     			&lt;/chemDrawOptions&gt;
	///     			&lt;leftPanelOptions&gt;
	///     				&lt;add name="ShowHitlistManagement" value="NO"/&gt;
	///     				&lt;!-- Allowable values are YES | NO --&gt;
	///     				&lt;add name="ShowQueryManagement" value="NO"/&gt;
	///     				&lt;!-- Allowable values are YES | NO --&gt;
	///     				&lt;add name="ShowExportManagement" value="NO"/&gt;
	///     				&lt;!-- Allowable values are YES | NO --&gt;
	///     			&lt;/leftPanelOptions&gt;
	///     			&lt;menuOptions&gt;
	///     				&lt;add name="ShowHitlistMenu" value="NO"/&gt;
	///     				&lt;!-- Allowable values are YES | NO --&gt;
	///     				&lt;add name="ShowQueryMenu" value="NO"/&gt;
	///     				&lt;!-- Allowable values are YES | NO --&gt;
	///     				&lt;add name="ShowExportMenu" value="NO"/&gt;
	///     				&lt;!-- Allowable values are YES | NO --&gt;
	///     			&lt;/menuOptions&gt;
	///     			&lt;actionLinks&gt;
	///     			&lt;/actionLinks&gt;
	///     		&lt;/applicationBehaviour&gt;
	///     		&lt;formBehaviour&gt;
	///     			&lt;form formId="4002"&gt;
	///     				&lt;actionLinks&gt;
	///     					&lt;link id="RegisterMarkedLink" href="/COERegistration/Forms/BulkRegisterMarked/ContentArea/RegisterMarked.aspx?COESavedHitListID={0}" text="Register Marked" target="_parent" tooltip="Registration - Register" cssClass="MenuItemLink" enabled="true"/&gt;
	///     					&lt;link id="DeleteMarkedLink" href="/COERegistration/Forms/BulkRegisterMarked/ContentArea/DeleteMarked.aspx?COESavedHitListID={0}" text="Delete Marked" target="_parent" tooltip="Registration - Delete" cssClass="MenuItemLink" enabled="true"/&gt;
	///     				&lt;/actionLinks&gt;
	///     			&lt;/form&gt;
	///     			&lt;form formId="4003"&gt;
	///     				&lt;actionLinks&gt;
	///     					&lt;link id="SendToInventoryLink" href="" text="Send To Inventory" target="_parent" tooltip="Inventory - Send to" cssClass="MenuItemLink" enabled="false"/&gt;
	///     					&lt;link id="DeleteMarkedLink" href="/COERegistration/Forms/BulkRegisterMarked/ContentArea/DeleteMarked.aspx?COESavedHitListID={0}" text="Delete Marked" target="_parent" tooltip="Registration - Delete" cssClass="MenuItemLink" enabled="true"/&gt;
	///     				&lt;/actionLinks&gt;
	///     			&lt;/form&gt;
	///     		&lt;/formBehaviour&gt;
	///     	&lt;/add&gt;
    ///     &lt;/parentApplication&gt;
    /// </code>
    /// </summary>
    public class ParentApplicationData : COENamedConfigurationElement
    {
        private const string applicationBehaviour = "applicationBehaviour";
        [ConfigurationProperty(applicationBehaviour, IsRequired = false)]
        public Behaviour ApplicationBehaviour
        {
            get { return (Behaviour) base[applicationBehaviour]; }
            set { base[applicationBehaviour] = value; }
        }
    }
}
