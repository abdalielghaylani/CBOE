import { Component, EventEmitter, Input, Output, OnChanges, AfterViewInit, ChangeDetectionStrategy, ViewChild, ViewEncapsulation } from '@angular/core';
import { RegBaseColumnItem } from '../base-column-item';
import { apiUrlPrefix } from '../../../configuration';

@Component({
  selector: 'reg-structure-column-item-template',
  template: require('./structure-column-item.component.html'),
  styles: [require('../common.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureColumnItem extends RegBaseColumnItem {
  @Input() smallImage: boolean = false;

  deserializeValue(value: any): any {
    let size = this.smallImage ? '/30/60?' : '/50/100?';
    return value ? `${apiUrlPrefix}StructureImage/${value}`.replace('?', size) : '';
  }

  update() {
    if (this.viewConfig && this.viewModel) {
      let options = this.viewConfig;
      this.smallImage = !!options.smallImage;
      this.value = this.deserializeValue(this.viewModel.value);
    }
  }
};
