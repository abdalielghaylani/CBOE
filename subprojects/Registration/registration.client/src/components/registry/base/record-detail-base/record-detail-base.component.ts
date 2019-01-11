import {
  Component, EventEmitter, Input, Output,
  OnChanges, OnInit, OnDestroy,
  ChangeDetectorRef, ChangeDetectionStrategy, ViewChild, ViewEncapsulation
} from '@angular/core';
import { NgRedux, select } from '@angular-redux/store';
import { IFormGroup, prepareFormGroupData, FormGroupType, PrivilegeUtils, notifyError } from '../../../../common';
import { RecordDetailActions, IAppState, IRecordDetail, ILookupData, CSystemSettings, IRecordSaveData, IInventoryContainerList } from '../../../../redux';
import { Subscription ,  Observable } from 'rxjs';
import { CFragment } from '../../../common';
import { CRegistryRecord, CViewGroup, CViewGroupContainer } from '../../base';
import { RegFormGroupView } from '../form-group-view';
import { RegFormGroupItemBase } from '../form-group-item-base';
import * as registryUtils from '../../registry.utils';
import * as X2JS from 'x2js';

@Component({
  selector: 'reg-record-detail-base',
  template: require('./record-detail-base.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegRecordDetailBase implements OnInit, OnDestroy, OnChanges {
  @ViewChild(RegFormGroupView) formGroupView: RegFormGroupView;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();
  @Output() contentReady: EventEmitter<any> = new EventEmitter<any>();
  @Input() temporary: boolean;
  @Input() template: boolean;
  @Input() id: number;
  @Input() activated: boolean;
  @Input() displayMode: string;
  @Input() revision?: number;
  @Input() updatable: boolean = false;
  @Input() invIntegrationEnabled: boolean = false;
  @Input() sendToInventoryEnabled: boolean = false;
  @Input() invContainers: IInventoryContainerList;
  @Input() useCurrent: boolean;
  @Input() parentHeight: string;
  public editMode: boolean;
  protected dataSubscription: Subscription;
  protected viewId: string;
  protected position: string;
  protected recordDoc: Document;
  protected regRecord: CRegistryRecord = new CRegistryRecord();
  protected formGroup: IFormGroup;
  protected viewGroupContainers: CViewGroupContainer[];
  private loadingVisible: boolean = false;
  protected validationError: { isValid: boolean, errorMessages: string[] } = { isValid: true, errorMessages: [] };
  @select(s => s.registry.currentRecord) recordDetail$: Observable<IRecordDetail>;

  constructor(
    protected ngRedux: NgRedux<IAppState>,
    protected actions: RecordDetailActions,
    protected changeDetector: ChangeDetectorRef
  ) {
  }

  private get isNewRecord(): boolean {
    return this.id < 0 || this.template;
  }

  private getElementValue(e: Element, path: string) {
    return registryUtils.getElementValue(e, path);
  }

  private clearDataSubscription() {
    if (this.dataSubscription != null) {
      this.dataSubscription.unsubscribe();
      this.dataSubscription = undefined;
    }
  }

  ngOnInit() {
  }

  ngOnDestroy() {
    this.clearDataSubscription();
  }

  ngOnChanges() {
    this.update();
  }

  protected update() {
    this.updateEditMode();
    if (this.id != null) {
      let viewId = `formGroupView_${this.id}`.replace('-', '_');
      if (this.revision != null) {
        viewId += `_${this.revision}`;
      }
      if (viewId !== this.viewId) {
        this.viewId = viewId;
        this.position = `{ of: '#${this.viewId}' }`;
        if (!this.dataSubscription) {
          this.dataSubscription = this.recordDetail$.subscribe((value: IRecordDetail) => this.loadRecordData(value));
        }
      }
    }
  }

  protected createViewGroupContainers(viewGroups: CViewGroup[]): CViewGroupContainer[] {
    let lookups = this.ngRedux.getState().session.lookups;
    let systemSettings = new CSystemSettings(lookups.systemSettings);
    let sameBatch = systemSettings.isSameBatchesIdentity;
    let containers: CViewGroupContainer[] = [];
    containers.push(new CViewGroupContainer(this.id + '_reg', 'Registry Information'));
    let compId = this.isNewRecord ? 'New' : this.temporary ? 'Temporary' : this.regRecord.ComponentList.Component[0].Compound.RegNumber.RegNumber;
    let batchId = this.isNewRecord ? 'New' : this.temporary ? 'Temporary' : this.regRecord.BatchList.Batch[0].FullRegNumber;
    containers.push(new CViewGroupContainer(this.id + '_comp', `Component: ${compId}`));
    containers.push(new CViewGroupContainer(this.id + '_batch', `Batch: ${batchId}`, [],
      this.temporary || this.isNewRecord ? null : this.regRecord.BatchList.Batch));
    viewGroups.forEach(vg => {
      let forms = vg.data;
      let form = forms.find(f => f._dataSourceId != null);
      let dataSource = form._dataSourceId.toLowerCase();
      if (dataSource.startsWith('mixture')) {
        containers[0].viewGroups.push(vg);
      } else if (dataSource.startsWith('comp')) {
        containers[1].viewGroups.push(vg);
      } else if (dataSource.startsWith('batch')) {
        containers[2].viewGroups.push(vg);
      } else if (dataSource.startsWith('fragments')) {
        if (lookups.systemSettings.find(i => i.name === 'EnableFragments').value === 'True') {
          containers[sameBatch ? 1 : 2].viewGroups.push(vg);
        }
      }
    });
    // containers.forEach(c => {
    //   if (c.viewGroups.length === 1) {
    //     c.title = c.viewGroups[0].title;
    //   }
    // });
    return containers;
  }

  protected loadRecordData(data: IRecordDetail) {
    if (!data.data || data.id !== this.id || (!this.isNewRecord && data.temporary !== this.temporary)) {
      if (this.useCurrent && data.id !== this.id) {
        return;
      } else {
        this.actions.retrieveRecord(this.temporary, this.template, this.id);
        return;
      }
    }
    this.clearDataSubscription();
    this.actions.clearRecord();

    this.recordDoc = registryUtils.getDocument(data.data);
    this.prepareRegistryRecord();
    let formGroupType = FormGroupType.SubmitMixture;
    if (this.id >= 0 && !this.template) {
      formGroupType = this.temporary ? FormGroupType.ReviewRegisterMixture : FormGroupType.ViewMixture;
    }
    prepareFormGroupData(formGroupType, this.ngRedux);
    let state = this.ngRedux.getState();
    this.formGroup = state.configuration.formGroups[FormGroupType[formGroupType]] as IFormGroup;
    if (this.displayMode == null) {
      this.displayMode = this.isNewRecord ? 'add' : this.formGroup.detailsForms.detailsForm[0].coeForms._defaultDisplayMode === 'Edit' ? 'edit' : 'view';
    }
    let lookups = state.session.lookups;
    let viewGroups = lookups ? CViewGroup.getViewGroups(this.temporary, this.formGroup, this.displayMode, lookups.disabledControls) : [];
    this.viewGroupContainers = this.createViewGroupContainers(viewGroups);
    this.contentReady.emit({ component: this, data: data });
    this.loadingVisible = false;
    this.changeDetector.markForCheck();
  }

  protected get x2jsTool() {
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
        'MultiCompoundRegistryRecord.BatchList.Batch.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.BatchList.Batch.ProjectList.Project',
        'MultiCompoundRegistryRecord.BatchList.Batch.PropertyList.Property',
        'MultiCompoundRegistryRecord.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.ProjectList.Project',
        'MultiCompoundRegistryRecord.PropertyList.Property',
      ]
    });
  }

  protected onValueUpdated(e) {
    // console.log(this.regRecord);
    this.validationError.isValid = true;
    this.valueUpdated.emit(e);
  }

  protected updateEditMode() {
    this.editMode = this.displayMode != null && this.displayMode !== 'view';
  }

  public getUpdatedRecord(): Document {
    let x2jsTool = this.x2jsTool;
    this.viewGroupContainers.forEach(vgc => {
      let items = vgc.getItems(this.displayMode);
      let validItems = RegFormGroupItemBase.getValidItems(items).map(i => i.dataField);
      this.regRecord.serializeFormData(this.viewGroupContainers, this.displayMode, validItems);
    });
    let recordDoc = x2jsTool.js2dom({ MultiCompoundRegistryRecord: this.regRecord });
    if (!recordDoc) {
      notifyError('Invalid content!', 5000);
    }
    return recordDoc;
  }

  validate(excludeStructureField?: boolean): boolean {
    let result = this.formGroupView.validate();
    this.validationError.isValid = !result || result.isValid;
    this.validationError.errorMessages = [];

    if (excludeStructureField) {
      // exclude structure field required validation for `Save as Template` feature.     
      let structureFieldValid: boolean = true;
      result.brokenRules.forEach(element => {
        let validationRule = element as any;
        if (validationRule.validator.errorMessage) {
          if (validationRule.validator.peer.viewModel.dataField === 'BaseFragmentStructure') {
            // dot not display structure field required validation message in UI 
            structureFieldValid = false;
          } else {
            this.validationError.errorMessages.push(validationRule.validator.errorMessage);
          }
        }
      });

      if (!structureFieldValid && result.brokenRules.length === 1 && this.validationError.errorMessages.length === 0) {
        // if structure field is empty and there is no other field validation errors,
        // then skip stucture field validation and return true
        this.validationError.isValid = true;
        this.changeDetector.markForCheck();
        return true;
      }
    } else {
      result.brokenRules.forEach(element => {
        let validationRule = element as any;
        if (validationRule.validator.errorMessage) {
          this.validationError.errorMessages.push(validationRule.validator.errorMessage);
        }
      });
    }
    this.changeDetector.markForCheck();
    return !result || result.isValid;
  }

  public save(type?: string): boolean {
    if (!this.validate()) {
      return false;
    }
    let recordDoc = this.getUpdatedRecord();
    if (!recordDoc) {
      return false;
    }
    this.recordDoc = recordDoc;
    this.actions.saveRecord(
      this.saveRecordData(false, type));
    return true;
  }

  saveRecordData(savePermanent: boolean, type?: string): IRecordSaveData {
    let data: IRecordSaveData = {
      temporary: this.temporary,
      id: this.template ? -1 : this.id,
      recordDoc: this.recordDoc,
      saveToPermanent: savePermanent,
      recordData: {}
    };

    if (type) {
      data.recordData.duplicateCheckOption = 'N';
      data.recordData.action = type;
    } else {
      if (this.isDuplicateResolutionEnabled(savePermanent)) {
        data.recordData.duplicateCheckOption = 'C';
      }
      if (savePermanent || (!this.isNewRecord && !this.temporary)) {
        data.recordData.action = '';
      }
    }
    return data;
  }

  isDuplicateResolutionEnabled(p?: boolean) {
    let duplicateEnabled: boolean = false;
    if (p || (!this.isNewRecord && !this.temporary)) {
      duplicateEnabled = this.ngRedux.getState().session.lookups.systemSettings.filter(s => s.name === 'CheckDuplication')[0].value === 'True' ? true : false;
    }
    return duplicateEnabled;
  }

  public register() {
    if (!this.validate()) {
      return false;
    }
    let recordDoc = this.getUpdatedRecord();
    if (!recordDoc) {
      return true;
    }
    this.recordDoc = recordDoc;
    this.actions.saveRecord(
      this.saveRecordData(true));
    return true;
  }

  public prepareRegistryRecord() {
    let recordJson: any = this.x2jsTool.dom2js(this.recordDoc);
    this.regRecord = CRegistryRecord.createFromPlainObj(recordJson.MultiCompoundRegistryRecord);
    if (!this.regRecord.ComponentList.Component[0].Compound.FragmentList) {
      this.regRecord.ComponentList.Component[0].Compound.FragmentList = { Fragment: [new CFragment()] };
    }
  }

  public get statusId(): number {
    let statusIdText = this.recordDoc ? this.getElementValue(this.recordDoc.documentElement, 'StatusID') : null;
    return statusIdText ? +statusIdText : null;
  }

  public set statusId(statusId: number) {
    registryUtils.setElementValue(this.recordDoc.documentElement, 'StatusID', statusId.toString());
  }

  public get recordId(): string {
    return this.temporary
      ? this.getElementValue(this.recordDoc.documentElement, 'ID')
      : this.getElementValue(this.recordDoc.documentElement, 'RegNumber/RegNumber');
  }

  public get selectedBatchId(): string {
    return this.temporary ? '' : this.regRecord.BatchList.Batch[0].BatchID;
  }

  public clear() {
    this.validationError = { isValid: true, errorMessages: [] };
    this.dataSubscription = this.recordDetail$.subscribe((value: IRecordDetail) => this.loadRecordData(value));
    this.changeDetector.markForCheck();
  }
}
