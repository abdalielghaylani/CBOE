import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';

@Component({
  selector: 'command-dropdown-list',
  styles: [require('../../app/reg-app.css')],
  template: `
  <div class="btn-group btn-group-xs pull-right m1" [title]="title"> 
     <button type="button"
        class="btn btn-default border-2 status border-blue blue btndropdown dropdown-toggle" data-toggle="dropdown" aria-haspopup="true"
        aria-expanded="false">
        <div class="left background-blue border-blue btnicon"><i class="fa fa-{{iconName}} white" aria-hidden="true"></i></div>
        <div class="left blue background-white btntext">{{title}}</div>
            <span class="caret"></span> 
        </button>
  <ng-content></ng-content>
 </div>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommandDropdownList {
  @Input() title: string;
  @Input() color: string = 'blue';
  @Input() size: string = 'xs';
  @Input() iconName: string = 'exclamation';
  @Input() items: string[];
  
};
