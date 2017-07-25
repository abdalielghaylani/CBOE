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
    let options = this.viewModel.editorOptions;
    this.dataSource = lookups ? lookups.projects.filter(i => i.ACTIVE === 'T') : [];
    this.value = options && options.value ? options.value.Project.map(v => +v.ProjectID) : [];
    this.displayExpr = 'NAME';
    this.valueExpr = 'PROJECTID';
  }

  protected onValueChanged(e, d) {
    if (e.previousValue !== e.value) {
      let value = { Project: e.value.map(v => { return { ProjectID: v.toString() }; }) };
      d.component.option('formData.' + d.dataField, value);
      this.onValueUpdated(this);
    }
  }
};
