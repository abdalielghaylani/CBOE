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
import { IResponseData, CRegistryRecordVM, ITemplateData, CTemplateData } from './registry.types';
import { DxFormComponent } from 'devextreme-angular';
import DxForm from 'devextreme/ui/form';
import { IRegistryRecord, CRegistryRecord, CFragment } from './base';
import { basePath, apiUrlPrefix } from '../../configuration';
import { FormGroupType, IFormContainer, getFormGroupData, notifyError, notifyException, notifySuccess } from '../../common';
import { HttpService } from '../../services';
import { RegTemplates } from './templates.component';
import { RegistryStatus } from './registry.types';
import { ChemDrawWeb } from '../common';
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
  @ViewChild(ChemDrawWeb) private chemDrawWeb: ChemDrawWeb;
  @ViewChildren(DxFormComponent) forms: QueryList<DxFormComponent>;
  @Input() temporary: boolean;
  @Input() template: boolean;
  @Input() id: number;
  @select(s => s.registry.currentRecord) recordDetail$: Observable<IRecordDetail>;
  @select(s => s.registry.duplicateRecords) duplicateRecord$: Observable<any[]>;
  @select(s => s.session.lookups) lookups$: Observable<ILookupData>;
  public formGroup: IFormGroup;
  public editMode: boolean = false;
  private displayMode: string = 'view';
  private title: string;
  private creatingCDD: boolean = false;
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
  private recordString: string;
  private recordDoc: Document;
  private regRecord: IRegistryRecord = new CRegistryRecord();
  private regRecordVM: CRegistryRecordVM = new CRegistryRecordVM(this.regRecord, this);
  private routeSubscription: Subscription;
  private dataSubscription: Subscription;
  private loadSubscription: Subscription;
  private currentIndex: number = 0;
  private saveTemplateForm: DxForm;
  private saveTemplatePopupVisible: boolean = false;
  private lookups: ILookupData;
  private lookupsSubscription: Subscription;
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
    let self = this;
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
    this.submissionTemplatesEnabled = this.isNewRecord
      && PrivilegeUtils.hasSubmissionTemplatePrivilege(userPrivileges) && ss.isSubmissionTemplateEnabled;
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

  private onResize(event: any) {
    this.parentHeight = this.getParentHeight();
  }

  private onDocumentClick(event: any) {
    if (event.srcElement.title === 'Full Screen') {
      let fullScreenMode = event.srcElement.className === 'fa fa-compress fa-stack-1x white';
      this.parentHeight = (this.elementRef.nativeElement.parentElement.clientHeight - (fullScreenMode ? 10 : 190)).toString();
    }
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
    // registryUtils.fixStructureData(this.recordDoc);
    let x2jsTool = new X2JS.default({
      arrayAccessFormPaths: [
        'MultiCompoundRegistryRecord.ComponentList.Component',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.PropertyList.Property',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.FragmentList.Fragment',
        'MultiCompoundRegistryRecord.ComponentList.Component.Compound.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.BatchList.Batch',
        'MultiCompoundRegistryRecord.BatchList.Batch.BatchComponentList.BatchComponent',
        'MultiCompoundRegistryRecord.BatchList.Batch.BatchComponentList.BatchComponent.BatchComponentFragmentList.BatchComponentFragment',
        'MultiCompoundRegistryRecord.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.ProjectList.Project',
        'MultiCompoundRegistryRecord.PropertyList.Property',
      ]
    });
    let recordJson: any = x2jsTool.dom2js(this.recordDoc);
    this.regRecord = recordJson.MultiCompoundRegistryRecord;
    let formGroupType = FormGroupType.SubmitMixture;
    if (recordDetail.id >= 0 && !recordDetail.temporary) {
      // TODO: For mixture, this should be ReviewRegistryMixture
      formGroupType = FormGroupType.ViewMixture;
    }
    prepareFormGroupData(formGroupType, this.ngRedux);
    let state = this.ngRedux.getState();
    this.formGroup = state.configuration.formGroups[FormGroupType[formGroupType]] as IFormGroup;
    this.displayMode = this.isNewRecord ? 'add' : this.formGroup.detailsForms.detailsForm[0].coeForms._defaultDisplayMode === 'Edit' ? 'edit' : 'view';
    this.regRecordVM = new CRegistryRecordVM(this.regRecord, this);
    if (!this.regRecord.ComponentList.Component[0].Compound.FragmentList) {
      this.regRecord.ComponentList.Component[0].Compound.FragmentList = { Fragment: [new CFragment()] };
    }
    let structureData = registryUtils.getElementValue(this.recordDoc.documentElement,
      'ComponentList/Component/Compound/BaseFragment/Structure/Structure');
    if (this.chemDrawWeb) {
      this.chemDrawWeb.setValue(structureData);
    }
    this.update();
  }

  getElementValue(e: Element, path: string) {
    return registryUtils.getElementValue(e, path);
  }

  private updateRecord() {
    if (this.chemDrawWeb) {
      registryUtils.setElementValue(this.recordDoc.documentElement,
        'ComponentList/Component/Compound/BaseFragment/Structure/Structure', this.chemDrawWeb.getValue());
    }
  }

  save() {
    this.updateRecord();
    let id = this.template ? -1 : this.id;
    if (this.isNewRecord) {
      // if user does not have SEARCH_TEMP privilege, should not re-direct to records list view, after successful save
      let canRedirectToTempRecordsView = PrivilegeUtils.hasSearchTempPrivilege(this.lookups.userPrivileges);
      this.actions.saveRecord({
        temporary: this.temporary, id: id, recordDoc: this.recordDoc,
        saveToPermanent: false, checkDuplicate: false, redirectToRecordsView: canRedirectToTempRecordsView
      });
    } else {
      this.setDisplayMode('view');
      this.actions.saveRecord({ temporary: this.temporary, id: id, recordDoc: this.recordDoc, saveToPermanent: false, checkDuplicate: false });
    }
  }

  createDuplicateRecord() {
    this.actions.createDuplicate(
      this.ngRedux.getState().registry.previousRecordDetail,
      'Duplicate');
  }

  cancel() {
    this.setDisplayMode('view');
  }

  cancelDuplicateResolution(e) {
    this.currentIndex = 0;
  }

  edit() {
    this.setDisplayMode('edit');
  }

  register() {
    this.updateRecord();
    let duplicateEnabled = this.lookups.systemSettings.filter(s => s.name === 'CheckDuplication')[0].value === 'True' ? true : false;
    this.actions.saveRecord({ temporary: this.temporary, id: this.id, recordDoc: this.recordDoc, saveToPermanent: true, checkDuplicate: duplicateEnabled });
  }

  private setDisplayMode(mode: string) {
    this.displayMode = mode;
    this.update();
    this.forms.forEach(f => {
      f.items.forEach(i => {
        if (i.template) {
          i.disabled = !this.editMode;
        }
      });
      f.readOnly = !this.editMode;
      f.instance.repaint();
    });
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
      this.updateRecord();
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

  private clear() {
    // TODO: Bind an empty data object to the form-group view
    // CBOE-5060: 'Clear' button is missing in submit new compound page
  }

  private getLookup(name: string): any[] {
    let lookups = this.ngRedux.getState().session.lookups;
    return lookups ? lookups[name] : [];
  }
};
