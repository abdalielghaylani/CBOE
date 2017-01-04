import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';

@Component({
  selector: 'circle-icon',
  template: `
   <a [attr.data-testid]="testid" data-toggle="tooltip" data-placement="right" aria-describedby="tooltip"
    title="{{ title }}" href="javacript:void(0)" (onClick)="handleClick($event)">
     <span class="fa-stack fa-2x">
       <i class="fa fa-circle{{ getCircleType() }} fa-stack-2x {{ getCircleColor() }}"></i>
       <i class="fa fa-{{ iconName }} fa-stack-1x {{ getIconColor() }}"></i>
     </span>
   </a>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CircleIcon {
  @Input() testid: string;
  @Input() title: string;
  @Input() color: string = 'ruby';
  @Input() circleColor: string;
  @Input() filled: boolean = false;
  @Input() iconName: string;
  @Input() iconColor: string;
  @Output() onClick = new EventEmitter<any>();

  handleClick(event) {
    this.onClick.emit(event);
  }

  getIconColor(): string {
    return this.iconColor == null ? this.color : this.iconColor;
  }

  getCircleColor(): string {
    return this.circleColor == null ? this.color : this.circleColor;
  }

  getCircleType(): string {
    return this.filled ? '' : '-thin';
  }
};
