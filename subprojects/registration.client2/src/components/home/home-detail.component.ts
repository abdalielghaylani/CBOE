import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';
import { ICounter } from '../../store';

@Component({
  selector: 'reg-home-detail',
  styles: [require('./home.css')],
  template: require('./home-detail.component.html'),
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegHomeDetail {
};
