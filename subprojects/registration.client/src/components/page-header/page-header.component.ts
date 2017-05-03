import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'reg-page-header',
  template: `
    <div class="page-header condensed">
    <div class="row padding-sm">
    <div class="col-md-6">
      <h4 
      class="yellow text-relaxed pull-left" 
      [attr.data-testid]="testid" 
      [id]="id">
      <ng-content></ng-content>
      </h4>
    </div>
    <div class="col-md-6" [attr.isCloseButtonVisible]="isCloseButtonVisible" [hidden]="isCloseButtonVisible=='false'">
     <i (click)="handleClick($event)" class="fa fa-2x fa-window-close coolgray-20 text-relaxed pull-right" aria-hidden="true"></i>
    </div>
    </div>
    </div>
  `
})
export class RegPageHeader {
  @Input() testid: string;
  @Input() id: string;
  @Input() isCloseButtonVisible: string = 'false';
  @Output() onClick = new EventEmitter<any>();
  handleClick(event) {
    this.onClick.emit(event);
  }
};
