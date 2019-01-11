import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';

@Component({
  selector: 'command-dropdown',
  styles: [require('../../app/reg-app.css')],
  template: `
  <div class="btn-group btn-group-xs pull-right m1" [title]="title"> 
     <button type="button" style="width:125px"
        class="btn btn-default border-2 status border-blue blue btndropdown dropdown-toggle" data-toggle="dropdown" aria-haspopup="true"
        aria-expanded="false">
        <div class="left background-blue border-blue btnicon"><i class="fa fa-{{iconName}} white" aria-hidden="true"></i></div>
        <div class="left blue background-white btntext">{{selectedText ? selectedText: items[0]}}</div>
            <span class="caret"></span> 
        </button>
  <ul class="dropdown-menu">
  <li *ngFor="let item of items;">
    <a (click)="getSelected(item)">{{item}}</a>
  </li>
  </ul> </div>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommandDropdown {
  @Input() title: string;
  @Input() color: string = 'blue';
  @Input() size: string = 'xs';
  @Input() iconName: string = 'exclamation';
  @Input() items: string[];
  @Input() selectedText: string;
  @Output() onSelected = new EventEmitter<any>();

  getSelected(e) {
    this.selectedText = e;
    this.onSelected.emit(e);
  }
  
}
