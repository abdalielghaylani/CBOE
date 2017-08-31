import {
  Component, EventEmitter, Input, Output,
  OnChanges, OnInit, OnDestroy,
  ChangeDetectorRef, ChangeDetectionStrategy, ViewChild, ViewEncapsulation
} from '@angular/core';
import { NgRedux, select } from '@angular-redux/store';
import { IFormGroup, prepareFormGroupData, FormGroupType, PrivilegeUtils, notifyError } from '../../../../common';
import { RecordDetailActions, IAppState, IRecordDetail, ILookupData, CSystemSettings } from '../../../../redux';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { CFragment } from '../../../common';
import { CRegistryRecord, CViewGroup, CViewGroupContainer } from '../../base';
import { RegFormGroupView } from '../form-group-view';
import { RegFormGroupItemView } from '../form-group-item-view';
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
  public editMode: boolean;
  protected dataSubscription: Subscription;
  protected viewId: string;
  protected position: string;
  protected loadingVisible: boolean = true;
  protected recordDoc: Document;
  protected regRecord: CRegistryRecord = new CRegistryRecord();
  protected formGroup: IFormGroup;
  protected viewGroupContainers: CViewGroupContainer[];
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
    containers.push(new CViewGroupContainer(this.id + '_reg', 'Registry Information', []));
    let compId = this.temporary ? 'Temporary' : this.regRecord.ComponentList.Component[0].Compound.RegNumber.RegNumber;
    let batchId = this.temporary ? 'Temporary' : this.regRecord.BatchList.Batch[0].FullRegNumber;
    containers.push(new CViewGroupContainer(this.id + '_comp', `Component: ${compId}`, []));
    containers.push(new CViewGroupContainer(this.id + '_batch', `Batch: ${batchId}`, []));
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
        containers[sameBatch ? 1 : 2].viewGroups.push(vg);
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
      this.loadingVisible = true;
      this.actions.retrieveRecord(this.temporary, this.template, this.id);
      return;
    }
    this.clearDataSubscription();
    this.actions.clearRecord();
    this.recordDoc = registryUtils.getDocument(data.data);
    this.prepareRegistryRecord();
    let formGroupType = FormGroupType.SubmitMixture;
    if (this.id >= 0 && !this.temporary && !this.template) {
      // TODO: For mixture, this should be ReviewRegistryMixture
      formGroupType = FormGroupType.ViewMixture;
    }
    prepareFormGroupData(formGroupType, this.ngRedux);
    let state = this.ngRedux.getState();
    this.formGroup = state.configuration.formGroups[FormGroupType[formGroupType]] as IFormGroup;
    if (this.displayMode == null) {
      this.displayMode = this.isNewRecord ? 'add' : this.formGroup.detailsForms.detailsForm[0].coeForms._defaultDisplayMode === 'Edit' ? 'edit' : 'view';
    }
    let lookups = state.session.lookups;
    let viewGroups = lookups ? CViewGroup.getViewGroups(this.formGroup, this.displayMode, lookups.disabledControls) : [];
    this.viewGroupContainers = this.createViewGroupContainers(viewGroups);
    this.loadingVisible = false;
    this.contentReady.emit({ component: this, data: data });
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
        'MultiCompoundRegistryRecord.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.ProjectList.Project',
        'MultiCompoundRegistryRecord.PropertyList.Property',
      ]
    });
  }

  protected onValueUpdated(e) {
    // console.log(this.regRecord);
    this.valueUpdated.emit(e);
  }

  protected togglePanel(e) {
    if (e.srcElement.children.length > 0) {
      e.srcElement.children[0].click();
    }
  }

  protected updateEditMode() {
    this.editMode = this.displayMode != null && this.displayMode !== 'view';
  }

  public getUpdatedRecord(): Document {
    let x2jsTool = this.x2jsTool;
    this.viewGroupContainers.forEach(vgc => {
      let items = vgc.getItems(this.displayMode);
      let validItems = RegFormGroupItemView.getValidItems(items).map(i => i.dataField);
      this.regRecord.serializeFormData(this.viewGroupContainers, this.displayMode, validItems);
    });
    let recordDoc = x2jsTool.js2dom({ MultiCompoundRegistryRecord: this.regRecord });
    if (!recordDoc) {
      notifyError('Invalid content!', 5000);
    }
    return recordDoc;
  }

  public save(): boolean {
    if (!this.formGroupView.validate()) {
      notifyError('One or more entries failed to validate!', 5000);
      return false;
    }
    let recordDoc = this.getUpdatedRecord();
    if (!recordDoc) {
      return false;
    }
    this.recordDoc = recordDoc;
    let id = this.template ? -1 : this.id;
    if (this.isNewRecord) {
      // if user does not have SEARCH_TEMP privilege, should not re-direct to records list view, after successful save
      let canRedirectToTempListView = PrivilegeUtils.hasSearchTempPrivilege(this.ngRedux.getState().session.lookups.userPrivileges);
      this.actions.saveRecord({
        temporary: this.temporary, id: id, recordDoc: this.recordDoc,
        saveToPermanent: false,
        checkDuplicate: this.isDuplicateResolutionEnabled(),
        redirectToRecordsView: canRedirectToTempListView
      });
    } else {
      this.actions.saveRecord({
        temporary: this.temporary, id: id,
        recordDoc: this.recordDoc, saveToPermanent: false,
        checkDuplicate: this.isDuplicateResolutionEnabled()
      });
    }
    return true;
  }

  isDuplicateResolutionEnabled(p?: boolean) {
    let duplicateEnabled: boolean = false;
    if (p || (!this.isNewRecord && !this.temporary)) {
      duplicateEnabled = this.ngRedux.getState().session.lookups.systemSettings.filter(s => s.name === 'CheckDuplication')[0].value === 'True' ? true : false;
      if (duplicateEnabled) {
        this.loadingVisible = true;
      }
    }
    return duplicateEnabled;
  }

  public register() {
    let recordDoc = this.getUpdatedRecord();
    if (!recordDoc) {
      return;
    }
    this.recordDoc = recordDoc;
    this.actions.saveRecord({
      temporary: this.temporary, id: this.id,
      recordDoc: this.recordDoc, saveToPermanent: true,
      checkDuplicate: this.isDuplicateResolutionEnabled(true)
    });
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

  public clear() {
    this.dataSubscription = this.recordDetail$.subscribe((value: IRecordDetail) => this.loadRecordData(value));
  }
}
