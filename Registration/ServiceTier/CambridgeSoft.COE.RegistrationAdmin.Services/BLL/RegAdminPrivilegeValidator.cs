using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COESecurityService;

namespace CambridgeSoft.COE.RegistrationAdmin.Services
{
    /// <summary>
    /// The purpose of this class is to tell the caller whether a user has a privilege to access a feature in the regAdmin tool
    /// </summary>
    public class RegAdminPrivilegeValidator
    {
        /// <summary>
        /// public constructor
        /// </summary>
        public RegAdminPrivilegeValidator()
        {

        }

        /// <summary>
        /// method for determining if user has rights to perform a task
        /// </summary>
        /// <param name="taskName">enum value for task</param>
        /// <returns>true or false</returns>
        public static bool HasPrivilege(RegAdminTasks privilegeName)
        {
            //Get the User object
            COEPrincipal principal = (COEPrincipal)Csla.ApplicationContext.User;
            COEIdentity myIdentity = (COEIdentity)principal.Identity;
            bool hasPriv = myIdentity.HasAppPrivilege("REGISTRATION", privilegeName.ToString());
            return hasPriv;
        }

        /// <summary>
        /// Enum for types of tasks that have associated user privileges
        /// </summary>
        public enum RegAdminTasks
        {
            IMPORT_CONFIG,
            MANAGE_TABLES,
            MANAGE_PROPERTIES,
            CUSTOMIZE_FORMS,
            MANAGE_ADDINS,
            EDIT_FORM_XML,
            MANAGE_SYSTEM_SETTINGS
        }


    
    }
}
