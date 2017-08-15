import {
  Component, EventEmitter, Input, Output,
  OnChanges, OnInit, OnDestroy,
  ChangeDetectorRef, ChangeDetectionStrategy, ViewEncapsulation
} from '@angular/core';
import { NgRedux, select } from '@angular-redux/store';
import { IFormGroup, prepareFormGroupData, FormGroupType } from '../../../../common';
import { RecordDetailActions, IAppState, IRecordDetail, ILookupData } from '../../../../redux';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { CRegistryRecord, CViewGroup, CFragment } from '../../base';
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
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();
  @Input() temporary: boolean;
  @Input() template: boolean;
  @Input() id: number;
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() displayMode: string;
  protected dataSubscription: Subscription;
  protected viewId: string;
  protected position: string;
  protected loadingVisible: boolean;
  protected recordDoc: Document;
  protected regRecord: CRegistryRecord = new CRegistryRecord();
  protected formGroup: IFormGroup;
  protected viewGroups: CViewGroup[];
  @select(s => s.registry.currentRecord) recordDetail$: Observable<IRecordDetail>;

  constructor(
    protected ngRedux: NgRedux<IAppState>,
    protected actions: RecordDetailActions,
    protected changeDetector: ChangeDetectorRef
  ) {
  }

  ngOnInit() {
    this.update();
  }

  ngOnDestroy() {
    if (this.dataSubscription) {
      this.dataSubscription.unsubscribe();
    }
  }

  ngOnChanges() {
    this.update();
  }

  protected update() {
    if (this.id != null) {
      let viewId = `formGroupView_${this.id}`.replace('-', '_');
      if (viewId !== this.viewId) {
        this.viewId = viewId;
        this.position = `{ of: '${this.viewId}' }`;
        this.actions.retrieveRecord(this.temporary, this.template, this.id);
        if (!this.dataSubscription) {
          this.dataSubscription = this.recordDetail$.subscribe((value: IRecordDetail) => this.loadRecordData(value));
        }
      }
    }
  }

  protected loadRecordData(data: IRecordDetail) {
    if (!data.data || data.id !== this.id) {
      return;
    }
    this.recordDoc = registryUtils.getDocument(data.data);
    this.prepareRegistryRecord();
    prepareFormGroupData(FormGroupType.ViewMixture, this.ngRedux);
    let state = this.ngRedux.getState();
    this.formGroup = state.configuration.formGroups[FormGroupType[FormGroupType.ViewMixture]] as IFormGroup;
    this.viewGroups = state.session.lookups ?
      CViewGroup.getViewGroups(this.formGroup, 'view', this.ngRedux.getState().session.lookups.disabledControls) : [];
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
        'MultiCompoundRegistryRecord.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.ProjectList.Project',
        'MultiCompoundRegistryRecord.PropertyList.Property',
      ]
    });
  }

  protected onValueUpdated(e) {
    this.valueUpdated.emit(e);
  }

  protected togglePanel(e) {
    if (e.srcElement.children.length > 0) {
      e.srcElement.children[0].click();
    }
  }

  protected prepareRegistryRecord() {
    let recordJson: any = this.x2jsTool.dom2js(this.recordDoc);
    this.regRecord = CRegistryRecord.createFromPlainObj(recordJson.MultiCompoundRegistryRecord);
    if (!this.regRecord.ComponentList.Component[0].Compound.FragmentList) {
      this.regRecord.ComponentList.Component[0].Compound.FragmentList = { Fragment: [new CFragment()] };
    }
  }
};
