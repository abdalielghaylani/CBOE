import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { CViewGroup, CViewGroupContainer, CRegistryRecord, IRegistryRecord, CEntryInfo, CBoundObject } from '../registry-base.types';
import { IViewControl, IPropertyList } from '../../../common';
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
  @Input() viewConfig: CViewGroupContainer;
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
      if (this.items.find(i => i.itemType === 'group') != null) {
        this.colCount = 1;
      }
      this.readVM();
    }
  }

  protected readVM() {
    let validItems = RegFormGroupItemView.getValidItems(this.items).map(i => i.dataField);
    this.formData = this.getFormData(validItems);
  }

  protected writeVM() {
    let validItems = RegFormGroupItemView.getValidItems(this.items).map(i => i.dataField);
    this.updateViewModelFromFormData(validItems);
  }

  protected onValueUpdated(e) {
    this.writeVM();
    this.valueUpdated.emit(this);
  }

  public static getValidItems(items: any[]): any[] {
    let validItems = [];
    items.forEach(i => {
      if (i.itemType === 'group') {
        validItems = validItems.concat(i.items.filter(ix => !ix.itemType || ix.itemType !== 'empty'));
      } else if (i.itemType !== 'empty') {
        validItems.push(i);
      }
    });
    return validItems;    
  }
};
