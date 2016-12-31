import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import {
  RegNavigator,
  RegNavigatorItem
} from './index';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  declarations: [
    RegNavigator,
    RegNavigatorItem
  ],
  exports: [
    RegNavigator,
    RegNavigatorItem
  ]
})
export class RegNavigatorModule { }
