import {
  Component,
  Input,
  OnInit,
  EventEmitter,
  ChangeDetectionStrategy,
  ChangeDetectorRef
} from '@angular/core';
import { Observable } from 'rxjs';
import { apiUrlPrefix } from '../../../configuration';
import { HttpService } from '../../../services';
import { CStructureImageService } from '../structure-image.service';

@Component({
  selector: 'reg-structure-image-grid-item',
  template: `<img [src]="image" class="chemdraw-image block mx-auto" />
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegStructureImageGridItem {
  private image;
  private spinnerImage = require('../assets/spinner.gif');
  private noStructureImage = require('../assets/no-structure.png');

  @Input() cdXml: string;

  constructor(private http: HttpService, private imageService: CStructureImageService, private changeDetector: ChangeDetectorRef) {
    this.image = this.spinnerImage;
  }

  ngOnInit() {
    let value = this.cdXml;
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
}
