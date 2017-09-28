import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate, RegTagBoxFormItem, tagBoxFormItemTemplate, IProject } from '../../../common';
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
    value.Project.forEach(v => {
      if (!v.ProjectID.__text) {
        // for data loading from template
        v.ID = v.ProjectID;
        v.ProjectID = { __text: v.ProjectID };
      }
    });
    return value.Project.map(v => +v.ProjectID.__text);
  }

  serializeValue(value: any[]): any {
    let index = 0;
    let projects: IProject[] = this.savedValue.Project;
    let projectsToRemove = projects.map(p => p.ID);
    value.forEach(v => {
      let existing = projects.find(p => p.ProjectID.__text === v.toString());
      if (existing != null) {
        projectsToRemove = projectsToRemove.filter(pId => pId !== existing.ID);
      } else {
        projects.push({ ProjectID: { __text: v.toString() } });
      }
      ++index;
    });
    this.savedValue.Project = projects.filter(p => projectsToRemove.findIndex(pId => pId === p.ID) < 0);
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
