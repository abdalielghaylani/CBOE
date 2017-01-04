import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import {
  CircleIcon,
  PageWithTools,
  ToolPanel
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
    ToolPanel
  ],
  exports: [
    CircleIcon,
    PageWithTools,
    ToolPanel,
    RouterModule
  ]
})
export class ToolModule { }
