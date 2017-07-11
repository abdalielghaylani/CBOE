import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CircleIcon } from './circle-icon.component';
import { CommandButton } from './command-button.component';
import { PageWithTools } from './page-with-tools.component';
import { ToolPanel } from './tool-panel.component';
import { FullScreenIcon } from './full-screen.component';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  declarations: [
    CircleIcon,
    CommandButton,
    PageWithTools,
    ToolPanel,
    FullScreenIcon
  ],
  exports: [
    CircleIcon,
    CommandButton,
    PageWithTools,
    ToolPanel,
    RouterModule,
    FullScreenIcon
  ]
})
export class RegToolModule { }
