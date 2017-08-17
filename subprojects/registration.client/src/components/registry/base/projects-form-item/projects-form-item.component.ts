import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate, RegTagBoxFormItem, tagBoxFormItemTemplate } from '../../../common';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';

@Component({
  selector: 'reg-projects-form-item-template',
  template: tagBoxFormItemTemplate,
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegProjectsFormItem extends RegTagBoxFormItem {
  protected dataSource: any[];
  protected valueExpr: string;
  protected displayExpr: string;
  protected savedValue: any;

  constructor(private ngRedux: NgRedux<IAppState>) {
    super();
  }

  deserializeValue(value: any): any[] {
    return value.Project.map(v => +v.ProjectID.__text);
  }

  serializeValue(value: any[]): any {
    let index = 0;
    let projects: any[] = this.savedValue.Project;
    value.forEach(v => {
      if (projects.length > index) {
        projects[index].ProjectID.__text = v.toString();
      } else {
        projects.push({ ProjectID: { __text: v.toString() } });
      }
      ++index;
    });
    while (projects.length > index) {
      projects.splice(index, 1);
    }
    return this.savedValue;
  }

  protected update() {
    let lookups = this.ngRedux.getState().session.lookups;
    let options = this.viewModel.editorOptions;
    this.dataSource = lookups ? lookups.projects.filter(i => i.ACTIVE === 'T' && (i.TYPE === 'A' || i.TYPE === options.projectType)) : [];
    this.savedValue = options && options.value ? options.value : { Project: [] };
    this.value = this.deserializeValue(this.savedValue);
    this.displayExpr = 'NAME';
    this.valueExpr = 'PROJECTID';
  }

  protected onValueChanged(e, d) {
    if (e.previousValue !== e.value) {
      let value = this.serializeValue(e.value);
      d.component.option('formData.' + d.dataField, value);
      this.onValueUpdated(this);
    }
  }
};
