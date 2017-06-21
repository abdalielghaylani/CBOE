import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import {
  CircleIcon,
  CommandButton,
  PageWithTools,
  ToolPanel,
  FullScreenIcon,
  ChemDrawingTool
} from './index';

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
    FullScreenIcon,
    ChemDrawingTool
  ],
  exports: [
    CircleIcon,
    CommandButton,
    PageWithTools,
    ToolPanel,
    RouterModule,
    FullScreenIcon,
    ChemDrawingTool
  ]
})
export class ToolModule { }
