import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RegNavigator } from './navigator.component';
import { RegNavigatorHeader } from './navigator-header.component';
import { RegNavigatorItems } from './navigator-items.component';
import { RegNavigatorItem } from './navigator-item.directive';
import { RegSidebar } from './sidebar.component';
import { RegSidebarItem } from './sidebar-item.component';

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
