import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';

@Component({
  selector: 'circle-icon',
  template: `<a (onClick)="handleClick($event)" [attr.isSelected]="isSelected" class="pointer-cursor" 
   [attr.data-testid]="testid" data-toggle="tooltip" data-placement="right" aria-describedby="tooltip"
    title="{{ title }}" href="javacript:void(0)" [routerLink]="routeLink">
     <span class="fa-stack fa-2x">
       <i class="fa fa-square{{ getCircleType() }} fa-stack-2x {{ getCircleColor() }}"></i>
       <i class="fa fa-{{ iconName }} fa-stack-1x {{ getIconColor() }}"></i>
     </span>
   </a>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CircleIcon {
  @Input() testid: string;
  @Input() isSelected: string = 'false';
  @Input() title: string;
  @Input() color: string = 'ruby';
  @Input() circleColor: string;
  @Input() filled: boolean = false;
  @Input() iconName: string;
  @Input() iconColor: string = 'coolgray-20';
  @Input() routeLink: string;
  @Output() onClick = new EventEmitter<any>();

  handleClick(event) {
    this.onClick.emit(event);
  }

  getIconColor(): string {
    return this.isSelected.trim() === 'true' ? 'white' : this.color;
  }

  getCircleColor(): string {
    return this.isSelected.trim() === 'true' ? this.color : this.iconColor;
  }

  getCircleType(): string {
    return (this.isSelected.trim() === 'true' ? '' : '-o');
  }
};
