import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectorRef, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { CViewGroup, CViewGroupContainer, CRegistryRecord, IRegistryRecord, CEntryInfo, CBoundObject, CSearchCriteria } from '../registry-base.types';
import * as dxDialog from 'devextreme/ui/dialog';
import { IAppState, CSystemSettings } from '../../../../redux';
import { HttpService } from '../../../../services';
import { basePath, apiUrlPrefix } from '../../../../configuration';
import { IViewControl, IPropertyList, IBatch } from '../../../common';
import { notifyError, notifyException, notifySuccess } from '../../../../common';
import { IFormGroup, IForm, ICoeForm, ICoeFormMode, IFormElement } from '../../../../common';
import { RegFormGroupItemBase } from '../form-group-item-base';

@Component({
  selector: 'reg-form-group-item-view',
  template: require('./form-group-item-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFormGroupItemView extends RegFormGroupItemBase {
  @Input() viewModel: CRegistryRecord;
  private batchCommandsEnabled: boolean = false;
  private addBatchEnabled: boolean = false;
  private moveBatchEnabled: boolean = false;
  private deleteBatchEnabled: boolean = false;

  constructor(private ngRedux: NgRedux<IAppState>, private http: HttpService, private changeDetector: ChangeDetectorRef) {
    super();
  }

  private getFormElementContainer(f: ICoeForm, mode: string): ICoeFormMode {
    return mode === 'add' ? f.addMode : mode === 'edit' ? f.editMode : f.viewMode;
  }

  private getDataField(fe: IFormElement): string {
    return fe.Id ? fe.Id : fe._name.replace(/\s/g, '');
  }

  private getEntryValue(id: string): any {
    let entryInfo = this.viewConfig.getEntryInfo(this.displayMode, id);
    let dataSource = this.viewModel.getDataSource(entryInfo.dataSource, this.viewConfig.subIndex);
    let foundObject = CRegistryRecord.findBoundObject(dataSource, entryInfo.bindingExpression);
    return foundObject.property ? foundObject.obj[foundObject.property] : undefined;
  }

  private getFormData(idList: string[]): any {
    let formData: any = {};
    idList.forEach(id => {
      formData[id] = this.getEntryValue(id);
    });
    return formData;
  }

  private updateViewModelFromFormData(idList: string[]) {
    idList.forEach(id => {
      this.viewModel.setEntryValue(this.viewConfig, this.displayMode, id, this.formData[id]);
    });
  }

  protected updateBatch() {
    this.viewConfig.title = 'Batch: ' + this.viewModel.BatchList.Batch[this.viewConfig.subIndex].FullRegNumber;
    this.update();
  }

  protected readVM() {
    let validItems = this.getValidItems().map(i => i.dataField);
    this.formData = this.getFormData(validItems);
  }

  protected writeVM() {
    let validItems = this.getValidItems().map(i => i.dataField);
    this.updateViewModelFromFormData(validItems);
  }

  protected selectBatch() {

  }

  protected addBatch() {

  }

  protected moveBatch() {

  }

  protected deleteBatch(showConfirmation: boolean = true) {
    if (showConfirmation) {
      let dialogResult = dxDialog.confirm(
        `Are you sure that you want to delete the batch?`,
        'Confirm Deleting a Batch');
      dialogResult.done(r => {
        if (r) {
          this.deleteBatch(false);
        }
      });
    } else {
      let batch = this.viewConfig.subArray[this.viewConfig.subIndex] as IBatch;
      let url = `${apiUrlPrefix}batches/${batch.BatchID}`;
      this.http.delete(url).toPromise()
        .then(res => {
          notifySuccess(`The batch was deleted successfully!`, 2000);
          this.viewModel.BatchList.Batch.splice(this.viewConfig.subIndex, 1);
          this.viewConfig.subArray = this.viewModel.BatchList.Batch;
          this.viewConfig.subIndex = Math.min(this.viewConfig.subIndex, this.viewConfig.subArray.length - 1);
          this.updateBatch();
          this.changeDetector.markForCheck();
        })
        .catch(error => {
          notifyException(`The batch was not deleted due to a problem`, error, 5000);
        });
    }
  }

  protected onBatchSelected(batchId) {
    this.viewConfig.subIndex = this.viewConfig.subArray.findIndex(b => b.BatchID === batchId);
    this.updateBatch();
  }

  protected update() {
    super.update();
    let lookups = this.ngRedux.getState().session.lookups;
    let systemSettings = new CSystemSettings(lookups.systemSettings);
    this.batchCommandsEnabled = this.viewConfig.subArray != null;
    this.addBatchEnabled = this.batchCommandsEnabled && !this.editMode && this.updatable;
    this.deleteBatchEnabled = this.addBatchEnabled && this.viewConfig.subArray.length > 1;
    this.moveBatchEnabled = this.deleteBatchEnabled && systemSettings.isMoveBatchEnabled;
  }
};
