import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  OnInit, OnDestroy, OnChanges, AfterViewInit,
  ElementRef, ChangeDetectorRef, ViewEncapsulation,
  ViewChild, ViewChildren, QueryList
} from '@angular/core';
import { ActivatedRoute, UrlSegment, Params, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { select, NgRedux } from '@angular-redux/store';
import * as X2JS from 'x2js';
import { RecordDetailActions, IAppState, IRecordDetail, ILookupData } from '../../redux';
import * as registryUtils from './registry.utils';
import { IShareableObject, CShareableObject, IFormGroup, prepareFormGroupData, notify } from '../../common';
import { IResponseData, ITemplateData, CTemplateData } from './registry.types';
import { DxFormComponent } from 'devextreme-angular';
import DxForm from 'devextreme/ui/form';
import { IRegistryRecord, CRegistryRecord, CViewGroup, RegRecordDetailBase } from './base';
import { basePath, apiUrlPrefix } from '../../configuration';
import { FormGroupType, IFormContainer, getFormGroupData, notifyError, notifyException, notifySuccess } from '../../common';
import { HttpService } from '../../services';
import { RegTemplates } from './templates.component';
import { RegistryStatus } from './registry.types';
import { CFragment } from '../common';
import { PrivilegeUtils } from '../../common';
import { CSystemSettings } from '../../redux';

@Component({
  selector: 'reg-record-detail',
  template: require('./record-detail.component.html'),
  styles: [require('./records.css')],
  host: { '(document:click)': 'onDocumentClick($event)' },
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecordDetail implements OnInit, OnDestroy, OnChanges {
  @ViewChild(RegTemplates) regTemplates: RegTemplates;
  @ViewChild(RegRecordDetailBase) recordDetailView: RegRecordDetailBase;
  @Input() temporary: boolean;
  @Input() template: boolean;
  @Input() id: number;
  @select(s => s.registry.currentRecord) recordDetail$: Observable<IRecordDetail>;
  @select(s => s.registry.duplicateRecords) duplicateRecord$: Observable<any[]>;
  private displayMode: string;
  private title: string;
  private parentHeight: string;
  private approvalsEnabled: boolean = false;
  private editButtonEnabled: boolean = false;
  private saveButtonEnabled: boolean = false;
  private cancelButtonEnabled: boolean = false;
  private registerButtonEnabled: boolean = false;
  private approveButtonEnabled: boolean = false;
  private cancelApprovalButtonEnabled: boolean = false;
  private deleteButtonEnabled: boolean = false;
  private clearButtonEnabled: boolean = false;
  private submissionTemplatesEnabled: boolean = false;
  private routeSubscription: Subscription;
  private duplicateSubscription: Subscription;
  private currentIndex: number = 0;
  private saveTemplateForm: DxForm;
  private saveTemplatePopupVisible: boolean = false;
  private newButtonEnabled: boolean = false;
  private backButtonEnabled: boolean = false;

  private saveTemplateItems = [{
    dataField: 'name',
    label: { text: 'Template Name' },
    dataType: 'string',
    editorType: 'dxTextBox',
    validationRules: [{ type: 'required', message: 'Name is required' }]
  }, {
    dataField: 'description',
    label: { text: 'Template Description' },
    dataType: 'string',
    editorType: 'dxTextArea'
  }, {
    dataField: 'isPublic',
    label: { text: 'Public Template' },
    dataType: 'boolean',
    editorType: 'dxCheckBox'
  }];
  private saveTemplateData: IShareableObject = new CShareableObject('', '', false);
  private isLoggedInUserOwner: boolean = false;
  private isLoggedInUserSuperVisor: boolean = false;

  constructor(
    public ngRedux: NgRedux<IAppState>,
    private elementRef: ElementRef,
    private router: Router,
    private http: HttpService,
    private actions: RecordDetailActions,
    private changeDetector: ChangeDetectorRef,
    private activatedRoute: ActivatedRoute) {
  }

  ngOnInit() {
    let state = this.ngRedux.getState();
    if (this.id >= 0 && (state.registry.records.data.rows.length === 0 && state.registry.tempRecords.data.rows.length === 0)) {
      this.router.navigate(['/']);
      return;
    }
    this.parentHeight = this.getParentHeight();
    this.routeSubscription = this.activatedRoute.url.subscribe((segments: UrlSegment[]) => this.initialize(segments));
  }

  ngOnChanges() {
    this.update(false);
  }

  private update(forceUpdate: boolean = true) {
    let lookups = this.ngRedux.getState().session.lookups;
    if (!lookups || !lookups.userPrivileges || !this.recordDetailView || this.displayMode == null) {
      return;
    }
    let editMode = this.displayMode !== 'view';
    let userPrivileges = lookups.userPrivileges;
    let ss = new CSystemSettings(this.getLookup('systemSettings'));
    let statusId = this.statusId;
    let canEdit = this.isNewRecord ||
      PrivilegeUtils.hasEditRecordPrivilege(this.temporary, this.isLoggedInUserOwner, this.isLoggedInUserSuperVisor, userPrivileges);

    this.approvalsEnabled = (this.isNewRecord || this.temporary)
      && ss.isApprovalsEnabled
      && PrivilegeUtils.hasApprovalPrivilege(userPrivileges);

    this.cancelApprovalButtonEnabled = this.approvalsEnabled
      && !editMode
      && !!statusId
      && this.temporary
      && statusId === RegistryStatus.Approved
      && PrivilegeUtils.hasCancelApprovalPrivilege(userPrivileges);

    this.editButtonEnabled = !this.isNewRecord && !this.cancelApprovalButtonEnabled && !editMode && canEdit;
    this.saveButtonEnabled = (this.isNewRecord && !this.cancelApprovalButtonEnabled) || editMode;
    this.cancelButtonEnabled = editMode && !this.isNewRecord;
    let canRegister = PrivilegeUtils.hasRegisterRecordPrivilege(this.isNewRecord, this.isLoggedInUserOwner, this.isLoggedInUserSuperVisor, userPrivileges);
    this.registerButtonEnabled = canRegister && (this.isNewRecord || (this.temporary && !editMode))
      && (!this.approvalsEnabled || this.cancelApprovalButtonEnabled);
    this.approveButtonEnabled = !editMode && !!statusId && this.temporary && this.approvalsEnabled && statusId !== RegistryStatus.Approved;

    this.deleteButtonEnabled = !this.isNewRecord
      && PrivilegeUtils.hasDeleteRecordPrivilege(this.temporary, this.isLoggedInUserOwner, this.isLoggedInUserSuperVisor, userPrivileges)
      && this.editButtonEnabled;

    let canRedirectToTempListView = PrivilegeUtils.hasSearchTempPrivilege(this.ngRedux.getState().session.lookups.userPrivileges);
    this.clearButtonEnabled = this.isNewRecord;
    this.newButtonEnabled = !canRedirectToTempListView && !editMode;
    this.submissionTemplatesEnabled = this.isNewRecord
      && PrivilegeUtils.hasSubmissionTemplatePrivilege(userPrivileges) && ss.isSubmissionTemplateEnabled;
    let state = this.ngRedux.getState();
    let hitListId = this.temporary ? state.registry.tempRecords.data.hitlistId : state.registry.records.data.hitlistId;
    this.backButtonEnabled = hitListId > 0;
    if (forceUpdate) {
      this.changeDetector.markForCheck();
    }
  }

  initialize(segments: UrlSegment[]) {
    let newIndex = segments.findIndex(s => s.path === 'new');
    if (newIndex >= 0 && newIndex < segments.length - 1) {
      this.id = +segments[segments.length - 1].path;
    }
    if (!this.duplicateSubscription) {
      this.duplicateSubscription = this.duplicateRecord$.subscribe((value) => this.duplicateData(value));
    }
  }

  duplicateData(e) {
    if (e) {
      this.currentIndex = 2;
      this.changeDetector.markForCheck();
    }
  }

  ngOnDestroy() {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
    if (this.duplicateSubscription) {
      this.duplicateSubscription.unsubscribe();
    }
  }

  private getParentHeight() {
    return ((this.elementRef.nativeElement.parentElement.clientHeight) - 100).toString();
  }

  private onResize(event: any) {
    this.parentHeight = this.getParentHeight();
  }

  private onDocumentClick(event: any) {
    if (event.srcElement.title === 'Full Screen') {
      let fullScreenMode = event.srcElement.className === 'fa fa-compress fa-stack-1x white';
      this.parentHeight = (this.elementRef.nativeElement.parentElement.clientHeight - (fullScreenMode ? 10 : 190)).toString();
    }
  }

  getElementValue(e: Element, path: string) {
    return registryUtils.getElementValue(e, path);
  }

  save() {
    if (this.recordDetailView.save()) {
      this.displayMode = 'view';
      if (this.isNewRecord) {
        if (this.recordDetailView.displayMode !== 'view') {
          return;
        }
        this.saveButtonEnabled = false;
        this.clearButtonEnabled = false;
        this.newButtonEnabled = true;
      }
      this.update();
    }
  }

  cancel() {
    this.recordDetailView.prepareRegistryRecord();
    this.displayMode = 'view';
    this.update();
  }

  cancelDuplicateResolution(e) {
    this.actions.clearDuplicateRecord();
    this.currentIndex = 0;
  }

  edit() {
    this.displayMode = 'edit';
    this.update();
  }

  newRecord() {
    this.router.navigate([`records/new?${new Date().getTime()}`]);
  }

  back() {
    let state = this.ngRedux.getState();
    let hitListId = this.temporary ? state.registry.tempRecords.data.hitlistId : state.registry.records.data.hitlistId;
    let path = this.temporary ? 'records/temp' : 'records';
    this.router.navigate([`${path}/restore/${hitListId}`]);
  }

  register() {
    this.recordDetailView.register();
  }

  private togglePanel(e) {
    if (e.srcElement.children.length > 0) {
      e.srcElement.children[0].click();
    }
  }

  private showSaveTemplate(e) {
    if (this.template) {
      if (confirm('Do you want to overwrite the saved template?')) {
      }
    } else {
      this.saveTemplatePopupVisible = true;
    }
  }

  private saveTemplate(e) {
    let result: any = this.saveTemplateForm.validate();
    if (result.isValid) {
      let recordDoc = this.recordDetailView.getUpdatedRecord();
      if (!recordDoc) {
        return;
      }
      let url = `${apiUrlPrefix}templates`;
      let data: ITemplateData = new CTemplateData(this.saveTemplateData.name);
      data.description = this.saveTemplateData.description;
      data.isPublic = this.saveTemplateData.isPublic;
      data.data = registryUtils.serializeData(recordDoc);
      this.http.post(url, data).toPromise()
        .then(res => {
          this.regTemplates.dataSource = undefined;
          notifySuccess((res.json() as IResponseData).message, 5000);
        })
        .catch(error => {
          notifyException(`The submission data was not saved properly due to a problem`, error, 5000);
        });
      this.saveTemplatePopupVisible = false;
    }
  }

  private cancelSaveTemplate(e) {
    this.saveTemplatePopupVisible = false;
  }

  private showTemplates(e) {
    this.currentIndex = 1;
    if (!this.regTemplates.dataSource) {
      this.regTemplates.loadData();
    }
    this.update();
  }

  private showDetails(e) {
    this.currentIndex = 0;
    this.update();
  }

  private get isNewRecord(): boolean {
    return this.id < 0 || this.template;
  }

  private get saveButtonTitle(): string {
    return this.isNewRecord ? 'Submit' : 'Save';
  }

  private onSaveTemplateFormInit(e) {
    this.saveTemplateForm = e.component as DxForm;
  }

  private get statusId(): number {
    return this.recordDetailView != null ? this.recordDetailView.statusId : null;
  }

  private set statusId(statusId: number) {
    if (this.recordDetailView != null) {
      this.recordDetailView.statusId = statusId;
    }
  }

  private cancelApproval() {
    let url = `${apiUrlPrefix}temp-records/${this.id}/${RegistryStatus.Submitted}`;
    this.http.put(url, undefined).toPromise()
      .then(res => {
        this.regTemplates.dataSource = undefined;
        this.statusId = RegistryStatus.Submitted;
        this.update();
        notifySuccess(`The current temporary record's approval was cancelled successfully!`, 5000);
      })
      .catch(error => {
        notifyException(`The approval cancelling process failed due to a problem`, error, 5000);
      });
    this.saveTemplatePopupVisible = false;
  }

  private approve() {
    let url = `${apiUrlPrefix}temp-records/${this.id}/${RegistryStatus.Approved}`;
    this.http.put(url, undefined).toPromise()
      .then(res => {
        this.regTemplates.dataSource = undefined;
        this.statusId = RegistryStatus.Approved;
        this.update();
        notifySuccess(`The current temporary record was approved successfully!`, 5000);
      })
      .catch(error => {
        notifyException(`The approval process failed due to a problem`, error, 5000);
      });
    this.saveTemplatePopupVisible = false;
  }

  private delete() {
    if (confirm('Are you sure you want to delete this Registry Record?')) {
      let url = `${apiUrlPrefix}${this.temporary ? 'temp-' : ''}records/${this.id}`;
      this.http.delete(url).toPromise()
        .then(res => {
          notifySuccess(`The record was deleted successfully!`, 5000);
          this.router.navigate([`records/${this.temporary ? 'temp' : ''}`]);
        })
        .catch(error => {
          notifyException(`The record was not deleted due to a problem`, error, 5000);
        });
    }
  }

  private clear() {
    this.recordDetailView.clear();
  }

  private getLookup(name: string): any[] {
    let lookups = this.ngRedux.getState().session.lookups;
    return lookups ? lookups[name] : [];
  }

  private onDetailContentReady(e) {
    let recordDetail: IRecordDetail = e.data;
    this.isLoggedInUserOwner = recordDetail.isLoggedInUserOwner;
    this.isLoggedInUserSuperVisor = recordDetail.isLoggedInUserSuperVisor;
    let recordDetailBase: RegRecordDetailBase = e.component;
    this.displayMode = recordDetailBase.displayMode;
    let recordId = recordDetailBase.recordId;
    this.title = this.isNewRecord ?
      'Register a New Compound' :
      this.temporary ?
        'Edit a Temporary Record: ' + recordId :
        'Edit a Registry Record: ' + recordId;
    this.update();
  }
};
