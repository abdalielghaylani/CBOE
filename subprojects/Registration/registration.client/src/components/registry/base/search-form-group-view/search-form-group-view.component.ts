import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { RegFormGroupView } from '../form-group-view';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { CdjsService } from '../../../../services';


@Component({
  selector: 'reg-search-form-group-view',
  template: require('./search-form-group-view.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegSearchFormGroupView extends RegFormGroupView {
  constructor(ngRedux: NgRedux<IAppState>, public cdjsService: CdjsService) {
    super(ngRedux, cdjsService);
  }
}
