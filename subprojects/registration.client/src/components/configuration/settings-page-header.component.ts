import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgReduxRouter } from '@angular-redux/router';
import { ILookupData } from '../../store';
import privileges from '../../common/utils/privilege.utils';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { select, NgRedux } from '@angular-redux/store';

@Component({
  selector: 'reg-settings-page-header',
  template: require('./settings-page-header.component.html')
})
export class RegSettingsPageHeader {
  @Input() testid: string;
  @Input() id: string;
  @Input() isMenu: boolean = true;
  @select(s => s.session.lookups) lookups$: Observable<ILookupData>;

  private lookups: ILookupData;
  private lookupsSubscription: Subscription;

  ngOnInit() {
    if (!this.lookupsSubscription) {
      this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.retrieveContents(d); } });
    }
  }

  retrieveContents(lookups: ILookupData) {
    this.lookups = lookups;
  }

  ngOnDestroy() {
    if (this.lookupsSubscription) {
      this.lookupsSubscription.unsubscribe();
    }
  }

  private get manageDataPropertiesMenuEnabled(): boolean {
    return privileges.hasManagePropertiesPrivilege(this.lookups.userPrivileges);
  }

  private get customizeFormsMenuEnabled(): boolean {
    return privileges.hasCustomizeFormsPrivilege(this.lookups.userPrivileges);
  }

  private get manageAddinsMenuEnabled(): boolean {
    return privileges.hasManageAddinsPrivilege(this.lookups.userPrivileges);
  }

  private get editFormXmlMenuEnabled(): boolean {
    return privileges.hasEditFormXmlPrivilege(this.lookups.userPrivileges);
  }

  private get systemSettingsMenuEnabled(): boolean {
    return privileges.hasManageSystemSettingsPrivilege(this.lookups.userPrivileges);
  }
};
