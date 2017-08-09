import { Component, Input, OnChanges, AfterViewInit, ChangeDetectionStrategy, ViewChild, ViewEncapsulation } from '@angular/core';
import { DxDropDownBoxComponent } from 'devextreme-angular';
import { RegBaseColumnItem } from '../base-column-item';

@Component({
  selector: 'reg-drop-down-column-item-template',
  template: require('./drop-down-column-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDropDownColumnItem extends RegBaseColumnItem implements AfterViewInit {
  @ViewChild(DxDropDownBoxComponent) ddb: DxDropDownBoxComponent;
  protected dataSource: any[];
  protected valueExpr: string;
  protected displayExpr: string;
  protected columns: any[];
  protected useNumericValue: boolean = false;

  deserializeValue(value: any): any {
    return this.useNumericValue ? +value : value;
  }

  ngOnChanges() {
    this.viewConfig = this.viewModel.column.editorOptions;
    this.update();
  }

  ngAfterViewInit() {
    this.setDropDownWidth();
  }

  protected setDropDownWidth() {
    if (this.ddb && this.ddb.instance && this.viewConfig && this.viewConfig.dropDownWidth) {
      this.ddb.instance.option('dropDownOptions.width', this.viewConfig.dropDownWidth);
    }
  }

  protected update() {
    this.setDropDownWidth();
    if (this.viewConfig && this.viewModel) {
      let options = this.viewConfig;
      this.dataSource = options.dataSource;
      this.valueExpr = options.valueExpr;
      this.displayExpr = options.displayExpr;
      this.columns = options.columns;
      this.value = this.deserializeValue(this.viewModel.value);
    }
  }
};
