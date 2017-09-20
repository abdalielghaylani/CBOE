import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { CViewGroup, CViewGroupContainer, CRegistryRecord, IRegistryRecord, CEntryInfo, CBoundObject, CSearchCriteria } from '../registry-base.types';
import { IViewControl, IPropertyList } from '../../../common';
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

  protected deleteBatch() {
    
  }
};
