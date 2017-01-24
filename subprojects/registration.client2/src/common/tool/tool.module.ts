import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import {
  CircleIcon,
  PageWithTools,
  ToolPanel,
  FullScreenIcon
} from './index';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  declarations: [
    CircleIcon,
    PageWithTools,
    ToolPanel,
    FullScreenIcon
  ],
  exports: [
    CircleIcon,
    PageWithTools,
    ToolPanel,
    RouterModule,
    FullScreenIcon
  ]
})
export class ToolModule { }
