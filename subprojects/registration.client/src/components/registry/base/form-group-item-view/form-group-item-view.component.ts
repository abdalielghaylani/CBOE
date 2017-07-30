import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IViewControl, CViewGroup, CRegistryRecord, IRegistryRecord, CEntryInfo, CBoundObject, IPropertyList } from '../registry-base.types';
import { IFormGroup, IForm, ICoeForm, ICoeFormMode, IFormElement } from '../../../../common';

@Component({
  selector: 'reg-form-group-item-view',
  template: require('./form-group-item-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFormGroupItemView implements IViewControl, OnChanges {
  @Input() id: string;
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() displayMode: string;
  @Input() viewModel: any;
  @Input() viewConfig: CViewGroup;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();
  protected items: any[] = [];
  protected formData: any = {};
  protected colCount: number = 5;

  constructor() {
  }

  ngOnChanges() {
    this.update();
  }

  private getFormElementContainer(f: ICoeForm, mode: string): ICoeFormMode {
    return mode === 'add' ? f.addMode : mode === 'edit' ? f.editMode : f.viewMode;
  }

  private getDataField(fe: IFormElement): string {
    return fe.Id ? fe.Id : fe._name.replace(/\s/g, '');
  }

  private getEntryValue(id: string): any {
    let entryInfo = this.viewConfig.getEntryInfo(this.displayMode, id);
    let dataSource = this.viewModel.getDataSource(entryInfo.dataSource);
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

  protected update() {
    if (this.viewConfig) {
      this.items = this.viewConfig.getItems(this.displayMode);
      this.readVM();
    }
  }

  protected readVM() {
    let validItems = this.items.filter(i => !i.itemType || i.itemType !== 'empty');
    this.formData = this.getFormData(validItems.map(i => i.dataField));
  }

  protected writeVM() {
    let validItems = this.items.filter(i => !i.itemType || i.itemType !== 'empty');
    this.updateViewModelFromFormData(validItems.map(i => i.dataField));
  }

  protected onValueUpdated(e) {
    this.writeVM();
    this.valueUpdated.emit(this);
  }
};
