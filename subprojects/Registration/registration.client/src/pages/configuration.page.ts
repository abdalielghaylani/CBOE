import { PrivilegeUtils } from '../common/utils/privilege.utils';
import { select } from '@angular-redux/store';
import { Observable ,  Subscription } from 'rxjs';
import { ILookupData } from './../redux/store/session/session.types';
import { Component, Inject, ApplicationRef } from '@angular/core';
import { RegContainer } from '../components';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'configuration-page',
  template: `
    <reg-container></reg-container>
  `
})
export class ConfigurationPage {
  private lookups: ILookupData;
  private lookupsSubscription: Subscription;
  @select(s => s.session.lookups) lookups$: Observable<ILookupData>;

  constructor(private router: Router) {
  }

  ngOnInit() {
    this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.retrieveLookups(d); } });
  }

  ngOnDestroy() {
    if (this.lookupsSubscription) {
      this.lookupsSubscription.unsubscribe();
    }
  }

  // lookups data
  retrieveLookups(lookups: ILookupData) {
    this.lookups = lookups;
    // get default configuration route based on the user privileges
    let defaultRoute = this.getDefaultConfigurationRoute();
    this.router.navigate([defaultRoute]);
  }

  private getDefaultConfigurationRoute(): string {
    if (PrivilegeUtils.hasCustomTablePrivilege(this.lookups.userPrivileges)) {
      return 'configuration/VW_PROJECT';
    } else {
      if (PrivilegeUtils.hasManagePropertiesPrivilege(this.lookups.userPrivileges)) {
        return 'configuration/properties';
      } else if (PrivilegeUtils.hasCustomizeFormsPrivilege(this.lookups.userPrivileges)) {
        return 'configuration/forms';
      } else if (PrivilegeUtils.hasManageAddinsPrivilege(this.lookups.userPrivileges)) {
        return 'configuration/addins';
      } else if (PrivilegeUtils.hasEditFormXmlPrivilege(this.lookups.userPrivileges)) {
        return 'configuration/xml-forms';
      } else if (PrivilegeUtils.hasManageSystemSettingsPrivilege(this.lookups.userPrivileges)) {
        return 'configuration/settings';
      }
    }

    return '';
  }
}
