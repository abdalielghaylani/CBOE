import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';

@Component({
  selector: 'badge-count',
  template: `
  <div class="count {{color}}">
  <div class="count-type">{{text}}</div>
  <div class="count-type-amount border-{{color}} round border-2 padding-xs">
    {{count}}
    <i class="fa fa-{{icon}}" aria-hidden="true"></i>
  </div>
 </div>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BadgeCount {
  @Input() color: string = 'blue';
  @Input() icon: string = 'book';
  @Input() text: string = '';
  @Input() count: string = '';
}
