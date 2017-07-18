import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate } from '../registry-base.types';
import { RegTagBoxFormItem } from '../tag-box-form-item';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';

@Component({
  selector: 'reg-projects-form-item-template',
  template: require('../tag-box-form-item/tag-box-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegProjectsFormItem extends RegTagBoxFormItem {
  protected dataSource: any[];
  protected valueExpr: string;
  protected displayExpr: string;

  constructor(private ngRedux: NgRedux<IAppState>) {
    super();
  }

  protected update() {
    let lookups = this.ngRedux.getState().session.lookups;
    this.dataSource = lookups ? lookups.projects.filter(i => i.ACTIVE === 'T') : [];
    this.value = this.data.editorOptions.value ? this.data.editorOptions.value : [];
    this.displayExpr = 'NAME';
    this.valueExpr = 'PROJECTID';
  }
};
