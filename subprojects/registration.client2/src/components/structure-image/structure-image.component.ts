import {
  Component,
  Input,
  OnInit,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';

const BASE_URL = '/Registration.Server/api';

@Component({
  selector: 'reg-structure-image',
  template: `
   <img [attr.data-testid]="testid" [src]="srcUrl$ | async" class='structure-image' />
  `,
  styles: [require('./structure-image.component.css')],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegStructureImage {
  @Input() testid: string;
  @Input() type: string;
  @Input() id: number;
  @Input() height: number = 100;
  @Input() width: number = 150;
  @Input() resolution: number = 300;
  @Input() src: string;

  private srcUrl$: Observable<string>;

  constructor(private http: Http) {}

  ngOnInit() {
    if (this.src) {
      let srcValues = this.src.split('.');
      this.type = srcValues[0];
      this.id = +srcValues[1];
    }
    let url = `${BASE_URL}/StructureImage/${this.type}/${this.id}/${this.height}/${this.width}/${this.resolution}`;
    this.srcUrl$ = this.http.get(url).map(res => res.text().replace(/\"/g, ''));
  }
};
