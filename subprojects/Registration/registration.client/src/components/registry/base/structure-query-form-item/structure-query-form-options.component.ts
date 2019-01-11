import { Component, EventEmitter, Input, Output, ElementRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { IFormItemTemplate, RegStructureBaseFormItem } from '../../../common';
import { RegFormView } from '../form-view';
import { IStructureQueryOptions } from '../registry-base.types';

@Component({
  selector: 'reg-structure-query-form-options-template',
  template: require('./structure-query-form-options.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureQueryOptions {
  @Input() viewModel = {};
  @Input() viewConfig: IStructureQueryOptions;
  @Input() colCount = 2;
  @Output() optionUpdated: EventEmitter<any> = new EventEmitter<any>();
  private tetrahedralSameImage = require('../assets/tetrahedral-same.png');
  private tetrahedralEitherImage = require('../assets/tetrahedral-either.png');
  private tetrahedralAnyImage = require('../assets/tetrahedral-any.png');
  private thickBondFirstImage = require('../assets/thick-bond-first.png');
  private thickBondSecondImage = require('../assets/thick-bond-second.png');
  private doubleBondSameImage = require('../assets/double-bond-same.png');
  private doubleBondAnyFirstImage = require('../assets/double-bond-any-first.png');
  private doubleBondAnySecondImage = require('../assets/double-bond-any-second.png');

  constructor() {
  }

  onFieldDataChanged(e) {
    this.optionUpdated.emit(e);
  }
}
