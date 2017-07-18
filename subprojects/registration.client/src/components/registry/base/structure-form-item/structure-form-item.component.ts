import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate } from '../registry-base.types';
import { RegTagBoxFormItem } from '../tag-box-form-item';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { ChemDrawWeb } from '../../../common';

@Component({
  selector: 'reg-projects-form-item-template',
  template: require('../structure-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureFormItem extends ChemDrawWeb {
  protected dataSource: any[];
  protected valueExpr: string;
  protected displayExpr: string;

  constructor(private ngRedux: NgRedux<IAppState>, elementRef: ElementRef) {
    super(elementRef);
  }

  protected update() {
    // let lookups = this.ngRedux.getState().session.lookups;
    // this.dataSource = lookups ? lookups.projects.filter(i => i.ACTIVE === 'T') : [];
    // this.value = this.data.editorOptions.value ? this.data.editorOptions.value : [];
    // this.displayExpr = 'NAME';
    // this.valueExpr = 'PROJECTID';
  }
};
