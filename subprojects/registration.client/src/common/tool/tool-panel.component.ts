import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';
import { CircleIcon } from './circle-icon.component';

@Component({
  selector: 'tool-panel',
  template: `
  <circle-icon>
  </circle-icon>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToolPanel {
  @Input() testid: string;
};
