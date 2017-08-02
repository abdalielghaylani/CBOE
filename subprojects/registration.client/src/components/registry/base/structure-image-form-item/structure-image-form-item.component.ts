import { Component, EventEmitter, Input, Output, ChangeDetectorRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { Http, ResponseContentType, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http';
import { RegBaseFormItem } from '../base-form-item';

@Component({
  selector: 'reg-structure-image-form-item-template',
  template: require('./structure-image-form-item.component.html'),
  styles: [require('../registry-base.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureImageFormItem extends RegBaseFormItem {
  private image;
  private spinnerImage = require('../assets/spinner.gif');
  private noStructureImage = require('../assets/no-structure.png');

  constructor(private http: Http, private changeDetector: ChangeDetectorRef) {
    super();
  }

  protected update() {
    this.image = this.noStructureImage;
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
    this.image = this.spinnerImage;
    self.changeDetector.markForCheck();
    this.http.post('https://chemdrawdirect.perkinelmer.com/1.5.0/rest/generateImage', data, options)
      .toPromise()
      .then(result => {
        self.image = 'data:image/png;base64,' + btoa(String.fromCharCode.apply(null, new Uint8Array(result.arrayBuffer())));
        self.changeDetector.markForCheck();
      })
      .catch(error => {
        this.image = this.noStructureImage;
        self.changeDetector.markForCheck();
      });
  }
};
