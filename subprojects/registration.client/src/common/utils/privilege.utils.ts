export default {
  isUserHasPrivilege(privilege: string, userPrivileges: any[]): boolean {
    if (userPrivileges) {
      let privilegeItem = userPrivileges.find(p => p.name === privilege);
      if (privilegeItem !== undefined) {
        return true;
      }
    }
    return false;
  },

  /**
   * Checks whether delete record privilege
   * If the privilege list contains 'DELETE_TEMP' or 'DELETE_REG', it returns true
   * @param {boolean} temporary whether temporary or permanent record
   * @param {any[]} userPrivileges list of user privileges for the logged in user
   * @returns {boolean} True if the delete privilege
   */
  hasDeleteRecordPrivilege(temporary: boolean, userPrivileges: any[]): boolean {
    let privilege = temporary ? 'DELETE_TEMP' : 'DELETE_REG';
    return this.isUserHasPrivilege(privilege, userPrivileges);
  },

  /**
   * Checks whether register record privilege
   * 
   * @param {boolean} newRecord whether new record (new record form display via submit new compound)
   * @param {boolean} isLoggedInUserOwner whether the logged in user is the owner of record
   * @param {boolean} isLoggedInUserSuperVisor whether the logged in user is super visor of record owner
   * @param {any[]} userPrivileges list of user privileges for the logged in user
   * @returns {boolean} True if register record privilege
   */
  hasRegisterRecordPrivilege(newRecord: boolean, isLoggedInUserOwner: boolean,
    isLoggedInUserSuperVisor: boolean, userPrivileges: any[]): boolean {

    // REGISTER_DIRECT privilege available, user can directly register new record
    if (newRecord && this.isUserHasPrivilege('REGISTER_DIRECT', userPrivileges)) {
      return true;
    }

    // REGISTER_TEMP privilege is required to register an already submitted record
    let hasBaseRegisterPrivilege = this.isUserHasPrivilege('REGISTER_TEMP', userPrivileges);
    if (!hasBaseRegisterPrivilege) {
      return false;
    }

    // if logged in user is owner of the record, he can register his record
    if (isLoggedInUserOwner) {
      return true;
    }

    // if the user has EDIT_SCOPE_ALL, he will be able to register records created by other users
    if (this.isUserHasPrivilege('EDIT_SCOPE_ALL', userPrivileges)) {
      return true;
    }

    // if the logged in user is a supervisor of the registry record owner
    // and EDIT_SCOPE_SUPERVISOR privilege, supervisor can register record
    if (isLoggedInUserSuperVisor) {
      return this.isUserHasPrivilege('EDIT_SCOPE_SUPERVISOR', userPrivileges);
    }
  },

  hasEditRecordPrivilege(temporary: boolean, isLoggedInUserOwner: boolean,
    isLoggedInUserSuperVisor: boolean, userPrivileges: any[]): boolean {

    let privilege = temporary ? 'EDIT_COMPOUND_TEMP' : 'EDIT_COMPOUND_REG';
    let hasBaseEditPrivilege = this.isUserHasPrivilege(privilege, userPrivileges);

    // base privilege EDIT_COMPOUND_TEMP or EDIT_COMPOUND_REG is required for editing a compound
    if (!hasBaseEditPrivilege) {
      return false;
    }

    // if logged in user is owner of the record, he can edit his record
    if (isLoggedInUserOwner) {
      return true;
    }

    // if the user has EDIT_SCOPE_ALL, he will be able to edit records created by other users
    if (this.isUserHasPrivilege('EDIT_SCOPE_ALL', userPrivileges)) {
      return true;
    }

    // if the logged in user is a supervisor of the registry record owner
    // and EDIT_SCOPE_SUPERVISOR privilege, supervisor can edit record
    if (isLoggedInUserSuperVisor) {
      return this.isUserHasPrivilege('EDIT_SCOPE_SUPERVISOR', userPrivileges);
    }
  },

  hasProjectsTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_PROJECTS_TABLE'
      : action === 'EDIT' ? 'EDIT_PROJECTS_TABLE'
        : action === 'DELETE' ? 'DELETE_PROJECTS_TABLE'
          : '';
    return this.isUserHasPrivilege(privilege, userPrivileges);
  },

  hasNotebookTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_NOTEBOOKS_TABLE'
      : action === 'EDIT' ? 'EDIT_NOTEBOOKS_TABLE'
        : action === 'DELETE' ? 'DELETE_NOTEBOOKS_TABLE'
          : '';
    return this.isUserHasPrivilege(privilege, userPrivileges);
  },

  hasSequenceTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_SEQUENCES_TABLE'
      : action === 'EDIT' ? 'EDIT_SEQUENCES_TABLE'
        : action === 'DELETE' ? 'DELETE_SEQUENCES_TABLE'
          : '';
    return this.isUserHasPrivilege(privilege, userPrivileges);
  },

  /**
   * Checks whether salt table privilege
   * Fragments and Fragment Types table privileges depends on SALT_TABLE or SOLVATES_TABLE privileges
   * @param {string} action 
   * @param {any[]} userPrivileges 
   * @returns {boolean} 
   */
  hasSaltTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_SALT_TABLE'
      : action === 'EDIT' ? 'EDIT_SALT_TABLE'
        : action === 'DELETE' ? 'DELETE_SALT_TABLE'
          : '';

    if (this.isUserHasPrivilege(privilege, userPrivileges)) {
      return true;
    }

    privilege = action === 'ADD' ? 'ADD_SOLVATES_TABLE'
      : action === 'EDIT' ? 'EDIT_SOLVATES_TABLE'
        : action === 'DELETE' ? 'DELETE_SOLVATES_TABLE'
          : '';

    return this.isUserHasPrivilege(privilege, userPrivileges);
  },

  hasSitesTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_SITES_TABLE'
      : action === 'EDIT' ? 'EDIT_SITES_TABLE'
        : action === 'DELETE' ? 'DELETE_SITES_TABLE'
          : '';
    return this.isUserHasPrivilege(privilege, userPrivileges);
  },

  hasIdentifierTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_IDENTIFIER_TYPE_TABLE'
      : action === 'EDIT' ? 'EDIT_IDENTIFIER_TYPE_TABLE'
        : action === 'DELETE' ? 'DELETE_IDENTIFIER_TYPE_TABLE'
          : '';
    return this.isUserHasPrivilege(privilege, userPrivileges);
  },

  hasPicklistTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_PICKLIST_TABLE'
      : action === 'EDIT' ? 'EDIT_PICKLIST_TABLE'
        : action === 'DELETE' ? 'DELETE_PICKLIST_TABLE'
          : '';
    return this.isUserHasPrivilege(privilege, userPrivileges);
  },

  /**
   * Checks submission template privilege
   * Note: Visibility of submission template button also depends on 
   * 'Enable Submission template' in System settings
   * @param {any[]} userPrivileges list of user privileges for the logged in user
   * @returns {boolean} True if register record privilege
   */
  hasSubmissionTemplatePrivilege(userPrivileges: any[]): boolean {
    return this.isUserHasPrivilege('LOAD_SAVE_RECORD', userPrivileges);
  },

  hasManagePropertiesPrivilege(userPrivileges: any[]): boolean {
    return this.isUserHasPrivilege('MANAGE_PROPERTIES', userPrivileges);
  },

  hasCustomizeFormsPrivilege(userPrivileges: any[]): boolean {
    return this.isUserHasPrivilege('CUSTOMIZE_FORMS', userPrivileges);
  },

  hasManageAddinsPrivilege(userPrivileges: any[]): boolean {
    return this.isUserHasPrivilege('MANAGE_ADDINS', userPrivileges);
  },

  hasEditFormXmlPrivilege(userPrivileges: any[]): boolean {
    return this.isUserHasPrivilege('EDIT_FORM_XML', userPrivileges);
  },

  hasManageSystemSettingsPrivilege(userPrivileges: any[]): boolean {
    return this.isUserHasPrivilege('MANAGE_SYSTEM_SETTINGS', userPrivileges);
  }
};
