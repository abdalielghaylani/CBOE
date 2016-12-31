import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import {
  RegNavigator,
  RegNavigatorHeader,
  RegNavigatorItems,
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
    RegNavigatorItems,
    RegNavigatorItem
  ],
  exports: [
    RegNavigator,
    RegNavigatorHeader,
    RegNavigatorItems,
    RegNavigatorItem
  ]
})
export class RegNavigatorModule { }
