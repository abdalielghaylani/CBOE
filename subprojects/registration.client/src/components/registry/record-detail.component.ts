import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
  OnInit, OnDestroy, AfterViewInit,
  ElementRef, ChangeDetectorRef,
  ViewChild, ViewChildren, QueryList
} from '@angular/core';
import { ActivatedRoute, UrlSegment, Params, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { select, NgRedux } from '@angular-redux/store';
import * as X2JS from 'x2js';
import { RecordDetailActions, ConfigurationActions } from '../../actions';
import { IAppState, IRecordDetail } from '../../store';
import * as registryUtils from './registry.utils';
import { IShareableObject, CShareableObject, CFormGroup, prepareFormGroupData, notify } from '../../common';
import { CRegistryRecord, CRegistryRecordVM, FragmentData, ITemplateData, CTemplateData } from './registry.types';
import { DxFormComponent } from 'devextreme-angular';
import { basePath, apiUrlPrefix } from '../../configuration';
import { CSystemSettings } from '../configuration';
import { FormGroupType, IFormContainer, getFormGroupData, notifyError, notifySuccess } from '../../common';
import { HttpService } from '../../services';
import { RegTemplates } from './templates.component';

declare var jQuery: any;

@Component({
  selector: 'reg-record-detail',
  template: require('./record-detail.component.html'),
  styles: [require('./records.css')],
  host: { '(document:click)': 'onDocumentClick($event)' },
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegRecordDetail implements IFormContainer, OnInit, OnDestroy {
  @ViewChild(RegTemplates) regTemplates: RegTemplates;
  @ViewChildren(DxFormComponent) forms: QueryList<DxFormComponent>;
  @Input() temporary: boolean;
  @Input() template: boolean;
  @Input() id: number;
  @select(s => s.registry.currentRecord) recordDetail$: Observable<IRecordDetail>;
  public formGroup: CFormGroup;
  public editMode: boolean = false;
  private title: string;
  private drawingTool;
  private creatingCDD: boolean = false;
  private cdxml: string;
  private parentHeight: string;
  private recordString: string;
  private recordDoc: Document;
  private regRecord: CRegistryRecord = new CRegistryRecord();
  private regRecordVM: CRegistryRecordVM = new CRegistryRecordVM(this.regRecord, this);
  private routeSubscription: Subscription;
  private dataSubscription: Subscription;
  private loadSubscription: Subscription;
  private currentIndex: number = 0;
  private saveTemplateForm;
  private saveTemplatePopupVisible: boolean = false;
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
    this.createDrawingTool();
    this.parentHeight = this.getParentHeight();
    let self = this;
    this.routeSubscription = this.activatedRoute.url.subscribe((segments: UrlSegment[]) => this.initialize(segments));
  }

  initialize(segments: UrlSegment[]) {
    let newIndex = segments.findIndex(s => s.path === 'new');
    if (newIndex >= 0 && newIndex < segments.length - 1) {
      this.id = +segments[segments.length - 1].path;
    }
    this.actions.retrieveRecord(this.temporary, this.template, this.id);
    if (!this.dataSubscription) {
      this.dataSubscription = this.recordDetail$.subscribe((value: IRecordDetail) => this.loadData(value));
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
    this.actions.clearRecord();
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

  loadData(recordDetail: IRecordDetail) {
    if (this.temporary !== recordDetail.temporary || this.id !== recordDetail.id) {
      return;
    }
    this.recordDoc = registryUtils.getDocument(recordDetail.data);
    this.title = this.isNewRecord() ?
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
    this.formGroup = state.configuration.formGroups[FormGroupType[formGroupType]] as CFormGroup;
    this.editMode = this.isNewRecord() || this.formGroup.detailsForms.detailsForm[0].coeForms._defaultDisplayMode === 'Edit';
    this.regRecordVM = new CRegistryRecordVM(this.regRecord, this);
    if (!this.regRecord.ComponentList.Component[0].Compound.FragmentList) {
      this.regRecord.ComponentList.Component[0].Compound.FragmentList = { Fragment: [new FragmentData()] };
    }
    let structureData = registryUtils.getElementValue(this.recordDoc.documentElement,
      'ComponentList/Component/Compound/BaseFragment/Structure/Structure');
    this.loadCdxml(structureData);
    this.changeDetector.markForCheck();
  }

  loadCdxml(cdxml: string) {
    if (this.drawingTool && !this.creatingCDD) {
      this.drawingTool.clear();
      if (cdxml) {
        this.drawingTool.loadCDXML(cdxml);
      }
    } else {
      this.cdxml = cdxml;
    }
  }

  getElementValue(e: Element, path: string) {
    return registryUtils.getElementValue(e, path);
  }

  createDrawingTool() {
    if (this.drawingTool || this.creatingCDD) {
      return;
    }
    this.creatingCDD = true;
    this.removePreviousDrawingTool();

    let cddContainer = jQuery(this.elementRef.nativeElement).find('.cdContainer');
    let cdWidth = cddContainer.innerWidth() - 4;
    let attachmentElement = cddContainer[0];
    let cdHeight = attachmentElement.offsetHeight;
    const self = this;
    jQuery(this.elementRef.nativeElement).find('.click_catch').height(cdHeight);
    let params = {
      element: attachmentElement,
      height: (cdHeight - 2),
      width: cdWidth,
      viewonly: false,
      callback: function (drawingTool) {
        self.drawingTool = drawingTool;
        jQuery(self.elementRef.nativeElement).find('.click_catch').addClass('hidden');
        if (drawingTool) {
          drawingTool.setViewOnly(false);
        }
        self.creatingCDD = false;
        drawingTool.fitToContainer();
        if (self.cdxml) {
          drawingTool.loadCDXML(self.cdxml);
          self.cdxml = null;
        }
      },
      licenseUrl: 'https://chemdrawdirect.perkinelmer.cloud/js/license.xml',
      config: { features: { disabled: ['ExtendedCopyPaste'] } }
    };

    (<any>window).perkinelmer.ChemdrawWebManager.attach(params);
  };

  removePreviousDrawingTool = function () {
    if (this.drawingTool) {
      let container = jQuery(this.elementRef.nativeElement).find('.cdContainer');
      container.find('div')[2].remove();
      this.drawingTool = undefined;
    }
  };

  private updateRecord() {
    registryUtils.setElementValue(this.recordDoc.documentElement,
      'ComponentList/Component/Compound/BaseFragment/Structure/Structure', this.drawingTool.getCDXML());
  }

  save() {
    this.updateRecord();
    if (this.isNewRecord()) {
      this.actions.saveRecord(this.temporary, this.id, this.recordDoc);
    } else {
      this.setEditMode(false);
      this.actions.saveRecord(this.temporary, this.id, this.recordDoc);
    }
  }

  cancel() {
    this.setEditMode(false);
  }

  edit() {
    // notify('Editing is experimental', 'warning');
    this.setEditMode(true);
  }

  register() {
    this.updateRecord();
    this.actions.saveRecord(this.temporary, this.id, this.recordDoc, true);
  }

  private setEditMode(editMode: boolean) {
    this.editMode = editMode;
    this.changeDetector.markForCheck();
    this.forms.forEach(f => {
      f.items.forEach(i => {
        if (i.template) {
          i.disabled = !editMode;
        }
      });
      f.readOnly = !editMode;
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
          notifySuccess(`The submission data was saved as template ${res.json().id} successfully!`, 5000);
        })
        .catch(error => {
          let message = `The submission data was not saved properly due to a problem`;
          let errorResult, reason;
          if (error._body) {
            errorResult = JSON.parse(error._body);
            reason = errorResult.Message;
          }
          message += (reason) ? ': ' + reason : '!';
          notifyError(message, 5000);
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
    this.changeDetector.markForCheck();
  }

  private showDetails(e) {
    this.currentIndex = 0;
    this.changeDetector.markForCheck();
  }

  private isNewRecord(): boolean {
    return this.id < 0 || this.template;
  }

  private onSaveTemplateFormInit(e) {
    this.saveTemplateForm = e.component;
  }

  private getStatusId(): number {
    let statusIdText = this.recordDoc ? this.getElementValue(this.recordDoc.documentElement, 'StatusID') : null;
    return statusIdText ? +statusIdText : null;
  }

  private getApprovalsEnabled(): boolean {
    return this.temporary && new CSystemSettings(this.ngRedux.getState().session.lookups.systemSettings).isApprovalsEnabled();
  }

  private editButtonEnabled(): boolean {
    return !this.isNewRecord() && !this.editMode && !this.cancelApprovalButtonEnabled();
  }

  private saveButtonEnabled(): boolean {
    return (this.isNewRecord() && !this.cancelApprovalButtonEnabled()) || this.editMode;
  }

  private registerButtonEnabled(): boolean {
    return this.temporary && (!this.editMode || this.isNewRecord()) && (!this.getApprovalsEnabled() || this.cancelApprovalButtonEnabled());
  }

  private approveButtonEnabled(): boolean {
    let statusId = this.getStatusId();
    return !this.editMode && statusId && this.temporary && this.getApprovalsEnabled() && statusId !== 2;
  }

  private cancelApprovalButtonEnabled(): boolean {
    let statusId = this.getStatusId();
    return !this.editMode && statusId && this.temporary && this.getApprovalsEnabled() && statusId === 2;
  }

  private cancelApproval() {
    // TODO: Call API to cancel approval of this record
  }

  private approve() {
    // TODO: Call API to approve this record
  }
};
