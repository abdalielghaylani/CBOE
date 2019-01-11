import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';

@Component({
  selector: 'full-screen-icon',
  template: `
  <span [id]="id" class="fa-stack fa-2x coolgray-20 hover-coolgray" 
  (onClick)="handleClick($event)" style="cursor: pointer" title="{{ title }}">
  <i class="fa fa-square fa-stack-2x" title="{{ title }}"></i>
  <i class="fa fa-{{iconName}} fa-stack-1x white" aria-hidden="true" data-toggle="tooltip" data-placement="top" title="{{ title }}"></i>
  </span> 
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FullScreenIcon {
  @Input() title: string = 'Full Screen';
  @Input() iconName: string = 'expand';
  @Input() cssClass: string;
  @Input() id: string;
  @Output() onClick = new EventEmitter<any>();

  handleClick(event) {
    this.onClick.emit(event);
  }

}
