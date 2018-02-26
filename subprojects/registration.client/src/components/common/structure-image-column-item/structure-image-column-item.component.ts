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
  private imageId;
  private spinnerImage = require('../assets/spinner.gif');
  
  constructor(private imageService: CStructureImageService, private changeDetector: ChangeDetectorRef) {
    this.image = nonStructureImage;
    this.imageId = '0';
  }

  ngOnInit() {
    let value = this.source;
    if (value && value.Structure != null && value.TEMPBATCHID) {
      this.imageId = `image${value.TEMPBATCHID}`;
      value = value.Structure;
    } else if (value && value.STRUCTUREAGGREGATION != null && value.REGID) {
      this.imageId = `image${value.REGID}`;
      value = value.STRUCTUREAGGREGATION;
    } else if ( value && value.structure != null) {
      value = value.structure;
    }
    if (!value) {
      return;
    }
    let self = this;
    this.image = this.spinnerImage;
    self.changeDetector.markForCheck();
    if (!value) {
      this.image = nonStructureImage;
    } else {
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
  }
};
