import {
  Component, Input, Output, EventEmitter, ElementRef, ViewChild,
  OnInit, OnDestroy, OnChanges, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';
import { NgRedux, select } from '@angular-redux/store';
import validationEngine from 'devextreme/ui/validation_engine';
import { IAppState, RecordDetailActions, IRecordDetail } from '../../../../redux';
import { IBatch } from '../../../common';
import { getExceptionMessage, notify, notifyError, notifySuccess } from '../../../../common';
import { CViewGroupContainer, CRegistryRecord } from '../registry-base.types';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import * as X2JS from 'x2js';
import * as registryUtils from '../../registry.utils';


@Component({
  selector: 'reg-batch-creator',
  template: require('./batch-creator.component.html'),
  styles: [require('../registry-base.css')],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegBatchCreator implements OnChanges, OnDestroy {
  @Input() viewModel: IBatch[] = [];
  private regRecord: CRegistryRecord;
  private errorMessages;
  @Input() viewConfig: CViewGroupContainer;
  @Output() onCreated = new EventEmitter<any>();
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();
  @Output() batchCreatorInitialised: EventEmitter<any> = new EventEmitter<any>();
  protected dataSubscription: Subscription;
  @select(s => s.registry.currentRecord) recordDetail$: Observable<IRecordDetail>;

  private formVisible: boolean = false;
  private items: any[];
  private formData: any;
  private colCount: number = 4;

  constructor(
    private ngRedux: NgRedux<IAppState>,
    protected actions: RecordDetailActions,
    private changeDetector: ChangeDetectorRef,
    private elementRef: ElementRef
  ) {
    this.update();
    let rec = this.ngRedux.getState().registry.currentRecord;
    if (!(rec.id && rec.id === -1 && rec.data !== null)) {
      this.actions.retrieveRecord(false, false, -1);
    }
    if (!this.dataSubscription) {
      this.dataSubscription =
        this.recordDetail$.subscribe((value: IRecordDetail) => this.loadRecordData(value));
    }
  }

  ngOnChanges() {
    this.update();
  }

  ngOnDestroy() {
    if (this.dataSubscription) {
      this.dataSubscription.unsubscribe();
    }
  }

  protected loadRecordData(rec: IRecordDetail) {
    if (rec.id && rec.id === -1 && rec.data !== null) {
      let recordData = this.ngRedux.getState().registry.currentRecord.data;
      let recordDoc = registryUtils.getDocument(recordData);
      let recordJson: any = this.x2jsTool().dom2js(recordDoc);
      this.regRecord = recordJson.MultiCompoundRegistryRecord;
    }
  }

  protected x2jsTool() {
    return new X2JS.default({
      arrayAccessFormPaths: [
        'MultiCompoundRegistryRecord.BatchList.Batch',
        'MultiCompoundRegistryRecord.BatchList.Batch.BatchComponentList.BatchComponent',
        'MultiCompoundRegistryRecord.BatchList.Batch.BatchComponentList.BatchComponent.BatchComponentFragmentList.BatchComponentFragment',
        'MultiCompoundRegistryRecord.BatchList.Batch.IdentifierList.Identifier',
        'MultiCompoundRegistryRecord.BatchList.Batch.ProjectList.Project',
        'MultiCompoundRegistryRecord.BatchList.Batch.PropertyList.Property'
      ]
    });
  }

  protected update() {
    this.items = this.viewConfig != null ? this.viewConfig.getItems('edit') : [];
    if (this.items.find(i => i.itemType === 'group') != null) {
      this.colCount = 1;
    }
    let validItems = this.viewConfig != null ? this.getValidItems() : [];
    this.formData = {};
    this.errorMessages = [];
    validItems.forEach(i => {
      this.formData[i.dataField] = undefined;
    });
    this.changeDetector.markForCheck();
  }

  protected onValueUpdated(e) {
    this.getValidItems().forEach(item => {
      let value = this.regRecord.BatchList.Batch[0].PropertyList.Property.find(i => i._name + 'Property' === item.dataField);
      if (value) {
        this.regRecord.BatchList.Batch[0].PropertyList.Property.find(i => i._name + 'Property' === item.dataField).__text = e.viewModel[item.dataField];
      } else {
        let entryInfo = this.viewConfig.getEntryInfo('edit', item.dataField);
        if (entryInfo.dataSource && entryInfo.bindingExpression) {
          let dataSource = this.getDataSource(entryInfo.dataSource, 0);
          let foundObject = CRegistryRecord.findBoundObject(dataSource, entryInfo.bindingExpression, true);
          if (foundObject.property) {
            dataSource[foundObject.property] = e.viewModel[item.dataField];
          }
        }
      }
    });
    this.validate();
  }

  private getDataSource(dataSource: string, subIndex: number): any {
    dataSource = dataSource.toLowerCase();
    return dataSource.indexOf('fragmentscsla') >= 0 ? this.regRecord.BatchList.Batch[subIndex].BatchComponentList.BatchComponent[0]
      : dataSource.indexOf('batch') >= 0 || dataSource.startsWith('fragments') ? this.regRecord.BatchList.Batch[subIndex]
        : this;
  }

  private getValidItems(): any[] {
    let validItems = [];
    this.viewConfig.getItems('edit').forEach(i => {
      if (i.itemType === 'group') {
        validItems = validItems.concat(i.items.filter(ix => !ix.itemType || ix.itemType !== 'empty'));
      } else if (i.itemType !== 'empty') {
        validItems.push(i);
      }
    });
    return validItems;
  }

  protected showForm(e) {
    this.update();
    this.formVisible = true;
  }

  protected createBatch(e) {
    if (this.validate().isValid) {
      this.formVisible = false;
      let recordJson: any = this.x2jsTool().js2xml(this.regRecord.BatchList);
      this.onCreated.emit(`<BatchList>${recordJson}</BatchList>`);
    }
  }

  validate() {
    let result = validationEngine.validateGroup('vg');
    this.errorMessages = [];
    if (result.brokenRules) {
      result.brokenRules.forEach(element => {
        if (element.validator.errorMessage) {
          this.errorMessages.push(element.validator.errorMessage);
        }
      });
    }
    return result;
  }

  protected cancel(e) {
    this.formVisible = false;
  }
};
