import { Component, EventEmitter, Input, Output, OnChanges, AfterViewInit, ChangeDetectionStrategy, ViewChild, ViewEncapsulation } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { DxDropDownBoxComponent } from 'devextreme-angular';
import { IAppState } from '../../../../redux';
import { RegBaseFormItem } from '../base-form-item';

@Component({
  selector: 'reg-drop-down-column-item-template',
  template: require('./drop-down-column-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegDropDownColumnItem implements OnChanges, AfterViewInit {
  @ViewChild(DxDropDownBoxComponent) ddb: DxDropDownBoxComponent
  @Input() viewModel: any;
  viewConfig: any;
  value: any;
  protected dataSource: any[];
  protected valueExpr: string;
  protected displayExpr: string;
  protected useNumericValue: boolean = false;

  constructor(private ngRedux: NgRedux<IAppState>) {
    this.update();
  }

  deserializeValue(value: any): any {
    return this.useNumericValue ? +value : value;
  }

  serializeValue(value: any): any {
    return this.useNumericValue ? value.toString() : value;
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
      this.value = this.deserializeValue(this.viewModel.value);
    }
  }
};
