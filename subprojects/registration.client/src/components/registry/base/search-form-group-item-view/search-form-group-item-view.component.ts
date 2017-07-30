import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { ISearchCriteriaItem, getSearchCriteriaItemObj } from '../registry-base.types';
import { RegFormGroupItemView } from '../form-group-item-view';

@Component({
  selector: 'reg-search-form-group-item-view',
  template: require('./search-form-group-item-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegSearchFormGroupItemView extends RegFormGroupItemView {
  constructor() {
    super();
  }

  private getQueryEntryValue(searchCriteriaItem: ISearchCriteriaItem): string {
    let value;
    let searchCriteriaItemObj: any = getSearchCriteriaItemObj(searchCriteriaItem);
    if (searchCriteriaItemObj && searchCriteriaItemObj.CSCartridgeStructureCriteria && searchCriteriaItemObj.CSCartridgeStructureCriteria.__text) {
      value = searchCriteriaItemObj.CSCartridgeStructureCriteria.__text;
    } else if (searchCriteriaItemObj.__text) {
      value = searchCriteriaItemObj.__text;
    }
    return value;
  }

  private getQueryFormData(idList: string[]): any {
    let formData: any = {};
    idList.forEach(id => {
      let entryInfo = this.viewConfig.getEntryInfo(this.displayMode, id);
      (this.viewModel as ISearchCriteriaItem[]).forEach(sg => {
        if (entryInfo.searchCriteriaItem._id === sg._id) {
          formData[id] = this.getQueryEntryValue(sg);
        }
      });
    });
    return formData;
  }

  private setQueryEntryValue(searchCriteriaItem: ISearchCriteriaItem, entryValue, serialize: boolean = false) {
    let searchCriteriaItemObj: any = getSearchCriteriaItemObj(searchCriteriaItem);
    if (searchCriteriaItemObj && searchCriteriaItemObj.CSCartridgeStructureCriteria && searchCriteriaItemObj.CSCartridgeStructureCriteria.__text) {
      searchCriteriaItemObj.CSCartridgeStructureCriteria.__text = entryValue;
    } else {
      searchCriteriaItemObj.__text = entryValue;
    }
  }

  private updateViewModelFromQueryFormData(idList: string[]) {
    idList.forEach(id => {
      let entryInfo = this.viewConfig.getEntryInfo(this.displayMode, id);
      (this.viewModel as ISearchCriteriaItem[]).forEach(sg => {
        if (entryInfo.searchCriteriaItem._id === sg._id) {
          this.setQueryEntryValue(sg, this.formData[id]);
        }
      });
    });
  }

  protected readVM() {
    let validItems = this.items.filter(i => !i.itemType || i.itemType !== 'empty');
    this.formData = this.getQueryFormData(validItems.map(i => i.dataField));
  }

  protected writeVM() {
    let validItems = this.items.filter(i => !i.itemType || i.itemType !== 'empty');
    this.updateViewModelFromQueryFormData(validItems.map(i => i.dataField));
  }
};
