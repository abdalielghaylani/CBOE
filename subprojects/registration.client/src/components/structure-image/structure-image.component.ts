import {
  Component,
  Input,
  OnInit,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { apiUrlPrefix } from '../../configuration';

@Component({
  selector: 'reg-structure-image',
  template: `
   <img src="{{ srcUrl$ | async }}" class='structure-image' />
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
  private modDate: string;

  constructor(private http: Http) { }

  ngOnInit() {
    if (this.src && this.src.indexOf('/') > 0) {
      let typeId = this.src.split('/');
      this.type = typeId[0];
      let idModDate = typeId[1].split('?');
      if (idModDate.length === 1) {
        this.id = +typeId[1];
      } else {
        this.id = +idModDate[0];
        this.modDate = idModDate[1];
      }
    }
    if (this.type && this.id) {
      let url = `${apiUrlPrefix}StructureUrl/${this.type}/${this.id}/${this.height}/${this.width}/${this.resolution}`;
      if (this.modDate) {
        url += `?${this.modDate}`;
      }
      this.srcUrl$ = this.http.get(url).map(res => res.text().replace(/\"/g, ''));
    }
  }
};
