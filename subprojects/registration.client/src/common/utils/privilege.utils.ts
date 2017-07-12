export default {
  isUserHasPrivilege(privilege: string, userPrivileges: any[]): boolean {
    if (userPrivileges) {
      let privilageItem = userPrivileges.find(p => p.name === privilege);
      if (privilageItem !== undefined) {
        return true;
      }
    }
    return false;
  },

  hasDeleteRecordPrivilege(temporary: boolean, userPrivileges: any[]): boolean {
    let privilege = temporary ? 'DELETE_TEMP' : 'DELETE_REG';
    return this.isUserHasPrivilege(privilege, userPrivileges);
  },

  hasEditRecordPrivilege(temporary: boolean, userPrivileges: any[]): boolean {
    // if  user has EDIT_SCOPE_ALL, he will be able to edit records created by other users
    if (this.isUserHasPrivilege('EDIT_SCOPE_ALL', userPrivileges)) {
      return true;
    }
    // TODO: based on logged in user return EDIT privilege
    let privilege = temporary ? 'EDIT_COMPOUND_TEMP' : 'EDIT_COMPOUND_REG';
    return this.isUserHasPrivilege(privilege, userPrivileges);
    // TODO: need to check EDIT_SCOPE_SUPERVISOR  privilge 
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

  // Fragments and Fragment Types table privilges depends on SALT_TABLE or SOLVATES_TABLE privilges
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
  }
};
