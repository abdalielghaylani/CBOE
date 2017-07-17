import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { RegToolModule } from './tool/tool.module';

export { RegToolModule };
export * from './types';
export * from './utils';

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    RegToolModule
  ],
  declarations: [
  ],
  exports: [
    RegToolModule
  ]
})
export class RegCommonModule { }
