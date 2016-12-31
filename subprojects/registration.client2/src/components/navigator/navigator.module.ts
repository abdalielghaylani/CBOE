import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import {
  RegNavigator,
  RegNavigatorHeader,
  RegNavigatorItem
} from './index';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  declarations: [
    RegNavigator,
    RegNavigatorHeader,
    RegNavigatorItem
  ],
  exports: [
    RegNavigator,
    RegNavigatorHeader,
    RegNavigatorItem
  ]
})
export class RegNavigatorModule { }
