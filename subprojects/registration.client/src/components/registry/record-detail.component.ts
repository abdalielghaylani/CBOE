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
import { IRegistryRecord, CRegistryRecord, CFragment, CViewGroup, RegFormGroupView } from './base';
import { basePath, apiUrlPrefix } from '../../configuration';
import { FormGroupType, IFormContainer, getFormGroupData, notifyError, notifyException, notifySuccess } from '../../common';
import { HttpService } from '../../services';
import { RegTemplates } from './templates.component';
import { RegistryStatus } from './registry.types';
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
export class RegRecordDetail implements IFormContainer, OnInit, OnDestroy, OnChanges {
  @ViewChild(RegTemplates) regTemplates: RegTemplates;
  @ViewChild(RegFormGroupView) formGroupView: RegFormGroupView;
  @Input() temporary: boolean;
  @Input() template: boolean;
  @Input() id: number;
  @select(s => s.registry.currentRecord) recordDetail$: Observable<IRecordDetail>;
  @select(s => s.registry.duplicateRecords) duplicateRecord$: Observable<any[]>;
  @select(s => s.session.lookups) lookups$: Observable<ILookupData>;
  private lookups: ILookupData;
  public formGroup: IFormGroup;
  public viewGroups: CViewGroup[];
  public editMode: boolean = false;
  private displayMode: string = 'view';
  private title: string;
  private parentHeight: string;
  private approvalsEnabled: boolean = false;
  private editButtonEnabled: boolean = false;
  private saveButtonEnabled: boolean = false;
  private registerButtonEnabled: boolean = false;
  private approveButtonEnabled: boolean = false;
  private cancelApprovalButtonEnabled: boolean = false;
  private deleteButtonEnabled: boolean = false;
  private clearButtonEnabled: boolean = false;
  private submissionTemplatesEnabled: boolean = false;
  private recordDoc: Document;
  private regRecord: CRegistryRecord = new CRegistryRecord();
  private routeSubscription: Subscription;
  private dataSubscription: Subscription;
  private loadSubscription: Subscription;
  private currentIndex: number = 0;
  private saveTemplateForm: DxForm;
  private saveTemplatePopupVisible: boolean = false;
  private lookupsSubscription: Subscription;
  private newButtonEnabled: boolean = false;
  private canRedirectToTempListView = true;
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
  private loadingVisible: boolean = false;


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
    if (!this.lookupsSubscription) {
      this.lookupsSubscription = this.lookups$.subscribe(d => { if (d) { this.retrieveContents(d); } });
    }
  }

  ngOnChanges() {
    this.update(false);
  }

  private update(forceUpdate: boolean = true) {
    this.editMode = this.displayMode !== 'view';
    if (!this.lookups || !this.lookups.userPrivileges) {
      return;
    }
    let userPrivileges = this.lookups.userPrivileges;
    let ss = new CSystemSettings(this.getLookup('systemSettings'));
    let statusId = this.statusId;
    let canEdit = this.isNewRecord ||
      PrivilegeUtils.hasEditRecordPrivilege(this.temporary, this.isLoggedInUserOwner, this.isLoggedInUserSuperVisor, userPrivileges);

    this.approvalsEnabled = (this.isNewRecord || this.temporary)
      && ss.isApprovalsEnabled
      && PrivilegeUtils.hasApprovalPrivilege(userPrivileges);

    this.cancelApprovalButtonEnabled = this.approvalsEnabled
      && !this.editMode
      && !!statusId
      && this.temporary
      && statusId === RegistryStatus.Approved
      && PrivilegeUtils.hasCancelApprovalPrivilege(userPrivileges);

    this.editButtonEnabled = !this.isNewRecord && !this.cancelApprovalButtonEnabled && !this.editMode && canEdit;
    this.saveButtonEnabled = (this.isNewRecord && !this.cancelApprovalButtonEnabled) || this.editMode;
    let canRegister = PrivilegeUtils.hasRegisterRecordPrivilege(this.isNewRecord, this.isLoggedInUserOwner, this.isLoggedInUserSuperVisor, userPrivileges);
    this.registerButtonEnabled = canRegister && (this.isNewRecord || (this.temporary && !this.editMode))
      && (!this.approvalsEnabled || this.cancelApprovalButtonEnabled);
    this.approveButtonEnabled = !this.editMode && !!statusId && this.temporary && this.approvalsEnabled && statusId !== RegistryStatus.Approved;

    this.deleteButtonEnabled = !this.isNewRecord
      && PrivilegeUtils.hasDeleteRecordPrivilege(this.temporary, this.isLoggedInUserOwner, this.isLoggedInUserSuperVisor, userPrivileges)
      && this.editButtonEnabled;

    this.clearButtonEnabled = this.isNewRecord;
    this.newButtonEnabled = !this.canRedirectToTempListView && !this.editMode;
    this.submissionTemplatesEnabled = this.isNewRecord
      && PrivilegeUtils.hasSubmissionTemplatePrivilege(userPrivileges) && ss.isSubmissionTemplateEnabled;
    let state = this.ngRedux.getState();
    this.backButtonEnabled = state.registry.records.data.hitlistId > 0;
    if (forceUpdate) {
      this.changeDetector.markForCheck();
    }
  }

  initialize(segments: UrlSegment[]) {
    let newIndex = segments.findIndex(s => s.path === 'new');
    if (newIndex >= 0 && newIndex < segments.length - 1) {
      this.id = +segments[segments.length - 1].path;
    }
    this.actions.retrieveRecord(this.temporary, this.template, this.id);
    if (!this.dataSubscription) {
      this.dataSubscription = this.recordDetail$.subscribe((value: IRecordDetail) => this.loadData(value));
      this.dataSubscription = this.duplicateRecord$.subscribe((value) => this.duplicateData(value));
    }
  }

  retrieveContents(lookups: ILookupData) {
    this.lookups = lookups;
  }

  duplicateData(e) {
    if (e) {
      this.loadingVisible = false;
      this.currentIndex = 2;
      this.changeDetector.markForCheck();
    }
  }

  ngOnDestroy() {
    if (this.routeSubscription) {
      this.routeSubscription.unsubscribe();
    }
    if (this.dataSubscription) {
      this.dataSubscription.unsubscribe();
    }
    if (this.loadSubscription) {
      this.loadSubscription.unsubscribe();
    }
    if (this.lookupsSubscription) {
      this.lookupsSubscription.unsubscribe();
    }
  }

  private getParentHeight() {
    return ((this.elementRef.nativeElement.parentElement.clientHeight) - 100).toString();
  }

  private get x2jsTool() {
    return new X2JS.default({
      arrayAccessFormPaths: [
        'MultiCompoundRegistryRecord.ComponentList.Component',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.PropertyList.Property',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.FragmentList.Fragment',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.BaseFragment.Structure.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.BatchList.Batch',
        'MultiCompoundRegistryRecord.BatchList.Batch.BatchComponentList.BatchComponent',
        'MultiCompoundRegistryRecord.BatchList.Batch.BatchComponentList.BatchComponent.BatchComponentFragmentList.BatchComponentFragment',
        'MultiCompoundRegistryRecord.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.ProjectList.Project',
        'MultiCompoundRegistryRecord.PropertyList.Property',
      ]
    });
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

  private onValueUpdated(e) {
    // console.log(this.regRecord);
  }

  loadData(recordDetail: IRecordDetail, duplicateResolution: boolean = false) {
    if (this.temporary !== recordDetail.temporary || this.id !== recordDetail.id) {
      return;
    }
    this.recordDoc = registryUtils.getDocument(recordDetail.data);
    this.isLoggedInUserOwner = recordDetail.isLoggedInUserOwner;
    this.isLoggedInUserSuperVisor = recordDetail.isLoggedInUserSuperVisor;
    this.title = this.isNewRecord ?
      'Register a New Compound' :
      recordDetail.temporary ?
        'Edit a Temporary Record: ' + this.getElementValue(this.recordDoc.documentElement, 'ID') :
        'Edit a Registry Record: ' + this.getElementValue(this.recordDoc.documentElement, 'RegNumber/RegNumber');
    let recordJson: any = this.x2jsTool.dom2js(this.recordDoc);
    this.regRecord = CRegistryRecord.createFromPlainObj(recordJson.MultiCompoundRegistryRecord);
    let formGroupType = FormGroupType.SubmitMixture;
    if (recordDetail.id >= 0 && !recordDetail.temporary) {
      // TODO: For mixture, this should be ReviewRegistryMixture
      formGroupType = FormGroupType.ViewMixture;
    }
    prepareFormGroupData(formGroupType, this.ngRedux);
    let state = this.ngRedux.getState();
    this.formGroup = state.configuration.formGroups[FormGroupType[formGroupType]] as IFormGroup;
    this.displayMode = this.isNewRecord ? 'add' : this.formGroup.detailsForms.detailsForm[0].coeForms._defaultDisplayMode === 'Edit' ? 'edit' : 'view';
    if (!this.regRecord.ComponentList.Component[0].Compound.FragmentList) {
      this.regRecord.ComponentList.Component[0].Compound.FragmentList = { Fragment: [new CFragment()] };
    }
    this.viewGroups = this.lookups ? CViewGroup.getViewGroups(this.formGroup, this.displayMode, this.lookups.disabledControls) : [];
    this.update();
  }

  getElementValue(e: Element, path: string) {
    return registryUtils.getElementValue(e, path);
  }

  private getUpdatedRecord() {
    let x2jsTool = this.x2jsTool;
    this.viewGroups.forEach(vg => {
      let items = vg.getItems(this.displayMode);
      let validItems = items.filter(i => !i.itemType || i.itemType !== 'empty').map(i => i.dataField);
      this.regRecord.serializeFormData(this.viewGroups, this.displayMode, validItems);
    });
    let recordDoc = x2jsTool.js2dom({ MultiCompoundRegistryRecord: this.regRecord });
    if (!recordDoc) {
      notifyError('Invalid content!', 5000);
    }
    return recordDoc;
  }

  save() {
    if (!this.formGroupView.validate()) {
      return;
    }
    let recordDoc = this.getUpdatedRecord();
    if (!recordDoc) {
      return;
    }
    this.recordDoc = recordDoc;
    let id = this.template ? -1 : this.id;
    if (this.isNewRecord) {
      // if user does not have SEARCH_TEMP privilege, should not re-direct to records list view, after successful save
      this.canRedirectToTempListView = PrivilegeUtils.hasSearchTempPrivilege(this.lookups.userPrivileges);
      this.actions.saveRecord({
        temporary: this.temporary, id: id, recordDoc: this.recordDoc,
        saveToPermanent: false, checkDuplicate: false, redirectToRecordsView: this.canRedirectToTempListView
      });
      if (!this.canRedirectToTempListView) {
        this.setDisplayMode('view');
        this.saveButtonEnabled = false;
        this.clearButtonEnabled = false;
        this.newButtonEnabled = true;
      }
    } else {
      this.setDisplayMode('view');
      this.actions.saveRecord({ temporary: this.temporary, id: id, recordDoc: this.recordDoc, saveToPermanent: false, checkDuplicate: false });
    }
  }

  cancel() {
    this.setDisplayMode('view');
  }

  cancelDuplicateResolution(e) {
    this.actions.clearDuplicateRecord();
    this.currentIndex = 0;
  }

  edit() {
    this.setDisplayMode('edit');
  }

  newRecord() {
    this.router.navigate([`records/new?${new Date().getTime()}`]);
  }

  back() {
    let state = this.ngRedux.getState();
    let hitListId = state.registry.records.data.hitlistId;
    this.router.navigate([`records/restore/${hitListId}`]);
  }
  register() {
    let recordDoc = this.getUpdatedRecord();
    if (!recordDoc) {
      return;
    }
    this.recordDoc = recordDoc;
    let duplicateEnabled = this.lookups.systemSettings.filter(s => s.name === 'CheckDuplication')[0].value === 'True' ? true : false;
    // this.loadingVisible = true;
    this.actions.saveRecord({ temporary: this.temporary, id: this.id, recordDoc: this.recordDoc, saveToPermanent: true, checkDuplicate: duplicateEnabled });
  }

  private setDisplayMode(mode: string) {
    this.displayMode = mode;
    this.update();
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
      let recordDoc = this.getUpdatedRecord();
      if (!recordDoc) {
        return;
      }
      this.recordDoc = recordDoc;
      let url = `${apiUrlPrefix}templates`;
      let data: ITemplateData = new CTemplateData(this.saveTemplateData.name);
      data.description = this.saveTemplateData.description;
      data.isPublic = this.saveTemplateData.isPublic;
      data.data = registryUtils.serializeData(this.recordDoc);
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

  private onSaveTemplateFormInit(e) {
    this.saveTemplateForm = e.component as DxForm;
  }

  private get statusId(): number {
    let statusIdText = this.recordDoc ? this.getElementValue(this.recordDoc.documentElement, 'StatusID') : null;
    return statusIdText ? +statusIdText : null;
  }

  private set statusId(statusId: number) {
    registryUtils.setElementValue(this.recordDoc.documentElement, 'StatusID', statusId.toString());
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
    // TODO: Bind an empty data object to the form-group view
    // CBOE-5060: 'Clear' button is missing in submit new compound page
  }

  private getLookup(name: string): any[] {
    let lookups = this.ngRedux.getState().session.lookups;
    return lookups ? lookups[name] : [];
  }
};
