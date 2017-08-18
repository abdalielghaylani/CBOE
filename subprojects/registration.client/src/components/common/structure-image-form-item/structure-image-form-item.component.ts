import { Component, EventEmitter, Input, Output, ChangeDetectorRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { Http, ResponseContentType, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http';
import { CStructureImageService } from '../structure-image.service';
import { RegBaseFormItem } from '../base-form-item';

@Component({
  selector: 'reg-structure-image-form-item-template',
  template: require('./structure-image-form-item.component.html'),
  styles: [require('../common.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureImageFormItem extends RegBaseFormItem {
  private image;
  private spinnerImage = require('../assets/spinner.gif');
  private noStructureImage = require('../assets/no-structure.png');

  constructor(private imageService: CStructureImageService, private changeDetector: ChangeDetectorRef) {
    super();
    this.image = this.noStructureImage;
  }

  protected update() {
    let value = this.viewModel.editorOptions.value;
    if (value && value.Structure != null) {
      value = value.Structure.__text;
    }
    if (!value) {
      return;
    }
    let self = this;
    this.image = this.spinnerImage;
    self.changeDetector.markForCheck();
    this.imageService.generateImage(value)
      .then(result => {
        self.image = result;
        self.changeDetector.markForCheck();
      })
      .catch(error => {
        this.image = this.noStructureImage;
        self.changeDetector.markForCheck();
      });
  }
};
