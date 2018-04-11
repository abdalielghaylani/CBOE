using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;

using CambridgeSoft.COE.Framework;
using CambridgeSoft.COE.Framework.Common;
using CambridgeSoft.COE.Framework.COEDataViewService;
using CambridgeSoft.COE.Framework.COESecurityService;
using CambridgeSoft.COE.Framework.Common.Messaging;

using FormDBLib;
using CBVUtilities;

namespace FormDBLib
{
    //---------------------------------------------------------------------
    public class PrivilegeChecker
    {
        #region Variables

        private COEPrincipal m_principal;
        private COEIdentity m_ident;
        private COERoleBOList m_roleList;

        private bool m_bCanSearch;
        private bool m_bCanEditForms;
        private bool m_bCanSavePublic;
        private bool m_bCanSaveSettings;

        #endregion

        #region Properties

        public bool CanSearch { get { Init(); return m_bCanSearch; } }
        public bool CanEditForms { get { Init(); return m_bCanEditForms; } }
        public bool CanSavePublic { get { Init(); return m_bCanSavePublic; } }
        public bool CanSaveSettings { get { Init(); return m_bCanSaveSettings; } }

        //---------------------------------------------------------------------
        public List<String> RoleNames
        {
            get
            {
                Init();
                List<String> names = new List<String>();
                foreach (COERoleBO role in m_roleList)
                    names.Add(role.RoleName);
                return names;
            }
        }
        #endregion

        #region Constructor
        public PrivilegeChecker()
        {
            m_principal = null;
            m_ident = null;
            m_roleList = null;
        }
        #endregion

        #region Methods

        public void Init()
        {
            if (m_principal == null)
            {
                m_principal = (COEPrincipal)Csla.ApplicationContext.User;
                m_ident = (COEIdentity)m_principal.Identity;
                m_roleList = COERoleBOList.GetListByUser(m_ident.Name);

                m_bCanSearch = HasPrivilege(CBVConstants.CAN_SEARCH);
                m_bCanEditForms = HasPrivilege(CBVConstants.CAN_EDIT_FORMS);
                m_bCanSavePublic = HasPrivilege(CBVConstants.CAN_SAVE_PUBLIC_FORM);
                m_bCanSaveSettings = HasPrivilege(CBVConstants.CAN_SAVE_DEFAULT_SETTINGS);
            }
        }
         //---------------------------------------------------------------------
        public bool HasAnyCBVPrivileges()
        {
            Init();
            return m_bCanSearch || m_bCanEditForms || m_bCanSavePublic || m_bCanSaveSettings;
        }
       //---------------------------------------------------------------------
        public void AllowMinimalCBVPrivileges()
        {
            m_bCanSearch = true;
        }
        //---------------------------------------------------------------------
        public bool HasPrivilege(String privName)
        {
            // true if current user has given privilege in any current role
            // privName is like "CAN_BROWSE", etc.
            Init();
            return HasPrivilege(m_ident, privName, m_roleList);
        }
        //---------------------------------------------------------------------
        private static bool HasPrivilege(COEIdentity ident, String privName, COERoleBOList roleList)
        {
            foreach (COERoleBO role in roleList)
                if (ident.HasAppPrivilege(role.COEIdentifier, privName))
                    return true;
            return false;
        }
        //---------------------------------------------------------------------
        public String GetPrivNamesForRole(String roleName)
        {
            Init();
            String s = string.Empty;
            COEPrivilegeBOList privList = COEPrivilegeBOList.GetList(roleName);
            if (privList != null)
            {
                foreach (COEPrivilegeBO priv in privList)
                {
                    if (String.IsNullOrEmpty(priv.PrivilegeName)) continue;
                    if (!priv.Enabled) continue;
                    if (!String.IsNullOrEmpty(s)) s += ",";
                    s += priv.PrivilegeName;
                }
            }
            return s;
        }
        //---------------------------------------------------------------------
        public bool ShowNoCBVPrivsMessage()
        {
            String msg = String.Format(
                "User '{0}' has not been assigned ChemBioViz privileges.\n\nProceed with restricted privileges?", m_ident.Name);
            bool bYes = MessageBox.Show(msg, "Privilege Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            return bYes;
        }
        //---------------------------------------------------------------------
        public void ShowRestrictionMessage(String which, bool bIsDelete)
        {
            // flag added for CSBR-115053: Error message says "Save" instead of delete
            String sRestricted = "proceed";
            if (which.Equals(CBVConstants.CAN_EDIT_FORMS))                  sRestricted = "edit forms";
            else if (which.Equals(CBVConstants.CAN_SAVE_PUBLIC_FORM))       sRestricted = bIsDelete? "delete public forms" : "save public forms";
            else if (which.Equals(CBVConstants.CAN_SAVE_DEFAULT_SETTINGS))  sRestricted = "save default settings";
            else if (which.Equals(CBVConstants.CAN_RENAME))                 sRestricted = "rename";

            String msg = String.Format("Permission to {0} is denied", sRestricted);
            MessageBox.Show(msg, "Privilege Restriction", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        //---------------------------------------------------------------------
        #endregion
    }
}