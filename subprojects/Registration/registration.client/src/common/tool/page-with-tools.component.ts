import {
  Component,
  Input,
  Output,
  EventEmitter,
  ChangeDetectionStrategy,
} from '@angular/core';
import {
  RouterModule
} from '@angular/router';
import { ToolPanel } from './tool-panel.component';

@Component({
  selector: 'page-with-tools',
  template: `
  <tool-panel>
    <ng-content></ng-content>
  </tool-panel>
  <router-outlet></router-outlet>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PageWithTools {
  @Input() testid: string;
}
