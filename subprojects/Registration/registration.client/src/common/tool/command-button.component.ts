import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';

@Component({
  selector: 'command-button',
  template: require('./command-button.component.html'),
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommandButton {
  @Input() testid: string;
  @Input() title: string;
  @Input() color: string = 'blue';
  @Input() size: string = 'xs';
  @Input() iconName: string = 'exclamation';
  @Input() pullRight: boolean = true;
  @Output() onClick = new EventEmitter<any>();

  handleClick(event) {
    this.onClick.emit(event);
  }
}
