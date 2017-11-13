import { Component, EventEmitter, Input, Output, ChangeDetectorRef, OnChanges, ChangeDetectionStrategy, ViewEncapsulation } from '@angular/core';
import { Http, ResponseContentType, RequestOptions, Request, RequestOptionsArgs, Response, Headers } from '@angular/http';
import { CStructureImageService } from '../structure-image.service';

export const nonStructureImage = require('../assets/no-structure.png');

@Component({
  selector: 'reg-structure-image-column-item-template',
  template: require('./structure-image-column-item.component.html'),
  styles: [require('../common.css')],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegStructureImageColumnItem {
  @Input() source: any;
  private image;
  private spinnerImage = require('../assets/spinner.gif');
  
  constructor(private imageService: CStructureImageService, private changeDetector: ChangeDetectorRef) {
    this.image = nonStructureImage;
  }

  ngOnInit() {
    let value = this.source;
    if (value && value.Structure != null) {
      value = value.Structure;
    } else if (value && value.STRUCTUREAGGREGATION != null) {
      value = value.STRUCTUREAGGREGATION;
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
        this.image = nonStructureImage;
        self.changeDetector.markForCheck();
      });
  }
};
