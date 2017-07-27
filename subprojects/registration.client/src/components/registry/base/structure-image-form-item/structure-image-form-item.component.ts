import { Component, EventEmitter, Input, Output, ChangeDetectorRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { Http, ResponseContentType, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http';
import { IFormItemTemplate } from '../registry-base.types';
import { RegTagBoxFormItem } from '../tag-box-form-item';
import { NgRedux } from '@angular-redux/store';
import { IAppState } from '../../../../redux';
import { ChemDrawWeb } from '../../../common';

@Component({
  selector: 'reg-structure-image-form-item-template',
  template: require('./structure-image-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureImageFormItem implements IFormItemTemplate, OnChanges {
  @Input() activated: boolean;
  @Input() editMode: boolean;
  @Input() viewModel: any;
  @Input() viewConfig: any;
  @Output() valueUpdated: EventEmitter<any> = new EventEmitter<any>();  
  private image = require('../assets/spinner.gif');

  constructor(private ngRedux: NgRedux<IAppState>, private http: Http, private changeDetector: ChangeDetectorRef) {
  }

  ngOnChanges() {
    this.update();
  }

  deserializeValue(value: any): any {
    return value;
  }

  serializeValue(value: any): any  {
    return value;
  }

  protected update() {
    let value = this.viewModel.editorOptions.value;
    if (!value) {
      return;
    }
    let data: any = {
      chemData: value,
      chemDataType: 'CDXML',
      resolution: '300',
      imageType: 'png'
    };
    let headers = new Headers({
      'Content-Type': 'application/json',
      Authorization: 'Basic VTlueFA3VzM/XlE4bVE6VFF6SzNlMyllNDNlQEE=',
      Accept: 'image/png'
    });
    let options = new RequestOptions({ headers: headers, responseType: ResponseContentType.ArrayBuffer });
    let self = this;
    this.http.post('https://chemdrawdirect.perkinelmer.com/1.5.0/rest/generateImage', data, options)
      .toPromise()
      .then(result => {
        self.image = 'data:image/png;base64,' + btoa(String.fromCharCode.apply(null, new Uint8Array(result.arrayBuffer())));
        self.changeDetector.markForCheck();
      })
      .catch(error => {
      });
  }
};
