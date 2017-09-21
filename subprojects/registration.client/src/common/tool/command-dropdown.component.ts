import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';

@Component({
  selector: 'command-dropdown',
  template: `
  <div class="btn-group btn-group-xs pull-right m1" [title]="title"> 
  <button type="button" class="btn btn-default border-2 status background-blue border-blue">
        <i class="fa fa-{{iconName}} white" aria-hidden="true"></i>
        </button>
        <button type="button" style="width:100px"
        class="btn btn-default border-2 status border-blue blue dropdown-toggle" data-toggle="dropdown" aria-haspopup="true"
        aria-expanded="false">{{selectedText ? selectedText: items[0]}} <span class="caret"></span> <span class="sr-only">Toggle Dropdown</span> </button>
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
  private selectedText: string;
  @Output() onSelected = new EventEmitter<any>();

  getSelected(e) {
    this.selectedText = e;
    this.onSelected.emit(e);
  }
  
};
