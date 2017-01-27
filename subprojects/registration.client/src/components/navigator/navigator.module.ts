import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import {
  RegNavigator,
  RegNavigatorHeader,
  RegNavigatorItems,
  RegNavigatorItem,
  RegSidebar,
  RegSidebarItem
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
    RegNavigatorItem,
    RegSidebar,
    RegSidebarItem
  ],
  exports: [
    RegNavigator,
    RegNavigatorHeader,
    RegNavigatorItems,
    RegNavigatorItem,
    RegSidebar,
    RegSidebarItem
  ]
})
export class RegNavigatorModule { }
