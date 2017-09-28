export class PrivilegeUtils {
  private static userHasPrivilege(privilege: string, userPrivileges: any[]): boolean {
    if (userPrivileges) {
      let privilegeItem = userPrivileges.find(p => p.name === privilege);
      if (privilegeItem !== undefined) {
        return true;
      }
    }
    return false;
  }

  /**
   * Checks whether delete record privilege
   * If the privilege list contains 'DELETE_TEMP' or 'DELETE_REG', it returns true
   * @param {boolean} temporary whether temporary or permanent record
   * @param {any[]} userPrivileges list of user privileges for the logged in user
   * @returns {boolean} True if the delete privilege
   */
  static hasDeletePrivilege(temporary: boolean, userPrivileges: any[]): boolean {
    let privilege = temporary ? 'DELETE_TEMP' : 'DELETE_REG';
    if (!this.userHasPrivilege(privilege, userPrivileges)) {
      return false;
    }

    privilege = temporary ? 'EDIT_COMPOUND_TEMP' : 'EDIT_COMPOUND_REG';
    return this.userHasPrivilege(privilege, userPrivileges);
  }

  static hasDeleteRecordPrivilege(temporary: boolean, isLoggedInUserOwner: boolean,
    isLoggedInUserSuperVisor: boolean, userPrivileges: any[]): boolean {

    let privilege = temporary ? 'DELETE_TEMP' : 'DELETE_REG';
    if (!this.userHasPrivilege(privilege, userPrivileges)) {
      return false;
    }

    privilege = temporary ? 'EDIT_COMPOUND_TEMP' : 'EDIT_COMPOUND_REG';
    let hasBaseEditPrivilege = this.userHasPrivilege(privilege, userPrivileges);

    // base privilege EDIT_COMPOUND_TEMP or EDIT_COMPOUND_REG is required for editing a compound
    if (!hasBaseEditPrivilege) {
      return false;
    }

    // if logged in user is owner of the record, he can edit his record
    if (isLoggedInUserOwner) {
      return true;
    }

    // if the user has EDIT_SCOPE_ALL, he will be able to edit records created by other users
    if (this.userHasPrivilege('EDIT_SCOPE_ALL', userPrivileges)) {
      return true;
    }

    // if the logged in user is a supervisor of the registry record owner
    // and EDIT_SCOPE_SUPERVISOR privilege, supervisor can edit record
    if (isLoggedInUserSuperVisor) {
      return this.userHasPrivilege('EDIT_SCOPE_SUPERVISOR', userPrivileges);
    }
  }

  /**
   * Checks whether register record privilege
   * 
   * @param {boolean} newRecord whether new record (new record form display via submit new compound)
   * @param {boolean} isLoggedInUserOwner whether the logged in user is the owner of record
   * @param {boolean} isLoggedInUserSuperVisor whether the logged in user is super visor of record owner
   * @param {any[]} userPrivileges list of user privileges for the logged in user
   * @returns {boolean} True if register record privilege
   */
  static hasRegisterRecordPrivilege(newRecord: boolean, isLoggedInUserOwner: boolean,
    isLoggedInUserSuperVisor: boolean, userPrivileges: any[]): boolean {

    // REGISTER_DIRECT privilege available, user can directly register new record
    if (newRecord) {
      return this.userHasPrivilege('REGISTER_DIRECT', userPrivileges);
    }

    // EDIT_COMPOUND_TEMP privilege is required to register an already submitted record
    if (!this.userHasPrivilege('EDIT_COMPOUND_TEMP', userPrivileges)) {
      return false;
    }

    // REGISTER_TEMP privilege also required to register an already submitted record
    let hasBaseRegisterPrivilege = this.userHasPrivilege('REGISTER_TEMP', userPrivileges);
    if (!hasBaseRegisterPrivilege) {
      return false;
    }

    // if logged in user is owner of the record, he can register his already submitted record
    if (isLoggedInUserOwner) {
      return true;
    }

    // if the user has EDIT_SCOPE_ALL, he will be able to register records created by other users
    if (this.userHasPrivilege('EDIT_SCOPE_ALL', userPrivileges)) {
      return true;
    }

    // if the logged in user is a supervisor of the registry record owner
    // and EDIT_SCOPE_SUPERVISOR privilege, supervisor can register record
    if (isLoggedInUserSuperVisor) {
      return this.userHasPrivilege('EDIT_SCOPE_SUPERVISOR', userPrivileges);
    }
  }

  static hasEditRecordPrivilege(temporary: boolean, isLoggedInUserOwner: boolean,
    isLoggedInUserSuperVisor: boolean, userPrivileges: any[]): boolean {

    let privilege = temporary ? 'EDIT_COMPOUND_TEMP' : 'EDIT_COMPOUND_REG';
    let hasBaseEditPrivilege = this.userHasPrivilege(privilege, userPrivileges);

    // base privilege EDIT_COMPOUND_TEMP or EDIT_COMPOUND_REG is required for editing a compound
    if (!hasBaseEditPrivilege) {
      return false;
    }

    // if logged in user is owner of the record, he can edit his record
    if (isLoggedInUserOwner) {
      return true;
    }

    // if the user has EDIT_SCOPE_ALL, he will be able to edit records created by other users
    if (this.userHasPrivilege('EDIT_SCOPE_ALL', userPrivileges)) {
      return true;
    }

    // if the logged in user is a supervisor of the registry record owner
    // and EDIT_SCOPE_SUPERVISOR privilege, supervisor can edit record
    if (isLoggedInUserSuperVisor) {
      return this.userHasPrivilege('EDIT_SCOPE_SUPERVISOR', userPrivileges);
    }
  }

  static hasRegisterPrivilege(userPrivileges: any[]): boolean {
    // REGISTER_TEMP privilege is required to register an already submitted record
    return this.userHasPrivilege('REGISTER_TEMP', userPrivileges);
  }

  static hasProjectsTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_PROJECTS_TABLE'
      : action === 'EDIT' ? 'EDIT_PROJECTS_TABLE'
        : action === 'DELETE' ? 'DELETE_PROJECTS_TABLE'
          : '';
    return this.userHasPrivilege(privilege, userPrivileges);
  }

  static hasApprovalPrivilege(userPrivileges: any[]): boolean {
    // privilege  SET_APPROVED_FLAG is required to approve records 
    return this.userHasPrivilege('SET_APPROVED_FLAG', userPrivileges);
  }

  static hasSearchTempPrivilege(userPrivileges: any[]): boolean {
    // privilege  SEARCH_TEMP is required to view temporary records
    return this.userHasPrivilege('SEARCH_TEMP', userPrivileges);
  }

  static hasCancelApprovalPrivilege(userPrivileges: any[]): boolean {
    //  privilege TOGGLE_APPROVED_FLAG is required to cancel the approved record 
    return this.userHasPrivilege('TOGGLE_APPROVED_FLAG', userPrivileges);
  }

  static hasNotebookTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_NOTEBOOKS_TABLE'
      : action === 'EDIT' ? 'EDIT_NOTEBOOKS_TABLE'
        : action === 'DELETE' ? 'DELETE_NOTEBOOKS_TABLE'
          : '';
    return this.userHasPrivilege(privilege, userPrivileges);
  }

  static hasSequenceTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_SEQUENCES_TABLE'
      : action === 'EDIT' ? 'EDIT_SEQUENCES_TABLE'
        : action === 'DELETE' ? 'DELETE_SEQUENCES_TABLE'
          : '';
    return this.userHasPrivilege(privilege, userPrivileges);
  }

  /**
   * Checks whether salt table privilege
   * Fragments and Fragment Types table privileges depends on SALT_TABLE or SOLVATES_TABLE privileges
   * @param {string} action 
   * @param {any[]} userPrivileges 
   * @returns {boolean} 
   */
  static hasSaltTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_SALT_TABLE'
      : action === 'EDIT' ? 'EDIT_SALT_TABLE'
        : action === 'DELETE' ? 'DELETE_SALT_TABLE'
          : '';

    if (this.userHasPrivilege(privilege, userPrivileges)) {
      return true;
    }

    privilege = action === 'ADD' ? 'ADD_SOLVATES_TABLE'
      : action === 'EDIT' ? 'EDIT_SOLVATES_TABLE'
        : action === 'DELETE' ? 'DELETE_SOLVATES_TABLE'
          : '';

    return this.userHasPrivilege(privilege, userPrivileges);
  }

  static hasSitesTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_SITES_TABLE'
      : action === 'EDIT' ? 'EDIT_SITES_TABLE'
        : action === 'DELETE' ? 'DELETE_SITES_TABLE'
          : '';
    return this.userHasPrivilege(privilege, userPrivileges);
  }

  static hasIdentifierTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_IDENTIFIER_TYPE_TABLE'
      : action === 'EDIT' ? 'EDIT_IDENTIFIER_TYPE_TABLE'
        : action === 'DELETE' ? 'DELETE_IDENTIFIER_TYPE_TABLE'
          : '';
    return this.userHasPrivilege(privilege, userPrivileges);
  }

  static hasPicklistTablePrivilege(action: string, userPrivileges: any[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_PICKLIST_TABLE'
      : action === 'EDIT' ? 'EDIT_PICKLIST_TABLE'
        : action === 'DELETE' ? 'DELETE_PICKLIST_TABLE'
          : '';
    return this.userHasPrivilege(privilege, userPrivileges);
  }

  /**
   * Checks submission template privilege
   * Note: Visibility of submission template button also depends on 
   * 'Enable Submission template' in System settings
   * @param {any[]} userPrivileges list of user privileges for the logged in user
   * @returns {boolean} True if register record privilege
   */
  static hasSubmissionTemplatePrivilege(userPrivileges: any[]): boolean {
    return this.userHasPrivilege('LOAD_SAVE_RECORD', userPrivileges);
  }

  static hasManagePropertiesPrivilege(userPrivileges: any[]): boolean {
    return this.userHasPrivilege('MANAGE_PROPERTIES', userPrivileges);
  }

  static hasCustomizeFormsPrivilege(userPrivileges: any[]): boolean {
    return this.userHasPrivilege('CUSTOMIZE_FORMS', userPrivileges);
  }

  static hasManageAddinsPrivilege(userPrivileges: any[]): boolean {
    return this.userHasPrivilege('MANAGE_ADDINS', userPrivileges);
  }

  static hasEditFormXmlPrivilege(userPrivileges: any[]): boolean {
    return this.userHasPrivilege('EDIT_FORM_XML', userPrivileges);
  }

  static hasManageSystemSettingsPrivilege(userPrivileges: any[]): boolean {
    return this.userHasPrivilege('MANAGE_SYSTEM_SETTINGS', userPrivileges);
  }

  static hasSystemBehaviourAccessPrivilege(userPrivileges: any[]): boolean {

    if (this.userHasPrivilege('MANAGE_PROPERTIES', userPrivileges)) {
      return true;
    }
    if (this.userHasPrivilege('CUSTOMIZE_FORMS', userPrivileges)) {
      return true;
    }

    if (this.userHasPrivilege('MANAGE_ADDINS', userPrivileges)) {
      return true;
    }

    if (this.userHasPrivilege('EDIT_FORM_XML', userPrivileges)) {
      return true;
    }

    if (this.userHasPrivilege('MANAGE_SYSTEM_SETTINGS', userPrivileges)) {
      return true;
    }

    return false;
  }

  static hasCustomTablePrivilege(userPrivileges: any[]): boolean {
    return this.userHasPrivilege('MANAGE_TABLES', userPrivileges);
  }
}
