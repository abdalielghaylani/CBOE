import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectorRef, ChangeDetectionStrategy, ViewEncapsulation, ViewChild } from '@angular/core';
import { NgRedux, select } from '@angular-redux/store';
import validationEngine from 'devextreme/ui/validation_engine';
import { IFormGroup, prepareFormGroupData, FormGroupType, IForm, ICoeForm } from '../../../../common';
import { RecordDetailActions, IAppState, IRecordDetail, ILookupData } from '../../../../redux';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { CRegistryRecord, CViewGroup, CFragment, RegFormGroupView } from '../../base';
import * as registryUtils from '../../registry.utils';
import * as X2JS from 'x2js';

@Component({
  selector: 'reg-duplicate-record-detail-view',
  template: require('./duplicate-records-detail-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDuplicateRecordDetailView {
  private dataSubscription: Subscription;
  private recordDoc: Document;
  private regRecord: CRegistryRecord = new CRegistryRecord();
  public formGroup: IFormGroup;
  @Input() id: number;
  private activated: boolean = true;
  private editMode: boolean = false;
  private displayMode: string = 'view';
  private viewModel: any;
  private position: string;
  private loadingVisible: boolean = true;
  public viewGroups: CViewGroup[];
  @Input() parentHeight: number;
  @select(s => s.registry.currentRecord) recordDetail$: Observable<IRecordDetail>;

  constructor(private ngRedux: NgRedux<IAppState>,
    private actions: RecordDetailActions,
    private changeDetector: ChangeDetectorRef, ) {
  }

  private togglePanel(e) {
    if (e.srcElement.children.length > 0) {
      e.srcElement.children[0].click();
    }
  }

  ngOnInit() {
    this.actions.retrieveRecord(false, false, this.id);
    this.position = `{ of: '#formGroupView_` + this.id + `' }`;
    if (!this.dataSubscription) {
      this.dataSubscription = this.recordDetail$.subscribe((value: IRecordDetail) => this.loadDetailData(value));
    }
  }

  loadDetailData(recordDetail: IRecordDetail) {
    if (recordDetail.id !== this.id) {
      return;
    }
    this.recordDoc = registryUtils.getDocument(recordDetail.data);
    let recordJson: any = this.x2jsTool.dom2js(this.recordDoc);
    this.regRecord = CRegistryRecord.createFromPlainObj(recordJson.MultiCompoundRegistryRecord);
    if (!this.regRecord.ComponentList.Component[0].Compound.FragmentList) {
      this.regRecord.ComponentList.Component[0].Compound.FragmentList = { Fragment: [new CFragment()] };
    }
    prepareFormGroupData(FormGroupType.ViewMixture, this.ngRedux);
    let state = this.ngRedux.getState();
    this.formGroup = state.configuration.formGroups[FormGroupType[FormGroupType.ViewMixture]] as IFormGroup;
    this.viewGroups = state.session.lookups ?
      CViewGroup.getViewGroups(this.formGroup, 'view', this.ngRedux.getState().session.lookups.disabledControls) : [];
    this.loadingVisible = false;
    this.changeDetector.markForCheck();
  }

  ngOnDestroy() {
    if (this.dataSubscription) {
      this.dataSubscription.unsubscribe();
    }
  }

  private get x2jsTool() {
    return new X2JS.default({
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
  }
};
