import { IAppPrivilege } from './../../redux/store/session/session.types';
export class PrivilegeUtils {
  /**
   * Checks whether user has privilege in registration app
   * 
   * @private
   * @static
   * @param {string} privilege 
   * @param {IAppPrivilege[]} userPrivileges 
   * @returns {boolean} 
   * @memberof PrivilegeUtils
   */
  private static userHasRegAppPrivilege(privilege: string, userPrivileges: IAppPrivilege[]): boolean {
    let regAppPrivileges = userPrivileges.find(e => e.appName === 'REGISTRATION');
    if (regAppPrivileges && regAppPrivileges.privileges) {
      let privilegeItem = regAppPrivileges.privileges.find(p => p.name === privilege);
      if (privilegeItem !== undefined) {
        return true;
      }
    }
    return false;
  }

  /**
   * Checks whether user has privilege in Inv app
   * 
   * @private
   * @static
   * @param {string} privilege 
   * @param {IAppPrivilege[]} userPrivileges 
   * @returns {boolean} 
   * @memberof PrivilegeUtils
   */
  private static userHasInventoryAppPrivilege(privilege: string, userPrivileges: IAppPrivilege[]): boolean {
    let invAppPrivileges = userPrivileges.find(e => e.appName === 'INVENTORY');
    if (invAppPrivileges && invAppPrivileges.privileges) {
      let privilegeItem = invAppPrivileges.privileges.find(p => p.name === privilege);
      if (privilegeItem !== undefined) {
        return true;
      }
    }
    return false;
  }

  static hasRegAppPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    let regAppPrivileges = userPrivileges.find(e => e.appName === 'REGISTRATION');
    return regAppPrivileges && regAppPrivileges.privileges ? true : false;
  }

  /**
   * Checks whether user has create container privilge in Inventory application
   * 
   * @static
   * @param {IAppPrivilege[]} list of user privileges for the logged in user 
   * @returns {boolean} 
   * @memberof PrivilegeUtils
   */
  static hasCreateContainerPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    return this.userHasInventoryAppPrivilege('INV_CREATE_CONTAINER', userPrivileges);
  }

  /**
 * Checks whether user has Inventory Container table view privilege
 * 
 * @static
 * @param {IAppPrivilege[]} list of user privileges for the logged in user 
 * @returns {boolean} 
 * @memberof PrivilegeUtils
 */
  static hasBatchContainersViewPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    if (!this.userHasInventoryAppPrivilege('INV_BROWSE_ALL', userPrivileges)) {
      return false;
    }
    return true;
  }

  static hasBatchContainersRequestPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    if (!this.userHasInventoryAppPrivilege('INV_SAMPLE_REQUEST', userPrivileges)) {
      return false;
    }
    return true;
  }

  /**
   * Checks whether delete record privilege
   * If the privilege list contains 'DELETE_TEMP' or 'DELETE_REG', it returns true
   * @param {boolean} temporary whether temporary or permanent record
   * @param {IAppPrivilege[]} list of user privileges for the logged in user 
   * @returns {boolean} True if the delete privilege
   */
  static hasDeletePrivilege(temporary: boolean, userPrivileges: IAppPrivilege[]): boolean {
    let privilege = temporary ? 'DELETE_TEMP' : 'DELETE_REG';
    if (!this.userHasRegAppPrivilege(privilege, userPrivileges)) {
      return false;
    }

    privilege = temporary ? 'EDIT_COMPOUND_TEMP' : 'EDIT_COMPOUND_REG';
    return this.userHasRegAppPrivilege(privilege, userPrivileges);
  }

  static hasDeleteRecordPrivilege(temporary: boolean, isLoggedInUserOwner: boolean,
    isLoggedInUserSuperVisor: boolean, userPrivileges: IAppPrivilege[]): boolean {

    let privilege = temporary ? 'DELETE_TEMP' : 'DELETE_REG';
    if (!this.userHasRegAppPrivilege(privilege, userPrivileges)) {
      return false;
    }

    privilege = temporary ? 'EDIT_COMPOUND_TEMP' : 'EDIT_COMPOUND_REG';
    let hasBaseEditPrivilege = this.userHasRegAppPrivilege(privilege, userPrivileges);

    // base privilege EDIT_COMPOUND_TEMP or EDIT_COMPOUND_REG is required for editing a compound
    if (!hasBaseEditPrivilege) {
      return false;
    }

    // if logged in user is owner of the record, he can edit his record
    if (isLoggedInUserOwner) {
      return true;
    }

    // if the user has EDIT_SCOPE_ALL, he will be able to edit records created by other users
    if (this.userHasRegAppPrivilege('EDIT_SCOPE_ALL', userPrivileges)) {
      return true;
    }

    // if the logged in user is a supervisor of the registry record owner
    // and EDIT_SCOPE_SUPERVISOR privilege, supervisor can edit record
    if (isLoggedInUserSuperVisor) {
      return this.userHasRegAppPrivilege('EDIT_SCOPE_SUPERVISOR', userPrivileges);
    }
  }

  /**
   * Checks whether register record privilege
   * 
   * @param {boolean} newRecord whether new record (new record form display via submit new compound)
   * @param {boolean} isLoggedInUserOwner whether the logged in user is the owner of record
   * @param {boolean} isLoggedInUserSuperVisor whether the logged in user is super visor of record owner
   * @param {IAppPrivilege[]} list of user privileges for the logged in user 
   * @returns {boolean} True if register record privilege
   */
  static hasRegisterRecordPrivilege(newRecord: boolean, isLoggedInUserOwner: boolean,
    isLoggedInUserSuperVisor: boolean, userPrivileges: IAppPrivilege[]): boolean {

    // REGISTER_DIRECT privilege available, user can directly register new record
    if (newRecord) {
      return this.userHasRegAppPrivilege('REGISTER_DIRECT', userPrivileges);
    }

    // EDIT_COMPOUND_TEMP privilege is required to register an already submitted record
    if (!this.userHasRegAppPrivilege('EDIT_COMPOUND_TEMP', userPrivileges)) {
      return false;
    }

    // REGISTER_TEMP privilege also required to register an already submitted record
    let hasBaseRegisterPrivilege = this.userHasRegAppPrivilege('REGISTER_TEMP', userPrivileges);
    if (!hasBaseRegisterPrivilege) {
      return false;
    }

    // if logged in user is owner of the record, he can register his already submitted record
    if (isLoggedInUserOwner) {
      return true;
    }

    // if the user has EDIT_SCOPE_ALL, he will be able to register records created by other users
    if (this.userHasRegAppPrivilege('EDIT_SCOPE_ALL', userPrivileges)) {
      return true;
    }

    // if the logged in user is a supervisor of the registry record owner
    // and EDIT_SCOPE_SUPERVISOR privilege, supervisor can register record
    if (isLoggedInUserSuperVisor) {
      return this.userHasRegAppPrivilege('EDIT_SCOPE_SUPERVISOR', userPrivileges);
    }
  }

  static hasEditRecordPrivilege(temporary: boolean, isLoggedInUserOwner: boolean,
    isLoggedInUserSuperVisor: boolean, userPrivileges: IAppPrivilege[]): boolean {

    let privilege = temporary ? 'EDIT_COMPOUND_TEMP' : 'EDIT_COMPOUND_REG';
    let hasBaseEditPrivilege = this.userHasRegAppPrivilege(privilege, userPrivileges);

    // base privilege EDIT_COMPOUND_TEMP or EDIT_COMPOUND_REG is required for editing a compound
    if (!hasBaseEditPrivilege) {
      return false;
    }

    // if logged in user is owner of the record, he can edit his record
    if (isLoggedInUserOwner) {
      return true;
    }

    // if the user has EDIT_SCOPE_ALL, he will be able to edit records created by other users
    if (this.userHasRegAppPrivilege('EDIT_SCOPE_ALL', userPrivileges)) {
      return true;
    }

    // if the logged in user is a supervisor of the registry record owner
    // and EDIT_SCOPE_SUPERVISOR privilege, supervisor can edit record
    if (isLoggedInUserSuperVisor) {
      return this.userHasRegAppPrivilege('EDIT_SCOPE_SUPERVISOR', userPrivileges);
    }
  }

  static hasRegisterPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    // REGISTER_TEMP privilege is required to register an already submitted record
    return this.userHasRegAppPrivilege('REGISTER_TEMP', userPrivileges);
  }

  static hasAddBatchPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    // ADD_BATCH_PERM privilege is required to add a batch 
    return this.userHasRegAppPrivilege('ADD_BATCH_PERM', userPrivileges);
  }

  static hasDeleteBatchPrivilege(loggedInUserBatchOwner: boolean, isLoggedInUserBatchOwnerSuperVisor: boolean, userPrivileges: IAppPrivilege[]): boolean {
    // DELETE_BATCH_REG privilege is required to delete a batch     
    if (!this.userHasRegAppPrivilege('DELETE_BATCH_REG', userPrivileges)) {
      return false;
    }
    // if the user has EDIT_SCOPE_ALL, he will be able to edit records created by other users
    if (this.userHasRegAppPrivilege('EDIT_SCOPE_ALL', userPrivileges)) {
      return true;
    }

    if (isLoggedInUserBatchOwnerSuperVisor && this.userHasRegAppPrivilege('EDIT_SCOPE_SUPERVISOR', userPrivileges)) {
      return true;
    }

    if (loggedInUserBatchOwner) {
      return true;
    }

    return false;
  }

  static hasEditBatchPrivilege(loggedInUserBatchOwner: boolean, isLoggedInUserBatchOwnerSuperVisor: boolean, userPrivileges: IAppPrivilege[]): boolean {

    let hasBaseEditPrivilege = this.userHasRegAppPrivilege('EDIT_COMPOUND_REG', userPrivileges);
    // base privilege EDIT_COMPOUND_REG is required for editing a compound
    if (!hasBaseEditPrivilege) {
      return false;
    }

    // if the user has EDIT_SCOPE_ALL, he will be able to edit records created by other users
    if (this.userHasRegAppPrivilege('EDIT_SCOPE_ALL', userPrivileges)) {
      return true;
    }

    if (isLoggedInUserBatchOwnerSuperVisor && this.userHasRegAppPrivilege('EDIT_SCOPE_SUPERVISOR', userPrivileges)) {
      return true;
    }

    if (loggedInUserBatchOwner) {
      return true;
    }

    return false;
  }

  static hasProjectsTablePrivilege(action: string, userPrivileges: IAppPrivilege[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_PROJECTS_TABLE'
      : action === 'EDIT' ? 'EDIT_PROJECTS_TABLE'
        : action === 'DELETE' ? 'DELETE_PROJECTS_TABLE'
          : '';
    return this.userHasRegAppPrivilege(privilege, userPrivileges);
  }

  static hasApprovalPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    // privilege  SET_APPROVED_FLAG is required to approve records 
    return this.userHasRegAppPrivilege('SET_APPROVED_FLAG', userPrivileges);
  }

  static hasSearchTempPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    // privilege  SEARCH_TEMP is required to view temporary records
    return this.userHasRegAppPrivilege('SEARCH_TEMP', userPrivileges);
  }

  static hasCancelApprovalPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    //  privilege TOGGLE_APPROVED_FLAG is required to cancel the approved record 
    return this.userHasRegAppPrivilege('TOGGLE_APPROVED_FLAG', userPrivileges);
  }

  static hasNotebookTablePrivilege(action: string, userPrivileges: IAppPrivilege[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_NOTEBOOKS_TABLE'
      : action === 'EDIT' ? 'EDIT_NOTEBOOKS_TABLE'
        : action === 'DELETE' ? 'DELETE_NOTEBOOKS_TABLE'
          : '';
    return this.userHasRegAppPrivilege(privilege, userPrivileges);
  }

  static hasSequenceTablePrivilege(action: string, userPrivileges: IAppPrivilege[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_SEQUENCES_TABLE'
      : action === 'EDIT' ? 'EDIT_SEQUENCES_TABLE'
        : action === 'DELETE' ? 'DELETE_SEQUENCES_TABLE'
          : '';
    return this.userHasRegAppPrivilege(privilege, userPrivileges);
  }

  /**
   * Checks whether salt table privilege
   * Fragments and Fragment Types table privileges depends on SALT_TABLE or SOLVATES_TABLE privileges
   * @param {string} action 
   * @param {IAppPrivilege[]} list of user privileges for the logged in user 
   * @returns {boolean} 
   */
  static hasSaltTablePrivilege(action: string, userPrivileges: IAppPrivilege[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_SALT_TABLE'
      : action === 'EDIT' ? 'EDIT_SALT_TABLE'
        : action === 'DELETE' ? 'DELETE_SALT_TABLE'
          : '';

    if (this.userHasRegAppPrivilege(privilege, userPrivileges)) {
      return true;
    }

    privilege = action === 'ADD' ? 'ADD_SOLVATES_TABLE'
      : action === 'EDIT' ? 'EDIT_SOLVATES_TABLE'
        : action === 'DELETE' ? 'DELETE_SOLVATES_TABLE'
          : '';

    return this.userHasRegAppPrivilege(privilege, userPrivileges);
  }

  static hasSitesTablePrivilege(action: string, userPrivileges: IAppPrivilege[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_SITES_TABLE'
      : action === 'EDIT' ? 'EDIT_SITES_TABLE'
        : action === 'DELETE' ? 'DELETE_SITES_TABLE'
          : '';
    return this.userHasRegAppPrivilege(privilege, userPrivileges);
  }

  static hasIdentifierTablePrivilege(action: string, userPrivileges: IAppPrivilege[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_IDENTIFIER_TYPE_TABLE'
      : action === 'EDIT' ? 'EDIT_IDENTIFIER_TYPE_TABLE'
        : action === 'DELETE' ? 'DELETE_IDENTIFIER_TYPE_TABLE'
          : '';
    return this.userHasRegAppPrivilege(privilege, userPrivileges);
  }

  static hasPicklistTablePrivilege(action: string, userPrivileges: IAppPrivilege[]): boolean {
    let privilege = action === 'ADD' ? 'ADD_PICKLIST_TABLE'
      : action === 'EDIT' ? 'EDIT_PICKLIST_TABLE'
        : action === 'DELETE' ? 'DELETE_PICKLIST_TABLE'
          : '';
    return this.userHasRegAppPrivilege(privilege, userPrivileges);
  }

  /**
   * Checks submission template privilege
   * Note: Visibility of submission template button also depends on 
   * 'Enable Submission template' in System settings
   * @param {IAppPrivilege[]} list of user privileges for the logged in user 
   * @returns {boolean} True if register record privilege
   */
  static hasSubmissionTemplatePrivilege(userPrivileges: IAppPrivilege[]): boolean {
    return this.userHasRegAppPrivilege('LOAD_SAVE_RECORD', userPrivileges);
  }

  static hasManagePropertiesPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    return this.userHasRegAppPrivilege('MANAGE_PROPERTIES', userPrivileges);
  }

  static hasCustomizeFormsPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    return this.userHasRegAppPrivilege('CUSTOMIZE_FORMS', userPrivileges);
  }

  static hasManageAddinsPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    return this.userHasRegAppPrivilege('MANAGE_ADDINS', userPrivileges);
  }

  static hasEditFormXmlPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    return this.userHasRegAppPrivilege('EDIT_FORM_XML', userPrivileges);
  }

  static hasManageSystemSettingsPrivilege(userPrivileges: IAppPrivilege[]): boolean {
    return this.userHasRegAppPrivilege('MANAGE_SYSTEM_SETTINGS', userPrivileges);
  }

  static hasSystemBehaviourAccessPrivilege(userPrivileges: IAppPrivilege[]): boolean {

    if (this.userHasRegAppPrivilege('MANAGE_PROPERTIES', userPrivileges)) {
      return true;
    }
    if (this.userHasRegAppPrivilege('CUSTOMIZE_FORMS', userPrivileges)) {
      return true;
    }

    if (this.userHasRegAppPrivilege('MANAGE_ADDINS', userPrivileges)) {
      return true;
    }

    if (this.userHasRegAppPrivilege('EDIT_FORM_XML', userPrivileges)) {
      return true;
    }

    if (this.userHasRegAppPrivilege('MANAGE_SYSTEM_SETTINGS', userPrivileges)) {
      return true;
    }

    return false;
  }

  static hasCustomTablePrivilege(userPrivileges: IAppPrivilege[]): boolean {
    return this.userHasRegAppPrivilege('MANAGE_TABLES', userPrivileges);
  }
}
