import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CircleIcon } from './circle-icon.component';
import { CommandButton } from './command-button.component';
import { CommandDropdown } from './command-dropdown.component';
import { PageWithTools } from './page-with-tools.component';
import { ToolPanel } from './tool-panel.component';
import { FullScreenIcon } from './full-screen.component';
import { BadgeCount } from './badge-count.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  declarations: [
    CircleIcon,
    CommandButton,
    CommandDropdown,
    PageWithTools,
    ToolPanel,
    FullScreenIcon,
    BadgeCount
  ],
  exports: [
    CircleIcon,
    CommandButton,
    CommandDropdown,
    PageWithTools,
    ToolPanel,
    RouterModule,
    FullScreenIcon,
    BadgeCount
  ]
})
export class RegToolModule { }
