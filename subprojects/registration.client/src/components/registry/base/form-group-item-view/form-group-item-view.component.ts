import { IInventoryContainerList } from './../../../../redux/store/registry/registry.types';
import { PrivilegeUtils } from './../../../../common/utils/privilege.utils';
import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectorRef, ChangeDetectionStrategy, ViewEncapsulation, OnInit } from '@angular/core';
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
import { IRegInvModel } from '../../registry.types';
import { RegInvContainerHandler } from '../../inventory-container-handler/inventory-container-handler';

@Component({
  selector: 'reg-form-group-item-view',
  template: require('./form-group-item-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFormGroupItemView extends RegFormGroupItemBase implements OnInit {
  @Input() viewModel: CRegistryRecord;
  @Input() invIntegrationEnabled: boolean = false;
  @Input() invContainers: IInventoryContainerList;
  @Output() batchValueChanged = new EventEmitter<any>();
  private batchCommandsEnabled: boolean = false;
  private addBatchEnabled: boolean = false;
  private moveBatchEnabled: boolean = false;
  private deleteBatchEnabled: boolean = false;
  private selectBatchEnabled: boolean = false;
  private createContainerButtonEnabled: boolean = false;
  private invModel: IRegInvModel;
  private selectedBatchId: number;
  private loadingVisible: boolean = false;

  constructor(private ngRedux: NgRedux<IAppState>,
    private http: HttpService,
    private changeDetector: ChangeDetectorRef) {
    super();
  }

  ngOnInit() {
    if (this.viewModel && this.viewConfig.subArray != null && this.viewModel.BatchList && this.viewConfig.subArray.length > 0) {
      this.selectedBatchId = Number(this.viewModel.BatchList.Batch[0].BatchID);
    }
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
    this.selectedBatchId = batchId;
  }

  protected onBatchCreated(e) {
    this.loadingVisible = true;
    let regNum = this.viewModel.RegNumber.RegNumber;
    let url = `${apiUrlPrefix}/records/` + regNum + `/batches`;
    this.http.post(url, e).toPromise()
      .then(res => {
        this.batchValueChanged.emit(e);
        notifySuccess(`The batch created successfully!`, 2000);
        this.loadingVisible = false;
      })
      .catch(error => {
        notifyException(`The batch was not created due to a problem`, error, 5000);
        this.loadingVisible = false;
      });
  }

  protected onBatchMoved(e) {
    this.loadingVisible = true;
    let regNum = this.viewModel.RegNumber.RegNumber;
    let url = `${apiUrlPrefix}/batches/${e.batchId}/${this.viewModel.RegNumber.RegNumber}/${e.targetRegNum}`;
    this.http.post(url, null).toPromise()
      .then(res => {
        this.batchValueChanged.emit(e);
        notifySuccess(res.json().message, 2000);
        this.loadingVisible = false;
      })
      .catch(error => {
        notifyException(`The batch was not moved due to a problem`, error, 5000);
        this.loadingVisible = false;
      });
  }


  protected update() {
    super.update();
    let lookups = this.ngRedux.getState().session.lookups;
    let systemSettings = new CSystemSettings(lookups.systemSettings);
    this.batchCommandsEnabled = this.viewConfig.subArray != null;
    let canModifyBatch: boolean = this.batchCommandsEnabled && !this.editMode && this.updatable;
    this.addBatchEnabled = canModifyBatch && PrivilegeUtils.hasAddBatchPrivilege(lookups.userPrivileges);
    this.deleteBatchEnabled = canModifyBatch && this.viewConfig.subArray.length > 1;
    this.moveBatchEnabled = this.deleteBatchEnabled && systemSettings.isMoveBatchEnabled;
    this.selectBatchEnabled = this.batchCommandsEnabled && this.viewConfig.subArray.length > 1;
    this.createContainerButtonEnabled = !this.editMode && this.invIntegrationEnabled;
  }

  private createInvContainer() {
    let regInvContainer = new RegInvContainerHandler();
    this.invModel = { batchIDs: [this.viewModel.BatchList.Batch[this.viewConfig.subIndex].BatchID], isBulkContainerCreation: false };
    regInvContainer.createContainer(this.invModel);
  }

  private get batchContainers(): any[] {
    let batchId = this.selectedBatchId ? this.selectedBatchId : 0;
    let batch = null;
    if (batchId > 0) {
      batch = this.viewConfig.subArray.find(b => b.BatchID === batchId) as IBatch;
    } else {
      batch = this.viewModel.BatchList.Batch[batchId];
    }
    return this.invContainers.batchContainers.filter(item => item.regBatchID === batch.FullRegNumber);
  }

  private get batchContainersEnabled(): boolean {
    return this.batchCommandsEnabled && this.invContainers && this.invContainers.batchContainers && this.batchContainers.length > 0 ? true : false;
  }
};
