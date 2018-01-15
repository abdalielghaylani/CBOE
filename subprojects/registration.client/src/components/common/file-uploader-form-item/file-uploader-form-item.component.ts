import { Component, EventEmitter, Input, Output, OnChanges, ChangeDetectorRef,  ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { RegBaseFormItem } from '../base-form-item';

@Component({
  selector: 'reg-file-uploader-form-item-template',
  template: require('./file-uploader-form-item.component.html'),
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegFileUploaderFormItem extends RegBaseFormItem {
  protected sdfFiles: any[] = [];

  protected onValueChanged(e) {
    let reader = new FileReader();
    reader.onload = (() => {
      let mols = reader.result.trim(null).split('$$$$\r\n');
      for (let i = 0; i < mols.length; i++) {
        if (mols[i] && mols[i] !== '') {
          let indexMEND = mols[i].indexOf('M  END');
          if (indexMEND > 0) {
            mols[i] = mols[i].substring(0, indexMEND + 6).trim();
          }
          let encoded = new DOMParser().parseFromString(mols[i], 'text/html').documentElement.textContent;
          this.sdfFiles.push(mols[i]);
        }        
      }
      let value = this.serializeValue(this.sdfFiles.join('$$$$\r\n\r\n'));
      this.viewModel.component.option('formData.' + this.viewModel.dataField, value);
      this.onValueUpdated(this);
    }).bind(this);
    reader.readAsText(e.value[0]);
  }
};
