import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgReduxRouter } from '@angular-redux/router';
import { ILookupData } from '../../redux';
import { PrivilegeUtils } from '../../common';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { select, NgRedux } from '@angular-redux/store';

@Component({
  selector: 'reg-settings-page-header',
  template: require('./settings-page-header.component.html'),
  styles: [require('./config.component.css')]
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
    return PrivilegeUtils.hasManagePropertiesPrivilege(this.lookups.userPrivileges);
  }

  private get customizeFormsMenuEnabled(): boolean {
    return PrivilegeUtils.hasCustomizeFormsPrivilege(this.lookups.userPrivileges);
  }

  private get manageAddinsMenuEnabled(): boolean {
    return PrivilegeUtils.hasManageAddinsPrivilege(this.lookups.userPrivileges);
  }

  private get editFormXmlMenuEnabled(): boolean {
    return PrivilegeUtils.hasEditFormXmlPrivilege(this.lookups.userPrivileges);
  }

  private get systemSettingsMenuEnabled(): boolean {
    return PrivilegeUtils.hasManageSystemSettingsPrivilege(this.lookups.userPrivileges);
  }

  private get systemBehaviorButtonEnabled(): boolean {
    return PrivilegeUtils.hasSystemBehaviourAccessPrivilege(this.lookups.userPrivileges);
  }

  private get customTableButtonEnabled(): boolean {
    return PrivilegeUtils.hasCustomTablePrivilege(this.lookups.userPrivileges);
  }
};
